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
using System.Windows.Media.Animation;
using MC_BSR_S2_Calculator.Utility.Validations;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {

    public class InputFinalizedEventArgs<T> : EventArgs {
        public required T? OldValue { get; set; }
        public required T? NewValue { get; set; }
    }

    public abstract class TypedTextBox<T> : ColorValidatedTextBox 
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
            public const string Manual = "Manual";
            public static readonly string[] All = { Constant, Final, Manual };
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
        private static void OnValidationTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            // check if valid mode
            if (ValidationTypes.All.All(type => type != (string)args.NewValue)) {
                throw new ArgumentException($"ValidationType was not set to a valid string");
            }

            // set validation events
            //    validate must always be set before the invoker because validation sets the last stable value
            if (sender is TypedTextBox<T> control) {
                UpdateValidationTypeEventAllocation(control);
            }
        }

        private static void UpdateValidationTypeEventAllocation(TypedTextBox<T> control) {
            // remove all events
            control.KeyDownEnter -= control.ValidateAndFinalize;
            control.LostFocus -= control.ValidateAndFinalize;
            control.TextChanged -= control.ValidateAndFinalize;

            // add events
            switch (control.ValidationType) {
                case ValidationTypes.Final:
                    control.KeyDownEnter += (_, args) => control.ValidateAndFinalize(control, args);
                    control.LostFocus += (_, args) => control.ValidateAndFinalize(control, args);
                    break;
                case ValidationTypes.Constant:
                    control.TextChanged += (_, args) => control.ValidateAndFinalize(control, args);
                    break;
                case ValidationTypes.Manual:
                    // no validation, done manually
                    break;
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
        public new T DefaultValue {
            get => (T)GetValue(DefaultValueProperty);
            set { // set base and new
                base.DefaultValue = value;
                SetValue(DefaultValueProperty, value);
            }
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
            new PropertyMetadata()
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

        /// <summary>
        /// The last value the textbox had that was stable before the current value
        /// </summary>
        private T? LastStableValue { get; set; }

        // - LastText -
        
        /// <summary>
        /// The last text in the textbox that was successfully validated
        /// </summary>
        private string LastText { get; set; } = string.Empty;

        // - InputFinalized Event -

        /// <summary>
        /// Runs when the input has been finalized; when losing focus or pressing enter or text changed depending on settings
        /// </summary>
        public event EventHandler<EventArgs>? InputFinalized;

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        /// <summary>
        /// Adds the validate method as a listener to the validation event
        /// </summary>
        public TypedTextBox() {
            Loaded += (sender, args) => { 
                if (HasBeenLoaded) { return; }
                HasBeenLoaded = true;

                UpdateValidationTypeEventAllocation(this);
                ValidateAndFinalize(sender, args); // validate upon creation
            };

            // set default values
            if (DefaultValue == null) {
                Text = "";
                LastText = "";
            } else {
                Text = DefaultValue.ToString() ?? "";
                LastText = DefaultValue.ToString() ?? "";
            }
            LastValue = (T?)DefaultValue;
            LastStableValue = (T?)DefaultValue;
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

        private void ValidateAndFinalize(object? sender, EventArgs args) {
            Validate(sender, args);

            // invoke finalize
            InputFinalized?.Invoke(this, new InputFinalizedEventArgs<T>() {
                OldValue = LastStableValue,
                NewValue = Value
            });
        }

        /// <summary>
        /// Ensures that contents of the textbox are the associated type
        /// </summary>
        public override void Validate(object? sender, EventArgs args) {
            // type cast sender
            if (sender == null) {
                IsValid = false;
                throw new ArgumentNullException(nameof(sender));
            }
            var textBox = (TypedTextBox<T>)sender;

            // method to set last values
            void setLastvalues() {
                LastValue = Value;
                LastText = textBox.Text;
            }

            // only if nullable
            if (!IsNullable) { 
                // check if empty and set to last or do nothing
                if (string.IsNullOrWhiteSpace(textBox.Text)) { // set current to last text
                    // if default value is null
                    if (DefaultValue == null) { // revert
                        RevertText(textBox);
                    } else { // set to default
                        Value = LastValue;
                        textBox.Text = DefaultValue.ToString() ?? "";
                    }

                    // set last values
                    setLastvalues();
                    IsValid = false;
                    return;
                } //else {
                //    // set last values as defaults
                //    Value = default; // null
                //    setLastvalues();
                //    IsValid = true;
                //    return;
                //}
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
                IsValid = false;
                return;
            }

            // set last stable
            LastStableValue = LastValue;

            // set last values to the current values
            setLastvalues();
            IsValid = true;
        }

        #endregion
    }
}
