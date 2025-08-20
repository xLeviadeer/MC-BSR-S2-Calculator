using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {

    /// <summary>
    /// creates and holds events (actions) to watch for dispatch of
    /// </summary>
    internal class EventWatcher {

        // --- VARIABLES ---

        // - Watched Event Actions -

        /// <summary>
        /// A list of watched event actions
        /// </summary>
        private Dictionary<EventHandler<EventArgs>, bool> _watchedEventActions = new();

        // - All Events Ran Event -

        /// <summary>
        /// Event runs when all event actions have been run
        /// </summary>
        public event EventHandler<EventArgs>? AllEventsRan;

        // --- CONSTRUCTOR ---

        /// <summary>
        /// Create a new event watcher ("category" instance)
        /// </summary>
        /// <param name="completionAction"> The default completion action tied to this watcher </param>
        public EventWatcher(EventHandler<EventArgs> completionAction)
            => AllEventsRan += completionAction;

        // --- METHODS ---

        // - New Watch Action -

        /// <summary>
        /// Creates a new event action to watch for the event running
        /// </summary>
        /// <remarks>
        /// This should be on the right side of a subscription
        /// </remarks>
        /// <returns> An EventHandler action to be ran/subscribed </returns>
        public EventHandler<EventArgs> NewWatchAction() {
            EventHandler<EventArgs>? watchAction = null;
            watchAction = (_, _) => {
                _watchedEventActions[watchAction!] = true;
                if (_watchedEventActions.Values.All(flag => flag == true)) {
                    AllEventsRan?.Invoke(this, EventArgs.Empty);
                }
            };

            _watchedEventActions[watchAction] = false;
            return watchAction;
        }
    }
}
