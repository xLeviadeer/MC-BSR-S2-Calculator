using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Validations {
    internal class ValidationException : Exception {
        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS

        // default constructor
        public ValidationException() { }

        // message constructor
        public ValidationException(string message)
            : base(message) { }

        // message and inner exception constructor
        public ValidationException(string message, Exception innerException)
            : base(message, innerException) { }

        #endregion
    }
}
