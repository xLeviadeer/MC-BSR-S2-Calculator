using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.GlobalColumns {
    public static class SystemMetricsHelper {

        // --- GETTER METHOD ---
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        // --- LOCATION CONSTANTS ---
        private const int SM_CXVSCROLL = 2;
        private const int SM_CYHSCROLL = 20;

        // --- GET METHODS ---
        public static int GetDefaultVerticalScrollBarWidth() => GetSystemMetrics(SM_CXVSCROLL);
    }
}
