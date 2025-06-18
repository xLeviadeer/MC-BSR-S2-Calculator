using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Identification;
using Newtonsoft.Json;

namespace MC_BSR_S2_Calculator.PlayerColumn {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Player : Displayable {

        // --- VARIABLES ---

        // - IDPrimary -

        [JsonProperty("player_id")]
        public IDPrimary PlayerID { get; set; } = new(typeof(Player), 'p');

        // - Name -

        private TextBlock NameTextBlock { get; set; } = new();

        [DisplayValue("Name", 5, GridUnitType.Star)]
        [JsonProperty("name")]
        public BoundDisplayValue<TextBlock, string> Name { get; init; }

        // - WasActive -

        private CheckBox WasActiveCheckBox { get; set; } = new();

        public event EventHandler<EventArgs>? WasActiveChanged;

        [DisplayValue("Active", 3, GridUnitType.Star, HorizontalAlignment.Center, VerticalAlignment.Center, isHitTestVisible: true)]
        [JsonProperty("was_active")]
        public BoundDisplayValue<CheckBox, bool> WasActive { get; set; }

        // - IsElectable -

        private CheckBox IsElectableCheckBox { get; set; } = new();

        public event EventHandler<EventArgs>? IsElectableChanged;

        [DisplayValue("Electable", 4, GridUnitType.Star, HorizontalAlignment.Center, VerticalAlignment.Center, isHitTestVisible: true)]
        [JsonProperty("electable")]
        public BoundDisplayValue<CheckBox, bool> IsElectable { get; set; }

        // - IsElectedOfficial - 

        public event EventHandler<BoolEventArgs>? IsElectedOfficialChanged;

        private bool _isElectedOfficial { get; set; } = false;

        [JsonProperty("elected_official")]
        public bool IsElectedOfficial {
            get => _isElectedOfficial;
            set {
                if (value != _isElectedOfficial) {
                    _isElectedOfficial = value;
                    IsElectedOfficialChanged?.Invoke(this, new(value));
                }
            }
        }

        // - Delete: Context Event -

        public event EventHandler<EventArgs>? DeleteContextClicked;

        // - Rename: Context Event -

        public event EventHandler<EventArgs>? RenameContextClicked;

        // --- CONSTRUCTORS ---

        private void SetDefaultValues() {
            // - core values -

            // check boxes
            WasActive = new(WasActiveCheckBox, CheckBox.IsCheckedProperty, true);
            IsElectable = new(IsElectableCheckBox, CheckBox.IsCheckedProperty, true);

            // - right click menu -

            RightClickMenu = new ContextMenu();
            RightClickMenu.Padding = new Thickness(0);

            // mark elected official item
            var electedOfficialOption = new MenuItem();
            electedOfficialOption.Header = "Elected Official";
            electedOfficialOption.IsCheckable = true;
            electedOfficialOption.IsChecked = IsElectedOfficial;
            electedOfficialOption.Click += (s, e) => {
                IsElectedOfficial = !IsElectedOfficial;
                if (IsElectedOfficial) {
                    IsElectable.Value = true;
                }
            };
            RightClickMenu.Items.Add(electedOfficialOption);

            // separator
            RightClickMenu.Items.Add(new Separator());

            // rename item
            var renameOption = new MenuItem();
            renameOption.Header = "Rename";
            renameOption.Click += (sender, args) => RenameContextClicked?.Invoke(this, args);
            RightClickMenu.Items.Add(renameOption);

            // delete item 
            var deleteOption = new MenuItem();
            deleteOption.Header = "Delete";
            deleteOption.Click += (sender, args) => DeleteContextClicked?.Invoke(this, args);
            RightClickMenu.Items.Add(deleteOption);

            // - other stuff -

            // electable changed event
            IsElectable.PropertyChanged += (s, args) => IsElectableChanged?.Invoke(this, args);
            WasActive.PropertyChanged += (s, args) => WasActiveChanged?.Invoke(this, args);

            // elected official change event
            IsElectedOfficialChanged += (s, args) => {
                if (args.Value == true) {
                    electedOfficialOption.IsChecked = args.Value ?? false;
                    IsElectable.DisplayObject.IsEnabled = false;
                } else {
                    IsElectable.DisplayObject.IsEnabled = true;
                }
            };
        }

        // parameterless requirement
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Player() {
            Name = new(NameTextBlock, TextBlock.TextProperty, "");
            SetDefaultValues();
        }

        public Player(string name) {
            Name = new(NameTextBlock, TextBlock.TextProperty, name);
            PlayerID.AssignNewID(this);
            SetDefaultValues();
        }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}
