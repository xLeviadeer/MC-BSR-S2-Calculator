using MC_BSR_S2_Calculator.GlobalColumns.DisplayList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    /// <summary>
    /// Contains data about grid composition and the list of data to display; 
    /// Generates grid from associated data;
    /// Default display structure for ListDisplays
    /// </summary>
    /// <typeparam name="T"> The class type of which data to display from </typeparam>
    internal abstract partial class ListDisplay<T> : UserControl
        where T : Displayable {

        // --- VARIABLES ---
        #region VARIABLES

        // -- Data Holders --

        // - Data List -

        /// <summary>
        /// Holds a list of classes to display data from
        /// </summary>
        public List<T> ClassDataList { get; set; }

        // - Data List by Rows -

        /// <summary>
        /// A list of IDisplayValues organized by row
        /// </summary>
        public List<Dictionary<string, IDisplayValue>> DataListByRows {
            get {
                if (ClassDataList == null) { return new(); }

                return ClassDataList.Select(
                    cls => cls.DisplayValues
                ).ToList();
            }
        }

        // - Data List by Columns -

        /// <summary>
        /// A list of IDisaplayValues organized by columns
        /// </summary>
        public Dictionary<string, List<IDisplayValue>> DataListByColumns {
            get {
                if (ClassDataList == null) { return new(); }

                return ClassDataList[0].DisplayHeaders.ToDictionary(
                    key => key, // key
                    key => ClassDataList.Select( // value
                        cls => cls.DisplayValues[key]
                    ).ToList()
                );
            }
        }

        // - List Headers -

        /// <summary>
        /// Gets the headers for this list
        /// </summary>
        public ImmutableList<string> Headers {
            get {
                if (ClassDataList == null) { return ImmutableList<string>.Empty; }

                if (ClassDataList.Count > 0) {
                    return ClassDataList[0].DisplayHeaders.ToImmutableList();
                } else {
                    return ImmutableList<string>.Empty;
                }
            }
        }

        // - Column Widths -

        private ImmutableDictionary<string, int> ColumnWidths {
            get {
                if (ClassDataList == null) { return ImmutableDictionary<string, int>.Empty; }

                return ClassDataList[0].DisplayHeaders.ToImmutableDictionary(
                    key => key, // key
                    key => ClassDataList[0].ColumnWidths[key]
                );
            }
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        /// <remarks>
        /// Extensions of this class MUST define which exact data to display (parameterless constructor)
        /// </remarks>
        /// <param name="classDataList"> The data to be displayed </param>
        public ListDisplay() {
            // loads data list and builds grid
            Loaded += (object sender, RoutedEventArgs args) => {
                SetClassDataList();
                BuildGrid();
            };

            // .Interface constructor
            Interface_Constructor();
        }

        /// <remarks>
        /// this method MUST set ClassDataList
        /// </remarks>
        protected abstract void SetClassDataList();

        #endregion
    }
}
