using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class PurchaseIncentivesList : IncentivesList {

        public override event EventHandler<EventArgs>? Updated;

        public PurchaseIncentivesList() =>
             Rebuilt += (_, args) => Updated?.Invoke(this, args);
    }
}
