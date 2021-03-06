using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Implements operations for the entry event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class EntryEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public EntryEventArgs(string eventName) : base(eventName) { }
    }
}
