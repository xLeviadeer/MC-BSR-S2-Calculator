using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs { 
    /// <summary>
    /// A framework element with an associated label
    /// </summary>
    /// <typeparam name="T"> The type of framework element that this class uses </typeparam>
    public abstract class LabeledInputBase<T> : UserControl 
        where T : FrameworkElement {
        // --- VARIABLES ---
        #region VARIABLES

        // - LabelText -

        [Category("Common")]
        [Description("The text to be placed in the TextBlock label")]
        public string LabelText {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
            nameof(LabelText),
            typeof(string),
            typeof(LabeledInputBase<T>),
            new PropertyMetadata(null)
        );

        // - Fluid Proportions Split Index -

        [Category("Common")]
        [Description("The index to split the (end of the) label and the (beginning of the) text box at")]
        public int FluidProportionsSplitIndex {
            get => (int)GetValue(FluidProportionsSplitIndexProperty);
            set => SetValue(FluidProportionsSplitIndexProperty, value);
        }

        public static readonly DependencyProperty FluidProportionsSplitIndexProperty = DependencyProperty.Register(
            nameof(FluidProportionsSplitIndex),
            typeof(int),
            typeof(LabeledInputBase<T>),
            new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsMeasure, null, CoerceFluidProportionsSplitIndex)
        );

        private static object CoerceFluidProportionsSplitIndex(DependencyObject d, object baseValue) {
            int value = (int)baseValue;
            if (
                (value != -1)
                && (value < 1)
            ) { return 1; }
            return value;
        }

        // - LayoutMode -

        public enum LabeledInputBaseLayoutModes {
            Above,
            Left
        }

        private LabeledInputBaseLayoutModes? OriginalLayoutMode { get; set; }

        [Category("Common")]
        [Description("The text to be placed in the TextBlock label")]
        public LabeledInputBaseLayoutModes LayoutMode {
            get => (LabeledInputBaseLayoutModes)GetValue(LayoutModeProperty);
            set => SetValue(LayoutModeProperty, value);
        }

        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register(
            nameof(LayoutMode),
            typeof(LabeledInputBaseLayoutModes),
            typeof(LabeledInputBase<T>),
            new PropertyMetadata(LabeledInputBaseLayoutModes.Left, OnLayoutModeChanged)
        );

        private static void OnLayoutModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            var control = (LabeledInputBase<T>)sender;
            control.ApplyLayoutMode(); // re-apply visuals on changes
        }

        // - label clipping -

        public bool LabelIsClipped {
            get => (bool)GetValue(LabelIsClippedProperty);
            set => SetValue(LabelIsClippedProperty, value);
        }

        public static readonly DependencyProperty LabelIsClippedProperty = DependencyProperty.Register(
            nameof(LabelIsClipped),
            typeof(bool),
            typeof(LabeledInputBase<T>),
            new PropertyMetadata(false, OnLabelIsClippedChanged)
        );

        public static void OnLabelIsClippedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {            
            // get values
            var control = (LabeledInputBase<T>)sender;
            bool isClipped = (bool)args.NewValue;

            // update layout mode (only if started as left)
            if (control.OriginalLayoutMode != LabeledInputBaseLayoutModes.Above) {
                control.LayoutMode = isClipped ? LabeledInputBaseLayoutModes.Above : LabeledInputBaseLayoutModes.Left;
            }
        }

        // - main grid -
        public Grid MainGrid { get; set; } = new();

        // - text block label -
        public Label TextLabel { get; set; } = new();

        // - some framework element -
        public abstract T Element { get; set; }

        // - has been loaded -
        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public LabeledInputBase() {
            // startup settings
            Loaded += OnLoaded;

            // original layout mode set
            Loaded += (_, __) => {
                // set original if not set
                if (OriginalLayoutMode == null) {
                    OriginalLayoutMode = LayoutMode;
                }
            };

            // clip checking
            Loaded += (_, __) => CheckClipping();
            SizeChanged += (_, __) => CheckClipping();
        }

        private void OnLoaded(object? sender, EventArgs args) {
            // don't run if already loaded
            if (HasBeenLoaded) { return; }
            HasBeenLoaded = true;

            // -- common settings --

            // auto sizing
            Height = double.NaN;
            Width = double.NaN;

            // label settings
            TextLabel.Margin = new Thickness(3, 3, 3, 0);
            TextLabel.Content = LabelText;

            // add main grid
            this.Content = MainGrid;
        }

        protected void ApplyLayoutMode() {
            // clear
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();

            // -- above layout mode --

            if (LayoutMode == LabeledInputBaseLayoutModes.Above) {
                // label font size
                TextLabel.FontSize = 11;

                // main grid divisions
                MainGrid.RowDefinitions.Add(new RowDefinition() {
                    Height = new GridLength(18, GridUnitType.Pixel)
                });
                MainGrid.RowDefinitions.Add(new RowDefinition());

                // label to grid
                MainGrid.Children.Add(TextLabel);
                Grid.SetRow(TextLabel, 0);

                // text box to grid
                MainGrid.Children.Add(Element);
                Grid.SetRow(Element, 1);
            }

            // -- left layout mode --

            if (LayoutMode == LabeledInputBaseLayoutModes.Left) {
                // label positioning and font
                TextLabel.HorizontalAlignment = HorizontalAlignment.Left;
                TextLabel.VerticalAlignment = VerticalAlignment.Center;
                TextLabel.FontSize = 12;

                // add to main grid
                MainGrid.Children.Add(TextLabel);
                MainGrid.Children.Add(Element);

                // - yes parent grid -
                if (
                    (FluidProportionsSplitIndex != -1)
                    && (this.Parent is Grid parent)
                ) { // if parent is a grid
                    // get starting and ending pos
                    int startingPos = Grid.GetColumn(this);
                    int endingPos = startingPos + Grid.GetColumnSpan(this);

                    // if parent has at least 2 columns
                    if ((endingPos - startingPos) < 2) { goto generate_standardly; }

                    // out of bounds check
                    if (FluidProportionsSplitIndex >= parent.ColumnDefinitions.Count) {
                        throw new IndexOutOfRangeException("the FluidProportionsSplitIndex was greater or equal to the amount of columns");
                    }

                    // duplicate the grid structure to main grid
                    for (int i = startingPos; i < endingPos; i++) {
                        ColumnDefinition currColumn = parent.ColumnDefinitions[i];
                        MainGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                            Width = currColumn.Width
                        });
                    }

                    // placing
                    Grid.SetColumn(TextLabel, startingPos);
                    Grid.SetColumnSpan(TextLabel, FluidProportionsSplitIndex);
                    Grid.SetColumn(Element, FluidProportionsSplitIndex);
                    Grid.SetColumnSpan(Element, endingPos);

                    // dont generate standard
                    return;
                }

            // - no parent grid -
            generate_standardly:
                // main grid divisions
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                    Width = new GridLength(3, GridUnitType.Star)
                });

                // placing
                Grid.SetColumn(TextLabel, 0);
                Grid.SetColumn(Element, 1);
            }
        }

        #endregion

        // --- CLIP CHECKING ---

        private void CheckClipping() {
            // ensure layout has completed
            if (
                (!IsLoaded) 
                || (TextLabel.ActualWidth == 0)
                || (OriginalLayoutMode == LabeledInputBaseLayoutModes.Above)
                || (FluidProportionsSplitIndex == -1) // non-fluid proportions can't format change based on clipping
            ) {
                return;
            }

            // check parent
            if (this.Parent is not Grid) { return; }
            Grid parent = (Grid)this.Parent;

            // get columns from the parent so they can be checked even when in above mode
            int startingPos = Grid.GetColumn(this);
            int endingPos = FluidProportionsSplitIndex;

            // get the width for the columns that contain the textlabel
            double availableWidth = 0;
            for (int i = startingPos; i < endingPos; i++) {
                availableWidth += parent.ColumnDefinitions[i].ActualWidth;
            }
            availableWidth -= TextLabel.Margin.Left + TextLabel.Margin.Right;

            // check if the label is too small
            bool clipped = (availableWidth < GetLabelDesiredWidth(TextLabel));
            LabelIsClipped = clipped;
        }

        private static double GetLabelDesiredWidth(Label label) {
            // do nothing is not valid
            if (
                (label.ActualWidth == 0)
                || (label.Content == null)
                || (
                    (label.Content is string)
                    && (string.IsNullOrWhiteSpace((string)label.Content))
                )
            ) {
                return 0;
            }

            // get text
            var text = label.Content.ToString();

            // create typeface
            var typeface = new Typeface(
                label.FontFamily,
                label.FontStyle,
                label.FontWeight,
                label.FontStretch);

            // create formatted text
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                label.FlowDirection,
                typeface,
                12, // uses the font size for side
                Brushes.Black,
                new NumberSubstitution(),
                1.0
            );

            // return width
            return formattedText.Width;
        }
    }
}
