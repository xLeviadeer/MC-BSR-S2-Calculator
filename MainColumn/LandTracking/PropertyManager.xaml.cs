using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking
{
    /// <summary>
    /// Interaction logic for PropertyManager.xaml
    /// </summary>
    public partial class PropertyManager : UserControl
    {
        // --- VARIABLES ---
        #region VARIABLES

        // - Validity Holder -

        private class ValidityHolder {
            public bool IsValid { get; set; } = false;
            public bool IsEnabled { get; set; } = true;
        }

        private Dictionary<string, ValidityHolder> Validity { get; set; } = new() {
            [nameof(NameInput)] = new(),
            [nameof(PropertyTypeInput)] = new(),
            [nameof(ResidentsCountInput)] = new() { IsEnabled = false }
        };

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public PropertyManager()
        {
            InitializeComponent();
        }

        #endregion

        // --- EVENT METHODS ---

        // -- Completion Buttons --
        #region Completion Buttons

        private void OnClearClicked(object? sender, EventArgs args) {
            // temporarily dont update buttons (so we dont waste time validating on every change)
            DoUpdateButtons = false;

            NameInput.Text = "";
            NameInput.IsValid = null;

            PropertyTypeInput.SelectedIndex = -1;

            ResidentsCountInput.Text = "";
            ResidentsCountInput.IsValid = null;

            // allow updates again and manually update buttons
            DoUpdateButtons = true;
            UpdateCreateButtonEnabledStatus();
        }

        private void OnCreateClicked(object? sender, EventArgs args) {
            // create a new property and add it to the property list

            // clear values
            OnClearClicked(this, args);

            // tab the user to the view/modify page
        }

        #endregion

        // -- Grid Methods --
        #region Grid Methods

        private void OnKeyDown(object? sender, KeyEventArgs args) {
            if (args.Key == Key.Enter) {
                if (CreateButton.IsEnabled) {
                    CreateButton.MarkAsPressed(this, args);
                }
            } else if (args.Key == Key.Escape) {
                ClearButton.MarkAsPressed(this, args);
            } else {
                if (CreateButton.IsChargeCycling) {
                    args.Handled = true;
                }
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs args) {
            if (args.Key == Key.Enter) {
                CreateButton.MarkAsUnpressed(this, args);
            } else if (args.Key == Key.Escape) {
                ClearButton.MarkAsUnpressed(this, args);
            }
        }

        private void OnTextInput(object sender, TextCompositionEventArgs args) {
            if (CreateButton.IsChargeCycling) {
                args.Handled = true;
            }
        }

        private void OnMouseDown(object? sender, MouseButtonEventArgs args) {
            MainGrid.Focus();
        }

        #endregion

        // -- Per-Item Methods --
        #region Per-Item Methods

        // - per item helper -

        private bool DoUpdateButtons { get; set; } = true;

        private void UpdateCreateButtonEnabledStatus() {
            // doesn't run if doupdatebuttons false
            if (DoUpdateButtons) {
                // updates buttons
                if (Validity.Values.All(validityHolder => {
                    if (validityHolder.IsEnabled) {
                        return validityHolder.IsValid;
                    } else {
                        return true;
                    }
                })) {
                    CreateButton.IsEnabled = true;
                } else {
                    CreateButton.IsEnabled = false;
                    CreateButton.MarkAsUnpressed(this, EventArgs.Empty);
                }
            }
        }

        // - per item -

        private void NameInput_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(NameInput)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        private void PropertyTypeInput_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            var propertyTypeInput = (ComboLabel)sender;

            // update the validity for the residents count if it's not collapsed
            if (Property.PlayerPropertyTypes[propertyTypeInput.SelectedIndex] == Property.SharedPrivate) {
                ResidentsCountInput.Visibility = Visibility.Visible;
                Validity[nameof(ResidentsCountInput)].IsEnabled = true;
            } else {
                ResidentsCountInput.Visibility = Visibility.Collapsed;
                Validity[nameof(ResidentsCountInput)].IsEnabled = false;
            }

            // set validity for this
            Validity[nameof(PropertyTypeInput)].IsValid = (propertyTypeInput.SelectedIndex != -1);
            UpdateCreateButtonEnabledStatus();
        }

        private void ResidentsCountInput_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(ResidentsCountInput)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        #endregion
    }
}
