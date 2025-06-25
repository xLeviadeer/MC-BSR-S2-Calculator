using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {
    public partial class CheckBoxDisplay : UserControl {

        // --- VARIABLES ---

        // -- Interface --
        #region Interface

        // - Match Status -

        public enum CheckBoxDisplayMatchStatus {
            Unchecked,
            Checked
        }

        private bool MatchStatusAsBool {
            get => MatchStatus switch {
                CheckBoxDisplayMatchStatus.Checked => true,
                CheckBoxDisplayMatchStatus.Unchecked => false,
                _ => false
            };
        }

        public CheckBoxDisplayMatchStatus MatchStatus {
            get => (CheckBoxDisplayMatchStatus)GetValue(MatchStatusProperty);
            set => SetValue(MatchStatusProperty, value);
        }

        public static readonly DependencyProperty MatchStatusProperty = DependencyProperty.Register(
            nameof(MatchStatus),
            typeof(CheckBoxDisplayMatchStatus),
            typeof(CheckBoxDisplay),
            new PropertyMetadata(CheckBoxDisplayMatchStatus.Checked)
        );

        // - Content -

        public new object Content {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public new static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(CheckBoxDisplay),
            new PropertyMetadata(null)
        );

        #endregion

        // -- Exposures --
        #region Exposures

        // - IsChecked -

        public bool? IsChecked {
            get => CheckBoxLabelObject.IsChecked;
            set => CheckBoxLabelObject.IsChecked = value;
        }

        // - Checked -

        public event RoutedEventHandler? Checked;
        public event RoutedEventHandler? Unchecked;
        public event RoutedEventHandler? Indeterminate;
        public event EventHandler<BoolEventArgs>? CheckChanged;

        #endregion

        // -- Other ---
        #region Other

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public CheckBoxDisplay() {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, RoutedEventArgs args) {
            // checked forwarding
            CheckBoxLabelObject.Checked += (_, args) => Checked?.Invoke(this, args);
            CheckBoxLabelObject.Unchecked += (_, args) => Unchecked?.Invoke(this, args);
            CheckBoxLabelObject.Indeterminate += (_, args) => Indeterminate?.Invoke(this, args);
            CheckBoxLabelObject.CheckChanged += (_, args) => CheckChanged?.Invoke(this, args);

            // set GridParent
            CheckBoxLabelObject.Context = this;

            // set exposure properties
            CheckBoxLabelObject.LabelText = CheckBoxLabelProperties.GetLabelText(this);
            CheckBoxLabelObject.LayoutMode = CheckBoxLabelProperties.GetLayoutMode(this);
            CheckBoxLabelObject.FluidProportionsSplitIndex = CheckBoxLabelProperties.GetFluidProportionsSplitIndex(this);
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        private void CheckBoxLabelObject_CheckChanged(object sender, BoolEventArgs args) {
            if ((args.Value ?? false) == MatchStatusAsBool) {
                ContentBorder.Visibility = Visibility.Visible;
                MainBackground.Visibility = Visibility.Visible;
            } else {
                ContentBorder.Visibility = Visibility.Collapsed;
                MainBackground.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
