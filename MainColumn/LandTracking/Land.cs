using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public class Land : ILandArea {

        // --- VARIABLES ---
        #region VARIABLES

        // - As ILandArea -

        public ILandArea AsILandArea => (ILandArea)this;

        // - Default Bottom/Top -

        public int DefaultBottom { get; } = ILandArea.SurfaceLandareaYLevelMin;

        public int DefaultTop { get; } = ILandArea.WORLD_HEIGHT;

        // - Bounds -

        public List<ICoordinateBoundAmbiguous> Bounds { get; private set; } = new();

        // - Name -

        public static List<string> ExistingNames { get; } = [];

        [JsonProperty("name")]
        public string Name {
            get => field;
            private set {
                string trimmedName = value.Trim();
                if (ExistingNames.Contains(trimmedName)) {
                    throw new ArgumentException($"Name, '{trimmedName}' for Land already exists");
                }
                field = trimmedName;
                ExistingNames.Add(value);
            }
        }

        // - Owner ID -

        public IDTrace? OwnerID { get; private set; }

        // - Land Type -

        public string LandType {
            get => field;
            private set {
                string valueLower = value.ToLower();
                if (!ILandArea.GovernmentalLandTypes.Contains(valueLower)) {
                    throw new ArgumentException($"Value for {nameof(LandType)}, '{valueLower}', is not a valid land type");
                }
                field = valueLower;
            }
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public Land() {
            // check validity of defaults
            AsILandArea.HasValidDefaults();
        }

        public Land(string name, IDTrace ownerID, List<ICoordinateBoundAmbiguous>? bounds=null) {
            // check validity of defaults
            AsILandArea.HasValidDefaults();

            // parameter sets
            Name = name;
            OwnerID = ownerID;
            if (bounds is not null) { Bounds = bounds; }
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        // - Get Squares/Cubes -

        public List<ICoordinateBoundAmbiguousReturn<FlatCoordinatePoint>> GetBoundsAsSquares()
            => AsILandArea.GetBoundsAsSquaresHelper();

        public List<ICoordinateBoundAmbiguousReturn<CoordinatePoint>> GetBoundsAsCubes()
            => AsILandArea.GetBoundsAsCubesHelper();

        #endregion

    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LandList : IStorable {

        [JsonProperty("data")]
        public NotifyingList<Land> All { get; private set; } = new();

        public IStorable AsIStorable => (IStorable)this;

        public JsonSerializerSettings? StorageSettings { get; } = new() {
            TypeNameHandling = TypeNameHandling.Auto, // needed for saving interfaces
            SerializationBinder = new SimplisticTypeNameBinder()
        };

        public List<string> SaveLocation => ["lands.json"];

        private static bool _hasLoaded { get; set; } = false;

        public LandList() {
            if (_hasLoaded) { return; }
            _hasLoaded = true;

            // try to load
            AsIStorable.TryLoad(new LandList());

            // storable requirement
            AddInstanceToInstances();

            // save event subscriptions
            All.ItemsChanged += (_, _) => AsIStorable.Save();
        }

        public void AddInstanceToInstances() => IStorable.Instances.Add((IStorable)this);

        public void LoadAs() => AsIStorable.Load<LandList>();

        public void MirrorValues<U>(U cls)
            where U : class, IStorable {
            if (cls is LandList clsCasted) {
                All = clsCasted.All;
            }
        }
    }

    /// <summary>
    /// Serializer for land based objects to simplfy type indication
    /// </summary>
    public class SimplisticTypeNameBinder : ISerializationBinder {

        // type cache
        private readonly Dictionary<string, Type> _typeCache = new();

        public void BindToName(Type serializedType, out string assemblyName, out string typeName) {
            // send assembly name and type name as empty aside from just class name
            assemblyName = string.Empty;
            typeName = serializedType.Name;

            // cache so it doesn't need to be resolved later using reflection
            _typeCache[typeName] = serializedType;
        }

        public Type BindToType(string? assemblyName, string typeName) {
            // try to find in cache
            if (_typeCache.TryGetValue(typeName, out Type? type)) {
                return type;
            }

            // use reflection if not found
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .First(type => type.Name == typeName);
        }
    }
}
