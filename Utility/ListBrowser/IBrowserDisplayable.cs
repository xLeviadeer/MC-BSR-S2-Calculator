using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.ListBrowser {
    public interface IBrowserDisplayable<Self, DisplayContent>
        where Self : Displayable, IBrowserDisplayable<Self, DisplayContent>
        where DisplayContent : FrameworkElement, ISwitchManaged {

        // --- ENSURANCES ---

        // -- Variables --

        private Self AsParent => ((Self)this);

        public abstract IBrowserDisplayable<Self, DisplayContent> AsBrowserDisplayable { get; }

        public abstract static DisplayContent? DisplayedContent { get; }

        public abstract static event EventHandler<EventArgs>? DisplayedContentChanged;

        public abstract static Self? LastSelectedDisplayable { get; }

        // -- Methods --

        public abstract void HeldLeftClickListener(object? sender, EventArgs args);

        // --- DEFAULTS ---

        // -- Methods --

        // - Ensure Object Validity -

        public void CheckBrowserValidity() {
            if (!AsParent.IsHoldingLeftClick) {
                throw new ArgumentException($"Displayable object does not hold a valid left click event");
            }
        }
    }
}
