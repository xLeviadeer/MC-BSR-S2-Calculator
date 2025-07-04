using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    /// <summary>
    /// Allows the Tracelist to be compared with mutable keys
    /// </summary>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    where T : class {
        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract partial class ID : IComparable<ID> {

        // --- STATIC ---

        // -- VARIABLES --
        #region VARIABLES

        protected static readonly string[] TraceListPath = [JsonHandler.DataPath, "tracelist.json"];

        // - trace list -

        private static bool TraceListHasBeenBuilt { get; set; } = false;

        private static MutableKeysDictionary<IDPrimaryMark, List<IDTraceMark>> _tracelist { get; set; } = new();

        public static MutableKeysDictionary<IDPrimaryMark, List<IDTraceMark>> TraceList {
            get {
                if (
                    (!TraceListHasBeenBuilt) 
                    && (_tracelist.Count <= 0)
                ) {
                    SetUpTraceList();
                }
                return _tracelist;
            }
            protected set => _tracelist = value;
        }

        private static void SetUpTraceList() {
            TraceListHasBeenBuilt = true;

            // read the value
            try {
                _tracelist = JsonHandler.Read<MutableKeysDictionary<IDPrimaryMark, List<IDTraceMark>>>(TraceListPath.ToList());
            } catch (FileNotFoundException) { // couldn't find file
                // tracelist will start empty
            }
        }

        protected static void SaveTracelist() {
            JsonHandler.Write(TraceListPath.ToList(), TraceList);
        }

        // - id types -

        protected static Dictionary<Type, char> IDTypes { get; set; } = new();

        #endregion

        // -- METHODS --

        // - Startup -

        // - Conversion Methods -
        #region Conversion Methods

        /// <summary>
        /// Gets a Type from a Character 
        /// </summary>
        /// <param name="typeCharacter"> The character to find a type from </param>
        /// <returns> The associated type </returns>
        /// <exception cref="ArgumentException"> Thrown if the character doesn't exist in the types dictionary </exception>
        public static Type GetTypeFromChar(char typeCharacter) {
            // if doesn't exist in dictionary
            if (!IDTypes.ContainsValue(typeCharacter)) {
                throw new ArgumentException($"typeCharacter doesn't exist in the IDTypes dictionary", nameof(typeCharacter));
            }

            // find key from value
            return IDTypes.First(kvp => kvp.Value == typeCharacter).Key;
        }

        /// <summary>
        /// Gets a Character from a Type 
        /// </summary>
        /// <param name="type"> The type to find a character from </param>
        /// <returns> The associated character </returns>
        /// <exception cref="ArgumentException"> Thrown if the type doesn't exist in the types dictionary </exception>
        public static char GetCharFromType(Type type) {
            // if doesn't exist in dictionary
            if (!IDTypes.ContainsKey(type)) {
                throw new ArgumentException($"type doesn't exist in the IDTypes dictionary", nameof(type));
            }

            // find value from key
            return IDTypes[type];
        }

        #endregion

        // - Operation Basis -
        #region Operation Basis

        /// <summary>
        /// Compares two ID objects for equality
        /// </summary>
        /// <returns> A boolean, true if equal </returns>
        public static bool Compare(ID idOne, ID idTwo) {
            return (
                (idOne.Type == idTwo.Type)
                && (idOne.Value == idTwo.Value)
            );
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

        // --- INSTANCE ---

        // -- VARIABLES --
        #region VARIABLES

        // id write build
        [JsonProperty("id")]
        public virtual string Identifier {
            // assumes Type is correct via validation of the assignment method
            get {
                ArgumentNullException.ThrowIfNull(Type, nameof(Type));
                if (Value < 0) { throw new ArgumentException($"Value was not set to a valid ID", nameof(Value)); }
                return $"{GetCharFromType(Type)}{Value.ToString().PadLeft(10, '0')}";
            }
            set {
                if (value.Length != 11) { throw new ArgumentException($"Provided Identifier was not of a valid format '{value}'; Identifier too long or too short"); }
                Type = GetTypeFromChar(value[0]);
                Value = int.Parse(value[1..]);
            }
        }

        // id type
        public Type? _type;
        public Type Type {
            get {
                if (_type == null) { throw new ArgumentNullException(nameof(_type), "Attemping to access type when it hasn't been assigned"); }
                return _type;
            }
            set => _type = value;
        }

        // id value
        public virtual int Value { get; set; } = 0;

        // associated instance
        protected object? _instance { get; set; } = null;
        public object Instance {
            get {
                if (_instance == null) { throw new ArgumentNullException(nameof(Instance), $"Instance of '{Identifier}' is not set to a value. Has it been constructed yet?"); }
                return _instance;
            } 
            set => _instance = value;
        }

        #endregion

        // -- CONSTRUCTORS --
        #region CONSTRUCTORS

        // paramless req
        /// <summary>
        /// creates an empty ID object with no association to any data
        /// </summary>
        public ID() { }

        #endregion

        // -- METHODS --

        public void Increment() => Value += 1;

        /// <summary>
        /// assigns an instance to the class
        /// </summary>
        /// <param name="instance"> some type of associated object </param>
        public virtual void AssignInstance(object instance) {
            Instance = instance;
        }

        // - Conversion Mirrors  -
        #region Conversion Mirrors

        public char GetChar() {
            return GetCharFromType(this.Type);
        }

        public override string ToString() {
            return this.Identifier;
        }

        #endregion

        // - Operation Mirrors -
        #region Operation Mirrors

        public bool Compare(ID id) => Compare(this, id);
        public bool GreaterThan(ID id) => GreaterThan(this, id);
        public bool GreaterThanOrEqual(ID id) => GreaterThanOrEqual(this, id);
        public bool LessThan(ID id) => LessThan(this, id);
        public bool LessThanOrEqual(ID id) => LessThanOrEqual(this, id);

        #endregion

        // --- OPERATIONS ---
        #region OPERATIONS

        public int CompareTo(ID? other) {
            if (other == null) return 1;
            return this.Identifier.CompareTo(other.Identifier);
        }

        public override bool Equals(object? obj) {
            return obj is ID id && Compare((ID)obj);
        }

        public override int GetHashCode() {
            return Identifier.GetHashCode();
        }

        public static bool operator >(ID idOne, ID idTwo) {
            return GreaterThan(idOne, idTwo);
        }

        public static bool operator >=(ID idOne, ID idTwo) {
            return GreaterThanOrEqual(idOne, idTwo);
        }

        public static bool operator <(ID idOne, ID idTwo) {
            return LessThan(idOne, idTwo);
        }

        public static bool operator <=(ID idOne, ID idTwo) {
            return LessThanOrEqual(idOne, idTwo);
        }

        public static bool operator ==(ID? idOne, ID? idTwo) {
            if (ReferenceEquals(idOne, idTwo)) return true;
            if (idOne is null || idTwo is null) return false;
            return Equals(idOne, idTwo);
        }

        public static bool operator !=(ID idOne, ID idTwo) {
            return !(idOne == idTwo);
        }

        #endregion
    }
}
