using System;
using System.Configuration.Provider;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Web;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using System.Data;
using System.Configuration;
using System.Xml;
using Mediachase.Cms.WebUtility.Commerce;

namespace Mediachase.Cms.Providers
{
    /// <summary>
    /// StoreSiteMapProvider implements SiteMapProvider class and communicates with backend web services to 
    /// return catalog structure.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class CatalogSiteMapProvider : SiteMapProvider
    {
        private string _CatalogName = String.Empty;
        private SiteMapNode _RootNode = null;

        /// <summary>
        /// Gets or sets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
        public string CatalogName
        {
            get
            {
                return _CatalogName;
            }
            set
            {
                _CatalogName = value;
            }
        }

        string AppDomainAppVirtualPathWithTrailingSlash =
            VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StoreSiteMapProvider"/> class.
        /// </summary>
        public CatalogSiteMapProvider() { }


        /// <summary>
        /// When overridden in a derived class, retrieves a <see cref="T:System.Web.SiteMapNode"></see> object that represents the page at the specified URL.
        /// </summary>
        /// <param name="rawUrl">A URL that identifies the page for which to retrieve a <see cref="T:System.Web.SiteMapNode"></see>.</param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the page identified by rawURL; otherwise, null, if no corresponding <see cref="T:System.Web.SiteMapNode"></see> is found or if security trimming is enabled and the <see cref="T:System.Web.SiteMapNode"></see> cannot be returned for the current user.
        /// </returns>
        public override SiteMapNode FindSiteMapNode(string rawUrl)
        {
            string appPath = CMSContext.Current.AppPath;
            if (!appPath.EndsWith("/", StringComparison.Ordinal))
            {
                appPath += "/";
            }

            string url = rawUrl;
            if (rawUrl.IndexOf(appPath, 0) == 0)
            {
                url = url.Substring(appPath.Length);
                object entryCode = HttpContext.Current.Request.QueryString["ec"];
                object code = HttpContext.Current.Request.QueryString["c"];
                if (entryCode != null)
                {
                    Entry entry = CatalogContext.Current.GetCatalogEntry(entryCode.ToString());
                    if (entry != null && !String.IsNullOrEmpty(entry.ID))
                    {
                        return CreateSiteMapNode(entry);
                    }
                }
                else if (code != null)
                {
                    CatalogNode node = CatalogContext.Current.GetCatalogNode(code.ToString());
                    if (node != null && !String.IsNullOrEmpty(node.ID))
                    {
                        return CreateSiteMapNode(node);
                    }
                }
                else
                {
                    Entry entry = CatalogContext.Current.GetCatalogEntryByUri(url, CMSContext.Current.LanguageName);
                    if (entry != null && !String.IsNullOrEmpty(entry.ID))
                    {
                        return CreateSiteMapNode(entry);
                    }

                    CatalogNode node = CatalogContext.Current.GetCatalogNode(url, CMSContext.Current.LanguageName);
                    if (node != null && !String.IsNullOrEmpty(node.ID))
                    {
                        return CreateSiteMapNode(node);
                    }
                }
            }
            return RootNode;
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the child nodes of a specific <see cref="T:System.Web.SiteMapNode"></see>.
        /// </summary>
        /// <param name="node">The <see cref="T:System.Web.SiteMapNode"></see> for which to retrieve all child nodes.</param>
        /// <returns>
        /// A read-only <see cref="T:System.Web.SiteMapNodeCollection"></see> that contains the immediate child nodes of the specified <see cref="T:System.Web.SiteMapNode"></see>; otherwise, null or an empty collection, if no child nodes exist.
        /// </returns>
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {            
            lock (this)
            {
                SiteMapNodeCollection col = new SiteMapNodeCollection();

                if (node == _RootNode)// root node
                {
                    CatalogNodes nodes = CatalogContext.Current.GetCatalogNodes(CatalogName);
                    if (nodes.CatalogNode != null && nodes.CatalogNode.Length > 0)
                    {
                        foreach (CatalogNode row in nodes.CatalogNode)
                            col.Add(CreateSiteMapNode(row));
                    }
                }
                else if (node.Key.IndexOf("c_", 0) == 0)
                {
                    string[] keys = node.Key.Substring("c_".Length).Split('_');
                    int catalogNodeId = Int32.Parse(keys[0]);
                    int parentNodeId = Int32.Parse(keys[1]);
                    int catalogId = Int32.Parse(keys[2]);

                    CatalogNodes nodes = CatalogContext.Current.GetCatalogNodes(catalogId, catalogNodeId);
                    if (nodes.CatalogNode != null && nodes.CatalogNode.Length > 0)
                    {
                        foreach (CatalogNode row in nodes.CatalogNode)
                            col.Add(CreateSiteMapNode(row));
                    }

                }
                return col;
            }           
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the parent node of a specific <see cref="T:System.Web.SiteMapNode"></see> object.
        /// </summary>
        /// <param name="node">The <see cref="T:System.Web.SiteMapNode"></see> for which to retrieve the parent node.</param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the parent of node; otherwise, null, if the <see cref="T:System.Web.SiteMapNode"></see> has no parent or security trimming is enabled and the parent node is not accessible to the current user.
        /// </returns>
        public override SiteMapNode GetParentNode(SiteMapNode catalogNode)
        {
            SiteMapNode ret = null;

            if (catalogNode != null)
            {
                lock (this)
                {
                    if (catalogNode.Key.IndexOf("ce_", 0) == 0)
                    {
                        int entryCodeId = Int32.Parse(catalogNode.Key.Substring("ce_".Length));
                        CatalogRelationDto relation = CatalogContext.Current.GetCatalogRelationDto(0, 0, entryCodeId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
                        if (relation.NodeEntryRelation.Count > 0)
                        {
                            int catalogNodeId = relation.NodeEntryRelation[0].CatalogNodeId;
                            CatalogNode node = CatalogContext.Current.GetCatalogNode(catalogNodeId);
                            if (node != null && !String.IsNullOrEmpty(node.ID))
                            {
                                return CreateSiteMapNode(node); ;
                            }
                        }
                    }
                    else if (catalogNode.Key.IndexOf("c_", 0) == 0)
                    {
                        string[] keys = catalogNode.Key.Substring("c_".Length).Split('_');
                        int parentNodeId = Int32.Parse(keys[1]);

                        CatalogNode node = CatalogContext.Current.GetCatalogNode(parentNodeId);
                        if (node != null && !String.IsNullOrEmpty(node.ID))
                        {
                            return CreateSiteMapNode(node);
                        }
                    }

                }
            }

            return ret;
        }

        /// <summary>
        /// When overidden in a derived class, retrieves the root node of all the nodes that are currently managed by the current provider.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the root node of the set of nodes that the current provider manages.
        /// </returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            SiteMapNode node = null;
            lock (this)
            {
                node = RootNode;
            }
            return node;
        }

        #region Implementation

        /// <summary>
        /// Gets the root <see cref="T:System.Web.SiteMapNode"></see> object of the site map data that the current provider represents.
        /// </summary>
        /// <value></value>
        /// <returns>The root <see cref="T:System.Web.SiteMapNode"></see> of the current site map data provider. The default implementation performs security trimming on the returned node.</returns>
        public override SiteMapNode RootNode
        {
            get
            {
                if (_RootNode == null)
                    _RootNode = new SiteMapNode(this, "", "", "");

                return _RootNode;
            }
        }

        /// <summary>
        /// Creates the site map node.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private SiteMapNode CreateSiteMapNode(CatalogNode node)
        {
            string url = String.Empty;
            string name = String.Empty;
            string key = String.Empty;

            string uri = String.Empty;
            if (node.SeoInfo != null && node.SeoInfo.Length > 0)
            {
                foreach (Seo seo in node.SeoInfo)
                {
                    if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        url = "~/" + seo.Uri;
                        break;
                    }
                }
            }

            name = StoreHelper.GetNodeDisplayName(node);
            key = "c_" + node.CatalogNodeId + "_" + node.ParentNodeId + "_" + node.CatalogId;

            if (String.IsNullOrEmpty(url))
                url = NavigationManager.GetUrl("NodeView", "nc", node.ID);

            SiteMapNode newNode = new SiteMapNode(this, key, MakeUrlAbsolute(url), name);
            return newNode;
        }

        /// <summary>
        /// Creates the site map node.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        private SiteMapNode CreateSiteMapNode(Entry entry)
        {
            string url = String.Empty;
            string name = String.Empty;
            string key = String.Empty;

            if (entry != null)
            {
                string uri = String.Empty;
                if (entry.SeoInfo != null && entry.SeoInfo.Length > 0)
                {
                    foreach (Seo seo in entry.SeoInfo)
                    {
                        if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            url = "~/" + seo.Uri;
                            break;
                        }
                    }
                }
                name = StoreHelper.GetEntryDisplayName(entry);
                key = "ce_" + entry.CatalogEntryId;

                if (String.IsNullOrEmpty(url))
                    url = NavigationManager.GetUrl("EntryView", "ec", entry.ID);
            }


            return new SiteMapNode(this, key, MakeUrlAbsolute(url), name);
        }

        /// <summary>
        /// Makes the URL absolute.
        /// </summary>
        /// <param name="oldUrl">The old URL.</param>
        /// <returns></returns>
        private string MakeUrlAbsolute(string oldUrl)
        {
            string url = oldUrl;

            // Make urls absolute.
            if (!String.IsNullOrEmpty(url))
            {
                // URL needs to be trimmed.
                url = url.Trim();

                if (IsRelativeUrl(url))
                {
                    string virtualPath = url;

                    int qs = url.IndexOf('?');
                    if (qs != -1)
                    {
                        virtualPath = url.Substring(0, qs);
                    }

                    // Make sure the path is adjusted properly
                    virtualPath =
                        VirtualPathUtility.Combine(AppDomainAppVirtualPathWithTrailingSlash, virtualPath);

                    // Make it an absolute virtualPath
                    virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);

                    if (qs != -1)
                    {
                        virtualPath += url.Substring(qs);
                    }

                    url = virtualPath;
                }

                // Reject any suspicious or mal-formed Urls.
                string decodedUrl = HttpUtility.UrlDecode(url);
                if (!String.Equals(url, decodedUrl, StringComparison.Ordinal))
                {
                    throw new ConfigurationErrorsException(String.Format("bad url '{0}'", url));
                }
            }

            return url;
        }

