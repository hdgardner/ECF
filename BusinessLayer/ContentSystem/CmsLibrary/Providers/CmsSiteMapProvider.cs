using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Web;
using mc = Mediachase.Cms;
using System.Data;
using Mediachase.Cms;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace Mediachase.Cms.Providers
{
    /// <summary>
    /// StoreSiteMapProvider implements SiteMapProvider class and communicates with backend web services to 
    /// return catalog structure.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class CmsSiteMapProvider : SiteMapProvider
    {
        internal object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StoreSiteMapProvider"/> class.
        /// </summary>
        public CmsSiteMapProvider()
        {
        }

        private int? _activeTopLevelMenuId = null;

        /// <summary>
        /// MenuId associated with the active menu
        /// </summary>
        public int? ActiveTopLevelMenuId
        {
            get
            {
                if (_activeTopLevelMenuId == null)
                {
                    if (TopLevelMenus.Count > 0)
                    {
                        _activeTopLevelMenuId = TopLevelMenus[0].MenuId;
                        _activeTopLevelMenuName = TopLevelMenus[0].MenuName;
                    }
                }

                return _activeTopLevelMenuId;
            }
            set
            {
                if (_activeTopLevelMenuId != value)
                {
                    //first make sure that the name is valid
                    foreach (TopLevelMenuItem item in TopLevelMenus)
                    {
                        if (item.MenuId == value)
                        {
                            _activeTopLevelMenuId = value;
                            _activeTopLevelMenuName = item.MenuName;
                            ResetRootNode();
                            ResetMenuNodes();
                            break;
                        }
                    }
                }
            }
        }

        private string _activeTopLevelMenuName = "";

        /// <summary>
        /// Setting this property allows the top-level menu to be used to be configured; if its not configured, then the first top-level menu 
        /// in the database is used when creating the datasource
        /// </summary>
        public string ActiveTopLevelMenuName
        {
            get
            {
                if (_activeTopLevelMenuName == "")
                {
                    if (TopLevelMenus.Count > 0)
                    {
                        _activeTopLevelMenuName = TopLevelMenus[0].MenuName;
                        _activeTopLevelMenuId = TopLevelMenus[0].MenuId;
                    }
                }

                return _activeTopLevelMenuName;
            }
            set
            {
                if (_activeTopLevelMenuName != value)
                {
                    //first make sure that the name is valid
                    foreach (TopLevelMenuItem item in TopLevelMenus)
                    {
                        if (item.MenuName == value)
                        {
                            _activeTopLevelMenuName = value;
                            _activeTopLevelMenuId = item.MenuId;
                            ResetRootNode();
                            ResetMenuNodes();
                            break;
                        }
                    }
                }
            }
        }

        private List<TopLevelMenuItem> _topLevelMenus = null;
        /// <summary>
        /// Gets the top level menus.
        /// </summary>
        /// <value>The top level menus.</value>
        public List<TopLevelMenuItem> TopLevelMenus
        {
            get
            {
                return GetTopLevelMenuItems();
            }
        }

        /// <summary>
        /// Retrieve a list of top-level menus from the database (if not already retrieved)
        /// </summary>
        private List<TopLevelMenuItem> GetTopLevelMenuItems()
        {
            string cacheKey = CmsCache.CreateCacheKey("sitemap-menus-top", CMSContext.Current.SiteId.ToString(), CMSContext.Current.LanguageId.ToString());

            object cachedObject = CMSContext.Current.Context.Items[cacheKey];

            if (cachedObject != null)
                return (List<TopLevelMenuItem>)cachedObject;

            cachedObject = CmsCache.Get(cacheKey);
            if (cachedObject != null)
                return (List<TopLevelMenuItem>)cachedObject;

            List<TopLevelMenuItem> menus = new List<TopLevelMenuItem>();
            lock (CmsCache.GetLock(cacheKey))
            {
                cachedObject = CmsCache.Get(cacheKey);
                if (cachedObject != null)
                    return (List<TopLevelMenuItem>)cachedObject;

                //retrieve and iterate through datareader of root nodes and build generic list of items
                IDataReader reader = mc.MenuItem.LoadAllRoot(CMSContext.Current.SiteId);
                while (reader.Read())
                {
                    TopLevelMenuItem item = new TopLevelMenuItem();

                    try
                    {
                        item.MenuId = int.Parse(reader["MenuId"].ToString());
                        item.MenuName = reader["Text"].ToString();
                    }
                    catch
                    {
                        reader.Close();
                        throw new Exception("Invalid menu data returned");
                    }

                    menus.Add(item);
                }

                reader.Close();

                CmsCache.Insert(cacheKey, menus, CmsConfiguration.Instance.Cache.MenuTimeout);
                CMSContext.Current.Context.Items[cacheKey] = menus;
            }

            return menus;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataSource()
        {
            string cacheKey = GetSitemapCacheKey();

            object cachedObject = CMSContext.Current.Context.Items[cacheKey];

            if (cachedObject != null)
                return (DataTable)cachedObject;

            cachedObject = CmsCache.Get(cacheKey);
            if (cachedObject != null)
                return (DataTable)cachedObject;

            DataTable dt = null;
            lock (CmsCache.GetLock(cacheKey))
            {
                cachedObject = CmsCache.Get(cacheKey);
                if (cachedObject != null)
                    return (DataTable)cachedObject;

                dt = mc.MenuItem.LoadAllDT(CMSContext.Current.SiteId, CMSContext.Current.LanguageId);

                if (ActiveTopLevelMenuId > 0)
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = String.Format("MenuId = {0}", ActiveTopLevelMenuId);
                    dt = dv.ToTable();
                }

                CmsCache.Insert(cacheKey, dt, CmsConfiguration.Instance.Cache.MenuTimeout);
                CMSContext.Current.Context.Items[cacheKey] = dt;
            }

            return dt;
        }

        private string GetSitemapCacheKey()
        {
            return CmsCache.CreateCacheKey("sitemap", CMSContext.Current.SiteId.ToString(), ActiveTopLevelMenuId.ToString(), CMSContext.Current.LanguageId.ToString());
        }

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

            SiteMapNode node = null;
            lock (_lock)
            {
                string url = rawUrl;
                if (rawUrl.IndexOf(appPath, 0) == 0)
                {
                    url = url.Substring(appPath.Length - 1);
                    if (url.IndexOf('?') > 0)
                    {
                        url = url.Substring(0, url.IndexOf('?'));
                    }

                    DataTable src = GetDataSource();
                    DataView dv = src.DefaultView;

                    dv.RowFilter = "IsRoot = 1";
                    if (dv.Count > 0)
                    {
                        // CommandType: Link
                        dv.RowFilter = String.Format("MenuId = {0} AND Outline LIKE Outline + '%' AND CommandType = 1 AND CommandText LIKE '~{1}'",
                                        ActiveTopLevelMenuId.ToString(), url);
                        if (dv.Count > 0)
                        {
                            node = CreateSiteMapNode(dv[0]);
                        }

                        // CommandType: Navigation
                        // TODO : FindSiteMapNode for navigation command type
                    }
                }
            }

            if (node != null)
                return node;

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
            SiteMapNodeCollection col = new SiteMapNodeCollection();
            if (node != null)
            {
                lock (_lock)
                {
                    DataTable dt = GetDataSource();
                    DataView dv = dt.DefaultView;

                    dv.RowFilter = String.Format("MenuItemId = {0}", node.Key);
                    if (dv.Count > 0)
                    {
                        int outlineLevel = (int)dv[0]["OutlineLevel"];
                        string outline = (string)dv[0]["Outline"] + node.Key + ".";
                        dv.RowFilter = String.Format("OutlineLevel = {0} AND Outline LIKE '{1}%' AND IsRoot = 0", outlineLevel + 1, outline);
                        for (int i = 0; i < dv.Count; i++)
                        {
                            col.Add(CreateSiteMapNode(dv[i]));
                        }
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the parent node of a specific <see cref="T:System.Web.SiteMapNode"></see> object.
        /// </summary>
        /// <param name="node">The <see cref="T:System.Web.SiteMapNode"></see> for which to retrieve the parent node.</param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"></see> that represents the parent of node; otherwise, null, if the <see cref="T:System.Web.SiteMapNode"></see> has no parent or security trimming is enabled and the parent node is not accessible to the current user.
        /// </returns>
        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            SiteMapNode ret = null;

            if (node != null)
            {
                lock (_lock)
                {
                    DataTable dt = GetDataSource();
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = String.Format("MenuItemId = {0} AND IsVisible = 1", node.Key);
                    if (dv.Count > 0)
                    {
                        string outline = (string)dv[0]["Outline"];// +node.Key + ".";
                        dv.RowFilter = String.Format("Outline + MenuItemId + '.' = '{0}'", outline);
                        if (dv.Count > 0)
                        {
                            ret = CreateSiteMapNode(dv[0]);
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
            lock (_lock)
            {
                node = RootNode;
            }
            return node;
        }

        #region Implementation
        //private SiteMapNode _RootNode = null;

        /// <summary>
        /// Gets the root <see cref="T:System.Web.SiteMapNode"></see> object of the site map data that the current provider represents.
        /// Uses the ActiveTopLevelMenuName to determine which top-level menu to use
        /// </summary>
        /// <value></value>
        /// <returns>The root <see cref="T:System.Web.SiteMapNode"></see> of the current site map data provider. The default implementation performs security trimming on the returned node.</returns>
        public override SiteMapNode RootNode
        {
            get
            {
                string cacheKey = CmsCache.CreateCacheKey("sitemap-rootnode", CMSContext.Current.SiteId.ToString(), CMSContext.Current.LanguageId.ToString());

                object cachedObject = CMSContext.Current.Context.Items[cacheKey];

                if (cachedObject != null)
                    return (SiteMapNode)cachedObject;

                cachedObject = CmsCache.Get(cacheKey);
                if (cachedObject != null)
                    return (SiteMapNode)cachedObject;

                SiteMapNode node = null;
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                        return (SiteMapNode)cachedObject;

                    node = GetRootNodeInternal();

                    CmsCache.Insert(cacheKey, node, CmsConfiguration.Instance.Cache.MenuTimeout);
                    CMSContext.Current.Context.Items[cacheKey] = node;
                }

                return node;
            }
        }

        /// <summary>
        /// Change the root name. This is kept private to prevent it from being externally called and impacting app performance. This
        /// functionality is exposed by changing the name of the root menu name
        /// </summary>
        private SiteMapNode GetRootNodeInternal()
        {
            DataTable dt = GetDataSource();
            DataView dv = dt.DefaultView;

            dv.RowFilter = string.Format("IsRoot = 1 AND MenuId = {0}", ActiveTopLevelMenuId.ToString());
            if (dv.Count > 0)
            {
                return new SiteMapNode(this, dv[0]["MenuItemId"].ToString());
            }

            return null;
        }

        /// <summary>
        /// Resets the root node.
        /// </summary>
        private void ResetRootNode()
        {
            string cacheKey = CmsCache.CreateCacheKey("sitemap-rootnode", CMSContext.Current.SiteId.ToString(), CMSContext.Current.LanguageId.ToString());
            CmsCache.CreateCacheKey(new string[] { cacheKey });
            if (CMSContext.Current.Context.Items.Contains(cacheKey))
                CMSContext.Current.Context.Items.Remove(cacheKey);

            object cachedObject = CmsCache.Get(cacheKey);
            if (cachedObject != null)
                CmsCache.Remove(cacheKey);
        }

        private void ResetMenuNodes()
        {
            string cacheKey = GetSitemapCacheKey();

            if (CMSContext.Current.Context.Items.Contains(cacheKey))
                CMSContext.Current.Context.Items.Remove(cacheKey);

            object cachedObject = CmsCache.Get(cacheKey);
            if (cachedObject != null)
                CmsCache.Remove(cacheKey);
        }

        /// <summary>
        /// Creates the site map node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private SiteMapNode CreateSiteMapNode(DataRowView reader)
        {
            //translation exists
            string url = String.Empty;
            string name = String.Empty;
            string key = String.Empty;
            string desc = String.Empty;

            name = reader["Text"].ToString();
            desc = reader["Tooltip"].ToString();
            key = reader["MenuItemId"].ToString();

            if (reader["CommandType"] != DBNull.Value)
            {
                switch ((int)reader["CommandType"])
                {
                    case 1: // link
                        url = CMSContext.Current.ResolveUrl(reader["CommandText"].ToString());
                        break;
                    case 2: // script
                        url = "javascript:" + reader["CommandText"].ToString();
                        break;
                    case 3: // navigation
                        url = GetAbsolutePath(GetNavigate((string)reader["CommandText"]));
                        break;
                }
            }


            SiteMapNode mapNode = new SiteMapNode(this, key, url, name, desc);
            mapNode["visible"] = reader["isvisible"].ToString();
            return mapNode;
        }

        /// <summary>
        /// Gets the navigate.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string GetNavigate(string text)
        {
            int itemId = Convert.ToInt32(text.Split('^')[0]);
            string param = text.Split('^')[1].Replace("?", String.Empty);

            List<string> parameters = new List<string>();
            List<string> values = new List<string>();

            string[] arr = param.Split('&');
            if (arr != null && arr.Length > 0)
            {
                foreach (string s in arr)
                {
                    string[] sa = s.Split('=');

                    if(sa != null && sa.Length > 0)
                        parameters.Add(sa[0]);

                    if (sa != null && sa.Length > 1)
                        values.Add(sa[1]);
                }
            }

            return mc.NavigationManager.GetUrl(mc.NavigationManager.GetItemNameById(itemId), parameters, values);
        }
        #endregion

        #region GetAbsolutePath
        /// <summary>
        /// Gets the absolute path.
        /// </summary>
        /// <param name="xsPath">The xs path.</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string xsPath)
        {
            if (!String.IsNullOrEmpty(xsPath) && xsPath.StartsWith("~"))
                xsPath = xsPath.Substring(1);

            StringBuilder builder = new StringBuilder();
            builder.Append(HttpContext.Current.Request.Url.Scheme);
            builder.Append("://");
            builder.Append(HttpContext.Current.Request.Url.Host);

            //GA: Cassini fix
            if (Regex.IsMatch(HttpContext.Current.Request.Url.Authority, ":[\\d]+"))
            {
                builder.Append(":");
                builder.Append(HttpContext.Current.Request.Url.Port);
            }

            //builder.Append(HttpContext.Current.Request.ApplicationPath);
            builder.Append(CMSContext.Current.AppPath);
            if (!CMSContext.Current.AppPath.EndsWith("/"))
                builder.Append("/");
            if (!String.IsNullOrEmpty(xsPath))
            {
                if (xsPath[0] == '/')
                    xsPath = xsPath.Substring(1, xsPath.Length - 1);
                builder.Append(xsPath);
            }
            return builder.ToString();
        }
        #endregion
    }

    public struct TopLevelMenuItem
    {
        public int MenuId;
        public string MenuName;
    }
}
