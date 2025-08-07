using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public interface ISwitchManaged {
        // --- VARIABLES ---

        // -- Resources --

        public enum TargetableProperties {
            Validity,
            TabContentsChanged
        }

        public static bool AskConfirmation()
            => (new ConfirmationWindow(
                "Tab contents will not be saved",
                "exit without saving",
                "stay here"
            ).ShowDialog() == true);

        // -- Enforcement --

        /// <summary>
        /// Tracks if changes were made to this class and hence require it to be reset when tabbing off
        /// </summary>
        public bool TabContentsChanged { get; }

        /// <summary>
        /// Determines whether tabbing off will require the tab to be reset or if it will save it's contents
        /// </summary>
        public bool RequiresReset { get; set; }

        // -- METHODS --

        // -- Resources --

        public static bool CheckBoolPropertyOn(object? element, TargetableProperties checkFor) {
            // determine boolean position of return value
            bool correctReturnValue;
            switch (checkFor) {
                case TargetableProperties.Validity:
                    correctReturnValue = true;
                    break;
                case TargetableProperties.TabContentsChanged:
                    correctReturnValue = false;
                    break;
                default:
                    correctReturnValue = false;
                    break;
            }

            // check immediate truths
            // - null
            // - primary values
            // - is switch managaged
            if (
                (element is null) // null primary
                || (element is not FrameworkElement frameworkElement) // primary type
            ) {
                return correctReturnValue;
            }

            // type specific checks
            if (frameworkElement is ISwitchManaged switchManagedElement) {
                switch (checkFor) {
                    case TargetableProperties.Validity:
                        return correctReturnValue;
                    case TargetableProperties.TabContentsChanged:
                        if (switchManagedElement.RequiresReset) {
                            bool returnBool = switchManagedElement.TabContentsChanged;
                            return returnBool;
                        }
                        return correctReturnValue;
                }
            }

            // if it's not already correct, then check if this element has contents
            // if it has contents, recurse
            if (frameworkElement is ContentControl contentControl) {
                return CheckBoolPropertyOn(contentControl.Content, checkFor);
            } else if (frameworkElement is ItemsControl itemsControl) {
                if (itemsControl.Items.Count <= 0) {
                    return correctReturnValue;
                }

                foreach (object? item in itemsControl.Items) {
                    if (CheckBoolPropertyOn(item, checkFor) == !correctReturnValue) {
                        return !correctReturnValue;
                    }
                }
                return correctReturnValue;
            } else if (frameworkElement is Panel panel) {
                if (panel.Children.Count <= 0) {
                    return correctReturnValue;
                }

                foreach (object? child in panel.Children) {
                    if (CheckBoolPropertyOn(child, checkFor) == !correctReturnValue) {
                        return !correctReturnValue;
                    }
                }
                return correctReturnValue;
            } else if (frameworkElement is Decorator decorator) {
                bool returnBool = CheckBoolPropertyOn(decorator.Child, checkFor);
                return returnBool;
            }

            // if it has no contents, fail
            return !correctReturnValue;
        }

        // -- Enforcement --

        /// <summary>
        /// Resets this class; sets this class back to it's default state
        /// </summary>
        public abstract void Reset();
    }
}
