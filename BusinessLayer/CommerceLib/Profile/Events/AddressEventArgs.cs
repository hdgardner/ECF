using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Profile.Events
{
    /// <summary>
    /// Implements operations for the address event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class AddressEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public AddressEventArgs(string eventName) : base(eventName) { }
    }
}
