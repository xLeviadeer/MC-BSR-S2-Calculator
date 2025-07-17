using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.Utility.Identification;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class IDDisplayable : Displayable {

        // --- VARIABLES ---

        [JsonProperty("self_id")]
        public IDPrimary DisplayableID { get; set; }

        // --- CONSTRUCTORS ---

        public IDDisplayable() 
            : base() { // runs validate

            // get type character
            var property = this.GetType().GetProperty(
                "TypeCharacter", 
                BindingFlags.Static | BindingFlags.Public
            );
            if (property == null) {
                throw new InvalidOperationException($"TypeCharacter has no value set");
            }

            // get property value
            char typeCharacter = (char)property.GetValue(null)!;

            // set ID
            DisplayableID = new(this.GetType(), typeCharacter);
        }

        // --- METHODS ---

        public override void Validate(object sender, EventArgs args) {
            base.Validate(sender, args);

            // check that this class extends IDHolder
            if (this is not IIDHolder) {
                throw new InvalidOperationException($"IDDisplayable class must extend IDHolder");
            }
        }

        public void AssignNewID() => DisplayableID.AssignNewID(this);

        public void AssignInstanceID(IDDisplayable instance) => DisplayableID.AssignInstance(instance);

        // RemoveID
    }
}
