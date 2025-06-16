using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Reflection;
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
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking
{
    /// <summary>
    /// Interaction logic for PropertyManager.xaml
    /// </summary>
    public partial class PropertyManager : UserControl, IValidityHolder
    {
        // --- VARIABLES ---
        #region VARIABLES

        // - Validity Holder -

        private ValidityHolder Validity = new(
            new () {
                [nameof(NameInput)] = new(),
                [nameof(PropertyTypeInput)] = new(),
                [nameof(ResidentsCountInput)] = new() { IsEnabled = false }, // starts as disabled
                [nameof(Sections)] = new() { IsValid = true } // sections start as valid
            }
        );

        // - Validity Changed Event -

        private bool IsValid { get; set; }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public PropertyManager()
        {
            InitializeComponent();

            // assign Sections and deletion events
            Loaded += (_, __) => {
                if (!HasBeenLoaded) {
                    ResetSections();
                    HasBeenLoaded = true;
                }

                // try to add margin to scroll bar
                var scrollBar = XamlConverter.FindVerticalScrollBar(MainScrollViewer);
                if (scrollBar != null) {
                    scrollBar.Margin = new Thickness(3, 0, 0, 0);
                }
            };
        }

        #endregion

        // --- EVENT METHODS ---

        // - Is Valid Exposure - 

        public bool CheckValidity() => Validity.CheckValidity();

        // -- Completion Buttons --
        #region Completion Buttons

        private void OnClearClicked(object? sender, EventArgs args) {
            // temporarily dont update buttons (so we dont waste time validating on every change)
            DoUpdateButtons = false;

            NameInput.Text = "";
            NameInput.IsValid = null;

            PropertyTypeInput.SelectedIndex = -1;

            ResidentsCountInput.Text = "0";
            ResidentsCountInput.IsValid = null;

            ResetSections();

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

        // -- Per-Item Group Methods --
        #region Per-Item Group Methods

        // - per item helper -

        private bool DoUpdateButtons { get; set; } = true;

        private void UpdateCreateButtonEnabledStatus() {
            // doesn't run if doupdatebuttons false
            if (DoUpdateButtons) {
                // updates buttons
                if (Validity.CheckValidity()) {
                    CreateButton.IsEnabled = true;
                } else {
                    CreateButton.IsEnabled = false;
                    CreateButton.MarkAsUnpressed(this, EventArgs.Empty);
                }

                // send validity changed event
                IsValid = CreateButton.IsEnabled; // mirror enabled state
                ValidityChanged?.Invoke(this, new(IsValid));
            }
        }

        #endregion

        // -- Per-Item Methods --
        #region Per-Item Methods

        // - per item -

        private void NameInput_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(NameInput)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        private void PropertyTypeInput_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            var propertyTypeInput = (ComboLabel)sender;

            void MakeVisible() {
                ResidentsCountInput.Visibility = Visibility.Visible;
                Validity[nameof(ResidentsCountInput)].IsEnabled = true;
            }

            void MakeInvisible() {
                ResidentsCountInput.Visibility = Visibility.Collapsed;
                Validity[nameof(ResidentsCountInput)].IsEnabled = false;
            }

            // update to invisible if -1
            if (propertyTypeInput.SelectedIndex == -1) {
                MakeInvisible();

            // update the validity for the residents count if it's not collapsed
            } else if (Property.PlayerPropertyTypes[propertyTypeInput.SelectedIndex] == Property.SharedPrivate) {
                MakeVisible();
            } else {
                MakeInvisible();
            }

            // set validity for this
            Validity[nameof(PropertyTypeInput)].IsValid = (propertyTypeInput.SelectedIndex != -1);
            CheckAndSetSectionsValidity();
            UpdateCreateButtonEnabledStatus();
        }

        private void ResidentsCountInput_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(ResidentsCountInput)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        #endregion
    }
}
