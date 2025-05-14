using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {
    internal class DisplayClassExample : Displayable {

        // a bound display value which can be updated
        [DisplayValue("Boolean Value", 0, 0)]
        [JsonProperty("Boolean Value")]
        public BoundDisplayValue<TextBlock, bool> BooleanTextBlock { get; init; }

        // a display value button
        [DisplayValue("Button", 0, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, 1)]
        public DisplayValue<Button> ButtonUpdater { get; init; }

        // add a text block and button to the values
        public DisplayClassExample() {
            var textBlock = new TextBlock();
            BooleanTextBlock = new(
                textBlock,
                TextBlock.TextProperty,
                false
            );

            var button = new Button() {
                Content = "swap!"
            };
            button.Click += ClickEvent;
            button.FontSize = 10;
            ButtonUpdater = new(button);
        }

        // click event for button
        private void ClickEvent(object? sender, EventArgs args) {
            BooleanTextBlock.Value = !BooleanTextBlock.Value;
        }

        // column click event
        public override void HeldListener(object? sender, EventArgs args) {
            BooleanTextBlock.Value = !BooleanTextBlock.Value;
        }
    }
}
