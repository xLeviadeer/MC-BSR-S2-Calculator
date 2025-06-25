using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public readonly struct ColorResources {
        // reds
        public static readonly Color DullRedColor = Color.FromRgb(210, 100, 100);
        public static readonly Color RedColor = Color.FromRgb(210, 80, 80);
        public static readonly Color BrightRedColor = Color.FromRgb(255, 40, 40);
        public static readonly Color LightRedColor = Color.FromRgb(255, 200, 200);
        public static readonly Color VeryLightRedColor = Color.FromRgb(255, 220, 220);

        // greys
        public static readonly Color VeryDarkGreyColor = Color.FromRgb(64, 64, 64);
        public static readonly Color DarkGreyColor = Color.FromRgb(96, 96, 96);
        public static readonly Color GreyColor = Color.FromRgb(128, 128, 128);
        public static readonly Color LightGreyColor = Color.FromRgb(160, 160, 160);
        public static readonly Color VeryLightGreyColor = Color.FromRgb(192, 192, 192);

        // border colors
        // 0 - 64 D
        // 64 - 128 MD
        // 128 - 192 ML
        // 192 - 255 L
        public static readonly Color InnerColorM = Color.FromRgb(128, 128, 128);
        public static readonly Color InnerColorML = Color.FromRgb(140, 140, 140);
        public static readonly Color InnerColorML1 = Color.FromRgb(150, 150, 150);
        public static readonly Color InnerColorML2 = Color.FromRgb(160, 160, 160);
        public static readonly Color InnerColorML3 = Color.FromRgb(170, 170, 170);
        public static readonly Color InnerColorML4 = Color.FromRgb(180, 180, 180);
        public static readonly Color InnerColorML5 = Color.FromRgb(192, 192, 192); // main tab color
        public static readonly Color InnerColorL = Color.FromRgb(200, 200, 200); // sub tab color
        public static readonly Color InnerColorL2 = Color.FromRgb(210, 210, 210); // tab inner backgrounded content border color
        public static readonly Color InnerColorL3 = Color.FromRgb(220, 220, 220); // tab inner content border color
        public static readonly Color InnerColorL4 = Color.FromRgb(230, 230, 230);
        public static readonly Color InnerColorL5 = Color.FromRgb(240, 240, 240); // tab inner backgrounded content background
        public static readonly Color InnerColorL6 = Color.FromRgb(250, 250, 250);

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
