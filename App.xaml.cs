using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MC_BSR_S2_Calculator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            // set up logging
            Logging.Initialize();
        }
    }
}
