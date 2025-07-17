using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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
    public partial class LandTracking : UserControl {

        // --- VARIABLES ---

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        // --- CONSTRUCTOR ---

        public LandTracking() {
            InitializeComponent();

            // load the properties display singleton reference into the grid
            Loaded += (_, _) => {
                // properties display settings
                MainResources.RemoveParent(MainResources.PropertiesDisplay);
                ModifyViewPropertyGrid.Children.Add(MainResources.PropertiesDisplay);
                Grid.SetColumn(MainResources.PropertiesDisplay, 0);
                Grid.SetRow(MainResources.PropertiesDisplay, 0);

                // only load once
                if (HasBeenLoaded) { return; }
                HasBeenLoaded = true;
            };
        }

        private void PropertyManager_CreateCompleted(object sender, EventArgs e) {
            // tab to create/modify page
            MainTabControl.SelectedItem = TabModifyViewProperty;
        }
    }
}
