using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.GlobalColumns {
    internal class TestClass : Displayable<TestClass> {
        [DisplayValue]
        internal DisplayValue<bool> SomeValue { get; init; } = true;
    }
}
