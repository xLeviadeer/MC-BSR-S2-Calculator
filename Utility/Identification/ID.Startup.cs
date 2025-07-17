using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {
    
    public interface IIDHolder {

        public abstract static char TypeCharacter { get; }

    }

    public abstract partial class ID : IComparable<ID> {

        /// <summary>
        /// Ensures IDTypes will contain all types: chars when method completes.
        /// </summary>
        /// <remarks>
        /// Classes participating in intitialization must be marked as IDHolder(Attribute) AND extend IIDHolder
        /// </remarks>
        public static void InitializeIDTypes() {
            // for all classes
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type == typeof(IIDHolder)) { continue; } // skip itself

                // check if implements interface
                bool implementsInterface = typeof(IIDHolder).IsAssignableFrom(type);
                if (implementsInterface) {
                    // get value
                    object? value = type.GetProperty(
                        "TypeCharacter",
                        (BindingFlags.Public | BindingFlags.Static)
                    )?.GetValue(null);

                    // if value not set 
                    if (value is null) {
                        throw new InvalidOperationException($"{type.Name} did not set TypeCharacter");
                    }

                    // assign value to IDTypes
                    IDTypes[type] = (char)value;
                }
            }
        }
    }
}
