using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class DownloadFilterSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DownloadFilterCollection))]
        public DownloadFilterCollection Filters
        {
            get
            {
                return (DownloadFilterCollection)base[string.Empty];
            }
        }
    }
}
