//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace divire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Mutex to prevent more than one processes running the same application.
        /// </summary>
        private static Mutex mutex;

        /// <summary>
        /// Startup event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var isFirst = false;

            // Allow a process to run the application unless it share one DLL.
            var location = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var mutexName = location.FullName.Replace(@"\", string.Empty) + "divire";
            mutex = new Mutex(true, mutexName, out isFirst);

            if (!isFirst)
            {
                MessageBox.Show("The application is already running.", @"divire", MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
            }
        }
    }
}
