using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MC_BSR_S2_Calculator.GlobalColumns.DisplayList {

    /// <summary>
    /// DisplayValue which's display mimics the value as it changes
    /// </summary>
    /// <typeparam name="T"> DisplayObject type T; T must be a type of FrameworkElement </typeparam>
    /// <typeparam name="U"> The type of value this class holds </typeparam>
    internal class BoundDisplayValue<T, U> : IDisplayValue
        where T : FrameworkElement {
        // --- VARIABLES ---
        #region VARIABLES

        // - Value -

        private U _value;
        public U Value {
            get => _value;
            set {
                _value = value;
                UpdateDisplayObjectValue();
            } 
        }

        // - Display Object -

        /// <summary>
        /// Holds the total object to be displayed
        /// </summary>
        public FrameworkElement DisplayObject { get; }

        // - Content Reference -

        /// <summary>
        /// Holds a reference to the object which's content will be updated when the value changed
        /// </summary>
        private object ContentReference { get; set; }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public BoundDisplayValue(U value, T displayObject, TextBox contentReference) {
            // set class properties
            DisplayObject = displayObject;
            ContentReference = contentReference;
            _value = value; 
            UpdateDisplayObjectValue();
        }

        public BoundDisplayValue(U value, T displayObject, TextBlock contentReference) {
            // set class properties
            DisplayObject = displayObject;
            ContentReference = contentReference;
            _value = value; 
            UpdateDisplayObjectValue();
        }

        public BoundDisplayValue(U value, T displayObject, ContentControl contentReference) {
            // set class properties
            DisplayObject = displayObject;
            ContentReference = contentReference;
            _value = value; 
            UpdateDisplayObjectValue();
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        /// <summary>
        /// Updates the display object's content reference display with the current value
        /// </summary>
        private void UpdateDisplayObjectValue() {
            // switch based on ContentReference type            
            switch (ContentReference) {
                case TextBox: {
                        var textBox = (TextBox)ContentReference;
                        textBox.Text = Value?.ToString() ?? "";
                        break;
                    }
                case TextBlock: {
                        var textBox = (TextBlock)ContentReference;
                        textBox.Text = Value?.ToString() ?? "";
                        break;
                    }
                case ContentControl:
                    var contentControl = (ContentControl)ContentReference;
                    contentControl.Content = Value?.ToString() ?? "";
                    break;
            }
        }

        #endregion
    }
}
