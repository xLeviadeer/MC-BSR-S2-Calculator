using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MC_BSR_S2_Calculator.Utility.Json {

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public interface IStorable {

        // --- VARIABLES ---

        // - As IStorable -

        /// <summary>
        /// Returns a cast of this as IStorable for function references
        /// </summary>
        /// <remarks>
        /// get => ((IStorable)this);
        /// </remarks>
        public IStorable AsIStorable { get; }

        // - Instance Tracking -

        public static List<IStorable> Instances = new();

        private static bool HasAlreadyBeenStored = false;
        private static bool HasAlreadyBeenRetrieved = false;

        // - Pathing -

        private List<string> ExtensionPath { get => new List<string>() { "Data" }.Concat(SaveLocation).ToList(); }
        
        /// <summary>
        /// A path to the save location for this ListDisplay
        /// </summary>
        public List<string> SaveLocation { get; }

        // --- SAVE MANAGEMNT ---

        /// <summary>
        /// MUST add the instance to the Instances list or instances will NOT be saved;
        /// MUST be run in the constructor or instances will NOT be saved
        /// </summary>
        /// <remarks>
        /// public void AddInstanceToInstances() => IStorable.Instances.Add(this);
        /// </remarks>
        public void AddInstanceToInstances();

        // - Load/Save All -

        /// <summary>
        /// Saves all stored instances
        /// </summary>
        public static void SaveAll(object? sender, EventArgs args) {
            if (!HasAlreadyBeenStored) {
                foreach (var instance in Instances) {
                    instance.Save();
                }

                HasAlreadyBeenStored = true;
            }
        }

        /// <summary>
        /// Loads all stored instances
        /// </summary>
        public static void LoadAll(object? sender, EventArgs args) {
            if (!HasAlreadyBeenRetrieved) {
                foreach (var instance in Instances) {
                    instance.LoadAs();
                }
            }
        }

        // - Storing -

        /// <summary>
        /// Saves the contents of this class to it's respective Data location
        /// </summary>
        public void Save() {
            // write
            JsonHandler.Write(ExtensionPath, this);
        }

        // - Retrieving -

        /// <summary>
        /// Function expected to mirror values from the read class to the existing class
        /// </summary>
        /// <typeparam name="T"> A class which extends IStorable </typeparam>
        /// <param name="cls"> An instance of the read class </param>
        public void MirrorValues<T>(T cls)
            where T : class, IStorable;

        /// <summary>
        /// Expected to call retrieve as the class it inherits from
        /// </summary>
        /// <remarks>
        /// AsIStorable.Retrieve&lt;T&gt;();
        /// </remarks>
        public void LoadAs();

        /// <summary>
        /// Tries to load json but sets to backup values upon failiure
        /// </summary>
        /// <typeparam name="T"> The type of object to retrieve </typeparam>
        /// <param name="backupObject"> A set of made-up (usually empty/new class) values to set to on failiure </param>
        public void TryLoad<T>(T backupObject)
            where T : class, IStorable {
            try {
                LoadAs();
            } catch (FileNotFoundException) {
                MirrorValues(backupObject);
            }
        }

        /// <summary>
        /// Loads the contents of this class from it's respective Data location
        /// </summary>
        /// <typeparam name="T"> The type of object to retrieve </typeparam>
        public void Load<T>()
            where T : class, IStorable {
            T cls;
            try {
                cls = JsonHandler.Read<T>(ExtensionPath);
            } catch (FileNotFoundException) {
                throw;
            }

            // mirror values
            MirrorValues(cls);
        }
    }
}
