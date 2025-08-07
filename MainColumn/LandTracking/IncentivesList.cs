using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public abstract class IncentivesList : ListDisplay<Incentive>, ISwitchManaged {

        // --- VARIABLES ---

        public int DefaultCount { get; set; } = 0;

        public virtual bool TabContentsChanged => Count != DefaultCount;

        public bool RequiresReset { get; set; } = true;

        public abstract event EventHandler<EventArgs>? Updated;

        // --- CONSTRUCTOR ---

        protected sealed override void SetClassDataList() {
            ClassDataList = new(); // empty
        }

        // --- METHODS ---

        public virtual void Reset()
            => Clear();
    }
}
