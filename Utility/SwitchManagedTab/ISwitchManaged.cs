using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public interface ISwitchManaged {
        // --- VARIABLES ---

        /// <summary>
        /// Tracks if changes were made to this class and hence require it to be reset when tabbing off
        /// </summary>
        public bool TabContentsChanged { get; }

        /// <summary>
        /// Determines whether tabbing off will require the tab to be reset or if it will save it's contents
        /// </summary>
        public bool RequiresReset { get; set; }

        // -- METHODS --

        /// <summary>
        /// Resets this class; sets this class back to it's default state
        /// </summary>
        public abstract void Reset();
    }
}
