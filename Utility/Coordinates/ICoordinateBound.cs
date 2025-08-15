using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal interface ICoordinateBound<StoredCoordinateType, ReturnCoordinateType> : 
        ICoordinateBoundAmbiguousReturn<StoredCoordinateType>
        where StoredCoordinateType : struct, IFlatCoordinate, IEquatable<StoredCoordinateType>
        where ReturnCoordinateType : struct, IFlatCoordinate, IEquatable<ReturnCoordinateType> {

        // --- VARIABLES ---

        // -- Cardinal Corners --

        public ReturnCoordinateType NW { get; }

        public ReturnCoordinateType NE { get; }

        public ReturnCoordinateType SW { get; }

        public ReturnCoordinateType SE { get; }
    }
}
