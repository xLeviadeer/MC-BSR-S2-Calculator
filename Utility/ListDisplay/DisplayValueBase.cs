using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.ListDisplay {
    /// <summary>
    /// Attribute class for DisplayValue; used to mark values as to-be-displayed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DisplayValueAttribute : Attribute {

        // --- VARIABLES ---
        #region VARIABLES

        // - DisplayLayers -

        public List<int> DisplayLayers { get; init; } = new();

        // - DisplayName - 

        public string DisplayName { get; init; }

        // - Column Width -

        public GridLength ColumnWidth { get; init; }

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

        // - Display Order -

        public int DisplayOrder { get; init; }

        // - Hit Testing -

        public bool IsHitTestVisible { get; init; }

        #endregion

        // --- CONSTRUCTORS --
        #region CONSTRUCTORS

        /// <summary>
        /// Singular display layer
        /// </summary>
        /// <param name="displayLayer"> The layer to be displayed on </param>
        public DisplayValueAttribute(
            string displayName, 
            double columnWidth,
            GridUnitType columnWidthUnit,
            int displayLayer=-1,
            int displayOrder=0,
            bool isHitTestVisible=false
        ) {
            // set name
            DisplayName = displayName;

            // set layers
            DisplayLayers.Add(displayLayer);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = new GridLength(columnWidth, columnWidthUnit);
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // default content alignmet
            ColumnContentAlignment = new(HorizontalAlignment.Left, VerticalAlignment.Center);

            // display order
            DisplayOrder = displayOrder;

            // hit testing
            IsHitTestVisible = isHitTestVisible;
        }

        /// <summary>
        /// Array of display layers to be added to
        /// </summary>
        /// <param name="displayLayers"> An array of layers to be displayed on </param>
        public DisplayValueAttribute(
            string displayName,
            double columnWidth,
            GridUnitType columnWidthUnit,
            int[] displayLayers,
            int displayOrder=0,
            bool isHitTestVisible=false
        ) {
            // set name
            DisplayName = displayName;

            if (displayLayers.Contains(-1) && displayLayers.Length > 1) { throw new ArgumentException($"A property or field cannot be displayed on both all layers (-1) and only some layers at once"); }
            if (displayLayers.Any(layerNumber => layerNumber < -1)) { throw new ArgumentOutOfRangeException($"The given layer number was out of range; layers cannot be negative"); }
            DisplayLayers.AddRange(displayLayers);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = new GridLength(columnWidth, columnWidthUnit);
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // default content alignmet
            ColumnContentAlignment = new(HorizontalAlignment.Left, VerticalAlignment.Center);

            // display order
            DisplayOrder = displayOrder;

            // hit testing
            IsHitTestVisible = isHitTestVisible;
        }

        /// <summary>
        /// Singular display layer
        /// </summary>
        /// <param name="displayLayer"> The layer to be displayed on </param>
        public DisplayValueAttribute(
            string displayName,
            double columnWidth,
            GridUnitType columnWidthUnit,
            HorizontalAlignment columnContentHorizontalAlignment, 
            VerticalAlignment columnContentVerticalAlignment, 
            int displayLayer=-1,
            int displayOrder=0,
            bool isHitTestVisible=false
        ) {
            // set name
            DisplayName = displayName;

            // set layers
            DisplayLayers.Add(displayLayer);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = new GridLength(columnWidth, columnWidthUnit);
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // content alignment
            ColumnContentAlignment = new(columnContentHorizontalAlignment, columnContentVerticalAlignment);

            // display order
            DisplayOrder = displayOrder;

            // hit testing
            IsHitTestVisible = isHitTestVisible;
        }

        /// <summary>
        /// Array of display layers to be added to
        /// </summary>
        /// <param name="displayLayers"> An array of layers to be displayed on </param>
        public DisplayValueAttribute(
            string displayName,
            double columnWidth,
            GridUnitType columnWidthUnit,
            HorizontalAlignment columnContentHorizontalAlignment, 
            VerticalAlignment columnContentVerticalAlignment, 
            int[] displayLayers,
            int displayOrder=0,
            bool isHitTestVisible=false
        ) {
            // set name
            DisplayName = displayName;

            if (displayLayers.Contains(-1) && displayLayers.Length > 1) { throw new ArgumentException($"A property or field cannot be displayed on both all layers (-1) and only some layers at once"); }
            if (displayLayers.Any(layerNumber => layerNumber < -1)) { throw new ArgumentOutOfRangeException($"The given layer number was out of range; layers cannot be negative"); }
            DisplayLayers.AddRange(displayLayers);

            // column width
            if (columnWidth >= 0) {
                ColumnWidth = new GridLength(columnWidth, columnWidthUnit);
            } else {
                throw new ArgumentException($"the provided columnWidth was less than 0");
            }

            // content alignment
            ColumnContentAlignment = new(columnContentHorizontalAlignment, columnContentVerticalAlignment);

            // display order
            DisplayOrder = displayOrder;

            // hit testing
            IsHitTestVisible = isHitTestVisible;
        }

        #endregion
    }

    /// <summary>
    /// Display value interface; requirss that DisplayValue extensions must have a DisplayObject
    /// </summary>
    public abstract class DisplayValueBase : OptionalMouseClickEventHolder {
        
        // --- VARIABLES ---
        
        // - DisplayObject -

        public abstract FrameworkElement DisplayObject { get; }

        // - HeldLeftClickListener -

        private EventHandler<EventArgs>? CellEvent { get; set; }
        public new void HeldLeftClickListener(object? sender, EventArgs args) {
            if (CellEvent == null) {
                base.HeldLeftClickListener(sender, args);
                return;
            }
            CellEvent.Invoke(sender, args);
        }

        // --- CONSTRUCTOR ---

        public DisplayValueBase(EventHandler<EventArgs>? eventListener=null, ContextMenu? rightClickMenu=null) {
            // set listener
            CellEvent = eventListener;

            // set context menu
            if (rightClickMenu != null) {
                RightClickMenu = rightClickMenu;
            }

            // set override
            IsHoldingLeftClickOverride = (eventListener != null);
        }
    }
}
