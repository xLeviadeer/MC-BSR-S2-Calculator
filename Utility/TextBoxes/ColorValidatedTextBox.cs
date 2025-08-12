using MC_BSR_S2_Calculator.Utility.Validations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {
    public abstract class ColorValidatedTextBox : EnterTextBox, IValidatable {

        // --- VARIABLES ---

        // - IsValid -

        private bool? _isValid;
        public bool? IsValid {
            get => _isValid;
            set {
                if (_isValid != value) {
                    // change color of text box (when the validity is updated)
                    if (DoColorChanges) {
                        BorderBrush = value switch {
                            true => ValidColor,
                            false => InvalidColor,
                            null => DefaultColor
                        };
                    }

                    // invoke change event
                    ValidityChanged?.Invoke(this, new BoolEventArgs(value));
                }
                _isValid = value;
            }
        }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // -- Coloring --
        #region Coloring

        // - Do Color Changes -

        public bool DoColorChanges {
            get => (bool)GetValue(DoColorChangesProperty);
            set => SetValue(DoColorChangesProperty, value);
        }

        public static DependencyProperty DoColorChangesProperty = DependencyProperty.Register(
            nameof(DoColorChanges),
            typeof(bool),
            typeof(ColorValidatedTextBox),
            new PropertyMetadata(true)
        );

        // - Default Color -

        public SolidColorBrush DefaultColor {
            get => (SolidColorBrush)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        public static DependencyProperty DefaultColorProperty = DependencyProperty.Register(
            nameof(DefaultColor),
            typeof(SolidColorBrush),
            typeof(ColorValidatedTextBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(171, 173, 179)))
        );

        // - Invalid Color -

        public SolidColorBrush InvalidColor {
            get => (SolidColorBrush)GetValue(InvalidColorProperty);
            set => SetValue(InvalidColorProperty, value);
        }

        public static DependencyProperty InvalidColorProperty = DependencyProperty.Register(
            nameof(InvalidColor),
            typeof(SolidColorBrush),
            typeof(ColorValidatedTextBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 100, 100)))
        );

        // - Valid Color -

        public SolidColorBrush ValidColor {
            get => (SolidColorBrush)GetValue(ValidColorProperty);
            set => SetValue(ValidColorProperty, value);
        }

        public static DependencyProperty ValidColorProperty = DependencyProperty.Register(
            nameof(ValidColor),
            typeof(SolidColorBrush),
            typeof(ColorValidatedTextBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(100, 250, 100)))
        );

        #endregion

        // --- CONSTRUCTOR ---

        public ColorValidatedTextBox() {
            // set to default color at startup
            Loaded += (s, e) => {
                BorderBrush = DefaultColor;
            };
        }

        // --- METHODS ---

        public abstract void Validate(object sender, EventArgs args);
    }
}
