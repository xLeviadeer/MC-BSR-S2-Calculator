using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn {
    public static class LandDefinitions {

        // world height
        public const int WORLD_HEIGHT = 320;
        public const int WORLD_DEPTH = -64;

        // surface landarea
        public const int SURFACE_LANDAREA_Y_LEVEL = 50;
        public const int SURFACE_LANDAREA_Y_LEVEL_UNDERWATER = 10;
        public static int SurfaceLandareaYLevelMax
            => Math.Max(SURFACE_LANDAREA_Y_LEVEL, SURFACE_LANDAREA_Y_LEVEL_UNDERWATER);
        public static int SurfaceLandareaYLevelMin
            => Math.Min(SURFACE_LANDAREA_Y_LEVEL, SURFACE_LANDAREA_Y_LEVEL_UNDERWATER);
    }
}
