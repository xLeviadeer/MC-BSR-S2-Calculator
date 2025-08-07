using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using Newtonsoft.Json;
using System.Windows;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System.Xml.Linq;
using System.Security.Cryptography.Xml;
using MC_BSR_S2_Calculator.Utility.Identification;
using System.Diagnostics;
using MC_BSR_S2_Calculator.Utility;

namespace MC_BSR_S2_Calculator.PlayerColumn {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class PlayerList : ListDisplay<Player>, IStorable {

        // --- VARIABLES ---

        // -- Events --

        public event EventHandler<BoolEventArgs>? AnyWasActiveChanged;

        public bool AnyWasActiveTrue => ClassDataList.Any(player => player.WasActive);

        // -- Storable --

        public List<string> SaveLocation { get => ["players.json"]; }

        public IStorable AsIStorable { get => ((IStorable)this); }

        // --- CONSTRUCTORS ---

        protected override void SetClassDataList() {
            // try to load data list
            AsIStorable.TryLoad(new PlayerList());
        }

        protected override void ForAllLoadedRowsAndNewItems(Player instance) {
            // id 
            instance.AssignInstanceID(instance);

            // event subscription
            instance.IsElectableChanged += (s, e) => AsIStorable.Save();
            instance.IsElectableChanged += (s, e) => BuildGrid();
            instance.IsElectedOfficialChanged += (s, e) => AsIStorable.Save();
            instance.IsElectedOfficialChanged += (s, e) => BuildGrid();
            instance.WasActiveChanged += (s, e) => AsIStorable.Save();
            instance.WasActiveChanged += (s, e) => AnyWasActiveChanged?.Invoke(this, new BoolEventArgs(instance.WasActive));
            instance.DeleteContextClicked += (s, e) => DeletePlayer(instance);
            instance.RenameContextClicked += (s, e) => RenamePlayer(instance);
        }

        private void DeletePlayer(Player player) {
            // confirmation
            if (new ConfirmationWindow(
                $"Are you sure you want to delete '{player.Name.Value}'?",
                descriptionText: "This will leave associations to this player empty"
                + "\nplayers should not be deleted unless they have accidentally been created",
                useConfirmColor: "red",
                useChargingButton: true,
                chargeTime: 2
            ).ShowDialog() == true) {
                player.DisplayableID.Delete(); // delete id
                ClassDataList.Remove(player); // delete player from list
                AsIStorable.Save();
                BuildGrid();
            }

            // event 
            AnyWasActiveChanged?.Invoke(this, new BoolEventArgs(player.WasActive));
        }

        private void RenamePlayer(Player player) {
            var dialog = new TextConfirmationWindow(
                "Enter new name",
                "Confirm",
                "Cancel",
                textBoxType: TextBoxTypes.StringTextBox,
                textMaxLength: (int)Application.Current.Resources["PlayerNameMaxLength"]
            );
            dialog.TextBoxInput.Text = player.Name.Value; // start with player name

            // set status of confirm button based on text box validity
            if (dialog.TextBoxInput is StringTextBox textBox) {
                textBox.ValidityChanged += (sender, args) => {
                    if (args.Value == null) {
                        dialog.ConfirmButton.IsEnabled = false;
                    } else {
                        dialog.ConfirmButton.IsEnabled = (bool)args.Value;
                    }
                };
            }

            // show/get results
            if (dialog.ShowDialog() == true) {
                // check if player name already used
                string name = dialog.TextBoxInput.Text;
                if (name == player.Name.Value) { return; } // name was not changed

                // check if name exists and add
                string nameTrimmed = name.Trim();
                if (!NameAlreadyExists(nameTrimmed)) {
                    // modify payer
                    player.Name.Value = name;
                    AsIStorable.Save();
                    BuildGrid(); // build required for sorting
                } else {
                    ShowPlayerNameTaken(nameTrimmed);
                }
            }
        }

        // --- METHODS ---

        // - name existence check -

        public void ShowPlayerNameTaken(string name) {
            MessageBox.Show($"Player name, '{name }', already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool NameAlreadyExists(string name) {
            name = name.ToLower();
            foreach (var player in ClassDataList) {
                if (player.Name.Value.ToLower() == name) {
                    return true;
                }
            }
            return false;
        }

        // -- Storable --

        public void MirrorValues<U>(U cls)
            where U : class, IStorable {
            if (cls is PlayerList clsCasted) {
                ClassDataList = clsCasted.ClassDataList;
            }
        }

        public void LoadAs() => AsIStorable.Load<PlayerList>();

        // -- Custom Sorting --

        protected override void SortClassData() {
            // create a sublist of elected officials (and sort)
            List<Player> electedOfficialsClassDataList = ClassDataList.Where(cls => cls.IsElectedOfficial).ToList();
            ClassDataList.RemoveAll(cls => cls.IsElectedOfficial);
            electedOfficialsClassDataList = electedOfficialsClassDataList.OrderBy(cls => cls.Name, StringComparer.OrdinalIgnoreCase).ToList();


            // create a sublist of non-electable people (and sort)
            List<Player> notElectableClassDataList = ClassDataList.Where(cls => !cls.IsElectable).ToList();
            ClassDataList.RemoveAll(cls => !cls.IsElectable);
            notElectableClassDataList = notElectableClassDataList.OrderBy(cls => cls.Name, StringComparer.OrdinalIgnoreCase).ToList();

            // sort the remaining list
            List<Player> remainingClassDataList = ClassDataList.OrderBy(cls => cls.Name, StringComparer.OrdinalIgnoreCase).ToList();
            ClassDataList.Clear();

            // recombine lists into the ClassDataList
            ClassDataList.AddRange(electedOfficialsClassDataList);
            ClassDataList.AddRange(remainingClassDataList);
            ClassDataList.AddRange(notElectableClassDataList);
        }
    }
}
