using MC_BSR_S2_Calculator.Utility.Coordinates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    /// <summary>
    /// Holds 2 sets of coordinates to create a square property subsection
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertySubsection {

        // --- VARIABLES ---
        #region VARIABLES

        // - Name -

        [JsonProperty("name")]
        public string? Name { get; set; } = null;

        // - A and B -

        [JsonProperty("set1")]
        public required FlatCoordinate A { get; set; } = new();

        [JsonProperty("set2")]
        public required FlatCoordinate B { get; set; } = new();

        // - Edges -

        public int Left {
            get {
                return Math.Min(A.X, B.X);
            }
        }

        public int Right {
            get {
                return Math.Max(A.X, B.X);
            }
        }

        public int Bottom {
            get {
                return Math.Min(A.Z, B.Z);
            }
        }

        public int Top {
            get {
                return Math.Max(A.Z, B.Z);
            }
        }

        // - Corners -

        public FlatCoordinate TopLeft => new FlatCoordinate(Left, Top);

        public FlatCoordinate TopRight => new FlatCoordinate(Right, Top);

        public FlatCoordinate BottomLeft => new FlatCoordinate(Left, Bottom);

        public FlatCoordinate BottomRight => new FlatCoordinate(Right, Bottom);

        // - Height and Width -
        
        public int Width => (int)Math.Abs(A.X - B.X) + 1;

        public int Height => (int)Math.Abs(A.Z - B.Z) + 1;

        // - Metric -

        public int Metric => Width * Height;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        [SetsRequiredMembers]
        public PropertySubsection() {}

        [SetsRequiredMembers]
        public PropertySubsection(FlatCoordinate a, FlatCoordinate b, string? name=null) {
            A = a; B = b;
            Name = name;
        }

        #endregion

        // --- CASTING ---

        public static implicit operator int(PropertySubsection section)
            => section.Metric;
    }
}
