using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IDTraceMark : IDTraceBase {

        // --- CONSTRUCTORS ---

        public IDTraceMark() { }

        // clone constructor
        public IDTraceMark(Type type, int value, object? instance) {
            Type = type;
            Value = value;
            _instance = instance;
        }

        // --- CASTING ---

        public IDTrace AsReference() => ReferenceOf(this);

        public static IDTrace ReferenceOf(IDTraceBase id) {
            var newID = new IDTrace();
            newID.Value = id.Value;
            newID.Type = id.Type;
            return newID;
        }

        public static explicit operator IDTrace(IDTraceMark id) => ReferenceOf(id);
    }
}
