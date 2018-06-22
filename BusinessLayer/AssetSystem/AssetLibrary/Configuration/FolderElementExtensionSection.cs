using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class FolderElementExtensionSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the element types.
        /// </summary>
        /// <value>The element types.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(FolderElementExtensionCollection))]
        public FolderElementExtensionCollection ElementTypes
        {
            get
            {
                return (FolderElementExtensionCollection)base[string.Empty];
            }
        }
    }
}
