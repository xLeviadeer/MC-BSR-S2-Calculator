using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {
    public interface IFlatCoordinate {

        // --- VARIABLES ---

        public int X { get; }

        public int Z { get; }
    }

    public interface IModifyableFlatCoordinate : IFlatCoordinate {

        // --- VARIABLES ---

        public new int X { get; set; }

        public new int Z { get; set; }
    }
}
