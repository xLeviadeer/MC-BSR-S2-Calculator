using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    /// <summary>
    /// Attribute class for DisplayValue; used to mark values as to-be-displayed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    internal class DisplayValueAttribute : Attribute {}

    /// <summary>
    /// Stores a value of type T in a fashion which it can be displayed in a DisplayList
    /// </summary>
    /// <typeparam name="T"> The type of value to store </typeparam>
    internal class DisplayValue<T> {

        // --- VARIABLES ---

        public T Value { get; init; }

        // --- CONSTRUCTOR ---

        public DisplayValue(T value) {
            Value = value;
        }

        // --- CASTING ---

        /// <summary>
        /// Implicit cast from T value to DisplayValue
        /// </summary>
        public static implicit operator DisplayValue<T>(T obj) {
            return new DisplayValue<T>(obj);
        }
    }
}
