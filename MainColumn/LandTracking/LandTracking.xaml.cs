using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.ListBrowser;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public partial class LandTracking : UserControl {
        // --- VARIABLES --- 
        #region VARIABLES
        // - Property Browser -

        public static ListBrowser<PropertyClickable, PropertyManager> PropertyBrowser { get; set; } = new();

        // - Has Been Loaded -

        private static bool PropertiesDisplayHasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public LandTracking() {
            this.InitializeComponent();

            // property browser settings
            PropertyBrowser.Column1Width = new GridLength(8, GridUnitType.Star);
            PropertyBrowser.Column2Width = new GridLength(13, GridUnitType.Star);
            PropertyBrowser.DisplayContentChanged += (_, _) => {
                if (PropertyBrowser.DisplayContent is not null) {
                    PropertyBrowser.DisplayContent.CompleteRequested += this.ViewModifyPropertyManager_CompleteRequested;
                    PropertyBrowser.DisplayContent.ResetRequested += this.ViewModifyPropertyManager_ResetRequested;
                }
            };

            // set modify/view contents
            MainResources.PropertiesDisplay.ClassDataListLoaded += (_, _) => {
                if (PropertiesDisplayHasBeenLoaded) { return; }
                PropertiesDisplayHasBeenLoaded = true;

                // set display as a clickable conversion of the property list
                PropertyBrowser.ListReference = MainResources.PropertiesDisplay.ConvertTo<
                    PropertyClickable,
                    PropertyClickableList
                >();

                // mirror changes from the properties display class list to the converted list
                MainResources.PropertiesDisplay.SearchableClassDataList.ItemsChanged += (sender, args) => {
                    if (args is ListChangedEventArgs listArgs) {
                        var convertedListDisplay = ((PropertyClickableList)PropertyBrowser.ListReference);

                        switch (listArgs.ListChangedType) {
                            case ListChangedType.ItemAdded:
                                convertedListDisplay.SearchableClassDataList.Add(
                                    PropertyClickable.From(((NotifyingList<Property>)sender!) [listArgs.NewIndex])
                                );
                                break;
                            case ListChangedType.ItemDeleted:
                                convertedListDisplay.SearchableClassDataList.RemoveAt(
                                    listArgs.NewIndex
                                );
                                break;
                            case ListChangedType.Reset:
                                convertedListDisplay.Clear();
                                break;
                        }

                        // update for changes
                        convertedListDisplay.BuildGrid();
                    }
                };
            };

            // set content
            this.TabModifyViewProperty.Content = PropertyBrowser;
        }

        #endregion

        // --- METHODS ---

        // -- General --
        #region General

        private void CreateNewPropertyFrom(PropertyManager propertyManager, IDTrace? fromExistingOwnerID = null, IDPrimary? fromExistingID = null) {
            // find associated player ID
            IDTrace playerTrace;
            if (fromExistingOwnerID is null) {
                IDPrimary associatedPlayerID = propertyManager.GetOwningPlayerID();
                playerTrace = associatedPlayerID.CreateTrace(this); // dummy pass of this, could be literally anything
            } else {
                // set 
                playerTrace = fromExistingOwnerID;
            }

            // create a new property and add it to the property list
            var newProperty = new Property(
                playerTrace, // owner player id trace
                propertyManager.NameInput.Text.Trim(), // property name
                (string)(propertyManager.PropertyTypeInput.SelectedItem ?? ""), // property type
                Math.Max(1, (int)propertyManager.ResidentsCountInput.TryGetValue<double>()), // residents count
                propertyManager.GetSubsections(), // subsections
                propertyManager.TaxIncentives.GetActiveIncentives(), // tax incentives
                propertyManager.PurchaseIncentives.GetActiveIncentives(), // purchase incentives
                propertyManager.ViolationIncentives.GetActiveIncentives() // violation incentives
                    .Cast<ActiveViolationIncentive>()
                    .ToArray(),
                (propertyManager.SubsurfaceLandProvisionCheck.IsChecked ?? false) // subsurface land provision
                    ? ((CoordinateInput)propertyManager.SubsurfaceLandProvisionCheck.Content).YCoordinate
                    : null,
                propertyManager.HasMailboxCheck.IsChecked ?? false, // has mailbox
                propertyManager.HasEdgeSpacingCheck.IsChecked ?? false, // follows guidelines
                propertyManager.ApprovedCheck.IsChecked ?? false, // is approved
                fromExistingID // create from existing ID
            );

            // assign player trace to the property just created
            playerTrace.AssignInstance(newProperty);

            // add to main
            MainResources.PropertiesDisplay.Add(newProperty);

            // clear values
            propertyManager.Reset();
        }

        #endregion

        // -- Per-Item --
        #region Per-Item

        private void CreatePropertyManager_CompleteRequested(object? sender, EventArgs args) {
            // create property
            this.CreateNewPropertyFrom(this.CreatePropertyManager);

            // tab to create/modify page
            this.MainTabControl.SelectedItem = this.TabModifyViewProperty;
        }

        private void CreatePropertyManager_ResetRequested(object? sender, EventArgs args) {
            PropertyBrowser.DisplayContent!.Reset();
        }

        private void ViewModifyPropertyManager_CompleteRequested(object? sender, EventArgs args) {
            // get current property clickable object
            PropertyClickable currPropertyClickable = PropertyBrowser.SelectedItem!;

            // delete this property
            MainResources.PropertiesDisplay.Remove(
                MainResources.PropertiesDisplay.FindByName(currPropertyClickable.Name.Value)
            );

            // create property
            this.CreateNewPropertyFrom(PropertyBrowser.DisplayContent!, currPropertyClickable.OwnerID, currPropertyClickable.DisplayableID);

            // set view to blank
            PropertyClickable.ClearDisplayContent();
        }

        private void ViewModifyPropertyManager_ResetRequested(object? sender, EventArgs args) {
            // get current property clickable object
            PropertyClickable currPropertyClickable = PropertyBrowser.SelectedItem!;

            // delete this property
            MainResources.PropertiesDisplay.Remove(
                MainResources.PropertiesDisplay.FindByName(currPropertyClickable.Name)
            );

            // clear display
            PropertyClickable.ClearDisplayContent();
        }

        #endregion
    }
}
