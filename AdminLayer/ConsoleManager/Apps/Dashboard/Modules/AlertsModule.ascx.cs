using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Catalog;
using System.Collections.Generic;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders;
using Mediachase.Cms;
using Mediachase.Commerce.Core;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Search;
using System.IO;

namespace Mediachase.Commerce.Manager.Dashboard.Modules
{
    /// <summary>
    /// This module display various alerts related to the current system configuration.
    /// </summary>
	public partial class AlertsModule : BaseUserControl
	{
        public struct AlertLevel
        {
            public const string Warning = "warning";
            public const string Info = "info";
            public const string Error = "error";
        }

        /// <summary>
        /// Contains alert information
        /// </summary>
        public class Alert
        {
            string _Text = String.Empty;

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            public string Text
            {
                get { return _Text; }
                set { _Text = value; }
            }

            string _Level = AlertLevel.Info;

            /// <summary>
            /// Gets or sets the level.
            /// </summary>
            /// <value>The level.</value>
            public string Level
            {
                get { return _Level; }
                set { _Level = value; }
            }


            /// <summary>
            /// Initializes a new instance of the <see cref="Alert"/> class.
            /// </summary>
            public Alert()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Alert"/> class.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="level">The level.</param>
            public Alert(string text, string level)
            {
                _Text = text;
                _Level = level;
            }
        }
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            ICatalogSystem catalogContext = CatalogContext.Current;
            ProfileContext profileContext = ProfileContext.Current;
            OrderContext orderContext = OrderContext.Current;

            List<Alert> alerts = GetAlertsDataSource();
            if (alerts.Count > 0)
                this.AlertsList.DataSource = alerts;
            this.AlertsList.DataBind();
		}

        /// <summary>
        /// Gets the alerts data source.
        /// </summary>
        /// <returns></returns>
        private List<Alert> GetAlertsDataSource()
        {
            List<Alert> alerts = new List<Alert>();

            foreach (Alert alert in GetContentManagementAlerts())
            {
                if (alert != null)
                    alerts.Add(alert);
            }

            foreach (Alert alert in GetCatalogManagementAlerts())
            {
                if (alert != null)
                    alerts.Add(alert);
            }

            foreach (Alert alert in GetSearchIndexAlerts())
            {
                alerts.Add(alert);
            }
            
            return alerts;
        }


