using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {

    /// <remarks>
    /// Should only be used with ID based classes
    /// </remarks>
    public interface IInstanceID {

        public object Instance { get; set; }

        public void AssignInstance(object instance);
    }
}
