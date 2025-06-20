using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public interface IIncentiveInfoHolder {
        public ImmutableList<IncentiveInfo> Selectable { get; }
        public ImmutableList<IncentiveInfo> All { get; }
        public abstract IncentiveInfo FindByName(string name);
        public IncentiveInfo FindByNameFromAll(string name) {
            foreach (var incentive in All) {
                if (incentive.Name == name) { return incentive; }
            }
            return IncentiveInfo.NoneInfo;
        }
    }

    public class IncentiveInfo {

        // --- VARIABLES ---

        // -- Static --

        // - None -

        public const string None = "None";

        public static readonly IncentiveInfo NoneInfo = new(
            None,
            0.0d,
            "null"
        );

        // - Tax Info -
        public class Tax : IIncentiveInfoHolder {

            // - selectable -

            public const string UnderTheIce = "Under the Ice";
            public const string AboveTheIce = "Above the Ice";
            public const string PuncturingTheIce = "Puncturing the Ice";
            public ImmutableList<IncentiveInfo> Selectable { get; } = [
                new(
                    UnderTheIce, 
                    -0.3d,
                    "Property Owners who own Property under the ice get a 30% reduction in Taxes.\r\nUnder the ice is defined as Property which is underneath the ice in a glacier biome, not underground and leaves an at least 1 block gap between the ice and the property (Y61 inclusive).\r\nProperties with larger (95% or more volumetric) portions above the ice are not considered under the ice."
                ),
                new(
                    AboveTheIce, 
                    0.3d,
                    "Property Owners who own Property above the ice get a 30% increase in Taxes.\r\nAbove the ice is defined as Property which is above the ice (Y64 inclusive) which at least 1 block of sits anywhere above the ice in a glacier biome.\r\nProperties with larger (95% or more volumetric) portions under the ice are not considered above the ice."
                ),
                new(
                    PuncturingTheIce, 
                    0.15d,
                    "Property Owners who own Property puncturing the ice get a 15% increase in Taxes.\r\nPuncturing the ice is defined as Property which is at least partially in the ice (Y62 - 63 inclusive) inside of a glacier biome."
                )
            ];

            // - unselectable -

            public const string OwnedProperty = "Owned Property";

            public ImmutableList<IncentiveInfo> Unselectable { get; } = [
                new(
                    OwnedProperty, 
                    0.3d,
                    "The Private Property over Owned Property Incentive aims to incentivize Citizens to build within Hard Land.\r\n\r\nOwned Property gets a 30% increase in Taxes."
                )
            ];

            // - all -

            public ImmutableList<IncentiveInfo> All {
                get => Selectable.Concat(Unselectable).ToImmutableList();
            }

            // - find function -

            public IncentiveInfo FindByName(string name) => ((IIncentiveInfoHolder)this).FindByNameFromAll(name);

            // - singleton -

            public static Tax Instance { get; } = new();

            private Tax() { }
        }

        // - violation info -
        public class Violation : IIncentiveInfoHolder {

            // - selectable -

            public const string LandAntiMutilation = "Land Anti-Mutilation";
            public const string GroundedStructures = "Grounded Structures";
            public const string AntiInflation = "Anti-Inflation";
            public ImmutableList<IncentiveInfo> Selectable { get; } = [
                new(
                    LandAntiMutilation,
                    5d,
                    "The Land Anti-Mutilation Incentive works to stop land from being mutilated and instead to incentivize Citizens to build \"with the land\". If prospectively purchased Land will be flattened before it is constructed on, depending on the severity of flattening, the price of land is increased.\r\n\r\nVariance is defined as the difference in height of Surface Blocks.\r\nTotal variance is defined as the total amount of variance a Property has.\r\nRemoval of variance can occur by making actions (destroying, placing blocks) which result in total variance becoming smaller. Additionally, actions which almost change the total variance, but not quite (80% of variance for a particular X, Z slice of blocks removed) count as as removing variance.\r\nWhen the total variance becomes 0, variance can no longer be removed.\r\n\r\nFor every block of variance (each Y level) removed for construction before becoming level increases (adds to) the Land price by 5 Diamonds."
                ),
                new(
                    GroundedStructures,
                    5d/30d,
                    "The Grounded Structures Incentive works to stop builds from floating in the air randomly.\r\n\r\nFloating land is land which floats at least 20 blocks from Surface Blocks.\r\nSupports are defined as at least 1 solid block (including blocks like fences, but not blocks like string, glass, or other transparent blocks (blocks that are more than 80% see-through)) thick pillars which extend from the floating land to Surface Blocks without any airspace.\r\nSupports must be placed within a 10 block square radius of each other and additionally at the corners (or most corner-like edges) of the the floating land.\r\nWalls can qualify as supports. Supports can be diagonal.\r\n\r\nIf a Citizen anticipates to construct a floating structure (any kind of build which has air between Surface Blocks and the structure's bottom which are not fully or 95% fully enclosed with walls) they must either add supports or be charged 5 Diamonds, ceilinged, for every 30 blocks of blocked sunlight. This means that blocks like glass don't count against the Grounded Structures Incentive."
                ),
                new(
                    AntiInflation,
                    30d,
                    "The Anti-Inflation Incentive works to stop the value of Diamonds to inflate and hence make currency less valuable.\r\n\r\nIf a Citizen wishes to construct a staircase reaching into the ground (a staircase which cumulatively reaches 20 blocks below Surface Blocks) on their land they must pay 30 Diamonds.\r\nA staircase separated by rooms or areas counts as a singular staircase."
                )
            ];

            // - all -

            public ImmutableList<IncentiveInfo> All {
                get => Selectable;
            }

            // - find function -

            public IncentiveInfo FindByName(string name) => ((IIncentiveInfoHolder)this).FindByNameFromAll(name);

            // - singleton -

            public static Violation Instance { get; } = new();

            private Violation() { }
        }

        // - purchase info -

        public class Purchase : IIncentiveInfoHolder {

            // - selectable -

            public const string UnderwaterConstruction = "Underwater Construction";
            public const string UnderIceConstruction = "Under-ice Construction";
            public ImmutableList<IncentiveInfo> Selectable { get; } = [
                new(
                    UnderwaterConstruction,
                    0.4d,
                    "If Citizens anticipate to construct a Private Property where no less than 80% of the Property Volumetric of the construction is underwater they will receive a 40% floored discount on their Property purchase. They will be given a book signed by an Elected Official signifying this. If the Property Owner doesn't follow the terms of this deal after buying the Property they will owe a Fine equal to the amount of the Diamonds they were discounted to the Government."
                ),
                new(
                    UnderIceConstruction,
                    0.8d,
                    "If Citizens anticipate to construct a Private Property where no less than 80% of the volume of the construction is underwater and the property is underneath Y62 in a Glacier biome and not underground they will receive a 80% floored discount on their Property purchase. They will be given a book signed by an Elected Official signifying this. If the Property Owner doesn't follow the terms of this deal after buying the Property they will owe a Fine equal to the amount of the Diamonds they were discounted to the Government."
                )
            ];

            // - all -

            public ImmutableList<IncentiveInfo> All {
                get => Selectable;
            }

            // - find function -

            public IncentiveInfo FindByName(string name) => ((IIncentiveInfoHolder)this).FindByNameFromAll(name);

            // - singleton -

            public static Purchase Instance { get; } = new();

            private Purchase() { }
        }

        // -- Instance --

        public string Name { get; protected init; }

        public double Value { get; protected init; }

        public string Description { get; protected init; }

        // --- CONSTRUCTOR ---

        public IncentiveInfo(
            string name,
            double value,
            string description
        ) {
            Name = name;
            Value = value;
            Description = description;
        }
    }
}
