using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class SearchableListDisplay<T> : ListDisplay<T>, IEnumerable<T>
        where T : Displayable {

        // --- VARIABLES ---

        // -- Structures --
        #region Structures

        private const string AllOptionPropertyName = "_";

        private record SearchComboItem(
            string DisplayName,
            string PropertyName
        );

        #endregion

        // -- Values --
        #region Values

        private static GridLength SearchComboColumnWidth { get; } = new GridLength(1, GridUnitType.Star);

        // - Class Data List (JsonIgnore) -

        [JsonIgnore]
        public override NotifyingList<T> ClassDataList {
            get => base.ClassDataList;
            set => base.ClassDataList = value;
        }

        // - Searchable Class Data List -

        [JsonProperty("searchable_data")]
        public NotifyingList<T> SearchableClassDataList { get; set; } = new();

        public override T SomeClassData => SearchableClassDataList[0];

        public override IEnumerator<T> GetEnumerator() => SearchableClassDataList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // - Data List by Rows -

        /// <summary>
        /// A list of IDisplayValues organized by row
        /// </summary>
        public new List<Dictionary<string, DisplayValueBase>> DataListByRows {
            get {
                return SearchableClassDataList.Select(
                    cls => cls.DisplayValues
                ).ToList();
            }
        }

        // - Index by Index -

        /// <summary>
        /// Accesses the data list by index
        /// </summary>
        /// <param name="index"> The element to retrieve </param>
        /// <returns> A dictionary element with a header name and it's associated DisplayValueBase </returns>
        public new T this[int index] {
            get => SearchableClassDataList[index];
            set => SearchableClassDataList[index] = value;
        }

        // - Count -

        public override int Count => SearchableCount;

        public int SearchableCount => SearchableClassDataList.Count;

        // - Data List by Columns -

        /// <summary>
        /// A list of IDisaplayValues organized by columns
        /// </summary>
        public new Dictionary<string, List<DisplayValueBase>> DataListByColumns {
            get {
                return SomeClassData.DisplayHeaders.ToDictionary(
                    key => key, // key
                    key => SearchableClassDataList.Select( // value
                        cls => cls.DisplayValues[key]
                    ).ToList()
                );
            }
        }

        // - Index by Headers -

        /// <summary>
        /// Gets the headers for this list
        /// </summary>
        public override ImmutableList<string> Headers {
            get {
                if (SearchableClassDataList.Count > 0) {
                    // gets the display headers in the order of their display order 
                    // this means that the display will be built in the order the headers are in
                    return SomeClassData.DisplayHeaders
                        .Zip(
                            SomeClassData.DisplayOrders, (header, order) => {
                                return new HeaderOrderPair(header, order.Value);
                            }
                        )
                        .OrderBy(pair => pair.order)
                        .Select(pair => pair.header)
                        .ToImmutableList();
                } else {
                    return ImmutableList<string>.Empty;
                }
            }
        }

        // - Search Container Grid -

        public Grid SearchContainerGrid { get; } = new();

        // - Search Bar -

        public TextLabel SearchBar { get; } = new();

        // - Search Combo -

        public ComboLabel SearchCombo { get; } = new();

        #endregion

        // -- Interface --
        #region Interface

        public bool HidePerHeaderSearchCombo {
            get => (bool)GetValue(HidePerHeaderSearchComboProperty);
            set => SetValue(HidePerHeaderSearchComboProperty, value);
        }

        public static readonly DependencyProperty HidePerHeaderSearchComboProperty = DependencyProperty.Register(
            nameof(HidePerHeaderSearchCombo),
            typeof(bool),
            typeof(SearchableListDisplay<T>),
            new PropertyMetadata(false, OnHidePerHeaderSearchComboChanged)
        );

        private static void OnHidePerHeaderSearchComboChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is SearchableListDisplay<T> control) {
                control.UpdateSearchComboVisibility();
            }
        }

        #endregion

        // -- Exposures --
        #region Exposures

        // - Search Bar Text Max Length -

        [Category("Common")]
        [Description("The MaxLength of the search bar's text")]
        public int SearchBarTextMaxLength {
            get => (int)GetValue(SearchBarTextMaxLengthProperty);
            set => SetValue(SearchBarTextMaxLengthProperty, value);
        }

        public static readonly DependencyProperty SearchBarTextMaxLengthProperty = DependencyProperty.Register(
            nameof(SearchBarTextMaxLength),
            typeof(int),
            typeof(SearchableListDisplay<T>),
            new PropertyMetadata(-1, OnSearchBarTextMaxLengthPropertyChanged)
        );

        private static void OnSearchBarTextMaxLengthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is SearchableListDisplay<T> searchableListDisplay) {
                searchableListDisplay.SearchBar.TextBoxMaxLength = (int)args.NewValue;
            }
        }

        // - Highlight Upon Tab -

        public bool SearchBarHighlightUponTab {
            get => (bool)GetValue(SearchBarHighlightUponTabProperty);
            set => SetValue(SearchBarHighlightUponTabProperty, value);
        }

        public static readonly DependencyProperty SearchBarHighlightUponTabProperty = DependencyProperty.Register(
            nameof(SearchBarHighlightUponTab),
            typeof(bool),
            typeof(SearchableListDisplay<T>),
            new PropertyMetadata(false, OnSearchBarHightlightUponTabPropertyChanged)
        );

        private static void OnSearchBarHightlightUponTabPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is SearchableListDisplay<T> searchableListDisplay) {
                searchableListDisplay.SearchBar.HighlightUponTabFromTextLabel = (bool)args.NewValue;
            }
        }

        // - Highlight Upon Click -

        public bool SearchBarHighlightUponClick {
            get => (bool)GetValue(SearchBarHighlightUponClickProperty);
            set => SetValue(SearchBarHighlightUponClickProperty, value);
        }

        public static readonly DependencyProperty SearchBarHighlightUponClickProperty = DependencyProperty.Register(
            nameof(SearchBarHighlightUponClick),
            typeof(bool),
            typeof(SearchableListDisplay<T>),
            new PropertyMetadata(false, OnSearchBarHighlightUponClickChanged)
        );

        private static void OnSearchBarHighlightUponClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is SearchableListDisplay<T> searchableListDisplay) {
                searchableListDisplay.SearchBar.HighlightUponClickFromTextLabel = (bool)args.NewValue;
            }
        }

        #endregion

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        protected override void ExtraSetupOnDataSet() {
            base.ExtraSetupOnDataSet();

            // mirror class data list to the searchable class data list if the searchable list is empty
            // (by the end of set class data list)
            if (SearchableClassDataList.Count == 0) {
                SearchableClassDataList = ClassDataList.CopyShallow();
            }

            // unsubscribe item added method
            ClassDataList.ItemAdded -= OnItemAdded;

            // resubscribe new item added method
            SearchableClassDataList.ItemAdded += OnItemAdded;

            // run for every instance on the searchable list contents
            foreach (var instance in SearchableClassDataList) {
                ForAllLoadedRowsAndNewItems(instance);
            }

            // update data list based on search after data has been set
            // updating should automatically mirror searchable data to class data for the first time
            UpdateForSearchSettings();
            // this happens before the grid is built but before it's loaded
        }

        protected override void ExtraSetupOnLoad() {
            base.ExtraSetupOnLoad();

            // originally set the header combo options
            UpdateSearchCombo();
        }

        private void SetDefaultValues() {
            // change the content from being the maingrid to the search container grid
            this.Content = SearchContainerGrid;

            // row setup for search container grid
            SearchContainerGrid.RowDefinitions.Add(new() {
                Name = "SearchBarRow",
                Height = new GridLength(1, GridUnitType.Auto)
            });
            SearchContainerGrid.RowDefinitions.Add(new() {
                Name = "DisplayRow",
                Height = new GridLength(1, GridUnitType.Star)
            });

            // column setup for search container grid
            SearchContainerGrid.ColumnDefinitions.Add(new() {
                Name = "ComboColumn",
                Width = SearchComboColumnWidth
            });
            SearchContainerGrid.ColumnDefinitions.Add(new() {
                Name = "BarColumn",
                Width = new GridLength(2, GridUnitType.Star)
            });

            // map the MainGrid as the contents of the search container grid
            SearchContainerGrid.Children.Add(MainGrid);
            Grid.SetRow(MainGrid, 1);
            Grid.SetColumn(MainGrid, 0);
            Grid.SetColumnSpan(MainGrid, 2);

            // add the search bar
            SearchContainerGrid.Children.Add(SearchBar);
            Grid.SetRow(SearchBar, 0);
            Grid.SetColumn(SearchBar, 1);
            Grid.SetColumnSpan(SearchBar, 1);

            // add the search combo
            SearchContainerGrid.Children.Add(SearchCombo);
            Grid.SetRow(SearchCombo, 0);
            Grid.SetColumn(SearchCombo, 0);
            Grid.SetColumnSpan(SearchCombo, 1);

            // main grid settings
            MainGrid.Margin = new Thickness(3);

            // set up search bar
            SearchBar.LayoutMode = TextLabel.LabeledInputLayoutModes.Above;
            SearchBar.LabelText = "Search";
            SearchBar.TextChanged += OnSearchBar_TextChanged;

            // set up search combo
            SearchCombo.LayoutMode = ComboLabel.LabeledInputLayoutModes.Above;
            SearchCombo.LabelText = "Search By";
            SearchCombo.DisplayMemberPath = "DisplayName";
            DisplayLayerChanged += (_, _) => UpdateSearchCombo(); // update the search by options when the display changes
            SearchBarHighlightUponClick = true;
            SearchBarHighlightUponTab = true;
            SearchCombo.SelectionChanged += SearchCombo_SelectionChanged;
        }

        public SearchableListDisplay()
            : base() => SetDefaultValues();

        public SearchableListDisplay(SearchableListDisplay<T> cls)
            : base(cls) => SetDefaultValues();

        #endregion

        // --- METHODS ---

        // -- Operations --
        #region Operations

        public override void Add(T cls) {
            SearchableClassDataList.Add(cls);
            BuildGrid();
        }

        public override void Remove(T cls) {
            SearchableClassDataList.Remove(cls);
            BuildGrid();
        }

        public override void Clear() {
            SearchableClassDataList.Clear();
            BuildGrid();
        }

        public new int IndexOf(T displayable)
            => SearchableClassDataList.IndexOf(displayable);

        #endregion

        // -- Updates and General --
        #region Updates and General

        // - Search Cancellation Variables -

        private bool SearchCancel = false;

        private bool IncompletedSearch => (RunningCount != 0);

        private ushort RunningCount = 0;

        // - Update For Search Settings -

        private void UpdateForSearchSettings() {
            // set status
            RunningCount++;

            // check if there's no search
            if (
                (SearchCombo.SelectedIndex == -1)
                && (string.IsNullOrWhiteSpace(SearchBar.Text))
            ) {
                ClassDataList = SearchableClassDataList.CopyShallow();
                goto success;
            }

            // cancel check
            if (SearchCancel) { goto cancel_search; }

            // format search text
            string searchText = SearchBar.Text
                .Trim()
                .ToLower();

            // filter data by 'search by' selection
            bool searchAll = true;
            SearchComboItem? selectedItem = null;
            if (SearchCombo.Visibility == Visibility.Visible) {
                if (SearchCombo.SelectedItem is SearchComboItem) {
                    selectedItem = (SearchComboItem)SearchCombo.SelectedItem;
                    if (selectedItem == null) {
                        throw new NullReferenceException("SearchCombo.SelectedItem was null; SearchCombo selection was not a valid index (it cannot be -1)");
                    }

                    searchAll = (selectedItem.PropertyName == AllOptionPropertyName);
                } // search all
            } // search all

            // filter data
            const string invalidCompareText = "";
            const MemberTypes memberTypeFlags = MemberTypes.Field | MemberTypes.Property;
            const BindingFlags memberPrivacyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ClassDataList = NotifyingList<T>.From(SearchableClassDataList.Where(
                cls => {
                    // helper method for getting text extraction from an object bound display value
                    // - expects a 'BoundDisplayValue' as it's selected property and get's the value of it's 'Value'
                    string getValueString(MemberInfo memberInfo, Displayable parent) {

                        // cancel check
                        if (SearchCancel) { goto cancel_search_sub_2; }

                        // get bound display value as an object
                        object? boundDisplayValue;
                        if (memberInfo is PropertyInfo propertyInfo) {
                            boundDisplayValue = propertyInfo.GetValue(parent);
                        } else if (memberInfo is FieldInfo fieldInfo) {
                            boundDisplayValue = fieldInfo.GetValue(parent);
                        } else {
                            return invalidCompareText;
                        }

                        // invalid bound display value
                        if (boundDisplayValue is null) {
                            throw new InvalidOperationException("boundDisplayValue is not a possible value");
                        }

                        // cancel check
                        if (SearchCancel) { goto cancel_search_sub_2; }

                        // get the value property off of it
                        MemberInfo? subMemberInfo = boundDisplayValue
                            .GetType()
                            .GetMember(
                                "Value",
                                memberTypeFlags,
                                memberPrivacyFlags
                            ).FirstOrDefault();
                        if (subMemberInfo is null) {
                            throw new InvalidOperationException("subMemberInfo is not a possible value");
                        }

                        // get the value of 'Value' 
                        // expects 'Value' as the selected property and gets it's value
                        if (subMemberInfo is PropertyInfo subPropertyInfo) {
                            return subPropertyInfo.GetValue(boundDisplayValue)?.ToString() ?? invalidCompareText;
                        } else if (subMemberInfo is FieldInfo subFieldInfo) {
                            return subFieldInfo.GetValue(boundDisplayValue)?.ToString() ?? invalidCompareText;
                        } else {
                            return invalidCompareText;
                        }

                        // cancel logic
                        cancel_search_sub_2:
                            return invalidCompareText;
                    }

                    // cancel check
                    if (SearchCancel) { goto cancel_search_sub; }

                    // check if all or specific
                    string compareText = "";
                    if (searchAll) {
                        foreach (SearchComboItem searchComboItem in SearchCombo.Items) {
                            // skip 'all'
                            if (searchComboItem.PropertyName == AllOptionPropertyName) { continue; }

                            // cancel check
                            if (SearchCancel) { goto cancel_search_sub; }

                            compareText += $"\0{getValueString( // adding will make searched work for any
                                cls.GetType()
                                    .GetMember(
                                        searchComboItem.PropertyName,
                                        memberTypeFlags,
                                        memberPrivacyFlags
                                    ).FirstOrDefault()!,
                                cls
                            )}"; 
                        } // no properties will result in compareText staying default
                    } else {
                        // get the value for the selected search property
                        // - property will be only the right type because we're using the property name of the selected item
                        compareText = getValueString(
                            cls.GetType()
                                .GetMember(
                                    selectedItem!.PropertyName,
                                    memberTypeFlags,
                                    memberPrivacyFlags
                                ).FirstOrDefault()!,
                            cls
                        );
                    }

                    // cancel check
                    if (SearchCancel) { goto cancel_search_sub; }

                    // check equality of text
                    if (string.IsNullOrWhiteSpace(compareText)) { return false; }
                    return (compareText
                        .ToLower()
                        .Contains(searchText));

                    // cancel logic
                    cancel_search_sub:
                        return false;
                }
            ));

            // cancel check
            if (SearchCancel) { goto cancel_search; }

            // success, build changes
            success:
                RunningCount--;
                base.BuildGrid();
                return;

            // cancel logic
            cancel_search:
                // decrement running count
                RunningCount--;

                // start a new search if this was the last search to cancel
                if (!IncompletedSearch) {
                    UpdateForSearchSettings();
                }
                // finish
                return;
        }

        // - Update Search Combo Options -

        private void UpdateSearchCombo() {
            UpdateSearchComboVisibility();
            UpdateSearchComboOptions();
        }

        private void UpdateSearchComboVisibility() {
            GridLength emptyGridColumnLength = new GridLength(0, GridUnitType.Pixel);
            if (HidePerHeaderSearchCombo) {
                SearchCombo.Visibility = Visibility.Collapsed;
                SearchContainerGrid.ColumnDefinitions[0].Width = emptyGridColumnLength;
            } else {
                if (VisibleHeaders.Count <= 1) {
                    SearchCombo.Visibility = Visibility.Collapsed;
                    SearchContainerGrid.ColumnDefinitions[0].Width = emptyGridColumnLength;
                } else {
                    SearchCombo.Visibility = Visibility.Visible;
                    SearchContainerGrid.ColumnDefinitions[0].Width = SearchComboColumnWidth;
                }
            }
        }

        private void UpdateSearchComboOptions() {
            // find options
            List<SearchComboItem> options = VisibleHeaders
                .Where(header => { // must be bound display value
                    DisplayValueBase displayValue = SomeClassData.DisplayValues[header];
                    return (
                        (displayValue?.GetType().IsGenericType == true)
                        && (displayValue.GetType().GetGenericTypeDefinition() == typeof(BoundDisplayValue<,>))
                    );
                })
                .Select(header => new SearchComboItem( // selects as a new search combo item
                    header, // display name
                    SomeClassData.PropertyNames[header] // property name
                ))
                .ToList();

            // add all option
            options.Insert(0, new SearchComboItem(
                "All",
                AllOptionPropertyName
            ));

            // set options
            SearchCombo.ItemsSource = options;

            // set to 'all' option by default
            if (SearchCombo.SelectedIndex == -1) {
                SearchCombo.SelectedIndex = 0;
            }
        }

        // - Build Grid -

        public new void BuildGrid() {
            UpdateForSearchSettings(); // automatically builds grid when complete
        }

        // - Is Valid Current Display Layer Override -

        protected override bool IsValidCurrentDisplayLayer(string header) {
            // display layer checking
            if (DisplayLayer == -1) { return true; }

            // check if the display layer being called for exists
            if (!SomeClassData.DisplayLayers
                .Select(displayLayers => displayLayers.Value)
                .Any(displayLayers => displayLayers.Contains(DisplayLayer))
            ) {
                throw new ArgumentException($"DisplayLayer is associated with no headers: {DisplayLayer}");
            }

            // check if display layer doesn't match
            if (!SomeClassData.DisplayLayers[header].Contains(DisplayLayer)) { return false; }
            return true;
        }

        #endregion

        // -- Per-Item Methods --
        #region Per-Item Methods

        private void OnSearchBar_TextChanged(object? sender, EventArgs args) {
            if (IncompletedSearch) {
                SearchCancel = true;
            } else {
                UpdateForSearchSettings();
            }
        }

        private void SearchCombo_SelectionChanged(object? sender, SelectionChangedEventArgs args)
            => UpdateForSearchSettings();

        #endregion
    }
}
