using MC_BSR_S2_Calculator.MainColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.ComponentModel;

namespace MC_BSR_S2_Calculator.PlayerColumn {
    /// <summary>
    /// Interaction logic for PlayerColumn.xaml
    /// </summary>
    public partial class PlayerColumn : UserControl {

        // --- VARIABLES ---

        // - player list reference -

        private PlayerList PlayersList => MainResources.PlayersDisplay;

        // --- CONSTRUCTORS ---
        
        public PlayerColumn() {
            InitializeComponent();

            // set up PlayerList
            Loaded += (_, _) => {
                ViewGrid.Children.Add(PlayersList);
                Grid.SetRow(PlayersList, 1);
                PlayersList.ScrollBarWidth = 10;
                PlayersList.ShowScrollBar = ScrollBarVisibility.Auto;
                PlayersList.Margin = new Thickness(5, 3, 5, 10);
                PlayersList.EmptyText = "No Players found";
                PlayersList.ItemBorderBrushSides = new SolidColorBrush(ColorResources.ItemBorderBrushSidesLighter);
                PlayersList.AnyWasActiveChanged += PlayersList_AnyWasActiveChanged;
                PlayersList.CompletedLoading += (_, _) => UpdateMarkAllAsInactiveButtonIsEnabled();
            };
        }

        // --- METHODS ---

        // - add player button -

        public void OnChargedAddPlayerButton(object sender, EventArgs args) {
            // check if player name already used
            string nameTrimmed = PlayerInputBox.Text.Trim();
            if (!PlayersList.NameAlreadyExists(nameTrimmed)) {
                // add player
                PlayersList.ClassDataList.Add(new(nameTrimmed));

                // erase text
                PlayerInputBox.Text = "";
                PlayerInputBox.IsValid = null;

                // save and rebuild grid
                PlayersList.AsIStorable.Save();
                PlayersList.BuildGrid();
                UpdateMarkAllAsInactiveButtonIsEnabled();
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
                AddPlayerButton.MarkAsPressed(this, args);
            }
        }

        public void OnPlayerInputBoxKeyUpEnter(object sender, KeyEventArgs args) {
            AddPlayerButton.MarkAsUnpressed(this, args);
        }

        // blocks pasting while charging
        public void OnPlayerInputBoxTextInput(object sender, TextCompositionEventArgs args) {
            if (AddPlayerButton.IsChargeCycling) {
                args.Handled = true;
            }
        }

        // blocks typing while charging
        public void OnPlayerInputTextBoxKeyPressed(object sender, KeyEventArgs args) {
            if (AddPlayerButton.IsChargeCycling && args.Key != Key.Enter) {
                args.Handled = true;
            }
        }

        // focus helper
        private void OnMouseDown(object? sender, MouseButtonEventArgs args) {
            PlayerInputBox.Element.Focus();
        }

        // - mark all as inactive -
        public void OnMarkAllAsInactiveButtonClicked(object sender, RoutedEventArgs args) {
            foreach (var player in PlayersList.ClassDataList) {
                player.WasActive.Value = false;
            }
            MarkAllAsInactiveButton.IsEnabled = false;
        }

        private void UpdateMarkAllAsInactiveButtonIsEnabled() 
            => MarkAllAsInactiveButton.IsEnabled = PlayersList.AnyWasActiveTrue;

        private void PlayersList_AnyWasActiveChanged(object? sender, BoolEventArgs args)
            => UpdateMarkAllAsInactiveButtonIsEnabled();
    }
}
