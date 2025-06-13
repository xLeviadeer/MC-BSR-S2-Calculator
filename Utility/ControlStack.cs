using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility {

    public class ControlStack : ItemsControl {

        // --- CONSTRUCTOR ---

        static ControlStack() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlStack),
                new FrameworkPropertyMetadata(typeof(ControlStack)));
        }
    }
}
