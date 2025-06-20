using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class ViolationIncentive : Incentive {

        // --- VARIABLES ---

        // - Violation Count -

        [DisplayValue("Violations", 50, GridUnitType.Pixel, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, displayOrder: 1, isHitTestVisible: true)]
        public BoundDisplayValue<IntegerTextBox, int> ViolationCount { get; set; }

        public event EventHandler<EventArgs>? ViolationCountChanged;

        // - Add Value -

        // multiplies the value by the amount of violations
        public override double AddValue {
            get => ViolationCount.Value * Value;
        }

        // - Remove Request -

        public override event EventHandler<EventArgs>? RemoveRequested;

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS 

        protected override void SetDefaultValues(string name, double value) {
            // set name and value
            base.SetDefaultValues(name, value);

            // violation count
            const int minimum = 1;
            var violationInput = new IntegerTextBox();
            violationInput.MaxInput = 1000;
            violationInput.MinInput = minimum;
            violationInput.MaxLength = 50;
            violationInput.DefaultValue = minimum;
            violationInput.HorizontalContentAlignment = HorizontalAlignment.Center;
            violationInput.VerticalContentAlignment = VerticalAlignment.Center;
            violationInput.HighlightUponClick = true;
            violationInput.HighlightUponTab = true;
            violationInput.InputFinalized += (sender, args) => {
                if (args is InputFinalizedEventArgs<double> inputFinalizedArgs) {
                    if (inputFinalizedArgs.OldValue != inputFinalizedArgs.NewValue) {
                        ViolationCountChanged?.Invoke(sender, args);
                    }
                }
            };
            ViolationCount = new(
                violationInput,
                IntegerTextBox.TextProperty,
                minimum
            );

            // remove button
            var button = new Button();
            button.Content = "Mark Paid";
            button.Click += (_, args) => RemoveRequested?.Invoke(this, args);
            button.IsTabStop = false;
            RemoveButton = new(button);
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ViolationIncentive() => SetDefaultValues(string.Empty, 0.0d);

        public ViolationIncentive(
            string name,
            double value
        ) => SetDefaultValues(name, value);
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        #endregion
    }
}
