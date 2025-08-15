using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public interface ICoordinateBoundAmbiguousReturn<StoredCoordinateType> : 
        ICoordinateBoundAmbiguous
        where StoredCoordinateType : struct, IFlatCoordinate, IEquatable<StoredCoordinateType> {

        // -- Stored Values --

        public StoredCoordinateType A { get; }

        public StoredCoordinateType B { get; }
    }
}
