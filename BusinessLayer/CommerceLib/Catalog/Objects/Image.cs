using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains information about an image.
    /// </summary>
    [DataContract]
    public partial class Image
    {
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Url;

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

      
        private string _Height;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "nonNegativeInteger")]
        public string Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        /// <remarks/>        
        private string _Width;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "nonNegativeInteger")]
        public string Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        /// <remarks/>
        private string _ThumbnailUrl;

        /// <summary>
        /// Gets or sets the thumbnail URL.
        /// </summary>
        /// <value>The thumbnail URL.</value>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ThumbnailUrl
        {
            get { return _ThumbnailUrl; }
            set { _ThumbnailUrl = value; }
        }
        
        private string _ThumbnailHeight;

        /// <summary>
        /// Gets or sets the height of the thumbnail.
        /// </summary>
        /// <value>The height of the thumbnail.</value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "nonNegativeInteger")]
        public string ThumbnailHeight
        {
            get { return _ThumbnailHeight; }
            set { _ThumbnailHeight = value; }
        }

       
        private string _ThumbnailWidth;

        /// <summary>
        /// Gets or sets the width of the thumbnail.
        /// </summary>
        /// <value>The width of the thumbnail.</value>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "nonNegativeInteger")]
        public string ThumbnailWidth
        {
            get { return _ThumbnailWidth; }
            set { _ThumbnailWidth = value; }
        }
    }
}
