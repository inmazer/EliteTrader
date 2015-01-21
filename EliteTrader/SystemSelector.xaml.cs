using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ThruddClient;

namespace EliteTrader
{
    /// <summary>
    /// Interaction logic for SystemSelector.xaml
    /// </summary>
    public partial class SystemSelector : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<AdminSearchResultItem> Systems { get; private set; }

        public ICommand OkCommand { get; private set; }

        public SystemSelector(Window parent, List<AdminSearchResultItem> systems)
        {
            InitializeComponent();

            Owner = parent;
            DataContext = this;
            Systems = systems;

            OkCommand = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(OkCommand, OkExecute, OkCanExecute));
        }

        private int? _selectedSystemId;
        public int? SelectedSystemId
        {
            get { return _selectedSystemId; }
            set
            {
                if (_selectedSystemId == value)
                {
                    return;
                }
                _selectedSystemId = value;
                RaisePropertyChanged("SelectedSystemId");
            }
        }

        private void OkCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedSystemId.HasValue)
            {
                e.CanExecute = true;
            }
        }

        private void OkExecute(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static int? Show(Window parent, List<AdminSearchResultItem> systems)
        {
            if (systems.Count == 0)
            {
                return null;
            }

            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher != null)
            {
                SystemSelector systemSelector = new SystemSelector(parent, systems);
                bool? result = systemSelector.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    return null;
                }
                return systemSelector.SelectedSystemId;
            }
            else
            {
                return Application.Current.Dispatcher.Invoke(delegate
                {
                    SystemSelector systemSelector = new SystemSelector(parent, systems);
                    bool? result = systemSelector.ShowDialog();
                    if (!result.HasValue || !result.Value)
                    {
                        return null;
                    }
                    return systemSelector.SelectedSystemId;
                });
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
