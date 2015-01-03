using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Capture.Interface;

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

        public MessageType Severity { get; set; }

        public string Message { get; set; }

        protected LogEntry()
        {

        }

        public LogEntry(DateTime dateTime, MessageType severity, string message)
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

    
}