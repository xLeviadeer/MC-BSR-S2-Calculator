using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.XamlConverters {
    public class XamlConverter {
        // -- Capitalize Words --
        #region Capitalize Words

        // - Capitalize Words -
        public static string CapitalizeWords(string str) {
            char[] newStr = str.ToCharArray();

            // if the string is only 1 letter
            if (str.Length == 1) {
                newStr[0] = char.ToUpper(str[0]);
            } else {
                // for all letters of the string
                for (int i = (str.Length - 2); i >= 0; i--) {
                    // -2 skips the first

                    // always capitalize last
                    if (i == 0) {
                        newStr[i] = char.ToUpper(str[i]);
                        break;
                    }

                    // find spaces and capitalize
                    if (str[i] == ' ') {
                        newStr[i + 1] = char.ToUpper(str[i + 1]);
                    }
                }
            }
            return string.Join("", newStr);
        }

        // - Capitalize Words List -
        public static string[] CapitalizeWords(string[] arr) {
            string[] newList = new string[arr.Length];
            foreach (string str in arr) {
                newList.Append(CapitalizeWords(str));
            }
            return newList;
        }

        // - Capitalize Words HashSet -
        public static ImmutableList<string> CapitalizeWords(ImmutableList<string> hashSet) {
            List<string> newHashSet = new();
            foreach (string str in hashSet) {
                newHashSet.Add(CapitalizeWords(str));
            }
            return newHashSet.ToImmutableList();
        }

        #endregion

        // -- Bool to Visibility --
        #region Bool to Visibility

        public static Visibility BoolToVisibility(bool value) {
            return (value ? Visibility.Visible : Visibility.Collapsed);
        }

        #endregion

        // -- Find Scroll Bar --
        #region Find Scroll bar

        /// <summary>
        /// Attempts to find a ScrollBar from a parent
        /// </summary>
        /// <param name="parent"> The object to search for a ScrollBar </param>
        /// <returns> A ScrollBar object </returns>
        public static ScrollBar? FindVerticalScrollBar(DependencyObject parent) {

            // get children amount
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            if (childrenCount == 0) { return null; }

            // for every child of the parent
            for (int i = 0; i < childrenCount; i++) {

                // check the child for a vertical scroll bar
                var child = VisualTreeHelper.GetChild(parent, i);
                if ((child is ScrollBar sb) && (sb.Orientation == Orientation.Vertical))
                    return sb;

                // if the scroll bar wasn't directly found, then try recursing to find it
                ScrollBar? scrollBar = FindVerticalScrollBar(child);
                if (scrollBar != null) {
                    return scrollBar;
                }

            }
            return null;
        }

        #endregion
    }
}
