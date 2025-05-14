using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Validations {
    public interface IValidatable {

        /// <summary>
        /// Whether or not the object is currently valid
        /// </summary>
        public bool? IsValid { get; set; }

        /// <remarks>
        /// Method is expected to fail by throwing exceptions
        /// </remarks>
        public void Validate(object sender, EventArgs args);
    }
}
