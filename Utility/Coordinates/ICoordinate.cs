using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {
    public interface ICoordinate : IFlatCoordinate {

        // --- VARIABLES ---

        public int Y { get; }
    }

    public interface IModifyableCoordinate : IModifyableFlatCoordinate, ICoordinate {
        // --- VARIABLES ---

        public new int Y { get; set; }
    }
}
