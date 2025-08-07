using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class PropertyClickableList : SearchableListDisplay<PropertyClickable>, ISwitchManaged {

        // --- VARIABLES ---
        #region VARIABLES

        // - Switch Management -

        public bool TabContentsChanged { get; } // not used

        public bool RequiresReset { get; set; } = false;

        public void Reset() { } // not used

        #endregion

        // --- CONSTRUCTORS ---

        protected override void SetClassDataList() {
            // do nothing    
        }

        public PropertyClickableList() : base() { }

        public PropertyClickableList(PropertyClickableList cls) : base(cls) { }

        // --- METHODS ---

        protected override void SortClassData() {
            ClassDataList = NotifyingList<PropertyClickable>.From(
                ClassDataList.OrderBy(cls => cls.Name.Value)
            );
        }
    }
}
