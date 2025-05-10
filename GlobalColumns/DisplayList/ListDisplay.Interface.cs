using MC_BSR_S2_Calculator.GlobalColumns.DisplayList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
//using MC_BSR_S2_Calculator.SystemMetricsHeler;

namespace MC_BSR_S2_Calculator.GlobalColumns {
    internal abstract partial class ListDisplay<T> : UserControl 
        where T : Displayable {

        // --- VARIABLES ---

        private const int HeaderMargin = 3;
        private const int ItemMargin = 3;

        // -- Interface --
        #region Interface

        // - Main Grid -

        /// <summary>
        /// Main grid to store scroll viewer
        /// </summary>
        public Grid MainGrid { get; } = new();

        // - Border -

        /// <summary>
        /// Border for display grid
        /// </summary>
        public Border MainBorder { get; } = new();

        // - Scroll Viewer -

        /// <summary>
        /// Main scroll viewer
        /// </summary>
        public ScrollViewer MainScrollViewer { get; } = new();

        // - ListDisplayGrid -

        /// <summary>
        /// Main grid to place things into
        /// </summary>
        public WatchingGrid ListDisplayGrid { get; } = new();

        #endregion

        // -- Interface Settings -
        #region Interface Settings

        #region Scroll Bar

        // - Show Scroll Bar -

        public ScrollBarVisibility? OriginalScrollBarVisbility { get; set; }

        private void SetOriginalScrollBarVisibilityOnLoaded(object sender, RoutedEventArgs args) {
            // set original if not set
            if (OriginalScrollBarVisbility == null) {
                OriginalScrollBarVisbility = ShowScrollBar;
            }

            // update view for first frame
            MainScrollViewer.VerticalScrollBarVisibility = ShowScrollBar;
        }

        [Category("Layout")]
        [Description("Whether or not to show the Scroll Bar")]
        public ScrollBarVisibility ShowScrollBar {
            get => (ScrollBarVisibility)GetValue(ShowScrollBarProperty);
            set => SetValue(ShowScrollBarProperty, value);
        }

        public static readonly DependencyProperty ShowScrollBarProperty = DependencyProperty.Register(
            nameof(ShowScrollBar),
            typeof(ScrollBarVisibility),
            typeof(ListDisplay<T>),
            new PropertyMetadata(ScrollBarVisibility.Auto, OnShowScrollBarChanged)
        );

        private static void OnShowScrollBarChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if ((sender is ListDisplay<T> control) && (control.MainScrollViewer != null)) { // sender must hold MainScrollViewer and not be null
                // get new value
                ScrollBarVisibility newVisibility = (ScrollBarVisibility)args.NewValue;

                // update base visibility
                control.MainScrollViewer.VerticalScrollBarVisibility = newVisibility; // set value
            }
        }

        // - Scroll Bar Width -

        [Category("Layout")]
        [Description("Determines how wide should the Scroll Bar is")]
        public int ScrollBarWidth {
            get => (int)GetValue(ScrollBarWidthProperty);
            set => SetValue(ScrollBarWidthProperty, value);
        }

        public static readonly DependencyProperty ScrollBarWidthProperty = DependencyProperty.Register(
            nameof(ScrollBarWidth),
            typeof(int),
            typeof(ListDisplay<T>),
            new PropertyMetadata(SystemMetricsHelper.GetDefaultVerticalScrollBarWidth())
        );

        #endregion

        #region Border

        // - Border Thickness -

