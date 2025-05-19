using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [TypeConverter(typeof(IDTypeConverter<IDPrimaryMark>))]
    public abstract class IDPrimaryBase : ID {

        // --- STATIC ---

        // -- VARIABLES --
        #region VARIABLES

        private static readonly string[] CurrentsPath = [JsonHandler.DataPath, "currents.json"];

        // - current ids -

        public static Dictionary<string, IDPrimaryMark> CurrentIDs { get; set; } = new();

        private static void SetUpCurrentIDs(IDPrimaryBase id) {
            // current key
            string currKey = id.GetChar().ToString();

            // if not alredy been read
            if (CurrentIDs.Count <= 0) {
                // read the value
                try {
                    CurrentIDs = JsonHandler.Read<Dictionary<string, IDPrimaryMark>>(CurrentsPath.ToList());
                } catch (FileNotFoundException) { // couldn't find file
                    CurrentIDs[currKey] = new IDPrimaryMark(id.Type, id.Value, id._instance, id.HasBeenAssigned, id.IsDeleted);
                }
            }

            // check if the key exists
            if (!CurrentIDs.ContainsKey(currKey)) {
                CurrentIDs[currKey] = new IDPrimaryMark(id.Type, id.Value, id._instance, id.HasBeenAssigned, id.IsDeleted);
            }
        }

        private static void SaveCurrents() {
            JsonHandler.Write(CurrentsPath.ToList(), CurrentIDs);
        }

        #endregion

        // -- METHODS --

        public static IDPrimaryMark MarkOf(IDPrimaryBase id) {
            if (id is IDPrimary idPrimary) {
                return idPrimary.Mark;
            } else if (id is IDPrimaryMark idPrimaryMark) {
                return idPrimaryMark;
            }
            throw new ArgumentException("could not find a mark from the provided value");
        }

        // - Conversion Methods -
        #region Conversion Methods

        public static IDTrace CreateTraceFrom(IDPrimaryBase idPrimary, object instance) {
            // create new trace mark and add to tracelist
            var newTraceMark = new IDTraceMark();
            newTraceMark.AssignIDPrimary(idPrimary, instance);

            // return reference
            return newTraceMark.AsReference();
        }

        /// <summary>
        /// gets a list of traces associated with a primary id
        /// </summary>
        /// <param name="id"> the primary id to match to </param>
        /// <returns> A list of traces </returns>
        public static List<IDTrace> GetTracesOf(IDPrimaryBase id) => TraceList[id.AsMark()]
            .Select(mark => mark.AsReference())
            .ToList();

        /// <summary>
        /// gets a list of traces associated with a primary id where the instance is of type T
        /// </summary>
        /// <typeparam name="T"> the type of Instance to select </typeparam>
        /// <param name="id"> the primary id to match to </param>
        /// <returns> A list of traces </returns>
        public static List<IDTrace> GetTracesOf<T>(IDPrimaryBase id) => TraceList[id.AsMark()]
                .Where(trace => {
                    try {
                        return (trace.Instance is T);
                    } catch (ArgumentNullException) {
                        return false;
                    }
                })
                .Select(mark => mark.AsReference())
                .ToList();

        /// <summary>
        /// gets a list of instances associated with a primary id
        /// </summary>
        /// <param name="id"> the primary id to match to </param>
        /// <returns> A list of instances </returns>
        public static List<object> GetChildrenOf(IDPrimaryBase id) => TraceList[id.AsMark()]
            .Select(mark => mark.Instance)
            .ToList();

        /// <summary>
        /// gets a list of instances associated with a primary id where the instance is of type T
        /// </summary>
        /// <typeparam name="T"> the type of Instance to select </typeparam>
        /// <param name="id"> the primary id to match to </param>
        /// <returns> A list of instances of type T </returns>
        public static List<T> GetChildrenOf<T>(IDPrimaryBase id) => TraceList[id.AsMark()]
                .Where(trace => {
                    try {
                        return (trace.Instance is T);
                    } catch (ArgumentNullException) {
                        return false;
                    }
                })
                .Select(mark => (T)mark.Instance)
                .ToList();

        #endregion

        // - Operation Basis -
        #region Operation Basis

        /// <summary>
        /// Assigns a new IDPrimary to the provided object
        /// </summary>
        public static void AssignNewID(IDPrimaryBase id, object instance, char typeCharacter) {
            if (id.Value != 0) { return; } // don't run if the value isn't 0

            // - check validity of assignment -

            // error if the value is already assigned
            if (id.HasBeenAssigned && !id.IsDeleted) { throw new ArgumentException("Provided ID is currently already assigned and cannot be re-assigned"); }

            // add to IDTypes
            if (!IDTypes.ContainsValue(typeCharacter)) {
                IDTypes[id.Type] = typeCharacter;
            }

            // - ensure primary mark exists -

            // use id as a mark
            IDPrimaryMark idPrimaryMark;
            if (id is IDPrimaryMark idCasted) {
                idPrimaryMark = idCasted;
            } else {
                idPrimaryMark = new IDPrimaryMark(id.Type, id.Value, id._instance, id.HasBeenAssigned, id.IsDeleted);
            }

            // add instance to the mark value
            idPrimaryMark.AssignInstance(instance);

            // - value setting -

            // set up current ids
            SetUpCurrentIDs(idPrimaryMark);

            // current key
            string currKey = idPrimaryMark.GetChar().ToString();

            // has been assigned
            idPrimaryMark.MarkAsAssigned(); // no longer deleted

            // set the value and increment
            CurrentIDs[currKey].Increment();
            idPrimaryMark.Value = CurrentIDs[currKey].Value;
            id.Value = CurrentIDs[currKey].Value; // value mirroring

            // check if tracelist doesn't contain key
            if (!TraceList.ContainsKey(idPrimaryMark)) {
                TraceList[idPrimaryMark] = new();
            }

            // save changes
            SaveTracelist();
            SaveCurrents();
        }

        /// <summary>
        /// Removes the provided IDPrimary
        /// </summary>
        /// <remarks>
        /// WARNING: traces to this IDPrimary will still exist unless manually deleted
        /// </remarks>
        public static void Delete(IDPrimaryBase id) {
            // set up current ids
            SetUpCurrentIDs(id);

            // prospective delete is in valid range
            if (
                (id.Value < 0) || (CurrentIDs[id.GetChar().ToString()].Value < id.Value) // range
            ) {
                throw new ArgumentException($"The specified IDPrimary is out of valid range {id.Identifier}");
            }

            // mark as deleted (technically tracelist should be saved here so we dont have to do it again)
            id.MarkAsDeleted();

            // set values to invalid (this will throw errors if the IDPrimary is tried to be used)
            id.Value = -1;
            id._type = null;

            // save the changes
            SaveCurrents();
        }

        #endregion

        // --- INSTANCE ---

        // -- VARIABLES --
        #region VARIABLES

        // identifier (json serialization changes) override
        public override string Identifier {
            get {
                int boolToInt(bool b) => b ? 1 : 0;
                return $"{boolToInt(HasBeenAssigned)}{boolToInt(IsDeleted)}{base.Identifier}";
            }
            set {
                bool charToBool(char c) => (int.Parse(c.ToString()) == 1) ? true : false;
                HasBeenAssigned = charToBool(value[0]);
                IsDeleted = charToBool(value[1]);
                base.Identifier = value[2..];
            }
        }

        // raw (base) identifier
        protected string RawIdentifier {
            get => base.Identifier;
        }

        // assignment tracking
        public abstract bool HasBeenAssigned { get; set; }

        // character
        private char? _char { get; set; }
        protected char Char {
            get {
                if (!_char.HasValue) { throw new ArgumentNullException("Attempting to access char when it hasn't been assigned"); }
                return _char.Value;
            }
            set => _char = value;
        }

        // deletion status
        public abstract bool IsDeleted { get; set; }

        #endregion

        // -- CONSTRUCTORS --
        #region CONSTRUCTORS

        // paramless req
        /// <summary>
        /// creates an empty IDPrimary object with no association to any data
        /// </summary>
        public IDPrimaryBase() { }

        // id with type
        /// <summary>
        /// creates an empty IDPrimary object of a particular type
        /// </summary>
        /// <param name="type"> The type of IDPrimary </param>
        public IDPrimaryBase(Type type, char typeCharacter) {
            Type = type;
            IDTypes[Type] = typeCharacter;
            Char = typeCharacter;
        }

        #endregion

        // -- METHODS --

        public IDPrimaryMark AsMark() => MarkOf(this);

        // -- Marking --
        #region Marking

        private void MarkAsDeleted() {
            IsDeleted = true;
            SaveTracelist();
        }

        private void MarkAsAssigned() {
            IsDeleted = false;
            HasBeenAssigned = true;
            SaveTracelist();
        }

        #endregion

        // - Conversion Mirrors -
        #region Conversion Mirrors

        public IDTrace CreateTrace(object instance) => CreateTraceFrom(this, instance);

        public List<IDTrace> GetTraces() => GetTracesOf(this);

        public List<IDTrace> GetTraces<T>() => GetTracesOf<T>(this);

        public List<object> GetChildren() => GetChildrenOf(this);

        public List<T> GetChildren<T>() => GetChildrenOf<T>(this);

        #endregion

        // - Operation Mirrors -
        #region Operation Mirrors

        public void AssignNewID(object instance) => AssignNewID(this, instance, Char);
        public void Delete() => Delete(this);

        #endregion
    }
}
