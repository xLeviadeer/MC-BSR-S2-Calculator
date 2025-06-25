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

        public virtual double AddValue {
            get => Value.Value;
        }

        // - Remove Button -

        [DisplayValue("", 70, GridUnitType.Pixel, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, displayOrder:2, isHitTestVisible:true)]
        public DisplayValue<Button> RemoveButton { get; set; }

        public abstract event EventHandler<EventArgs>? RemoveRequested;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        protected virtual void SetDefaultValues(string name, double value) {
            // name
            var nameLabel = new Label();
            Name = new(
                nameLabel,
                Label.ContentProperty,
                name
            );

            // value
            var valueLabel = new Label();
            Value = new(
                valueLabel,
                Label.ContentProperty,
                value
            );
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Incentive() => SetDefaultValues(string.Empty, 0.0d);

        public Incentive(
            string name,
            double value
        ) => SetDefaultValues(name, value);
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


        #endregion
    }
}
