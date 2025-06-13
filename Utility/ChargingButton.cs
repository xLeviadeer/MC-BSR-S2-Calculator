using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.Utility {
    public class ChargingButton : ButtonBase {

        // --- VARIABLES ---

        private bool IsBeingPressed { get; set; } = false;

        // -- Charging --
        #region Charging

        // - publics -

        public bool IsCharging {
            get => IsBeingPressed;
        }

        public bool IsChargeCycling {
            get => (IsBeingPressed || IsDischargingFromCompletion);
        }

        // - privates -

        private bool IsDischargingFromCompletion { get; set; } = false;
        private bool IsFirstDischargeFromCompletionTick { get; set; } = true;

        private double ChargeWidth {
            get => ChargeRectangle.ActualWidth;
            set => ChargeRectangle.Width = value;
        }

        private double MaxChargeWidth {
            get => HoverRectangle.ActualWidth;
        }

        private Stopwatch Stopwatch { get; set; } = new();

        private double LastElaspedSeconds { get; set; } = 0;

        private DispatcherTimer ChargeTimer { get; set; } = new();

        #endregion

        // -- Events ---
        #region Events

        // ChargCycled
        public event EventHandler<EventArgs>? ChargeCycled;

        // FullyCharged
        public event EventHandler<EventArgs>? FullyCharged;

        // ChargingStarted
        public event EventHandler<EventArgs>? ChargingStarted;

        // DischargingStarted
        public event EventHandler<EventArgs>? DischargingStarted;

        // ZeroCharge
        public event EventHandler<EventArgs>? ZeroCharge;

        #endregion

        // -- Interface --
        #region Interface

        private Rectangle ChargeRectangle { get; set; } = new();
        private Rectangle HoverRectangle { get; set; } = new();

        // - Hover Color -

        [Category("Brush")]
        [Description("The color displayed when hovering")]
        public SolidColorBrush HoverColor {
            get => (SolidColorBrush)GetValue(HoverColorProperty);
            set => SetValue(HoverColorProperty, value);
        }

        public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register(
            nameof(HoverColor),
            typeof(SolidColorBrush),
            typeof(ChargingButton),
            new PropertyMetadata(new SolidColorBrush(ColorResources.HoverColor))
        );

        // - Click Color -

        [Category("Brush")]
        [Description("The color displayed when clicking")]
        public SolidColorBrush ClickColor {
            get => (SolidColorBrush)GetValue(ClickColorProperty);
            set => SetValue(ClickColorProperty, value);
        }

        public static readonly DependencyProperty ClickColorProperty = DependencyProperty.Register(
            nameof(ClickColor),
            typeof(SolidColorBrush),
            typeof(ChargingButton),
            new PropertyMetadata(new SolidColorBrush(ColorResources.ClickColor))
        );

        // - Charge Color -

        [Category("Brush")]
        [Description("The displayed when charging")]
        public SolidColorBrush ChargingColor {
            get => (SolidColorBrush)GetValue(ChargingColorProperty);
            set => SetValue(ChargingColorProperty, value);
        }

        public static readonly DependencyProperty ChargingColorProperty = DependencyProperty.Register(
            nameof(ChargingColor),
            typeof(SolidColorBrush),
            typeof(ChargingButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)))
        );

        // - Charged Color -

        [Category("Brush")]
        [Description("The color displayed when charging has been completed")]
        public SolidColorBrush ChargedColor {
            get => (SolidColorBrush)GetValue(ChargedColorProperty);
            set => SetValue(ChargedColorProperty, value);
        }

        public static readonly DependencyProperty ChargedColorProperty = DependencyProperty.Register(
            nameof(ChargedColor),
            typeof(SolidColorBrush),
            typeof(ChargingButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 255, 64)))
        );

        // - Color Palette (override) -

        public enum ChargingButtonColorPalettes {
            Green,
            Red
        }

        [Category("Brush")]
        [Description("A pre-determined color palette choice; overrides ChargingColor and ChargedColor")]
        public ChargingButtonColorPalettes ColorPalette {
            get => (ChargingButtonColorPalettes)GetValue(ColorPaletteProperty);
            set => SetValue(ColorPaletteProperty, value);
        }

        public static readonly DependencyProperty ColorPaletteProperty = DependencyProperty.Register(
            nameof(ColorPalette),
            typeof(ChargingButtonColorPalettes),
            typeof(ChargingButton),
            new PropertyMetadata(ChargingButtonColorPalettes.Green)
        );

        // - Charge Time -

        [Category("Common")]
        [Description("The time which the button takes to charge in seconds")]
        public double ChargeTime {
            get => (double)GetValue(ChargeTimeProperty);
            set => SetValue(ChargeTimeProperty, value);
        }

        public static readonly DependencyProperty ChargeTimeProperty = DependencyProperty.Register(
            nameof(ChargeTime),
            typeof(double),
            typeof(ChargingButton),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceChargeTime)
        );

        // ensures value can only be set between 0 and 10
        private static object CoerceChargeTime(DependencyObject d, object baseValue) {
            double value = (double)baseValue;
            if (value < 0.0) return 0.0;
            if (value > 10.0) return 10.0;
            return value;
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // get charge rectangle
            Rectangle? chargeRectangle = GetTemplateChild("ChargeRectangle") as Rectangle;
            if (chargeRectangle == null) { throw new ArgumentException(nameof(chargeRectangle)); }
            ChargeRectangle = chargeRectangle;

            // track size changes of the charge rectangle
            ChargeRectangle.SizeChanged += (s, e) => { // access the actual size so that it will always be updated
                double _ = ChargeRectangle.ActualWidth;
            };

            // get hover rectangle
            Rectangle? hoverRectangle = GetTemplateChild("HoverRectangle") as Rectangle;
            if (hoverRectangle == null) { throw new ArgumentException(nameof(hoverRectangle)); }
            HoverRectangle = hoverRectangle;

            // track size changes of hover rectangle (not really needed)
            HoverRectangle.SizeChanged += (s, e) => {
                double _ = HoverRectangle.ActualWidth;
            };

            // apply Palette
            ApplyPalette();
        }

        public void ApplyPalette() {
            switch (ColorPalette) {
                case ChargingButtonColorPalettes.Green:
                    // default green already applied
                    break;
                case ChargingButtonColorPalettes.Red:
                    // border/foreground colors
                    BorderBrush = new SolidColorBrush(ColorResources.DarkerRedColor);
                    Foreground = new SolidColorBrush(ColorResources.MediumerRedColor);

                    // charge colors
                    ChargingColor = new SolidColorBrush(ColorResources.BrighterRedColor);
                    ChargedColor = new SolidColorBrush(ColorResources.MediumerRedColor);

                    break;
            }
        }

        static ChargingButton() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ChargingButton),
                new FrameworkPropertyMetadata(typeof(ChargingButton))
            );
        }

        public ChargingButton() {
            // subscribe events
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseLeave += OnMouseLeave;
            ChargeTimer.Tick += ChargeTick;

            // set ticks to occur every 100 milliseconds
            ChargeTimer.Interval = TimeSpan.FromMilliseconds(10);
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        // - charge management -

        private void ChargeTick(object? sender, EventArgs args) {
            // gets the time passed since the last tick
            double elaspedSeconds = Stopwatch.Elapsed.TotalSeconds;
            double deltaSeconds = elaspedSeconds - LastElaspedSeconds;
            LastElaspedSeconds = elaspedSeconds;

            // filled condition
            if (IsDischargingFromCompletion) {
                // only run this once per completion discharge
                if (IsFirstDischargeFromCompletionTick) {
                    IsFirstDischargeFromCompletionTick = false;

                    // event
                    FullyCharged?.Invoke(this, EventArgs.Empty);

                    // color
                    ChargeRectangle.Fill = ChargedColor;
                }

                // add more alpha every completion discharge tick
                int currentAlpha = ((SolidColorBrush)ChargeRectangle.Fill).Color.A;
                double deltaAlpha = (ChargedColor.Color.A * (deltaSeconds / ChargeTime)) * 6; // x12 the charge time
                double prospectiveAlpha = currentAlpha - deltaAlpha;
                ChargeRectangle.Fill = new SolidColorBrush(Color.FromArgb(
                    (byte)Math.Max(prospectiveAlpha, 0),
                    ChargedColor.Color.R,
                    ChargedColor.Color.G,
                    ChargedColor.Color.B
                ));

                // once completely discharged
                if (currentAlpha == 0) {
                    // stop timers
                    Stopwatch.Stop();
                    ChargeTimer.Stop();

                    // set charge width back
                    ChargeWidth = 0;

                    // event
                    ChargeCycled?.Invoke(this, EventArgs.Empty);

                    // allow completion discharge again
                    IsFirstDischargeFromCompletionTick = true;
                    IsDischargingFromCompletion = false;
                }
            } else if (IsBeingPressed) {
                // update
                double deltaWidth = MaxChargeWidth * (deltaSeconds / ChargeTime);
                double prospectiveWidth = ChargeWidth + deltaWidth;
                ChargeWidth = Math.Min(prospectiveWidth, MaxChargeWidth); // clamps so it can never exceed the width

                // check for completion
                if (ChargeWidth == MaxChargeWidth) {
                    IsDischargingFromCompletion = true;
                }
            } else {
                // empty condition
                if (ChargeWidth == 0) {
                    // event
                    ZeroCharge?.Invoke(sender, EventArgs.Empty);

                    // stop timers
                    ChargeTimer.Stop();
                    Stopwatch.Stop();
                }

                // update
                double deltaWidth = MaxChargeWidth * (deltaSeconds / ChargeTime);
                double prospectiveWidth = ChargeWidth - (deltaWidth * 5); // x5 as fast discharge
                ChargeWidth = Math.Max(prospectiveWidth, 0); // clamps at 0
            }
        }

        // - bindable press listeners -

        /// <summary>
        /// Should be run when the button is being pressed
        /// </summary>
        public void MarkAsPressed(object? sender, EventArgs args) {
            IsBeingPressed = true;
            ChargingStarted?.Invoke(this, EventArgs.Empty);

            // start new charge if not already charging
            if (ChargeWidth == 0) {
                // reset color
                ChargeRectangle.Fill = ChargingColor;

                // start timing
                LastElaspedSeconds = 0;
                Stopwatch.Restart();
                ChargeTimer.Start();
            }
        }

        /// <summary>
        /// Should be run when the button is being unpressed
        /// </summary>
        public void MarkAsUnpressed(object? sender, EventArgs args) {
            IsBeingPressed = false;
            DischargingStarted?.Invoke(this, EventArgs.Empty);
        }

        // - mouse click listeners -
        private void OnMouseLeftButtonDown(object? sender, MouseEventArgs args) => MarkAsPressed(sender, args);
        private void OnMouseLeftButtonUp(object? sender, MouseEventArgs args) => MarkAsUnpressed(sender, args);
        private void OnMouseLeave(object? sender, MouseEventArgs args) => MarkAsUnpressed(sender, args);

        #endregion
    }
}
