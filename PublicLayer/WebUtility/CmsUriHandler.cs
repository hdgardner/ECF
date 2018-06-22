using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms;
using System.Data;
using System.Web.Hosting;
using System.Globalization;
using System.Web.Security;

namespace Mediachase.Cms.WebUtility
{
    public class CmsUriHandler : IHttpHandlerFactory
    {
        /// <summary>
        /// Returns an instance of a class that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An instance of the <see cref="T:System.Web.HttpContext"></see> class that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        /// <param name="requestType">The HTTP data transfer method (GET or POST) that the client uses.</param>
        /// <param name="url">The <see cref="P:System.Web.HttpRequest.RawUrl"></see> of the requested resource.</param>
        /// <param name="pathTranslated">The <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath"></see> to the requested resource.</param>
        /// <returns>
        /// A new <see cref="T:System.Web.IHttpHandler"></see> object that processes the request.
        /// </returns>
        public IHttpHandler GetHandler(
            HttpContext context, string requestType, string url, string pathTranslated)
        {
            if (url.EndsWith("logout.aspx"))
            {
                FormsAuthentication.SignOut();
                context.Response.Redirect(CMSContext.Current.ResolveUrl("~"));
                context.Response.End();
            }

            string outline = url;

            if (CMSContext.Current.AppPath.Length != 1)
                outline = outline.Substring(CMSContext.Current.AppPath.Length);

            // Outline must start with "/", so add it if we are missing
            if (!outline.StartsWith("/", StringComparison.Ordinal))
            {
                outline = "/" + outline;
            }

            // Set the outline
            CMSContext.Current.Outline = outline;

            // Load current outline
            bool isFolder = false;
            int folderId = 0;
            int pageId = -1;
            string masterFile = String.Empty;

            string cacheKey = CmsCache.CreateCacheKey("filetree", CMSContext.Current.SiteId.ToString(), outline);

            DataTable fileTreeItemTable = null;

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
                fileTreeItemTable = (DataTable)cachedObject;

            if (fileTreeItemTable == null)
            {
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        fileTreeItemTable = (DataTable)cachedObject;
                    }
                    else
                    {
                        fileTreeItemTable = FileTreeItem.GetItemByOutlineAllDT(outline, CMSContext.Current.SiteId);

                        if (fileTreeItemTable.Rows.Count > 0)
                        {
                            CmsCache.Insert(cacheKey, fileTreeItemTable, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                        }
                    }
                }
            }

            if (fileTreeItemTable != null && fileTreeItemTable.Rows.Count > 0)
            {
                DataRow row = fileTreeItemTable.Rows[0];
                folderId = (int)row[0];
                CMSContext.Current.PageId = folderId;
                isFolder = (bool)row[4];
                masterFile = (string)row[8];
                if (String.Compare(outline, (string)row[2], true, CultureInfo.InvariantCulture) == 0)
                {
                    pageId = (int)row[0];
                }
            }

            /*
            using (IDataReader reader = FileTreeItem.GetItemByOutlineAll(outline, CMSContext.Current.SiteId))
            {
                if (reader.Read())
                {
                    folderId = reader.GetInt32(0);
                    CMSContext.Current.PageId = folderId;
                    isFolder = reader.GetBoolean(4);
                    masterFile = reader.GetString(8);
                    if (String.Compare(outline, reader.GetString(2), true, CultureInfo.InvariantCulture) == 0)
                    {
                        pageId = reader.GetInt32(0);                        
                    }
                }

                reader.Close();
            }
             * */

