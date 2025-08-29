using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract partial class RelationalID : ID, IInstanceID {

        // --- STATIC ---

        // -- VARIABLES --
        #region VARIABLES

        protected static readonly string[] TraceListPath = [JsonHandler.DATA_PATH, "tracelist.json"];

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

        // - Try Find Parent or Self ID -

        /// <summary>
        /// Intakes and ID and attempts to find
        /// - if it's a trace: it's parent
        /// - if it's a primary: itself
        /// </summary>
        /// <returns> An IDPrimary corresponding to the parent of this trace or itself </returns>
        public static IDPrimary? FindParentOrSelfID(RelationalID id) 
            => id switch {
                IDPrimary idPrimary => idPrimary,
                IDPrimaryMark idPrimaryMark => idPrimaryMark.AsReference(),
                IDTraceBase idTrace => idTrace.GetPrimary(),
                _ => throw new InvalidOperationException($"Value for {nameof(id)}, '{id}' is not a valid ID")
            };

        // - Construct Highest Identifier -
        #region Construct Highest Identifier

        /// <summary>
        /// Constructs the highest identifier for this current object;
        /// Takes all valid portions of the identifier and displays them 
        /// while displaying error portions for invalid parts of the identifier
        /// </summary>
        /// <param name="id"> The ID to construct the highest identifier on </param>
        /// <returns> A string with the most valid information that could be created from this ID </returns>
        public static string ConstructHighestIdentifier(RelationalID? id) {
            const string NULL_ID = "<null ID>";
            const string TYPE_NOT_FOUND = "<unable to find Type>";

            // if null ID
            if (id is null) {
                return NULL_ID;
            }

            // try to get whole identifier
            try {
                return id.Identifier;
            } catch (Exception e) when (
                (e is ArgumentNullException)
                || (e is ArgumentException)
            ) {
                // try to get type
                string typeString;
                try {
                    typeString = id.GetChar().ToString();
                } catch (ArgumentNullException) {
                    typeString = TYPE_NOT_FOUND;
                }

                // return
                return $"{typeString}{id.valuePadded}";
            }
        }

        #endregion

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

        // --- INSTANCE ---

        // -- VARIABLES --
        #region VARIABLES

        // id write build
        public override string Identifier {
            // assumes Type is correct via validation of the assignment method
            get {
                ArgumentNullException.ThrowIfNull(Type, nameof(Type));
                if (Value < 0) { throw new ArgumentException($"Value was not set to a valid ID", nameof(Value)); }
                return $"{GetCharFromType(Type)}{base.Identifier}";
            }
            set {
                if (value.Length != (PADDING_AMOUNT + 1)) { throw new ArgumentException($"Provided Identifier was not of a valid format '{value}'; Identifier too long or too short"); }
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
        public RelationalID() { }

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

        // - Construct Highest Identifier -
        #region Construct Highest Identifier

        /// <summary>
        /// Constructs the highest identifier for this current object;
        /// Takes all valid portions of the identifier and displays them 
        /// while displaying error portions for invalid parts of the identifier
        /// </summary>
        /// <returns> A string with the most valid information that could be created from this ID </returns>
        public virtual string ConstructHighestIdentifier()
            => ConstructHighestIdentifier(this);

        #endregion

        // - Conversion Mirrors  -
        #region Conversion Mirrors

        public char GetChar() {
            return GetCharFromType(this.Type);
        }

        #endregion

        // - Operation Mirrors -
        #region Operation Mirrors

        public override bool Compare(ID? bareId) {
            bool bareCheck = ID.Compare(this, bareId);
            if (bareId is RelationalID id) {
                return (
                    bareCheck
                    && (this._type == id._type)
                );
            }
            return bareCheck;
        }
        
        #endregion
    }
}
