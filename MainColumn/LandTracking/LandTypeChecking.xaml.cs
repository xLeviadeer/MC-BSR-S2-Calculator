using MC_BSR_S2_Calculator.PlayerColumn;
using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Coordinates;
using MC_BSR_S2_Calculator.Utility.Identification;
using MC_BSR_S2_Calculator.Utility.LabeledInputs;
using MC_BSR_S2_Calculator.Utility.SwitchManagedTab;
using MC_BSR_S2_Calculator.Utility.XamlConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {

    [BlocksSwitchManagement]
    public partial class LandTypeChecking : UserControl {

        // --- CONSTRUCTORS ---

        public LandTypeChecking() {
            InitializeComponent();

            // default result update only after players have been loaded and the coordinates have been loaded
            EventWatcher updateOnLoaded = new((_, _) => UpdateResults());
            MainResources.PlayersDisplay.ClassDataListLoaded += updateOnLoaded.NewWatchAction();
            MainResources.PropertiesDisplay.ClassDataListLoaded += updateOnLoaded.NewWatchAction();
            Coordinates.LayoutLoaded += updateOnLoaded.NewWatchAction();

            // try to add margin to scroll bar
            var scrollBar = XamlConverter.FindVerticalScrollBar(MainScrollViewer);
            if (scrollBar != null) {
                scrollBar.Margin = new Thickness(3, 0, 0, 0);
            }
        }

        // --- METHODS ---

        public void UpdateResults() {
            // list of found locations
            LandTypeCheckingResult.ClearResults();
            List<ILandArea> foundLands = new();

            // check all landarea
            foreach (ILandArea landArea in MainResources.LandAreasList) {
                IFlatCoordinate coord = (UseYCoordinate.IsChecked) ?? false
                    ? Coordinates.GetCoordinatesAs<CoordinatePoint>()
                    : Coordinates.GetCoordinatesAs<FlatCoordinatePoint>();
                // check if the landArea contains the current typed coordinates
                if (landArea.Contains(coord)) {
                    // add to list of found locations
                    foundLands.Add(landArea);
                }
            }

            // if no results, set to free land
            if (foundLands.Count < 1) {
                LandTypeCheckingResult.AddNewResult(
                    XamlConverter.CapitalizeWords(ILandArea.FREE), 
                    "No-one"
                );
                goto set_bolding;
            }

            // sort the found locations by priority
            foundLands = foundLands.OrderBy(land => land.LandType switch {
                ILandArea.PRIVATE => 1,
                ILandArea.SHARED_PRIVATE => 2,
                ILandArea.OWNED => 3,
                ILandArea.PUBLIC => 4,
                ILandArea.PROVISIONED => 5,
                ILandArea.UNOWNED => 6,
                ILandArea.FREE => 7,
                _ => throw new ArgumentException("Impossible value found in LandType")
            }).ToList();

            // set results
            foundLands.ForEach(land => {
                // try to get owner name
                string owningPlayerName;
                try {
                    owningPlayerName = Player.GetPlayerNameFrom(land.OwnerID);
                } catch (ArgumentException) {
                    string identifier = ID.ConstructHighestIdentifier(land.OwnerID);
                    MessageBox.Show($"Invalid ID used, {identifier}, for OwnerID when attempting to process Lands (from Lands.json)");
                    throw;
                }

                // add new result
                LandTypeCheckingResult.AddNewResult(
                    XamlConverter.CapitalizeWords(land.LandType),
                    owningPlayerName
                );
            });

        // set bolding
        set_bolding:
            LandTypeCheckingResult.BoldFirst();
        }

        private void _useYCoordinateCheckChanged(object sender, Utility.BoolEventArgs args) {
            // set Y coordinate
            Coordinates.ShowYInput = args.Value ?? false;
            UpdateResults();
        }

        private void _coordinatesCoordinatesChanged(object sender, EventArgs e)
            => UpdateResults();
    }

    public class LandTypeCheckingResult : UserControl {

        // --- VARIABLES ---

        // -- Static --

        public static ObservableCollection<LandTypeCheckingResult> ResultsCollection { get; set; } = new();

        private static LandTypeCheckingResult _firstResult => ResultsCollection[0];

        // -- Instance --

        private Grid? _mainGrid { get; set; } = null;

        private GridSplitter? _resultSplitter { get; set; } = null;

        private ResultDisplay? _landTypeResult { get; set; } = null;

        private ResultDisplay? _ownerNameResult { get; set; } = null;

        // --- METHODS ---

        public static void BoldFirst() {
            // error if null
            if (_firstResult._landTypeResult is null) {
                throw new NullReferenceException($"First result was null or could not be found");
            }

            // set outline color darker
            _firstResult._landTypeResult.ResultBorderBrush = new SolidColorBrush(ColorResources.InnerColorM);

            // set to bold
            _firstResult._landTypeResult.LayoutLoaded += (_, _) => {
                ((TextBlock)_firstResult._landTypeResult!.Element.Child).FontWeight = FontWeights.Bold;
            };
        }

        public static void ClearResults()
            => ResultsCollection.Clear();

        public static void RemoveResultAt(int index)
            => ResultsCollection.RemoveAt(index);

        public static void RemoveResult(LandTypeCheckingResult result) 
            => ResultsCollection.Remove(result);

        public static void AddNewResult(string landType, string ownerName) {
            // new result
            var result = new LandTypeCheckingResult();

            // create main grid
            Grid mainGrid = new();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { // columns
                Width = new GridLength(160, GridUnitType.Pixel)
            });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition() {
                Width = new GridLength(1, GridUnitType.Star)
            });
            mainGrid.RowDefinitions.Add(new RowDefinition()); // rows
            mainGrid.RowDefinitions.Add(new RowDefinition() {
                Height = new GridLength(5, GridUnitType.Pixel)
            });
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());
            result._mainGrid = mainGrid;

            // create grid split
            GridSplitter resultSplitter = new() {
                Height = 3,
                IsEnabled = false,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            mainGrid.Children.Add(resultSplitter);
            Grid.SetRow(resultSplitter, 0);
            Grid.SetColumn(resultSplitter, 0);
            Grid.SetColumnSpan(resultSplitter, 2);
            result._resultSplitter = resultSplitter;

            // create land type result
            ResultDisplay landTypeResult = new() {
                LabelText = "Land Type at Coordinates",
                LayoutMode = ResultDisplay.LabeledInputLayoutModes.Left,
                FluidProportionsSplitIndex = 1
            };
            landTypeResult.LayoutLoaded += (_, _) => {
                landTypeResult.Result = landType;
            };
            mainGrid.Children.Add(landTypeResult);
            Grid.SetRow(landTypeResult, 2);
            Grid.SetColumn(landTypeResult, 0);
            Grid.SetColumnSpan(landTypeResult, 2);
            result._landTypeResult = landTypeResult;

            // create owner name result
            ResultDisplay ownerNameResult = new() {
                LabelText = "Land Owner at Coordinates",
                LayoutMode = ResultDisplay.LabeledInputLayoutModes.Left,
                FluidProportionsSplitIndex = 1
            };
            ownerNameResult.LayoutLoaded += (_, _) => {
                ownerNameResult.Result = ownerName;
            };
            mainGrid.Children.Add(ownerNameResult);
            Grid.SetRow(ownerNameResult, 3);
            Grid.SetColumn(ownerNameResult, 0);
            Grid.SetColumnSpan(ownerNameResult, 2);
            result._ownerNameResult = ownerNameResult;

            // set content
            result.Content = mainGrid;
            ResultsCollection.Add(result);
        }
    }
}
