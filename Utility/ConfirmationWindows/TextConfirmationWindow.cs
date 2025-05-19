using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using static MC_BSR_S2_Calculator.Utility.TextLabelAbove;

namespace MC_BSR_S2_Calculator.Utility.ConfirmationWindows
{
    public class TextConfirmationWindow : ConfirmationWindow {

        // --- VARIABLES ---

        // - contained text box -

        public TextBox TextBoxInput { get; set; } = new();

        // --- CONSTRUCTORS ---

        private void AddTextBoxToGrid(TextBoxTypes textBoxType, int textBoxMaxLength) {
            // create text box of specified type
            TextBoxInput = textBoxType switch {
                TextBoxTypes.TextBox => new TextBox(),
                TextBoxTypes.IntegerTextBox => new IntegerTextBox(),
                TextBoxTypes.DoubleTextBox => new DoubleTextBox(),
                TextBoxTypes.StringTextBox => new StringTextBox(),
                _ => new TextBox()
            };

            // add row
            OuterGrid.RowDefinitions.Insert(1, new RowDefinition() {
                Height = GridLength.Auto
            });

            // shift children down
            foreach (UIElement child in OuterGrid.Children) {
                int row = Grid.GetRow(child);
                if (row >= 2) {
                    Grid.SetRow(child, row + 1);
                }
            }
            
            // text box settings
            TextBoxInput.Height = (double)Application.Current.Resources["TextBoxHeight"];
            TextBoxInput.HorizontalAlignment = HorizontalAlignment.Stretch;
            TextBoxInput.VerticalAlignment = VerticalAlignment.Center;
            TextBoxInput.HorizontalContentAlignment = HorizontalAlignment.Left;
            TextBoxInput.VerticalContentAlignment = VerticalAlignment.Center;
            TextBoxInput.Margin = new Thickness(5, 0, 3, 5);
            TextBoxInput.MaxLength = textBoxMaxLength;

            // enter event
            TextBoxInput.KeyDown += (sender, args) => {
                if (args.Key == Key.Enter) {
                    if (ConfirmButton.IsEnabled) {
                        OnConfirm(this, args);
                    }
                }
            };

            // add text box
            OuterGrid.Children.Add(TextBoxInput);
            Grid.SetRow(TextBoxInput, 1);
            Grid.SetColumnSpan(TextBoxInput, 4);
        }

        public TextConfirmationWindow() 
            : base() {
            AddTextBoxToGrid(TextBoxTypes.TextBox, 0);
        }

        public TextConfirmationWindow(
            string titleText = "Are you sure?",
            string confirmButtonText = "Yes",
            string denyButtontext = "No",
            string descriptionText = "",
            bool useConfirmColor = false,
            TextBoxTypes textBoxType = TextBoxTypes.TextBox,
            int textMaxLength = 0,
            bool startHighlighted = true
        ) : base(
            titleText,
            confirmButtonText,
            denyButtontext, 
            descriptionText,
            useConfirmColor
        ) {
            AddTextBoxToGrid(textBoxType, textMaxLength);

            // focus user on textbox
            Loaded += (s, e) => {
                TextBoxInput.Focus();
                TextBoxInput.CaretIndex = TextBoxInput.Text.Length;
                if (startHighlighted) { TextBoxInput.SelectAll(); }
            };
        }
    }
}
