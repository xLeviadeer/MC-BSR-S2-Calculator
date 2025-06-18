using MC_BSR_S2_Calculator.Utility.DisplayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public abstract class IncentiveList : ListDisplay<Incentive> {
        public abstract event EventHandler<EventArgs>? Updated;
    }
}
