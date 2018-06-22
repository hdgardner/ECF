using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Implements operations for the catalog event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class CatalogEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public CatalogEventArgs(string eventName) : base(eventName) { }
    }
}
