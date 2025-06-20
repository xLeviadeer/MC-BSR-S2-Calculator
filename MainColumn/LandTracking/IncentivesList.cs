using MC_BSR_S2_Calculator.Utility.DisplayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public abstract class IncentivesList : ListDisplay<Incentive> {

        // --- VARIABLES ---

        public abstract event EventHandler<EventArgs>? Updated;

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR 

        protected sealed override void SetClassDataList() {
            ClassDataList = new(); // empty
        }

        #endregion
    }
}
