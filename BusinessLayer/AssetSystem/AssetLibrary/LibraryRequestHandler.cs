using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using Mediachase.Ibn.Blob;

namespace Mediachase.Ibn.Library
{
    public class LibraryRequestHandler : IHttpHandler
    {

        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            string libPath = HttpUtility.UrlDecode(context.Request.Url.AbsolutePath);
            string appPath = context.Request.ApplicationPath;
            appPath = appPath.TrimEnd(new char[] { '/' });

            if (!String.IsNullOrEmpty(appPath))
                 libPath = libPath.Substring(appPath.Length + 1);
                        
            int index = libPath.LastIndexOf('/');

            if (index != -1)
            {
                String elemName = libPath.Substring(index + 1);
                //remove iis extension from element name
                elemName = elemName.Substring(0, elemName.LastIndexOf('.'));
                libPath = libPath.Substring(0, index);
                FolderElement[] elements = FolderElement.GetElementsByPath(libPath);

                if(elements != null)
                {                                                  
                    foreach (FolderElement element in elements)
                    {
                        /*
                        if (provider is PublicDiskStorageProvider)
                            downloadProfile = BaseBlobProfile.GetAccessProfileByCfgName("iis");
                         * */

                        if (elemName.Equals(element.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                //"open" - is DonwloadProfile in Web.config
                                BaseBlobProfile downloadProfile = BaseBlobProfile.GetAccessProfileByCfgName("open");

                                //If public disk provider then redirect
                                BlobStorageProvider provider = BlobStorage.Providers[element.BlobStorageProvider];

                                BlobInfo blobInfo = element.GetBlobInfo();
                                string profileName = String.Empty;
                                string providerName = String.Empty;
                                if (FolderElement.TryRecognizeStorageProvider(element, blobInfo.FileName, blobInfo.ContentType, blobInfo.ContentSize, out providerName, out profileName))
                                {
                                    if (!String.IsNullOrEmpty(profileName))
                                        downloadProfile = BaseBlobProfile.GetAccessProfileByCfgName(profileName);
                                }

                                String filterName = context.Request[FolderElement.filterParam];

                                //Download filter impl
                                if(!String.IsNullOrEmpty(filterName))
                                {
                                    DownloadFilterBase filter = DownloadFilterBase.GetFilterByCfgName(filterName);
                                    if(filter != null)
                                    {
                                        filter.Initialize(context.Request.Params);
                                        //subscribe to filter event
                                        downloadProfile.FilterEvent += filter.ProcessFilter;
                                    }
                                }
                              
                                //Begin download
                                downloadProfile.ProcessRequest(context, blobInfo);
                            }
                            catch (BlobDownloadException e)
                            {
                                if (e.Code == 204) //No content
                                {
                                    context.Response.ClearContent();
                                    context.Response.AddHeader("Content-Length", "0");
                                }
                                //throw new HttpException(e.Code, e.Message);
                                context.Response.StatusCode = e.Code;
                                context.Response.StatusDescription = e.Message;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    };
}
