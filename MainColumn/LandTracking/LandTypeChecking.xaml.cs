using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [BlocksSwitchManagement]
    public partial class LandTypeChecking : UserControl {

        // --- VARIABLES ---

        // --- CONSTRUCTORS ---

        public LandTypeChecking() {
            InitializeComponent();
        }

        // --- METHODS ---

        private void _updateResults() {

        }

        private void _useYCoordinateCheckChanged(object sender, Utility.BoolEventArgs args) {
            // set Y coordinate
            Coordinates.ShowYInput = args.Value ?? false;
        }

        private void _coordinatesCoordinatesChanged(object sender, EventArgs e) {

        }
    }
}
