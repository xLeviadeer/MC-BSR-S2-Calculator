using MC_BSR_S2_Calculator.MainColumn;
using MC_BSR_S2_Calculator.Utility.ListDisplay;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC_BSR_S2_Calculator.Utility.ListBrowser {

    public class ListBrowser<T, DC> : UserControl, ISwitchManaged
        where T : Displayable, IBrowserDisplayable<T, DC>
        where DC : FrameworkElement, ISwitchManaged {

        // --- VARIABLES ---

        // -- Management --
        #region Management

        // fowards tab content changes by checking the content for changes
        public bool TabContentsChanged 
            => ISwitchManaged.CheckBoolPropertyOn(DisplayContent, ISwitchManaged.TargetableProperties.TabContentsChanged);

        public bool RequiresReset { get; set; } = true;

        #endregion

        // -- Values --
        #region Values

        private int? _selectedIndex {
            get => field;
            set {
                if (value >= ListReference.Count) {
                    throw new IndexOutOfRangeException($"index {value} is out of range of the ListReference");
                }
                field = value;

                // update if null
                if (value is null) {
                    _displayContent = null;
                }
            }
        } = null;

        public int? SelectedIndex {
            get => _selectedIndex;
            set {
                _selectedIndex = value;
                OnSelectedIndexChanged();
            }
        }

        private T? _selectedItem {
            get {
                if (_selectedIndex is not null) {
                    return ListReference[(int)_selectedIndex];
                } else { return null; }
            }
            set {
                // value check
                if (value is null) {
                    _selectedIndex = null;
                    return;
                }

                // try to get displayable
                T? displayable = ListReference.FirstOrDefault(cls => cls == value);

                // set selected index based on found status/position of displayable
                if (displayable is null) {
                    _selectedIndex = null;
                    return;
                }
                _selectedIndex = ListReference.IndexOf(displayable);
            }
        }

        public T? SelectedItem {
            get => _selectedItem;
            set {
                _selectedItem = value;
                OnSelectedIndexChanged();
            }
        }

        private void OnSelectedIndexChanged() {
            if (SelectedItem is not null) {
                // trigger a switch
                SelectedItem.HeldLeftClickListener(this, EventArgs.Empty);
            } else { Reset(); }
        }

        #endregion

        // -- Interface Values --
        #region Interface Values

        public Grid MainGrid { get; private set; } = new();

        public Grid ContentGrid { get; private set; } = new();

        public Border NoContentBorder { get; private set; } = new();

        private DC? _displayContent {
            get => field;
            set {
                if (_displayContent != value) {
                    // name clarity
                    DC? newValue = value;
                    DC? oldValue = _displayContent;

                    // set to blank
                    if (newValue is null) {
                        NoContentBorder.Visibility = Visibility.Visible;
                        ContentGrid.Margin = new Thickness(0);

                    // add new content
                    } else {
                        NoContentBorder.Visibility = Visibility.Collapsed;
                        ContentGrid.Margin = new Thickness(5, 0, 0, 0);
                        ContentGrid.Children.Add(newValue);
                    }

                    // remove old content
                    if (oldValue is not null) {
                        ContentGrid.Children.Remove(oldValue);
                        if (oldValue is IDisposable disposable) {
                            disposable.Dispose();
                        }
                    }

                    // set value and update
                    field = value;
                    DisplayContentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public DC? DisplayContent {
            get => _displayContent;
            set {
                if (_displayContent != value) {
                    // set to blank
                    if (value is null) { // new value is null
                        _selectedIndex = null;
                    } else {
                        _selectedItem = T.LastSelectedDisplayable;
                    }

                    // set value
                    _displayContent = value;
                }
            }
        }

        public event EventHandler<EventArgs>? DisplayContentChanged;

        #endregion

        // -- Interface --
        #region Interface

        [Category("Common")]
        [Description("A reference to a list extending IBrowserDisplayable to reference data from")]
        public ListDisplay<T> ListReference {
            get => (ListDisplay<T>)GetValue(ListReferenceProperty);
            set => SetValue(ListReferenceProperty, value);
        }

        public static readonly DependencyProperty ListReferenceProperty = DependencyProperty.Register(
            nameof(ListReference),
            typeof(ListDisplay<T>),
            typeof(ListBrowser<T, DC>),
            new PropertyMetadata(null, OnListReferenceChanged)
        );

        private static void OnListReferenceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListBrowser<T, DC> control) {
                // null check
                if (control.ListReference is null) {
                    throw new ArgumentNullException("ListReference was not set");
                }

                // properties display settings
                if (args.OldValue is UIElement uiElement) {
                    control.MainGrid.Children.Remove(uiElement); // remove old
                }
                MainResources.RemoveParent(control.ListReference); // remove new parent
                control.MainGrid.Children.Add(control.ListReference); // add new
                Grid.SetColumn(control.ListReference, 0);

                // set up list reference
                control.SetupListReference();
            }
        }

        // - Column 1 Width -

        [Category("Layout")]
        [Description("The width of Column 1")]
        public GridLength Column1Width {
            get => (GridLength)GetValue(Column1WidthProperty);
            set => SetValue(Column1WidthProperty, value);
        }

        public static readonly DependencyProperty Column1WidthProperty = DependencyProperty.Register(
            nameof(Column1Width),
            typeof(GridLength),
            typeof(ListBrowser<T, DC>),
            new PropertyMetadata(new GridLength(1, GridUnitType.Star), OnColumn1WidthPropertyChanged)
        );

        private static void OnColumn1WidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListBrowser<T, DC> control) {
                control.MainGrid.ColumnDefinitions[0].Width = control.Column1Width;
            }
        }

        // - Column 2 Width -

        [Category("Layout")]
        [Description("The width of Column 2")]
        public GridLength Column2Width {
            get => (GridLength)GetValue(Column2WidthProperty);
            set => SetValue(Column2WidthProperty, value);
        }

        public static readonly DependencyProperty Column2WidthProperty = DependencyProperty.Register(
            nameof(Column2Width),
            typeof(GridLength),
            typeof(ListBrowser<T, DC>),
            new PropertyMetadata(new GridLength(1, GridUnitType.Star), OnColumn2WidthPropertyChanged)
        );

        private static void OnColumn2WidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ListBrowser<T, DC> control) {
                control.MainGrid.ColumnDefinitions[1].Width = control.Column2Width;
            }
        }

        #endregion

        // -- Utility --
        #region Utility

        private bool HasBeenLoaded { get; set; } = false;

        #endregion

        // --- CONSTRUCTORS ---
        #region CONSTRUCTORS 

        public ListBrowser() {
            // window settings
            Height = double.NaN;
            Width = double.NaN;

            // set up main grid
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Column1Width });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = Column2Width });

            // set list display content from the list reference
            Loaded += (_, _) => {
                // only load once
                if (HasBeenLoaded) { return; }
                HasBeenLoaded = true;

                // content space (default empty)
                NoContentBorder.Background = new SolidColorBrush(ColorResources.InnerColorL3);
                ContentGrid.Children.Add(NoContentBorder);

                // bind display content
                T.DisplayedContentChanged += (_, _)
                    => DisplayContent = T.DisplayedContent;

                // content grid to main grid
                MainGrid.Children.Add(ContentGrid);
                Grid.SetColumn(ContentGrid, 1);

                // set content
                this.Content = MainGrid;
            };
        }

        public void TabSwitchingCheck(object? sender, MouseButtonEventArgs args) {
            // reminder: selected item hasn't been updated yet
            
            // check tab contents
            if (TabContentsChanged) {
                // get confirmation
                Button targettedButton = (Button)(sender ?? throw new NullReferenceException("sender was null when attempting to intercept ListBrowser content changing"));

                // check if already selecting this object
                if (
                    (SelectedItem is not null)
                    && (targettedButton != SelectedItem.HeldButton)
                ) {
                    // get confirmation
                    if (ISwitchManaged.AskConfirmation()) {
                        // confirmed, continue
                        targettedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // simulate click on targeted button
                    } // not confirmed, don't continue
                }

                // handled
                args.Handled = true;
            }
        }

        private void SetupListReference() {
            // add event (and triggers) for tab switching behavior on selection changes
            ListReference.ClassDataListLoaded += (_, _) => {
                // existing items
                foreach (T instance in ListReference) {
                    Button? button = instance.HeldButton;
                    if (button is not null) {
                        button.PreviewMouseLeftButtonDown += TabSwitchingCheck;
                    }
                }

                // item added
                ListReference.ClassDataList.ItemAdded += (_, args) => {
                    if (args is ListChangedEventArgs castedArgs) {
                        Button? button = ListReference[castedArgs.NewIndex].HeldButton;
                        if (button is not null) {
                            button.PreviewMouseLeftButtonDown += TabSwitchingCheck;
                        }
                    }
                };
            };
        }

        #endregion

        // --- METHODS ---
        #region METHODS

        // - Reset -

        public void Reset()
            => _selectedItem = null;

        #endregion
    }
}
