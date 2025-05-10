using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns.DisplayList {
    internal class TestClass : Displayable {

        [DisplayValue("Boolean Text Block", 0, 0)]
        public BoundDisplayValue<TextBlock, bool> BooleanTextBlock { get; init; }

        [DisplayValue("Button", 0, HorizontalAlignment.Stretch, VerticalAlignment.Stretch, 1)]
        public DisplayValue<Button> ButtonUpdater { get; init; }

        public TestClass() {
            var textBlock = new TextBlock();
            BooleanTextBlock = new(
                false,
                textBlock,
                textBlock
            );

            var button = new Button() {
                Content = "swap!"
            };
            button.Click += (sender, args) => {
                BooleanTextBlock.Value = !BooleanTextBlock.Value;
            };
            button.FontSize = 10;
            ButtonUpdater = new(button);
        }
    }
}
