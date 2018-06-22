using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class LocationRuleCollections : ConfigurationElementCollection
    {

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LocationRuleElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            LocationRuleElement locationEl = (LocationRuleElement)element;
            return locationEl.Extension + locationEl.MimeType 
                   + locationEl.MaxSize + locationEl.StorageProvider;
        }
    }
}
