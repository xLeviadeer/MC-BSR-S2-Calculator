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
using MC_BSR_S2_Calculator.Validations;

namespace MC_BSR_S2_Calculator
{
    public partial class MainWindow : Window {

        // test static
        internal static List<TestClass> TestClassList;

        public MainWindow() {
            InitializeComponent();

            // section 1

            Button button = new();

            BoundDisplayValue<bool> someDisplayValue = new(
                false,
                button,
                button
            );

            void someFunction(object sender, EventArgs args) {
                someDisplayValue.Value = !someDisplayValue.Value;
            }

            button.Height = 20;
            button.Width = 50;
            button.Click += someFunction;
            SectionsGrid.Children.Add(button);
            Grid.SetColumn(button, 1);

            // section 2
            IntegerTextBox textBox = new();
            textBox.Height = 20;
            textBox.Width = 200;
            textBox.Text = "HELLO GRANNY";
            SectionsGrid.Children.Add(textBox);
            Grid.SetColumn(textBox, 2);

            // section 3

            TestClassList = new() {
                new() {
                    SomeValue = someDisplayValue
                },
                new() {
                    SomeOtherValue = new DisplayValue(textBox)
                }
            };
        }
    }
}