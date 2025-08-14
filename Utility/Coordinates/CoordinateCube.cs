using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {
    internal record struct CoordinateCube : 
        ICoordinateBound<CoordinatePoint, FluidCoordinatePoint> {

        // --- VARIABLES ---

        // -- Stored Values --

        public IFlatCoordinate AmbigA { get; private init; }

        public CoordinatePoint A {
            get => (CoordinatePoint)AmbigA;
            private init => AmbigA = value;
        }

        public IFlatCoordinate AmbigB { get; private init; }

        public CoordinatePoint B {
            get => (CoordinatePoint)AmbigB;
            private init => AmbigB = value;
        }

        // -- Cardinal Edges --

        public int West => Math.Max(A.X, B.X);

        public int East => Math.Min(A.X, B.X);

        public int North => Math.Max(A.Z, B.Z);

        public int South => Math.Max(A.Z, B.Z);

        public int Top => Math.Max(A.Y, B.Y);

        public int Bottom => Math.Min(A.Y, B.Y);

        // -- Cardinal Corners --

        public IFlatCoordinate AmbigNW => new FluidCoordinatePoint(new FlatCoordinatePoint(West, North), Top, Bottom);
        public FluidCoordinatePoint NW => (FluidCoordinatePoint)AmbigNW;

        public IFlatCoordinate AmbigNE => new FluidCoordinatePoint(new FlatCoordinatePoint(East, North), Top, Bottom);
        public FluidCoordinatePoint NE => (FluidCoordinatePoint)AmbigNE;

        public IFlatCoordinate AmbigSW => new FluidCoordinatePoint(new FlatCoordinatePoint(West, South), Top, Bottom);
        public FluidCoordinatePoint SW => (FluidCoordinatePoint)AmbigSW;

        public IFlatCoordinate AmbigSE => new FluidCoordinatePoint(new FlatCoordinatePoint(East, South), Top, Bottom);
        public FluidCoordinatePoint SE => (FluidCoordinatePoint)AmbigSE;

        // -- Measures --

        public int Length => (int)Math.Abs(North - South) + 1;

        public int Width => (int)Math.Abs(East - West) + 1;

        public int Height => (int)Math.Abs(Top - Bottom) + 1;

        public int Area => Length * Width;

        public int Volume => Area * Height;

        // --- CONSTRUCTORS ---

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public CoordinateCube(CoordinatePoint a, CoordinatePoint b)
            => (A, B) = (a, b);
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // --- CASTING ---

        public static CoordinateSquare ToCoordinateSquare(CoordinateCube cube)
            => new CoordinateSquare(cube.NW.AsFlat(), cube.SE.AsFlat());

        public CoordinateSquare ToCoordinateSquare()
            => ToCoordinateSquare(this);

        public static explicit operator CoordinateSquare(CoordinateCube cube)
            => cube.ToCoordinateSquare();
    }
}
