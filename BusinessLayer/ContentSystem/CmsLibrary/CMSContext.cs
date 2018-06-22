using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Web;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Core;
using Mediachase.Data.Provider;
using Mediachase.Cms.Data;
using System.Text;

namespace Mediachase.Cms
{
    /// <summary>
    /// Holds context values used by the Content Management system.
    /// </summary>
    //[Serializable]
    public class CMSContext
    {
        private HttpContext _httpContext = null;
        private string _sitePath = null;
        private string _appPath = null;
        private string _CurrentUrl = String.Empty;
        private bool _IsUrlReWritten = false;
        private Guid _SiteId;
        private static readonly object _lockObject = new object();

        public bool OverrideAccess { get; set; }
        #region .ctor()
        /// <summary>
        /// Initializes a new instance of the <see cref="CMSContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        private CMSContext(HttpContext context)
        {
            _QueryString = new Hashtable();
            pageStatusAccess = new Hashtable();
            TemplateId = -1;
            PageId = -1;
            VersionId = -1;
            this._httpContext = context;

            if (context != null)
            {
                _appPath = context.Request.ApplicationPath;
                _sitePath = GetSitePath();
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public HttpContext Context
        {
            get
            {
                return _httpContext;
            }
        }

        /// <summary>
        /// Gets the site path.
        /// </summary>
        /// <value>The site path.</value>
        public string SitePath
        {
            get { return _sitePath; }
        }

        /// <summary>
        /// Current url, this will be rewritten URL and not a physical one.
        /// </summary>
        /// <value>The current URL.</value>
        public string CurrentUrl
        {
            get
            {
                return _CurrentUrl;
            }
            set
            {
                _CurrentUrl = value;
            }
        }

        /// <summary>
        /// Identifies if the URL has been re written during current request.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is URL re written; otherwise, <c>false</c>.
        /// </value>
        public bool IsUrlReWritten
        {
            get
            {
                return _IsUrlReWritten;
            }

            set
            {
                _IsUrlReWritten = value;
            }
        }

        /// <summary>
        /// Gets the application id, which identified which set of sites to use.
        /// </summary>
        /// <value>The application id.</value>
        public Guid ApplicationId
        {
            get
            {
                return AppContext.Current.ApplicationId;;
            }
        }

        private DateTime _ContextDate = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the current date time. This property is used to filter out cms content elements.
        /// </summary>
        /// <value>The context date time.</value>
        public DateTime CurrentDateTime
        {
            get
            {
                if (_ContextDate == DateTime.MinValue)
                    _ContextDate = DateTime.UtcNow;

                return _ContextDate;
            }
            set
            {
                _ContextDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the site id that current CMS system uses.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get
            {
                return _SiteId;
            }
            set
            {
                _SiteId = value;
            }
        }

        #region property: PageStatusAccess
        private Hashtable pageStatusAccess;
        /// <summary>
        /// Gets the page status access.
        /// </summary>
        /// <value>The page status access.</value>
        public Hashtable PageStatusAccess
        {
            get
            {
                return pageStatusAccess;
            }
        }
        #endregion

        #region property: IsDesignMode
        private bool isDesignMode;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is design mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is design mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsDesignMode
        {
            get { return isDesignMode; }
            set { isDesignMode = value; }
        }
        #endregion
        #endregion

        #region QueryString
        private Hashtable _QueryString;
        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public Hashtable QueryString
        {
            get
            {
                return _QueryString;
            }
        }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <returns></returns>
        public string GetQueryString(string outline)
        {
			if (!String.IsNullOrEmpty(outline))
			{
				string separator = "?";
				string[] reqPath = outline.Split(separator.ToCharArray());
				if (reqPath.Length > 1)
				{
					string pars = reqPath[1];
					separator = "&";
					string[] paramArray = pars.Split(separator.ToCharArray());
					for (int i = 0; i < paramArray.Length; i++)
					{
						separator = "=";
						if (!paramArray[i].Contains(separator))
							return reqPath[0];

						string[] singleParam = paramArray[i].Split(separator.ToCharArray());
						if (QueryString[singleParam[0]] == null)
							QueryString.Add(singleParam[0], singleParam[1]);
					}
				}
				if (reqPath[0].Length == 0)
				{
					reqPath[0] = "/";
				}
				else if ((reqPath[0].LastIndexOf("/") + 1 < reqPath[0].Length) && !(reqPath[0].IndexOf(".") > 0))
				{
					reqPath[0] += "/";
				}

				// fix: sasha, need to add "/" infront of the page always
				if (!reqPath[0].StartsWith("/"))
					reqPath[0] = "/" + reqPath[0];

				return reqPath[0];
			}
			else
				return String.Empty;
        }

        #endregion

        #region Current Context

        /// <summary>
        /// Creates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static CMSContext Create(HttpContext context)
        {
            if (context == null)
                return new CMSContext(context);

            if (context.Items["CMSContext"] == null)
            {
                lock (_lockObject)
                {
                    if (context.Items["CMSContext"] == null)
                    {
                        CMSContext cmsContext = new CMSContext(context);
                        if (!context.Items.Contains("CMSContext"))
                            context.Items.Add("CMSContext", cmsContext);
                        else
                            context.Items["CMSContext"] = cmsContext;
                    }
                }
            }

            return (CMSContext)context.Items["CMSContext"];
        }

        /// <summary>
        /// Gets current CMS context.
        /// </summary>
        /// <value>The current.</value>
        public static CMSContext Current
        {
            get
            {
                return Create(HttpContext.Current);
            }
        }
        #endregion

        #region PageId
        private int pageId = -1;
        /// <summary>
        /// Gets or sets the page id.
        /// </summary>
        /// <value>The page id.</value>
        public int PageId
        {
            get { return pageId; }
            set { pageId = value; }

        }
        #endregion

        #region VersionId
        private int versionId;
        /// <summary>
        /// Gets or sets the version id.
        /// </summary>
        /// <value>The version id.</value>
        public int VersionId
        {
            get { return versionId; }
            set { versionId = value; }

        }
        #endregion

        #region TemplateId
        private int templateId;
        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        /// <value>The template id.</value>
        public int TemplateId
        {
            get { return templateId; }
            set { templateId = value; }

        }
        #endregion

        #region Outline
        private string outline;
        /// <summary>
        /// Gets or sets the outline.
        /// </summary>
        /// <value>The outline.</value>
        public string Outline
        {
            get { return outline; }
            set { outline = value; }
        }

        /// <summary>
        /// Gets or sets the app path.
        /// </summary>
        /// <value>The app path.</value>
        public string AppPath
        {
            get
            {
                return _appPath;
            }
            set
            {
                _appPath = value;
            }
        }
        #endregion

        #region ControlPlaces
        private string controlPlaces;
        /// <summary>
        /// Gets or sets the control places.
        /// </summary>
        /// <value>The control places.</value>
        public string ControlPlaces
        {
            get { return controlPlaces; }
            set { controlPlaces = value; }

        }
        #endregion

        #region ToolBoxVisible
        private bool toolBoxVisible;
        /// <summary>
        /// Gets or sets a value indicating whether [tool box visible].
        /// </summary>
        /// <value><c>true</c> if [tool box visible]; otherwise, <c>false</c>.</value>
        public bool ToolBoxVisible
        {
            get { return toolBoxVisible; }
            set { toolBoxVisible = value; }
        }
        #endregion

        #region ToolBarVisible
        private bool toolBarVisible;

        /// <summary>
        /// Gets or sets a value indicating whether [tool bar visible].
        /// </summary>
        /// <value><c>true</c> if [tool bar visible]; otherwise, <c>false</c>.</value>
        public bool ToolBarVisible
        {
            get { return toolBarVisible; }
            set { toolBarVisible = value; }
        }

        #endregion

        #region LangChange
        private bool langChange;
        /// <summary>
        /// Gets or sets a value indicating whether [lang change].
        /// </summary>
        /// <value><c>true</c> if [lang change]; otherwise, <c>false</c>.</value>
        public bool LangChange
        {
            get { return langChange; }
            set { langChange = value; }
        }
        #endregion

        #region LanguageId
        private int _languageId = -1;
        /// <summary>
        /// Gets the language id.
        /// </summary>
        /// <value>The language id.</value>
        public int LanguageId
        {
            get
            {
                // if we already checked language once, do not do any more checks
                if (_languageId > 0)
                    return _languageId;

                // Load language parameters
                LoadLanguage();
 
                return _languageId;
            }
        }
        #endregion

        #region LanguageName
        private string _languageName = String.Empty;
        /// <summary>
        /// Gets the name of the language.
        /// </summary>
        /// <value>The name of the language.</value>
        public string LanguageName
        {
            get
            {
                if (!String.IsNullOrEmpty(_languageName))
                    return _languageName;

                LoadLanguage();

                /*
                //return Thread.CurrentThread.CurrentCulture.Name;
                using (IDataReader reader = Language.LoadLanguage(LanguageId))
                {
                    if (reader.Read())
                    {
                        _languageName = reader["LangName"].ToString();
                        return _languageName;
                    }
                }
                 * */

                if (String.IsNullOrEmpty(_languageName))
                    _languageName = "en-US";

                return _languageName;
            }
        }
        #endregion

        private string _CurrencyCode = String.Empty;
        /// <summary>
        /// Gets or sets the currency code. Currency code is automatically taken from the CultureInfo.CurrentCulture property. Which is a
        /// culture for a current thread. The property can be changed by setting the currency directly.
        /// </summary>
        /// <value>The currency code.</value>
        public string CurrencyCode
        {
            get
            {
                if (String.IsNullOrEmpty(_CurrencyCode))
                {
                    _CurrencyCode = new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;
                }

                return _CurrencyCode;
            }
            set
            {
                _CurrencyCode = value;
            }
        }

        private bool? _CanAccessCurrentPage;

        /// <summary>
        /// Gets or sets the can access page. Results are cached.
        /// </summary>
        /// <value>The can access page.</value>
        public bool CanAccessPage
        {
            get
            {
                if (OverrideAccess) return true;

                if (this.PageId <= 0)
                    throw new NullReferenceException("CMSContext.Current.PageId can't be null.");

                if (_CanAccessCurrentPage != null)
                    return _CanAccessCurrentPage == true;

                // Try checking cache first
                // Assign new cache key, specific for site guid and response groups requested
                string cacheKey = CmsCache.CreateCacheKey("page-access-everyone", PageId.ToString());

                bool? accessGranted = null;
                int rootId = -1;

                // check cache first
                object cachedObject = CmsCache.Get(cacheKey);

                if (cachedObject != null)
                    accessGranted = (bool)cachedObject;

                // Load the object
                if (accessGranted == null)
                {
                    lock (CmsCache.GetLock(cacheKey))
                    {
                        cachedObject = CmsCache.Get(cacheKey);
                        if (cachedObject != null)
                        {
                            accessGranted = (bool)cachedObject;
                        }
                        else
                        {
                            //GET FOLDERID
                            using (IDataReader reader0 = FileTreeItem.LoadParent(PageId))
                            {
                                if (reader0.Read())
                                    rootId = (int)reader0["PageId"];

                                reader0.Close();
                            }

                            //CHECK ACCESS FOR EVERYONE
                            using (IDataReader reader1 = FileTreeItem.PageAccessGetByRoleIdPageId(AppRoles.EveryoneRole, rootId))
                            {
                                if (reader1.Read())
                                    accessGranted = true;

                                reader1.Close();
                            }

                            // Insert to the cache collection
                            if (accessGranted == true)
                                CmsCache.Insert(cacheKey, accessGranted, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                        }
                    }
                }

                if (accessGranted == true)
                {
                    _CanAccessCurrentPage = true;
                }
                else
                {
                    //CHECK ACCESS FOR ROLES
                    using (IDataReader reader2 = FileTreeItem.PageAccessGetByPageId(rootId))
                    {
                        while (reader2.Read())
                        {
                            if (HttpContext.Current.User.IsInRole((string)reader2["RoleId"]))
                            {
                                accessGranted = true;
                                break;
                            }
                        }

                        reader2.Close();
                    }

                    //GA 11.07.2006 : fix access denied
                    if ((accessGranted == false || accessGranted == null) 
                        && rootId == FileTreeItem.GetRoot(CMSContext.Current.SiteId))
                    {
                        accessGranted = true;

                        // Insert to the cache collection
                        if (accessGranted == true)
                            CmsCache.Insert(cacheKey, accessGranted, CmsConfiguration.Instance.Cache.PageDocumentTimeout);
                    }
                }

                _CanAccessCurrentPage = accessGranted;

                return _CanAccessCurrentPage == true;
            }
        }


        #region Private Methods
        /// <summary>
        /// Loads the language information. Results are cached usiong CmsConfiguration.Instance.Cache.SiteVariablesTimeout.
        /// </summary>
        private void LoadLanguage()
        {
            string cacheKey = CmsCache.CreateCacheKey("languages", CMSContext.Current.ApplicationId.ToString(), Thread.CurrentThread.CurrentCulture.Name);

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                _languageId = Int32.Parse(((string[])cachedObject)[0]);
                _languageName = (string)((string[])cachedObject)[1];
                return;
            }

            using (IDataReader reader = Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
            {
                if (reader.Read())
                {
                    _languageId = (int)reader["LangId"];
                    _languageName = reader["LangName"].ToString();
                }
                else
                {
                    using (IDataReader reader2 = Language.GetAllLanguages())
                    {
                        while (reader2.Read())
                        {
                            if ((bool)reader2["IsDefault"])
                            {
                                _languageName = reader2["LangName"].ToString();
                                _languageId = (int)reader2["LangId"];
                            }
                        }

                        reader2.Close();
                    }
                }
                reader.Close();
            }

            // Save cache
            CmsCache.Insert(cacheKey, new string[] { _languageId.ToString(), _languageName}, CmsConfiguration.Instance.Cache.SiteVariablesTimeout);
        }

        /// <summary>
        /// Gets the site path.
        /// </summary>
        /// <returns></returns>
        private string GetSitePath()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_httpContext.Request.Url.Host.Replace("www.", string.Empty));

            for (int index = 0; index < _httpContext.Request.Url.Segments.Length - 1; index++ )
            {
                builder.Append(_httpContext.Request.Url.Segments[index]);
            }

            string lastSegment = _httpContext.Request.Url.Segments[_httpContext.Request.Url.Segments.Length - 1];

            if (lastSegment.EndsWith("/"))
                builder.Append(lastSegment);
            else if(!lastSegment.Contains("."))
            {
                builder.Append(lastSegment);
                //builder.Append("/");
            }

            return builder.ToString();
        }

        /*
        private string GetAppUrl()
        {
            string hostName = _httpContext.Request.Url.Host.Replace("www.", string.Empty);
            string applicationPath = _httpContext.Request.ApplicationPath;

            if (applicationPath.EndsWith("/"))
                applicationPath = applicationPath.Remove(applicationPath.Length - 1, 1);

            return hostName + applicationPath;

        }
         * */
        #endregion

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetContentSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = ContentDataHelper.CreateDataCommand();
			command.CommandText = "GetContentSchemaVersionNumber";
			DataResult result = DataService.LoadDataSet(command);
			if (result.DataSet != null)
			{
				if (result.DataSet.Tables.Count > 0 && result.DataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = result.DataSet.Tables[0].Rows[0];
					major = (int)row["Major"];
					minor = (int)row["Minor"];
					patch = (int)row["Patch"];
					installDate = (DateTime)row["InstallDate"];
				}
			}

			return retval;
		}

        /// <summary>
        /// Gets the sites dto.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <returns></returns>
        public SiteDto GetSitesDto(Guid appId)
        {
            return SiteManager.GetSites(appId);
        }

		/// <summary>
		/// Gets the sites dto.
		/// </summary>
		/// <param name="appId">The app id.</param>
		/// <param name="returnInactive"></param>
		/// <returns></returns>
		public SiteDto GetSitesDto(Guid appId, bool returnInactive)
		{
			return SiteManager.GetSites(appId, returnInactive);
		}

        /// <summary>
        /// Gets the site dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public SiteDto GetSiteDto(Guid siteId)
        {
            return SiteManager.GetSite(siteId);
        }

		/// <summary>
		/// Gets the site dto.
		/// </summary>
		/// <param name="siteId">The site id.</param>
		/// <param name="returnInactive"></param>
		/// <returns></returns>
		public SiteDto GetSiteDto(Guid siteId, bool returnInactive)
		{
			return SiteManager.GetSite(siteId, returnInactive);
		}

        /// <summary>
        /// Saves the site.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveSite(SiteDto dto)
        {
            SiteManager.SaveSite(dto);
        }

        /// <summary>
        /// Resets the language.
        /// </summary>
        public void ResetLanguage()
        {
            _languageId = -1;
            _languageName = String.Empty;
        }

        #region Url Resolution Methods
        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        public string ResolveUrl(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
                return originalUrl;

            // *** Absolute path - just return
            if (IsAbsolutePath(originalUrl))
                return originalUrl;

            // *** We don't start with the '~' -> we don't process the Url
            if (!originalUrl.StartsWith("~"))
                return originalUrl;

            // *** Fix up path for ~ root app dir directory
            // VirtualPathUtility blows up if there is a 
            // query string, so we have to account for this.
            int queryStringStartIndex = originalUrl.IndexOf('?');
            if (queryStringStartIndex != -1)
            {
                string queryString = originalUrl.Substring(queryStringStartIndex);
                string baseUrl = originalUrl.Substring(0, queryStringStartIndex);

                return string.Concat(
                    VirtualPathUtility.ToAbsolute(baseUrl, this.AppPath),
                    queryString);
            }
            else
            {
                return VirtualPathUtility.ToAbsolute(originalUrl, this.AppPath);
            }

        }

        /// <summary>
        /// Determines whether [is absolute path] [the specified original URL].
        /// </summary>
        /// <param name="originalUrl">The original URL.</param>
        /// <returns>
        /// 	<c>true</c> if [is absolute path] [the specified original URL]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAbsolutePath(string originalUrl)
        {
            // *** Absolute path - just return
            int IndexOfSlashes = originalUrl.IndexOf("://");
            int IndexOfQuestionMarks = originalUrl.IndexOf("?");

            if (IndexOfSlashes > -1 &&
                 (IndexOfQuestionMarks < 0 ||
                  (IndexOfQuestionMarks > -1 && IndexOfQuestionMarks > IndexOfSlashes)
                  )
                )
                return true;

            return false;
        }
        #endregion

        #region Redirect Helpers
        /// <summary>
        /// Redirects the specified URL. The redirect is done using SEO friendly method and is permanent.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void Redirect(string url)
        {
            Context.Response.Clear();
            Context.Response.Status = "301 Moved Permanently";
            Context.Response.AddHeader("Location", url);
            Context.Response.End();
        }
        #endregion
    }
}
