using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.DisplayList;
using MC_BSR_S2_Calculator.Utility.Identification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Property : Displayable, IIDHolder {

        // --- VARIABLES ---

        // -- STATIC --

        // - Property Types - 
        #region Property Types

        public const string Public = "public";
        public const string Provisioned = "provisioned";
        public const string Unowned = "unowned";

        public static ImmutableList<string> GovernmentalPropertyTypes { get; set; } = [
            Public,
            Provisioned,
            Unowned
        ];

        public const string Private = "private";
        public const string SharedPrivate = "shared private";
        public const string Owned = "owned";

        public static ImmutableList<string> PlayerPropertyTypes { get; set; } = [
            Private,
            SharedPrivate,
            Owned
        ];

        public static ImmutableList<string> AllPropertyTypes = [
            Public,
            SharedPrivate,
            Private,
            Unowned,
            Provisioned,
            Owned
        ];

        #endregion

        // - Property Sizes -
        #region Property Sizes

        public static class PropertySize {
            public static string Invalid { get; } = "-";
            public static string Small { get; } = "Small";
            public static string Large { get; } = "Large";
            public static string Massive { get; } = "Massive";
        }

        #endregion

        // -- INSTANCE --
        #region INSTANCE

        // - ID -

        public static char TypeCharacter { get; } = 'o';

        [JsonProperty("property_id")]
        public IDPrimary PropertyID { get; set; } = new(typeof(Property), TypeCharacter);

        // - Name -

        [JsonProperty("name")]
        [DisplayValue("Name", 1, GridUnitType.Star)]
        public BoundDisplayValue<Label, string> Name { get; set; }

        #endregion

        // --- CONSTRUCTOR ---

        #region CONSTRUCTOR

        private void SetDefaultValues(
            string name
        ) {
            // name
            Name = new(
                new Label(),
                Label.ContentProperty,
                name
            );
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// JSON entry only; DO NOT USE
        /// </summary>
        public Property() {
            SetDefaultValues(
                string.Empty
            );
        }

        /// <summary>
        /// Use this constructor only
        /// </summary>
        public Property(string name) {
            PropertyID.AssignNewID(this);
            SetDefaultValues(
                name
            );
        }

        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        #endregion

        // --- METHODS ---

        // -- Property Metric and Results --
        #region Property Metric and Results

        // - Property Metric -

        public static int GetPropertyMetric(PropertySubsection[] subsections) {
            int total = 0;
            foreach (PropertySubsection subsection in subsections) {
                total += subsection.Metric;
            }
            return total;
        }

        // - Property Size -

        public static string GetPropertySize(PropertySubsection[] subsections)
            => GetPropertySize(GetPropertyMetric(subsections));

        public static string GetPropertySize(int propertyMetric)
            => propertyMetric switch {
                (>= 0 and <= 400) => PropertySize.Small,
                (> 400 and <= 20000) => PropertySize.Large,
                (> 20000) => PropertySize.Massive,
                _ => throw new ArgumentOutOfRangeException($"the property metric was out of range {propertyMetric}")
            };

        // - Purchase Pricing -

        public static int GetPurcahseValueBase(PropertySubsection[] subsections)
            => GetPurcahseValueBase(GetPropertyMetric(subsections));

        private static int GetPurcahseValueBase(int propertyMetric)
            => (int)Math.Ceiling(propertyMetric / 8d);            

        public static double GetTotalIncentivesValue(ActiveIncentive[] activeIncentives) {
            double total = 0;
            foreach (ActiveIncentive incentive in activeIncentives) {
                total += incentive.Total;
            }
            return Math.Round(total, 2);
        }

        public static int GetPurchaseValueFinal(
            PropertySubsection[] subsections,
            ActiveViolationIncentive[] violationIncentives,
            ActiveIncentive[] purchaseIncentives,
            out int purchaseValue,
            out int incentivesAmount
        ) => GetPurchaseValueFinal(
            GetPurcahseValueBase(subsections),
            violationIncentives,
            purchaseIncentives,
            out purchaseValue,
            out incentivesAmount
        );

        public static int GetPurchaseValueFinal(
            int propertyMetric,
            ActiveViolationIncentive[] violationIncentives,
            ActiveIncentive[] purchaseIncentives,
            out int purchaseValue,
            out int incentivesAmount
        ) {
            // get property metric amount (value before incentives)
            purchaseValue = GetPurcahseValueBase(propertyMetric);

            // apply purchase incentives
            double purchaseIncentivesValue = GetTotalIncentivesValue(purchaseIncentives);
            double valueAfterPurchaseIncentives = (1 + purchaseIncentivesValue) * purchaseValue;

            // apply violation incentives
            double violationIncentivesValue = GetTotalIncentivesValue(
                violationIncentives
                .Where(incentive => incentive.ViolationType == ViolationIncentive.ViolationTypes.Once)
                .ToArray()
            );
            int valueAfterViolationIncentives = (int)Math.Ceiling(valueAfterPurchaseIncentives + violationIncentivesValue);
            incentivesAmount = valueAfterViolationIncentives - purchaseValue;

            // return
            return valueAfterViolationIncentives;
        }

        // - Tax Contribution -

        public static double GetTaxContributionBase(PropertySubsection[] subsections)
            => GetTaxContributionBase(GetPropertyMetric(subsections));

        private static double GetTaxContributionBase(int propertyMetric)
            => propertyMetric / 200d;

        public static int GetTotalTaxContribution(
            PropertySubsection[] subsections,
            ActiveIncentive[] taxIncentives,
            ActiveViolationIncentive[] violationIncentives,
            string propertyType,
            int numberOfResidents,
            out double taxValue,
            out double propertyTypesAmount,
            out double incentivesAmount
        ) => GetTotalTaxContribution(
            GetPropertyMetric(subsections),
            taxIncentives,
            violationIncentives,
            propertyType,
            numberOfResidents,
            out taxValue,
            out propertyTypesAmount,
            out incentivesAmount
        );

        public static int GetTotalTaxContribution(
            int propertyMetric,
            ActiveIncentive[] taxIncentives,
            ActiveViolationIncentive[] violationIncentives,
            string propertyType,
            int numberOfResidents,
            out double taxValue,
            out double propertyTypesAmount,
            out double incentivesAmount
        ) {
            // propertyMetric is the value before incentives
            taxValue = GetTaxContributionBase(propertyMetric);

            // property type amount
            double valueAfterPropertyType = propertyType.ToLower() switch {
                Private => taxValue,
                SharedPrivate => (taxValue * (1 + (numberOfResidents / 10d))),
                Owned => (taxValue * 1.3),
                _ => throw new ArgumentException($"Property type was not a valid string '{propertyType}'")
            };
            propertyTypesAmount = valueAfterPropertyType - taxValue;

            // apply tax incentives
            double taxIncentivesValue = GetTotalIncentivesValue(taxIncentives);
            double valueAfterTaxIncentives = (1 + taxIncentivesValue) * valueAfterPropertyType;

            // apply violation incentives
            double violationIncentivesValue = GetTotalIncentivesValue(
                violationIncentives
                .Where(incentive => incentive.ViolationType == ViolationIncentive.ViolationTypes.Recur)
                .ToArray()
            );
            double valueAfterViolationIncentives = valueAfterTaxIncentives + violationIncentivesValue;
            incentivesAmount = valueAfterViolationIncentives - valueAfterPropertyType;

            // final
            return (int)Math.Ceiling(valueAfterViolationIncentives);
        }

        #endregion
    }
}
