using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedDefaultProperties {

        private static void ValidateObjectType(DependencyObject obj) {
            if (obj is not ISwitchManagedDefault) {
                throw new ArgumentException($"Type {obj.GetType()} is not a valid type for use with SwitchManagedProperties");
            }
        }

        public static void SetDefaultValue(DependencyObject obj, object value) {
            ValidateObjectType(obj);
            obj.SetValue(DefaultValueProperty, value);
        }

        public static object GetDefaultValue(DependencyObject obj) {
            ValidateObjectType(obj);
            return (object)obj.GetValue(DefaultValueProperty);
        }

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.RegisterAttached(
            "DefaultValue",
            typeof(object),
            typeof(SwitchManagedProperties),
            new PropertyMetadata(null, OnDefaultValueChanged)
        );

        private static void OnDefaultValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ISwitchManagedDefault managed) {
                managed.DefaultValue = args.NewValue;
            }
        }
    }
}
