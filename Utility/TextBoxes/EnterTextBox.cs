using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MC_BSR_S2_Calculator.Utility.TextBoxes {
    public class EnterTextBox : TextBox {

        // --- VARIABLES ---

        public event EventHandler<KeyEventArgs>? KeyDownEnter;

        public event EventHandler<KeyEventArgs>? KeyUpEnter;

        // --- CONSTRUCTOR ---

        public EnterTextBox() {
            // enter down
            KeyDown += (object sender, KeyEventArgs args) => {
                if (args.Key == Key.Enter) {
                    KeyDownEnter?.Invoke(this, args);
                }
            };

            // enter up
            KeyUp += (object sender, KeyEventArgs args) => {
                if (args.Key == Key.Enter) {
                    KeyUpEnter?.Invoke(this, args);
                }
            };
        }
    }
}
