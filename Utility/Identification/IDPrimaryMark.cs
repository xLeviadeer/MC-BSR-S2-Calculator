using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class IDPrimaryMark : IDPrimaryBase {

        // --- VARIABLES ---

        // - assignment tracking - 

        private bool _hasBeenAssigned { get; set; } = false;

        public override bool HasBeenAssigned {
            get => _hasBeenAssigned;
            set {
                if (value != _hasBeenAssigned) {
                    _hasBeenAssigned = value;
                }
            }
        }

        // - deletion status -
        private bool _isDeleted { get; set; } = false;

        public override bool IsDeleted {
            get => _isDeleted;
            set {
                if (value != _isDeleted) {
                    _isDeleted = value;
                }
            }
        }

        // --- CONSTRUCTORS ---

        public IDPrimaryMark() { }

        public IDPrimaryMark(Type type, char typeCharacter) 
            : base(type, typeCharacter) { }

        // clone constructor
        public IDPrimaryMark(Type type, int value, object? instance, bool hasBeenAssigned, bool isDeleted) 
            : base(type, GetCharFromType(type)) {
            Value = value;
            _instance = instance;
            HasBeenAssigned = hasBeenAssigned;
            IsDeleted = isDeleted;
        }

        // --- CASTING ---

        public IDPrimary AsReference() => ReferenceOf(this);

        public static IDPrimary ReferenceOf(IDPrimaryMark id) {
            var newID = new IDPrimary();
            newID.Value = id.Value;
            newID.Type = id.Type;
            return newID;
        }

        public static explicit operator IDPrimary(IDPrimaryMark id) => ReferenceOf(id);
    }
}
