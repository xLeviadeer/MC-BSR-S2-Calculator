using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using MC_BSR_S2_Calculator.Utility.Validations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {
    public class SwitchManagedTabItem : TabItem, IValidatable, IValidityHolder {

        // --- VALIDATIONS ---

        protected override void OnInitialized(EventArgs args) {
            base.OnInitialized(args);
            Validate();
            if (CheckValidity() == false) {
                throw new InvalidOperationException($"All lowest contents of a SwitchManagedTabItem must extend ISwitchManaged");
            }
        }

        // -- SELF --

        public bool? IsValid { get; set; }

        public void Validate(object? _=null, EventArgs? __=null) {
            if (this.Parent is not SwitchManagedTabControl) {
                IsValid = false;
                throw new InvalidOperationException($"Parent, '{((FrameworkElement)this.Parent).Name}', is not a SwitchManagedTabItem; parents of SwitchManagedTabItems must be switch managed");
            }
            IsValid = true;
        }

        // -- CONTENTS --

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        public enum SwitchManagedProperties {
            Validity,
            TabContentsChanged,
            Reset
        }

        private bool CheckBoolPropertyOn(object? element, SwitchManagedProperties checkFor) {
            // determine boolean position of return value
            bool correctReturnValue;
            switch (checkFor) {
                case SwitchManagedProperties.Validity:
                    correctReturnValue = true;
                    break;
                case SwitchManagedProperties.TabContentsChanged:
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
                    case SwitchManagedProperties.Validity:
                        return correctReturnValue;
                    case SwitchManagedProperties.TabContentsChanged:
                        if (switchManagedElement.RequiresReset) {
                            return switchManagedElement.TabContentsChanged;
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
                return CheckBoolPropertyOn(decorator.Child, checkFor);
            }

            // if it has no contents, fail
            return !correctReturnValue;
        }

        public bool CheckValidity()
            => CheckBoolPropertyOn(this.Content, SwitchManagedProperties.Validity);

        public bool CheckForProperty(SwitchManagedProperties checkFor)
            => CheckBoolPropertyOn(this.Content, checkFor);

        /// <summary>
        /// Resets all ISwitchManaged content
        /// </summary>
        private void ResetContentOn(object? element) {
            // check immediate truths
            if (
                (element is null) // null primary
                || (element is not FrameworkElement frameworkElement) // primary type
            ) {
                return;
            }

            // if its switch managed
            if (frameworkElement is ISwitchManaged switchManagedElement) {
                if (switchManagedElement.RequiresReset) {
                    switchManagedElement.Reset();
                    return; // don't reset deeper
                }
            }

            // if it's not already correct, then check if this element has contents
            // if it has contents, recurse
            if (frameworkElement is ContentControl contentControl) {
                ResetContentOn(contentControl.Content);
            } else if (frameworkElement is ItemsControl itemsControl) {
                foreach (object? item in itemsControl.Items) {
                    ResetContentOn(item);
                }
            } else if (frameworkElement is Panel panel) {
                foreach (object? child in panel.Children) {
                    ResetContentOn(child);
                }
            } else if (frameworkElement is Decorator decorator) {
                ResetContentOn(decorator.Child);
            }
        }

        public void ResetContent()
            => ResetContentOn(this.Content);
    }
}
