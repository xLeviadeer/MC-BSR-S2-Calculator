using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
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

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    /// <summary>
    /// Interaction logic for IncentivesManager.xaml
    /// </summary>
    public partial class IncentivesManager : UserControl, INotifyPropertyChanged {

        // --- VARIABLES ---
        #region VARIABLES

        // - Info Target -

        public IInfoHolder InfoTarget { get; set; }

        // - IncentivesList Type -

        public enum IncentivesListTypes {
            None,
            Tax
        }

        public IncentivesListTypes IncentiveListType {
            get => (IncentivesListTypes)GetValue(IncentivesListTypesProperty);
            set => SetValue(IncentivesListTypesProperty, value);
        }

        public static readonly DependencyProperty IncentivesListTypesProperty = DependencyProperty.Register(
            nameof(IncentiveListType),
            typeof(IncentivesListTypes),
            typeof(IncentivesManager),
            new PropertyMetadata(IncentivesListTypes.None)
        );

        // - Incentive List -

        public IncentiveList IncentivesDisplay { get; set; }

        // - property changes -

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // - Incentives - 

        public class IncentiveOption : INotifyPropertyChanged {
            // - property changes -

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            // - name -

            public required string Name { get; set; } = string.Empty;

            // - is enabled -

            private bool _isEnabled { get; set; } = true;
            public bool IsEnabled {
                get => _isEnabled;
                set {
                    if (_isEnabled != value) {
                        _isEnabled = value;
                        OnPropertyChanged(nameof(IsEnabled));
                    }
                }
            }
        }

        public List<IncentiveOption> IncentiveOptions { get; private init; } = [];

        public void UpdateOptions() {
            SelectionComboLabel.ItemsSource = IncentiveOptions
                .Where(incentive => incentive.IsEnabled)
                .Select(incentive => incentive.Name)
                .ToImmutableList();
        }

        private IncentiveOption FindByName(string name) {
            foreach (IncentiveOption option in IncentiveOptions) {
                if (option.Name == name) {
                    return option;
                }
            }
            return IncentiveOptions[0]; // none
        }

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public IncentivesManager() {
            InitializeComponent();

            Loaded += (_, __) => {
                // has been loaded check
                if (HasBeenLoaded) { return; }
                HasBeenLoaded = true;

                // attempt to set based on incentives type
                switch (IncentiveListType) {
                    case IncentivesListTypes.Tax:
                        InfoTarget = IncentiveInfo.Tax.Instance;
                        IncentivesDisplay = new TaxIncentivesList();
                        IncentivesDisplay.EmptyText = "No tax incentives";
                        break;
                    case IncentivesListTypes.None:
                    default:
                        throw new InvalidEnumArgumentException($"Incentive List Type must not be None to use this class");
                }

                // add default list properties
                MainGrid.Children.Add(IncentivesDisplay);
                Grid.SetRow(IncentivesDisplay, 0);
                Grid.SetColumn(IncentivesDisplay, 0);
                Grid.SetColumnSpan(IncentivesDisplay, 3);
                IncentivesDisplay.Margin = new Thickness(3);
                IncentivesDisplay.ShowScrollBar = ScrollBarVisibility.Hidden;
                IncentivesDisplay.ItemBorderBrushSides = new SolidColorBrush(Color.FromRgb(179, 179, 176));
                IncentivesDisplay.Updated += (_, __) => UpdateTotalIncentiveResult();

                // set incentive option default values
                IncentiveOptions.Add(new() { Name = IncentiveInfo.None });
                foreach (IncentiveInfo incentiveInfo in InfoTarget.SelectableIncentives) {
                    var incentiveOption = new IncentiveOption() { Name = incentiveInfo.Name };
                    incentiveOption.PropertyChanged += (_, __) => {
                        UpdateOptions();
                    };
                    IncentiveOptions.Add(incentiveOption);
                }
                UpdateOptions();
                OnPropertyChanged(nameof(IncentiveOptions));

                // set selected index to none
                SelectionComboLabel.SelectedIndex = 0;
            };
        }

        #endregion

        // --- METHODS ---
        #region METHODS 

        private IncentiveInfo TryGetInfo() {
            if (SelectionComboLabel.SelectedIndex == -1) { return IncentiveInfo.NoneInfo; }
            return InfoTarget.FindByName(
                (string)SelectionComboLabel.Items[SelectionComboLabel.SelectedIndex]
            );
        }

        private void Remove(Incentive incentive) {
            IncentivesDisplay.Remove(incentive);
            IncentiveOption incentiveOption = FindByName(incentive.Name);
            incentiveOption.IsEnabled = true;
            if (IncentiveOptions.IndexOf(incentiveOption) < SelectionComboLabel.SelectedIndex) {
                SelectionComboLabel.SelectedIndex += 1;
            }
        }

        public void Reset() {
            // remove until empty
            while (IncentivesDisplay.ClassDataList.Count > 0) {
                Remove(IncentivesDisplay.ClassDataList[0]);
            }
        }

        private void AddIncentiveButton_Click(object sender, RoutedEventArgs e) {
            // find incentive info
            IncentiveInfo info = TryGetInfo();

            // add incentive to displaylist
            TaxIncentive incentive = new(info.Name, info.Value);
            incentive.RemoveRequested += (_, __) => {
                Remove(incentive);
            };
            IncentivesDisplay.Add(incentive);

            // remove incentive option
            FindByName(info.Name).IsEnabled = false;
            UpdateOptions();
            SelectionComboLabel.SelectedIndex = 0; // none
        }

        private void SelectionComboLabel_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // get selection info
            IncentiveInfo info = TryGetInfo();

            // if none option
            if (info.Name == IncentiveInfo.None) {
                DescriptionBody.Text = "No description";
                DescriptionBorder.Visibility = Visibility.Collapsed;
                AddIncentiveButton.IsEnabled = false;

            // not none, update description
            } else {
                DescriptionBody.Text = info.Description;
                DescriptionBorder.Visibility = Visibility.Visible;
                AddIncentiveButton.IsEnabled = true;
            }
        }

        private void UpdateTotalIncentiveResult() {
            double total = 0;
            foreach (Incentive incentive in IncentivesDisplay.ClassDataList) {
                total += incentive.Value;
            }
            TotalIncentiveResult.Result = Math.Round(total, 2).ToString();
        }

        #endregion
    }
}
