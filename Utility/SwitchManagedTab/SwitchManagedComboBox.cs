using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedComboBox : ComboBox {

        // --- VARIABLES ---

        public virtual object DefaultValue { get; set; } = -1;

        public bool TabContentsChanged => SelectedIndex != (int)DefaultValue;

        public bool RequiresReset { get; set; } = true;

        // --- METHODS ---

        public void Reset() {
            SelectedIndex = (int)DefaultValue;
        }
    }
}
