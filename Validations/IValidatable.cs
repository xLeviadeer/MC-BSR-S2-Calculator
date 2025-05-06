using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Validations {
    internal interface IValidatable {

        /// <remarks>
        /// Method is expected to fail by throwing exceptions
        /// </remarks>
        internal void Validate(Object sender, EventArgs args);
    }
}
