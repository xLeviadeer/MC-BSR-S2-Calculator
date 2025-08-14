using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    internal record struct FluidCoordinatePoint : IFlatCoordinate {

        // --- VARIABLES ---

        // - Position -

        private FlatCoordinatePoint _position { get; init; }

        public readonly int X => _position.X;

        public readonly int Z => _position.Z;

        // - Bottom -

        public int Bottom { get; private init; }

        // - Top -
        
        public int Top { get; private init; }


        // --- CONSTRUCTORS ---

        public FluidCoordinatePoint(FlatCoordinatePoint position, int bottom, int top) {
            _position = position;
            Bottom = bottom;
            Top = top;
        }

        // --- METHODS ---

        // - As/Of Bottom/Top -

        public static CoordinatePoint BottomOf(FluidCoordinatePoint coord)
            => new CoordinatePoint(coord.X, coord.Bottom, coord.Z);

        public readonly CoordinatePoint AsBottom()
            => BottomOf(this);

        public static CoordinatePoint TopOf(FluidCoordinatePoint coord)
            => new CoordinatePoint(coord.X, coord.Top, coord.Z);

        public readonly CoordinatePoint AsTop()
            => TopOf(this);

        // - As Flat -

        public readonly FlatCoordinatePoint AsFlat()
            => this._position;

        // --- CASTING ---

        public readonly FlatCoordinatePoint ToFlatCoordinate()
            => this._position;

        public static explicit operator FlatCoordinatePoint(FluidCoordinatePoint coord)
            => coord.ToFlatCoordinate();
    }
}
