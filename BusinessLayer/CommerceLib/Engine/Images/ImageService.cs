using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web;

namespace Mediachase.Commerce.Engine.Images
{
    /// <summary>
    /// Implements operations for and represents the image service.
    /// </summary>
    public class ImageService
    {
        private static ImageProvider _provider = null;
        private static ImageProviderCollection _providers = null;
        private static object _lock = new object();

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public ImageProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public ImageProviderCollection Providers
        {
            get { return _providers; }
        }

        /// <summary>
        /// Removes the image.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void RemoveImage(string name)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.RemoveImage(name);
        }

        /// <summary>
        /// Retrieves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Image RetrieveImage(string name)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.RetrieveImage(name);
        }

        /// <summary>
        /// Retrieves the thumbnail image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string RetrieveImageUrl(string name)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.RetrieveImageUrl(name);
        }

        /// <summary>
        /// Retrieves the thumbnail image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string RetrieveThumbnailImageUrl(string name)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.RetrieveThumbnailImageUrl(name);
        }

        /// <summary>
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool Exists(string name)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.Exists(name);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public static void SaveImage(string name, Image image)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public static void SaveImage(string name, byte[] image)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public static void SaveImage(string name, Image image, string contentType)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image, contentType);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public static void SaveImage(string name, byte[] image, string contentType)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image, contentType);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">The format.</param>
        public static void SaveImage(string name, Image image, ImageFormat format)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image, format);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">The format.</param>
        public static void SaveImage(string name, byte[] image, ImageFormat format)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.SaveImage(name, image, format);
        }

        /// <summary>
        /// Loads the providers.
        /// </summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <imageService> section
                        ImageServiceSection section = (ImageServiceSection)
                            ConfigurationManager.GetSection
                            ("FrameworkProviders/imageService");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new ImageProviderCollection();
                        ProvidersHelper.InstantiateProviders
                            (section.Providers, _providers,
                            typeof(ImageProvider));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException
                                ("Unable to load default ImageProvider");
                    }
                }
            }
        }
    }


}
