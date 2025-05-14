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
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.DisplayList;

namespace MC_BSR_S2_Calculator.PlayerColumn {
    /// <summary>
    /// Interaction logic for PlayerColumn.xaml
    /// </summary>
    public partial class PlayerColumn : UserControl {
        
        // --- CONSTRUCTORS ---
        
        public PlayerColumn() {
            InitializeComponent();
        }

        // --- METHODS ---

        // - add player button -

        public void OnClickAddPlayerButton(object sender, RoutedEventArgs args) {
            PlayerList.ClassDataList.Add(new(PlayerInputBox.Text));
            PlayerList.BuildGrid();
        }

        // - validity changed function -

        public void OnPlayerInputBoxValidityChanged(object? sender, BoolEventArgs args) {
            if (args.Value == null) {
                AddPlayerButton.IsEnabled = false;
            } else {
                AddPlayerButton.IsEnabled = (bool) args.Value;
            }
        }

        // - text enter function -
        public void OnPlayerInputBoxKeyDownEnter(object sender, KeyEventArgs args) {
            if (AddPlayerButton.IsEnabled) {
                OnClickAddPlayerButton(this, new());
            }
        }
    }
}
