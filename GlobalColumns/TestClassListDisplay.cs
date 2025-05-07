using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    internal class TestClassListDisplay : ListDisplay<TestClass> {

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public TestClassListDisplay()
            : base(MainWindow.TestClassList) {
            
        }

        #endregion
    }
}
