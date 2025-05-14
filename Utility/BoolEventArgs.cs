using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public class BoolEventArgs : EventArgs {
        public bool? Value { get; }

        public BoolEventArgs(bool? value) {
            Value = value;
        }
    }
}
