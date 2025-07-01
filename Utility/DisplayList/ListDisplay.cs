using MC_BSR_S2_Calculator.MainColumn.LandTracking;
using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Json;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {

    /// <summary>
    /// Contains data about grid composition and the list of data to display; 
    /// Generates grid from associated data;
    /// Default display structure for ListDisplays
    /// </summary>
    /// <typeparam name="T"> The class type of which data to display from </typeparam>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract partial class ListDisplay<T> : UserControl
        where T : Displayable {

        // --- VARIABLES ---
        #region VARIABLES

        // -- Data Holders --

        // - Data List -

        /// <summary>
        /// Holds a list of classes to display data from
        /// </summary>
        [JsonProperty("data")]
        public virtual NotifyingList<T> ClassDataList { get; set; } = new();

        // - Data List by Rows -

        /// <summary>
        /// A list of IDisplayValues organized by row
        /// </summary>
        public List<Dictionary<string, DisplayValueBase>> DataListByRows {
            get {
                return ClassDataList.Select(
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
        public T this[int index] {
            get => ClassDataList[index];
            set => ClassDataList[index] = value;
        }

        // - Count -

        public int Count {
            get => ClassDataList.Count;
        }

        // - Data List by Columns -

        /// <summary>
        /// A list of IDisaplayValues organized by columns
        /// </summary>
        public Dictionary<string, List<DisplayValueBase>> DataListByColumns {
            get {
                return ClassDataList[0].DisplayHeaders.ToDictionary(
                    key => key, // key
                    key => ClassDataList.Select( // value
                        cls => cls.DisplayValues[key]
                    ).ToList()
                );
            }
        }

        // - Index by Headers -

        /// <summary>
        /// Accesses the data list by header
        /// </summary>
        /// <param name="header"> The header of the elements to retrieve </param>
        /// <returns> A list of IDisplayValues associated with a header </returns>
        public List<DisplayValueBase> this[string header] {
            get => DataListByColumns[header];
            set => DataListByColumns[header] = value;
        }

        // - List Headers -

        private record HeaderOrderPair(
            string header,
            int order
        );

        /// <summary>
        /// Gets the headers for this list
        /// </summary>
        public ImmutableList<string> Headers {
            get {
               if (ClassDataList.Count > 0) {
                    // gets the display headers in the order of their display order 
                    // this means that the display will be built in the order the headers are in
                    return ClassDataList[0].DisplayHeaders
                        .Zip(
                            ClassDataList[0].DisplayOrders, (header, order) => {
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

        // - Column Widths -

        private ImmutableDictionary<string, GridLength> ColumnWidths {
            get {
                return ClassDataList[0].DisplayHeaders.ToImmutableDictionary(
                    key => key, // key
                    key => ClassDataList[0].ColumnWidths[key]
                );
            }
        }

        // - Rebuilt Event -

        public event EventHandler<EventArgs>? Rebuilt;

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        /// <remarks>
        /// Extensions of this class MUST define which exact data to display (parameterless constructor)
        /// </remarks
        public ListDisplay() {
            Loaded += (object sender, RoutedEventArgs args) => {
                // has been loaded
                if (HasBeenLoaded) { return; }
                HasBeenLoaded = true;

                // loads data list and builds grid
                SetClassDataList();
                ExtraSetup();
                BuildGrid();

                // sets the original scroll bar visibility
                SetOriginalScrollBarVisibilityOnLoaded(sender, args);

                // set the scroll bar visibility state when starting up
                if (OriginalScrollBarVisbility == ScrollBarVisibility.Visible) {
                    OnMainGridSizeChange(sender, args); // ensures the sizing immediately takes place
                } else {
                    // update the scroll bar state when starting up
                    OnShowScrollBarChanged(this, new DependencyPropertyChangedEventArgs(ShowScrollBarProperty, null, ShowScrollBar));
                }
            };

            // event handling for ensuring content never overlaps the scrollbar
            MainGrid.SizeChanged += OnMainGridSizeChange;
            ListDisplayGrid.ChildrenChanged += OnMainGridSizeChange;

            // set main content parents
            this.Content = MainGrid;

            // alignment settings for the grid
            ListDisplayGrid.VerticalAlignment = VerticalAlignment.Top;

            // add instance to the instances storable (if it's storable)
            if (typeof(IStorable).IsAssignableFrom(this.GetType())) {
                AddInstanceToInstances();
            }
        }

        // - minimizes IStorable requirements in extended classes -

        public void AddInstanceToInstances() => IStorable.Instances.Add((IStorable)this);

        // - class data setter -

        /// <remarks>
        /// this method MUST set ClassDataList
        /// </remarks>
        protected abstract void SetClassDataList();

        // - row and new item helper overridable -

        private void ExtraSetup() {
            // expose scroll wheel updates
            //MainScrollViewer.PreviewMouseWheel += (_, args) => {
            //    PreviewMouseWheel.Invoke(this, args);
            //};

            // on item added
            ClassDataList.ItemAdded += (sender, args) => {
                if (args is ListChangedEventArgs argsCasted) {
                    ForAllLoadedRowsAndNewItems(ClassDataList[argsCasted.NewIndex]);
                } else { throw new ArgumentException($"the arguments send by the ItemsAdded event weren't of the correct type"); }
            };

            // for every instance on load
            foreach (var instance in ClassDataList) {
                ForAllLoadedRowsAndNewItems(instance);
            }
        }

        /// <summary>
        /// runs extra setup for items which are either newly added to the list or pre-loaded via JSON
        /// </summary>
        /// <remarks>
        /// Does nothing by default
        /// </remarks>
        /// <param name="instance"> An instance of an object contained in the list </param>
        protected virtual void ForAllLoadedRowsAndNewItems(T instance) { }

        #endregion

        // --- METHODS ---

        // -- Operations --
        #region Operations

        // - Add -
        public void Add(T cls) {
            ClassDataList.Add(cls);
            BuildGrid();
        }

        // - Remove -

        public void Remove(T cls) {
            ClassDataList.Remove(cls);
            BuildGrid();
        }

        public void RemoveAt(int index) {
            // range check
            if (
                (index >= ClassDataList.Count)
                || (index < 0)
            ) {
                throw new ArgumentOutOfRangeException(nameof(index), index, "The provided index was out of range of the ClassDataList");
            }

            // remove at
            ClassDataList.RemoveAt(index);
        }

        // - Clear -

        public void Clear() {
            ClassDataList.Clear();
            BuildGrid();
        }

        #endregion

        // -- General --
        #region General

        /// <summary>
        /// Changes the visibility and margin for the scrollbar and manages display max height when the layout is changed
        /// </summary>
        private void OnMainGridSizeChange(object? sender, EventArgs args) {
            // designer check
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }

            // validate max display height
            MaxDisplayHeight = (MaxDisplayHeight < -1) ? -1 : MaxDisplayHeight;
            if ((MaxDisplayHeight != -1) && (MaxDisplayHeight < 50)) { throw new ArgumentException($"The MaxDisplayHeight cannot be les than 50"); }

            // - main grid height managament -

            // check max display height (-1 has no max)
            if ((MaxDisplayHeight != -1) && (MainGrid.ActualHeight > MaxDisplayHeight)) {
                // set list display and scroll to the max
                MainScrollViewer.Height = (
                    MaxDisplayHeight
                    - (MainBorderThickness.Bottom + MainBorderThickness.Top)
                );
                MainBorder.Height = MaxDisplayHeight;

                // stretching
                MainScrollViewer.VerticalAlignment = VerticalAlignment.Top;
                MainBorder.VerticalAlignment = VerticalAlignment.Top;
            } else {
                // height adjustments
                MainScrollViewer.ClearValue(FrameworkElement.HeightProperty);
                MainBorder.ClearValue(FrameworkElement.HeightProperty);

                // stretching
                MainScrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
                MainBorder.VerticalAlignment = VerticalAlignment.Stretch;
            }

            // - scroll bar management -

            // find the content height
            double listContentTotalHeight = (
                ((ListDisplayGrid.RowDefinitions.Count - 1) * ItemHeight)
                + HeaderHeight
                + MainBorderThickness.Top + MainBorderThickness.Bottom
            );

            // not null check
            if (OriginalScrollBarVisbility != null) {

                // check if original was auto
                if (OriginalScrollBarVisbility == ScrollBarVisibility.Auto) {
                    // auto show/hide scrollbar
                    if (listContentTotalHeight > MainBorder.ActualHeight) { // if listContentTotalHeight of the grid is too much
                                                                            // (can check either MainScrollViewer or ListDisplayGrid or MainBorder)
                        MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    } else { // listContentTotalHeight is too small
                        MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    }

                    // original wasn't auto
                } else {
                    if (OriginalScrollBarVisbility != null) {
                        MainScrollViewer.VerticalScrollBarVisibility = (ScrollBarVisibility)OriginalScrollBarVisbility;
                    }
                }

                // set margin
                if (MainScrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Visible) {
                    ListDisplayGrid.Margin = new Thickness(0, 0, 3, 0);
                } else {
                    ListDisplayGrid.Margin = new Thickness(0);
                }
            }
        }

        #endregion

        // -- Grid Building --

        // - general grid building -
        #region general grid building

        private int YcurrBuildPosition = 0;
        private int YcurrItemRow { get => YcurrBuildPosition + 1; }
        private int XcurrBuildPosition = 0;

        // grid additions helper
        private void AddToCurrentItem(FrameworkElement objectToAdd, int? rowOverride = null, int? columnOverride = null) {
            ListDisplayGrid.Children.Add(objectToAdd);
            Grid.SetColumn(
                objectToAdd,
                ((columnOverride != null) ? (int)columnOverride : XcurrBuildPosition)
            );
            Grid.SetRow(
                objectToAdd,
                ((rowOverride != null) ? (int)rowOverride : YcurrBuildPosition)
            );
        }

        protected virtual void SortClassData() {
            // no sorting default
        }

        private void PrepareGridForBuilding() {
            // clear current grid contents
            MainGrid.Children.Clear();
            ListDisplayGrid.Children.Clear();
            ListDisplayGrid.ColumnDefinitions.Clear();
            ListDisplayGrid.RowDefinitions.Clear();

            // content relationships
            MainGrid.Children.Add(MainBorder);
            MainGrid.Children.Add(MainScrollViewer);
            MainScrollViewer.Content = ListDisplayGrid;

            // border settings
            MainBorder.BorderThickness = MainBorderThickness;
            MainBorder.BorderBrush = MainBorderBrush;
            MainBorder.Background = MainBorderBackground;

            // scroll bar settings
            Application.Current.Dispatcher.InvokeAsync(() => {
                // find scroll bar
                ScrollBar? scrollBar = XamlConverter.FindVerticalScrollBar(MainScrollViewer);
                if (scrollBar == null) { throw new ArgumentException($"MainScrollViewer didn't contain a ScrollBar"); }

                // scroll width
                scrollBar.MinWidth = 10;
                scrollBar.Width = ScrollBarWidth;
            }, DispatcherPriority.Loaded);

            // scroll viewer margins
            MainScrollViewer.Margin = MainBorder.BorderThickness;

            // if class data is null or empty
            if ((ClassDataList == null) || (ClassDataList.Count == 0)) {
                // create empty and add
                var emptyTextBlock = CreateEmptyTextBlock(false, HorizontalAlignment.Stretch);
                ListDisplayGrid.Children.Add(emptyTextBlock);
                return; // dont add anything else
            }

            // headers row
            ListDisplayGrid.RowDefinitions.Add(new RowDefinition());
        }

        private TextBlock CreateEmptyTextBlock(bool isPerHeader, HorizontalAlignment headerHorizontalAlignment) {
            // main create
            var emptyTextBlock = new TextBlock();
            emptyTextBlock.Foreground = HeaderFontColor;
            emptyTextBlock.VerticalAlignment = VerticalAlignment.Top;
            emptyTextBlock.TextWrapping = TextWrapping.Wrap;

            // generate per header or main block
            if (isPerHeader) {
                // settings
                emptyTextBlock.Text = EmptyHeaderText;
                emptyTextBlock.FontSize = ItemHeight * 0.5;
                emptyTextBlock.HorizontalAlignment = headerHorizontalAlignment;
                emptyTextBlock.Margin = new Thickness(
                    ItemBorderThickness.Left + ItemMargin,
                    ItemBorderThickness.Top + ItemMargin,
                    ItemBorderThickness.Right + ItemMargin,
                    ItemBorderThickness.Bottom + ItemMargin
                );

                // return
                return emptyTextBlock;
            } else {
                // settings
                emptyTextBlock.Text = EmptyText;
                emptyTextBlock.FontSize = HeaderHeight * 0.5;
                emptyTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                emptyTextBlock.Margin = new Thickness(
                    HeaderBorderThickness.Left + HeaderMargin,
                    HeaderBorderThickness.Top + HeaderMargin,
                    HeaderBorderThickness.Right + HeaderMargin,
                    HeaderBorderThickness.Bottom + HeaderMargin
                );

                // return
                return emptyTextBlock;
            }
        }

        private bool IsValidCurrentDisplayLayer(string header) {
            // display layer checking
            if (DisplayLayer == -1) { return true; }

            // check if the display layer being called for exists
            if (!ClassDataList[0].DisplayLayers
                .Select(displayLayers => displayLayers.Value)
                .Any(displayLayers => displayLayers.Contains(DisplayLayer))
            ) {
                throw new ArgumentException($"DisplayLayer is associated with no headers: {DisplayLayer}");
            }

            // check if display layer doesn't match
            if (!ClassDataList[0].DisplayLayers[header].Contains(DisplayLayer)) { return false; }
            return true;
        }

        /// <summary>
        /// Builds the associated grid display with the current data
        /// </summary>
        /// <remarks>
        /// Font Size is set to the listContentTotalHeight of it's associated grid
        /// </remarks>
        public void BuildGrid() {
            // prepare main grid
            PrepareGridForBuilding();
            SortClassData();

            // add headers
            AddHeaders();

            // add items 
            AddItemGridBottoms(); // add the item grid bottoms
            AddClickableButtons(); // add buttons if applicable
            AddItemGridTops(); // add the item grid tops

            // rebuilt
            Rebuilt?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        // - header grid building -
        #region header grid building

        private void AddHeaders() {
            // for every header
            bool usePrimaryHeaderColor = true;
            for (XcurrBuildPosition = 0; XcurrBuildPosition < Headers.Count; XcurrBuildPosition++) {
                var header = Headers[XcurrBuildPosition];

                // - setup -

                // get alignments
                var headerHorizontalAlignment = ClassDataList[0].ColumnContentAlignments[header].HorizontalAlignment;
                var headerVerticalAlignment = ClassDataList[0].ColumnContentAlignments[header].VerticalAlignment;

                // display layer checking
                if (!IsValidCurrentDisplayLayer(header)) { continue; }

                // - add headers -

                // create header
                Grid headerGrid = CreateHeaderGrid(header, ref usePrimaryHeaderColor, headerHorizontalAlignment, headerVerticalAlignment);

                // create a new column with each header
                ListDisplayGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                    Width = ColumnWidths[header],
                });

                // add header grid to main grid
                AddToCurrentItem(headerGrid, 0);

                // - add empty if applicable -

                // if the header is null or empty
                int maxItemsCount = ((header == null) || (DataListByColumns[header].Count == 0)) ? 0 : maxItemsCount = DataListByColumns.Values.Max(list => list.Count);
                if (maxItemsCount == 0) {
                    // create empty text box
                    var emptyTextBlock = CreateEmptyTextBlock(true, headerHorizontalAlignment);

                    // add rows if needed
                    if (ListDisplayGrid.RowDefinitions.Count < 2) {
                        ListDisplayGrid.RowDefinitions.Add(new RowDefinition());
                    }

                    // add empty to grid
                    AddToCurrentItem(emptyTextBlock, 1);
                    Grid.SetRowSpan(emptyTextBlock, maxItemsCount);

                    // dont add items
                    continue;
                }
            }
        }

        private Grid CreateHeaderGrid(
            string header,
            ref bool usePrimaryHeaderColor,
            HorizontalAlignment headerHorizontalAlignment,
            VerticalAlignment headerVerticalAlignment
        ) {
            // create grid
            var headerGrid = new Grid();
            headerGrid.Height = HeaderHeight;

            // create border
            var headerBorder = new Border();
            headerBorder.BorderThickness = HeaderBorderThickness;
            headerBorder.BorderBrush = HeaderBorderBrush;
            headerBorder.Background = (usePrimaryHeaderColor ? HeaderPrimaryFillBrush : HeaderSecondaryFillBrush);
            usePrimaryHeaderColor = !usePrimaryHeaderColor; // swap
            headerGrid.Children.Add(headerBorder);

            // create label
            var headerLabel = new Label();
            headerLabel.Content = header;
            headerLabel.FontSize = HeaderHeight * 0.5;
            headerLabel.Foreground = HeaderFontColor;
            var horiAlign = (headerHorizontalAlignment == HorizontalAlignment.Stretch) ? HorizontalAlignment.Center : headerHorizontalAlignment;
            headerLabel.HorizontalAlignment = horiAlign;
            headerLabel.HorizontalContentAlignment = horiAlign;
            var vertAlign = (headerVerticalAlignment == VerticalAlignment.Stretch) ? VerticalAlignment.Center : VerticalAlignment;
            headerLabel.VerticalAlignment = vertAlign;
            headerLabel.VerticalContentAlignment = vertAlign;
            headerLabel.Margin = new Thickness(
                HeaderBorderThickness.Left + HeaderMargin,
                HeaderBorderThickness.Top + HeaderMargin,
                HeaderBorderThickness.Right + HeaderMargin,
                HeaderBorderThickness.Bottom + HeaderMargin
            );
            headerGrid.Children.Add(headerLabel);

            // return
            return headerGrid;
        }

        #endregion

        // - item grid building - 
        #region item grid building

        // item grid bottom
        private void AddItemGridBottoms() {
            // iterate over every header
            bool isFirstPass = true;
            for (XcurrBuildPosition = 0; XcurrBuildPosition < Headers.Count; XcurrBuildPosition++) {
                // display layer checking
                if (!IsValidCurrentDisplayLayer(Headers[XcurrBuildPosition])) { continue; }

                // iterate over every class data for this header
                bool usePrimaryItemColor = true;
                for (YcurrBuildPosition = 0; YcurrBuildPosition < ClassDataList.Count; YcurrBuildPosition++) {
                    // create a new row with each header
                    if (isFirstPass) {
                        ListDisplayGrid.RowDefinitions.Add(new RowDefinition());
                    }

                    // create border (background)
                    var bottomItemBorder = new Border();
                    bottomItemBorder.Background = (usePrimaryItemColor ? ItemPrimaryFillBrush : ItemSecondaryFillBrush);
                    usePrimaryItemColor = !usePrimaryItemColor; // swap
                    AddToCurrentItem(bottomItemBorder, YcurrItemRow);
                }

                // no longer first pass
                isFirstPass = false;
            }
        }

        // clickable buttons
        private void AddClickableButtons() {
            // iterate over every header
            bool isLastPass = false;
            for (XcurrBuildPosition = 0; XcurrBuildPosition < Headers.Count; XcurrBuildPosition++) {
                string header = Headers[XcurrBuildPosition];

                // display layer checking
                if (!IsValidCurrentDisplayLayer(header)) { continue; }

                // set last past
                if (XcurrBuildPosition == Headers.Count - 1) {
                    isLastPass = true;
                }

                // button creation helper
                Button CreateClickableButton() {
                    // - re-create the button template so you can change it's overriden style -

                    // new template 
                    var template = new ControlTemplate(typeof(Button));

                    // create border
                    var border = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                    border.SetValue(Border.SnapsToDevicePixelsProperty, true);

                    // create content
                    var content = new FrameworkElementFactory(typeof(ContentPresenter));
                    content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                    content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Stretch);

                    // add content to border
                    border.AppendChild(content);

                    // add border to template
                    template.VisualTree = border;

                    // - regular settings modification -

                    // create button to host click event
                    Brush normalColor = Brushes.Transparent;
                    var button = new Button() {
                        OverridesDefaultStyle = true,
                        Background = normalColor
                    };
                    button.Template = template; // use the control template

                    // hover color
                    button.MouseEnter += (sender, args) => button.Background = ItemsHoverColor;
                    button.MouseLeave += (sender, args) => button.Background = normalColor;

                    // click color
                    button.PreviewMouseLeftButtonDown += (sender, args) => button.Background = ItemsClickColor;
                    button.PreviewMouseLeftButtonUp += (sender, args) => button.Background = ItemsHoverColor;

                    return button;
                }

                // iterate over every class data for this header
                for (YcurrBuildPosition = 0; YcurrBuildPosition < ClassDataList.Count; YcurrBuildPosition++) {
                    DisplayValueBase displayValue = DataListByColumns[header][YcurrBuildPosition];
                    T cls = ClassDataList[YcurrBuildPosition];

                    // button adder helper
                    Button AddButton(OptionalMouseClickEventHolder holder ) {
                        // create button and add event
                        var button = CreateClickableButton();
                        button.Click += holder.HeldLeftClickListener;

                        // if it's the last pass and row
                        if (IsClickable == ListDisplayClickable.ByRow) {
                            AddToCurrentItem(button, YcurrItemRow, 0);
                            Grid.SetColumnSpan(button, Headers.Count); // change span
                        } else {
                            AddToCurrentItem(button, YcurrItemRow);
                        }

                        // return button reference
                        return button;
                    }

                    // context wrapper helper
                    void AddContextWrapper(ContextMenu menu) {
                        // create wrapper, add context and add
                        var wrapperGrid = new Grid();
                        wrapperGrid.ContextMenu = menu;
                        wrapperGrid.Background = Brushes.Transparent;

                        // hover background coloring
                        wrapperGrid.MouseEnter += (s, e) => {
                            wrapperGrid.Background = new SolidColorBrush(Color.FromArgb(20, 255, 255, 255));
                        };
                        wrapperGrid.MouseLeave += (s, e) => {
                            wrapperGrid.Background = new SolidColorBrush(Colors.Transparent);
                        };

                        // if it's the last pass and row
                        if (ContextClickable == ListDisplayClickable.ByRow) {
                            AddToCurrentItem(wrapperGrid, YcurrItemRow, 0);
                            Grid.SetColumnSpan(wrapperGrid, Headers.Count); // change span
                        } else {
                            AddToCurrentItem(wrapperGrid, YcurrItemRow);
                        }
                    }

                    // if only buttons are clickable
                    if (
                        (IsClickable != ListDisplayClickable.NotClickable)
                        && (ContextClickable == ListDisplayClickable.NotClickable)
                    ) {
                        // if by cell
                        if (
                            (IsClickable == ListDisplayClickable.ByCell)
                            && (displayValue.IsHoldingLeftClick)
                        ) {
                            AddButton(displayValue);
                        }

                        // if by row
                        else if (
                            (IsClickable == ListDisplayClickable.ByRow)
                            && (isLastPass)
                            && (cls.IsHoldingRightClick)
                        ) {
                            AddButton(cls);
                        }

                        // if only contexts clickable
                    } else if (
                        (IsClickable == ListDisplayClickable.NotClickable)
                        && (ContextClickable != ListDisplayClickable.NotClickable)
                    ) {
                        // if by cell
                        if (
                            (ContextClickable == ListDisplayClickable.ByCell)
                            && (displayValue.IsHoldingLeftClick)
                        ) {
                            AddContextWrapper(displayValue.RightClickMenu);

                        // if by row
                        } else if (
                            (ContextClickable == ListDisplayClickable.ByRow)
                            && (isLastPass)
                            && (cls.IsHoldingRightClick)
                        ) {
                            AddContextWrapper(cls.RightClickMenu);
                        }
                    }

                    // if both are clickable
                    else if (
                        (IsClickable != ListDisplayClickable.NotClickable)
                        && (ContextClickable != ListDisplayClickable.NotClickable)
                    ) {
                        // if click styles dont match
                        if (IsClickable != ContextClickable) {
                            throw new ArgumentException($"button clickability and context clickability must match: button as {IsClickable}, context as {ContextClickable}");
                        }

                        // only run if it's the last pass if it's row
                        if ((IsClickable == ListDisplayClickable.ByRow) && (!isLastPass)) {
                            continue;
                        }

                        // select holder based on click status
                        OptionalMouseClickEventHolder holder = (IsClickable == ListDisplayClickable.ByRow) ? cls : displayValue;

                        // if both exist for this one
                        if (
                            (holder.IsHoldingLeftClick)
                            && (holder.IsHoldingRightClick)
                        ) {
                            Button button = AddButton(holder);
                            button.ContextMenu = holder.RightClickMenu;
                        }

                        // if only button exists for this one
                        else if (holder.IsHoldingLeftClick) {
                            AddButton(holder);
                        }

                        // if only context exists for this one
                        else if (holder.IsHoldingRightClick) {
                            AddContextWrapper(holder.RightClickMenu);
                        }
                    }
                }
            }
        }

        // item grid tops
        private void AddItemGridTops() {
            // iterate over every header
            for (XcurrBuildPosition = 0; XcurrBuildPosition < Headers.Count; XcurrBuildPosition++) {
                var header = Headers[XcurrBuildPosition];

                // display layer checking
                if (!IsValidCurrentDisplayLayer(header)) { continue; }

                // iterate over every class data for this header
                for (YcurrBuildPosition = 0; YcurrBuildPosition < ClassDataList.Count; YcurrBuildPosition++) {
                    // create border left/right (outline)
                    var topItemBorderSides = new Border();
                    topItemBorderSides.BorderThickness = new Thickness(
                        ItemBorderThickness.Left,
                        0,
                        ItemBorderThickness.Right,
                        0
                    );
                    topItemBorderSides.BorderBrush = ItemBorderBrushSides;
                    topItemBorderSides.IsHitTestVisible = false;
                    AddToCurrentItem(topItemBorderSides, YcurrItemRow);

                    // create border top/bottom (outline)
                    var topItemBorderEnds = new Border();
                    topItemBorderEnds.BorderThickness = new Thickness(
                        0,
                        ItemBorderThickness.Top,
                        0,
                        ItemBorderThickness.Bottom
                    );
                    topItemBorderEnds.BorderBrush = ItemBorderBrushEnds;
                    topItemBorderEnds.IsHitTestVisible = false;
                    AddToCurrentItem(topItemBorderEnds, YcurrItemRow);

                    // add display object
                    var displayObject = DataListByColumns[header][YcurrBuildPosition].DisplayObject;
                    displayObject.Margin = new Thickness(
                        ItemBorderThickness.Left + ItemMargin,
                        ItemBorderThickness.Top + ItemMargin,
                        ItemBorderThickness.Right + ItemMargin,
                        ItemBorderThickness.Bottom + ItemMargin
                    );
                    displayObject.HorizontalAlignment = ClassDataList[0].ColumnContentAlignments[header].HorizontalAlignment;
                    displayObject.VerticalAlignment = ClassDataList[0].ColumnContentAlignments[header].VerticalAlignment;
                    displayObject.IsHitTestVisible = ClassDataList[0].HitTestVisibilities[header];
                    if (displayObject.Parent != null) { ((Panel)displayObject.Parent).Children.Remove(displayObject); } // detach it from the current parent if it has one for some reason
                    AddToCurrentItem(displayObject, YcurrItemRow);
                }
            }
        }

        #endregion
    }
}
