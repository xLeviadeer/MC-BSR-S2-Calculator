using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn {
    public static class MainResources {

        // --- VARIABLES ---

        // - Properties Display -

        public static PropertyList PropertiesDisplay { get; } = new();

        // - Players Display -

        public static PlayerList PlayersDisplay { get; } = new();

        // - Government Player -

        public static Player GovernmentPlayer { get; private set; }

        public const string GOVERNMENT_IDENTIFIER = "10p0000000001";

        // --- CONSTRUCTOR ---

        static MainResources() {
            // adds government player if they dont exist
            PlayersDisplay.ClassDataListLoaded += (_, _) => {
                // check if already exists
                foreach (Player player in PlayersDisplay) {
                    if (player.DisplayableID.Identifier == GOVERNMENT_IDENTIFIER) {
                        GovernmentPlayer = player;
                        player.AssignInstanceID(player);
                        PlayersDisplay.IDBlockList.Add(MainResources.GovernmentPlayer.DisplayableID);
                        return;
                    }
                }

                // couldn't find existing government id
                throw new InvalidOperationException("Government Player not found. Add Governemnt user with ID p,'0' to continue launch");
            };
        }

        // --- METHODS ---
        #region METHODS 

        public static void RemoveParent(FrameworkElement element) {
            // if panel
            if (element.Parent is Panel panel) {
                panel.Children.Remove(element);

            // if content control
            } else if (element.Parent is ContentControl contentControl) {
                if (contentControl.Content == element) {
                    contentControl.Content = null;
                }

            // if decorator
            } else if (element.Parent is Decorator decorator) {
                if (decorator.Child == element) {
                    decorator.Child = null;
                }

            // if items control
            } else if (element.Parent is ItemsControl itemsControl) {
                itemsControl.Items.Remove(element);
            }

            // none, do nothing
        }

        #endregion

    }
}
