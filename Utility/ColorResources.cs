using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public struct ColorResources {
        public static Color HoverColor = Color.FromArgb(77, 80, 180, 255);
        public static Color ClickColor = Color.FromArgb(77, 140, 200, 255);
        public static Color DisabledOverlayColor = Color.FromArgb(125, 170, 170, 170);
        public static Color InnerBorderColor = Color.FromRgb(187, 187, 187);
        public static Color InnerBorderLightColor = Color.FromRgb(200, 200, 200);
        public static Color InnerBorderVeryLightColor = Color.FromRgb(220, 220, 220);

        // charging button
        public static Color DarkeRedColor = Color.FromRgb(108, 49, 49);
        public static Color MediumRedColor = Color.FromRgb(164, 33, 33);
        public static Color BrighterRedColor = Color.FromRgb(204, 73, 73);
    }
}
