using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn {
    public static class LandDefinitions {
        
        // surface landarea
        public const int SurfaceLandareaYLevel = 50;
        public const int SurfaceLandareaYLevelUnderwater = 10;
        public static int SurfaceLandareaYLevelMax { 
            get => Math.Max(SurfaceLandareaYLevel, SurfaceLandareaYLevelUnderwater); 
        }
    }
}
