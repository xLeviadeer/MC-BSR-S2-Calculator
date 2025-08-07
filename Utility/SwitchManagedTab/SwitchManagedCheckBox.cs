using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedCheckBox : CheckBox, ISwitchManagedDefault {

        // --- VARIABLES ---

        public virtual object? DefaultValue { get; set; } = false;

        public bool TabContentsChanged => IsChecked != (bool?)DefaultValue;

        public bool RequiresReset { get; set; } = true;

        // --- METHODS ---

        public void Reset() {
            IsChecked = (bool?)DefaultValue;
        }
    }
}
