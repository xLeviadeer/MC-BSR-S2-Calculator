using MC_BSR_S2_Calculator.Utility.Validations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    /// <summary>
    /// Class to extend to mark a class as containing DisplayValues
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class Displayable : OptionalMouseClickEventHolder, IValidatable {

        // --- VARIABLES ---
        #region VARIABLES

        // - Held Button -

        public Button? HeldButton { get; set; } = null;

        // - IsValid -

        public bool? IsValid { get; set; }

        // - Attribute Building -

        /// <summary>
        /// Bool for controlling when/if the DisplayValues have been built
        /// </summary>
        private bool AttributeValuesHaveBeenBuilt = false;

        /// <summary>
        /// Builds the DisplayValues when they are first accessed
        /// </summary>
        private void BuildAttributeValues() {
            // build state recursion control
            AttributeValuesHaveBeenBuilt = true;

            // flags to check from
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

            // for every property/field
            foreach (var memberInfo in GetType().GetMembers(flags).ToList()) {
                // check if value is an attribute
                if (memberInfo.IsDefined(
                    typeof(DisplayValueAttribute),
                    inherit: true
                )) {
                    // get associated attribute
                    var attribute = memberInfo.GetCustomAttribute<DisplayValueAttribute>(inherit: true);
                    if (attribute == null) { continue; }

                    // get the attribute's associated value
                    if (memberInfo is PropertyInfo propertyInfo) { // property value
                        var value = propertyInfo.GetValue(this);
                        if (value is DisplayValueBase displayValue) {
                            DisplayValues[attribute.DisplayName] = displayValue;
                            PropertyNames[attribute.DisplayName] = propertyInfo.Name;
                            DisplayOrders[attribute.DisplayName] = attribute.DisplayOrder;
                            ColumnWidths[attribute.DisplayName] = attribute.ColumnWidth;
                            ColumnContentAlignments[attribute.DisplayName] = attribute.ColumnContentAlignment;
                            DisplayLayers[attribute.DisplayName] = attribute.DisplayLayers;
                            HitTestVisibilities[attribute.DisplayName] = attribute.IsHitTestVisible;
                        }
                    } else if (memberInfo is FieldInfo fieldInfo) { // field value
                        var value = fieldInfo.GetValue(this);
                        if (value is DisplayValueBase displayValue) {
                            DisplayValues[attribute.DisplayName] = displayValue;
                            PropertyNames[attribute.DisplayName] = fieldInfo.Name;
                            DisplayOrders[attribute.DisplayName] = attribute.DisplayOrder;
                            ColumnWidths[attribute.DisplayName] = attribute.ColumnWidth;
                            ColumnContentAlignments[attribute.DisplayName] = attribute.ColumnContentAlignment;
                            DisplayLayers[attribute.DisplayName] = attribute.DisplayLayers;
                            HitTestVisibilities[attribute.DisplayName] = attribute.IsHitTestVisible;
                        }
                    }
                }
            }
        }

        // - Display Names -

        private Dictionary<string, DisplayValueBase> _displayValues = new();

        /// <summary>
        /// holds a list of display name values locatable by header name
        /// </summary>
        public Dictionary<string, DisplayValueBase> DisplayValues {
            get {
                if (!AttributeValuesHaveBeenBuilt) { // only builds once
                    BuildAttributeValues();
                }
                return _displayValues;
            }
            set => _displayValues = value;
        }

        // - Property Names -

        private Dictionary<string, string> _propertyNames = new();

        /// <summary>
        /// holds a list of property/field names as they are found by display name
        /// </summary>
        public Dictionary<string, string> PropertyNames {
            get {
                if (!AttributeValuesHaveBeenBuilt) {
                    BuildAttributeValues();
                }
                return _propertyNames;
            }
            set => _propertyNames = value;
        }

        // - Display Orders -

        private Dictionary<string, int> _displayOrders = new();

        public Dictionary<string, int> DisplayOrders {
            get {
                if (!AttributeValuesHaveBeenBuilt) {
                    BuildAttributeValues();
                }
                return _displayOrders;
            }
            set => _displayOrders = value;
        }

        // - Column Widths -

        private Dictionary<string, GridLength> _columnWidths = new();

        /// <summary>
        /// holds a list of column width values locatable by header name
        /// </summary>
        public Dictionary<string, GridLength> ColumnWidths {
            get {
                if (!AttributeValuesHaveBeenBuilt) { // only builds once
                    BuildAttributeValues();
                }
                return _columnWidths;
            }
            set => _columnWidths = value;
        }

        // - Column Content Alignment -

        private Dictionary<string, DisplayValueAttribute.ColumnContentAlignmnetRecord> _columnContentAlignments = new();

        /// <summary>
        /// holds a list of column alignments locatable by header name
        /// </summary>
        public Dictionary<string, DisplayValueAttribute.ColumnContentAlignmnetRecord> ColumnContentAlignments {
            get {
                if (!AttributeValuesHaveBeenBuilt) { // only builds once
                    BuildAttributeValues();
                }
                return _columnContentAlignments;
            }
            set => _columnContentAlignments = value;
        }

        // - Column Display Layers -

        private Dictionary<string, List<int>> _displayLayers = new();

        /// <summary>
        /// holds a list of list display layer ints locatable by header name
        /// </summary>
        public Dictionary<string, List<int>> DisplayLayers {
            get {
                if (!AttributeValuesHaveBeenBuilt) { // only builds once
                    BuildAttributeValues();
                }
                return _displayLayers;
            }
            set => _displayLayers = value;
        }

        // - Hit Testing -

        private Dictionary<string, bool> _hitTestVisibilities = new();

        public Dictionary<string, bool> HitTestVisibilities {
            get {
                if (!AttributeValuesHaveBeenBuilt) { // only builds once
                    BuildAttributeValues();
                }
                return _hitTestVisibilities;
            }
            set => _hitTestVisibilities = value;
        }

        // - Headers -

        /// <summary>
        /// Gets the headers for the display values
        /// </summary>
        public List<string> DisplayHeaders {
            get => DisplayValues.Keys.ToList();
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR 

        /// <summary>
        /// Validates on construction
        /// </summary>
        public Displayable() {
            Validate(this, new());
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        protected virtual void ValidateForEachMemberInfo(Displayable displayable, MemberInfo memberInfo) {
            // check if attribute's associated value is of type DisplayValue
            if (memberInfo is PropertyInfo propertyInfo) { // property value
                var value = propertyInfo.GetValue(displayable);
                if (value == null) { return; }
                if (value is not DisplayValueBase) { // check if it's not a display value
                    IsValid = false;
                    throw new ValidationException($"A value attributed with DisplayValueBase in class {displayable} was not an DisplayValueBase");
                }
            } else if (memberInfo is FieldInfo fieldInfo) { // field value
                var value = fieldInfo.GetValue(displayable);
                if (value == null) { return; }
                if (value is not DisplayValueBase) { // check if it's not a display value
                    IsValid = false;
                    throw new ValidationException($"A value attributed with DisplayValueBase in class {displayable} was not an DisplayValueBase");
                }
            }
        }

        /// <summary>
        /// Validates that the class contains at least one DisplayValue marked value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ValidationException"></exception>
        public virtual void Validate(object sender, EventArgs args) {
            // check that sender is not null and is displayable
            if (sender == null) {
                IsValid = false;
                throw new ArgumentNullException(nameof(sender));
            }

            // if sender is displayable
            if (sender is Displayable displayable) {

                // flags to check from
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

                // for every property/field
                bool foundAnyDisplayValues = false;
                foreach (var memberInfo in displayable.GetType().GetMembers(flags).ToList()) {
                    // check if attribute is a display value attribute
                    if (memberInfo.IsDefined(
                        typeof(DisplayValueAttribute),
                        inherit: true
                    )) {
                        // found at least one attribute
                        foundAnyDisplayValues = true;

                        // validate member
                        ValidateForEachMemberInfo(displayable, memberInfo);
                    }
                }

                // if contains no DisplayValues
                if (!foundAnyDisplayValues) {
                    IsValid = false;
                    throw new ValidationException("Displayable child had no public DisplayValueAttributes");
                }

                // valid
                IsValid = true;
            }
        }

        #endregion
    }
}
