using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.Identification {
    /// <summary>
    /// dictionary key conversion help
    /// </summary>
    public class IDTypeConverter<T> : TypeConverter
        where T : ID, new() {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
            if (value is string s) {
                return new T { Identifier = s };
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
            if (destinationType == typeof(string) && value is ID id) {
                return id.Identifier;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
