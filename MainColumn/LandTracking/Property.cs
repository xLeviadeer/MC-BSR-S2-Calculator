using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Identification;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class Property : Displayable {

        // --- VARIABLES ---
        #region VARIABLES

        // -- STATIC --

        // - Property Types - 

        public const string Public = "public";
        public const string Provisioned = "provisioned";
        public const string Unowned = "unowned";

        public static ImmutableList<string> GovernmentalPropertyTypes { get; set; } = [
            Public,
            Provisioned,
            Unowned
        ];

        public const string Private = "private";
        public const string SharedPrivate = "shared private";
        public const string Owned = "owned";

        public static ImmutableList<string> PlayerPropertyTypes { get; set; } = [
            Private,
            SharedPrivate,
            Owned
        ];

        public static ImmutableList<string> AllPropertyTypes = [
            Public,
            SharedPrivate,
            Private,
            Unowned,
            Provisioned,
            Owned
        ];

        // -- INSTANCE --

        // - ID -

        [JsonProperty("property_id")]
        public IDPrimary PropertyID { get; set; } = new(typeof(Property), 'o');

        // - Name -

        [JsonProperty("name")]
        [DisplayValue("Name", 0, GridUnitType.Star)]
        public BoundDisplayValue<Label, string> Name { get; set; }

        #endregion

        // --- CONSTRUCTOR ---

        #region CONSTRUCTOR

        public Property() {
            
        }

        #endregion
    }
}
