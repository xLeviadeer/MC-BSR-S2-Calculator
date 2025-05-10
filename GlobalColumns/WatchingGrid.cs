using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns
{
    /// <summary>
    /// EventArgs type for changes to children
    /// </summary>
    public class ChildChangedEventArgs : EventArgs {
        public DependencyObject? Added { get; } // added value
        public DependencyObject? Removed { get; } // removed value

        /// <summary>
        /// Constructor values for children changes
        /// </summary>
        /// <param name="added"> What was added </param>
        /// <param name="removed"> What was removed </param>
        public ChildChangedEventArgs(DependencyObject? added, DependencyObject? removed) {
            Added = added;
            Removed = removed;
        }
    }

    /// <summary>
    /// Grid class with an event that runs when it's children are updated (added or removed)
    /// </summary>
    class WatchingGrid : Grid {
        /// <summary>
        /// Event for when the children of the grid has been updated (added or removed)
        /// </summary>
        public event EventHandler<ChildChangedEventArgs>? ChildrenChanged;

        /// <summary>
        /// Invokes the ChildChanged event
        /// </summary>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            // invoke event if changed
            if ((visualAdded != null) || (visualRemoved != null)) {
                ChildrenChanged?.Invoke(this, new ChildChangedEventArgs(visualAdded, visualRemoved));
            }
        }
    }
}
