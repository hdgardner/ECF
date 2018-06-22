using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    class FileLibraryLocationSection: ConfigurationSection
    {

        /// <summary>
        /// Gets the locations.
        /// </summary>
        /// <value>The locations.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(FileLibraryLocationCollection))]
        public FileLibraryLocationCollection Locations
        {
            get
            {
                return (FileLibraryLocationCollection)base[string.Empty];
            }
        }
     }
}
