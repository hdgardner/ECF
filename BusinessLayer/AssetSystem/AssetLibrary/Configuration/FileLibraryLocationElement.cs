using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn.Library.Configuration
{
    public class FileLibraryLocationElement : ConfigurationElement
    {

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
      [ConfigurationProperty("path", DefaultValue = "*", IsRequired = true, IsKey = true),
      StringValidator(MinLength = 1)]
        public String Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }

        }

      /// <summary>
      /// Gets the location rules.
      /// </summary>
      /// <value>The location rules.</value>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(LocationRuleCollections))]
        public LocationRuleCollections LocationRules
        {
            get
            {
                return (LocationRuleCollections)this[string.Empty];
            }
        }

    }
}
