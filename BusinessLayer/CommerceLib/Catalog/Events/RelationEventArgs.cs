using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Implements operations for the relation event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class RelationEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public RelationEventArgs(string eventName) : base(eventName) { }
    }
}
