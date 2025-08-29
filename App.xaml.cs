using MC_BSR_S2_Calculator.MainColumn;
using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Alphaleonis.Win32.Filesystem;
using System.Windows;
using System.Linq;

namespace MC_BSR_S2_Calculator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App() {
            // set up logging
            Logging.Initialize();

            // ID setup
            RelationalID.InitializeIDTypes();
        }

        private static readonly string[] BACKUPS_PATH = ["Backups"];
        private static readonly string[] INDEX_PATH = BACKUPS_PATH.Concat(["Index.json"]).ToArray();

        private void _application_Exit(object sender, ExitEventArgs e) {

            // -- back up data files --

            // - read/create index -
            int index = 0;

            // create index storage file if it doesn't exist
            if (!File.Exists(JsonHandler.CreatePath(INDEX_PATH))) {
                JsonHandler.Write<List<int>>(INDEX_PATH, [0]);
            } else {
                // read index
                index = JsonHandler.Read<List<int>>(INDEX_PATH)[0];
            }

            // update index
            index += 1;

            // - copy files -
            Directory.Copy(
                JsonHandler.FULL_DATA_PATH, // source
                JsonHandler.CreatePath( // destination
                    BACKUPS_PATH
                    .Concat([index.ToString()])
                    .ToArray()
                )
            );

            // - write index -
            JsonHandler.Write<List<int>>(INDEX_PATH, [index]);
        }
    }
}
