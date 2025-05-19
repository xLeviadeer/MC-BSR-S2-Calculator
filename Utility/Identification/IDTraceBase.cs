using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [TypeConverter(typeof(IDTypeConverter<IDTraceMark>))]
    public abstract class IDTraceBase : ID {
        // --- STATIC ---

        // -- METHODS --

        public static IDTraceMark MarkOf(IDTraceBase id) {
            if (id is IDTrace idTrace) {
                return idTrace.Mark;
            } else if (id is IDTraceMark idTraceMark) {
                return idTraceMark;
            }
            throw new ArgumentException("could not find a mark from the provided value");
        }

        // - Conversion Methods -
        #region Conversion Methods

        /// <summary>
        /// gets the associated primary id
        /// </summary>
        /// <param name="id"> the trace id to match to </param>
        /// <returns> A primary id </returns>
        public static IDPrimary GetPrimaryOf(IDTraceBase id) {
            foreach (var kvp in TraceList) {
                IDPrimaryMark idPrimary = kvp.Key;
                List<IDTraceMark> idTraces = kvp.Value;

                foreach (var idTrace in idTraces) {
                    if (idTrace == id) {
                        return idPrimary.AsReference();
                    }
                }
            }

            // couldn't be found
            throw new ArgumentException("Provided IDTraceMark doesn't exist in the TraceList");
        }

        /// <summary>
        /// gets the instance associated with the associated primary id
        /// </summary>
        /// <param name="id"> the trace id to match to </param>
        /// <returns> An instance </returns>
        public static object GetParentOf(IDTraceBase id) => GetPrimaryOf(id).Instance;

        /// <summary>
        /// gets the instance associated with the associated primary id
        /// </summary>
        /// <typeparam name="T"> the type of Instance to select </typeparam>
        /// <param name="id"> the trace id to match to </param>
        /// <returns> An instance of type T </returns>
        public static T GetParentOf<T>(IDTraceBase id) {
            var instance = GetPrimaryOf(id).Instance;
            if (instance is T castedInstance) { return castedInstance; }
            throw new ArgumentException($"could not get parent of type T because parent was not of type T: {instance.GetType()}");
        }

        #endregion

        // - Operation Basis -
        #region Operation Basis

        /// <summary>
        /// Assigns a new IDPrimary to the provided IDTraceMark
        /// </summary>
        public static void AssignIDPrimary(IDPrimaryBase idPrimary, IDTraceBase idTrace, object instance) {
            if (idTrace.Value != 0) { return; } // don't run if the value isn't 0

            // make sure idPrimary isn't deleted
            if (idPrimary.IsDeleted) { throw new ArgumentException("Cannot assign to idPrimary because it is deleted"); }

            // create a trace mark clone
            IDTraceMark idTraceMark = new IDTraceMark(idPrimary.Type, idPrimary.Value, instance);

            // check if original is already in the tracelist
            try {
                // try to delete it
                idTrace.AsMark().Delete(); 
            } catch (ArgumentException) {
                // couldn't be deleted, must have not existed
            }

            // value mirroring
            idTrace.Value = idPrimary.Value;
            idTrace.Type = idPrimary.Type;

            // add trace to primary
            TraceList[idPrimary.AsMark()].Add(idTraceMark);

            // save changes
            SaveTracelist();
        }

        /// <summary>
        /// Removes the provided IDPrimary
        /// </summary>
        public static void Delete(IDTraceBase id) {
            // use id as a mark
            IDTraceMark idTraceMark = id.AsMark();

            // find trace and delete it
            foreach (var kvp in TraceList) { // for every primary key
                IDPrimaryMark idPrimary = kvp.Key;
                List<IDTraceMark> idTraces = kvp.Value;

                foreach (var trace in TraceList[idPrimary]) { // for every primary trace id
                    // if the primary id contains a trace which is the exact same reference
                    if (trace == idTraceMark) {
                        try {
                            if (trace.Instance == idTraceMark.Instance) {
                                // remove, save and quit
                                idTraces.Remove(idTraceMark);
                                SaveTracelist();
                                return;
                            }
                        } catch (ArgumentNullException) {
                            // didn't match
                        }
                    }
                } 
            }

            // couldn't be found
            throw new ArgumentException("Provided IDTraceMark doesn't exist in the TraceList");
        }

        #endregion

        // --- INSTANCE ---

        // -- CONSTRUCTORS --
        #region CONSTRUCTORS

        // paramless req
        /// <summary>
        /// creates an empty IDPrimary object with no association to any data
        /// </summary>
        public IDTraceBase() { }

        #endregion

        // -- METHODS --

        public IDTraceMark AsMark() => MarkOf(this);

        // - Conversion Mirrors -
        #region Conversion Mirrors

        public IDPrimary GetPrimary() => GetPrimaryOf(this);

        public object GetParent() => GetParentOf(this);

        public T GetParent<T>() => GetParentOf<T>(this);

        #endregion

        // - Operation Mirrors -
        #region Operation Mirrors

        public void AssignIDPrimary(IDPrimaryBase id, object instance) => AssignIDPrimary(id, this, instance);

        public void Delete() => Delete(this);

        #endregion
    }
}
