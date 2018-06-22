using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Web.Configuration;
using System.Drawing;
using System.Configuration.Provider;
using System.IO;
using System.Web;
using System.Drawing.Imaging;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace Mediachase.Commerce.Engine.Images.Providers
{
    public class HandlerImageProvider : ImageProvider
    {
        private string _applicationName;
        private string _UrlFormatString;

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }


        /// <summary>
        /// Gets or sets the URL format string.
        /// </summary>
        /// <value>The URL format string.</value>
        public string UrlFormatString
        {
            get { return _UrlFormatString; }
            set { _UrlFormatString = value; }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        public override void Initialize(string name,
            NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "HandlerImageProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description",
                    "Handler Image provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            _applicationName = config["applicationName"];

            if (string.IsNullOrEmpty(_applicationName))
                _applicationName = "/";

            config.Remove("applicationName");

            // Initialize _path
            _UrlFormatString = config["urlFormatString"];

            if (String.IsNullOrEmpty(_UrlFormatString))
            {
                _UrlFormatString = "~/images/{0}.ashx";
            }

            config.Remove("urlFormatString");


            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }
        }

        /// <summary>
        /// Retrieves the image object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override Image RetrieveImage(string name)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string RetrieveImageUrl(string name)
        {
            return String.Format(UrlFormatString, name);
        }

        /// <summary>
        /// Retrieves the thumbnail image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string RetrieveThumbnailImageUrl(string name)
        {
            return String.Format(UrlFormatString, name + "-thumb");
        }

        /// <summary>
        /// Removes the image by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void RemoveImage(string name)
        {
        }

        /// <summary>
        /// Returns true if image with specified name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override bool Exists(string name)
        {
            return true;
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public override void SaveImage(string name, byte[] image)
        {
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public override void SaveImage(string name, Image image)
        {
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public override void SaveImage(string name, byte[] image, string contentType)
        {
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public override void SaveImage(string name, byte[] image, ImageFormat format)
        {
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public override void SaveImage(string name, Image image, string contentType)
        {
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public override void SaveImage(string name, Image image, ImageFormat format)
        {
        }

        /// <summary>
        /// Resolves the path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string ResolvePath(string name)
        {
            return GetPath(name, ResolveNameExt(name));
        }

        /// <summary>
        /// Resolves the name ext.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string ResolveNameExt(string name)
        {
            if (File.Exists(GetPath(name, "jpeg")))
                return "jpeg";
            else if (File.Exists(GetPath(name, "jpg")))
                return "jpg";
            else if (File.Exists(GetPath(name, "gif")))
                return "gif";
            else if (File.Exists(GetPath(name, "png")))
                return "png";
            else if (File.Exists(GetPath(name, "bmp")))
                return "bmp";
            else if (File.Exists(GetPath(name, "tif")))
                return "tif";
            else if (File.Exists(GetPath(name, "tiff")))
                return "tiff";

            return String.Empty;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ext">The ext.</param>
        /// <returns></returns>
        private string GetPath(string name, string ext)
        {
            return String.Format("{0}/{1}.{2}", _UrlFormatString, name, ext);
        }

        /// <summary>
        /// Gets the image format string.
        /// </summary>
        /// <param name="format">Image format.</param>
        /// <returns></returns>
        private string GetImageFormatString(ImageFormat format)
        {
            if (format.Equals(ImageFormat.Bmp))
                return "bmp";
            if (format.Equals(ImageFormat.Png))
                return "png";
            if (format.Equals(ImageFormat.Gif))
                return "gif";
            if (format.Equals(ImageFormat.Jpeg))
                return "jpg";
            if (format.Equals(ImageFormat.Tiff))
                return "tiff";

            return "jpg";
        }

        /// <summary>
        /// Gets the image format.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private ImageFormat GetImageFormat(string contentType)
        {
            if (contentType.Contains("gif"))
                return ImageFormat.Gif;

            if (contentType.Contains("png"))
                return ImageFormat.Png;

            if (contentType.Contains("jpeg") || contentType.Contains("jpg"))
                return ImageFormat.Jpeg;

            if (contentType.Contains("bmp"))
                return ImageFormat.Bmp;

            if (contentType.Contains("tiff") || contentType.Contains("tif"))
                return ImageFormat.Tiff;

            return ImageFormat.Jpeg;
        }
    }
}
