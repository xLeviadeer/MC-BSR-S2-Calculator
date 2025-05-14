using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.PlayerColumn {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class Player : Displayable {

        // --- VARIABLES ---

        // - Name -

        private TextBlock NameTextBlock { get; set; } = new();

        [DisplayValue("Name", 0)]
        [JsonProperty("name")]
        public BoundDisplayValue<TextBlock, string> Name { get; init; }

        // - WasActive -

        private CheckBox WasActiveCheckBox { get; set; } = new();

        [DisplayValue("Active", 0)]
        [JsonProperty("was_active")]
        public BoundDisplayValue<CheckBox, bool> WasActive { get; set; }

        // - IsElectable -

        private CheckBox IsElectableCheckBox { get; set; } = new();

        [DisplayValue("Electable", 0)]
        [JsonProperty("electable")]
        public BoundDisplayValue<CheckBox, bool> IsElectable { get; set; }

        // --- CONSTRUCTORS ---

        private void SetDefaultValues() {
            WasActive = new(WasActiveCheckBox, CheckBox.IsCheckedProperty, true);
            IsElectable = new(IsElectableCheckBox, CheckBox.IsCheckedProperty, true);
        }

        // parameterless requirement
        public Player() {
            Name = new(NameTextBlock, TextBlock.TextProperty, "");
            SetDefaultValues();
        }

        public Player(string name) {
            Name = new(NameTextBlock, TextBlock.TextProperty, name);
            SetDefaultValues();
        }
    }
}
