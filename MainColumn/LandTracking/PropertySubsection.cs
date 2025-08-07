using MC_BSR_S2_Calculator.Utility.Coordinates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    /// <summary>
    /// Holds 2 sets of coordinates to create a square property subsection
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PropertySubsection : ICloneable {

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

        [SetsRequiredMembers]
        public PropertySubsection(PropertySubsection subsection) {
            A = subsection.A; B = subsection.B;
            Name = subsection.Name;
        }

        #endregion

        // --- METHODS ---

        // - Equality Checking -

        public static bool IsEqualValue(PropertySubsection subsectionA, PropertySubsection subsectionB) {
            return ((subsectionA.A == subsectionB.A)
                && (subsectionA.B == subsectionB.B));
        }

        public bool IsEqualValue(PropertySubsection otherSubsection)
            => IsEqualValue(this, otherSubsection);

        // - To String -

        public override string ToString() {
            return $"PropertySubsection {{ Name = {Name}, A = {A.ToString()}, B = {B.ToString()}}}";
        }

        // - Copy -

        public static PropertySubsection HardCopy(PropertySubsection subsection) 
            => new PropertySubsection(subsection);

        public PropertySubsection HardCopy()
            => HardCopy(this);

        public object Clone()
            => HardCopy();

        // --- CASTING ---

        public static implicit operator int(PropertySubsection section)
            => section.Metric;
    }
}
