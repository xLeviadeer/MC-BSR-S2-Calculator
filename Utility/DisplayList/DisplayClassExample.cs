using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.CompilerServices;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {
    internal class DisplayClassExample : Displayable {

        // a bound display value which can be updated
        [DisplayValue("Boolean Value", 0, GridUnitType.Star, 0)]
        [JsonProperty("Boolean Value")]
        public BoundDisplayValue<TextBlock, bool> BooleanTextBlock { get; init; }

        // a display value button
        [DisplayValue("Button", 0, GridUnitType.Star, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, 1)]
        public DisplayValue<Button> ButtonUpdater { get; init; }

        // add a text block and button to the values
        public DisplayClassExample() : base() {
            // text
            var textBlock = new TextBlock();
            BooleanTextBlock = new(
                textBlock,
                TextBlock.TextProperty,
                false
            );

            // button
            var button = new Button() {
                Content = "swap!"
            };
            button.Click += ClickEvent;
            button.FontSize = 10;
            ButtonUpdater = new(button);

            // right click
            RightClickMenu = new ContextMenu();
            var optionOne = new MenuItem();
            optionOne.Header = "Option 1";
            RightClickMenu.Items.Add(optionOne);
        }

        // click event for button
        private void ClickEvent(object? sender, EventArgs args) {
            BooleanTextBlock.Value = !BooleanTextBlock.Value;
        }

        // column click event
        public override void HeldLeftClickListener(object? sender, EventArgs args) {
            BooleanTextBlock.Value = !BooleanTextBlock.Value;
        }
    }
}
