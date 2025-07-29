using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertyList : SearchableListDisplay<Property>, IStorable, ISwitchManaged {

        // --- VARIABLES ---

        // -- Management --
        #region Management

        // - Switch Management -

        public bool TabContentsChanged { get; } // not used

        public bool RequiresReset { get; set; } = false; 

        public void Reset() { } // not used

        // - Storable -

        public List<string> SaveLocation { get => ["properties.json"]; }

        public IStorable AsIStorable { get => ((IStorable)this); }

        #endregion

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        protected override void SetClassDataList() { // only runs once
            // try to load data list
            AsIStorable.TryLoad(new PropertyList());
        }

        protected override void ExtraSetupOnDataSet() {
            base.ExtraSetupOnDataSet();

            // set defaults
            ScrollBarWidth = 10;
            ShowScrollBar = System.Windows.Controls.ScrollBarVisibility.Auto;
            ItemBorderBrushSides = new SolidColorBrush(ColorResources.ItemBorderBrushSidesLighter);
            EmptyText = "No properties found.";

            // set player rebuilt -> rebuild this event subscription
            MainResources.PlayersDisplay.Rebuilt += (_, _) => {
                ClassDataList.ForEach(property => property.UpdatePropertyDisplay());
                this.BuildGrid();
            };

            // search bar settings
            SearchBarTextMaxLength = 50;
        }

        protected override void ForAllLoadedRowsAndNewItems(Property instance) {
            instance.AssignInstanceID(instance);
        }

        public PropertyList() : base() { }

        public PropertyList(SearchableListDisplay<Property> propertyList) : base(propertyList) { }

        #endregion

        // --- METHODS ---

        // - Name Already Used -

        public bool NameAlreadyUsed(string name, ID playerID) {
            IDPrimary playerPrimaryID = ID.FindParentOrSelfID(playerID);
            foreach (Property property in ClassDataList) {
                if (
                    (property.OwnerID == playerPrimaryID)
                    && (property.Name == name) 
                ) {
                    return true;
                }
            }
            return false;
        }

        // - Find By Name -

        public Property FindByName(string name)
            => ClassDataList.First(property => property.Name == name);

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

        public override void Clear() {
            base.Clear();
            AsIStorable.Save();
        }

        #endregion

        // -- Storable --
        #region Storable

        public void MirrorValues<U>(U cls)
            where U : class, IStorable {
            if (cls is PropertyList clsCasted) {
                this.SearchableClassDataList = clsCasted.SearchableClassDataList;
            }
        }

        public void LoadAs() => AsIStorable.Load<PropertyList>();

        #endregion
    }
}
