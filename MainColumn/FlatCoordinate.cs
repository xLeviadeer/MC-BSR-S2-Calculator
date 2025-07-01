using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn {

    /// <summary>
    /// Holds 2 integers representing a coordinate from top->down (no y)
    /// </summary>
    public record struct FlatCoordinate : ICoordinate {
        // --- VARIABLES ---

        public required int X { get; set; } = 0;

        public int Y { get; } = 0;

        public required int Z { get; set; } = 0;

        // --- CONSTRUCTOR ---

        [SetsRequiredMembers]
        public FlatCoordinate() { }

        [SetsRequiredMembers]
        public FlatCoordinate(int x = 0, int z = 0) {
            X = x; Z = z;
        }

        // --- METHODS ---

        // - ToCoordinate -

        public static Coordinate ToCoordinate(FlatCoordinate flat) 
            => new Coordinate(flat.X, flat.Z);

        public Coordinate ToCoordinate() => ToCoordinate(this);
    }
}
