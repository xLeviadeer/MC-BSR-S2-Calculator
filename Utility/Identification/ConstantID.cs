using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {
    public class ConstantID : ID, IInstanceID {

        // --- STATIC ---

        // -- Variables --
        #region Variables

        // - Next Constant Value -

        private static readonly string[] NEXT_CONSTANT_VALUE_PATH = [JsonHandler.DATA_PATH, "constants.json"];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "backing value")]
        private static int? __nextConstantvalue = null;
        private static int _nextConstantValue {
            get {
                // null check
                if (__nextConstantvalue is null) {
                    // try to read from file
                    try {
                        __nextConstantvalue = JsonHandler.Read<List<int>>(NEXT_CONSTANT_VALUE_PATH)[0];
                    } catch (Exception err) when (
                        (err is FileNotFoundException)
                        || (err is ArgumentException) 
                    ) {
                        // create new
                        __nextConstantvalue = 0;

                        // write file
                        JsonHandler.Write<List<int>>(NEXT_CONSTANT_VALUE_PATH, [(int)__nextConstantvalue]);
                    }
                }

                // return
                return (int)__nextConstantvalue;
            }
            set {
                // set value
                __nextConstantvalue = value;

                // write file
                JsonHandler.Write<List<int>>(NEXT_CONSTANT_VALUE_PATH, [value]);
            }
        }

        private static int _getNextConstantValue() {
            int returnValue = _nextConstantValue;
            _nextConstantValue += 1;
            return returnValue;
        }

        #endregion

        // -- Methods --
        #region Methods

        // - Assign New ID -

        public static void AssignNewID(ConstantID id, object instance) {
            if (id.Value != 0) { return; } // don't run if the value isn't 0

            // assign value
            id.Value = _getNextConstantValue();

            // assign instance
            id.AssignInstance(instance);
        }

        // - Copy -

        public static ConstantID Copy(ConstantID id)
            => new ConstantID(id);

        public static ConstantID Copy(ConstantID id, object newInstance) {
            ConstantID copy = Copy(id);
            copy.AssignInstance(newInstance);
            return copy;
        }

        #endregion

        // --- INSTANCE ---

        // -- Variables --
        #region Variables

        // - Instance -

        public object Instance {
            get {
                if (field == null) { throw new ArgumentNullException(nameof(Instance), $"Instance of '{Identifier}' is not set to a value. Has it been assigned yet?"); }
                return field;
            }
            set => field = value;
        } = null;

        #endregion

        // -- Constructor --
        #region Constructor

        // json constructor
        // base constructor
        public ConstantID() : base() {}

        // copy constructor
        public ConstantID(ConstantID copy) : base(copy) {}

        #endregion

        // -- Methods --
        #region Methods

        // - Assign Instance -

        public void AssignInstance(object instance) {
            Instance = instance;
        }

        // - Assign New ID -

        public void AssignNewID(object instance)
            => AssignNewID(this, instance);

        // - Copy -

        public ConstantID Copy()
            => Copy(this);

        public ConstantID Copy(object newInstance) 
            => Copy(this, newInstance);

        #endregion
    }
}
