using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public readonly struct ColorResources {
        // greys
        public static readonly Color InnerBorderColor = Color.FromRgb(187, 187, 187);
        public static readonly Color InnerBorderLightColor = Color.FromRgb(200, 200, 200);
        public static readonly Color InnerBorderVeryLightColor = Color.FromRgb(220, 220, 220);

        // reds
        public static readonly Color DullRedColor = Color.FromRgb(210, 100, 100);
        public static readonly Color RedColor = Color.FromRgb(210, 80, 80);
        public static readonly Color BrightRedColor = Color.FromRgb(255, 40, 40);
        public static readonly Color LightRedColor = Color.FromRgb(255, 200, 200);
        public static readonly Color VeryLightRedColor = Color.FromRgb(255, 220, 220);

        // hovering 
        public static readonly Color HoverColor = Color.FromArgb(77, 80, 180, 255);
        public static readonly Color ClickColor = Color.FromArgb(77, 140, 200, 255);
        public static readonly Color DisabledOverlayColor = Color.FromArgb(125, 170, 170, 170);

        // charging button
        public static readonly Color DarkerRedColor = Color.FromRgb(108, 49, 49);
        public static readonly Color MediumerRedColor = Color.FromRgb(164, 33, 33);
        public static readonly Color BrighterRedColor = Color.FromRgb(204, 73, 73);
    }
}
