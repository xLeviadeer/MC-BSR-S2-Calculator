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
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.TextBoxes;

namespace MC_BSR_S2_Calculator.PlayerColumn {
    /// <summary>
    /// Interaction logic for PlayerColumn.xaml
    /// </summary>
    public partial class PlayerColumn : UserControl {
        
        // --- VARIABLES ---

        // - player list reference

        private PlayerList PlayersList { get => (PlayerList) this.FindName("_playersList"); }

        // --- CONSTRUCTORS ---
        
        public PlayerColumn() {
            InitializeComponent();
        }

        // --- METHODS ---

        // - add player button -

        public void OnClickAddPlayerButton(object sender, RoutedEventArgs args) {
            // check if player name already used
            string nameTrimmed = PlayerInputBox.Text.Trim();
            if (!PlayersList.NameAlreadyExists(nameTrimmed)) {
                // add player
                PlayersList.ClassDataList.Add(new(nameTrimmed));
                PlayerInputBox.Text = "";
                PlayersList.AsIStorable.Save();
                PlayersList.BuildGrid();
            } else {
                PlayersList.ShowPlayerNameTaken(nameTrimmed);
            }
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

        // - mark all as inactive -
        public void OnMarkAllAsInactiveButtonClicked(object sender, RoutedEventArgs args) {
            foreach (var player in PlayersList.ClassDataList) {
                player.WasActive.Value = false;
            }
        }
    }
}
