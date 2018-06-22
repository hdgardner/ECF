using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Engine.Events
{
    /// <summary>
    /// Implements operations for the framework event arguments. (Inherits <see cref="System.EventArgs"/>.)
    /// </summary>
    public class FrameworkEventArgs : System.EventArgs
    {
        private string _EventName = String.Empty;
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName
        {
            get
            {
                return _EventName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public FrameworkEventArgs(string eventName)
        {
            _EventName = eventName;
        }
    }
}
