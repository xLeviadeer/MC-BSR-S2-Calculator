using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {
    public abstract class IDListDisplay<T> : ListDisplay<T>
        where T : IDDisplayable {

        // --- CONSTRUCTORS ---

        protected override void ForAllLoadedRowsAndNewItems(T instance) {
            instance.AssignInstanceID(instance);
        }
    }
}
