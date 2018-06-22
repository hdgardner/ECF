using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains collection of images.
    /// </summary>
    [DataContract]
    public partial class Images
    {
        /// <summary>Collection of files</summary>
        private Image[] _Image;

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public Image[] Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Catalog.Objects.Image"/> with the specified name.
        /// </summary>
        /// <value></value>
        public Image this[string name]
        {
            get
            {
                if (_Image != null)
                    foreach (Image image in _Image)
                    {
                        if (image.Name.ToLower().CompareTo(name.ToLower()) == 0)
                            return image;
                    }

                return null;
            }
        }
    }
}
