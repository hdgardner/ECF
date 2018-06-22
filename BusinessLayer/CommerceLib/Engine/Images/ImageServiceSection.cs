using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Engine.Images
{

    /// <summary>
    /// Implements operations for the image service section. (Inherits <see cref="ConfigurationSection"/>.)
    /// </summary>
    public class ImageServiceSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        /// <summary>
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider",
            DefaultValue = "CachedImageProvider")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }

}
