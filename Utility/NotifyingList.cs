using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public class NotifyingList<T> : List<T> {
        
        // --- VARIABLES ---

        public event EventHandler<EventArgs>? ItemsChanged;
        public event EventHandler<EventArgs>? ItemAdded;
        public event EventHandler<EventArgs>? ItemRemoved;

        // --- METHODS ---

        public new void Add(T item) {
            base.Add(item);
            var args = new ListChangedEventArgs(ListChangedType.ItemAdded, this.Count - 1);
            ItemsChanged?.Invoke(this, args);
            ItemAdded?.Invoke(this, args);
        }

        // ... more methods can be added as needed    
        // adding AddRange would screw up the sorting method for many ClassDataList sort overrides
    }
}
