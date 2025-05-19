using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IDTrace : IDTraceBase {
        // --- VARIABLES ---

        // mark accessor
        public IDTraceMark Mark {
            get {
                // search the tracelist for an id which matches
                foreach (IDPrimaryMark primaryMark in TraceList.Keys) { // for every primary
                    foreach (IDTraceMark traceMark in TraceList[primaryMark]) { // for every trace
                        if (traceMark == this) {
                            return traceMark;
                        }
                    }
                }

                // if mark didn't exist then add one
                return new IDTraceMark();
            } 
        }

        // identifier
        public new string Identifier {
            get => Mark.Identifier;
            set => Mark.Identifier = value;
        }

        // instance
        public new object Instance {
            get => Mark.Instance;
            set => Mark.Instance = value;
        }

        // --- CONSTRUCTORS ---

        public IDTrace() { }

        // --- METHODS ---

        public override void AssignInstance(object instance) {
            Mark.AssignInstance(instance);
        }
    }
}
