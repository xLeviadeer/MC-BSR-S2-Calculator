using MC_BSR_S2_Calculator.Utility.TextBoxes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MC_BSR_S2_Calculator.Utility.LabeledInputs {

    [ContentProperty(nameof(Items))] // allows the class to take combo box items
    public class ComboLabel : LabeledInputBase<ComboBox> {

        // --- VARIABLES ---
        #region VARIABLES

        // - combo box -
        public override ComboBox Element { get; set; } = new();

        // - has been loaded -
        private bool HasBeenLoaded { get; set; } = false;

        // - expose selection changed -
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        // - expose items -
        public ItemCollection Items {
            get => Element.Items;
        }

        public IEnumerable ItemsSource {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(ComboLabel),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged)
        );
        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            if (sender is ComboLabel control) {
                control.Element.ItemsSource = control.ItemsSource;
            }
        }

        public int SelectedIndex {
            get => Element.SelectedIndex;
            set => Element.SelectedIndex = value;
        }

        public object SelectedItem {
            get => Element.SelectedItem;
            set => Element.SelectedItem = value;
        }

        #endregion

        // --- CONSTRUCTOR ---
        #region CONSTRUCTOR

        public ComboLabel() {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, EventArgs args) {
            // don't run if already loaded
            if (HasBeenLoaded) { return; }
            HasBeenLoaded = true;

            // combo box settings
            var horiAlign = HorizontalAlignment.Left;
            var vertAlign = VerticalAlignment.Center;
            Element.HorizontalContentAlignment = horiAlign;
            Element.VerticalContentAlignment = vertAlign;
            Element.Height = (double)Application.Current.Resources["ComboBoxHeight"];
            Element.Margin = new Thickness(3);
            Element.FontSize = 11;

            // fixes (debug only) faulty binding message
            var itemContainerStyle = new Style(typeof(ComboBoxItem));
            itemContainerStyle.Setters.Add(new Setter(ComboBoxItem.HorizontalContentAlignmentProperty, horiAlign));
            itemContainerStyle.Setters.Add(new Setter(ComboBoxItem.VerticalContentAlignmentProperty, vertAlign));
            Element.ItemContainerStyle = itemContainerStyle;

            // event exposure
            Element.SelectionChanged += (_, args) => {
                SelectionChanged?.Invoke(this, args);
            };

            // apply layout mode
            ApplyLayoutMode();
        }

        #endregion
    }
}
