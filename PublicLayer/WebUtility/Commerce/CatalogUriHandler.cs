using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms;

namespace Mediachase.Cms.WebUtility.Commerce
{
    public class CatalogUriHandler : IHttpHandlerFactory
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
            string newUrl = url;
            if (CMSContext.Current.AppPath.Length != 1)
                newUrl = newUrl.Substring(CMSContext.Current.AppPath.Length);

            if(newUrl.StartsWith("/"))
                newUrl = newUrl.Substring(1);

            CatalogEntryDto entry = CatalogContext.Current.GetCatalogEntryByUriDto(newUrl, CMSContext.Current.LanguageName);
            if (entry.CatalogItemSeo.Count > 0)
            {
                SaveOriginalUrl(context, url);
                Uri rawUrl = BuildUri(NavigationManager.GetUrl("EntryView", "ec", entry.CatalogEntry[0].Code), HttpContext.Current.Request.IsSecureConnection);
                string filePath;
                string sendToUrlLessQString;
                string sendToUrl = rawUrl.PathAndQuery;
                RewriteUrl(context, sendToUrl, out sendToUrlLessQString, out filePath);

                return new CmsUriHandler().GetHandler(context, requestType, sendToUrlLessQString, filePath);
            }

            CatalogNodeDto node = CatalogContext.Current.GetCatalogNodeDto(newUrl, CMSContext.Current.LanguageName);
            if (node.CatalogItemSeo.Count > 0)
            {
                SaveOriginalUrl(context, url);
                Uri rawUrl = BuildUri(NavigationManager.GetUrl("NodeView", "nc", node.CatalogNode[0].Code), HttpContext.Current.Request.IsSecureConnection);
                string filePath;
                string sendToUrlLessQString;
                string sendToUrl = rawUrl.PathAndQuery;
                RewriteUrl(context, sendToUrl, out sendToUrlLessQString, out filePath);

                return new CmsUriHandler().GetHandler(context, requestType, sendToUrlLessQString, filePath);
            }

            if (!string.IsNullOrEmpty(pathTranslated))
            {
				return new CmsUriHandler().GetHandler(context, requestType, url, pathTranslated);
				//return PageParser.GetCompiledPageInstance(url, pathTranslated, context);
            }
            else
            {
                return new CmsUriHandler().GetHandler(context, requestType, url, pathTranslated);
            }
        }

        /// <summary>
        /// Saves the original URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void SaveOriginalUrl(HttpContext context, string url)
        {
            if (!CMSContext.Current.IsUrlReWritten)
            {
                CMSContext.Current.IsUrlReWritten = true;
                if (context.Request.QueryString.Count == 0)
                    CMSContext.Current.CurrentUrl = url;
                else
                    CMSContext.Current.CurrentUrl = String.Format(url + "?" + HttpContext.Current.Request.QueryString);
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
                string newTarget = CMSContext.Current.AppPath;
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
