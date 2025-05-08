using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    /// <summary>
    /// Regular DisplayValue class, extension with a constructor
    /// </summary>
    internal class DisplayValue : IDisplayValue {
        public FrameworkElement DisplayObject { get; init; }

        public DisplayValue(FrameworkElement displayObject) {
            // set display object
            DisplayObject = displayObject;
        }
    }
}
