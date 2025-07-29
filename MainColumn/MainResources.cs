using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.PlayerColumn;
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
