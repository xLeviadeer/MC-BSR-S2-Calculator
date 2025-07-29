using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    class SwitchManagedBorder : Border, ISwitchManaged {

        // --- VARIABLES ---

        public bool TabContentsChanged { get; set; } = false;

        public bool RequiresReset { get; set; } = false;

        // --- METHODS ---

        public void Reset() { }
    }
}
