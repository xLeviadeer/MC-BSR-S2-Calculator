using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.Utility.ConfirmationWindows
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window {

        // --- VARIABLES ---

        private const double TextSizeFactor = 0.5;
        private const int MaxTitleTextLength = 150;
        private const int MaxDescriptionTextLength = 1000;

        // - screen heights and widths -

        public static double ThirdScreenWidth { 
            get {
                var source = PresentationSource.FromVisual(Application.Current.MainWindow);

                if (source?.CompositionTarget != null) {
                    Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
                    double dpiScaleX = transformToDevice.M11;

                    // actual pixel width
                    return (SystemParameters.PrimaryScreenWidth * dpiScaleX) / 3;
                }
                return 500;
            }
        }

        public static double ScreenHeight {
            get {
                var source = PresentationSource.FromVisual(Application.Current.MainWindow);

                if (source?.CompositionTarget != null) {
                    Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
                    double dpiScaleY = transformToDevice.M22;

                    // actual pixel width
                    return SystemParameters.PrimaryScreenHeight * dpiScaleY;
                }
                return 500;
            }
        }

        // - button -

        public ButtonBase ConfirmButton { get; set; }

        // --- CONSTRUCTORS ---

        private void CenterText() {
            // set data context
            DataContext = this;

            // set text size
            TitleText.FontSize = TitleText.Height * TextSizeFactor;

            // get factor sizes
            var textSizeFactorInverse = (1 - TextSizeFactor);
            var changeAmountForCentering = TitleText.Height * (textSizeFactorInverse / 2);

            // adjust margin
            TitleText.Margin = new Thickness(
                TitleText.Margin.Left,
                TitleText.Margin.Top + changeAmountForCentering,
                TitleText.Margin.Right,
                TitleText.Margin.Bottom - changeAmountForCentering
            );

            // adjust text height
            TitleText.Height -= textSizeFactorInverse;
        }

        public ConfirmationWindow() {
            InitializeComponent();
            CenterText();
            ConfirmButton = new Button();
            ConfirmButton.Content = "Test Yes";
        }

        public ConfirmationWindow(
            string titleText="Are you sure?",
            string confirmButtonText="Yes", 
            string denyButtontext="No",
            string descriptionText="",
            string useConfirmColor="",
            bool useChargingButton=false,
            double? chargeTime=null
        ) {
            InitializeComponent();
            
            // title/description
            TitleText.Text = (titleText.Length > MaxTitleTextLength) ? "Invalid Title" : titleText;
            DescriptionText.Text = (descriptionText.Length > MaxDescriptionTextLength) ? "Invalid Description" : descriptionText;
            
            // confirm button type
            if (useChargingButton) {
                ConfirmButton = new ChargingButton();
            } else {
                ConfirmButton = new Button();
            }

            // confirm button default setup
            ConfirmButton.Style = (Style)Application.Current.Resources["ConfirmButtons"];
            ConfirmButton.Content = confirmButtonText;
            ConfirmButton.Margin = new Thickness(5, 0, 3, 5);

            // confirm button color and dependent settings
            switch (useConfirmColor.ToLower()) {
                case "red":
                    // set colors
                    ConfirmButton.BorderBrush = new SolidColorBrush(ColorResources.DarkerRedColor);
                    ConfirmButton.Foreground = new SolidColorBrush(ColorResources.MediumerRedColor);

                    // set charging settings
                    if (ConfirmButton is ChargingButton confirmChargingButton) {
                        confirmChargingButton.ColorPalette = ChargingButton.ChargingButtonColorPalettes.Red;
                        confirmChargingButton.ApplyPalette();
                        
                        // charge time
                        if (chargeTime != null) {
                            confirmChargingButton.ChargeTime = (double)chargeTime;
                        }
                        
                        // event
                        confirmChargingButton.ChargeCycled += (sender, args) => OnConfirm(this, new());
                    } else {
                        // event
                        ConfirmButton.Click += OnConfirm;
                    }
                    break;
            }

            // add confirm button to grid
            OuterGrid.Children.Add(ConfirmButton);
            Grid.SetColumn(ConfirmButton, 2);
            Grid.SetRow(ConfirmButton, 2);

            // deny button
            DenyButton.Content = denyButtontext;

            // default
            CenterText();
        }

        // --- METHODS ---

        // - key pressed in window -

        private void OnKeyDown(object sender, KeyEventArgs args) {
            // close if pressed escape
            if (args.Key == Key.Escape) {
                this.Close();
            }
        }

        // - confirm pressed -
        
        protected void OnConfirm(object sender, RoutedEventArgs args) {
            DialogResult = true;
            this.Close();
        }

        // - deny pressed -

        protected void OnDeny(object sender, RoutedEventArgs args) {
            DialogResult = false;
            this.Close();
        }
    }
}
