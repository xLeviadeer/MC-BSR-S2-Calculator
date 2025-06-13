using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC_BSR_S2_Calculator.Utility.Validations;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading.Tasks.Dataflow;
using MC_BSR_S2_Calculator.Utility;
using System.Windows.Media;

namespace MC_BSR_S2_Calculator.MainColumn.LandTracking {
    public partial class PropertyManager : UserControl, IValidityHolder {
        // --- VARIABLES ---
        #region VARIABLES

        // - Property Sections -

        public ObservableCollection<PropertySection> Sections { get; } = new();

        private const int MinSectionsCount = 1;
        private const int MaxSectionsCount = 30;

        #endregion

        // --- METHODS ---
        #region METHODS

        // - check sections overlap -

        private string GetSectionIntersectionInvalidityMessage(int propertySectionIndex) {
            string name = Sections[propertySectionIndex].SectionName.Text;
            return $"This section intersects with section {(string.IsNullOrWhiteSpace(name) ? $"at index {propertySectionIndex}" : $"\"{name}\"")}";
        }

        private void CheckSectionsOverlap(int? checkForIndex = null) {
            // only runs if there's 2 or more sections
            bool onlyRunSizeChecks = false;
            if (Sections.Count <= 1) {
                // set valid to true
                if (Sections.Count == 1) {
                    Sections[0].IsValidByConstraints = true;
                }

                // return
                onlyRunSizeChecks = true;
            }

            // holds an array of "sections" where each "section" is a HashSet containing a list of "section" indexes correlated with the already checked sections
            var defaultFoundStates = Enumerable.Repeat(true, Sections.Count).ToArray(); // true array of sections.count length

            // helper for setting error messages
            void setSectionErrorMessage(int propertySectionindex, bool doSet, string message) {
                PropertySection section = Sections[propertySectionindex];

                if (doSet) {
                    // override normal result text
                    section.MetricResult.Result = message;

                    // coloring
                    section.MetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.BrightRedColor);
                    section.MetricResult.ResultForeground = new SolidColorBrush(ColorResources.RedColor);
                } else {
                    section.MetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.InnerBorderVeryLightColor);
                    section.MetricResult.ResultForeground = new SolidColorBrush(Colors.Black);
                }
            }

            // finds an edge axis coordinate
            int getEdge(PropertySection section, bool getMax, bool getFromZAxis) {
                // select X or Z
                int selectedMin = (
                    (!getFromZAxis) ?
                    section.CoordinateInputCornerA.XCoordinate
                    : section.CoordinateInputCornerA.ZCoordinate
                );
                int selectedMax = (
                    (!getFromZAxis) ?
                    section.CoordinateInputCornerB.XCoordinate
                    : section.CoordinateInputCornerB.ZCoordinate
                );

                // min/max get section
                if (!getMax) { // min
                    return Math.Min(selectedMin, selectedMax);
                } else { // max
                    return Math.Max(selectedMin, selectedMax);
                }
            }

            // function for checking a square against other squares
            (bool, string) checkSectionForPerimeterSharing(int propertySectionIndex) {
                // get the section
                PropertySection section = Sections[propertySectionIndex];

                // tracking variables (flags basically)
                string errorMessage = "";
                bool foundAtLeastOneSpacingError = false;
                bool foundAtLeastOneValidConnection = false;

                // get coordinates for section
                int aMinX = getEdge(section, getMax: false, getFromZAxis: false);
                int aMaxX = getEdge(section, getMax: true, getFromZAxis: false);
                int aMinZ = getEdge(section, getMax: false, getFromZAxis: true);
                int aMaxZ = getEdge(section, getMax: true, getFromZAxis: true);

                // for every other section which hasn't already been checked
                for (int i = 0; i < Sections.Count; i++) {
                    PropertySection againstSection = Sections[i];

                    // don't run if the section combo has already been checked
                    if (i == propertySectionIndex) { continue; }

                    // - get the intersection space - 

                    // check x
                    int left = Math.Max(
                        aMinX,
                        getEdge(againstSection, getMax: false, getFromZAxis: false) // bMinX
                    );
                    int right = Math.Min( 
                        aMaxX,
                        getEdge(againstSection, getMax: true, getFromZAxis: false) // bMaxX
                    );
                    if ( // bounds less than 0 (not touching)
                        !foundAtLeastOneValidConnection
                        && (right < left)
                    ) { goto spacing_error; }

                    // check z
                    int bottom = Math.Max( 
                        aMinZ,
                        getEdge(againstSection, getMax: false, getFromZAxis: true) // bMinZ
                    );
                    int top = Math.Min( 
                        aMaxZ,
                        getEdge(againstSection, getMax: true, getFromZAxis: true) // bMaxZ
                    );
                    if ( // bounds less than 0 (not touching)
                        !foundAtLeastOneValidConnection
                        && (top < bottom)
                    ) { goto spacing_error; }

                    // find the intersection square dimensions
                    int width = Math.Abs(right - left) + 1;
                    int height = Math.Abs(top - bottom) + 1; 
                    if ((width > 1) && (height > 1)) { // intersecting
                        // set regardless of sections.count
                        defaultFoundStates[propertySectionIndex] = false;
                        defaultFoundStates[i] = false;
                        errorMessage = GetSectionIntersectionInvalidityMessage(i);
                        section.IsInvalidByIntersection = true;
                        continue;
                    } else if ( // not touching 
                        !foundAtLeastOneValidConnection
                        && ((width < 2) && (height < 2))
                    ) { goto spacing_error; } 

                    // find area and verify perimeter length min requrirement
                    int area = Math.Max(height, width);
                    if ( // perimeter share less than 3
                        !foundAtLeastOneValidConnection
                        && (area < 3)
                    ) {
                        defaultFoundStates[propertySectionIndex] = false;
                        defaultFoundStates[i] = false;
                        errorMessage = "This section doesn't share a perimeter of at least 3 with another section";
                        section.IsInvalidByIntersection = false;
                        continue;
                    }

                    // found a valid connection
                    foundAtLeastOneValidConnection = true;
                    continue;

                // continuation helper
                spacing_error:
                    foundAtLeastOneSpacingError = true;
                    errorMessage = "This section is not touching any other section";
                    section.IsInvalidByIntersection = false;
                    continue;
                }

                // check for spacing error
                // - if a spacing error was found but the section is otherwise valid, turn it to invalid
                bool isValid = defaultFoundStates[propertySectionIndex];
                if (!foundAtLeastOneValidConnection && defaultFoundStates[propertySectionIndex] && foundAtLeastOneSpacingError) {
                    isValid = false;
                }
                
                // update real validity
                section.IsValidByConstraints = isValid;
                return (!isValid, errorMessage);
            }

            // function for checking if a square is at least 5 by 5
            (bool, string) checkSectionForMinSize(int propertySectionIndex) {
                PropertySection section = Sections[propertySectionIndex];
                const int minSize = 5;

                // check height and width
                bool doSet = false;
                if (
                    (section.MetricX < minSize)
                    || (section.MetricZ < minSize)
                ) {
                    section.IsValidByConstraints = false;
                    doSet = true;
                }

                return (doSet, section.MetricResult.DefaultResult);
            }

            // check one section or check all of them
            if (checkForIndex != null) {
                bool doSet = false;
                string message = "";
                if (!onlyRunSizeChecks) {
                    (doSet, message) = checkSectionForPerimeterSharing((int)checkForIndex);
                }
                (bool doSetFromMinSize, string messageFromMinSize) = checkSectionForMinSize((int)checkForIndex);

                // check do set override for min size
                if (doSetFromMinSize) {
                    message = messageFromMinSize;
                    doSet = doSetFromMinSize;
                }

                // message and colors
                setSectionErrorMessage((int)checkForIndex, doSet, message);
            } else {
                for (int i = 0; i < Sections.Count; i++) {
                    bool doSet = false;
                    string message = "";
                    if (!onlyRunSizeChecks) {
                        Sections[i].CalculateAndDisplayMetric(); // recalculate metric (clears error text)
                        (doSet, message) = checkSectionForPerimeterSharing(i);
                    }
                    (bool doSetFromMinSize, string messageFromMinSize) = checkSectionForMinSize(i);

                    // check do set override for min size
                    if (doSetFromMinSize) {
                        message = messageFromMinSize;
                        doSet = doSetFromMinSize;
                    }

                    // message and colors
                    setSectionErrorMessage(i, doSet, message);
                }
            }
        }

        // - update total metric result -

        private void UpdateTotalMetricResult() {
            Debug.WriteLine("first");
            Debug.WriteLine(Sections.Any(section => !section.IsValidByConstraints));
            foreach (var section in Sections) {
                Debug.WriteLine(section.IsValidByConstraints);
            }



            if (Sections.Any(section => !section.IsValidByConstraints)) {
                // text
                TotalMetricResult.Result = TotalMetricResult.DefaultResult;

                // colors
                TotalMetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.RedColor);
                TotalMetricResult.ResultForeground = new SolidColorBrush(ColorResources.DullRedColor);
            } else {
                // text
                double result = 0;
                foreach (PropertySection section in Sections) {
                    result += section.Metric;
                }
                TotalMetricResult.Result = result.ToString();

                // colors
                TotalMetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.InnerBorderVeryLightColor);
                TotalMetricResult.ResultForeground = new SolidColorBrush(Colors.Black);
            }
        }

        // - update remaining sections display -

        private void UpdateRemainingSectionsDisplay() {
            RemainingSectionsDisplay.Content = $"{Sections.Count} / {MaxSectionsCount}";
        }

        // - set up section -

        private void SetUpSection(PropertySection section) {
            section.DeletionRequested += (_, __) => {
                RemoveSection(section);
            };
            section.ValidityChanged += (_, __) => {
                CheckAndSetSectionsValidity();
                UpdateCreateButtonEnabledStatus();
            };
            section.CoordinatesChanged += (_, __) => {
                CheckSectionsOverlap();
                CheckAndSetSectionsValidity();
                UpdateCreateButtonEnabledStatus();
                UpdateTotalMetricResult();
            };
            section.SectionName.TextChanged += (_, __) => {
                // updates just invalid by intersections text only
                for (int i = 0; i < Sections.Count; i++) {
                    PropertySection section = Sections[i];
                    if (section.IsInvalidByIntersection) {
                        section.MetricResult.Result = GetSectionIntersectionInvalidityMessage(i);
                    }
                }
            };
        }

        // - per item section delete -

        private void RemoveSection(PropertySection section) {
            // remove
            Sections.Remove(section);
            UpdateRemainingSectionsDisplay();

            // check if too little
            if (Sections.Count <= MinSectionsCount) {
                HideSectionDeleteButtons();
            }

            // update button and validity
            CheckSectionsOverlap();
            CheckAndSetSectionsValidity();
            UpdateCreateButtonEnabledStatus();
            UpdateTotalMetricResult();

            // check if there are no longer too many sections            
            if (Sections.Count < MaxSectionsCount) {
                AddPropertySectionButton.IsEnabled = true;
            }
        }

        private int _lastCountTaken = 0;

        private void ShowSectionDeleteButtons() {
            if (_lastCountTaken != Sections.Count) {
                foreach (var section in Sections) {
                    section.ShowDeleteButton = true;
                }

                _lastCountTaken = Sections.Count;
            }
        }

        private void HideSectionDeleteButtons() {
            if (_lastCountTaken != Sections.Count) {
                foreach (var section in Sections) {
                    section.ShowDeleteButton = false;
                }

                _lastCountTaken = Sections.Count;
            }
        }

        // - add section -

        private void AddPropertySectionButton_Click(object sender, RoutedEventArgs args) {
            // add new section
            Sections.Add(new());
            UpdateRemainingSectionsDisplay();
            Dispatcher.BeginInvoke(() => { // adding is special,
                                           // because you need to wait until element has been set to continue (aka it's been loaded)
                CheckSectionsOverlap();
            }, DispatcherPriority.Loaded);
            UpdateTotalMetricResult();

            // assign deletion event
            SetUpSection(Sections[Sections.Count - 1]);

            // check if too little sections
            if (Sections.Count > MinSectionsCount) {
                ShowSectionDeleteButtons();
            }

            // update button and validity
            CheckAndSetSectionsValidity();
            UpdateCreateButtonEnabledStatus();

            // check if there are no longer too many sections            
            if (Sections.Count >= MaxSectionsCount) {
                AddPropertySectionButton.IsEnabled = false;
            }
        }

        // - sections reset -

        private void ResetSections() {
            // clear
            Sections.Clear();

            // add base section
            Sections.Add(new());
            CheckSectionsOverlap();
            UpdateRemainingSectionsDisplay();
            UpdateTotalMetricResult();

            // section events
            PropertySection section = Sections[Sections.Count - 1];
            SetUpSection(section);

            // delete button visibility false
            section.ShowDeleteButton = false;
        }

        // - section validity -

        private void CheckAndSetSectionsValidity() {
            Validity[nameof(Sections)].IsValid = Sections.All(section => section.CheckValidity());
        }

        #endregion

    }
}
