using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public class NotifyingList<T> : List<T>, IFromConverter<NotifyingList<T>> {
        
        // --- VARIABLES ---

        public event EventHandler<EventArgs>? ItemsChanged;
        public event EventHandler<EventArgs>? ItemAdded;
        public event EventHandler<EventArgs>? ItemRemoved;
        public event EventHandler<EventArgs>? ItemsReset; 

        // --- CONSTRUCTOR ---

        // empty constructor
        public NotifyingList() { }

        // copy constructor
        public NotifyingList(IEnumerable<T> lst)
            : base(lst) { }

        // --- METHODS ---

        public new void Add(T item) {
            base.Add(item);
            var args = new ListChangedEventArgs(ListChangedType.ItemAdded, this.Count - 1);
            ItemsChanged?.Invoke(this, args);
            ItemAdded?.Invoke(this, args);
        }

        public new void Remove(T item) {
            var args = new ListChangedEventArgs(ListChangedType.ItemDeleted, IndexOf(item));
            base.Remove(item);
            ItemsChanged?.Invoke(this, args);
            ItemRemoved?.Invoke(this, args);
        }

        public new void RemoveAt(int index) {
            this.Remove(this[index]);
        }

        public new void Clear() {
            while (this.Count > 0) {
                RemoveAt(0);
            }
            var args = new ListChangedEventArgs(ListChangedType.Reset, -1);
            ItemsChanged?.Invoke(this, args);
            ItemsReset?.Invoke(this, args);
        }

        // ... more methods can be added as needed    
        // adding AddRange would screw up the sorting method for many ClassDataList sort overrides

        public NotifyingList<T> CopyShallow()
            => new NotifyingList<T>(this);

        public static NotifyingList<T> From(object source)
            => new NotifyingList<T>((IEnumerable<T>)source);
    }
}
