using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    /// <summary>
    /// class which *can* hold an event listener function but doesn't have to
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class OptionalMouseClickEventHolder {

        // --- VARIABLES ---

        // - Right Click Context Menu -

        public bool IsHoldingRightClick {
            get {
                // check holding status
                return ((RightClickMenu != null) && RightClickMenu.Items.Count > 0);
            }
        }

        // set to a value to use right click menu
        public ContextMenu? RightClickMenu { get; set; } = new();

        // - Left Click Event -
        public bool? IsHoldingLeftClickOverride { get; set; } = null;

        public bool IsHoldingLeftClick {
            get {
                if (IsHoldingLeftClickOverride == null) {
                    // reflect to get method
                    var methodToCheck = GetType().GetMethod(
                        nameof(HeldLeftClickListener),
                        BindingFlags.Instance | BindingFlags.Public
                    );

                    // check if overriden and return
                    return methodToCheck.DeclaringType != typeof(OptionalMouseClickEventHolder);
                }

                // return override
                return (bool)IsHoldingLeftClickOverride;
            }
        }

        // --- METHODS ---

        // override to use left click
        public virtual void HeldLeftClickListener(object? sender, EventArgs args) {
            throw new NotImplementedException($"HeldListener wasn't extended or called the base case");
        }

        // --- CASTING ---

        public static implicit operator EventHandler<EventArgs>(OptionalMouseClickEventHolder optionalEventHolder) {
            return optionalEventHolder.HeldLeftClickListener;
        }
    }
}
