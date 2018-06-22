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
    /// <summary>
    /// Stores images in the file system.
    /// </summary>
    class FileImageProvider : ImageProvider
    {
        private static object _Sync = new object();
        private string _applicationName;
        private string _storagePath;
        private string _storageUrl; // used only when HttpContext is null
        private string _storageUrlPath; // used only when HttpContext is null
        private int _duration;
        private bool _cacheImages;
        //private static string _ApplicationPath = HttpContext.Current.Request.ApplicationPath;
        private string _StoragePhysicalPath;

        private static ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

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
        /// Gets or sets the storage path.
        /// </summary>
        /// <value>The storage path.</value>
        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        /// <summary>
        /// Gets or sets the storage url.
        /// </summary>
        /// <value>The storage path.</value>
        public string StorageUrl
        {
            get { return _storageUrl; }
            set { _storageUrl = value; }
        }

        /// <summary>
        /// Gets or sets the storage url path.
        /// </summary>
        /// <value>The storage url path.</value>
        public string StorageUrlPath
        {
            get { return _storageUrlPath; }
            set { _storageUrlPath = value; }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration
        {
            get { return _duration; }
            set { _duration = value; }
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
                name = "FileImageProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description",
                    "Cached image provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            _applicationName = config["applicationName"];

            if (string.IsNullOrEmpty(_applicationName))
                _applicationName = "/";

            config.Remove("applicationName");

            // Initialize _path
            _storagePath = config["storagePath"];

            if (String.IsNullOrEmpty(_storagePath))
                throw new ProviderException
                    ("Empty or missing storagePath");

            config.Remove("storagePath");

            if (String.IsNullOrEmpty(_storagePath))
                throw new ProviderException("Empty StoragePath string");

            if (HttpContext.Current != null)
                _StoragePhysicalPath = HttpContext.Current.Server.MapPath("~/" + StoragePath);
            else
            {
                _storagePath = StoragePath;

                _storageUrl = config["storageUrl"];

                if (String.IsNullOrEmpty(_storageUrl))
                    throw new ProviderException("Empty or missing storageUrl");

                config.Remove("storageUrl");

                _storageUrlPath = config["storageUrlPath"];

                if (String.IsNullOrEmpty(_storageUrlPath))
                    throw new ProviderException("Empty or missing storageUrlPath");

                config.Remove("storageUrlPath");

                _StoragePhysicalPath = _storageUrlPath;
            }

            _cacheImages = Boolean.Parse(config["cacheImages"]);
            config.Remove("cacheImages");

            // Initialize _applicationName
            string duration = config["duration"];

            if (string.IsNullOrEmpty(duration))
                _duration = 300; // 5 min
            else
                _duration = Int32.Parse(duration);

            config.Remove("duration");


            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

            if (!FileImageProvider.HasWriteAccess(new DirectoryInfo(_StoragePhysicalPath)))
                throw new ProviderException
                      ("ImageServiceProvider Failed to initialize. Must have read/write permissions to the following directory: " + _StoragePhysicalPath);

            // Clean up the cache directory
            CleanCachedImages();
        }

        /// <summary>
        /// Retrieves the image object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override Image RetrieveImage(string name)
        {
            _cacheLock.EnterReadLock();
            Image image = null;

            try
            {
                if (Exists(name))
                    image = Image.FromFile(ResolvePath(name));
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }

            return image;
        }

        /// <summary>
        /// Retrieves the image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string RetrieveImageUrl(string name)
        {
            return String.Format("{0}/{1}.{2}", ResolveUrl(StoragePath, StorageUrl), name, ResolveNameExt(name));
        }

        /// <summary>
        /// Retrieves the thumbnail image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string RetrieveThumbnailImageUrl(string name)
        {
            return String.Format("{0}/{1}.{2}", ResolveUrl(StoragePath, StorageUrl), name + "-thumb", ResolveNameExt(name));
        }


        /// <summary>
        /// Removes the image by name.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void RemoveImage(string name)
        {
            string path = ResolvePath(name);
            if (File.Exists(path))
            {
                _cacheLock.EnterWriteLock();
                try
                {
                    File.Delete(path);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Returns true if image with specified name already exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override bool Exists(string name)
        {
            string path = GetPath(name, "jpeg");
            if (File.Exists(path))
                return false;

            return !IsExpired(path);
        }

        /// <summary>
        /// Removes old images from the storage directory.
        /// </summary>
        private void CleanCachedImages()
        {
            _cacheLock.EnterWriteLock();
            try
            {
                string[] files = Directory.GetFiles(_StoragePhysicalPath, "*", SearchOption.TopDirectoryOnly);

                if (files == null)
                    return;

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    string ext = fileInfo.Extension.ToLower();
                    if (this.IsExpired(file) && (ext == ".jpeg" || ext == ".gif" || ext == ".jpg" || ext == ".png" || ext == ".bmp" || ext == ".tif" || ext == ".tiff"))
                    {
                        File.Delete(file);
                    }
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Determines whether the specified path is expired.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified path is expired; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExpired(string path)
        {
            if (File.GetLastWriteTime(path).AddSeconds(Duration) < DateTime.Now)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public override void SaveImage(string name, byte[] image)
        {
            SaveImage(name, image, "images/jpg");
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        public override void SaveImage(string name, Image image)
        {
            SaveImage(name, image, "images/jpg");
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public override void SaveImage(string name, byte[] image, string contentType)
        {
            MemoryStream stream = new MemoryStream(image);
            SaveImage(name, Image.FromStream(stream), contentType);
            stream.Close();
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public override void SaveImage(string name, byte[] image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream(image);
            SaveImage(name, Image.FromStream(stream), format);
            stream.Close();
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="contentType">Type of the content.</param>
        public override void SaveImage(string name, Image image, string contentType)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                {
                    ImageFormat format = GetImageFormat(contentType);
                    string path = GetPath(name, format.ToString());

                    if (File.Exists(path))
                    {
                        try 
                        {
                            File.Delete(path);
                        }
                        catch (Exception) {}
                    }

                    try
                    {
                        image.Save(path, format);
                    }
                    catch (Exception)
                    {
                        //throw new ProviderException("ImageServiceProvider Failed. Check the writes permissions to the following directory: " + _StoragePhysicalPath);
                    }
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="image">The image.</param>
        /// <param name="format">Image format.</param>
        public override void SaveImage(string name, Image image, ImageFormat format)
        {

            _cacheLock.EnterWriteLock();
            try
            {
                string path = GetPath(name, GetImageFormatString(format));

                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                    }                
                }
                try
                {
                    image.Save(path, format);
                }
                catch (Exception)
                {
                    //throw new ProviderException("ImageServiceProvider Failed. Check the writes permissions to the following directory: " + _StoragePhysicalPath);
                }
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
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
            return String.Format("{0}\\{1}.{2}", _StoragePhysicalPath, name, ext);
        }

        /// <summary>
        /// Resolves the URL.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private static string ResolveUrl(string path, string url)
        {
            if (HttpContext.Current != null)
            {
                HttpRequest request = HttpContext.Current.Request;
                StringBuilder sb = new StringBuilder();
                sb.Append(request.Url.Scheme);
                sb.Append("://");
                sb.Append(request.Url.Host);

                if (request.Url.Port != 80)
                    sb.Append(":" + request.Url.Port);

                if (request.ApplicationPath.Length > 0)
                    sb.Append(request.ApplicationPath + "/" + path);
                else
                    sb.Append(path);

                return sb.ToString();
            }
            else
            {
                string tmpUrl = String.Empty;
                int index = url.LastIndexOf('/');
                if (index > 0)
                    tmpUrl = url.Substring(0, index+1);

                return tmpUrl + path;
            }
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

        /// <summary>
        /// Determines whether [has write access] [the specified index dir].
        /// </summary>
        /// <param name="indexDir">The index dir.</param>
        /// <returns>
        /// 	<c>true</c> if [has write access] [the specified index dir]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasWriteAccess(DirectoryInfo indexDir)
        {
            string tempFileName = Path.Combine(indexDir.FullName, Guid.NewGuid().ToString());
            //Yuck! but it is the simplest way
            try
            {
                File.CreateText(tempFileName).Close();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            try
            {
                File.Delete(tempFileName);
            }
            catch (UnauthorizedAccessException)
            {
                //we may have permissions to create but not delete, ignoring
            }
            return true;
        }
    }
}
