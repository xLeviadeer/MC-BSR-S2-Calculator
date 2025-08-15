using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.Utility
{
    public partial class CoordinateInput : UserControl, IValidityHolder, INotifyPropertyChanged {
        // --- VARIABLES ---

        // -- Interface --
        #region Interface

        // property changes
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // - Display Mode -

        public enum DisplayModes {
            Fancy,
            Simple
        }

        [Category("Common")]
        [Description("How to display the coordinate input")]
        public DisplayModes DisplayMode {
            get => (DisplayModes)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
            nameof(DisplayMode),
            typeof(DisplayModes),
            typeof(CoordinateInput),
            new PropertyMetadata(DisplayModes.Fancy, OnDisplayModeChanged)
        );

        private static void OnDisplayModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is CoordinateInput control) {
                switch (control.DisplayMode) {
                    case DisplayModes.Simple: {
                        FrameworkElement? coordinateGrid = (FrameworkElement)control.CoordinateInputBorder.Child;
                        if (coordinateGrid.Name == "InputsGrid") {
                            // child handling
                            control.CoordinateInputBorder.Child = null;
                            control.MainGrid.Children.Add(coordinateGrid);
                            Grid.SetRow(coordinateGrid, 1);

                            // other visibility changes
                            control.CoordinateInputBorder.Visibility = Visibility.Collapsed;
                            control.TitleLabel.Visibility = Visibility.Collapsed;

                            control.OnPropertyChanged(nameof(DisplayMode));
                        }
                        break;
                    } case DisplayModes.Fancy: {
                        // find coordinate grid
                        FrameworkElement coordinateGrid;
                        foreach (UIElement gridChild in control.MainGrid.Children) {
                            if (
                                (gridChild is FrameworkElement frameworkElement)
                                && (frameworkElement.Name == "InputsGrid")
                            ) {
                                coordinateGrid = frameworkElement;
                                goto found_coordinateGrid;
                            }
                        }
                        break;

                        found_coordinateGrid:
                            // child handling
                            control.MainGrid.Children.Remove(coordinateGrid);
                            control.CoordinateInputBorder.Child = coordinateGrid;

                            // other visibility changes
                            control.CoordinateInputBorder.Visibility = Visibility.Visible;
                            control.TitleLabel.Visibility = Visibility.Visible;

                            control.OnPropertyChanged(nameof(DisplayMode));
                            break;
                    }
                }
            }
        }

        // - Show X -

        [Category("Common")]
        [Description("Whether or not to show the X input box")]
        public bool ShowXInput {
            get => (bool)GetValue(ShowXInputProperty);
            set => SetValue(ShowXInputProperty, value);
        }

        public static readonly DependencyProperty ShowXInputProperty = DependencyProperty.Register(
            nameof(ShowXInput),
            typeof(bool),
            typeof(CoordinateInput),
            new PropertyMetadata(true, OnShowXInputChanged)
        );

        private static void OnShowXInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is CoordinateInput control) {
                control.OnPropertyChanged(nameof(ShowXInputVisibility));
                control.Validity[nameof(XInput)].IsEnabled = control.ShowXInput;
            }
        }

        public Visibility ShowXInputVisibility => XamlConverter.BoolToVisibility(ShowXInput);

        // - Show Y -

        [Category("Common")]
        [Description("Whether or not to show the Y input box")]
        public bool ShowYInput {
            get => (bool)GetValue(ShowYInputProperty);
            set => SetValue(ShowYInputProperty, value);
        }

        public static readonly DependencyProperty ShowYInputProperty = DependencyProperty.Register(
            nameof(ShowYInput),
            typeof(bool),
            typeof(CoordinateInput),
            new PropertyMetadata(true, OnShowYInputChanged)
        );

        private static void OnShowYInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is CoordinateInput control) {
                control.OnPropertyChanged(nameof(ShowYInputVisibility));
                control.Validity[nameof(YInput)].IsEnabled = control.ShowYInput;
            }
        }

        public Visibility ShowYInputVisibility => XamlConverter.BoolToVisibility(ShowYInput);

        // - Show Z -

        [Category("Common")]
        [Description("Whether or not to show the Z input box")]
        public bool ShowZInput {
            get => (bool)GetValue(ShowZInputProperty);
            set => SetValue(ShowZInputProperty, value);
        }

        public static readonly DependencyProperty ShowZInputProperty = DependencyProperty.Register(
            nameof(ShowZInput),
            typeof(bool),
            typeof(CoordinateInput),
            new PropertyMetadata(true, OnShowZInputChanged)
        );

        private static void OnShowZInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is CoordinateInput control) {
                control.OnPropertyChanged(nameof(ShowZInputVisibility));
                control.Validity[nameof(ZInput)].IsEnabled = control.ShowZInput;
            }
        }

        public Visibility ShowZInputVisibility => XamlConverter.BoolToVisibility(ShowZInput);

        // - Name Extension -

        [Category("Common")]
        [Description("The name (extending \"Coordinates \") displayed with this coordinate box")]
        public string NameExtension {
            get => (string)GetValue(NameExtensionProperty);
            set => SetValue(NameExtensionProperty, value);
        }

        public static readonly DependencyProperty NameExtensionProperty = DependencyProperty.Register(
            nameof(NameExtension),
            typeof(string),
            typeof(CoordinateInput),
            new PropertyMetadata(string.Empty, OnDisplayNameChanged)
        );

        private static void OnDisplayNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is CoordinateInput control) {
                control.OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName => $"Coordinates {NameExtension}";

        #endregion

        // -- Other --
        #region Other

        // - Validity Holder -

        private ValidityHolder Validity { get; set; } = new(
            new() {
                [nameof(XInput)] = new(),
                [nameof(YInput)] = new(),
                [nameof(ZInput)] = new()
            }
        );

        // - Validity Changed Event -

        private bool IsValid { get; set; }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - Coordinates Changed Event -

        public event EventHandler<EventArgs>? CoordinatesChanged;

        // - Expose Coordinate Values -

        public int XCoordinate {
            get => (int)XInput.TryGetValue<double>();
        }

        public int YCoordinate {
            get => (int)YInput.TryGetValue<double>();
        }

        public int ZCoordinate {
            get => (int)ZInput.TryGetValue<double>();
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public CoordinateInput()
        {
            InitializeComponent();

            // initial validity
            Loaded += (_, __) => {
                IsValid = CheckValidity();
            };

            Loaded += (_, __) => {
                // coordinates changed
                void InvokeCoordinatesChanged(object? sender, EventArgs args) {
                    if (args is InputFinalizedEventArgs<double> inputFinalizedArgs) {
                        if (inputFinalizedArgs.OldValue != inputFinalizedArgs.NewValue) {
                            CoordinatesChanged?.Invoke(this, args);
                        }
                    } else { // fallback
                        CoordinatesChanged?.Invoke(this, args);
                    }
                }
                XInput.InputFinalized += InvokeCoordinatesChanged;
                YInput.InputFinalized += InvokeCoordinatesChanged;
                ZInput.InputFinalized += InvokeCoordinatesChanged;
            };
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        // - Get Coordinates -

        public CoordinateStructure GetCoordinatesAs<CoordinateStructure>()
            where CoordinateStructure : IModifyableFlatCoordinate, new() {
            // return coord
            var coord = new CoordinateStructure();
            coord.X = XCoordinate;
            coord.Z = ZCoordinate;

            // based on type structure
            switch (typeof(CoordinateStructure)) {
                case IModifyableCoordinate:
                    ((IModifyableCoordinate)coord).Y = YCoordinate;
                    return coord;
                case IModifyableFlatCoordinate:
                    return coord;
                default:
                    throw new ArgumentException("GetCoordinatesAs's CoordinateStructure type was not a valid type");
            }
        }

        // - Expose IsValid -

        public bool CheckValidity() => Validity.CheckValidity();

        // - Per Coord Validity Changes -

        private void TriggerIfValidityChanged() {
            bool isValid = CheckValidity();
            if (IsValid != isValid) {
                IsValid = isValid;
                ValidityChanged?.Invoke(this, new(isValid));
            }
        }

        private void XInput_ValidityChanged(object? sender, BoolEventArgs args) {
            Validity[nameof(XInput)].IsValid = args.Value ?? false;
            TriggerIfValidityChanged();
        }

        private void YInput_ValidityChanged(object? sender, BoolEventArgs args) {
            Validity[nameof(YInput)].IsValid = args.Value ?? false;
            TriggerIfValidityChanged();
        }

        private void ZInput_ValidityChanged(object? sender, BoolEventArgs args) {
            Validity[nameof(ZInput)].IsValid = args.Value ?? false;
            TriggerIfValidityChanged();
        }

        #endregion
    }
}
