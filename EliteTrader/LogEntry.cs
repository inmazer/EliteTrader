using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using EliteTrader.EliteOcr.Logging;

namespace EliteTrader
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class LogEntry : PropertyChangedBase
    {
        public DateTime DateTime { get; set; }

        public EnumMessageType Severity { get; set; }

        public string Message { get; set; }

        protected LogEntry()
        {

        }

        public LogEntry(DateTime dateTime, EnumMessageType severity, string message)
        {
            DateTime = dateTime;
            Severity = severity;
            Message = message;
        }
    }

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }

    public class Logger : ILogger
    {
        private readonly ICollection<LogEntry> _logEntries;
        private bool Enabled { get; set; }

        public Logger(ICollection<LogEntry> logEntries, bool enabled = true)
        {
            _logEntries = logEntries;
            Enabled = enabled;
        }

        public void Log(string message, EnumMessageType messageType = EnumMessageType.Info)
        {
            if (!Enabled)
            {
                return;
            }


            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher != null)
            {
                _logEntries.Add(new LogEntry(DateTime.Now, messageType, message));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _logEntries.Add(new LogEntry(DateTime.Now, messageType, message));
                });
            }
        }
    }
}