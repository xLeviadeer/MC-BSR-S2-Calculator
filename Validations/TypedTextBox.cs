using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MC_BSR_S2_Calculator.Validations {
    public abstract class TypedTextBox<T> : TextBox, IValidatable 
        where T : IParsable<T> {

        // --- VARIABLES ---
        #region VARIABLES

        // - ValidationType -

        /// <summary>
        /// container for all possible ValidationType values
        /// </summary>
        public struct ValidationTypes {
            public const string Constant = "Constant";
            public const string Final = "Final";
            public static readonly string[] All = { Constant, Final };
        };

        /// <summary>
        /// Determines when the text of the text box is validated
        /// </summary>
        public string ValidationType {
            get => (string)GetValue(ValidationTypeProperty);
            set => SetValue(ValidationTypeProperty, value);
        }

        /// <summary>
        /// Dependency property for ValidationType
        /// </summary>
        [Category("Common")]
        [Description("determines when the text is validated")]
        public static readonly DependencyProperty ValidationTypeProperty = DependencyProperty.Register(
            nameof(ValidationType),
            typeof(string),
            typeof(TypedTextBox<T>),
            new PropertyMetadata(ValidationTypes.Final, OnValidationTypeChanged) // includes event when changed
        );

        /// <summary>
        /// checks if the ValidationType is a valid ValidationTypes string
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if invalid ValidationType string</exception>
        private static void OnValidationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (ValidationTypes.All.Any(type => (type != (string)e.NewValue))) {
                throw new ArgumentException($"ValidationType was not set to a valid string");
            }
        }

        // - IsNullable -

        /// <summary>
        /// Determines if the text can be null/empty
        /// </summary>
        public bool IsNullable {
            get => (bool)GetValue(IsNullableProperty);
            set => SetValue(IsNullableProperty, value);
        }

        /// <summary>
        /// Dependency property for IsNullable
        /// </summary>
        [Category("Common")]
        [Description("determines if the text can be set to null/empty")]
        public static readonly DependencyProperty IsNullableProperty = DependencyProperty.Register(
            nameof(IsNullable), // name
            typeof(bool), // type of value
            typeof(TypedTextBox<T>), // type of owner
            new PropertyMetadata(false) // default value
        );

        // - DefaultValue -

        /// <summary>
        /// The default value for this textbox
        /// </summary>
        public T DefaultValue {
            get => (T)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        /// <summary>
        /// Determines what the default value is 
        /// </summary>
        [Category("Common")]
        [Description("determines what the default value is")]
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
            nameof(DefaultValue),
            typeof(T),
            typeof(TypedTextBox<T>),
            new PropertyMetadata(default)
        );

        // - Value -

        /// <summary>
        /// The value parsed from the text
        /// </summary>
        public T? Value { get; private set; }

        /// <summary>
        /// The last value that was successfully validated
        /// </summary>
        private T? LastValue { get; set; }

        // - LastText -
        
        /// <summary>
        /// The last text in the textbox that was successfully validated
        /// </summary>
        private string LastText { get; set; } = string.Empty;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        /// <summary>
        /// Adds the validate method as a listener to the validation event
        /// </summary>
        public TypedTextBox() {
            Validate(this, new()); // validate upon creation

            // set validation events
            switch (ValidationType) {
                case ValidationTypes.Final:
                    this.KeyDown += (sender, args) => {
                        if (args.Key == Key.Enter) {
                            Validate(sender, args);
                        }
                    };
                    this.LostFocus += Validate;
                    break;
                case ValidationTypes.Constant:
                    this.TextChanged += Validate;
                    break;
            }

            // set default values
            this.Text = DefaultValue.ToString();
            LastText = DefaultValue.ToString() ?? "";
            LastValue = DefaultValue;

            // set max length
            this.MaxLength = 25;
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        protected void RevertText(TypedTextBox<T> textBox) {
            // value
            Value = LastValue;

            // text box
            int cursorPosition = textBox.CaretIndex - 1; // hold cursor position
            textBox.Text = LastText; // change text
            if (cursorPosition > textBox.Text.Length) {
                textBox.CaretIndex = textBox.Text.Length;
            } else if (cursorPosition < 0) {
                textBox.CaretIndex = 0;
            } else {
                textBox.CaretIndex = cursorPosition;
            }
        }

        /// <summary>
        /// Ensures that contents of the textbox are the associated type
        /// </summary>
        public virtual void Validate(Object? sender, EventArgs args) {
            // type cast sender
            ArgumentNullException.ThrowIfNull(sender);
            var textBox = (TypedTextBox<T>)sender;

            // method to set last values
            void setLastvalues() {
                LastValue = Value;
                LastText = textBox.Text;
            }

            // check if empty and set to last or do nothing
            if (string.IsNullOrWhiteSpace(textBox.Text)) {
                if (!IsNullable) { // set current to last text
                    // if default value is null
                    if (DefaultValue == null) { // revert
                        RevertText(textBox);
                    } else { // set to default
                        Value = LastValue;
                        textBox.Text = DefaultValue.ToString() ?? "";
                    }

                    // set last values
                    setLastvalues();
                    return;
                } else {
                    // set last values as defaults
                    Value = default; // null
                    setLastvalues();
                    return;
                }
            }

            // validate type via parsing
            try {
                Value = T.Parse(textBox.Text, CultureInfo.InvariantCulture);
            } catch (Exception err) when (
                err is FormatException 
                || err is ArgumentNullException 
                || err is OverflowException
                || err is SyntaxErrorException
            ) { // revert text if there was parsing error
                RevertText(textBox);
                return;
            }

            // set last values to the current values
            setLastvalues();
        }

        #endregion
    }
}
