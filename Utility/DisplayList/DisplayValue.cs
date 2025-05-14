using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {

    /// <summary>
    /// Regular DisplayValue class, extension with a constructor
    /// </summary>
    /// <typeparam name="T"> DisplayObject type T; T must be a type of FrameworkElement </typeparam>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class DisplayValue<T> : DisplayValueBase
        where T : FrameworkElement {
        
        // --- VARIABLES ---

        public override FrameworkElement DisplayObject { get; }

        // --- CONSTRUCTOR ---

        public DisplayValue(T displayObject, EventHandler<EventArgs>? eventListener=null)
            : base(eventListener) {
            DisplayObject = displayObject;
        }
    }
}
