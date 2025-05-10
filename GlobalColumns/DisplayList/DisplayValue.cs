using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns.DisplayList {

    /// <summary>
    /// Regular DisplayValue class, extension with a constructor
    /// </summary>
    /// <typeparam name="T"> DisplayObject type T; T must be a type of FrameworkElement </typeparam>
    internal class DisplayValue<T> : IDisplayValue
        where T : FrameworkElement {
        public FrameworkElement DisplayObject { get; init; }

        public DisplayValue(T displayObject) {
            // set display object
            DisplayObject = displayObject;
        }
    }
}
