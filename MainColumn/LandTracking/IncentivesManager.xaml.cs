using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
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
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    /// <summary>
    /// Interaction logic for IncentivesManager.xaml
    /// </summary>
    public partial class IncentivesManager : UserControl, 
        INotifyPropertyChanged, ISwitchManaged {

        // --- VARIABLES ---
        #region VARIABLES

        // - Management -

        public bool TabContentsChanged => IncentivesDisplay.TabContentsChanged;

        public bool RequiresReset { get; set; } = true;

        // - Info Target and Casting Type -

        public IIncentiveInfoHolder InfoTarget { get; set; }

        private Type CastingType { get; set; }

        // - IncentivesList Type -

        public enum IncentivesListTypes {
            None,
            Violation,
            Purchase,
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

        public IncentivesList IncentivesDisplay { get; set; }

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

        public event EventHandler<EventArgs>? IncentivesChanged;

        // - Has Been Loaded -

        private bool HasBeenLoaded { get; set; } = false;
        public event EventHandler<EventArgs>? CompletedLoading;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR
        
        public IncentivesManager() {
            InitializeComponent();

            // load event
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
                        CastingType = typeof(TaxIncentive);
                        break;
                    case IncentivesListTypes.Violation:
                        InfoTarget = IncentiveInfo.Violation.Instance;
                        IncentivesDisplay = new ViolationIncentivesList();
                        IncentivesDisplay.EmptyText = "No violation incentives";
                        CastingType = typeof(ViolationIncentive);
                        break;
                    case IncentivesListTypes.Purchase:
                        InfoTarget = IncentiveInfo.Purchase.Instance;
                        IncentivesDisplay = new PurchaseIncentivesList();
                        IncentivesDisplay.EmptyText = "No purchase incentives";
                        CastingType = typeof(PurchaseIncentive);
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
                IncentivesDisplay.ShowScrollBar = ScrollBarVisibility.Disabled;
                IncentivesDisplay.ItemBorderBrushSides = new SolidColorBrush(ColorResources.ItemBorderBrushSidesLighter);
                IncentivesDisplay.Updated += (sender, args) => {
                    Dispatcher.BeginInvoke(new Action(() => {
                        UpdateTotalIncentiveResult();
                        IncentivesChanged?.Invoke(this, args);
                    }), DispatcherPriority.Background);
                };

                // set incentive option default values
                IncentiveOptions.Add(new() { Name = IncentiveInfo.None });
                foreach (IncentiveInfo incentiveInfo in InfoTarget.Selectable) {
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

                // completed
                CompletedLoading?.Invoke(this, EventArgs.Empty);
            };
        }

        #endregion

        // --- METHODS ---
        #region METHODS 

        public ActiveIncentive[] GetActiveIncentives() 
            => IncentivesDisplay.ClassDataList.Select(
                incentive => incentive.GetActiveIncentive()
            ).ToArray();

        private IncentiveInfo TryGetInfo() {
            if (SelectionComboLabel.SelectedIndex == -1) { return IncentiveInfo.NoneInfo; }
            return InfoTarget.FindByName(
                (string)SelectionComboLabel.Items[SelectionComboLabel.SelectedIndex]
            );
        }

        private void Remove(Incentive incentive) {
            // remove from display
            IncentivesDisplay.Remove(incentive);
            IncentiveOption incentiveOption = FindByName(incentive.Name);
            incentiveOption.IsEnabled = true;

            // hold selected index
            if (IncentiveOptions.IndexOf(incentiveOption) < SelectionComboLabel.SelectedIndex) {
                SelectionComboLabel.SelectedIndex += 1;
            }
        }

        public void Clear() {
            // remove until empty
            // ensures that it's cleared properly
            while (IncentivesDisplay.Count > 0) {
                Remove(IncentivesDisplay[0]);
            }
        }

        private void _add(IncentiveInfo info) {
            // add incentive to displaylist
            object? instance = Activator.CreateInstance(CastingType, info.Name, info.Value);
            if (instance is null) { throw new NullReferenceException("the instance of an Incentive extension was null"); }
            Incentive incentive = (Incentive)instance;
            incentive.RemoveRequested += (_, __) => {
                Remove(incentive);
            };
            dynamic incentiveExtension = instance;
            IncentivesDisplay.Add(incentiveExtension);

            // remove incentive option
            FindByName(info.Name).IsEnabled = false;
            UpdateOptions();
            SelectionComboLabel.SelectedIndex = 0; // none
        }

        public void Add(string name)
            => _add(InfoTarget.FindByName(name));   

        private void Add()
            => _add(TryGetInfo());

        private void AddIncentiveButton_Click(object sender, RoutedEventArgs args)
            => Add();

        private void UpdateTotalIncentiveResult() 
            => TotalIncentiveResult.Result = Property.GetTotalIncentivesValue(
                IncentivesDisplay.ClassDataList
                .Select(incentive => incentive.GetActiveIncentive())
                .ToArray()
            ).ToString();

        public void Reset()
            => Clear();

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

        #endregion
    }
}
