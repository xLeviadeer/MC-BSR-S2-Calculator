using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedProperties : FrameworkElement {

        private static void ValidateObjectType(DependencyObject obj) {
            if (obj is not ISwitchManaged) {
                throw new ArgumentException($"Type {obj.GetType()} is not a valid type for use with SwitchManagedProperties");
            }
        }

        public static void SetRequiresReset(DependencyObject obj, bool value) {
            ValidateObjectType(obj);
            obj.SetValue(RequiresResetProperty, value);
        }

        public static bool GetRequiresReset(DependencyObject obj) {
            ValidateObjectType(obj);
            return (bool)obj.GetValue(RequiresResetProperty);
        }

        public static readonly DependencyProperty RequiresResetProperty = DependencyProperty.RegisterAttached(
            "RequiresReset",
            typeof(bool),
            typeof(SwitchManagedProperties),
            new PropertyMetadata(true, OnRequiresResetChanged)
        );

        private static void OnRequiresResetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ISwitchManaged managed) {
                managed.RequiresReset = (bool)args.NewValue;
            }
        }
    }
}
