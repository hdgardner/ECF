using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Implements operations for the node event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class NodeEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public NodeEventArgs(string eventName) : base(eventName) { }
    }
}
