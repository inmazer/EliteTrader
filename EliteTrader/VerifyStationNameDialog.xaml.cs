using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace EliteTrader
{
    /// <summary>
    /// Interaction logic for VerifyStationNameDialog.xaml
    /// </summary>
    public partial class VerifyStationNameDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OkCommand { get; private set; }

        public VerifyStationNameDialog(Window parent, string stationName)
        {
            InitializeComponent();

            Owner = parent;
            DataContext = this;
            StationName = stationName;

            OkCommand = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(OkCommand, OkExecute, OkCanExecute));
        }

        private void OkCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(StationName))
            {
                e.CanExecute = true;
            }
        }

        private void OkExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private string _stationName;
        public string StationName
        {
            get { return _stationName; }
            set
            {
                if (_stationName == value)
                {
                    return;
                }
                _stationName = value;
                RaisePropertyChanged("StationName");
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
