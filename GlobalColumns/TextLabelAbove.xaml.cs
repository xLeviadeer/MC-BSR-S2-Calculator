using MC_BSR_S2_Calculator.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class TextLabelAbove : UserControl {

        // --- VARIABLES ---
        #region VARIABLES

        // - LabelText -

        [Category("Common")]
        [Description("The text to be placed in the TextBlock label")]
        public string LabelText {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
            nameof(LabelText),
            typeof(string),
            typeof(TextLabelAbove),
            new PropertyMetadata(null)
        );

        // - TextBoxType -

        [Category("Common")]
        [Description("The type of TextBox to use")]
        public string TextBoxType {
            get => (string)GetValue(TextBoxTypeProperty);
            set => SetValue(TextBoxTypeProperty, value);
        }

        public static readonly DependencyProperty TextBoxTypeProperty = DependencyProperty.Register(
            nameof(TextBoxType),
            typeof(string),
            typeof(TextLabelAbove),
            new PropertyMetadata("TextBox")
        );

        // - TextBoxMaxLength -

        [Category("Common")]
        [Description("The MaxLength of the TextBox")]
        public int TextBoxMaxLength {
            get => (int)GetValue(TextBoxMaxLengthProperty);
            set => SetValue(TextBoxMaxLengthProperty, value);
        }

        public static readonly DependencyProperty TextBoxMaxLengthProperty = DependencyProperty.Register(
            nameof(TextBoxMaxLength),
            typeof(int),
            typeof(TextLabelAbove),
            new PropertyMetadata(0)
        );

        // - TextBoxInput -

        public TextBox TextBoxInput;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public TextLabelAbove() {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args) {
            // basic textbox settings
            TextBoxInput = new() {
                Margin = new Thickness(3, 1, 3, 3),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Height = 22,
                MaxLength = TextBoxMaxLength
            };

            // create textbox instance
            switch (TextBoxType) {
                case "TextBox":
                    // valid, do nothing
                    break;
                case "IntegerTextBox":
                    TextBoxInput = (IntegerTextBox)TextBoxInput;
                    break;
                case "DoubleTextBox":
                    TextBoxInput = (DoubleTextBox)TextBoxInput;
                    break;
                default:
                    throw new ArgumentException("The provided value for TextBoxType was not a valid type");
            }

            // add to grid
            MainGrid.Children.Add(TextBoxInput);
            Grid.SetRow(TextBoxInput, 1);
        }

        #endregion
    }
}