        // Returns whether the virtual path is relative.  Note that this returns true for
        // app relative paths (e.g. "~/sub/foo.aspx")
        /// <summary>
        /// Determines whether [is relative URL] [the specified virtual path].
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>
        /// 	<c>true</c> if [is relative URL] [the specified virtual path]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsRelativeUrl(string virtualPath)
        {
            // If it has a protocol, it's not relative
            if (virtualPath.IndexOf(":", StringComparison.Ordinal) != -1)
                return false;

            return !IsRooted(virtualPath);
        }

        /// <summary>
        /// Determines whether the specified basepath is rooted.
        /// </summary>
        /// <param name="basepath">The basepath.</param>
        /// <returns>
        /// 	<c>true</c> if the specified basepath is rooted; otherwise, <c>false</c>.
        /// </returns>
        private bool IsRooted(String basepath)
        {
            return (String.IsNullOrEmpty(basepath) || basepath[0] == '/' || basepath[0] == '\\');
        }

        /// <summary>
        /// Gets the and remove string attribute internal.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private XmlNode GetAndRemoveStringAttributeInternal(XmlNode node, string attrib, bool fRequired, ref string val)
        {
            XmlNode a = GetAndRemoveAttribute(node, attrib, fRequired);
            if (a != null)
            {
                val = a.Value;
            }

            return a;
        }


        /// <summary>
        /// Gets the and remove attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <returns></returns>
        private XmlNode GetAndRemoveAttribute(XmlNode node, string attrib, bool fRequired)
        {
            XmlNode a = node.Attributes.RemoveNamedItem(attrib);

            // If the attribute is required and was not present, throw
            if (fRequired && a == null)
            {
                throw new ConfigurationErrorsException(
                    String.Format("Missing required attribute {0} {1}", attrib, node.Name),
                    node);
            }

            return a;
        }

        /// <summary>
        /// Gets the and remove string attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private XmlNode GetAndRemoveStringAttribute(XmlNode node, string attrib, ref string val)
        {
            return GetAndRemoveStringAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }
        #endregion

    }
}