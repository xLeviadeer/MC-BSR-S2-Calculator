using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
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

namespace MC_BSR_S2_Calculator.Utility.DisplayList {
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

        // - Items Left/Right Border Color -

        [Category("Items")]
        [Description("Determines the left/right border brush for items")]
        public SolidColorBrush ItemBorderBrushSides {
            get => (SolidColorBrush)GetValue(ItemBorderBrushSidesProperty);
            set => SetValue(ItemBorderBrushSidesProperty, value);
        }

        public static readonly DependencyProperty ItemBorderBrushSidesProperty = DependencyProperty.Register(
            nameof(ItemBorderBrushSides),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Colors.DimGray))
        );

        // Items Border Top/Bottom Color -

        [Category("Items")]
        [Description("Determines the left/right border brush for items")]
        public SolidColorBrush ItemBorderBrushEnds {
            get => (SolidColorBrush)GetValue(ItemBorderBrushEndsProperty);
            set => SetValue(ItemBorderBrushEndsProperty, value);
        }

        public static readonly DependencyProperty ItemBorderBrushEndsProperty = DependencyProperty.Register(
            nameof(ItemBorderBrushEnds),
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

        #region Clickable

        // - Clickable Status -

        public enum ListDisplayClickable {
            NotClickable,
            ByCell,
            ByRow
        }

        private ListDisplayClickable? _isClickable;

        public ListDisplayClickable IsClickable {
            get {
                if (_isClickable == null) {
                    // holder variables
                    bool anyRowHasClickMethod = false;
                    bool anyCelleHasClickMethod = false;

                    // check all classes for an event
                    foreach (T cls in ClassDataList) {
                        if (cls.IsHoldingEvent) {
                            anyRowHasClickMethod = true;
                        }

                        // check all display values of a class for an event
                        foreach (string header in Headers) {
                            if (cls.DisplayValues[header].IsHoldingEvent) {
                                anyCelleHasClickMethod = true;
                            }
                        }
                    }

                    // if both contain methods
                    if (anyRowHasClickMethod && anyCelleHasClickMethod) { // both
                        throw new ArgumentException($"both rows and cells contained methods for a displayable object");
                    } else if (anyRowHasClickMethod ^ anyCelleHasClickMethod) { // only one (xor)
                        if (anyRowHasClickMethod) { // by row
                            _isClickable = ListDisplayClickable.ByRow;
                        } else { // by cell
                            _isClickable = ListDisplayClickable.ByCell;
                        }
                    } else { // neither
                        _isClickable = ListDisplayClickable.NotClickable;
                    }
                }
                return (ListDisplayClickable)_isClickable;
            }
        }

        // - Hover Color -

        [Category("Items")]
        [Description("Determines the hover color of items if they are clickable")]
        public SolidColorBrush ItemsHoverColor {
            get => (SolidColorBrush)GetValue(ItemsHoverColorProperty);
            set => SetValue(ItemsHoverColorProperty, value);
        }

        public static readonly DependencyProperty ItemsHoverColorProperty = DependencyProperty.Register(
            nameof(ItemsHoverColor),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(77, 80, 180, 255)), OnMainBorderBrushPropertyChanged)
        );

        // - Click Color -

        [Category("Items")]
        [Description("Determines the click color of items if they are clickable")]
        public SolidColorBrush ItemsClickColor {
            get => (SolidColorBrush)GetValue(ItemsClickColorProperty);
            set => SetValue(ItemsClickColorProperty, value);
        }

        public static readonly DependencyProperty ItemsClickColorProperty = DependencyProperty.Register(
            nameof(ItemsClickColor),
            typeof(SolidColorBrush),
            typeof(ListDisplay<T>),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(77, 140, 200, 255)), OnMainBorderBrushPropertyChanged)
        );

        #endregion

        #region General

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

        // - Empty Text - 

        [Category("Common")]
        [Description("Determines what text to display when the grid is empty")]
        public string EmptyText {
            get => (string)GetValue(EmptyTextProperty);
            set => SetValue(EmptyTextProperty, value);
        }

        public static readonly DependencyProperty EmptyTextProperty = DependencyProperty.Register(
            nameof(EmptyText),
            typeof(string),
            typeof(ListDisplay<T>),
            new PropertyMetadata("No columns found")
        );

        // - Empty Header Text -

        [Category("Headers")]
        [Description("Determines what text to display when a header is empty")]
        public string EmptyHeaderText {
            get => (string)GetValue(EmptyHeaderTextProperty);
            set => SetValue(EmptyHeaderTextProperty, value);
        }

        public static readonly DependencyProperty EmptyHeaderTextProperty = DependencyProperty.Register(
            nameof(EmptyHeaderText),
            typeof(string),
            typeof(ListDisplay<T>),
            new PropertyMetadata("No items found")
        );

        // - Max Height -

        [Category("Layout")]
        [Description("Determines the maximum height the main grid can have")]
        public int MaxDisplayHeight {
            get => (int)GetValue(MaxDisplayHeightProperty);
            set => SetValue(MaxDisplayHeightProperty, value);
        }

        public static readonly DependencyProperty MaxDisplayHeightProperty = DependencyProperty.Register(
            nameof(MaxDisplayHeight),
            typeof(int),
            typeof(ListDisplay<T>),
            new PropertyMetadata(-1)
        );

        #endregion

    }
}
