using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Represents the arguments of the association event and inherits the <see cref="FrameworkEventArgs"/> class.
    /// </summary>
    public class AssociationEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public AssociationEventArgs(string eventName) : base(eventName) { }
    }
}
