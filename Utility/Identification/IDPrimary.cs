using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IDPrimary : IDPrimaryBase {

        // --- VARIABLES ---

        // mark accessor
        public IDPrimaryMark Mark {
            get {
                // search the tracelist for an id which matches
                foreach (IDPrimaryMark mark in TraceList.Keys) {
                    if (mark == this) {
                        return mark;
                    }
                }

                // if mark didn't exist then add one
                return new IDPrimaryMark(Type, Char);
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

        // assignment tracking
        public override bool HasBeenAssigned {
            get => Mark.HasBeenAssigned;
            set => Mark.HasBeenAssigned = value;
        }

        // deletion status
        public override bool IsDeleted {
            get => Mark.IsDeleted;
            set => Mark.IsDeleted = value;
        }

        // --- CONSTRUCTORS ---

        public IDPrimary() { }

        public IDPrimary(Type type, char typeCharacter)
            : base(type, typeCharacter) { }

        // --- METHODS ---

        public override void AssignInstance(object instance) {
            Mark.AssignInstance(instance);
        }
    }
}
