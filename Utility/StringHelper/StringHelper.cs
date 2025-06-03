using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MC_BSR_S2_Calculator.Utility.StringHelper {
    public class StringHelper {

        // - Capitalize Words -
        public static string CapitalizeWords(string str) {
            char[] newStr = str.ToCharArray();
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
    }
}
