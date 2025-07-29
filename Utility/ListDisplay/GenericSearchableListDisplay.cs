using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {
    public class GenericSearchableListDisplay<T> : SearchableListDisplay<T>
        where T : Displayable {
        protected override void SetClassDataList() {
            // do nothing    
        }

        public GenericSearchableListDisplay() : base() { }

        public GenericSearchableListDisplay(SearchableListDisplay<T> cls) : base(cls) { }
    }
}
