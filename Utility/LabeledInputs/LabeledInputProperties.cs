using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {

    /// <typeparam name="T"> T must be the type that LabeledInput T uses as T </typeparam>
    public class LabeledInputProperties<T>
        where T : FrameworkElement {

        // valid uses

        public static ImmutableArray<Type> ValidTypes { get; } = [
            typeof(CheckBoxDisplay)
        ];

        private static void ValidateObjectType(DependencyObject obj) {
            if (!ValidTypes.Contains(obj.GetType())) {
                throw new ArgumentException($"Type {obj.GetType()} is not a valid type for use with LabeledInputProperties");
            }
        }

        // - label text -

        public static void SetLabelText(DependencyObject obj, string value) {
            ValidateObjectType(obj);
            obj.SetValue(LabeledInput<T>.LabelTextProperty, value);
        }

        public static string GetLabelText(DependencyObject obj) {
            return (string)obj.GetValue(LabeledInput<T>.LabelTextProperty);
        }

        public static readonly DependencyProperty LabelTextProperty = LabeledInput<T>.LabelTextProperty;

        // - fluid proportion split index - 

        public static void SetFluidProportionsSplitIndex(DependencyObject obj, int value) {
            ValidateObjectType(obj);
            obj.SetValue(LabeledInput<T>.FluidProportionsSplitIndexProperty, value);
        }

        public static int GetFluidProportionsSplitIndex(DependencyObject obj) {
            return (int)obj.GetValue(LabeledInput<T>.FluidProportionsSplitIndexProperty);
        }

        public static readonly DependencyProperty FluidProportionsSplitIndexProperty = LabeledInput<T>.FluidProportionsSplitIndexProperty;

        // - layout mode - 

        public static void SetLayoutMode(DependencyObject obj, LabeledInput<T>.LabeledInputLayoutModes value) {
            ValidateObjectType(obj);
            obj.SetValue(LabeledInput<T>.LayoutModeProperty, value);
        }

        public static LabeledInput<T>.LabeledInputLayoutModes GetLayoutMode(DependencyObject obj) {
            return (LabeledInput<T>.LabeledInputLayoutModes)obj.GetValue(LabeledInput<T>.LayoutModeProperty);
        }

        public static readonly DependencyProperty LayoutModeProperty = LabeledInput<T>.LayoutModeProperty;
    }
}
