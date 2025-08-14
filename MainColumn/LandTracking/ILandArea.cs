using MC_BSR_S2_Calculator.Utility.Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public interface ILandArea {

        // --- VARIABLES ---

        // - As Requirement -

        public ILandArea AsILandArea { get; }

        // - Default Top/Bottom -

        public int DefaultBottom { get; set; }

        public int DefaultTop { get; set; }

        public bool HasValidDefaults() {
            if (
                (DefaultBottom < DefaultTop)
                && (LandDefinitions.WORLD_DEPTH <= DefaultBottom)
                && (DefaultTop <= LandDefinitions.WORLD_HEIGHT)
            ) { return true; } else { return false; }
        }

        // - Bounds -

        public List<ICoordinateBoundAmbiguous> Bounds { get; }
    }
}
