using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    class ViolationIncentivesList : IncentivesList {

        public override event EventHandler<EventArgs>? Updated;

        protected override void ForAllLoadedRowsAndNewItems(Incentive instance) {
            ViolationIncentive incentive = (ViolationIncentive)instance;
            incentive.ViolationCountChanged += (sender, args) => {
                BuildGrid(); // ensures instances are rebuilt and up to date
                Updated?.Invoke(sender, args);
            };
        }

        public ViolationIncentivesList() =>
             Rebuilt += (_, args) => Updated?.Invoke(this, args);
    }
}
