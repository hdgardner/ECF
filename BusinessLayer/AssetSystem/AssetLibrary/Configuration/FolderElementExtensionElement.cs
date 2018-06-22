using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class FolderElementExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        [ConfigurationProperty("mimeType", IsRequired = true, IsKey = true)]
        public String MimeType
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
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }

        }
    }
}
