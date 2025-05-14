using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {
    public class StringTextBox : TypedTextBox<string> {
        // --- METHODS ---

        public override void Validate(object? sender, EventArgs args) {
            // type cast sender
            if (sender == null) {
                IsValid = false;
                throw new ArgumentNullException(nameof(sender));
            }
            var textBox = (StringTextBox)sender;

            // catch empty as default
            if (textBox.Text == "") {
                IsValid = null;
                return;
            }

            // letters only
            if (!Regex.IsMatch(textBox.Text, "^[A-Za-z.' -]+$")) {
                IsValid = false;
                return;
            }

            // run base
            base.Validate(sender, args);
        }
    }
}
