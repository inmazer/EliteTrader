using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using EliteTrader.EliteOcr.Logging;
using EliteTrader.ProgressReporting;
using Microsoft.WindowsAPICodePack.Dialogs;
using ThruddClient;

namespace EliteTrader
{
    [Serializable]
    public class EliteTraderSettings : INotifyPropertyChanged
    {
        [XmlIgnore]
        public ICommand TestConnection { get; private set; }

        [XmlIgnore]
        public ICommand SelectFolder { get; private set; }

        private static string _configFilePath;
        private static string ConfigFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_configFilePath))
                {
                    _configFilePath = Path.Combine(ResourceFilesCopier.AppDataPath, "Settings.xml");
                }
                return _configFilePath;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ScreenshotPathChanged;

        private ILogger _logger;

        public EliteTraderSettings()
        {
            TestConnection = new RoutedCommand();
            SelectFolder = new RoutedCommand();
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                {
                    return;
                }
                _username = value;
                RaisePropertyChanged("UsernameBoundProperty");
            }
        }

        private string _passwordEncrypted;
        public string PasswordEncrypted
        {
            get { return _passwordEncrypted; }
            set
            {
                if (_passwordEncrypted == value)
                {
                    return;
                }
                _passwordEncrypted = value;
                RaisePropertyChanged("PasswordBoundProperty");
            }
        }

        private string _screenshotFolder;
        public string ScreenshotFolder
        {
            get { return _screenshotFolder; }
            set
            {
                if (_screenshotFolder == value)
                {
                    return;
                }
                _screenshotFolder = value;
                RaisePropertyChanged("ScreenshotFolderBoundProperty");
                if (ScreenshotPathChanged != null)
                {
                    ScreenshotPathChanged(this, new EventArgs());
                }
            }
        }

        [XmlIgnore]
        public string UsernameBoundProperty
        {
            get { return Username; }
            set
            {
                Username = value;
                Save();
            }
        }

        [XmlIgnore]
        public string PasswordBoundProperty
        {
            get { return PasswordEncrypter.Decrypt(PasswordEncrypted); }
            set
            {
                PasswordEncrypted = PasswordEncrypter.Encrypt(value);
                Save();
            }
        }

        [XmlIgnore]
        public string ScreenshotFolderBoundProperty
        {
            get { return ScreenshotFolder; }
            set
            {
                ScreenshotFolder = value;
                Save();
            }
        }

        public ThruddCredentials Credentials
        {
            get
            {
                return new ThruddCredentials(Username, PasswordBoundProperty);
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

        public static EliteTraderSettings Load(Window window, ILogger logger)
        {
            EliteTraderSettings settings;

            if (!File.Exists(ConfigFilePath))
            {
                settings = new EliteTraderSettings();
            }
            else
            {
                using (FileStream stream = File.OpenRead(ConfigFilePath))
                {
                    settings = (EliteTraderSettings)new XmlSerializer(typeof(EliteTraderSettings)).Deserialize(stream);
                }
            }

            settings._logger = logger;

            window.CommandBindings.Add(new CommandBinding(settings.TestConnection, settings.TestConnectionExecute, settings.TestConnectionCanExecute));
            window.CommandBindings.Add(new CommandBinding(settings.SelectFolder, settings.SelectFolderExecute, settings.SelectFolderCanExecute));

            if (string.IsNullOrEmpty(settings.ScreenshotFolderBoundProperty))
            {
                string myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (!string.IsNullOrEmpty(myPictures))
                {
                    settings.ScreenshotFolderBoundProperty = Path.Combine(myPictures, @"Frontier Developments\Elite Dangerous");
                }
            }

            return settings;
        }

        public void Save()
        {
            using (FileStream stream = new FileStream(ConfigFilePath, FileMode.Create))
            {
                new XmlSerializer(typeof(EliteTraderSettings)).Serialize(stream, this);
            }
        }

        private void TestConnectionCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameBoundProperty) && !string.IsNullOrEmpty(PasswordBoundProperty))
            {
                e.CanExecute = true;
            }
        }

        private void TestConnectionExecute(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            eventArgs.Handled = true;

            ProgressDialogResult dialogResult = ProgressDialog.Execute((Window)sender, "Testing connection", () =>
            {
                Client client = new Client(_logger);
                try
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(0, "Logging in");
                    client.Login(Credentials);
                }
                finally
                {
                    ProgressDialog.Current.ReportWithCancellationCheck(90, "Logging out");
                    client.Logout();
                }

            }, new ProgressDialogSettings(true, false, true));

            if (dialogResult.OperationFailed)
            {
                MessageBox.Show(dialogResult.Error.ToString());
                return;
            }

            MessageBox.Show("Success");
        }

        private void SelectFolderCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SelectFolderExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            if (!string.IsNullOrEmpty(ScreenshotFolderBoundProperty))
            {
                dialog.InitialDirectory = ScreenshotFolderBoundProperty;
            }
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string folder = dialog.FileName;

                ScreenshotFolderBoundProperty = folder;
            }
        }
    }
}