using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Core.Log
{
    /// <summary>
    /// Implements operations for the profile search options. (Inherits <see cref="SearchOptions"/>.)
    /// </summary>
    public class ApplicationLogOptions : SearchOptions
    {
        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogOptions"/> class.
        /// </summary>
        public ApplicationLogOptions() : base()
        {
            Namespace = "Mediachase.Commerce.Core";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogOptions"/> class.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        public ApplicationLogOptions(ApplicationLogOptions searchOptions)
            : base(searchOptions)
        {
            Namespace = "Mediachase.Commerce.Core";
        }
    }
}