        /// <summary>
        /// Gets the content management alerts.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Alert> GetContentManagementAlerts()
        {
            bool isAdmin = false;

            if (ProfileConfiguration.Instance.EnablePermissions)
                isAdmin = ProfileContext.Current.CheckPermission("content:site:mng:edit");
            else
                isAdmin = SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole });

            // Check if there are any sites
            SiteDto sitedto = CMSContext.Current.GetSitesDto(AppContext.Current.ApplicationId);
            if (sitedto.Site.Count == 0)
            {
                if(isAdmin)
                    yield return new Alert(String.Format("No sites found, please create a new <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-List\")'>here</a>  or import a sample site <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Import\")'>here</a>."), AlertLevel.Error);
                else
                    yield return new Alert(String.Format("No sites configured, please contact site admin."), AlertLevel.Error);
            }
            else // check if there active sites
            {
                bool foundActive = false;
                bool foundDefault = false;
                foreach (SiteDto.SiteRow site in sitedto.Site)
                {
                    if (site.IsActive)
                        foundActive = true;
                    if (site.IsDefault)
                        foundDefault = true;

                    // Check if urls are configured correctly, since that will cause some functions not to work correctly
                    bool? urlConfigured = null;
                    bool urlLocalhostConfigured = false;
                    bool? foundAnalytics = null;
                    SiteDto.main_GlobalVariablesRow[] varRows = site.Getmain_GlobalVariablesRows();

                    if (varRows.Length > 0)
                    {
                        foreach (SiteDto.main_GlobalVariablesRow varRow in varRows)
                        {
                            if (varRow.KEY.Equals("url", StringComparison.OrdinalIgnoreCase))
                            {
                                if (String.IsNullOrEmpty(varRow.VALUE))
                                {
                                    urlConfigured = false;
                                    continue;
                                }
                                else
                                {
                                    urlConfigured = true;

                                    if (varRow.VALUE.Contains("localhost"))
                                        urlLocalhostConfigured = true;

                                    continue;
                                }
                            }

                            if (varRow.KEY.Equals("cm_url", StringComparison.OrdinalIgnoreCase))
                            {
                                if (String.IsNullOrEmpty(varRow.VALUE))
                                {
                                    urlConfigured = false;
                                    continue;
                                }
                                else
                                {
                                    urlConfigured = true;
                                    continue;
                                }

                            }

                            if (varRow.KEY.Equals("page_include", StringComparison.OrdinalIgnoreCase))
                            {
                                if (String.IsNullOrEmpty(varRow.VALUE))
                                {
                                    foundAnalytics = false;
                                    continue;
                                }
                                else
                                {
                                    foundAnalytics = true;
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        foundAnalytics = false;
                        urlConfigured = false;
                    }

                    /*
                    if (urlConfigured == null || !(bool)urlConfigured)
                    {
                        if (isAdmin)
                            yield return new Alert(String.Format("The \"{0}\" site does not have Public or Admin URLs configured correctly. This will cause certain features to not work correctly. Please correct it by going <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Edit\", \"siteid={1}\")'>here</a>.", site.Name, site.SiteId), AlertLevel.Error);
                        else
                            yield return new Alert(String.Format("The \"{0}\" site does not have Public or Admin URLs configured correctly. Please contact site admin.", site.Name), AlertLevel.Error);
                    }
                     * */

                    if (foundAnalytics == null || !(bool)foundAnalytics)
                    {
                        if (isAdmin)
                            yield return new Alert(String.Format("The <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Edit\", \"siteid={1}\")'>\"{0}\"</a> site does not have Analytics configured, it is very important for a site to track visitors and such statistic as conversion rate. Please correct it by going <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Edit\", \"siteid={1}\")'>here</a> or you can read more about this topic <a target=\"_blank\" href=\"http://docs.mediachase.com/doku.php?id=ecf:50:operations:analytics\">here</a>.", site.Name, site.SiteId), AlertLevel.Warning);
                        else
                            yield return new Alert(String.Format("The \"{0}\" site does not have Analytics configured, it is very important for a site to track visitors and such statistic as conversion rate. Please contact site admin or read more about this topic <a target=\"_blank\" href=\"http://docs.mediachase.com/doku.php?id=ecf:50:operations:analytics\">here</a>.", site.Name, site.SiteId), AlertLevel.Warning);
                    }

                    if (urlLocalhostConfigured)
                    {
                        if (isAdmin)
                            yield return new Alert(String.Format("The <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Edit\", \"siteid={1}\")'>\"{0}\"</a> site has a public URL configured as localhost which will not work correctly for external users. Please correct it by going <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-Edit\", \"siteid={1}\")'>here</a>.", site.Name, site.SiteId), AlertLevel.Warning);
                        else
                            yield return new Alert(String.Format("The \"{0}\" site has a public URL configured as localhost which will not work correctly for external users. Please contact site admin or read more about this topic <a target=\"_blank\" href=\"http://docs.mediachase.com/doku.php?id=ecf:50:operations:analytics\">here</a>.", site.Name, site.SiteId), AlertLevel.Warning);
                    }
                }

                if(!foundActive)
                {
                    if(isAdmin)
                        yield return new Alert(String.Format("No active sites found, please make atleast one site active <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-List\")'>here</a>."), AlertLevel.Warning);
                    else
                        yield return new Alert(String.Format("No active sites configured, please contact site admin."), AlertLevel.Warning);
                }

                if (!foundDefault)
                {
                    if(isAdmin)
                        yield return new Alert(String.Format("No default site found, please make one site default <a href='javascript:CSManagementClient.ChangeView(\"Content\", \"Site-List\")'>here</a>."), AlertLevel.Warning);
                    else
                        yield return new Alert(String.Format("No default sites configured, please contact site admin."), AlertLevel.Warning);
                }

            }
            yield return null;
        }

        /// <summary>
        /// Gets the catalog management alerts.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Alert> GetCatalogManagementAlerts()
        {
            bool isAdmin = false;
            bool isManager = false;
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                isAdmin = ProfileContext.Current.CheckPermission("catalog:ctlg:mng:edit");
                isManager = ProfileContext.Current.CheckPermission("catalog:ctlg:mng:edit");
            }
            else
            {
                isAdmin = SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole });
                isManager = SecurityManager.CheckPermission(new string[] { CatalogRoles.CatalogManagerRole });
            }

            // Check if there are any catalogs
            CatalogDto catalogdto = CatalogContext.Current.GetCatalogDto();
            if (catalogdto.Catalog.Count == 0)
            {
                if (isAdmin || isManager)
                    yield return new Alert(String.Format("No catalogs found, please create a new catalog <a href='javascript:CSManagementClient.ChangeView(\"Catalog\", \"Edit\")'>here</a> or import a sample catalog <a href='javascript:CSManagementClient.ChangeView(\"Catalog\", \"Catalog-Import\")'>here</a>."), AlertLevel.Error);
                else
                    yield return new Alert(String.Format("No catalogs configured, please catalog admin or catalog manager."), AlertLevel.Error);
            }
            else // check if there active sites
            {
                bool foundActive = false;
                foreach (CatalogDto.CatalogRow catalog in catalogdto.Catalog)
                {
                    if (catalog.IsActive && catalog.StartDate <= DateTime.UtcNow && catalog.EndDate > DateTime.UtcNow)
                        foundActive = true;
                }

                if (!foundActive)
                {
                    if (isAdmin || isManager)
                        yield return new Alert(String.Format("No active catalogs found, please make atleast one catalog active <a href='javascript:CSManagementClient.ChangeView(\"Catalog\", \"Catalog-List\")'>here</a> by setting it to active and making sure start and end dates fall within today."), AlertLevel.Warning);
                    else
                        yield return new Alert(String.Format("No active catalogs configured, please contact catalog admin or catalog manager."), AlertLevel.Warning);
                }
            }
            yield return null;
        }

        /// <summary>
        /// Gets the search index alerts.
        /// </summary>
        /// <returns></returns>
        private Alert[] GetSearchIndexAlerts()
        {
            bool isAdmin = false;

            if (ProfileConfiguration.Instance.EnablePermissions)
                isAdmin = ProfileContext.Current.CheckPermission("core:mng:search");
            else
                isAdmin = SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole });

            SearchManager manager = new SearchManager(AppContext.Current.ApplicationName);

            List<Alert> alerts = new List<Alert>();
            try
            {
                manager.CheckConfiguration();
            }
            catch (DirectoryNotFoundException ex)
            {
                alerts.Add(new Alert(String.Format("Directory \"{0}\" configured for indexing does not exist. Please create that folder with read/write permissions.", ex.Message), AlertLevel.Error));
            }
            catch (UnauthorizedAccessException ex)
            {
                alerts.Add(new Alert(String.Format("Directory \"{0}\" configured for indexing does not have correct read/write permissions.", ex.Message), AlertLevel.Error));
            }

            try
            {
                manager.CheckIndexes();
            }
            catch (FileNotFoundException)
            {
                if(isAdmin)
                    alerts.Add(new Alert(String.Format("No search indexes exist. This will cause public site search to not work, you can build indexes by going <a href='javascript:CSManagementClient.ChangeView(\"Core\", \"Search\")'>here</a>."), AlertLevel.Error));
                else
                    alerts.Add(new Alert(String.Format("No search indexes exist. This will cause public site search to not work, please contact administrator."), AlertLevel.Error));

            }

            return alerts.ToArray();
        }


        /// <summary>
        /// Makes the internal URL.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        private string MakeInternalUrl(string app, string view)
        {
            return this.ResolveClientUrl(String.Format("~/admin/Apps/Shell/Pages/ContentFrame.aspx?_a={0}&_v={1}", app, view));
        }
	}
}