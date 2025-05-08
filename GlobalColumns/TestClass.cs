using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns {
    internal class TestClass : Displayable {

        [DisplayValue("Some Value")]
        public BoundDisplayValue<bool> SomeValue { get; init; }

        [DisplayValue("Some Other Value")]
        public DisplayValue SomeOtherValue { get; init; }

        public TestClass() {
            TextBlock someTextBlock = new();
            SomeValue = new(
                true,
                someTextBlock,
                someTextBlock
            );

            SomeOtherValue = new(someTextBlock);
        }
    }
}
