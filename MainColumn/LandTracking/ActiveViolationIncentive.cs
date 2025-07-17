using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public record ActiveViolationIncentive : ActiveIncentive {
        // --- VARIABLES ---

        // - Violation Type -

        [JsonProperty("type")]
        public required ViolationIncentive.ViolationTypes ViolationType { get; init; }

        // - Violation Count -

        [JsonProperty("violations")]
        public required int ViolationCount { get; init; }

        // - Total -

        public override double Total => base.Total * ViolationCount;

        // --- CONSTRUCTOR ---

        public ActiveViolationIncentive() {}

        [SetsRequiredMembers]
        public ActiveViolationIncentive(
            IIncentiveInfoHolder infoTarget, 
            string name, 
            ViolationIncentive.ViolationTypes violationType, 
            int violationCount
        ) : base(infoTarget, name) {
            ViolationType = violationType;
            ViolationCount = violationCount;
        }
    }
}
