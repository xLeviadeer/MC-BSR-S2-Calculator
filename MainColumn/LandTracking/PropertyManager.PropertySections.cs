using MC_BSR_S2_Calculator.Utility;
using MC_BSR_S2_Calculator.Utility.Validations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

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

        private enum PropertySectionErrorTypes {
            None = 0,
            Spacing = 1,
            Area = 2,
            AtLeastOneValid = 3,
            Intersection = 4,
            Size = 5
        }

        private int GetPropertySectionErrorPriority(PropertySectionErrorTypes errorType) {
            return (int)errorType;
        }

        private string GetSectionIntersectionInvalidityMessage(int propertySectionIndex) {
            string name = Sections[propertySectionIndex].SectionName.Text;
            return $"This section intersects with section {
                (string.IsNullOrWhiteSpace(name) 
                ? $"at index {propertySectionIndex}" 
                : $"\"{name}\"")
            }";
        }

        private void SetSectionErrorMessage(int propertySectionindex, bool doSet, string message) {
            PropertySection section = Sections[propertySectionindex];

            if (doSet) {
                // override normal result text
                section.MetricResult.Result = message;

                // coloring
                section.MetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.BrightRedColor);
                section.MetricResult.ResultForeground = new SolidColorBrush(ColorResources.RedColor);
            } else {
                section.MetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.InnerColorL3);
                section.MetricResult.ResultForeground = new SolidColorBrush(Colors.Black);
            }
        }

        private void CheckSectionsOverlap() {
            // array (length of sections) of tuples
            // which contains the error type found by other checks of highest priority
            // and the index (in Sections) which this error occured at
            var errorsBySection = new (PropertySectionErrorTypes error, int index)[Sections.Count];

            // array (length of sections)
            // which contains hashsets
            // which contain a list of indexes which have already been checked
            var alreadyChecked = new HashSet<int>[Sections.Count];

            void checkSectionForMinSize(int propertySectionIndex) {
                PropertySection section = Sections[propertySectionIndex];
                const int minSize = 5;

                // check height and width
                if (
                    (section.Subsection.Height < minSize)
                    || (section.Subsection.Width < minSize)
                ) {
                    // set error
                    errorsBySection[propertySectionIndex] = (
                        PropertySectionErrorTypes.Size, 
                        propertySectionIndex
                    );
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

            // function for checking a square against other squares for intersection, area and spacing
            void checkSectionForPerimeterSharing(int propertySectionIndex) {
                // returning from this function means holding whatever validity state was determined in the size check
                // error priorities are as follows
                // - intersect : immediate return
                // - area : still checks intersections
                // - spacing : still checks area and intersect
                // - none : all checks
                // error priority is found from the enum position
                // - higher error priority is more restrictive
                // - lower error priority is less restrictive

                // section reference
                PropertySection section = Sections[propertySectionIndex];
                int errorPriority = GetPropertySectionErrorPriority(errorsBySection[propertySectionIndex].error);

                // don't run if there's not enough sections
                if (Sections.Count <= 1) { return; }

                // don't run if the error priority is an intersection or more
                if (errorPriority >= GetPropertySectionErrorPriority(PropertySectionErrorTypes.Intersection)) { return; }

                // for every other section which hasn't already been checked
                for (int i = 0; i < Sections.Count; i++) {
                    // set the alreadyChecked hashmap if not set
                    if (alreadyChecked[i] is null) {
                        alreadyChecked[i] = new();
                    }

                    // skip if attempting to check against self
                    // skip if already checked
                    if (
                        (i == propertySectionIndex)
                        || alreadyChecked[propertySectionIndex].Contains(i)
                    ) { continue; }

                    // re-assess error priority
                    errorPriority = GetPropertySectionErrorPriority(errorsBySection[propertySectionIndex].error);

                    // section reference
                    PropertySection againstSection = Sections[i];

                    // helper for setting errorsBySection
                    void SetError(PropertySectionErrorTypes errorType) {
                        errorsBySection[propertySectionIndex] = (
                            errorType,
                            propertySectionIndex
                        );
                        errorsBySection[i] = (
                            errorType,
                            i
                        );
                    }

                    // - get coordinates for box c - 
                    // checks for (indirect) spacing errors

                    // check x
                    int cLeft = Math.Max(
                        getEdge(section, getMax: false, getFromZAxis: false), // a left
                        getEdge(againstSection, getMax: false, getFromZAxis: false) // b left
                    );
                    int cRight = Math.Min(
                        getEdge(section, getMax: true, getFromZAxis: false), // a right
                        getEdge(againstSection, getMax: true, getFromZAxis: false) // b right
                    );
                    if (cRight < cLeft) { // spacing error
                        if (errorPriority <= GetPropertySectionErrorPriority(PropertySectionErrorTypes.None)) { // if spacing errors are in priority
                            goto spacing_error;
                        } else { // spacing errors are not in priority
                            continue;
                        }
                    } 
                    
                    // check z
                    int cBottom = Math.Max(
                        getEdge(section, getMax: false, getFromZAxis: true), // a bottom
                        getEdge(againstSection, getMax: false, getFromZAxis: true) // b bottom
                    );
                    int cTop = Math.Min(
                        getEdge(section, getMax: true, getFromZAxis: true), // a top
                        getEdge(againstSection, getMax: true, getFromZAxis: true) // b top
                    );
                    if (cTop < cBottom) { // spacing error
                        if (errorPriority <= GetPropertySectionErrorPriority(PropertySectionErrorTypes.None)) { // if spacing errors are in priority
                            goto spacing_error;
                        } else { // spacing errors are not in priority
                            continue;
                        }
                    }

                    // get dimensions of box c
                    int width = Math.Abs(cRight - cLeft) + 1;
                    int height = Math.Abs(cTop - cBottom) + 1;

                    // - check for intersection errors -

                    // check intersection
                    // must be less than intersection priority
                    // aka equal to area or less because that's the total function scope
                    if ((width > 1) && (height > 1)) {
                        SetError(PropertySectionErrorTypes.Intersection);
                        section.IsInvalidByIntersection = true;
                        alreadyChecked[i].Add(propertySectionIndex);
                        return; // immediate return, highest priority
                    } 

                    // - check for area errors -

                    // find area 
                    int area = Math.Max(height, width);

                    // check area
                    if (area < 3) {
                        // check if area is in priority
                        if (errorPriority <= GetPropertySectionErrorPriority(PropertySectionErrorTypes.Spacing)) {
                            SetError(PropertySectionErrorTypes.Area);
                            goto non_spacing_error;
                        }

                        // don't make further checks
                        continue;
                    }

                    // - check for (direct) spacing errors -

                    // check spacing
                    if ((width < 2) && (height < 2)) {
                        if (errorPriority <= GetPropertySectionErrorPriority(PropertySectionErrorTypes.None)) {
                            goto spacing_error;
                        }

                        // don't make further checks
                        continue;
                    }

                    // found a valid connection; don't run spacing error
                    SetError(PropertySectionErrorTypes.AtLeastOneValid);
                    section.IsInvalidByIntersection = false;
                    continue;

                // spacing errors
                spacing_error:
                    SetError(PropertySectionErrorTypes.Spacing);
                    section.IsInvalidByIntersection = false;

                    // don't make further checks
                    continue;

                // non-spacing errors
                non_spacing_error:
                    // set against's alreadyChecked
                    // self's alreadyChecked isn't set because we never recursively check this element again
                    alreadyChecked[i].Add(propertySectionIndex);

                    // isn't an intersection error
                    section.IsInvalidByIntersection = false;

                    // don't make further checks
                    continue;
                }
            }

            // check every section
            for (int i = 0; i < Sections.Count; i++) {
                PropertySection section = Sections[i];

                // recalculate metric (clears error text)
                if (Sections.Count > 1) {
                    section.CalculateAndDisplayMetric();
                } else if (Sections.Count == 1) {
                    // set default to valid
                    // because size check will only be ran anyway, which overrides this
                    errorsBySection[0] = (
                        PropertySectionErrorTypes.AtLeastOneValid,
                        0
                    );
                }

                // always run size checks (can set errorsBySection)
                checkSectionForMinSize(i);

                // perimeter check (can set errorsBySection)
                checkSectionForPerimeterSharing(i);

                // set section color and validity
                section.IsValidByConstraints = (errorsBySection[i].error == PropertySectionErrorTypes.AtLeastOneValid);
                string message = errorsBySection[i].error switch {
                    PropertySectionErrorTypes.Size => section.MetricResult.DefaultResult,
                    PropertySectionErrorTypes.Intersection => GetSectionIntersectionInvalidityMessage(errorsBySection[i].index),
                    PropertySectionErrorTypes.AtLeastOneValid => "",
                    PropertySectionErrorTypes.Area => "This section doesn't share a perimeter of at least 3 with another section",
                    PropertySectionErrorTypes.Spacing => "This section is not touching any other section",
                    PropertySectionErrorTypes.None => "",
                    _ => ""
                };
                SetSectionErrorMessage(i, !section.IsValidByConstraints, message);
            }
        }

        // - update total metric result -

        private void UpdateTotalMetricResult() {
            if (Sections.Any(section => !section.IsValidByConstraints)) {
                // text
                TotalMetricResult.Result = TotalMetricResult.DefaultResult;

                // colors
                TotalMetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.RedColor);
                TotalMetricResult.ResultForeground = new SolidColorBrush(ColorResources.DullRedColor);
            } else {
                // text
                TotalMetricResult.Result = Property.GetPropertyMetric(Sections // selects all subsections from sections
                    .Select(section => section.Subsection)
                    .ToArray()
                ).ToString();

                // colors
                TotalMetricResult.ResultBorderBrush = new SolidColorBrush(ColorResources.InnerColorL3);
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
            section.MoveUpRequested += (_, __) => {
                MoveSectionUp(section);
            };
            section.MoveDownRequested += (_, __) => {
                MoveSectionDown(section);
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

        // - per item section movement -

        private void DisableTopBottomSectionMoveButtons() {
            // enable all sections
            foreach (PropertySection section in Sections) {
                section.UpButton.IsEnabled = true;
                section.DownButton.IsEnabled = true;
            }

            // disable top
            Sections[0].UpButton.IsEnabled = false;

            // disable bottom
            Sections[Sections.Count - 1].DownButton.IsEnabled = false;
        }

        private void MoveSectionUp(PropertySection section) {
            // find index
            int currPos = Sections.IndexOf(section);

            // button disable/enable handling
            if (currPos == 0) {
                section.UpButton.IsEnabled = false;
            } else {
                section.UpButton.IsEnabled = true;
            }

            // move
            Sections.Move(currPos, currPos - 1);

            // apply enable disable
            DisableTopBottomSectionMoveButtons();
        }

        private void MoveSectionDown(PropertySection section) {
            // find index
            int currPos = Sections.IndexOf(section);

            // button disable/enable handling
            if (currPos == (Sections.Count - 1)) {
                section.DownButton.IsEnabled = false;
            } else {
                section.DownButton.IsEnabled = true;
            }

            // move
            Sections.Move(currPos, currPos + 1);

            // apply enable disable
            DisableTopBottomSectionMoveButtons();
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
            DisableTopBottomSectionMoveButtons();
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
            DisableTopBottomSectionMoveButtons();
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
            DisableTopBottomSectionMoveButtons();
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
            UpdateFinalResults();
        }

        private bool CheckIfSectionsChanged() {
            // changed checks
            if (
                (Sections.Count != 1) // check for section count
                || (Sections[0].SectionName.Element.TabContentsChanged) // check for name
                || (Sections[0].Subsection.TopLeft != new FlatCoordinate(0, 0)) // check if section isn't at 0,0
                || (Sections[0].Subsection.BottomRight != new FlatCoordinate(0, 0)) // check if section isn't at 0,0
            ) {
                return true;
            }

            // no changes
            return false;
        }

        #endregion

    }
}
