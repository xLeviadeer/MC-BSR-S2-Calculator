using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace MC_BSR_S2_Calculator.Utility {
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

        public enum TextBoxTypes {
            TextBox,
            IntegerTextBox,
            DoubleTextBox,
            StringTextBox
        }

        [Category("Common")]
        [Description("The type of TextBox to use")]
        public TextBoxTypes TextBoxType {
            get => (TextBoxTypes)GetValue(TextBoxTypeProperty);
            set => SetValue(TextBoxTypeProperty, value);
        }

        public static readonly DependencyProperty TextBoxTypeProperty = DependencyProperty.Register(
            nameof(TextBoxType),
            typeof(TextBoxTypes),
            typeof(TextLabelAbove),
            new PropertyMetadata(TextBoxTypes.TextBox)
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

        // - Text Box Input -

        public TextBox TextBoxInput { get; set; } = new();

        // - expose IsValid -

        public bool? IsValid {
            get {
                if (TextBoxInput is ColorValidatedTextBox colorTextBox) {
                    return colorTextBox.IsValid;
                } else {
                    return true; // assume textboxes are always valid
                }
            }
        }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - expose KeyDownEnter -

        public event EventHandler<KeyEventArgs>? KeyDownEnter;
        public event EventHandler<KeyEventArgs>? KeyUpEnter;

        // - expose Text -

        public string Text {
            get => TextBoxInput.Text;
            set => TextBoxInput.Text = value;
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public TextLabelAbove() {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args) {
            // create text box of specified type
            TextBoxInput = TextBoxType switch {
                TextBoxTypes.TextBox => new TextBox(),
                TextBoxTypes.IntegerTextBox => new IntegerTextBox(),
                TextBoxTypes.DoubleTextBox => new DoubleTextBox(),
                TextBoxTypes.StringTextBox => new StringTextBox(),
                _ => new TextBox()
            };

            // settings
            TextBoxInput.MaxLength = TextBoxMaxLength;
            TextBoxInput.HorizontalContentAlignment = HorizontalAlignment.Left;
            TextBoxInput.VerticalContentAlignment = VerticalAlignment.Center;
            TextBoxInput.Height = 22;
            TextBoxInput.Margin = new Thickness(3, 1, 3, 3);

            // set textbox to the grid
            MainGrid.Children.Add(TextBoxInput);
            Grid.SetRow(TextBoxInput, 1);

            // if the textbox is color validated, expose IsValid
            if (TextBoxInput is ColorValidatedTextBox colorTextBox) {
                // expose event
                colorTextBox.ValidityChanged += (object? sender, BoolEventArgs args) => { this.ValidityChanged?.Invoke(this, args); };

                // invoke to attempt changes at startup
                this.ValidityChanged?.Invoke(this, new(colorTextBox.IsValid));
            }

            // if it's an EnterTextBox
            if (TextBoxInput is EnterTextBox enterTextBox) {
                // expose event
                enterTextBox.KeyDownEnter += (object? sender, KeyEventArgs args) => { this.KeyDownEnter?.Invoke(this, args); };
                enterTextBox.KeyUpEnter += (object? sender, KeyEventArgs args) => { this.KeyUpEnter?.Invoke(this, args); };
            }
        }

        #endregion
    }
}
