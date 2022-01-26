using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Tsp.Ui
{
    //todo implement DI 
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //todo add logging

            MessageBox.Show($"Unexpected error happened, see {Environment.CurrentDirectory}/logs for more information.");
            e.Handled = true;
        }
    }
}
