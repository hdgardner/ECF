using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Mediachase.Cms;
using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Data;
using System.Globalization;
using System.Threading;
using System.IO;
using Mediachase.Cms.Dto;
using Common.Logging;
using Mediachase.Cms.Util;

namespace Mediachase.Cms.WebUtility
{
    public class CmsHttpModule : IHttpModule
    {
		private const string _toolBarVisibleCookieString = "ToolBarVisibleCookie";
		private const string _toolBoxVisibleCookieString = "ToolBoxVisibleCookie";
		private const string _currentCultureRequestString = "CurrentCulture";
		private const string _currentCultureCookieString = "MediachaseCMSCurrentCulture";
		private const string _versionIdQueryString = "VersionId";
		private const string _languageQueryString = "lang";

        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.Error += new EventHandler(context_Error);
        }

        #region Exception Handlers
        /// <summary>
        /// Handles the Error event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void context_Error(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
			
            Exception lastError = context.Server.GetLastError();
            Exception ex = lastError.GetBaseException();
			
            if (lastError.InnerException != null && lastError.InnerException is Mediachase.Search.SearchException)
            {
                HandleException(context, (Mediachase.Search.SearchException)lastError.InnerException);
            }
            else if (ex != null)
            {
                HandleException(context, ex);
            }
            else
            {
                // Log the exception
                LogManager.GetLogger(GetType()).Error("Front end encountered unhandled error.", ex);
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="searchException">The search exception.</param>
        void HandleException(HttpContext context, Mediachase.Search.SearchException searchException)
        {
            Write(new string[] { "{message}" }, new string[] { searchException.ToString() }, "SearchConfigError.htm");
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The ex.</param>
        void HandleException(HttpContext context, Exception ex)
        {
            if (typeof(System.ComponentModel.LicenseException) == ex.GetType())
            {
                //Response.Redirect("~/Errors/LicensingError.htm");
            }
            if (typeof(Mediachase.Search.SearchException) == ex.GetType())
            {
                context.Response.Redirect("~/Errors/SearchConfigError.htm");
            }
            else if (typeof(HttpException) == ex.GetType())
            {
                int errorCode = ((HttpException)ex).GetHttpCode();
                if (errorCode == 500) // consider 500 a fatal exception
                {
                    // Log the exception
                    LogManager.GetLogger(GetType()).Fatal("Front end encountered unhandled error.", ex);
                    return;
                }
				else if (!context.IsCustomErrorEnabled)
				{
					if (errorCode == 404)
					{
						// Log the exception
						//context.Response.Redirect("~/Errors/PageNotFound.htm");
						LogManager.GetLogger(GetType()).Error("Could not find the requested page.", ex);
						Write(new string[] { "{message}" }, new string[] { ex.Message }, "PageNotFound.htm");
						return;
					}
				}
            }

            LogManager.GetLogger(GetType()).Error("Front end encountered unhandled error.", ex);
        }


        /// <summary>
        /// Writes the specified filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="messages">The messages.</param>
        /// <param name="errorFile">The error file.</param>
        private static void Write(string[] filters, string[] messages, string errorFile)
        {
            string defaultLanguage = Thread.CurrentThread.CurrentCulture.Name;

            StreamReader reader = null;
            string path = String.Format("~/errors/{0}/{1}", defaultLanguage, errorFile);
            if (File.Exists(HttpContext.Current.Server.MapPath(path)))
                reader = new StreamReader(HttpContext.Current.Server.MapPath(path));
            else
                reader = new StreamReader(HttpContext.Current.Server.MapPath(String.Format("~/errors/{0}", errorFile)));

            string html = reader.ReadToEnd();
            reader.Close();

            if (filters != null && messages != null)
            {
                for (int index = 0; index < filters.Length; index++)
                {
                    html = html.Replace(filters[index], messages[index]);
                }
            }

            System.Web.HttpContext.Current.Response.Write(html);
            System.Web.HttpContext.Current.Response.End();
        }
        #endregion

        /// <summary>
        /// Finds the site row from URL.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <returns></returns>
        SiteDto.SiteRow FindSiteFromUrl(CMSContext ctx)
        {
            SiteDto.SiteRow siteRow = null;

            SiteDto sites = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId);

            string url = ctx.SitePath;
            string defaultDomain = ctx.Context.Request.Url.Host.Replace("www.", string.Empty);
            string appPath = ctx.Context.Request.ApplicationPath;
            Guid siteId = Guid.Empty;

            // Check if we have siteid reference
            if (!String.IsNullOrEmpty(ctx.Context.Request["siteid"]))
            {
                siteId = new Guid(ctx.Context.Request["siteid"]);
            }

            // Match score is used to score the URL matching site definitions, the higher the score, the better the match,
            // it is used to prevent lower matches overwriting higher matched
            int matchScore = 0;

            if (appPath == "/")
                appPath = String.Empty;

            foreach (SiteDto.SiteRow row in sites.Site.Rows)
            {
                // Check if site is active
                if (!row.IsActive)
                    continue;

                // Check if we reference specific site id
                if (siteId != Guid.Empty && row.SiteId == siteId)
                {
                    ctx.AppPath = row.Folder;

                    if (String.IsNullOrEmpty(ctx.AppPath))
                        ctx.AppPath = "/";

                    siteRow = row;
                    return siteRow;
                }

                // Check if site is default
                if (row.IsDefault && siteRow == null)
                    siteRow = row;

                // Split domains
                string rowDomain = row.Domain.Replace("\r\n", "\n");
                string[] domains = rowDomain.Split(new char[] { '\n' });

                // Cycle through domains
                foreach (string domain in domains)
                {
                    int domainScoreModifier = 0;
                    string d = domain;
                    if (String.IsNullOrEmpty(domain))
                        d = defaultDomain;
                    else
                        domainScoreModifier = 100;

                    if (url.Equals(d + row.Folder, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check matching score
                        int newMatchScore = (row.Folder).Length + domainScoreModifier;
                        if (newMatchScore <= matchScore)
                            continue;
                        matchScore = newMatchScore;
                        ctx.AppPath = row.Folder;

                        if (String.IsNullOrEmpty(ctx.AppPath))
                            ctx.AppPath = "/";

                        siteRow = row;
                    }
                    else if (url.Equals(d + appPath + row.Folder, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check matching score
                        int newMatchScore = (appPath + row.Folder).Length + domainScoreModifier;
                        if (newMatchScore <= matchScore)
                            continue;
                        matchScore = newMatchScore;
                        ctx.AppPath = appPath + row.Folder;

                        if (String.IsNullOrEmpty(ctx.AppPath))
                            ctx.AppPath = "/";

                        siteRow = row;
                    }
                    else if (url.StartsWith(d + appPath + row.Folder, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check matching score
                        int newMatchScore = (appPath + row.Folder).Length + domainScoreModifier;
                        if (newMatchScore <= matchScore)
                            continue;

                        matchScore = newMatchScore;
                        ctx.AppPath = appPath + row.Folder;

                        if (String.IsNullOrEmpty(ctx.AppPath))
                            ctx.AppPath = "/";

                        siteRow = row;
                    }
                    else if (url.StartsWith(d + row.Folder, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check matching score
                        int newMatchScore = (row.Folder).Length + domainScoreModifier;
                        if (newMatchScore <= matchScore)
                            continue;

                        matchScore = newMatchScore;
                        ctx.AppPath = row.Folder;

                        if (String.IsNullOrEmpty(ctx.AppPath))
                            ctx.AppPath = "/";

                        siteRow = row;
                    }
                }
            }

            return siteRow;
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// 
        /// It will execute the following actions:
        /// 
        ///     1. Determine which site this request belongs to.
        ///     2. Check if request is for the template.aspx and if it is redirect to the homepage instead.
        ///     3. Determine if it is a folder request or not and if it is detect the default page and redirect.
        ///     4. Restore settings from user cookies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            // Process only aspx pages and directories
            if (!(context.Request.Url.ToString().ToLower().IndexOf(".aspx") > 0)
                && !context.Request.Url.ToString().EndsWith("/"))
            {
                // If page name does not contain ".", then assume it is a folder and try to process the request
                if (context.Request.Url.Segments[context.Request.Url.Segments.Length - 1].ToString().Contains("."))
                    return;
            }

            CMSContext ctx = CMSContext.Create(context);

            // Determine SiteId
            SiteDto.SiteRow row = FindSiteFromUrl(ctx);

            // throw does not exist error
			if (row == null)
				//throw new HttpException(404, "Site does not exist");
				Write(null, null, "StoreClosed.htm");

            ctx.SiteId = row.SiteId;

            string appPath = ctx.Context.Request.ApplicationPath;
            if (appPath == "/")
                appPath = String.Empty;

            string outline = context.Request.RawUrl;
            int folderId = -1;

            if (ctx.AppPath.Length != 1)
                outline = outline.Substring(ctx.AppPath.Length);

            // Check if the request is for the template.aspx
            if (outline.Equals("/template.aspx", StringComparison.OrdinalIgnoreCase))
                ctx.Redirect(CMSContext.Current.ResolveUrl("~"));

            // If empty, we assume it is the site root that we are requesting
            if (String.IsNullOrEmpty(outline))
                outline = "/";

            // Is it a folder?
            if (outline.EndsWith("/") || !outline.Contains("."))
            {
                using (IDataReader reader = FileTreeItem.GetItemByOutlineAll(outline, CMSContext.Current.SiteId))
                {
                    if (reader.Read())
                    {
                        folderId = (int)reader["PageId"];
                    }

                    reader.Close();
                }
                if (folderId != -1)
                {
                    //try to find default page for folder
                    using (IDataReader reader = FileTreeItem.GetFolderDefaultPage(folderId))
                    {
                        if (reader.Read())
                        {
                            string urlPage = String.Empty;
                            if (context.Request.QueryString.Count > 0) urlPage = reader.GetString(2) + "?" + context.Request.QueryString;
                            else urlPage = reader.GetString(2);

                            // Add the relative path
                            if (urlPage.StartsWith("/"))
                                urlPage = "~" + urlPage;

                            // Redirect
                            ctx.Redirect(CMSContext.Current.ResolveUrl(urlPage));
                        }
                        else
                        {
                            reader.Close();
                            throw new HttpException(204, "Default page for folder not found");
                        }

                        reader.Close();
                    }
                }
            }

            // TODO: remove hard coded cookie names and put it into CMS configuration instead
            HttpCookie cookie;
            //CHECK ToolBar/ToolBox visible
			if (context.Request.Cookies[_toolBarVisibleCookieString] != null)
            {
				cookie = (HttpCookie)context.Request.Cookies[_toolBarVisibleCookieString];
                CMSContext.Current.ToolBarVisible = Convert.ToBoolean(cookie.Value);
            }
            //CHECK ToolBox
			if (context.Request.Cookies[_toolBoxVisibleCookieString] != null)
            {
				cookie = (HttpCookie)context.Request.Cookies[_toolBoxVisibleCookieString];
                CMSContext.Current.ToolBoxVisible = Convert.ToBoolean(cookie.Value);
            }

            //CHECK IsDesignMode
			CMSContext.Current.IsDesignMode = CommonHelper.CheckDesignMode(context);

            //CHECK CULTURE
            string currentCulture = string.Empty;
            //CHECK HIDDEN FIELDS
			if (!String.IsNullOrEmpty(context.Request.Form[_currentCultureRequestString]))
            {
				currentCulture = context.Request.Form[_currentCultureRequestString].Trim();
            }
			else if (!String.IsNullOrEmpty(context.Request.QueryString[_currentCultureRequestString]))
            {
				currentCulture = context.Request.QueryString[_currentCultureRequestString].Trim();
            }

            //CHECK QUERYSTRING
			if (!String.IsNullOrEmpty(context.Request.QueryString[_languageQueryString]))
            {
				currentCulture = context.Request.QueryString[_languageQueryString];
            }
            //CHECK VERSION LANGUAGE
			if (!String.IsNullOrEmpty(context.Request.QueryString[_versionIdQueryString]))
            {
                int LangId = -1;
				int versionId = int.Parse(context.Request.QueryString[_versionIdQueryString]);
                //get version language id
                using (IDataReader reader = PageVersion.GetVersionById(versionId))
                {
                    if (reader.Read())
                    {
                        LangId = (int)reader["LangId"];
                    }

                    reader.Close();
                }

                //get language name
                using (IDataReader lang = Language.LoadLanguage(LangId))
                {
                    if (lang.Read())
                    {
                        currentCulture = CultureInfo.CreateSpecificCulture(lang["LangName"].ToString()).Name;
                    }

                    lang.Close();
                }
            }

            if (currentCulture != string.Empty)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(currentCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);
            }
            else
            {
                //CHECK COOKIES
				if (context.Request.Cookies[_currentCultureCookieString] != null)
                {
					cookie = (HttpCookie)context.Request.Cookies[_currentCultureCookieString];
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(cookie.Value);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
                }
                else
                {
                    // culture should be set to the one specified in the web.config file
                    //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                }
            }

            if (!String.IsNullOrEmpty(currentCulture))
            {
                //ConfigurationManager.AppSettings["HtmlEditorControl"];
				cookie = new HttpCookie(_currentCultureCookieString);
                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.Value = Thread.CurrentThread.CurrentCulture.Name;
                context.Response.Cookies.Add(cookie);
            }
        }

        #endregion
    }
}
