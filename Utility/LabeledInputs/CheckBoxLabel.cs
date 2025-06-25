using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {
    public class CheckBoxLabelProperties : LabeledInputProperties<CheckBox>;

    public class CheckBoxLabel : LabeledInput<CheckBox> {
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
        public event RoutedEventHandler? Unchecked;
        public event RoutedEventHandler? Indeterminate;
        public event EventHandler<BoolEventArgs>? CheckChanged;

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

            // make it so that clicking anywhere on this object changes the ischecked state
            MainGrid.IsHitTestVisible = true;
            MainGrid.Background = new SolidColorBrush(Colors.Transparent);
            MainGrid.MouseLeftButtonUp += (_, _) => {
                Element.IsChecked = !(Element.IsChecked ?? false);
            };

            // expose Checked
            Element.Checked += (_, args) => {
                Checked?.Invoke(this, args);
                CheckChanged?.Invoke(this, new(Element.IsChecked));
            };
            Element.Unchecked += (_, args) => {
                Unchecked?.Invoke(this, args);
                CheckChanged?.Invoke(this, new(Element.IsChecked));
            };
            Element.Indeterminate += (_, args) => {
                Indeterminate?.Invoke(this, args);
                CheckChanged?.Invoke(this, new(Element.IsChecked));
            };

            // apply layout mode
            ApplyLayoutMode();
        }

        #endregion
    }
}
