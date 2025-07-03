using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Coordinates {

    /// <summary>
    /// Holds 3 integers representing a coordinate
    /// </summary>
    public record struct Coordinate : ICoordinate {

        // --- VARIABLES ---

        public required int X { get; set; } = 0;

        public required int Y { get; set; } = 0;

        public required int Z { get; set; } = 0;

        // --- CONSTRUCTOR ---

        [SetsRequiredMembers]
        public Coordinate() { }

        [SetsRequiredMembers]
        public Coordinate(int x=0, int y=0, int z=0) {
            X = x; Y = y; Z = z;
        }

        // --- METHODS ---

        // - ToFlatCoordinate -

        /// <summary>
        /// <para><b>WARNING:</b> Discards Y value </para>
        /// </summary>
        public static FlatCoordinate ToFlatCoordinate(Coordinate coord)
            => new FlatCoordinate(coord.X, coord.Z);

        /// <summary>
        /// <para><b>WARNING:</b> Discards Y value </para>
        /// </summary>
        public FlatCoordinate ToFlatCoordinate() => ToFlatCoordinate(this);
    }
}
