using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Security.Cryptography;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System.Windows.Media;
using System.Security.Policy;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {
    public class TextLabel : LabeledInputBase<TextBox> {
        // --- VARIABLES ---
        #region VARIABLES

        // - TextBoxType -

        [Category("Common")]
        [Description("The type of TextBox to use")]
        public TextBoxTypes TextBoxType {
            get => (TextBoxTypes)GetValue(TextBoxTypeProperty);
            set => SetValue(TextBoxTypeProperty, value);
        }

        public static readonly DependencyProperty TextBoxTypeProperty = DependencyProperty.Register(
            nameof(TextBoxType),
            typeof(TextBoxTypes),
            typeof(TextLabel),
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
            typeof(TextLabel),
            new PropertyMetadata(0)
        );

        // - text box input -
        public override TextBox Element { get; set; } = new();

        // - expose IsValid -

        public bool? IsValid {
            get {
                if (Element is ColorValidatedTextBox colorTextBox) {
                    return colorTextBox.IsValid;
                } else {
                    return true; // assume textboxes are always valid
                }
            }
            set {
                if (Element is ColorValidatedTextBox colorTextBox) {
                    colorTextBox.IsValid = value;
                } // does nothing if not color validated text box
            }
        }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        protected void ValidityChangedInvoke(object? sender, BoolEventArgs args) {
            ValidityChanged?.Invoke(sender, args);
        }

        // - expose KeyDownEnter -

        public event EventHandler<KeyEventArgs>? KeyDownEnter;

        protected void KeyDownEnterInvoke(object? sender, KeyEventArgs args) {
            KeyDownEnter?.Invoke(sender, args);
        }

        public event EventHandler<KeyEventArgs>? KeyUpEnter;

        protected void KeyUpEnterInvoke(object? sender, KeyEventArgs args) {
            KeyUpEnter?.Invoke(sender, args);
        }

        // - expose Text -

        public string Text {
            get => Element.Text;
            set => Element.Text = value;
        }

        // - expose min and max input values (only if they exist) -
        
        public double MinInputFromTextLabel {
            get => (double)GetValue(MinInputFromTextLabelProperty);
            set => SetValue(MinInputFromTextLabelProperty, value);
        }

        public static readonly DependencyProperty MinInputFromTextLabelProperty = DependencyProperty.Register(
            nameof(MinInputFromTextLabel),
            typeof(double),
            typeof(TextLabel),
            new PropertyMetadata(double.MinValue)
        );

        public double MaxInputFromTextLabel {
            get => (double)GetValue(MaxInputFromTextLabelProperty);
            set => SetValue(MaxInputFromTextLabelProperty, value);
        }

        public static readonly DependencyProperty MaxInputFromTextLabelProperty = DependencyProperty.Register(
            nameof(MaxInputFromTextLabel),
            typeof(double),
            typeof(TextLabel),
            new PropertyMetadata(double.MaxValue)
        );

        // - has been loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public TextLabel() {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, EventArgs args) {
            // handles basic loading details

            // don't run if already loaded
            if (HasBeenLoaded) { return; }
            HasBeenLoaded = true;

            // create text box of specified type
            Element = TextBoxType switch {
                TextBoxTypes.TextBox => new TextBox(),
                TextBoxTypes.IntegerTextBox => new IntegerTextBox(),
                TextBoxTypes.DoubleTextBox => new DoubleTextBox(),
                TextBoxTypes.StringTextBox => new StringTextBox(),
                _ => new TextBox()
            };

            // text box input settings
            Element.MaxLength = TextBoxMaxLength;
            Element.HorizontalContentAlignment = HorizontalAlignment.Left;
            Element.VerticalContentAlignment = VerticalAlignment.Center;
            Element.Height = (double)Application.Current.Resources["TextBoxHeight"];
            Element.Margin = new Thickness(3, 1, 3, 3);
            Element.FontSize = 11;

            // if the textbox is color validated, expose IsValid
            if (Element is ColorValidatedTextBox colorTextBox) {
                // expose event
                colorTextBox.ValidityChanged += (object? sender, BoolEventArgs args) => { this.ValidityChangedInvoke(this, args); };

                // invoke to attempt changes at startup
                this.ValidityChangedInvoke(this, new(colorTextBox.IsValid));
            }

            // if it's an EnterTextBox
            if (Element is EnterTextBox enterTextBox) {
                // expose event
                enterTextBox.KeyDownEnter += (object? sender, KeyEventArgs args) => { this.KeyDownEnterInvoke(this, args); };
                enterTextBox.KeyUpEnter += (object? sender, KeyEventArgs args) => { this.KeyUpEnterInvoke(this, args); };
            }

            // if it's a number text box
            if (Element is NumberTextBox numberTextBox) {
                numberTextBox.MinInput = MinInputFromTextLabel;
                numberTextBox.MaxInput = MaxInputFromTextLabel; 
            }

            // apply layout mode
            ApplyLayoutMode();
        }

        #endregion
    }
}
