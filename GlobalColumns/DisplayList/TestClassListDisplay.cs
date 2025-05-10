using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MC_BSR_S2_Calculator.GlobalColumns.DisplayList {

    internal class TestClassListDisplay : ListDisplay<TestClass> {

        // static test
        private List<TestClass> list { get; set; } = new();

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        protected override void SetClassDataList() {
            list.AddRange(
                new(),
                new(),
                new(),
                new()
            );

            ClassDataList = list;
        }

        #endregion
    }
}
