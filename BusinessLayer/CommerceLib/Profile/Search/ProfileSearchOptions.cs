using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Profile.Search
{
    /// <summary>
    /// Implements operations for the profile search options. (Inherits <see cref="SearchOptions"/>.)
    /// </summary>
    public class ProfileSearchOptions : SearchOptions
    {
        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSearchOptions"/> class.
        /// </summary>
        public ProfileSearchOptions() : base()
        {
            Namespace = "Mediachase.Commerce.Profile";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileSearchOptions"/> class.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        public ProfileSearchOptions(ProfileSearchOptions searchOptions)
            : base(searchOptions)
        {
            Namespace = "Mediachase.Commerce.Profile";
        }
    }
}
