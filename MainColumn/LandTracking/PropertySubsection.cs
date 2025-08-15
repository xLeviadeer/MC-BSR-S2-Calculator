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
    public class PropertySubsection : ICoordinateBound<FlatCoordinatePoint, FlatCoordinatePoint>, ICloneable {

        // --- VARIABLES ---
        #region VARIABLES

        // - Name -

        [JsonProperty("name")]
        public string? Name { get; set; } = null;

        // - A and B -

        public IFlatCoordinate AmbigA { get; set; } = new FlatCoordinatePoint();

        [JsonProperty("set1")]
        public required FlatCoordinatePoint A {
            get => (FlatCoordinatePoint)AmbigA;
            set => AmbigA = value;
        }

        public IFlatCoordinate AmbigB { get; set; } = new FlatCoordinatePoint();

        [JsonProperty("set2")]
        public required FlatCoordinatePoint B {
            get => (FlatCoordinatePoint)AmbigB;
            set => AmbigB = value;
        }

        // - Edges -

        public int West {
            get {
                return Math.Min(A.X, B.X);
            }
        }

        public int East {
            get {
                return Math.Max(A.X, B.X);
            }
        }

        public int South {
            get {
                return Math.Min(A.Z, B.Z);
            }
        }

        public int North {
            get {
                return Math.Max(A.Z, B.Z);
            }
        }

        // - Corners -

        public IFlatCoordinate AmbigNW => new FlatCoordinatePoint(West, North);
        public FlatCoordinatePoint NW => (FlatCoordinatePoint)AmbigNW;

        public IFlatCoordinate AmbigNE => new FlatCoordinatePoint(East, North);
        public FlatCoordinatePoint NE => (FlatCoordinatePoint)AmbigNE;

        public IFlatCoordinate AmbigSW => new FlatCoordinatePoint(West, South);
        public FlatCoordinatePoint SW => (FlatCoordinatePoint)AmbigSW;

        public IFlatCoordinate AmbigSE => new FlatCoordinatePoint(East, South);
        public FlatCoordinatePoint SE => (FlatCoordinatePoint)AmbigSE;

        // - Height and Width -

        public int Width => (int)Math.Abs(A.X - B.X) + 1;

        public int Length => (int)Math.Abs(A.Z - B.Z) + 1;

        // - Metric -

        public int Area => Width * Length;

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        [SetsRequiredMembers]
        public PropertySubsection() {}

        [SetsRequiredMembers]
        public PropertySubsection(FlatCoordinatePoint a, FlatCoordinatePoint b, string? name=null) {
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
        #region METHODS

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

        // - Contains -

        public bool Contains(IFlatCoordinate coord)
            => ((ICoordinateBoundAmbiguous)this).Contains2DHelper(coord);

        // --- CASTING ---

        public static implicit operator int(PropertySubsection section)
            => section.Area;

        #endregion
    }
}
