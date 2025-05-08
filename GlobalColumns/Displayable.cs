using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MC_BSR_S2_Calculator.Validations;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    /// <summary>
    /// Class to extend to mark a class as containing DisplayValues
    /// </summary>
    internal abstract class Displayable : IValidatable {

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

        /// <summary>
        /// Validates that the class contains at least one DisplayValue marked value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ValidationException"></exception>
        public void Validate(object sender, EventArgs args) {
            // check that sender is not null and is displayable
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            if (sender is Displayable displayable) {

                // flags to check from
                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

                // for every property/field
                bool foundAnyDisplayValues = false;
                foreach (var memberInfo in displayable.GetType().GetMembers(flags).ToList()) {
                    // check if value is an attribute
                    if (memberInfo.IsDefined(
                        typeof(DisplayValueAttribute),
                        inherit: true
                    )) {
                        // found at least one attribute
                        foundAnyDisplayValues = true;

                        // check if attribute's associated value is of type DisplayValue
                        if (memberInfo is PropertyInfo propertyInfo) { // property value
                            var value = propertyInfo.GetValue(displayable);
                            if (value == null) { continue; }
                            if (value is not IDisplayValue) { // check if it's not a display value
                                throw new ValidationException($"A value attributed with IDisplayValue in class {displayable} was not an IDisplayValue");
                            }
                        } else if (memberInfo is FieldInfo fieldInfo) { // field value
                            var value = fieldInfo.GetValue(displayable);
                            if (value == null) { continue; }
                            if (value is not IDisplayValue) { // check if it's not a display value
                                throw new ValidationException($"A value attributed with IDisplayValue in class {displayable} was not an IDisplayValue");
                            }
                        }
                    }
                }

                // if contains no DisplayValues
                if (!foundAnyDisplayValues) {
                    throw new ValidationException("Displayable child had no public DisplayValueAttributes");
                }
            }
        }

        #endregion
    }
}
