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
    /// <typeparam name="T"> The type of class which is to be displayed </typeparam>
    internal abstract class Displayable<T> : IValidatable {
        
        // --- CONSTRUCTOR ---

        /// <summary>
        /// Validates on construction
        /// </summary>
        public Displayable() {
            Validate(new(), new());
        }

        // --- METHODS ---

        /// <summary>
        /// Validates that the class contains at least one DisplayValue marked value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ValidationException"></exception>
        public void Validate(object sender, EventArgs args) {
            // get the type from T
            Type type = typeof(T);

            // flags to check fromo
            var flags = BindingFlags.Instance | BindingFlags.Public;

            // check for properties
            if (
                (!type.GetProperties(flags)
                    .Any(prop => prop.IsDefined(
                        typeof(DisplayValueAttribute),
                        inherit: true
                    ))
                ) && (!type.GetFields(flags)
                    .Any(prop => prop.IsDefined(
                        typeof(DisplayValueAttribute),
                        inherit: true
                    ))
                )
            ) {
                throw new ValidationException("Displayable child had no DisplayValueAttributes");
            } 
        }
    }
}
