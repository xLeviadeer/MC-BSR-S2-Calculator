using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class ViolationIncentive : Incentive {

        // --- VARIABLES ---

        // - Violation Type -

        public enum ViolationTypes {
            Once,
            Recur
        }

        [DisplayValue("Type", 40, GridUnitType.Pixel, HorizontalAlignment.Center, VerticalAlignment.Center, displayOrder: 1)]
        public BoundDisplayValue<Label, ViolationTypes> ViolationType { get; set; }

        // - Violation Count -

        [DisplayValue("#", 30, GridUnitType.Pixel, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, displayOrder:1, isHitTestVisible: true)]
        public BoundDisplayValue<IntegerTextBox, int> ViolationCount { get; set; }

        public event EventHandler<EventArgs>? ViolationCountChanged;

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
                        ViolationCountChanged?.Invoke(this, args);
                    }
                }
            };
            ViolationCount = new(
                violationInput,
                IntegerTextBox.TextProperty,
                minimum
            );

            // violation type
            ViolationType = new(
                new Label(),
                Label.ContentProperty,
                (name switch {
                    IncentiveInfo.Violation.AntiInflation => ViolationTypes.Once,
                    IncentiveInfo.Violation.GroundedStructures => ViolationTypes.Once,
                    IncentiveInfo.Violation.LandAntiMutilation => ViolationTypes.Once,
                    IncentiveInfo.Violation.InvalidEdgeSpacing => ViolationTypes.Recur,
                    IncentiveInfo.Violation.InvalidSignage => ViolationTypes.Recur,
                    _ => ViolationTypes.Once
                })
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

        // --- METHODS ---

        public override ActiveIncentive GetActiveIncentive() => new ActiveViolationIncentive() {
            InfoTarget = IncentiveInfo.Violation.Instance,
            Name = Name,
            ViolationType = ViolationType,
            ViolationCount = (int)((IntegerTextBox)ViolationCount.DisplayObject).Value
        };
    }
}
