using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {
    public class EnterTextBox : SwitchManagedTextBox {

        // --- VARIABLES ---

        // -- Interface --
        #region Interface

        // - Tab Highlighting -

        public bool HighlightUponTab {
            get => (bool)GetValue(HighlightUponTabProperty);
            set => SetValue(HighlightUponTabProperty, value);
        }

        public static readonly DependencyProperty HighlightUponTabProperty = DependencyProperty.Register(
            nameof(HighlightUponTab),
            typeof(bool),
            typeof(EnterTextBox),
            new PropertyMetadata(false)
        );

        // - Click Highlighting -

        public bool HighlightUponClick {
            get => (bool)GetValue(HighlightUponClickProperty);
            set => SetValue(HighlightUponClickProperty, value);
        }

        public static readonly DependencyProperty HighlightUponClickProperty = DependencyProperty.Register(
            nameof(HighlightUponClick),
            typeof(bool),
            typeof(EnterTextBox),
            new PropertyMetadata(false)
        );

        #endregion

        // -- Events --
        #region Events

        public event EventHandler<KeyEventArgs>? KeyDownEnter;

        public event EventHandler<KeyEventArgs>? KeyUpEnter;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public EnterTextBox() {
            // enter down
            KeyDown += (object sender, KeyEventArgs args) => {
                if (args.Key == Key.Enter) {
                    KeyDownEnter?.Invoke(this, args);
                }
            };

            // enter up
            KeyUp += (object sender, KeyEventArgs args) => {
                if (args.Key == Key.Enter) {
                    KeyUpEnter?.Invoke(this, args);
                }
            };

            // tab highlighting
            GotKeyboardFocus += (sender, args) => {
                if (
                    (HighlightUponTab) 
                    && (sender is EnterTextBox enterTextBox)
                ) {
                    enterTextBox.SelectAll();
                }
            };

            // click highlighting
            PreviewMouseLeftButtonDown += (sender, args) => {
                if (
                    (HighlightUponClick)
                    && (sender is EnterTextBox enterTextBox)
                ) {
                    args.Handled = true; // stops default focus behavior
                    enterTextBox.Focus(); // for mouse, must focus before selecting all
                    enterTextBox.SelectAll();
                }
            };
        }

        #endregion
    }
}
