using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {
    public class CheckBoxLabel : LabeledInputBase<CheckBox> {
        // --- VARIABLES ---
        #region VARIABLES

        // - Check Box -

        public override CheckBox Element { get; set; } = new();

        // - Has Been Loaded -
        private bool HasBeenLoaded { get; set; } = false;

        // -- Exposures --

        // - IsChecked -

        public bool? IsChecked {
            get => Element.IsChecked;
            set => Element.IsChecked = value;
        }

        // - Checked -

        public event RoutedEventHandler? Checked;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public CheckBoxLabel() {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, EventArgs args) {
            // don't run if already loaded
            if (HasBeenLoaded) { return; }
            HasBeenLoaded = true;

            // check box settings
            Element.Margin = new Thickness(3);
            Element.FontSize = 11;

            // expose Checked
            Element.Checked += (_, args) => Checked?.Invoke(this, args);

            // apply layout mode
            ApplyLayoutMode();
        }

        #endregion
    }
}
