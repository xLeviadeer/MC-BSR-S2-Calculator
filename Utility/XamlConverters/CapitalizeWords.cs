using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MC_BSR_S2_Calculator.Utility.XamlConverters {
    public class CapitalizeWords : MarkupExtension {
        public string? Str { get; set; }
        public string[]? Arr { get; set; }
        public ImmutableList<string>? List { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            // based on list or str
            if (Str != null) {
                return XamlConverter.CapitalizeWords(Str);
            } else if (
                (Arr != null)
                && (Arr.Length > 0)
            ) {
                return XamlConverter.CapitalizeWords(Arr);
            } else if (
                (List != null)
                && (List.Count > 0)
            ) {
                return XamlConverter.CapitalizeWords(List);
            } else {
                throw new ArgumentException("Str was not set");
            }
        }
    }
}
