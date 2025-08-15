using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public interface ILandArea {

        // --- VARIABLES ---

        // -- Static --

        // - Property Types - 
        #region Property Types

        public const string PUBLIC = "public";
        public const string PROVISIONED = "provisioned";
        public const string UNOWNED = "unowned";

        public static ImmutableList<string> GovernmentalLandTypes { get; set; } = [
            PUBLIC,
            PROVISIONED,
            UNOWNED
        ];

        public const string PRIVATE = "private";
        public const string SHARED_PRIVATE = "shared private";
        public const string OWNED = "owned";

        public static ImmutableList<string> PlayerLandTypes { get; set; } = [
            PRIVATE,
            SHARED_PRIVATE,
            OWNED
        ];

        public static ImmutableList<string> AllLandTypes = [
            PUBLIC,
            SHARED_PRIVATE,
            PRIVATE,
            UNOWNED,
            PROVISIONED,
            OWNED
        ];

        #endregion

        // - Land Definitions

        // world height
        public const int WORLD_HEIGHT = 320;
        public const int WORLD_DEPTH = -64;

        // surface landarea
        public const int SURFACE_LANDAREA_Y_LEVEL = 50;
        public const int SURFACE_LANDAREA_Y_LEVEL_UNDERWATER = 10;
        public static int SurfaceLandareaYLevelMax
            => Math.Max(SURFACE_LANDAREA_Y_LEVEL, SURFACE_LANDAREA_Y_LEVEL_UNDERWATER);
        public static int SurfaceLandareaYLevelMin
            => Math.Min(SURFACE_LANDAREA_Y_LEVEL, SURFACE_LANDAREA_Y_LEVEL_UNDERWATER);

        // -- Enforcement --
        #region Enforcement

        // - As Requirement -

        public ILandArea AsILandArea { get; }

        // - Default Top/Bottom -


        public int DefaultBottom { get; }

        public int DefaultTop { get; }

        public bool HasValidDefaults() {
            if (
                (DefaultBottom < DefaultTop)
                && (ILandArea.WORLD_DEPTH <= DefaultBottom)
                && (DefaultTop <= ILandArea.WORLD_HEIGHT)
            ) { return true; } else { return false; }
        }

        // - Bounds -

        [JsonProperty("bounds")]
        public List<ICoordinateBoundAmbiguous> Bounds { get; }

        public abstract List<ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>> GetBoundsAsSquares();

        public List<ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>> GetBoundsAsSquaresHelper() {
            // return list
            List<ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>> lst = [];

            // check every bound
            foreach (ICoordinateBoundAmbiguous bound in Bounds) {
                if (bound is ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint> twoDBound) {
                    // add to return
                    lst.Add(twoDBound);
                } else if (bound is ICoordinateBoundAmbiguousReturn<CoordinatePoint> threeDBound) {
                    // cast to square
                    lst.Add((ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>)threeDBound);
                } else {
                    throw new InvalidOperationException("at least one ICoordinateBound in Bounds was not a valid bound");
                }
            }

            // return
            return lst;
        }

        public abstract List<ICoordinateBoundAmbiguousReturn<CoordinatePoint>> GetBoundsAsCubes();

        public List<ICoordinateBoundAmbiguousReturn<CoordinatePoint>> GetBoundsAsCubesHelper() {
            // return list
            List<ICoordinateBoundAmbiguousReturn<CoordinatePoint>> lst = [];

            // check every bounds
            foreach (ICoordinateBoundAmbiguous bound in Bounds) {
                if (bound is ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint> twoDBound) {
                    // add to return
                    lst.Add(new CoordinateCube(
                        new(twoDBound.A.X, DefaultBottom, twoDBound.A.Z),
                        new(twoDBound.B.X, DefaultTop, twoDBound.B.Z)
                    ));
                } else if (bound is ICoordinateBoundAmbiguousReturn<CoordinatePoint> threeDBound) {
                    // cast to square
                    lst.Add(threeDBound);
                } else {
                    throw new InvalidOperationException("at least one ICoordinateBound in Bounds was not a valid bound");
                }
            }

            // return
            return lst;
        }

        // - Owner ID -

        [JsonProperty("owner")]
        public IDTrace? OwnerID { get; }

        // - Land Type -

        [JsonProperty("type")]
        public string LandType { get; }

        #endregion
    }
}
