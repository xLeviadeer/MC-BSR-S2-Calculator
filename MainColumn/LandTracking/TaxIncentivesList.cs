using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class TaxIncentivesList : IncentiveList {
        // --- VARIABLES ---

        public override event EventHandler<EventArgs>? Updated;
        
        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR 

        protected override void ForAllLoadedRowsAndNewItems(Incentive instance) {
            instance.RemoveRequested += (_, __) => ClassDataList.Remove(instance);
        }

        protected override void SetClassDataList() {
            ClassDataList = new(); // empty
        }

        public TaxIncentivesList() => 
            Rebuilt += (_, args) => Updated?.Invoke(this, args);

        #endregion
    }
}
