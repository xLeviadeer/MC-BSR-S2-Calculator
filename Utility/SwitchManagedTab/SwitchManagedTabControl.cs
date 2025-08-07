using MC_BSR_S2_Calculator.Utility.ConfirmationWindows;
using MC_BSR_S2_Calculator.Utility.Validations;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.Utility.SwitchManagedTab {

    /// <summary>
    /// Ensures all items of the control are Switch Managed
    /// </summary>
    public class SwitchManagedTabControl : TabControl, IValidatable {

        // --- VARIABLES ---

        // - Validity -

        public bool? IsValid { get; set; }

        // --- CONSTRUCTOR ---

        public SwitchManagedTabControl() {
            // subscribe tab changed event logic
            PreviewMouseDown += (_, args) => {
                // find source
                // assumes relevant parent is 3 upwards
                DependencyObject source = (DependencyObject)args.OriginalSource;
                source = VisualTreeHelper.GetParent(source);
                source = VisualTreeHelper.GetParent(source);
                source = VisualTreeHelper.GetParent(source);

                // don't show if not an other tab header click
                if (
                    (source is not SwitchManagedTabItem clickedTab) // not a tab header click
                    || (SelectedItem == clickedTab) // already selected tab
                ) {
                    return;
                }

                // only confirm when changes have been made
                foreach (TabItem item in Items) {
                    if (item is SwitchManagedTabItem switchManagedTabItem) {
                        if (switchManagedTabItem.CheckForProperty(
                            SwitchManagedTabItem.SwitchManagedProperties.TabContentsChanged
                        ) == true) { // if any contents have changed
                            // confirm switch with dialog
                            if (ISwitchManaged.AskConfirmation()) {
                                // find the containing tabcontrol (assumes 3 up)
                                source = ((FrameworkElement)VisualTreeHelper.GetParent(source));
                                source = ((FrameworkElement)VisualTreeHelper.GetParent(source));
                                source = ((FrameworkElement)VisualTreeHelper.GetParent(source));
                                SwitchManagedTabControl containingTabControl = (SwitchManagedTabControl)source;

                                // switch tab
                                ((SwitchManagedTabItem)containingTabControl.SelectedItem).ResetContent();
                                containingTabControl.SelectedItem = clickedTab;
                            }
                            args.Handled = true;

                            // end loop
                            return;
                        }
                    }
                }
                // do nothing
            };
        }

        // --- METHODS ---

        // validates that all items are switch managed
        public void Validate(object? _=null, EventArgs? __=null) {
            foreach (TabItem item in Items) {
                if (item is not SwitchManagedTabItem) {
                    IsValid = false;
                    throw new InvalidOperationException($"Item, '{item.Name}' is not an SwitchManagedTabItem; all TabItems must be switch managed");
                }
            }
            IsValid = true;
        }

        protected override void OnInitialized(EventArgs args) {
            base.OnInitialized(args);
            Validate();
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs args) {
            base.OnItemsChanged(args);
            Validate();
        }
    }
}
