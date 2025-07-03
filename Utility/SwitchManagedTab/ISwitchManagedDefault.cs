using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public interface ISwitchManagedDefault : ISwitchManaged {
        
        // --- VARIABLES ---

        /// <summary>
        /// The default value this object holds
        /// </summary>
        public object? DefaultValue { get; set; }
    }
}
