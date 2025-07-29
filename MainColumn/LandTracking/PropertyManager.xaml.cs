using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking
{
    /// <summary>
    /// Interaction logic for PropertyManager.xaml
    /// </summary>
    public partial class PropertyManager : UserControl, 
        IValidityHolder, 
        ISwitchManaged, 
        INotifyPropertyChanged,
        IDisposable {
        // --- VARIABLES ---

        // -- Verirfications --
        #region Verifications

        // - Property Changed Notification -

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // - Validity Holder -

        public ValidityHolder Validity { get; private set; } = new(
            new() {
                [nameof(OwningPlayerInput)] = new(),
                [nameof(NameInput)] = new(),
                [nameof(PropertyTypeInput)] = new(),
                [nameof(Sections)] = new(),
                [nameof(HasMailboxCheck)] = new(),
                [nameof(HasEdgeSpacingCheck)] = new(),
                [nameof(ApprovedCheck)] = new() { IsEnabled = false }
            }
        );

        // - Validity Changed Event -

        private bool IsValid { get; set; }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - Tab Contents Changed -

        private Dictionary<string, Func<bool>> ContentChanges; // set in constructor

        public bool TabContentsChanged => ContentChanges.Any(item => item.Value() == true);

        public bool RequiresReset { get; set; } = true;

        #endregion

        // -- Events --
        #region Events

        // - Create Charged -

        public event EventHandler<EventArgs>? CompleteRequested;

        // - Loading Completed -

        public event EventHandler<EventArgs>? LoadingCompleted;

        #endregion

        // -- Interface --
        #region Interface

        // - Owning Player Options -

        public Dictionary<string, IDPrimary> OwningPlayerOptions { get; private set; } = new();

        public event EventHandler<EventArgs>? OwningPlayerInputUpdated;

        // - Title -

        public string TitleText {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            nameof(TitleText),
            typeof(string),
            typeof(PropertyManager),
            new PropertyMetadata("Property Manager")
        );

        // - Show Reset Button -

        public bool ShowResetButton {
            get => (bool)GetValue(ShowResetButtonProperty);
            set => SetValue(ShowResetButtonProperty, value);
        }

        public static readonly DependencyProperty ShowResetButtonProperty = DependencyProperty.Register(
            nameof(ShowResetButton),
            typeof(bool),
            typeof(PropertyManager),
            new PropertyMetadata(true, OnShowResetButtonChanged)
        );

        private static void OnShowResetButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is PropertyManager control) {
                control.OnPropertyChanged(nameof(ShowResetButtonVisibilty));
            }
        }

        public Visibility ShowResetButtonVisibilty => XamlConverter.BoolToVisibility(ShowResetButton);

        #endregion

        // -- Utility --
        #region Utility

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public PropertyManager()
        {
            InitializeComponent();

            // setup content changed
            ContentChanges = new Dictionary<string, Func<bool>>() {
                [nameof(OwningPlayerInput)] = () => OwningPlayerInput.Element.TabContentsChanged,
                [nameof(NameInput)] = () => NameInput.Element.TabContentsChanged,
                [nameof(PropertyTypeInput)] = () => PropertyTypeInput.Element.TabContentsChanged,
                [nameof(ResidentsCountInput)] = () => NameInput.Element.TabContentsChanged,
                [nameof(Sections)] = CheckIfSectionsChanged,
                [nameof(TaxIncentives)] = () => (TaxIncentives.IncentivesDisplay.Count > 0),
                [nameof(ViolationIncentives)] = () => (ViolationIncentives.IncentivesDisplay.Count > 0),
                [nameof(PurchaseIncentives)] = () => (PurchaseIncentives.IncentivesDisplay.Count > 0),
                [nameof(SubsurfaceLandProvisionCheck)] = () => SubsurfaceLandProvisionCheck.CheckBoxLabelObject.Element.TabContentsChanged,
                [nameof(HasMailboxCheck)] = () => HasMailboxCheck.Element.TabContentsChanged,
                [nameof(HasEdgeSpacingCheck)] = () => HasEdgeSpacingCheck.Element.TabContentsChanged,
                [nameof(ApprovedCheck)] = () => ApprovedCheck.Element.TabContentsChanged
            };

            // on rebuilt player list, update names list (only if visible rn)
            MainResources.PlayersDisplay.Rebuilt += OnPlayersListRebuilt;

            // assign Sections and deletion events
            Loaded += (_, __) => {
                // set the owning player items
                UpdateOwningPlayerInputItems();

                if (!HasBeenLoaded) {
                    ResetSections();
                    HasBeenLoaded = true;
                }

                // try to add margin to scroll bar
                var scrollBar = XamlConverter.FindVerticalScrollBar(MainScrollViewer);
                if (scrollBar != null) {
                    scrollBar.Margin = new Thickness(3, 0, 0, 0);
                }

                // set y input max to the surface land max
                ((CoordinateInput)SubsurfaceLandProvisionCheck.Content).YInput.MaxInputFromTextLabel = LandDefinitions.SurfaceLandareaYLevelMax;

                // loading completed
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
            };

            // name input validation type
            NameInput.LayoutLoaded += (_, _) => {
                ((StringTextBox)NameInput.Element).ValidationType = StringTextBox.ValidationTypes.Manual;
            };
        }

        private void OnPlayersListRebuilt(object? sender, EventArgs args) {
            // doesn't update if it's not visible
            if (this.IsVisible) {
                UpdateOwningPlayerInputItems();
            }
        }

        #endregion

        // --- EVENT METHODS ---

        // - Is Valid Exposure - 

        public bool CheckValidity() => Validity.CheckValidity();

        // - Get Subsections -

        public static PropertySubsection[] GetSubsections(PropertySection[] sections) {
            return sections
            .Select(section => section.Subsection)
            .ToArray();
        }

        public PropertySubsection[] GetSubsections()
            => GetSubsections(this.Sections.ToArray());

        // - Owning Player -

        public IDPrimary GetOwningPlayerID() {
            if (OwningPlayerInput.SelectedItem is not string playerName) {
                throw new InvalidOperationException($"OwningPlayerInput's SelectedItem could not be casted to a string: {OwningPlayerInput.SelectedItem?.ToString()}");
            }
            return OwningPlayerOptions[playerName];
        }

        // - Dispose -

        public void Dispose() {
            MainResources.PlayersDisplay.Rebuilt -= OnPlayersListRebuilt;
        }

        // -- Completion Buttons --
        #region Completion Buttons

        private void OnClearCharged(object? sender, EventArgs args) => Reset();

        public void Reset() {
            // temporarily dont update buttons (so we dont waste time validating on every change)
            DoUpdateButtons = false;

            OwningPlayerInput.SelectedIndex = -1;

            NameInput.Text = "";
            NameInput.IsValid = null;

            PropertyTypeInput.SelectedIndex = -1;

            ResidentsCountInput.Text = "0";
            ResidentsCountInput.IsValid = null;

            ResetSections();

            TaxIncentives.Clear();
            ViolationIncentives.Clear();
            PurchaseIncentives.Clear();

            SubsurfaceLandProvisionCheck.IsChecked = false;
            HasMailboxCheck.IsChecked = false;
            HasEdgeSpacingCheck.IsChecked = false;

            Dispatcher.BeginInvoke(new Action(() => {
                ResetFinalResults();
            }), DispatcherPriority.Background);

            // allow updates again and manually update buttons
            DoUpdateButtons = true;
            UpdateCreateButtonEnabledStatus();
        }

        private void OnCreateCharged(object? sender, EventArgs args) {
            // run create completed event
            CompleteRequested?.Invoke(this, args);
        }

        #endregion

        // -- Grid Methods --
        #region Grid Methods

        private void OnKeyDown(object? sender, KeyEventArgs args) {
            if (args.Key == Key.Enter) {
                if (CreateButton.IsEnabled) {
                    CreateButton.MarkAsPressed(this, args);
                }
            } else if (args.Key == Key.Escape) {
                ClearButton.MarkAsPressed(this, args);
            } else {
                if (CreateButton.IsChargeCycling) {
                    args.Handled = true;
                }
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs args) {
            if (args.Key == Key.Enter) {
                CreateButton.MarkAsUnpressed(this, args);
            } else if (args.Key == Key.Escape) {
                ClearButton.MarkAsUnpressed(this, args);
            }
        }

        private void OnTextInput(object sender, TextCompositionEventArgs args) {
            if (CreateButton.IsChargeCycling) {
                args.Handled = true;
            }
        }

        private void OnMouseDown(object? sender, MouseButtonEventArgs args) {
            MainGrid.Focus();
        }

        #endregion

        // -- Per-Item Group Methods --
        #region Per-Item Group Methods

        // - per item helper -

        private bool DoUpdateButtons { get; set; } = true;

        private void UpdateCreateButtonEnabledStatus() {
            // doesn't run if doupdatebuttons false
            if (DoUpdateButtons) {
                // updates buttons
                if (Validity.CheckValidity()) {
                    CreateButton.IsEnabled = true;
                } else {
                    CreateButton.IsEnabled = false;
                    CreateButton.MarkAsUnpressed(this, EventArgs.Empty);
                }

                // send validity changed event
                IsValid = CreateButton.IsEnabled; // mirror enabled state
                ValidityChanged?.Invoke(this, new(IsValid));
            }
        }

        // - final results updates -

        private void setApprovedCheck(int propertyMetric) {
            if (Validity[nameof(Sections)].IsValid) {
                // property size
                string propertySize = Property.GetPropertySize(propertyMetric);
                if (
                    (propertySize == Property.PropertySize.Large)
                    || (propertySize == Property.PropertySize.Massive)
                ) {
                    // show approved box
                    ApprovedCheck.Visibility = Visibility.Visible;
                    Validity[nameof(ApprovedCheck)].IsEnabled = true;
                } else {
                    // hide approved box
                    ApprovedCheck.Visibility = Visibility.Collapsed;
                    Validity[nameof(ApprovedCheck)].IsEnabled = false;
                }
                UpdateCreateButtonEnabledStatus();
                PropertySizeResult.Result = propertySize;
            } else {
                // hide approved box
                ApprovedCheck.Visibility = Visibility.Collapsed;
                Validity[nameof(ApprovedCheck)].IsEnabled = false;

                UpdateCreateButtonEnabledStatus();
                PropertySizeResult.Result = Property.PropertySize.Invalid;
            }
        }

        private void UpdateFinalResults() {
            const int roundingDecimals = 2;

            // addition format helper
            void additionFormat(
                ResultDisplay display,
                double added, 
                double total, 
                bool showTotal=false
            ) {
                if (added != 0) {
                    display.Visibility = Visibility.Visible;
                    display.Result = $"{(
                        (added > 0)
                        ? "+"
                        : string.Empty
                    )}{Math.Round(added, roundingDecimals)} {(
                        showTotal 
                        ? $"({Math.Round(total, roundingDecimals)})" 
                        : string.Empty
                    )}";
                } else {
                    display.Visibility = Visibility.Collapsed;
                }
            }

            // calculate metric
            int propertyMetric = Property.GetPropertyMetric(this.GetSubsections());

            // - purchase results -

            // if invalid
            if (Validity[nameof(Sections)].CheckValidity() == false) {
                PropertySizeResult.Result = Property.PropertySize.Invalid;
                FinalPurchaseValueResult.Result = FinalPurchaseValueResult.DefaultResult;
                setApprovedCheck(propertyMetric);

            // if valid
            } else {
                // approved check box
                setApprovedCheck(propertyMetric);

                // property purchase values
                int finalPurchaseValue = Property.GetPurchaseValueFinal(
                    propertyMetric,
                    ViolationIncentives.GetActiveIncentives()
                        .Cast<ActiveViolationIncentive>()
                        .ToArray(),
                    PurchaseIncentives.GetActiveIncentives(),
                    out int purchaseValue,
                    out int addedByIncentives
                );
                additionFormat(PurchaseValueByIncentivesResult, addedByIncentives, finalPurchaseValue);

                // original value
                if (PurchaseValueByIncentivesResult.Visibility != Visibility.Collapsed) {
                    PurchaseValueResult.Result = purchaseValue.ToString();
                    PurchaseValueResult.Visibility = Visibility.Visible;
                } else {
                    PurchaseValueResult.Visibility = Visibility.Collapsed;
                }

                // final value
                FinalPurchaseValueResult.Result = Math.Round((double)finalPurchaseValue, roundingDecimals).ToString();
            }

            // - tax results updates -

            // if invalid
            if (
                (Validity[nameof(Sections)].CheckValidity() == false)
                || (Validity[nameof(PropertyTypeInput)].CheckValidity() == false)
            ) {
                FinalTaxContributionResult.Result = FinalTaxContributionResult.DefaultResult;
            
            // if valid
            } else {
                // property tax 
                int finalTaxValue = Property.GetTotalTaxContribution(
                    propertyMetric,
                    TaxIncentives.GetActiveIncentives(),
                    ViolationIncentives.GetActiveIncentives()
                        .Cast<ActiveViolationIncentive>()
                        .ToArray(),
                    PropertyTypeInput?.SelectedItem?.ToString()!,
                    (int)((IntegerTextBox)ResidentsCountInput.Element).Value,
                    out double taxValue,
                    out double addedByPropertyType,
                    out double addedByIncentives
                );
                bool addedByPropertyTypeShowTotal = TaxContributionByIncentivesResult.Visibility != Visibility.Collapsed;
                additionFormat(TaxContributionByPropertyTypeResult, addedByPropertyType, (taxValue + addedByPropertyType), addedByPropertyTypeShowTotal);
                additionFormat(TaxContributionByIncentivesResult, addedByIncentives, finalTaxValue);

                // original value
                if (
                    (TaxContributionByPropertyTypeResult.Visibility != Visibility.Collapsed)
                    || (TaxContributionByIncentivesResult.Visibility != Visibility.Collapsed)
                ) {
                    TaxContributionResult.Result = taxValue.ToString();
                    TaxContributionResult.Visibility = Visibility.Visible;
                } else {
                    TaxContributionResult.Visibility = Visibility.Collapsed;
                }

                // final value
                FinalTaxContributionResult.Result = Math.Round((double)finalTaxValue, roundingDecimals).ToString();
            }
        }

        private void ResetFinalResults() {
            // purchase
            PurchaseValueResult.Visibility = Visibility.Collapsed;
            PurchaseValueByIncentivesResult.Visibility = Visibility.Collapsed;
            FinalPurchaseValueResult.Result = FinalPurchaseValueResult.DefaultResult;
            setApprovedCheck(0);

            // property size
            PropertySizeResult.Result = Property.PropertySize.Invalid;

            // tax
            TaxContributionResult.Visibility = Visibility.Collapsed;
            TaxContributionByPropertyTypeResult.Visibility = Visibility.Collapsed;
            TaxContributionByIncentivesResult.Visibility = Visibility.Collapsed;
            FinalTaxContributionResult.Result = FinalTaxContributionResult.DefaultResult;
        }

        #endregion

        // -- Per-Item Methods --
        #region Per-Item Methods

        // - per item -

        private void OwningPlayerInput_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            Validity[nameof(OwningPlayerInput)].IsValid = (OwningPlayerInput.SelectedIndex != -1);
            NameInput_TextChanged(new(), EventArgs.Empty); // trigger name input check
                                                           // automatically updates createbutton enabled
        }

        public void UpdateOwningPlayerInputItems() {
            // hold cursor item
            object? oldItem = OwningPlayerInput.SelectedItem;

            // sets the items to the players displays list of players where the name is displayed and a copy of the primary ID is held alongside it
            OwningPlayerOptions = MainResources.PlayersDisplay.ClassDataList.Select(
                player => new KeyValuePair<string, IDPrimary>(player.Name.Value, player.DisplayableID)
            ).ToDictionary();
            OwningPlayerInput.ItemsSource = OwningPlayerOptions.Select(player => player.Key);

            // set cursor by item
            int position = -1;
            if (oldItem != null) {
                position = OwningPlayerInput.Items.IndexOf(oldItem); // returns -1 if not found
            } 
            OwningPlayerInput.SelectedIndex = position;

            // updated
            OwningPlayerInputUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void NameInput_TextChanged(object sender, EventArgs args) {
            // checks to see if the name has been used before and sets validity based on that

            // get textbox
            ColorValidatedTextBox nameInputTextBox = ((ColorValidatedTextBox)NameInput.Element);

            // checks basic validity
            nameInputTextBox.Validate(nameInputTextBox, EventArgs.Empty); // manually checks validity
            bool isValid = nameInputTextBox.IsValid ?? false; // gets validity

            // only checks duplicate names when already valid
            if (
                (isValid)
                && (OwningPlayerInput.SelectedIndex != -1)
            ) {
                // true if name not used
                isValid = !MainResources.PropertiesDisplay.NameAlreadyUsed(
                    NameInput.Text.Trim(),
                    GetOwningPlayerID()
                );
                nameInputTextBox.IsValid = isValid; // set text box validity
            }

            // set Validity based on isValid state
            Validity[nameof(NameInput)].IsValid = isValid;
            UpdateCreateButtonEnabledStatus();
        }

        private void PropertyTypeInput_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            var propertyTypeInput = (ComboLabel)sender;

            void MakeVisible() {
                ResidentsCountInput.Visibility = Visibility.Visible;
            }

            void MakeInvisible() {
                ResidentsCountInput.Visibility = Visibility.Collapsed;
            }

            // update to invisible if -1
            if (propertyTypeInput.SelectedIndex == -1) {
                MakeInvisible();

            // update the validity for the residents count if it's not collapsed
            } else if (Property.PlayerPropertyTypes[propertyTypeInput.SelectedIndex] == Property.SharedPrivate) {
                MakeVisible();
            } else {
                MakeInvisible();
            }

            // set validity for this
            Validity[nameof(PropertyTypeInput)].IsValid = (propertyTypeInput.SelectedIndex != -1);
            CheckAndSetSectionsValidity();
            UpdateCreateButtonEnabledStatus();
            UpdateFinalResults();
        }

        private void ResidentsCountInput_InputFinalized(object sender, EventArgs args)
            => UpdateFinalResults();

        private void HideSubItemScrollPassToParent(object sender, MouseWheelEventArgs args) {
            args.Handled = true;

            // Manually raise MouseWheel on parent
            var eventArg = new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta) {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };

            MainScrollViewer.RaiseEvent(eventArg);
        }

        private void TaxIncentives_IncentivesChanged(object sender, EventArgs args) 
            => UpdateFinalResults();

        private void TaxIncentives_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
            => HideSubItemScrollPassToParent(sender, args);

        private void ViolationIncentives_IncentivesChanged(object sender, EventArgs args) 
            => UpdateFinalResults();

        private void ViolationIncentives_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
            => HideSubItemScrollPassToParent(sender, args);

        private void PurchaseIncentives_IncentivesChanged(object sender, EventArgs args) 
            => UpdateFinalResults();

        private void PurchaseIncentives_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
            => HideSubItemScrollPassToParent(sender, args);

        private void HasMailboxCheck_CheckChanged(object sender, BoolEventArgs args) {
            Validity[nameof(HasMailboxCheck)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        private void HasEdgeSpacingCheck_CheckChanged(object sender, BoolEventArgs args) {
            Validity[nameof(HasEdgeSpacingCheck)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        private void ApprovedCheck_CheckChanged(object sender, BoolEventArgs args) {
            Validity[nameof(ApprovedCheck)].IsValid = args.Value ?? false;
            UpdateCreateButtonEnabledStatus();
        }

        #endregion
    }
}
