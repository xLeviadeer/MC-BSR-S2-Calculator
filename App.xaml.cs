using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace MC_BSR_S2_Calculator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            // set up logging
            Logging.Initialize();

            var test = new Land();
            test.Bounds.Add(new CoordinateCube(new(0, 0, 0), new(10, 10, 10)));
            if (test.Bounds[0] is CoordinateCube cube) {
                Debug.WriteLine(cube);
                Debug.WriteLine(cube.NW.AsTop());
                Debug.WriteLine(cube.NW.AsBottom());
            }
        }
    }
}
