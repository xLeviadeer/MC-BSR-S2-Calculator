using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedTextBox : TextBox, ISwitchManagedDefault {

        // --- VARIABLES ---

        public virtual object? DefaultValue { get; set; } = null;

        public bool TabContentsChanged {
            get {
                // check for empty or actual value
                switch (DefaultValue) {
                    case "":
                    case null:
                        return !string.IsNullOrWhiteSpace(Text);
                    default:
                        return !string.Equals(Text, DefaultValue);
                }
            }
        }

        public bool RequiresReset { get; set; } = true;

        // --- METHODS ---

        public void Reset() {
            Text = DefaultValue as string;
        }
    }
}
