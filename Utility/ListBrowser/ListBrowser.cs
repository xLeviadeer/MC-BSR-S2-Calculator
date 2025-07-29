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

    public class ListBrowser<T, DC> : UserControl
        where T : Displayable, IBrowserDisplayable<T, DC>
        where DC : FrameworkElement {

        // --- VARIABLES ---

        // -- Values --
        #region MainGrid

        public Grid MainGrid { get; private set; } = new();

        public Grid ContentGrid { get; private set; } = new();

        public Border NoContentBorder { get; private set; } = new();

        public DC? DisplayContent {
            get => field;
            set {
                if (field != value) {
                    OnDisplayContentChanged(this, value, field);
                    field = value;
                    DisplayContentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs>? DisplayContentChanged;

        /// <summary>
        /// hides/shows the NoContentBorder depending on if the display content is null
        /// sets content to the content grid
        /// </summary>
        private void OnDisplayContentChanged(ListBrowser<T, DC> sender, DC? newValue, DC? oldValue) {
            // set to blank
            if (newValue is null) {
                NoContentBorder.Visibility = Visibility.Visible;
                ContentGrid.Margin = new Thickness(0);
            
            // add new content
            } else {
                NoContentBorder.Visibility = Visibility.Collapsed;
                ContentGrid.Margin = new Thickness(5,0,0,0);
                ContentGrid.Children.Add(newValue);
            }

            // remove old content
            if (oldValue is not null) {
                ContentGrid.Children.Remove(oldValue);
                if (oldValue is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }

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

        #endregion
    }
}
