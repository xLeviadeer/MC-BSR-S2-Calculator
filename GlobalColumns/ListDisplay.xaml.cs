using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.GlobalColumns {

    /// <summary>
    /// Contains data about grid composition and the list of data to display; 
    /// Generates grid from associated data;
    /// Default display structure for ListDisplays
    /// </summary>
    /// <typeparam name="T"> The class type of which data to display from </typeparam>
    internal abstract partial class ListDisplay<T> : UserControl
        where T : Displayable<T> {
        // --- VARIABLES ---
        #region VARIABLES

        // - Reference Class -

        public List<T> DataList { get; init; }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        /// <remarks>
        /// Extensions of this class MUST define which exact data to display (parameterless constructor)
        /// </remarks>
        /// <param name="dataList"> The data to be displayed </param>
        public ListDisplay(List<T> dataList) {
            DataList = dataList;
        }

        #endregion

        // --- METHODS ---
        #region MEHTODS

        #endregion
    }
}
