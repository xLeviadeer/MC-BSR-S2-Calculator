using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyList : ListDisplay<Property>, IStorable, ISwitchManaged {

        // --- VARIABLES ---

        // -- Switch Management --

        public bool TabContentsChanged { get; } // not used

        public bool RequiresReset { get; set; } = false; 

        public void Reset() { } // not used

        // -- Storable --

        public List<string> SaveLocation { get => ["properties.json"]; }

        public IStorable AsIStorable { get => ((IStorable)this); }

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        protected override void ForAllLoadedRowsAndNewItems(Property instance) {
            instance.PropertyID.AssignInstance(instance);
        }

        protected override void SetClassDataList() {
            // try to load data list
            AsIStorable.TryLoad(new PropertyList());

            // set defaults
            ScrollBarWidth = 10;
            ShowScrollBar = System.Windows.Controls.ScrollBarVisibility.Auto;
            ItemBorderBrushSides = new SolidColorBrush(ColorResources.ItemBorderBrushSidesLighter);
            EmptyText = "No properties yet. Create one!";
        }

        #endregion

        // --- METHODS ---

        // -- Operation Overrides --
        #region Operation Overrides

        public override void Add(Property cls) {
            base.Add(cls);
            AsIStorable.Save();
        }

        public override void Remove(Property cls) {
            base.Remove(cls);
            AsIStorable.Save();
        }

        public override void RemoveAt(int index) {
            base.RemoveAt(index);
            AsIStorable.Save();
        }

        public override void Clear() {
            base.Clear();
            AsIStorable.Save();
        }

        #endregion

        // -- Storable --

        public void MirrorValues<U>(U cls)
            where U : class, IStorable {
            if (cls is PropertyList clsCasted) {
                ClassDataList = clsCasted.ClassDataList;
            }
        }

        public void LoadAs() => AsIStorable.Load<PropertyList>();
    }
}
