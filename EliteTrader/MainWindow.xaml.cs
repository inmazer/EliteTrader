using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using EliteTrader.EliteOcr;
using EliteTrader.EliteOcr.Data;
using EliteTrader.EliteOcr.Interfaces;
using EliteTrader.ProgressReporting;
using ThruddClient;

namespace EliteTrader
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly string DateTimeUiFormat = CultureInfo.CurrentUICulture.DateTimeFormat.SortableDateTimePattern;

        private readonly ICommodityNameRepository _commodityNameRepository;

        private ObservableCollection<string> _files;
        private string _selectedFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LogEntry> LogEntries { get; private set; }

        public ICommand Import { get; private set; }
        public ICommand DeleteScreenshots { get; private set; }
        
        public EliteTraderSettings Settings { get; private set; }

        public Logger Logger { get; private set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                _commodityNameRepository = new CommodityItemNameMatcher();

                LogEntries = new ObservableCollection<LogEntry>();
                Logger = new Logger(LogEntries);

                Settings = EliteTraderSettings.Load(this, Logger);

                SettingsTabItem.DataContext = Settings;
                DataContext = this;

                Import = new RoutedCommand();
                CommandBindings.Add(new CommandBinding(Import, ImportExecute, ImportCanExecute));

                DeleteScreenshots = new RoutedCommand();
                CommandBindings.Add(new CommandBinding(DeleteScreenshots, DeleteScreenshotsExecute, DeleteScreenshotsCanExecute));

                UpdateFilesList();
                UpdateFolderWatcher();

                Settings.ScreenshotPathChanged += HandleScreenshotPathChanged;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Close();
            }
        }

        private void DeleteScreenshotsExecute(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> selectedPaths = GetSelectedPaths();
            if (selectedPaths.Count == 0)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(string.Format("Delete ({0}) screenshots. Are you sure?", selectedPaths.Count),
                "Delete screenshots", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            
            foreach (string path in selectedPaths)
            {
                File.Delete(path);
            }
            UpdateFilesList();
        }

        private void DeleteScreenshotsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            List<string> selectedPaths = GetSelectedPaths();
            if (selectedPaths.Count > 0)
            {
                e.CanExecute = true;
            }
        }

        private void HandleScreenshotPathChanged(object sender, EventArgs e)
        {
            UpdateFolderWatcher();
            UpdateFilesList();
        }

        private FileSystemWatcher _watcher;
        private void UpdateFolderWatcher()
        {
            if (_watcher == null)
            {
                _watcher = new FileSystemWatcher
                {
                    NotifyFilter = NotifyFilters.FileName
                };

                _watcher.Changed += ScreenshotFolderChanged;
                _watcher.Created += ScreenshotFolderChanged;
                _watcher.Deleted += ScreenshotFolderChanged;
                _watcher.Renamed += ScreenshotFolderChanged;
            }
            else
            {
                _watcher.EnableRaisingEvents = false;
            }

            if (!Directory.Exists(Settings.ScreenshotFolder))
            {
                return;
            }

            _watcher.Path = Settings.ScreenshotFolder;
            _watcher.EnableRaisingEvents = true;
        }

        private void ScreenshotFolderChanged(object sender, FileSystemEventArgs e)
        {
            UpdateFilesList();
        }

        private void UpdateFilesList()
        {
            Files = new ObservableCollection<string>();

            if (!Directory.Exists(Settings.ScreenshotFolder))
            {
                return;
            }

            List<string> allFiles = new List<string>();
            foreach (string fileFilter in new[] { "*.bmp", "*.png" })
            {
                allFiles.AddRange(Directory.GetFiles(Settings.ScreenshotFolder, fileFilter, SearchOption.TopDirectoryOnly));
            }

            Files = new ObservableCollection<string>(allFiles.Select(Path.GetFileName));
        }

        private void ImportCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(SelectedFile);
        }

        private ParsedScreenshot GetParsedScreenshot(List<string> selectedPaths)
        {
            ProgressDialogResult dialogResult = ProgressDialog.Execute(this, "Running OCR on screenshot(s)", () =>
            {
                ScreenshotParser parser = new ScreenshotParser(ResourceFilesCopier.AppDataPath);
                ParsedScreenshot p = parser.ParseMultiple(selectedPaths);

                return p;

            }, new ProgressDialogSettings(false, false, true));

            if (dialogResult.OperationFailed)
            {
                MessageBox.Show(dialogResult.Error.ToString());

                MessageBoxResult shouldUpload = MessageBox.Show("OCR Failed. Do you want to upload the image(s) to the developer as a bug report?",
                    "OCR Failed", MessageBoxButton.YesNo);
                if (shouldUpload == MessageBoxResult.Yes)
                {
                    try
                    {
                        MegaClient.Upload(Settings, selectedPaths, dialogResult.Error);
                        MessageBox.Show("Successfully uploaded image(s) to the developer");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }

                return null;
            }

            ParsedScreenshot parsedScreenshot = (ParsedScreenshot) dialogResult.Result;

            VerifyStationNameDialog dialog = new VerifyStationNameDialog(this, parsedScreenshot.StationName);
            bool? result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return null;
            }

            parsedScreenshot.UpdateStationName(dialog.StationName);

            return parsedScreenshot;
        }

        private void ImportExecute(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                ImportExecuteInternal();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ImportExecuteInternal()
        {
            if (string.IsNullOrEmpty(SelectedFile))
            {
                MessageBox.Show("Please select a screenshot");
                return;
            }

            List<string> selectedPaths = GetSelectedPaths();

            ParsedScreenshot parsedScreenshot = GetParsedScreenshot(selectedPaths);
            if (parsedScreenshot == null)
            {
                return;
            }

            StationSearchResult stationSearchResult = GetStationSearchResult(parsedScreenshot);
            if (stationSearchResult == null)
            {
                return;
            }

            // Go to next wizard page
            VerifyDataPage verifyDataPage = new VerifyDataPage(_commodityNameRepository, parsedScreenshot, stationSearchResult, Settings.Credentials, Logger);
            verifyDataPage.ShowDialog();

            if (verifyDataPage.Success)
            {
                MessageBoxResult result = MessageBox.Show("Successfully imported data. Delete the involved screenshots?", Title, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (string path in selectedPaths)
                    {
                        File.Delete(path);
                    }
                    UpdateFilesList();
                }
            }
        }

        private StationSearchResult GetStationSearchResult(ParsedScreenshot parsedScreenshot)
        {
            ProgressDialogResult dialogResult = ProgressDialog.Execute(this, "Getting station information from Thrudd's website", () =>
            {
                Client client = new Client(Logger);
                try
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(0, "Logging in");
                    client.Login(Settings.Credentials);

                    ProgressDialog.Current.ReportWithCancellationCheck(20, "Searching for station ({0})", parsedScreenshot.StationName);
                    List<AdminSearchResultItem> searchResult = client.DoAdminSearchQuery(parsedScreenshot.StationName);

                    List<AdminSearchResultItem> stationResults = searchResult.Where(a => string.Compare(parsedScreenshot.StationName, a.Station, StringComparison.InvariantCultureIgnoreCase) == 0).ToList();

                    if (stationResults.Count == 0)
                    {
                        MessageBox.Show(
                            string.Format(
                                "Failed to find the station ({0}) on Thrudd's website. Please register the station on the website and try again", parsedScreenshot.StationName));
                        //TODO Add support for registering this station from the application
                        return null;
                    }

                    AdminSearchResultItem stationResult;
                    if (stationResults.Count == 1)
                    {
                        stationResult = stationResults[0];
                    }
                    else
                    {
                        int? systemId = SystemSelector.Show(this, stationResults);
                        if (!systemId.HasValue)
                        {
                            return null;
                        }
                        stationResult = stationResults.Single(a => a.SystemId == systemId);
                    }

                    ProgressDialog.Current.ReportWithCancellationCheck(40, "Receiving station commodities");
                    int stationId = stationResult.StationId;
                    StationCommoditiesResult commodities = client.GetStationCommodities(stationId);
                    List<StationCommoditiesData> stationCommodities = new List<StationCommoditiesData>();
                    if (commodities != null)
                    {
                        stationCommodities = commodities.StationCommodities;
                    }
                    return new StationSearchResult(stationCommodities, stationResult);
                }
                finally
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(90, "Logging out");
                    client.Logout();
                }

            }, new ProgressDialogSettings(true, false, false));

            if (dialogResult.OperationFailed)
            {
                throw new Exception("Exception during Thrudd website operations", dialogResult.Error);
            }

            StationSearchResult stationSearchResult = (StationSearchResult)dialogResult.Result;

            return stationSearchResult;
        }

        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                if (_files == value)
                {
                    return;
                }
                _files = value;
                RaisePropertyChanged("Files");
            }
        }

        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (_selectedFile == value)
                {
                    return;
                }
                _selectedFile = value;
                RaisePropertyChanged("SelectedFile");
                RaisePropertyChanged("SelectedImage");
            }
        }

        private List<string> GetSelectedPaths()
        {
            List<string> paths = new List<string>();
            foreach (string fileName in FileList.SelectedItems)
            {
                paths.Add(Path.Combine(Settings.ScreenshotFolder, fileName));
            }
            return paths;
        }

        public BitmapImage SelectedImage
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedFile))
                {
                    return null;
                }

                BitmapImage image = new BitmapImage();
                using (FileStream stream = new FileStream(Path.Combine(Settings.ScreenshotFolder, _selectedFile), FileMode.Open, FileAccess.Read))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                return image;
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