        [Category("Appearance")]
        [Description("Determines the thickness of the main Border")]
        public Thickness MainBorderThickness {
            get => (Thickness)GetValue(MainBorderThicknessProperty);
            set => SetValue(MainBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty MainBorderThicknessProperty = DependencyProperty.Register(
            nameof(MainBorderThickness),
            typeof(Thickness),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new Thickness(3), OnMainBorderThicknessPropertyChanged)
        );

        private static void OnMainBorderThicknessPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListDisplay<T> control) {
                // update property
                control.MainBorder.BorderThickness = (Thickness)args.NewValue;
            } 
        }

        // - Border Brush 

        [Category("Brush")]
        [Description("Determines the border brush of the main Border")]
        public SolidColorBrush MainBorderBrush {
            get => (SolidColorBrush)GetValue(MainBorderBrushProperty);
            set => SetValue(MainBorderBrushProperty, value);
        }

        public static readonly DependencyProperty MainBorderBrushProperty = DependencyProperty.Register(
            nameof(MainBorderBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(187, 187, 187)), OnMainBorderBrushPropertyChanged)
        );

        private static void OnMainBorderBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListDisplay<T> control) {
                // update property
                control.MainBorder.BorderBrush = (SolidColorBrush)args.NewValue;
            }
        }

        // - Border Background -

        [Category("Brush")]
        [Description("Determines the background brush of the main Border")]
        public SolidColorBrush MainBorderBackground {
            get => (SolidColorBrush)GetValue(MainBorderBackgroundProperty);
            set => SetValue(MainBorderBackgroundProperty, value);
        }

        public static readonly DependencyProperty MainBorderBackgroundProperty = DependencyProperty.Register(
            nameof(MainBorderBackground),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(228, 231, 241)), OnMainBorderBackgroundPropertyChanged)
        );

        private static void OnMainBorderBackgroundPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListDisplay<T> control) {
                // update property
                control.MainBorder.Background = (SolidColorBrush)args.NewValue;
            }
        }

        #endregion

        #region Headers

        // - Header Height -

        [Category("Headers")]
        [Description("Determines how tall headers are")]
        public int HeaderHeight {
            get => (int)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
            nameof(HeaderHeight),
            typeof(int),
            typeof(ListDisplay<T>),
            new PropertyMetadata(28)
        );

        // - Header Font Color -

        [Category("Headers")]
        [Description("Determines the color of font for headers")]
        public SolidColorBrush HeaderFontColor {
            get => (SolidColorBrush)GetValue(HeaderFontColorProperty);
            set => SetValue(HeaderFontColorProperty, value);
        }

        public static readonly DependencyProperty HeaderFontColorProperty = DependencyProperty.Register(
            nameof(HeaderFontColor),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Colors.Black))
        );

        // - Header Border Thickness -

        [Category("Headers")]
        [Description("Determines the border thickness for headers")]
        public Thickness HeaderBorderThickness {
            get => (Thickness)GetValue(HeaderBorderThicknessProperty);
            set => SetValue(HeaderBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty HeaderBorderThicknessProperty = DependencyProperty.Register(
            nameof(HeaderBorderThickness),
            typeof(Thickness),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new Thickness(1))
        );

        // - Header Border Color -

        [Category("Headers")]
        [Description("Determines the border brush for headers")]
        public SolidColorBrush HeaderBorderBrush {
            get => (SolidColorBrush)GetValue(HeaderBorderBrushProperty);
            set => SetValue(HeaderBorderBrushProperty, value);
        }

        public static readonly DependencyProperty HeaderBorderBrushProperty = DependencyProperty.Register(
            nameof(HeaderBorderBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(100, 100, 100)))
        );

        // - Header Fill Color 1 -

        [Category("Headers")]
        [Description("Determines the primary fill brush for headers")]
        public SolidColorBrush HeaderPrimaryFillBrush {
            get => (SolidColorBrush)GetValue(HeaderPrimaryFillBrushProperty);
            set => SetValue(HeaderPrimaryFillBrushProperty, value);
        }

        public static readonly DependencyProperty HeaderPrimaryFillBrushProperty = DependencyProperty.Register(
            nameof(HeaderPrimaryFillBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(198, 204, 221)))
        );

        // - Header Fill Color 2 -

        [Category("Headers")]
        [Description("Determines the secondary fill brush for headers")]
        public SolidColorBrush HeaderSecondaryFillBrush {
            get => (SolidColorBrush)GetValue(HeaderSecondaryFillBrushProperty);
            set => SetValue(HeaderSecondaryFillBrushProperty, value);
        }

        public static readonly DependencyProperty HeaderSecondaryFillBrushProperty = DependencyProperty.Register(
            nameof(HeaderSecondaryFillBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(189, 189, 200)))
        );

        #endregion

        #region Items

        // - Item Height -

        [Category("Items")]
        [Description("Determines how tall items are")]
        public int ItemHeight {
            get => (int)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(int),
            typeof(ListDisplay<T>),
            new PropertyMetadata(22)
        );

        // - Items Border Thickness -

        [Category("Items")]
        [Description("Determines the border thickness for items")]
        public Thickness ItemBorderThickness {
            get => (Thickness)GetValue(ItemBorderThicknessProperty);
            set => SetValue(ItemBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty ItemBorderThicknessProperty = DependencyProperty.Register(
            nameof(ItemBorderThickness),
            typeof(Thickness),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new Thickness(1))
        );

        // - Items Border Color -

        [Category("Items")]
        [Description("Determines the border brush for items")]
        public SolidColorBrush ItemBorderBrush {
            get => (SolidColorBrush)GetValue(ItemBorderBrushProperty);
            set => SetValue(ItemBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ItemBorderBrushProperty = DependencyProperty.Register(
            nameof(ItemBorderBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray))
        );

        // - Items Fill Color 1 -

        [Category("Items")]
        [Description("Determines the primary fill brush for items")]
        public SolidColorBrush ItemPrimaryFillBrush {
            get => (SolidColorBrush)GetValue(ItemPrimaryFillBrushProperty);
            set => SetValue(ItemPrimaryFillBrushProperty, value);
        }

        public static readonly DependencyProperty ItemPrimaryFillBrushProperty = DependencyProperty.Register(
            nameof(ItemPrimaryFillBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(216, 219, 225)))
        );

        // - Items Fill Color 2 -

        [Category("Items")]
        [Description("Determines the secondary fill brush for items")]
        public SolidColorBrush ItemSecondaryFillBrush {
            get => (SolidColorBrush)GetValue(ItemSecondaryFillBrushProperty);
            set => SetValue(ItemSecondaryFillBrushProperty, value);
        }

        public static readonly DependencyProperty ItemSecondaryFillBrushProperty = DependencyProperty.Register(
            nameof(ItemSecondaryFillBrush),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(197, 200, 209)))
        );

        #endregion

        // - Display Layer -

        [Category("Common")]
        [Description("Determines which display layer to use")]
        public int DisplayLayer {
            get => (int)GetValue(DisplayLayerProperty);
            set => SetValue(DisplayLayerProperty, value);
        }

        public static readonly DependencyProperty DisplayLayerProperty = DependencyProperty.Register(
            nameof(DisplayLayer),
            typeof(int),
            typeof(ListDisplay<T>),
            new PropertyMetadata(-1, OnDisplayLayerChanged)
        );

        private static void OnDisplayLayerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListDisplay<T> control) {
                control.BuildGrid(); // rebuilds the grid if the display layer was changed
            }
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        private void Interface_Constructor() {
            // event handling for ensuring content never overlaps the scrollbar
            MainGrid.SizeChanged += OnScrollBarVisibilityChange;
            ListDisplayGrid.ChildrenChanged += OnScrollBarVisibilityChange;

            // sets the original scroll bar visibility
            Loaded += SetOriginalScrollBarVisibilityOnLoaded;

            // set main content parents
            this.Content = MainGrid;

            // alignment settings for the ListDisplayGrid
            ListDisplayGrid.VerticalAlignment = VerticalAlignment.Top;
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        /// <summary>
        /// Attempts to find a ScrollBar from a parent
        /// </summary>
        /// <param name="parent"> The object to search for a ScrollBar </param>
        /// <returns> A ScrollBar object </returns>
        private static ScrollBar? FindVerticalScrollBar(DependencyObject parent) {
            
            // get children amount
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childrenCount == 0) { return null; }
            
            // for every child of the parent
            for (int i = 0; i < childrenCount; i++) {

                // check the child for a vertical scroll bar
                var child = VisualTreeHelper.GetChild(parent, i);
                if ((child is ScrollBar sb) && (sb.Orientation == Orientation.Vertical))
                    return sb;

                // if the scroll bar wasn't directly found, then try recursing to find it
                ScrollBar? scrollBar = FindVerticalScrollBar(child);
                if (scrollBar != null) {
                    return scrollBar;
                }

            }
            return null;
        }

        /// <summary>
        /// Changes the visibility and margin for the scrollbar when the layout is changed
        /// </summary>
        private void OnScrollBarVisibilityChange(object? sender, EventArgs args) {
            // designer check
            if (DesignerProperties.GetIsInDesignMode(this)) { return; }

            // find the content height
            double listContentTotalHeight = (
                ((ListDisplayGrid.RowDefinitions.Count) * ItemHeight)
                + HeaderHeight
                + MainBorderThickness.Top + MainBorderThickness.Bottom
            );

            // not null check
            if (OriginalScrollBarVisbility != null) {

                // check if original was auto
                if (OriginalScrollBarVisbility == ScrollBarVisibility.Auto) {
                    // auto show/hide scrollbar
                    if (listContentTotalHeight > MainGrid.ActualHeight) { // if listContentTotalHeight of the grid is too much
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

        /// <summary>
        /// Builds the associated grid display with the current data
        /// </summary>
        /// <remarks>
        /// Font Size is set to the listContentTotalHeight of it's associated grid
        /// </remarks>
        public void BuildGrid() {
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

            // scroll bar width
            Application.Current.Dispatcher.InvokeAsync(() => {
                ScrollBar? scrollBar = FindVerticalScrollBar(MainScrollViewer);
                if (scrollBar == null) { throw new ArgumentException($"MainScrollViewer didn't contain a ScrollBar"); }
                scrollBar.MinWidth = 10;
                scrollBar.Width = ScrollBarWidth;
            }, DispatcherPriority.Loaded);
            

            // scroll viewer margins
            MainScrollViewer.Margin = MainBorder.BorderThickness;

            // headers row
            ListDisplayGrid.RowDefinitions.Add(new RowDefinition());

            // for every header
            bool usePrimaryHeaderColor = true;
            bool isFirstPass = true;
            for (int x = 0; x < Headers.Count; x++) {
                var header = Headers[x];

                // display layer checking
                if (DisplayLayer != -1) {
                    // check if the display layer being called for exists
                    if (!ClassDataList[0].DisplayLayers
                        .Select(displayLayers => displayLayers.Value)
                        .Any(displayLayers => displayLayers.Contains(DisplayLayer))
                    ) {
                        throw new ArgumentException($"DisplayLayer is associated with no headers: {DisplayLayer}");
                    }

                    // check if display layer doesn't match
                    if (!ClassDataList[0].DisplayLayers[header].Contains(DisplayLayer)) { continue; }
                }

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

                // create textblock
                var headerTextBlock = new TextBlock();
                headerTextBlock.Text = header;
                headerTextBlock.FontSize = HeaderHeight * 0.5;
                headerTextBlock.Foreground = HeaderFontColor;
                headerTextBlock.HorizontalAlignment = ClassDataList[0].ColumnContentAlignments[header].HorizontalAlignment;
                headerTextBlock.VerticalAlignment = ClassDataList[0].ColumnContentAlignments[header].VerticalAlignment;
                headerTextBlock.Margin = new Thickness(
                    HeaderBorderThickness.Left + HeaderMargin,
                    HeaderBorderThickness.Top + HeaderMargin,
                    HeaderBorderThickness.Right + HeaderMargin,
                    HeaderBorderThickness.Bottom + HeaderMargin
                );

                // create a new column with each header
                headerGrid.Children.Add(headerTextBlock);
                ListDisplayGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                    Width = new GridLength(ColumnWidths[header], GridUnitType.Star),
                });

                // add header grid to main grid
                ListDisplayGrid.Children.Add(headerGrid);
                Grid.SetColumn(headerGrid, x);
                Grid.SetRow(headerGrid, 0);

                // for every value associated with this header
                bool usePrimaryItemColor = true;
                for (int y = 0; y < ClassDataList.Count; y++) {
                    T value = ClassDataList[y];

                    // create grid
                    var itemGrid = new Grid();
                    itemGrid.Height = ItemHeight;

                    // create border
                    var itemBorder = new Border();
                    itemBorder.BorderThickness = ItemBorderThickness;
                    itemBorder.BorderBrush = ItemBorderBrush;
                    itemBorder.Background = (usePrimaryItemColor ? ItemPrimaryFillBrush : ItemSecondaryFillBrush);
                    usePrimaryItemColor = !usePrimaryItemColor; // swap
                    itemGrid.Children.Add(itemBorder);

                    // create a new row with each header
                    if (isFirstPass) {
                        ListDisplayGrid.RowDefinitions.Add(new RowDefinition());
                    }

                    // add display object
                    var displayObject = DataListByColumns[header][y].DisplayObject;
                    displayObject.Margin = new Thickness(
                        ItemBorderThickness.Left + ItemMargin,
                        ItemBorderThickness.Top + ItemMargin,
                        ItemBorderThickness.Right + ItemMargin,
                        ItemBorderThickness.Bottom + ItemMargin
                    );
                    displayObject.HorizontalAlignment = ClassDataList[0].ColumnContentAlignments[header].HorizontalAlignment;
                    displayObject.VerticalAlignment = ClassDataList[0].ColumnContentAlignments[header].VerticalAlignment;
                    if (displayObject.Parent != null) { ((Panel)displayObject.Parent).Children.Remove(displayObject); } // detach it from the current parent if it has one for some reason
                    itemGrid.Children.Add(displayObject);

                    // add item to main grid
                    ListDisplayGrid.Children.Add(itemGrid);
                    Grid.SetColumn(itemGrid, x);
                    Grid.SetRow(itemGrid, (y + 1)); // + 1 to avoid headers
                }

                // no longer first pass
                isFirstPass = false;
            }
        }

        #endregion
    }
}
