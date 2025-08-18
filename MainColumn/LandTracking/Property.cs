using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.Validations;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
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
    public class Property : IDDisplayable, IIDHolder, ILandArea {

        // --- VARIABLES ---

        // -- STATIC --

        // - Property Sizes -
        #region Property Sizes

        public static class PropertySize {
            // bounds
            public const int SMALL_UPPER_BOUND = 400;
            public const int LARGE_UPPER_BOUND = 20000;

            // names
            public const string INVALID = "-";
            public const string SMALL = "Small";
            public const string LARGE = "Large";
            public const string MASSIVE = "Massive";
        }

        #endregion

        // -- INSTANCE --
        #region INSTANCE

        // - As ILandArea -

        public ILandArea AsILandArea => (ILandArea)this;

        // - Default Bottom/Top -

        public int DefaultBottom { get; private set; } = ILandArea.WORLD_HEIGHT;

        public int DefaultTop => (SubsurfaceLandProvisionLevel is null)
            ? ILandArea.SurfaceLandareaYLevelMin
            : (int)SubsurfaceLandProvisionLevel;

        // - ID -

        public static char TypeCharacter { get; } = 'o';

        // - Owner Player ID -

        public IDTrace? OwnerID { get; private set; }

        [DisplayValue("Owner", 100, GridUnitType.Pixel, displayOrder:1)]
        public BoundDisplayValue<Label, string> OwnerName { get; private set; }

        // - Property Name -

        [JsonProperty("name")]
        [DisplayValue("Name", 1, GridUnitType.Star, displayOrder:0)]
        public BoundDisplayValue<Label, string> Name { get; private set; }

        // - Property Type -

        public string LandType {
            get => field;
            set {
                string valueLower = value.ToLower();
                if (!ILandArea.PlayerLandTypes.Contains(valueLower)) {
                    throw new ArgumentException($"Value for {nameof(LandType)}, '{valueLower}', is not a valid land type");
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
        public List<ICoordinateBoundAmbiguous> Bounds { get; set; }

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
                        (value >= ILandArea.SurfaceLandareaYLevelMax)
                        || (value < ILandArea.WORLD_DEPTH)
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
            LandType = propertyType;
            ResidentsCount = residentsCount;
            Bounds = subsections.Cast<ICoordinateBoundAmbiguous>().ToList();
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
                ILandArea.PRIVATE,
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
            bool isApproved,
            IDPrimary? fromExistingID=null
        ) {
            // ID assignment
            if (fromExistingID is null) {
                AssignNewID();
            } else {
                DisplayableID = fromExistingID;
            }

            // defaults
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

        // copy constructor
        public Property(Property property) {
            DisplayableID = property.DisplayableID;
            SetDefaultValues(
                property.OwnerID,
                property.Name,
                property.LandType,
                property.ResidentsCount,
                property.Bounds.Cast<PropertySubsection>().ToArray(),
                property.TaxIncentives.ToArray(),
                property.PurchaseIncentives.ToArray(),
                property.ViolationIncentives.ToArray(),
                property.SubsurfaceLandProvisionLevel,
                property.HasMailbox,
                property.FollowsPropertyMetricGuidelines,
                property.IsApproved
            );
        }

        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        #pragma warning restore CS9264 // Non-nullable property

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context) {
            UpdatePropertyDisplay(); 
        }

        #endregion

        // --- METHODS ---

        // - Update PropertyList -

        public void UpdatePropertyDisplay()
            => OwnerName.Value = Player.GetPlayerNameFrom(OwnerID);

        // -- Property Metric and Results --

        // - Static -
        #region Static 

        // - Property Metric -

        public static int GetPropertyMetric(PropertySubsection[] subsections) {
            int total = 0;
            foreach (PropertySubsection subsection in subsections) {
                total += subsection.Area;
            }
            return total;
        }

        // - Property Size -

        public static string GetPropertySize(PropertySubsection[] subsections)
            => GetPropertySize(GetPropertyMetric(subsections));

        public static string GetPropertySize(int propertyMetric)
            => propertyMetric switch {
                (>= 0 and <= PropertySize.SMALL_UPPER_BOUND) => PropertySize.SMALL,
                (> PropertySize.SMALL_UPPER_BOUND and <= PropertySize.LARGE_UPPER_BOUND) => PropertySize.LARGE,
                (> PropertySize.LARGE_UPPER_BOUND) => PropertySize.MASSIVE,
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
                ILandArea.PRIVATE => taxValue,
                ILandArea.SHARED_PRIVATE => (taxValue * (1 + (numberOfResidents / 10d))),
                ILandArea.OWNED => (taxValue * 1.3),
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
            => GetPropertyMetric(Bounds.Cast<PropertySubsection>().ToArray());

        // - Property Size -

        public string GetPropertySize()
            => GetPropertySize(Bounds.Cast<PropertySubsection>().ToArray());

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
            => GetPurcahseValueBase(Bounds.Cast<PropertySubsection>().ToArray());

        public int GetPurchaseValueFinal(
            out int purchaseValue,
            out int incentivesAmount
        ) => GetPurchaseValueFinal(
            Bounds.Cast<PropertySubsection>().ToArray(),
            ViolationIncentives.ToArray(),
            PurchaseIncentives.ToArray(),
            out purchaseValue,
            out incentivesAmount
        );

        // - Tax Contribution -

        public double GetTaxContributionBase()
            => GetTaxContributionBase(Bounds.Cast<PropertySubsection>().ToArray());

        public int GetTotalTaxContribution(
            out double taxValue,
            out double propertyTypesAmount,
            out double incentivesAmount
        ) => GetTotalTaxContribution(
            Bounds.Cast<PropertySubsection>().ToArray(),
            TaxIncentives.ToArray(),
            ViolationIncentives.ToArray(),
            LandType,
            ResidentsCount,
            out taxValue,
            out propertyTypesAmount,
            out incentivesAmount
        );

        // - Get Squares/Cubes -

        public List<ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>> GetBoundsAsSquares()
            => AsILandArea.GetBoundsAsSquaresHelper();

        public List<ICoordinateBoundAmbiguousReturn<CoordinatePoint>> GetBoundsAsCubes()
            => AsILandArea.GetBoundsAsCubesHelper();

        // - Contains -

        public bool Contains(IFlatCoordinate coord)
            => AsILandArea.ContainsHelper(coord);

        #endregion
    }
}
