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
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.Utility.ConfirmationWindows
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        private const double TextSizeFactor = 0.5;

        // --- CONSTRUCTORS ---

        private void CenterText() {
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

        public ConfirmationWindow()
        {
            InitializeComponent();
            CenterText();
        }

        public ConfirmationWindow(
            string titleText="Are you sure?", 
            string confirmButtonText="Yes", 
            string denyButtontext="No",
            bool useConfirmColor=false
        ) {
            InitializeComponent();
            TitleText.Text = titleText;
            ConfirmButton.Content = confirmButtonText;
            DenyButton.Content = denyButtontext;
            if (useConfirmColor) {
                ConfirmButton.BorderBrush = new SolidColorBrush(Color.FromRgb(108, 49, 49));
                ConfirmButton.Foreground = new SolidColorBrush(Color.FromRgb(164, 33, 33));
            }
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
