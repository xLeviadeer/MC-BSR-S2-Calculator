using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.DisplayList {
    /// <summary>
    /// Attribute class for DisplayValue; used to mark values as to-be-displayed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class DisplayValueAttribute : Attribute {

        // --- VARIABLES ---
        #region VARIABLES

        // - DisplayLayers -

        public List<int> DisplayLayers { get; init; } = new();

        // - DisplayName - 

        public string DisplayName { get; init; }

        // - Column Width -

        public int ColumnWidth { get; init; }

        // - Column Alignment -

        public record ColumnContentAlignmnetRecord {
            public VerticalAlignment VerticalAlignment { get; }
            public HorizontalAlignment HorizontalAlignment { get; }

            public ColumnContentAlignmnetRecord(HorizontalAlignment horizontalAlignment, VerticalAlignment vertialAlignment) {
                HorizontalAlignment = horizontalAlignment;
                VerticalAlignment = vertialAlignment;
            }
        }

        public ColumnContentAlignmnetRecord ColumnContentAlignment { get; init; }

        #endregion

        // --- CONSTRUCTORS --
        #region CONSTRUCTORS

        /// <summary>
        /// Singular display layer
        /// </summary>
        /// <param name="displayLayer"> The layer to be displayed on </param>
        public DisplayValueAttribute(string displayName, int columnWidth, int displayLayer=-1) {
            // set name
            DisplayName = displayName;

            // set layers
            DisplayLayers.Add(displayLayer);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = columnWidth + 1;
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // default content alignmet
            ColumnContentAlignment = new(HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        /// <summary>
        /// Array of display layers to be added to
        /// </summary>
        /// <param name="displayLayers"> An array of layers to be displayed on </param>
        public DisplayValueAttribute(string displayName, int columnWidth, int[] displayLayers) {
            // set name
            DisplayName = displayName;

            if (displayLayers.Contains(-1) && displayLayers.Length > 1) { throw new ArgumentException($"A property or field cannot be displayed on both all layers (-1) and only some layers at once"); }
            if (displayLayers.Any(layerNumber => layerNumber < -1)) { throw new ArgumentOutOfRangeException($"The given layer number was out of range; layers cannot be negative"); }
            DisplayLayers.AddRange(displayLayers);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = columnWidth + 1;
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // default content alignmet
            ColumnContentAlignment = new(HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        /// <summary>
        /// Singular display layer
        /// </summary>
        /// <param name="displayLayer"> The layer to be displayed on </param>
        public DisplayValueAttribute(string displayName, int columnWidth, HorizontalAlignment columnContentHorizontalAlignment, VerticalAlignment columnContentVerticalAlignment, int displayLayer=-1) {
            // set name
            DisplayName = displayName;

            // set layers
            DisplayLayers.Add(displayLayer);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = columnWidth + 1;
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // content alignment
            ColumnContentAlignment = new(columnContentHorizontalAlignment, columnContentVerticalAlignment);
        }

        /// <summary>
        /// Array of display layers to be added to
        /// </summary>
        /// <param name="displayLayers"> An array of layers to be displayed on </param>
        public DisplayValueAttribute(string displayName, int columnWidth, HorizontalAlignment columnContentHorizontalAlignment, VerticalAlignment columnContentVerticalAlignment, int[] displayLayers) {
            // set name
            DisplayName = displayName;

            if (displayLayers.Contains(-1) && displayLayers.Length > 1) { throw new ArgumentException($"A property or field cannot be displayed on both all layers (-1) and only some layers at once"); }
            if (displayLayers.Any(layerNumber => layerNumber < -1)) { throw new ArgumentOutOfRangeException($"The given layer number was out of range; layers cannot be negative"); }
            DisplayLayers.AddRange(displayLayers);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = columnWidth + 1;
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // content alignment
            ColumnContentAlignment = new(columnContentHorizontalAlignment, columnContentVerticalAlignment);
        }

        #endregion
    }

    /// <summary>
    /// Display value interface; requirss that DisplayValue extensions must have a DisplayObject
    /// </summary>
    internal abstract class DisplayValueBase : OptionalEventHolder {
        
        // --- VARIABLES ---
        
        // - DisplayObject -

        public abstract FrameworkElement DisplayObject { get; }

        // - HeldListener -

        private EventHandler<EventArgs>? CellEvent { get; set; }
        public new void HeldListener(object? sender, EventArgs args) {
            if (CellEvent == null) {
                base.HeldListener(sender, args);
                return;
            }
            CellEvent.Invoke(sender, args);
        }

        // --- CONSTRUCTOR ---

        public DisplayValueBase(EventHandler<EventArgs>? eventListener=null) {
            // set listener
            CellEvent = eventListener;

            // set override
            IsHoldingEventOverride = (eventListener != null);
        }
    }
}
