using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Mediachase.Cms;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.ResourceHandler
{
    public class FileSystemResourceProvider : IResourceProvider
    {
        #region Repository
        private string _repository;
        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>The repository.</value>
        public string Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                _repository = value;
            }
        }
        #endregion

        #region Archive
        private string _archive;
        /// <summary>
        /// Gets or sets the archive.
        /// </summary>
        /// <value>The archive.</value>
        public string Archive
        {
            get
            {
                return _archive;
            }
            set
            {
                _archive = value;
            }
        }

        #endregion

        #region Temp
        private string _temp;
        /// <summary>
        /// Gets or sets the temp.
        /// </summary>
        /// <value>The temp.</value>
        public string Temp
        {
            get
            {
                return _temp;
            }
            set
            {
                _temp = value;
            }
        }
        #endregion

        #region ctor: FileSystemResourceProvider
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FileSystemResourceProvider"/> class.
        /// </summary>
        /// <param name="Repository">The repository directory.</param>
        /// <param name="Archive">The archive directory.</param>
        /// <param name="Temp">The temp directory.</param>
        public FileSystemResourceProvider(string Repository, string Archive, string Temp)
        {
            this.Repository = Repository;
            this.Archive = Archive;
            this.Temp = Temp;
        }
        #endregion

        #region IResourceProvider Members

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            //get application path
            string appPath = request.ApplicationPath;
            if (appPath != "/")
            {
                appPath += "/";
            }

            //get resource folders paths
            string repPath = appPath + Repository + "/";
            string archPath = appPath + Archive + "/";
            string resPath = appPath + "Repository/Private/";


            // get requested file path
            string filePath = request.FilePath.ToUpper();
            //if (filePath.Contains("APP_THEME") || filePath.Contains("ECF\\MASTERTEMPLATES") || filePath.Contains("PUBLIC")) return;
            if (filePath.Contains("ECF\\MASTERTEMPLATES")) return;

            if (CMSContext.Current.VersionId != -1)
            {
                if (filePath.StartsWith(resPath.ToUpper()))
                {
                    if (CMSContext.Current.VersionId != -2)
                    {
                        filePath = filePath.Replace(resPath.ToUpper(), archPath + CMSContext.Current.VersionId.ToString() + "/");
                    }
                    else
                    {
                        filePath = filePath.Replace(resPath.ToUpper(), archPath + ProfileContext.Current.User.UserName + "/");
                    }
                }
            }

            try
            {
                response.WriteFile(request.MapPath(filePath));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Checks the version folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="archPath">The arch path.</param>
        private void CheckVersionFolder(HttpRequest request, string archPath)
        {
            DirectoryInfo verDir = new DirectoryInfo(request.MapPath(archPath + CMSContext.Current.VersionId.ToString() + "/"));
            if (!verDir.Exists)
            {
                verDir.Create();
            }
        }
        #endregion
    }
}
