using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    /// <summary>
    /// Regular DisplayValue class, extension with a constructor
    /// </summary>
    /// <typeparam name="T"> DisplayObject type T; T must be a type of FrameworkElement </typeparam>
    /// <typeparam name="U"> The type of value this class holds </typeparam>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BoundDisplayValue<T, U> : DisplayValueBase, INotifyPropertyChanged
        where T : FrameworkElement {

        // --- VARIABLES ---

        // - Value Storage -

        private U _value;

        [JsonProperty("value")]
        public U Value {
            get => _value;
            set {
                if (!Equals(_value, value)) { // value not the same
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        // - Property Changes -

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // - Display Object -

        public override FrameworkElement DisplayObject { get; }

        // --- CONSTRUCTOR ---

        public BoundDisplayValue(T displayObject, DependencyProperty targetProperty, U defaultValue, EventHandler<EventArgs>? eventListener=null, ContextMenu? rightClickMenu = null)
            : base(eventListener, rightClickMenu) {
            // set DisplayObject
            DisplayObject = displayObject;

            // create binding to Value
            var binding = new Binding(nameof(Value)) {
                Source = this,
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(displayObject, targetProperty, binding);

            // set default
            Value = defaultValue;
        }

        // --- CASTING ---

        public static implicit operator U(BoundDisplayValue<T, U> value) {
            return value.Value;
        }
    }
}
