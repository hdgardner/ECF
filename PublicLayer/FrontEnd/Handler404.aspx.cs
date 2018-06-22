using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Cms.Dto;
using System.Text.RegularExpressions;
using Mediachase.Cms.WebUtility;

namespace Mediachase.Cms.Website
{
    /// <summary>
    /// Smart 404 error handler, will find the optimal page to redirect the traffic to.
    /// </summary>
    public partial class Handler404 : PageBase
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessRedirect(Request.QueryString["aspxerrorpath"]);
        }

        /// <summary>
        /// Processes the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void ProcessRedirect(string url)
        {
            string newUrl = url;
            if (HttpContext.Current.Request.ApplicationPath.Length != 1)
                newUrl = newUrl.Substring(HttpContext.Current.Request.ApplicationPath.Length);

            if (newUrl.StartsWith("/"))
                newUrl = newUrl.Substring(1);

            // Get list of languages
            SiteDto dto = CMSContext.Current.GetSiteDto(CMSContext.Current.SiteId);
            foreach (SiteDto.SiteLanguageRow row in dto.SiteLanguage.Rows)
            {
                CatalogEntryDto entry = CatalogContext.Current.GetCatalogEntryByUriDto(newUrl, row.LanguageCode);
                if (entry.CatalogItemSeo.Count > 0)
                {
                    Uri rawUrl = BuildUri(NavigationManager.GetUrl("EntryView", "ec", entry.CatalogEntry[0].Code), HttpContext.Current.Request.IsSecureConnection);
                    string sendToUrl = rawUrl.PathAndQuery;
                    Redirect(sendToUrl);
                }

                CatalogNodeDto node = CatalogContext.Current.GetCatalogNodeDto(newUrl, row.LanguageCode);
                if (node.CatalogItemSeo.Count > 0)
                {
                    Uri rawUrl = BuildUri(NavigationManager.GetUrl("NodeView", "nc", node.CatalogNode[0].Code), HttpContext.Current.Request.IsSecureConnection);
                    string sendToUrl = rawUrl.PathAndQuery;
                    Redirect(sendToUrl);
                }
            }

            // if no entry found, perform search and redirect user there
            if (!newUrl.Contains("searchresults.aspx"))
                Response.Redirect(String.Format(ResolveUrl("~/catalog/searchresults.aspx?filter={0}"), HttpUtility.UrlEncode(CleanUrlField(newUrl))));
            else
                Response.Redirect("~");
        }

        /// <summary>
        /// Cleans the URL field.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string CleanUrlField(string input)
        {
            // step 1: remove leading spaces
            input = input.Trim();

            // step 2: replace one or more decimals with a dash
            input = Regex.Replace(input, @"\.+", " ");

            // step 3: replace all consecutive spaces with one dash
            input = Regex.Replace(input, @"\s+", " ");

            // step 4: replace all nonalphanumeric characters with " "
            input = Regex.Replace(input, @"[^\w\-]", " ");

            // step 5: replace all consecutive dashes with one dash
            input = Regex.Replace(input, @"\-+", " ");

            return input;
        }

        /// <summary>
        /// Redirects the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        protected void Redirect(string url)
        {
            Response.Clear();
            Response.Status = "301 Permanently moved";
            Response.AddHeader("location", url);
            Response.End();
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
    }
}
