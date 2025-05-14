using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {

    /// <summary>
    /// class which *can* hold an event listener function but doesn't have to
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal abstract class OptionalEventHolder {

        // --- VARIABLES ---

        // overrides the check for IsHoldingEvent
        protected bool? IsHoldingEventOverride { get; init; } = null;

        public bool IsHoldingEvent {
            get {
                if (IsHoldingEventOverride == null) {
                    // reflect to get method
                    var methodToCheck = GetType().GetMethod(
                        nameof(HeldListener),
                        BindingFlags.Instance | BindingFlags.Public
                    );

                    // check if overriden and return
                    return methodToCheck.DeclaringType != typeof(OptionalEventHolder);
                }

                // return override
                return (bool)IsHoldingEventOverride;
            }
        }

        // --- METHODS ---

        public virtual void HeldListener(object? sender, EventArgs args) {
            throw new NotImplementedException($"HeldListener wasn't extended or called the base case");
        }

        // --- CASTING ---

        public static implicit operator EventHandler<EventArgs>(OptionalEventHolder optionalEventHolder) {
            return optionalEventHolder.HeldListener;
        }
    }
}
