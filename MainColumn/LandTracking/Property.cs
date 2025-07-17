using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Validations;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Property : IDDisplayable, IIDHolder {

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

        // - Owner Player ID -

        [JsonProperty("owner")]
        public IDTrace? OwnerID { get; private set; }

        [DisplayValue("Owner", 100, GridUnitType.Pixel, displayOrder:1)]
        public BoundDisplayValue<Label, string> OwnerName { get; private set; }

        // - Property Name -

        [JsonProperty("name")]
        [DisplayValue("Name", 1, GridUnitType.Star, displayOrder:0)]
        public BoundDisplayValue<Label, string> Name { get; private set; }

        // - Property Type -

        [JsonProperty("type")]
        public string PropertyType {
            get => field;
            set {
                string valueLower = value.ToLower();
                if (!AllPropertyTypes.Contains(valueLower)) {
                    throw new ArgumentException($"Value for {nameof(PropertyType)}, '{valueLower}', is not a valid property type");
                }
                field = valueLower;
            }
        }

        // - Number of Residents -

        [JsonProperty("residents")]
        public int ResidentsCount {
            get => field;
            set {
                if (value < 1) {
                    throw new ArgumentException($"Value for {nameof(ResidentsCount)}, '{value}', must be positive and at least 1");
                }
                field = value;
            }
        }

        // - Property Subsections -

        /// <summary>
        /// WARNING: this isn't validated unless it's placed into UI via PropertyManager
        /// </summary>
        [JsonProperty("subsections")]
        public ImmutableList<PropertySubsection> Subsections { get; set; }

        // - Tax Incentives -

        [JsonProperty("taxes")]
        public List<ActiveIncentive> TaxIncentives { get; private set; }

        // - Purchase Incentives -

        [JsonProperty("purchases")]
        public List<ActiveIncentive> PurchaseIncentives { get; private set; }

        // - Violation Incentives -

        [JsonProperty("violations")]
        public List<ActiveViolationIncentive> ViolationIncentives { get; private set; }

        // - Subsurface Land Provision -

        [JsonProperty("subsurface_level")]
        public int? SubsurfaceLandProvisionLevel {
            get => field;
            set {
                if (
                    (value is not null)
                    && (
                        (value >= LandDefinitions.SurfaceLandareaYLevelMax)
                        || (value < LandDefinitions.WorldDepth)
                    )
                ) {
                    throw new ArgumentException($"Value for {nameof(SubsurfaceLandProvisionLevel)}, '{value}', was not in the bounds of allowed subsurface land provision levels");
                }
                field = value;
            }
        }

        // - Has Mailbox - 

        [JsonProperty("mailbox")]
        public bool HasMailbox { get; set; }

        // - Follows Property Metric Guidelines -

        [JsonProperty("guidelines")]
        public bool FollowsPropertyMetricGuidelines { get; set; }

        // - Approved -

        [JsonProperty("approved")]
        public bool IsApproved { get; set; }

        #endregion

        // --- CONSTRUCTOR ---

        #region CONSTRUCTOR

        private void SetDefaultValues(
            IDTrace? ownerID,
            string name,
            string propertyType,
            int residentsCount,
            PropertySubsection[] subsections,
            ActiveIncentive[] taxIncentives,
            ActiveIncentive[] purchaseIncentives,
            ActiveViolationIncentive[] violationIncentives,
            int? subsurfaceLandProvisionLevel,
            bool hasMailbox,
            bool followsPropertyMetricGuidelines,
            bool isApproved
        ) {
            // owner id
            OwnerID = ownerID;
            OwnerName = new(
                new Label(),
                Label.ContentProperty,
                string.Empty
            );
            UpdatePropertyDisplay();

            // name
            Name = new(
                new Label(),
                Label.ContentProperty,
                name
            );

            // data sets
            PropertyType = propertyType;
            ResidentsCount = residentsCount;
            Subsections = subsections.ToImmutableList();
            TaxIncentives = taxIncentives.ToList();
            PurchaseIncentives = purchaseIncentives.ToList();
            ViolationIncentives = violationIncentives.ToList();
            SubsurfaceLandProvisionLevel = subsurfaceLandProvisionLevel;
            HasMailbox = hasMailbox;
            FollowsPropertyMetricGuidelines = followsPropertyMetricGuidelines;
            IsApproved = isApproved;
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        #pragma warning disable CS9264 // Non-nullable property

        /// <summary>
        /// JSON entry only; DO NOT USE
        /// </summary>
        public Property() {
            SetDefaultValues(
                null,
                string.Empty,
                Public,
                1,
                Array.Empty<PropertySubsection>(),
                Array.Empty<ActiveIncentive>(),
                Array.Empty<ActiveIncentive>(),
                Array.Empty<ActiveViolationIncentive>(),
                null,
                false,
                false,
                false
            );
        }

        /// <summary>
        /// Use this constructor only
        /// </summary>
        public Property(
            IDTrace ownerID,
            string name,
            string propertyType,
            int residentsCount,
            PropertySubsection[] subsections,
            ActiveIncentive[] taxIncentives,
            ActiveIncentive[] purchaseIncentives,
            ActiveViolationIncentive[] violationIncentives,
            int? subsurfaceLandProvisionLevel,
            bool hasMailbox,
            bool followsPropertyMetricGuidelines,
            bool isApproved
        ) {
            AssignNewID();
            SetDefaultValues(
                ownerID,
                name,
                propertyType,
                residentsCount,
                subsections,
                taxIncentives,
                purchaseIncentives,
                violationIncentives,
                subsurfaceLandProvisionLevel,
                hasMailbox,
                followsPropertyMetricGuidelines,
                isApproved
            );
        }

        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        #pragma warning restore CS9264 // Non-nullable property

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
            => UpdatePropertyDisplay();

        #endregion

        // --- METHODS ---

        // - Update PropertyList -

        public void UpdatePropertyDisplay() {
            OwnerName.Value = OwnerID?.GetParent<Player>().Name.Value ?? "Unknown";
        }

        // -- Property Metric and Results --

        // - Static -
        #region Static 

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

        // - Instance -
        #region Instance 

        // - Property Metric -

        public int GetPropertyMetric()
            => GetPropertyMetric(Subsections.ToArray());

        // - Property Size -

        public string GetPropertySize()
            => GetPropertySize(Subsections.ToArray());

        // - Incentives -

        public double GetTotalIncentivesValue(IncentiveTypes type)
            => GetTotalIncentivesValue(type switch {
                IncentiveTypes.Purchase => PurchaseIncentives.ToArray(),
                IncentiveTypes.Violation => ViolationIncentives.ToArray(),
                IncentiveTypes.Tax => TaxIncentives.ToArray(),
                _ => throw new ArgumentException($"Value provided for {nameof(type)}, '{type}' was not a valid IncentiveType")
            });

        // - Purchasing Pricing -

        public int GetPurcahseValueBase()
            => GetPurcahseValueBase(Subsections.ToArray());

        public int GetPurchaseValueFinal(
            out int purchaseValue,
            out int incentivesAmount
        ) => GetPurchaseValueFinal(
            Subsections.ToArray(),
            ViolationIncentives.ToArray(),
            PurchaseIncentives.ToArray(),
            out purchaseValue,
            out incentivesAmount
        );

        // - Tax Contribution -

        public double GetTaxContributionBase()
            => GetTaxContributionBase(Subsections.ToArray());

        public int GetTotalTaxContribution(
            out double taxValue,
            out double propertyTypesAmount,
            out double incentivesAmount
        ) => GetTotalTaxContribution(
            Subsections.ToArray(),
            TaxIncentives.ToArray(),
            ViolationIncentives.ToArray(),
            PropertyType,
            ResidentsCount,
            out taxValue,
            out propertyTypesAmount,
            out incentivesAmount
        );

        #endregion
    }
}
