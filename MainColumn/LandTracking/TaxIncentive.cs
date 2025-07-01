using MC_BSR_S2_Calculator.Utility.DisplayList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class TaxIncentive : Incentive {

        // --- VARIABLES ---

        public override event EventHandler<EventArgs>? RemoveRequested;

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS 

        protected override void SetDefaultValues(string name, double value) {
            // set name and value
            base.SetDefaultValues(name, value);

            // remove button
            var button = new Button();
            button.Content = "Remove";
            button.Click += (_, args) => RemoveRequested?.Invoke(this, args);
            button.IsTabStop = false;
            RemoveButton = new(button);
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public TaxIncentive() => SetDefaultValues(string.Empty, 0.0d);

        public TaxIncentive(
            string name,
            double value
        ) => SetDefaultValues(name, value);
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        #endregion

        // --- METHODS ---

        public override ActiveIncentive GetActiveIncentive() => new() {
            InfoTarget = IncentiveInfo.Tax.Instance,
            Name = Name
        };
    }
}