            //if (FileTreeItem.IsVirtual(outline))
            if (pageId != -1) // is this folder/page virtual?
            {
                // If URL is not rewritten, then save the one originally requested
                if (!CMSContext.Current.IsUrlReWritten)
                {
                    CMSContext.Current.IsUrlReWritten = true;
                    if (HttpContext.Current.Request.QueryString.Count == 0)
                        CMSContext.Current.CurrentUrl = url;
                    else
                        CMSContext.Current.CurrentUrl = String.Format(url + "?" + HttpContext.Current.Request.QueryString);
                }

                /*
                bool isFolder = false;
                int folderId = 0;
                string masterFile = String.Empty;

                using (IDataReader reader = FileTreeItem.GetItemByOutlineAll(outline, CMSContext.Current.SiteId))
                {
                    if (reader.Read())
                    {
                        folderId = reader.GetInt32(0);
                        isFolder = reader.GetBoolean(4);
                        masterFile = reader.GetString(8);
                    }
                }
                 * */

                if (!isFolder)
                {
                    Uri rawUrl = BuildUri(String.Format("~/template.aspx"), HttpContext.Current.Request.IsSecureConnection);
                    string filePath;
                    string sendToUrlLessQString;
                    string sendToUrl = rawUrl.PathAndQuery;
                    RewriteUrl(context, sendToUrl, out sendToUrlLessQString, out filePath);

                    return PageParser.GetCompiledPageInstance(sendToUrlLessQString, filePath, context);
                }
                else
                {
                    string newUrl = String.Empty;
                    //try to find default page for folder
                    using (IDataReader reader = FileTreeItem.GetFolderDefaultPage(folderId))
                    {
                        if (reader.Read())
                        {
                            newUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(reader.GetString(2));
                        }
                        else
                        {
                            reader.Close();
                            throw new HttpException(204, "Default page for folder not found");
                        }

                        reader.Close();
                    }

                    Uri rawUrl = BuildUri(newUrl, HttpContext.Current.Request.IsSecureConnection);
                    string filePath;
                    string sendToUrlLessQString;
                    string sendToUrl = rawUrl.PathAndQuery;
                    RewriteUrl(context, sendToUrl, out sendToUrlLessQString, out filePath);

                    return PageParser.GetCompiledPageInstance(sendToUrlLessQString, filePath, context);
                }
            }
            else if (!string.IsNullOrEmpty(pathTranslated))
            {
                return PageParser.GetCompiledPageInstance(url, pathTranslated, context);
            }
            else
            {
                return
                    PageParser.GetCompiledPageInstance(
                        url, context.Server.MapPath("~"), context);
            }
        }


        /// <summary>
        /// Enables a factory to reuse an existing handler instance.
        /// </summary>
        /// <param name="handler">The <see cref="T:System.Web.IHttpHandler"></see> object to reuse.</param>
        public void ReleaseHandler(IHttpHandler handler)
        {
        }

        /// <summary>
        /// Builds a Uri that refers to target.  If target is application-relative a relative
        /// Uri is created.
        /// </summary>
        /// <param name="target">Target for uri.</param>
        /// <param name="secure">Indicates whether Uri should be https or http.</param>
        /// <returns>Uri for <paramref name="target"/></returns>
        public Uri BuildUri(string target, bool secure)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            // Check to see if target is an application-relative URL.
            if (target.StartsWith("~/", StringComparison.Ordinal))
            {
                string newTarget = HttpContext.Current.Request.ApplicationPath;
                if (!newTarget.EndsWith("/", StringComparison.Ordinal))
                {
                    newTarget += "/";
                }
                newTarget += target.Substring("~/".Length);
                target = newTarget;
            }
            Uri targetUri =
                new Uri(
                    HttpContext.Current.Request.Url, new Uri(target, UriKind.RelativeOrAbsolute));
            UriBuilder builder = new UriBuilder(targetUri);
            builder.Scheme = (secure)
                                 ? Uri.UriSchemeHttps
                                 : Uri.UriSchemeHttp;
            builder.Port = -1; // Removes the port setting from the site.
            return builder.Uri;
        }


        /// <summary>
        /// Rewrites the URL.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sendToUrl">The send to URL.</param>
        /// <param name="sendToUrlLessQString">The send to URL less Q string.</param>
        /// <param name="filePath">The file path.</param>
        private void RewriteUrl(
            HttpContext context,
            string sendToUrl,
            out string sendToUrlLessQString,
            out string filePath)
        {
            // see if we need to add any extra querystring information
            if (context.Request.QueryString.Count > 0)
            {
                if (sendToUrl.IndexOf('?') != -1)
                {
                    sendToUrl += "&" + context.Request.QueryString.ToString();
                }
                else
                {
                    sendToUrl += "?" + context.Request.QueryString.ToString();
                }
            }

            // first strip the querystring, if any
            string queryString = String.Empty;
            sendToUrlLessQString = sendToUrl;
            if (sendToUrl.IndexOf('?') > 0)
            {
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
                queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
            }

            // grab the file's physical path
            filePath = context.Server.MapPath(sendToUrlLessQString);

            // rewrite the path...
            context.RewritePath(sendToUrlLessQString, String.Empty, queryString);
        }
    }

}
