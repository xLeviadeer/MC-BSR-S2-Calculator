using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MC_BSR_S2_Calculator.GlobalColumns;

namespace MC_BSR_S2_Calculator
{
    public partial class MainWindow : Window {

        // test static
        internal static List<TestClass> TestClassList = new() { new() { SomeValue = false } };

        public MainWindow() {
            InitializeComponent();

            // testing zone
            foreach (TestClass value in PlayerColumnControl.TestClassListDisplay.DataList) {
                Debug.WriteLine(value.SomeValue.Value);
            }
        }
    }
}