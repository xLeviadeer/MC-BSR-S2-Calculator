using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class PurchaseIncentive : Incentive {

        // --- VARIABLES ---

        public override event EventHandler<EventArgs>? RemoveRequested;

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS 

        protected override void SetDefaultValues(string name, double value) {
            // set name and value
            base.SetDefaultValues(name, value);

            // remove button
            var button = new Button();
            button.Content = "Revoke";
            button.Click += (_, args) => RemoveRequested?.Invoke(this, args);
            button.IsTabStop = false;
            RemoveButton = new(button);
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public PurchaseIncentive() => SetDefaultValues(string.Empty, 0.0d);

        public PurchaseIncentive(
            string name,
            double value
        ) => SetDefaultValues(name, value);
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        #endregion

        // --- METHODS ---

        public override ActiveIncentive GetActiveIncentive() => new() {
            InfoTarget = IncentiveInfo.Purchase.Instance,
            Name = Name
        };
    }
}
