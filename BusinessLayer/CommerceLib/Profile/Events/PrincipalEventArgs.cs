using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Profile.Events
{
    /// <summary>
    /// Implements operations for the principal event arguments. (Inherits <see cref="FrameworkEventArgs"/>.)
    /// </summary>
    public class PrincipalEventArgs : FrameworkEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalEventArgs"/> class.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public PrincipalEventArgs(string eventName) : base(eventName) { }
    }
}
