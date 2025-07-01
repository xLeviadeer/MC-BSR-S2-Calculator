using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn {
    public interface ICoordinate {

        // --- VARIABLES ---

        public int X { get; set; }

        public int Y { get; }

        public int Z { get; set; }
    }
}
