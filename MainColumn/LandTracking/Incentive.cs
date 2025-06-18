using MC_BSR_S2_Calculator.Utility.DisplayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public abstract class Incentive : Displayable {
        // --- VARIABLES ---
        #region VARIABLES 

        // - Name -

        [DisplayValue("Name", 1, GridUnitType.Star, HorizontalAlignment.Left, VerticalAlignment.Center, displayOrder:0)]
        public BoundDisplayValue<Label, string> Name { get; set; }

        // - Value -

        [DisplayValue("Value", 50, GridUnitType.Pixel, HorizontalAlignment.Center, VerticalAlignment.Center, displayOrder:2)]
        public BoundDisplayValue<Label, double> Value { get; set; }

        // - Remove Button -

        [DisplayValue("", 75, GridUnitType.Pixel, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, displayOrder:2, isHitTestVisible:true)]
        public DisplayValue<Button> RemoveButton { get; set; }

        public abstract event EventHandler<EventArgs>? RemoveRequested;

        #endregion
    }
}
