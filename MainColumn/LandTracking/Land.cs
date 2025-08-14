using MC_BSR_S2_Calculator.Utility.Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class Land : ILandArea {

        // --- VARIABLES ---

        // - As ILandArea -

        public ILandArea AsILandArea => (ILandArea)this;

        // - Default Bottom/Top -

        public int DefaultBottom { get; set; } = LandDefinitions.SurfaceLandareaYLevelMin;

        public int DefaultTop { get; set; } = LandDefinitions.WORLD_HEIGHT;

        // - Bounds -

        public List<ICoordinateBoundAmbiguous> Bounds { get; } = new();

        // --- CONSTRUCTOR ---

        public Land() {
            // check validity of defaults
            AsILandArea.HasValidDefaults();
        }
    }
}
