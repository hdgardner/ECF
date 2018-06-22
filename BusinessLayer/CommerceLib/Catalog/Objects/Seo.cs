using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Implements operations for and represents the catalog object Seo.
    /// </summary>
    [DataContract]
    public partial class Seo
    {
        private string _Title;
        private string _Description;
        private string _Keywords;
        private string _Uri;
        private string _LanguageCode;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri
        {
            get
            {
                return _Uri;
            }
            set
            {
                _Uri = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get
            {
                return _Keywords;
            }
            set
            {
                _Keywords = value;
            }
        }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
            }
        }
    }
}
