using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {
    public class DoubleTextBox : NumberTextBox {
        // --- VARIABLES ---
        #region VARIABLES

        /// <summary>
        /// Controls how many digits to round to; leave as null for no rounding
        /// </summary>
        public int? DigitsOfRounding {
            get => (int?)GetValue(DigitsOfRoundingProperty);
            set => SetValue(DigitsOfRoundingProperty, value);
        }

        /// <summary>
        /// Dependency property for DigitsOfRounding
        /// </summary>
        [Category("Common")]
        [Description("controls how many digits to round to; leave as null for no rounding")]
        public static readonly DependencyProperty DigitsOfRoundingProperty = DependencyProperty.Register(
            nameof(DigitsOfRounding),
            typeof(int?),
            typeof(NumberTextBox),
            new PropertyMetadata(2)
        );

        #endregion

        // --- METHODS ---
        #region METHODS

        /// <summary>
        /// Rounds accordinging to the digits of rounding
        /// </summary>
        protected override double ResultsRounding(double result) {
            if (DigitsOfRounding != null) {
                return (double)Math.Round((decimal)result, (int)DigitsOfRounding);
            }
            return result;
        }

        #endregion
    }

    public class IntegerTextBox : NumberTextBox {
        // --- METHODS ---
        #region METHODS

        /// <summary>
        /// Always rounds
        /// </summary>
        protected override double ResultsRounding(double result) {
            return (double)Math.Round((decimal)result, 0);
        }

        #endregion
    }

    public abstract class NumberTextBox : TypedTextBox<double> {

        // --- CONSTRUCTOR ---

        public NumberTextBox() : base() {
            DoColorChanges = false;
        }

        // --- METHODS ---
        #region METHODS

        protected abstract double ResultsRounding(double result);

        public override void Validate(object? sender, EventArgs args) {
            // conversion method
            double convert(double value, char asType) {
                if (asType == 'h') {
                    return value * 100;
                } else { // must be k
                    return value * 1000;
                }
            }
            
            // type cast sender
            ArgumentNullException.ThrowIfNull(sender);
            var textBox = (TypedTextBox<double>)sender;

            // clean string
            string cleanedText = textBox.Text;
            string[] cleanerChars = [" ", ","];
            foreach (string cleanerChar in cleanerChars) {
                cleanedText = cleanedText.Replace(cleanerChar, "");
            }

            // prepend/append parenthesis; stupid solution but it works
            cleanedText = '(' + cleanedText + ')';

            // stack of characters and indexes
            Stack<(char chr, int index)> charStack = new();

            // data table for computational evaluations
            DataTable table = new();
            (bool succeeded, double result) evaluate(string str) {
                // dont evaluate if it's just a single number
                string parenthesilessString = str.Substring(1, str.Length - 1);
                if (Regex.IsMatch(
                    parenthesilessString, 
                    @"^\d+(\.\d+)?$")
                ) {
                    try { // try to parse immediately
                        return (true, double.Parse(parenthesilessString, CultureInfo.InvariantCulture));
                    } catch (Exception err) when (
                        err is FormatException
                        || err is ArgumentNullException
                        || err is OverflowException
                        || err is SyntaxErrorException
                    ) { // return failiure
                        return (false, -1);
                    }           
                }

                // evaluate algebra
                double result;
                try {
                    result = Convert.ToDouble(table.Compute(str, null));
                } catch (Exception err) when (
                    err is SyntaxErrorException
                    || err is EvaluateException
                    || err is InvalidCastException
                    || err is FormatException
                    || err is OverflowException
                ) {
                    return (false, -1);
                }

                // success
                return (true, result);
            }

            // offset tracker
            int offsetCount = 0; // amount of offset created by evaluation

            // evaluation method; converts h and k and evals sub-parts
            bool evaluatePortion(int currIndex, bool applyOffset) {
                // pop last (assumes error free)
                (char chr, int index) currPop = charStack.Pop();

                // only applies offset if specified
                int offsetIndex = currPop.index + (applyOffset ? offsetCount : 0);

                // get eval portion from main string
                string evalPortion = cleanedText.Substring(currIndex, offsetIndex - currIndex);

                // evaluate portion
                (bool succeeded, double result) = evaluate(evalPortion);
                if (!succeeded) { return false; }

                // if divided by 0
                if (double.IsInfinity(result)) {
                    RevertText(textBox);
                    return false;
                }

                // apply k and h conversion factors
                result = convert(result, currPop.chr);

                // update offsets to exclude k or h letter
                offsetIndex += 1;

                // place result into cleanedString
                cleanedText = 
                    cleanedText.Substring(0, currIndex)
                    + result.ToString()
                    + cleanedText.Substring(offsetIndex)
                ;

                // apply offset
                offsetCount += result.ToString().Length - evalPortion.Length - 1; // - 1 accounts for removal of h or k

                // success
                return true;
            }

            // for every char going from end to beginning
            int runLength = cleanedText.Length - 1;
            for (int i = runLength; i >= 0; i--) {
                char currChar = char.ToLower(cleanedText[i]);

                // character checking
                switch (currChar) {
                    case 'h':
                    case 'k':
                    case ')':
                        charStack.Push((currChar, i));
                        break;
                    case '(': // eval for closing parenthesis
                        // if less than 0 there aren't enough to have a conversion
                        if (charStack.Count <= 0) {
                            continue;
                        }

                        // if k or h
                        if (charStack.First().chr == 'k' || charStack.First().chr == 'h') {
                            // skip if last symbol is not a digit
                            if (i < runLength - 1 && !char.IsDigit(cleanedText[i + 1])) {
                                RevertText(textBox);
                                return;
                            }

                            // evaluate
                            if (!evaluatePortion(i + 1, applyOffset:false)) { // dont include parenthesis
                                RevertText(textBox);
                                return; // return if failed
                            } 
                        }

                        // if closed parenthesis area
                        if (charStack.First().chr == ')') {
                            charStack.Pop(); // dispose of opening parenthesis

                            // if less than 0 there aren't enough to have a conversion
                            if (charStack.Count <= 0) {
                                continue;
                            }

                            // check the last char in the stack
                            char charStackLast = charStack.First().chr;
                            if (charStackLast == ')') {
                                continue;
                            } else if (charStackLast == 'h' || charStackLast == 'k') {
                                // skip if last symbol is not a digit
                                if (i < runLength - 1 && !char.IsDigit(cleanedText[i + 1])) {
                                    RevertText(textBox);
                                    return;
                                }

                                // evaluate
                                if (!evaluatePortion(i, applyOffset:true)) {
                                    RevertText(textBox);
                                    return; // return if failed
                                }
                            }
                        }
                        break;
                    default: // math symbol and other handling
                        // skip if currently parenthesized
                        if (charStack.Count <= 0 || charStack.First().chr == ')') {
                            continue;
                        }

                        // check if the curr char is a math symbol
                        char[] mathSymbols = ['+', '-', '*', '/'];
                        bool wasEvaluated = false;
                        foreach (char mathSymbol in mathSymbols) {
                            if (currChar == mathSymbol) {
                                // skip if last symbol is not a digit
                                if (i < runLength - 1 && !char.IsDigit(cleanedText[i + 1])) {
                                    RevertText(textBox);
                                    return;
                                }

                                // evaluate
                                if (!evaluatePortion(i + 1, applyOffset:false)) { // we dont want to include the math symbol itself
                                    RevertText(textBox);
                                    return; // return if failed
                                }
                                wasEvaluated = true;
                                break;
                            }
                        } 

                        // if math symbol was found, continue
                        if (wasEvaluated == true) {
                            continue;
                        }

                        // if the char isn't a (), h, k, math symbol or digit, revert
                        if (!char.IsDigit(currChar) && currChar != '.') {
                            RevertText(textBox);
                            return;
                        }
                        break;
                }
            }

            // eval the final value
            (bool succeeded, double finalResult) = evaluate(cleanedText); // eval
            if (!succeeded) {
                RevertText(textBox);
                return;
            }

            // rounding
            finalResult = ResultsRounding(finalResult);

            // set text and cleaned text
            textBox.Text = finalResult.ToString();

            // base validation
            base.Validate(sender, new());
        }

        #endregion
    }
}
