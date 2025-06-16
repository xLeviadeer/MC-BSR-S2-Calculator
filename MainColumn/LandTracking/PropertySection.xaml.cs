using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using MC_BSR_S2_Calculator.Utility.Validations;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using static System.Collections.Specialized.BitVector32;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking
{
    public partial class PropertySection : UserControl, INotifyPropertyChanged, IValidityHolder
    {
        // --- VARIABLES ---

        // -- Interface --
        #region Interface

        // property changes
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // - Show Delete -

        [Category("Common")]
        [Description("Whether or not to show the delete button")]
        public bool ShowDeleteButton {
            get => (bool)GetValue(ShowDeleteButtonProperty);
            set => SetValue(ShowDeleteButtonProperty, value);
        }

        public static readonly DependencyProperty ShowDeleteButtonProperty = DependencyProperty.Register(
            nameof(ShowDeleteButton),
            typeof(bool),
            typeof(PropertySection),
            new PropertyMetadata(true, OnShowDeleteButtonChanged)
        );

        private static void OnShowDeleteButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is PropertySection control) {
                control.OnPropertyChanged(nameof(ShowDeleteButtonVisibility));
            }
        }

        public Visibility ShowDeleteButtonVisibility => XamlConverter.BoolToVisibility(ShowDeleteButton);

        #endregion

        // -- Other --
        #region Other

        // - Delete Requested Event -

        public event EventHandler<EventArgs>? DeletionRequested;

        // - Up Requested Event -

        public event EventHandler<EventArgs>? MoveUpRequested;

        // - Down Requested Event -

        public event EventHandler<EventArgs>? MoveDownRequested;

        // - Metric Changed Event -

        public event EventHandler<EventArgs>? MetricChanged;

        // - Coordinates Changed Event -

        public event EventHandler<EventArgs>? CoordinatesChanged;

        // - Validity -

        private ValidityHolder Validity { get; set; } = new(
            new() {
                [nameof(SectionName)] = new() { IsValid = true }, // starts empty as true
                [nameof(CoordinateInputCornerA)] = new(),
                [nameof(CoordinateInputCornerB)] = new()
            }
        );

        public bool CheckValidity() => (Validity.CheckValidity() && IsValidByConstraints);

        private bool IsValid { get; set; }

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - Metric -

        public int Metric { get => MetricX * MetricZ; }

        public int MetricX { get; private set; } = 1;

        public int MetricZ { get; private set; } = 1;

        // - Has Valid Connection -

        private bool _isValidByConstraints { get; set; } = false;

        public bool IsValidByConstraints {
            get {
                // return
                return _isValidByConstraints;
            }
            set {
                // change color
                if (value == false) {
                    MainBorder.BorderBrush = new SolidColorBrush(ColorResources.LightRedColor);
                } else {
                    MainBorder.BorderBrush = new SolidColorBrush(ColorResources.InnerBorderVeryLightColor);
                }

                // set
                _isValidByConstraints = value;
            }
        }

        public bool IsInvalidByIntersection { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public PropertySection()
        {
            InitializeComponent();

            // initial validity
            Loaded += (_, __) => {
                IsValid = CheckValidity();
            };

            // metric changed as result changed exposure/rename
            MetricResult.ResultChanged += (_, __) => MetricChanged?.Invoke(this, EventArgs.Empty);

            // coordinates changed forwarding
            void InvokeCoordinatesChanged() => CoordinatesChanged?.Invoke(this, EventArgs.Empty);
            CoordinateInputCornerA.CoordinatesChanged += (_, __) => InvokeCoordinatesChanged();
            CoordinateInputCornerB.CoordinatesChanged += (_, __) => InvokeCoordinatesChanged();
        }

        #endregion

        // --- METHODS ---

        // -- Other --
        #region Other

        // - update title from name -

        private void SectionName_TextChanged(object sender, EventArgs args) {
            TitleText.Text = XamlConverter.CapitalizeWords(SectionName.Text);
        }

        // - calculate new metric -

        public void CalculateAndDisplayMetric() {
            // find the x and y size of the square
            MetricX = (int)(Math.Abs( // 1
                ((IntegerTextBox)CoordinateInputCornerA.XInput.Element).Value
                - ((IntegerTextBox)CoordinateInputCornerB.XInput.Element).Value
            ) + 1);
            MetricZ = (int)(Math.Abs( // 100
                ((IntegerTextBox)CoordinateInputCornerA.ZInput.Element).Value
                - ((IntegerTextBox)CoordinateInputCornerB.ZInput.Element).Value
            ) + 1);

            // set metric
            MetricResult.Result = Metric.ToString();
        }

        private void CalculateAndDisplayMetric(object sender, EventArgs e) {
            CalculateAndDisplayMetric();
        }

        private void TriggerIfValidityChanged() {
            bool isValid = CheckValidity();
            if (IsValid != isValid) {
                IsValid = isValid;
                ValidityChanged?.Invoke(this, new(isValid));
            }
        }

        #endregion

        // -- Per-Item Methods
        #region Per-Item Methods

        private void SectionName_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(SectionName)].IsValid = args.Value ?? true; // true if empty
            TriggerIfValidityChanged();
        }

        private void CoordinateInputCornerA_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(CoordinateInputCornerA)].IsValid = args.Value ?? false;
            TriggerIfValidityChanged();
        }

        private void CoordinateInputCornerB_ValidityChanged(object sender, BoolEventArgs args) {
            Validity[nameof(CoordinateInputCornerB)].IsValid = args.Value ?? false;
            TriggerIfValidityChanged();
        }

        private void DeleteButton_ChargeCycled(object sender, EventArgs args) {
            // request deletion when delete charged
            DeletionRequested?.Invoke(this, args);
        }

        private void DownButton_Click(object sender, RoutedEventArgs args) {
            MoveDownRequested?.Invoke(this, args);
        }

        private void UpButton_Click(object sender, RoutedEventArgs args) {
            MoveUpRequested?.Invoke(this, args);
        }

        #endregion

    }
}
