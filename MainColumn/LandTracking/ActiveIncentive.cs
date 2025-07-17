using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public record ActiveIncentive {
        // --- VARIABLES ---
        #region VARIABLES

        // - Info Target -

        public required IIncentiveInfoHolder InfoTarget { get; init; }

        // - Name -

        [JsonProperty("name")]
        public required string Name { get; init; }

        // - Total -

        public virtual double Total => InfoTarget.FindByName(Name).Value;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public ActiveIncentive() {}

        [SetsRequiredMembers]
        public ActiveIncentive(IIncentiveInfoHolder infoTarget, string name) {
            InfoTarget = infoTarget;
            Name = name;
        }

        #endregion

        // --- METHODS ---

        public IncentiveInfo FindInfo() 
            => InfoTarget.FindByName(Name);
    }
}
