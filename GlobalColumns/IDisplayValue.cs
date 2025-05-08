using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.GlobalColumns {
    /// <summary>
    /// Attribute class for DisplayValue; used to mark values as to-be-displayed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    internal class DisplayValueAttribute : Attribute {

        // --- VARIABLES ---
        #region VARIABLES

        // - DisplayLayers -

        public List<int> DisplayLayers { get; init; } = new();

        // - DisplayName - 

        public string DisplayName { get; init; }

        #endregion

        // --- CONSTRUCTORS --
        #region CONSTRUCTORS

        /// <summary>
        /// Assums attribute should be on all layers
        /// </summary>
        public DisplayValueAttribute(string displayName) {
            // set name
            DisplayName = displayName;

            // set to all
            DisplayLayers.Append(-1);
        }

        /// <summary>
        /// Singular display layer
        /// </summary>
        /// <param name="displayLayer"> The layer to be displayed on </param>
        public DisplayValueAttribute(string displayName, int displayLayer) {
            // set name
            DisplayName = displayName;

            // set layers
            DisplayLayers.Append(displayLayer);
        }

        /// <summary>
        /// Array of display layers to be added to
        /// </summary>
        /// <param name="displayLayers"> An array of layers to be displayed on </param>
        public DisplayValueAttribute(string displayName, int[] displayLayers) {
            // set name
            DisplayName = displayName;

            if (displayLayers.Contains(-1) && displayLayers.Length > 1) { throw new ArgumentException($"A property or field cannot be displayed on both all layers (-1) and only some layers at once"); }
            if (displayLayers.Any(layerNumber => (layerNumber < -1))) { throw new ArgumentOutOfRangeException($"The given layer number was out of range; layers cannot be negative"); }
            DisplayLayers.AddRange(displayLayers);
        }

        #endregion
    }

    /// <summary>
    /// Display value interface; requirss that DisplayValue extensions must have a DisplayObject
    /// </summary>
    internal interface IDisplayValue {
        public FrameworkElement DisplayObject { get; }
    }
}
