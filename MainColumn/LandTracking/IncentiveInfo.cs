using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public interface IInfoHolder {
        public ImmutableList<IncentiveInfo> SelectableIncentives { get; }
        public ImmutableList<IncentiveInfo> UnselectableIncentives { get; }
        public ImmutableList<IncentiveInfo> AllIncentives { get; }
        public abstract IncentiveInfo FindByName(string name);
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
        public class Tax : IInfoHolder {

            // - selectable -

            public const string UnderTheIce = "Under the Ice";
            public const string AboveTheIce = "Above the Ice";
            public const string PuncturingTheIce = "Puncturing the Ice";
            public ImmutableList<IncentiveInfo> SelectableIncentives { get; } = [
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

            public ImmutableList<IncentiveInfo> UnselectableIncentives { get; } = [
                new(
                    OwnedProperty, 
                    0.3d,
                    "The Private Property over Owned Property Incentive aims to incentivize Citizens to build within Hard Land.\r\n\r\nOwned Property gets a 30% increase in Taxes."
                )
            ];

            // - all -

            public ImmutableList<IncentiveInfo> AllIncentives {
                get => SelectableIncentives.Concat(UnselectableIncentives).ToImmutableList();
            }

            // - find function -

            public IncentiveInfo FindByName(string name) {
                foreach (var incentive in AllIncentives) {
                    if (incentive.Name == name) { return incentive; }
                }
                return NoneInfo;
            }

            // - singleton -

            public static Tax Instance { get; } = new();

            private Tax() { }
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
