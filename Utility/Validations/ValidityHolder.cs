using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MC_BSR_S2_Calculator.Utility.Validations {

    /// <summary>
    /// Interface for classes which have an IsValid function
    /// </summary>
    public interface IValidityHolder {
        public event EventHandler<BoolEventArgs> ValidityChanged;

        public bool CheckValidity();
    }

    /// <summary>
    /// A single validity holder that is valid/invalid and enabled/disabled
    /// </summary>
    public class SingleValidityHolder : IValidityHolder {

        // - validity changed event -

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // - is valid -

        private bool _isValid { get; set; } = false;

        public bool IsValid {
            get => _isValid;
            set {
                if (_isValid != value) {
                    _isValid = value;
                    ValidityChanged?.Invoke(this, new(value));
                }
            }
        }

        // - is enabled -

        private bool _isEnabled { get; set; } = true;

        public bool IsEnabled {
            get => _isEnabled;
            set {
                if (_isEnabled != value) {
                    _isEnabled = value;
                    ValidityChanged?.Invoke(this, new(value));
                }
            }
        }

        // - IsValid exposure -

        public bool CheckValidity() {
            if (IsEnabled) {
                return IsValid;
            } else {
                return true;
            }
        }
    }

    /// <summary>
    /// Holds multiple single validity holders for validity checking
    /// </summary>
    public class ValidityHolder : IValidityHolder, IEnumerable<KeyValuePair<string, SingleValidityHolder>> {

        // --- VARIABLES ---

        // - Holder -

        // dictionary

        private Dictionary<string, SingleValidityHolder> Validity { get; set; }

        // indexing

        public SingleValidityHolder this[string key] {
            get => Validity[key];
            set {
                if (Validity[key] != value) {
                    // set value
                    Validity[key] = value;

                    // check validity and trigger event if changed
                    bool isValid = CheckValidity();
                    if (IsValid != isValid) {
                        IsValid = isValid;
                        if (DoValidityChangedInvocation) { // only send events after construction has finished
                            ValidityChanged?.Invoke(this, new(isValid));
                        }
                    }
                }
            }
        }

        // - Is Valid - 

        public bool IsValid { get; set; }

        // - Validity Changed Event -

        private bool DoValidityChangedInvocation { get; set; } = false;

        public event EventHandler<BoolEventArgs>? ValidityChanged;

        // --- CONSTRUCTOR --

        public ValidityHolder(Dictionary<string, SingleValidityHolder> validity) {
            // set dictionary without sending events
            Validity = validity;

            // allow events
            DoValidityChangedInvocation = true;

            // set inital validity
            IsValid = CheckValidity();
        }

        // --- METHODS ---

        // - IsValid Exposure -

        public bool CheckValidity() {
            // get validity
            return (Validity.Values.All(validityHolder => validityHolder.CheckValidity()));
        }

        // - Copy -

        /// <summary>
        /// Copies validity so that it can be changed, however elemnts still refer to the same objects
        /// </summary>
        /// <returns> a new validity holder </returns>
        public ValidityHolder SoftCopy()
            => new ValidityHolder(this.Validity.ToDictionary());

        // - Remove -

        public void Remove(string key) => Validity.Remove(key);

        // - foreach support -

        public IEnumerator<KeyValuePair<string, SingleValidityHolder>> GetEnumerator() {
            return Validity.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
