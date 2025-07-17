using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn {
    public static class LandDefinitions {

        // world height
        public const int WorldHeight = 320;
        public const int WorldDepth = -64;

        // surface landarea
        public const int SurfaceLandareaYLevel = 50;
        public const int SurfaceLandareaYLevelUnderwater = 10;
        public static int SurfaceLandareaYLevelMax { 
            get => Math.Max(SurfaceLandareaYLevel, SurfaceLandareaYLevelUnderwater); 
        }
    }
}
