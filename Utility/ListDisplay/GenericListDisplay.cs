using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {
    public class GenericListDisplay<T> : ListDisplay<T> 
        where T : Displayable {
        protected override void SetClassDataList() {
            // do nothing    
        }

        public GenericListDisplay() : base() { }

        public GenericListDisplay(ListDisplay<T> cls) : base(cls) { }
    }
}
