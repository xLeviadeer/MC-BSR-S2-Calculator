using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    /// <summary>
    /// Holds 2 integers representing a coordinate from top->down (no y)
    /// </summary>
    public record struct FlatCoordinatePoint : IFlatCoordinate {

        // --- VARIABLES ---

        public required int X { get; set; } = 0;

        public required int Z { get; set; } = 0;

        // --- CONSTRUCTOR ---

        [SetsRequiredMembers]
        public FlatCoordinatePoint() { }

        [SetsRequiredMembers]
        public FlatCoordinatePoint(int x = 0, int z = 0) {
            X = x; Z = z;
        }

        // --- METHODS ---

        // - ToCoordinate -

        public static CoordinatePoint ToCoordinate(FlatCoordinatePoint flat) 
            => new CoordinatePoint(flat.X, flat.Z);

        public CoordinatePoint ToCoordinate() => ToCoordinate(this);
    }
}
