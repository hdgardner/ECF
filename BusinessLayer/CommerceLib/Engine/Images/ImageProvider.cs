using System;
using System.Configuration.Provider;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mediachase.Commerce.Engine.Images
{
    /// <summary>
    /// Image provider used for implementing extensible provider model. It will be used
    /// to persist and retrieve images. One existing provider distributed with ECF is FileImageProvider
    /// which uses file system to store images.
    /// </summary>
    public abstract class ImageProvider : ProviderBase
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public abstract string ApplicationName { get; set; }

        // Methods
        /// <summary>
        /// Retrieves the image object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract Image RetrieveImage(string name);
        /// <summary>
        /// Retrieves the image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract string RetrieveImageUrl(string name);
        /// <summary>
        /// Retrieves the thumbnail image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract string RetrieveThumbnailImageUrl(string name);
        /// <summary>
        /// Removes the image by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void RemoveImage(string name);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public abstract void SaveImage(string name, Image image);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public abstract void SaveImage(string name, byte[] image);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public abstract void SaveImage(string name, Image image, string contentType);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public abstract void SaveImage(string name, byte[] image, string contentType);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public abstract void SaveImage(string name, Image image, ImageFormat format);
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public abstract void SaveImage(string name, byte[] image, ImageFormat format);
        /// <summary>
        /// Returns true if image with specified name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract bool Exists(string name);
    }
}
