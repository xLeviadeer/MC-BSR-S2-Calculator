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
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {
    public class ResultDisplay : LabeledInputBase<Border> {

        // --- VARIABLES ---

        // -- Interface --
        #region Interface

        // - Result -

        [Category("Common")]
        [Description("The result")]
        public string Result {
            get => (string)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
            nameof(Result),
            typeof(string),
            typeof(ResultDisplay),
            new PropertyMetadata(string.Empty, OnResultChanged)
        );

        private static void OnResultChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ResultDisplay control) {
                // only invoke if result is different
                if (!string.Equals(
                    (args.OldValue as string),
                    (args.NewValue as string)
                )) {
                    control.ResultChanged?.Invoke(control, EventArgs.Empty);
                }
            }
        }

        // - Default Result -

        [Category("Common")]
        [Description("The default result to fall back on")]
        public string DefaultResult {
            get => (string)GetValue(DefaultResultProperty);
            set => SetValue(DefaultResultProperty, value);
        }

        public static readonly DependencyProperty DefaultResultProperty = DependencyProperty.Register(
            nameof(DefaultResult),
            typeof(string),
            typeof(ResultDisplay),
            new PropertyMetadata(string.Empty)
        );

        // - Foreground -

        [Category("Brush")]
        [Description("The brush of the element border")]
        public SolidColorBrush ResultForeground {
            get => (SolidColorBrush)GetValue(ResultForegroundProperty);
            set => SetValue(ResultForegroundProperty, value);
        }

        public static readonly DependencyProperty ResultForegroundProperty = DependencyProperty.Register(
            nameof(ResultForeground),
            typeof(SolidColorBrush),
            typeof(ResultDisplay),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), OnResultForegroundChanged)
        );

        private static void OnResultForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (
                (sender is ResultDisplay control)
                && (control.Element.Child is TextBlock textBlock)
            ) {
                textBlock.Foreground = args.NewValue as SolidColorBrush;
            }
        }

        // - Border Brush -

        [Category("Brush")]
        [Description("The brush of the element border")]
        public SolidColorBrush ResultBorderBrush {
            get => (SolidColorBrush)GetValue(ResultBorderBrushProperty);
            set => SetValue(ResultBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ResultBorderBrushProperty = DependencyProperty.Register(
            nameof(ResultBorderBrush),
            typeof(SolidColorBrush),
            typeof(ResultDisplay),
            new PropertyMetadata(new SolidColorBrush(ColorResources.InnerBorderVeryLightColor), OnResultBorderBrushChanged)
        );

        private static void OnResultBorderBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ResultDisplay control) {
                control.Element.BorderBrush = args.NewValue as SolidColorBrush;
            }
        }

        #endregion

        // - Result Changed Event -

        public event EventHandler<EventArgs>? ResultChanged;

        // - Element -
        public override Border Element { get; set; } = new();

        // - has been loaded -

        private bool HasBeenLoaded { get; set; } = false;

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public ResultDisplay() {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, EventArgs args) {
            // don't run if already loaded
            if (HasBeenLoaded) { return; }
            HasBeenLoaded = true;

            // border settings for main grid
            Element.BorderBrush = ResultBorderBrush;
            Element.BorderThickness = new Thickness(1);
            Element.Margin = new Thickness(3);
            Element.Padding = new Thickness(3);

            // add textblock to border
            TextBlock resultTextBlock = new TextBlock();
            resultTextBlock.FontSize = 12;
            resultTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            resultTextBlock.VerticalAlignment = VerticalAlignment.Stretch;
            resultTextBlock.TextWrapping = TextWrapping.Wrap;
            resultTextBlock.Foreground = ResultForeground;
            Element.Child = resultTextBlock;

            // text binding
            resultTextBlock.SetBinding(TextBlock.TextProperty, new Binding("Result") {
                Source = this,
                Mode = BindingMode.OneWay,
                TargetNullValue = DefaultResult
            });

            // set default result to start
            Result = DefaultResult;

            // apply layout mode
            ApplyLayoutMode();
        }

        #endregion
    }
}
