using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public interface ICoordinateBoundAmbiguous {

        // --- VARIABLES ---

        // -- Stored Values --

        [JsonProperty("a")]
        public IFlatCoordinate AmbigA { get; }

        [JsonProperty("b")]
        public IFlatCoordinate AmbigB { get; }

        // -- Cardinal Edges --

        public int West { get; }

        public int East { get; }

        public int North { get; }

        public int South { get; }

        // -- Cardinal Corners --

        public IFlatCoordinate AmbigNW { get; }

        public IFlatCoordinate AmbigNE { get; }

        public IFlatCoordinate AmbigSW { get; }

        public IFlatCoordinate AmbigSE { get; }

        // -- Measures --

        public int Length { get; }

        public int Width { get; }

        public int Area { get; }
    }
}
