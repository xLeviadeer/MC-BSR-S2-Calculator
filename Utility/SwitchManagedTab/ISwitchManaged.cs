using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using Microsoft.Extensions.Logging;
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
                    Logging.SwitchManagement.LogInformation("- checkFor Validity (true)");
                    break;
                case TargetableProperties.TabContentsChanged:
                    correctReturnValue = false;
                    Logging.SwitchManagement.LogInformation("- checkFor TabContentsChanged (true)");
                    break;
                default:
                    correctReturnValue = false;
                    Logging.SwitchManagement.LogInformation("- checkFor Other (false)");
                    break;
            }

            // check immediate truths
            // - null
            // - primary values
            // - is switch managaged
            Logging.SwitchManagement.LogInformation("- checking if element is a framework element");
            if (
                (element is null) // null primary
                || (element is not FrameworkElement frameworkElement) // primary type
            ) {
                Logging.SwitchManagement.LogInformation("- returned: element wasn't a framework element");
                return correctReturnValue;
            }
            Logging.SwitchManagement.LogInformation($"- element was a framework element, '{frameworkElement.Name}'");

            // type specific checks
            Logging.SwitchManagement.LogInformation("- checking if framework element is switch managed");
            if (frameworkElement is ISwitchManaged switchManagedElement) {
                Logging.SwitchManagement.LogInformation("- framework element was switch managed, returning checkFor result");
                switch (checkFor) {
                    case TargetableProperties.Validity:
                        Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                        return correctReturnValue;
                    case TargetableProperties.TabContentsChanged:
                        if (switchManagedElement.RequiresReset) {
                            bool returnBool = switchManagedElement.TabContentsChanged;
                            Logging.SwitchManagement.LogInformation($"- returned: {returnBool}");
                            return returnBool;
                        }
                        Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                        return correctReturnValue;
                }
            }
            Logging.SwitchManagement.LogInformation("- framework element was not switch managed");

            // if it's not already correct, then check if this element has contents
            // if it has contents, recurse
            if (frameworkElement is ContentControl contentControl) {
                Logging.SwitchManagement.LogInformation("- recursing: framework element is a content control");
                return CheckBoolPropertyOn(contentControl.Content, checkFor);
            } else if (frameworkElement is ItemsControl itemsControl) {
                Logging.SwitchManagement.LogInformation("- framework element is an item control");
                if (itemsControl.Items.Count <= 0) {
                    Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                    return correctReturnValue;
                }

                foreach (object? item in itemsControl.Items) {
                    Logging.SwitchManagement.LogInformation("- recursing: on new item");
                    if (CheckBoolPropertyOn(item, checkFor) == !correctReturnValue) {
                        Logging.SwitchManagement.LogInformation($"- returned: {!correctReturnValue}");
                        return !correctReturnValue;
                    }
                }
                Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                return correctReturnValue;
            } else if (frameworkElement is Panel panel) {
                Logging.SwitchManagement.LogInformation("- framework element is a panel");
                if (panel.Children.Count <= 0) {
                    Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                    return correctReturnValue;
                }

                foreach (object? child in panel.Children) {
                    Logging.SwitchManagement.LogInformation("- recursing: on new child");
                    if (CheckBoolPropertyOn(child, checkFor) == !correctReturnValue) {
                        Logging.SwitchManagement.LogInformation($"- returned: {!correctReturnValue}");
                        return !correctReturnValue;
                    }
                }
                Logging.SwitchManagement.LogInformation($"- returned: {correctReturnValue}");
                return correctReturnValue;
            } else if (frameworkElement is Decorator decorator) {
                Logging.SwitchManagement.LogInformation("- recursing: framework element is a decorator");
                bool returnBool = CheckBoolPropertyOn(decorator.Child, checkFor);
                Logging.SwitchManagement.LogInformation($"returned: {returnBool}");
                return returnBool;
            }

            // if it has no contents, fail
            Logging.SwitchManagement.LogInformation($"returned: {!correctReturnValue}; there were no contents");
            return !correctReturnValue;
        }

        // -- Enforcement --

        /// <summary>
        /// Resets this class; sets this class back to it's default state
        /// </summary>
        public abstract void Reset();
    }
}
