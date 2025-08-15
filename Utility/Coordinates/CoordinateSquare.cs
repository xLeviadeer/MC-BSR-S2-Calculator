using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {
    public record struct CoordinateSquare : 
        ICoordinateBound<FlatCoordinatePoint, FlatCoordinatePoint> {

        // --- VARIABLES ---

        // -- Stored Values --

        public IFlatCoordinate AmbigA { get; private init; }

        public FlatCoordinatePoint A {
            get => (FlatCoordinatePoint)AmbigA;
            private init => AmbigA = value;
        }

        public IFlatCoordinate AmbigB { get; private init; }

        public FlatCoordinatePoint B {
            get => (FlatCoordinatePoint)AmbigB;
            private init => AmbigB = value;
        }

        // -- Cardinal Edges --

        public int West => Math.Max(A.X, B.X);

        public int East => Math.Min(A.X, B.X);

        public int North => Math.Max(A.Z, B.Z);

        public int South => Math.Max(A.Z, B.Z);

        // -- Cardinal Corners --

        public IFlatCoordinate AmbigNW => new FlatCoordinatePoint(West, North);
        public FlatCoordinatePoint NW => (FlatCoordinatePoint)AmbigNW;

        public IFlatCoordinate AmbigNE => new FlatCoordinatePoint(East, North);
        public FlatCoordinatePoint NE => (FlatCoordinatePoint)AmbigNE;

        public IFlatCoordinate AmbigSW => new FlatCoordinatePoint(West, South);
        public FlatCoordinatePoint SW => (FlatCoordinatePoint)AmbigSW;

        public IFlatCoordinate AmbigSE => new FlatCoordinatePoint(East, South);
        public FlatCoordinatePoint SE => (FlatCoordinatePoint)AmbigSE;

        // -- Measures --

        public int Length => (int)Math.Abs(North - South) + 1;

        public int Width => (int)Math.Abs(East - West) + 1;

        public int Area => Length * Width;

        // --- CONSTRUCTORS ---

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public CoordinateSquare(FlatCoordinatePoint a, FlatCoordinatePoint b)
            => (A, B) = (a, b);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // --- METHODS ---

        public bool Contains(IFlatCoordinate coord)
            => ((ICoordinateBoundAmbiguous)this).Contains2DHelper(coord);
    }
}
