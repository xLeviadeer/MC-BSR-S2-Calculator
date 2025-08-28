using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]

    public abstract class ID : IComparable<ID> {
        // --- VARIABLES ---
        #region VARIABLES

        // -- Static --

        public const int PADDING_AMOUNT = 10;

        public const int ZERO_ID_VALUE = 0;
        public static readonly string ZERO_ID_IDENTIFIER = ZERO_ID_VALUE
            .ToString()
            .PadLeft(PADDING_AMOUNT, '0');

        // -- Instance --

        // - Identifier -

        [JsonProperty("id")]
        public virtual string Identifier {
            // assumes Type is correct via validation of the assignment method
            get {
                if (Value < 0) { throw new ArgumentException($"Value was not set to a valid ID", nameof(Value)); }
                Debug.WriteLine($"padded check: {valuePadded}");
                Debug.WriteLine($"value check: {Value}");
                return valuePadded;
            }
            set {
                if (value.Length != PADDING_AMOUNT) { throw new ArgumentException($"Provided Identifier was not of a valid format '{value}'; Identifier too long or too short"); }
                Value = int.Parse(value);
            }
        }

        // - Value -

        public int Value { get; set; } = 0;

        protected string valuePadded => Value.ToString().PadLeft(PADDING_AMOUNT, '0');

        #endregion

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        // json constructor
        // default constructor
        public ID() { }

        // copy constructor
        public ID(ID id) {
            Identifier = id.Identifier;
        }

        #endregion

        // --- METHODS ---

        // - To String (override) -

        public override string ToString()
            => Identifier;

        // -- Operations --

        // - Basis -
        #region Basis

        /// <summary>
        /// Gets the first child that extends this class
        /// </summary>
        /// <param name="id"> An ID instance to get the first class extension on </param>
        /// <returns> The type of the first class extension, null if  </returns>
        private static Type? _getFirstExtension(ID id) {
            // set the current type to the highest type
            Type currentType = id.GetType();

            // move down base types until found
            while (currentType != typeof(object)) {
                // if the base type is BareID then current must be it's first extension
                if (currentType!.BaseType == typeof(ID)) {
                    return currentType; 
                }
                
                // check for invalid base type
                if (currentType.BaseType == null) {
                    throw new ArgumentNullException("BaseType was null");
                }

                // move down one base
                currentType = currentType.BaseType;
            }

            // impossible condition
            throw new InvalidOperationException("ID didn't extend anything, impossible instance");
        }

        /// <summary>
        /// Compares two ID objects for equality
        /// </summary>
        /// <returns> A boolean, true if equal </returns>
        public static bool Compare(ID? idOne, ID? idTwo) {
            // null comparisons
            if (idOne is null && idTwo is null) { return true; }
            if (idOne is null || idTwo is null) { return false; }

            // ID's must have the same first child to be equal
            if (_getFirstExtension(idOne) != _getFirstExtension(idTwo)) { return false; }

            // check if the values are equal
            return (idOne.Value == idTwo.Value);
        }

        /// <summary>
        /// Compares two ID objects if the first is greater than the second
        /// </summary>
        /// <returns> A boolean, true if the first is greater than the second </returns>
        public static bool GreaterThan(ID idOne, ID idTwo) => idOne.Value > idTwo.Value;

        /// <summary>
        /// Compares two ID objects if the first is greater than or equal to the second
        /// </summary>
        /// <returns> A boolean, true if the first is greater than or equal to the second </returns>
        public static bool GreaterThanOrEqual(ID idOne, ID idTwo) => idOne.Value >= idTwo.Value;

        /// <summary>
        /// Compares two ID objects if the first is less than the second
        /// </summary>
        /// <returns> A boolean, true if the first is less than the second </returns>
        public static bool LessThan(ID idOne, ID idTwo) => idOne.Value < idTwo.Value;

        /// <summary>
        /// Compares two ID objects if the first is less than or equal to the second
        /// </summary>
        /// <returns> A boolean, true if the first is less than or equal to the second </returns>
        public static bool LessThanOrEqual(ID idOne, ID idTwo) => idOne.Value <= idTwo.Value;

        #endregion

        // - Mirrors -
        #region Mirrors

        public virtual bool Compare(ID? id) => Compare(this, id);
        public bool GreaterThan(ID id) => GreaterThan(this, id);
        public bool GreaterThanOrEqual(ID id) => GreaterThanOrEqual(this, id);
        public bool LessThan(ID id) => LessThan(this, id);
        public bool LessThanOrEqual(ID id) => LessThanOrEqual(this, id);

        #endregion

        // - Real -
        #region Real

        public int CompareTo(ID? other) {
            if (other == null) return 1;
            return this.Identifier.CompareTo(other.Identifier);
        }

        public override bool Equals(object? obj) {
            return (
                (obj is ID)
                && Compare((ID?)obj)
            );
        }

        public override int GetHashCode() {
            try {
                return Identifier.GetHashCode();
            } catch (ArgumentNullException) { // tolerates deleted IDs
                return ZERO_ID_IDENTIFIER.GetHashCode();
            }
        }

        public static bool operator >(ID idOne, ID idTwo)
            => GreaterThan(idOne, idTwo);

        public static bool operator >=(ID idOne, ID idTwo)
            => GreaterThanOrEqual(idOne, idTwo);

        public static bool operator <(ID idOne, ID idTwo)
            => LessThan(idOne, idTwo);

        public static bool operator <=(ID idOne, ID idTwo)
            => LessThanOrEqual(idOne, idTwo);

        public static bool operator ==(ID? idOne, ID? idTwo) {
            if (ReferenceEquals(idOne, idTwo)) { return true; }
            if (idOne is null || idTwo is null) return false;
            return idOne.Compare(idTwo); // using this version of compare will make sure it uses the overriden version
        }

        public static bool operator !=(ID idOne, ID idTwo)
            => !(idOne == idTwo);

        #endregion
    }
}
