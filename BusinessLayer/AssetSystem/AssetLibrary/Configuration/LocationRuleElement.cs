using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class LocationRuleElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        [ConfigurationProperty("extension")]
        public String Extension
        {
            get
            {
                return (string)this["extension"];
            }
            set
            {
                this["extension"] = value;
            }

        }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        [ConfigurationProperty("mimeType")]
        public string MimeType
        {
            get
            {
                return (string)this["mimeType"];
            }
            set
            {
                this["mimeType"] = value;
            }

        }

        /// <summary>
        /// Gets or sets the size of the max.
        /// </summary>
        /// <value>The size of the max.</value>
        [ConfigurationProperty("maxSize")]
        public string MaxSize
        {
            get
            {
                return (string)this["maxSize"];
            }
            set
            {
                this["maxSize"] = value;
            }

        }

        /// <summary>
        /// Gets or sets the storage provider.
        /// </summary>
        /// <value>The storage provider.</value>
        [ConfigurationProperty("storageProvider")]
        public string StorageProvider
        {
            get
            {
                return (string)this["storageProvider"];
            }
            set
            {
                this["storageProvider"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the download profile.
        /// </summary>
        /// <value>The download profile.</value>
        [ConfigurationProperty("downloadProfile")]
        public string DownloadProfile
        {
            get
            {
                return (string)this["downloadProfile"];
            }
            set
            {
                this["downloadProfile"] = value;
            }
        }
    }
}
