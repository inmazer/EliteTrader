using System;
using System.Windows;

namespace EliteTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs eventArgs)
        {
            try
            {
                ResourceFilesCopier.SetupFiles();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Shutdown();
            }
            
        }
    }
}
