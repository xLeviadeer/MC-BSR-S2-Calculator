using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.ListBrowser;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class PropertyClickable : Property, 
        IBrowserDisplayable<PropertyClickable, PropertyManager>, 
        IFromConverter<PropertyClickable> {

        // --- VARIABLES ---

        // -- Browser Displayable -
        #region Browser Displayable

        // - As Browser Displayable -

        public IBrowserDisplayable<PropertyClickable, PropertyManager> AsBrowserDisplayable => this;

        // - Displayed Content -

        public static PropertyManager? DisplayedContent { get; private set; } = null;

        public static void ClearDisplayContent() {
            DisplayedContent = null;
            DisplayedContentChanged?.Invoke(null, EventArgs.Empty);
        }

        public static event EventHandler<EventArgs>? DisplayedContentChanged;

        public static PropertyClickable? LastSelectedDisplayable { get; private set; } = null;

        #endregion

        // -- Management --
        #region Management

        // - ID -

        public static new char TypeCharacter { get; } = 'o';

        #endregion

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        public PropertyClickable()
            : base() {
            AsBrowserDisplayable.CheckBrowserValidity();
        }

        // copy constructor
        public PropertyClickable(Property property)
            : base(property) {
            AsBrowserDisplayable.CheckBrowserValidity();
        }

        #endregion

        // --- METHODS ---

        // - From -

        public static PropertyClickable From(object property)
            => new((Property)property);

        // - Left Click/Loading -

        public void LoadToListBrowser() {
            // create property manager
            DisplayedContent = new PropertyManager() {
                TitleText = $"Modify '{this.Name.Value}'",
                ResetText = "Delete",
                CreateText = "Modify Property"
            };

            // owning player
            DisplayedContent.OwningPlayerInputUpdated += setOwningPlayer;
            void setOwningPlayer(object? sender, EventArgs args) {
                if (
                    (this.OwnerID is not null)
                    && (DisplayedContent is not null)
                ) {
                    // set selected item
                    int selectedIndex = -1;
                    int i = 0;
                    foreach (string playerName in DisplayedContent.OwningPlayerInput.Items) {
                        var playerID = DisplayedContent.OwningPlayerOptions[playerName];
                        if (playerID == this.OwnerID) {
                            selectedIndex = i;
                            break;
                        }
                        i++;
                    }
                    DisplayedContent.OwningPlayerInput.SelectedIndex = selectedIndex;

                    // default value
                    DisplayedContent.OwningPlayerInput.TrySetDefaultValue(selectedIndex);

                    // validity
                    DisplayedContent.Validity["OwningPlayerInput"].IsValid = (this.OwnerID is not null);

                    // run only once
                    DisplayedContent.OwningPlayerInputUpdated -= setOwningPlayer;
                }
            }

            // property name
            DisplayedContent.NameInput.LayoutLoaded += (_, _) => {
                DisplayedContent.NameInput.Text = this.Name.Value;
                DisplayedContent.NameInput.TrySetDefaultValue(this.Name.Value);
                DisplayedContent.Validity["NameInput"].IsValid = true;
            };

            // property type
            DisplayedContent.PropertyTypeInput.LayoutLoaded += (_, _) => {
                string capitalizedPropertyType = XamlConverter.CapitalizeWords(this.PropertyType);
                DisplayedContent.PropertyTypeInput.SelectedItem = capitalizedPropertyType;
                DisplayedContent.PropertyTypeInput.TrySetDefaultValue(DisplayedContent.PropertyTypeInput.SelectedIndex);
                DisplayedContent.Validity["PropertyTypeInput"].IsValid = true;
            };

            // residents count
            DisplayedContent.ResidentsCountInput.LayoutLoaded += (_, _) => {
                DisplayedContent.ResidentsCountInput.Text = this.ResidentsCount.ToString();
                DisplayedContent.ResidentsCountInput.TrySetDefaultValue(this.ResidentsCount.ToString());
            };

            // subsections
            DisplayedContent.LoadingCompleted += (_, _) => {
                DisplayedContent.SetSections(this.Subsections.ToArray());
                DisplayedContent.SetDefaultSections(DisplayedContent.Sections.ToList());
                DisplayedContent.Validity["Sections"].IsValid = true;
            };

            // tax incentives
            DisplayedContent.TaxIncentives.CompletedLoading += (_, _) => {
                IncentivesManager manager = DisplayedContent.TaxIncentives;
                foreach (ActiveIncentive incentive in this.TaxIncentives) {
                    manager.Add(incentive.Name);
                }
                IncentivesList display = manager.IncentivesDisplay;
                display.DefaultCount = display.Count;
            };

            // purchase incentives
            DisplayedContent.PurchaseIncentives.CompletedLoading += (_, _) => {
                IncentivesManager manager = DisplayedContent.PurchaseIncentives;
                foreach (ActiveIncentive incentive in this.PurchaseIncentives) {
                    manager.Add(incentive.Name);
                }
                IncentivesList display = manager.IncentivesDisplay;
                display.DefaultCount = display.Count;
            };

            // violation incentives
            DisplayedContent.ViolationIncentives.CompletedLoading += (_, _) => {
                IncentivesManager manager = DisplayedContent.ViolationIncentives;
                IncentivesList display = manager.IncentivesDisplay;
                foreach (ActiveViolationIncentive incentive in this.ViolationIncentives) {
                    manager.Add(incentive.Name);

                    // set violation count
                    ViolationIncentive addedIncentive = (ViolationIncentive)display[^1];
                    addedIncentive.ViolationCountFromDisplay = incentive.ViolationCount;
                    addedIncentive.DefaultValue = incentive.ViolationCount;
                }
                display.DefaultCount = display.Count;
            };

            // subsurface land provision
            DisplayedContent.SubsurfaceLandProvisionCheck.CompletedLoading += (_, _) => {
                bool hasValue = (this.SubsurfaceLandProvisionLevel != null);
                DisplayedContent.SubsurfaceLandProvisionCheck.IsChecked = hasValue;
                if (hasValue) {
                    CoordinateInput coordinateInput = ((CoordinateInput)DisplayedContent.SubsurfaceLandProvisionCheck.Content);
                    coordinateInput.YInput.LayoutLoaded += (_, _) => {
                        var coordinateTextBox = ((IntegerTextBox)coordinateInput.YInput.Element);
                        var setValue = (int)(this.SubsurfaceLandProvisionLevel
                            ?? (coordinateInput.YInput.Element.DefaultValue ?? 0));
                        coordinateTextBox.Value = setValue;
                        coordinateTextBox.DefaultValue = setValue;
                    };
                }
                DisplayedContent.SubsurfaceLandProvisionCheck.CheckBoxLabelObject.Element.DefaultValue = hasValue;
            };

            // has mailbox
            void checkMailbox(object? sender, EventArgs args) {
                DisplayedContent!.HasMailboxCheck.IsChecked = this.HasMailbox;
                DisplayedContent!.HasMailboxCheck.Element.DefaultValue = this.HasMailbox;
                DisplayedContent!.Validity["HasMailboxCheck"].IsValid = this.HasMailbox;
                DisplayedContent!.HasMailboxCheck.LayoutLoaded -= checkMailbox;
            }
            DisplayedContent.HasMailboxCheck.LayoutLoaded += checkMailbox;

            // follows guidelines
            void checkGuidelines(object? sender, EventArgs args) {
                DisplayedContent!.HasEdgeSpacingCheck.IsChecked = this.FollowsPropertyMetricGuidelines;
                DisplayedContent!.HasEdgeSpacingCheck.Element.DefaultValue = this.FollowsPropertyMetricGuidelines;
                DisplayedContent!.Validity["HasEdgeSpacingCheck"].IsValid = this.FollowsPropertyMetricGuidelines;
                DisplayedContent!.HasEdgeSpacingCheck.LayoutLoaded -= checkGuidelines;
            }
            DisplayedContent.HasEdgeSpacingCheck.LayoutLoaded += checkGuidelines;

            // is approved
            void checkApproved(object? sender, EventArgs args) {
                DisplayedContent!.ApprovedCheck.IsChecked = this.IsApproved;
                DisplayedContent!.ApprovedCheck.Element.DefaultValue = this.IsApproved;
                DisplayedContent!.Validity["ApprovedCheck"].IsValid = this.IsApproved;
                DisplayedContent!.ApprovedCheck.LayoutLoaded -= checkApproved;
            }
            DisplayedContent.ApprovedCheck.LayoutLoaded += checkApproved;

            // dipslayed content update
            LastSelectedDisplayable = this;
            DisplayedContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void HeldLeftClickListener(object? sender, EventArgs args)
            => LoadToListBrowser();
    }
}
