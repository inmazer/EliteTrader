using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Capture;
using Capture.Hook;
using Capture.Interface;

namespace EliteTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static CaptureProcess _captureProcess;

        public static readonly string DateTimeUiFormat = CultureInfo.CurrentUICulture.DateTimeFormat.SortableDateTimePattern;

        public ObservableCollection<LogEntry> LogEntries { get; private set; }

        public ICommand AttachToElite { get; private set; }
        public ICommand DetachFromElite { get; private set; }
        public ICommand CaptureScreen { get; private set; }
        public ICommand ShowSettings { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            AttachToElite = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(AttachToElite, AttachToEliteExecute, AttachToEliteExecuteCanExecute));

            DetachFromElite = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(DetachFromElite, DetachFromEliteExecute, DetachFromEliteExecuteCanExecute));

            CaptureScreen = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(CaptureScreen, CaptureScreenExecute, CaptureScreenExecuteCanExecute));

            ShowSettings = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(ShowSettings, ShowSettingsExecute, ShowSettingsExecuteCanExecute));

            LogEntries = new ObservableCollection<LogEntry>();

            HotKey attachHotkey = new HotKey(Key.A, KeyModifier.Alt | KeyModifier.Ctrl, OnAttachHotkeyHandler);
            HotKey captureScreenHotkey = new HotKey(Key.S, KeyModifier.Alt | KeyModifier.Ctrl, OnCaptureHotkeyHandler);
        }

        private void ShowSettingsExecute(object sender, ExecutedRoutedEventArgs e)
        {

            e.Handled = true;
        }

        private void ShowSettingsExecuteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void OnAttachHotkeyHandler(HotKey hotKey)
        {
            AttachToEliteExecuteInternal();
        }

        private void AttachToEliteExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AttachToEliteExecuteInternal();
            
            e.Handled = true;
        }

        private void AttachToEliteExecuteInternal()
        {
            if (_captureProcess != null)
            {
                Log(MessageType.Error, "Already attached!");
                return;
            }

            Process process = Process.GetProcessesByName("EliteDangerous32").FirstOrDefault();

            if (process == null)
            {
                Log(MessageType.Error, "EliteDangeroud32 process not found");
                return;
            }

            CaptureConfig cc = new CaptureConfig
            {
                Direct3DVersion = Direct3DVersion.Direct3D11,
                ShowOverlay = false
            };

            CaptureInterface captureInterface = new CaptureInterface();
            captureInterface.RemoteMessage += CaptureInterface_RemoteMessage;
            _captureProcess = new CaptureProcess(process, cc, captureInterface);

            Log(MessageType.Information, "Successfully attached to Elite process");
        }

        private void AttachToEliteExecuteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _captureProcess == null;
            e.Handled = true;
        }

        private void DetachFromEliteExecute(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (_captureProcess == null)
            {
                Log(MessageType.Error, "Unable to detach from Elite, not attached");
                return;
            }

            HookManager.RemoveHookedProcess(_captureProcess.Process.Id);
            _captureProcess.CaptureInterface.Disconnect();
            _captureProcess = null;

            Log(MessageType.Information, "Successfully detached from the Elite process");
        }
        
        private void DetachFromEliteExecuteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _captureProcess != null;
            e.Handled = true;
        }

        private void OnCaptureHotkeyHandler(HotKey hotKey)
        {
            CaptureScreenExecuteInternal();
        }

        private void CaptureScreenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CaptureScreenExecuteInternal();
            e.Handled = true;
        }

        private void CaptureScreenExecuteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _captureProcess != null;
            e.Handled = true;
        }

        private void CaptureScreenExecuteInternal()
        {
            if (_captureProcess == null)
            {
                Log(MessageType.Error, "Unable to take capture screen, not attached");
                return;
            }

            _captureProcess.CaptureInterface.BeginGetScreenshot(new TimeSpan(0, 0, 2), Callback);

            Log(MessageType.Debug, "Screen capture initiated");
        }

        /// <summary>
        /// Display messages from the target process
        /// </summary>
        /// <param name="message"></param>
        private void CaptureInterface_RemoteMessage(MessageReceivedEventArgs message)
        {
            LogThroughDispatcher(message.MessageType, message.Message);
        }

        private void LogThroughDispatcher(MessageType severity, string message)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Log(severity, message);
            });
        }

        private void Log(MessageType severity, string message)
        {
            LogEntries.Add(new LogEntry(DateTime.Now, severity, message));
        }

        private void Callback(IAsyncResult result)
        {
            if (result == null)
            {
                throw new Exception(string.Format("Result was null"));
            }
            using (Screenshot screenshot = _captureProcess.CaptureInterface.EndGetScreenshot(result))
            {
                try
                {
                    if (screenshot == null)
                    {
                        LogThroughDispatcher(MessageType.Error, "Screenshot was null");
                        return;
                    }
                    if (screenshot.CapturedBitmap == null)
                    {
                        LogThroughDispatcher(MessageType.Error, "Screenshot.CapturedBitmap was null");
                        return;
                    }

                    byte[] bytes = screenshot.CapturedBitmap;
                    File.WriteAllBytes(GetNextFilenameBmp(@"c:\tmp\screens"), bytes);

                    LogThroughDispatcher(MessageType.Information, "Screen capture successful");

                    Bitmap bitmap = new Bitmap(new MemoryStream(bytes));
                }
                catch (Exception e)
                {
                    LogThroughDispatcher(MessageType.Error, e.ToString());
                }
                catch
                {
                    LogThroughDispatcher(MessageType.Error, "Something undefined but bad happened");
                }
            }
        }

        private static int _fileNameCounterBmp = 1;
        private static string GetNextFilenameBmp(string path)
        {
            while (File.Exists(Path.Combine(path, string.Format("{0}.bmp", _fileNameCounterBmp))))
            {
                ++_fileNameCounterBmp;
            }

            return Path.Combine(path, string.Format("{0}.bmp", _fileNameCounterBmp));
        }

        //private void AddRandomEntry()
        //{
        //    Dispatcher.BeginInvoke((Action) (() => _logEntries.Add(GetRandomEntry())));
        //}

        //private LogEntry GetRandomEntry()
        //{
        //    if (random.Next(1,10) > 1)
        //    {
        //        return new LogEntry()
        //        {
        //            Index = index++,
        //            DateTime = DateTime.Now,
        //            Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
        //                                                 .Select(x => words[random.Next(0, maxword)])),
        //        };
        //    }

        //    return new CollapsibleLogEntry()
        //               {
        //                   Index = index++,
        //                   DateTime = DateTime.Now,
        //                   Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
        //                                                .Select(x => words[random.Next(0, maxword)])),
        //                   Contents = Enumerable.Range(5, random.Next(5, 10))
        //                                        .Select(i => GetRandomEntry())
        //                                        .ToList()
        //               };

        //}
    }
}
