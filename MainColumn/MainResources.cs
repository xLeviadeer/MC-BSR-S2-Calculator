using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // - Lands List -

        public static LandList LandsList { get; private set; } = new();

        // - Landarea -

        public static List<ILandArea> LandAreasList {
            get {
                List<ILandArea> lst = new();
                lst.AddRange(LandsList.All);
                lst.AddRange(PropertiesDisplay.ClassDataList);
                return lst;
            }
        }

        // - Players Display -

        public static PlayerList PlayersDisplay { get; } = new();

        // - Government Player -

        public static Player GovernmentPlayer { get; private set; }

        public const string GOVERNMENT_IDENTIFIER = "10p0000000001";

        public const string GOVERNMENT_PLAYER_NAME = "Government";

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

                // check if government player exits but not at 0
                void checkGovernmentPlayerID() {
                    if (PlayersDisplay.Any(player => player.Name == GOVERNMENT_PLAYER_NAME)) {
                        throw new InvalidOperationException("Government Player found with incorrect ID. Ensure Governemnt user ID is at p...1 to continue launch");
                    }
                }
                checkGovernmentPlayerID();

                // add government player
                GovernmentPlayer = new Player(GOVERNMENT_PLAYER_NAME);
                GovernmentPlayer.DisplayableID.AssignNewID(GovernmentPlayer);
                PlayersDisplay.Add(GovernmentPlayer);
                PlayersDisplay.AsIStorable.Save();
                checkGovernmentPlayerID();
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
