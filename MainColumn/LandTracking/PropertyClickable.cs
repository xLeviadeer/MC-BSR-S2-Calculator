using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.ListBrowser;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class PropertyClickable : Property, IBrowserDisplayable<PropertyClickable, PropertyManager>, IFromConverter<PropertyClickable> {

        // --- VARIABLES ---

        // -- Browser Displayable -
        #region Browser Displayable

        // - As Browser Displayable -

        public IBrowserDisplayable<PropertyClickable, PropertyManager> AsBrowserDisplayable => this;

        public static PropertyManager? DisplayedContent { get; private set; } = null;

        public static void ClearDisplayContent() {
            DisplayedContent = null;
            DisplayedContentChanged?.Invoke(null, EventArgs.Empty);
        }

        public static event EventHandler<EventArgs>? DisplayedContentChanged;

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
        public PropertyClickable(Property propertyClickable)
            : base(propertyClickable) {
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
                ShowResetButton = false
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

                    // validity
                    DisplayedContent.Validity["OwningPlayerInput"].IsValid = (this.OwnerID is not null);

                    // run only once
                    DisplayedContent.OwningPlayerInputUpdated -= setOwningPlayer;
                }
            }

            // property name
            DisplayedContent.NameInput.LayoutLoaded += (_, _) => {
                DisplayedContent.NameInput.Text = this.Name.Value;
                DisplayedContent.Validity["NameInput"].IsValid = true;
            };

            // property type
            DisplayedContent.PropertyTypeInput.LayoutLoaded += (_, _) => {
                DisplayedContent.PropertyTypeInput.SelectedItem = XamlConverter.CapitalizeWords(this.PropertyType);
                DisplayedContent.Validity["PropertyTypeInput"].IsValid = true;
            };

            // residents count
            DisplayedContent.ResidentsCountInput.LayoutLoaded += (_, _) => {
                DisplayedContent.ResidentsCountInput.Text = this.ResidentsCount.ToString();
            };

            // subsections
            DisplayedContent.LoadingCompleted += (_, _) => {
                DisplayedContent.SetSections(this.Subsections.ToArray());
                DisplayedContent.Validity["Sections"].IsValid = true;
            };

            // tax incentives
            DisplayedContent.TaxIncentives.CompletedLoading += (_, _) => {
                foreach (ActiveIncentive incentive in this.TaxIncentives) {
                    DisplayedContent.TaxIncentives.Add(incentive.Name);
                }
            };

            // purchase incentives
            DisplayedContent.PurchaseIncentives.CompletedLoading += (_, _) => {
                foreach (ActiveIncentive incentive in this.PurchaseIncentives) {
                    DisplayedContent.PurchaseIncentives.Add(incentive.Name);
                }
            };

            // violation incentives
            DisplayedContent.ViolationIncentives.CompletedLoading += (_, _) => {
                foreach (ActiveViolationIncentive incentive in this.ViolationIncentives) {
                    IncentivesManager violationIncentivesManager = DisplayedContent.ViolationIncentives;
                    violationIncentivesManager.Add(incentive.Name);

                    // set violation count
                    ViolationIncentive addedIncentive = (ViolationIncentive)violationIncentivesManager.IncentivesDisplay[^1];
                    addedIncentive.ViolationCountFromDisplay = incentive.ViolationCount;
                }
            };

            // subsurface land provision
            DisplayedContent.SubsurfaceLandProvisionCheck.CompletedLoading += (_, _) => {
                bool hasValue = (this.SubsurfaceLandProvisionLevel != null);
                DisplayedContent.SubsurfaceLandProvisionCheck.IsChecked = hasValue;
                if (hasValue) {
                    CoordinateInput coordinateInput = ((CoordinateInput)DisplayedContent.SubsurfaceLandProvisionCheck.Content);
                    coordinateInput.YInput.LayoutLoaded += (_, _) => {
                        ((IntegerTextBox)coordinateInput.YInput.Element).Value
                            = (int)(this.SubsurfaceLandProvisionLevel
                            ?? (coordinateInput.YInput.Element.DefaultValue ?? 0));
                    };
                }
            };

            // has mailbox
            void checkMailbox(object? sender, EventArgs args) {
                DisplayedContent!.HasMailboxCheck.IsChecked = this.HasMailbox;
                DisplayedContent!.Validity["HasMailboxCheck"].IsValid = this.HasMailbox;
                DisplayedContent!.HasMailboxCheck.LayoutLoaded -= checkMailbox;
            }
            DisplayedContent.HasMailboxCheck.LayoutLoaded += checkMailbox;

            // follows guidelines
            void checkGuidelines(object? sender, EventArgs args) {
                DisplayedContent!.HasEdgeSpacingCheck.IsChecked = this.FollowsPropertyMetricGuidelines;
                DisplayedContent!.Validity["HasEdgeSpacingCheck"].IsValid = this.FollowsPropertyMetricGuidelines;
                DisplayedContent!.HasEdgeSpacingCheck.LayoutLoaded -= checkGuidelines;
            }
            DisplayedContent.HasEdgeSpacingCheck.LayoutLoaded += checkGuidelines;

            // is approved
            void checkApproved(object? sender, EventArgs args) {
                DisplayedContent!.ApprovedCheck.IsChecked = this.IsApproved;
                DisplayedContent!.Validity["ApprovedCheck"].IsValid = this.IsApproved;
                DisplayedContent!.ApprovedCheck.LayoutLoaded -= checkApproved;
            }
            DisplayedContent.ApprovedCheck.LayoutLoaded += checkApproved;

            DisplayedContent.CompleteRequested += (_, _) => {
                // delete this property
                MainResources.PropertiesDisplay.Remove(
                    MainResources.PropertiesDisplay.FindByName(this.Name)
                );
            };

            // dipslayed content update
            DisplayedContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void HeldLeftClickListener(object? sender, EventArgs args)
            => LoadToListBrowser();
    }
}
