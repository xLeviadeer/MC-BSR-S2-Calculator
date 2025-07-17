using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class TaxIncentivesList : IncentivesList {

        public override event EventHandler<EventArgs>? Updated;

        public TaxIncentivesList() =>
             Rebuilt += (_, args) => Updated?.Invoke(this, args);
    }
}
