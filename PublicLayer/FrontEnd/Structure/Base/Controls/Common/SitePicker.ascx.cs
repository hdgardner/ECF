namespace Mediachase.Cms.Web.Common
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Collections;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Threading;
	using Mediachase.Commerce.Shared;
    using System.Globalization;
    using System.Collections.Generic;
    using Mediachase.Cms.Dto;
    using Mediachase.Cms.Util;
    using System.Collections.Specialized;

    public partial class SitePicker : System.Web.UI.UserControl
    {
        #region BindToolBar()
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
        public void BindToolbar()
        {
            SiteDto sites = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId);

            if (sites.Site.Count > 1)
            {
                ComponentArt.Web.UI.MenuItem sitesRoot = new ComponentArt.Web.UI.MenuItem();
                sitesRoot.Text = "Sites";
                sitesRoot.LookId = "TopItemLook";

                foreach (SiteDto.SiteRow row in sites.Site.Rows)
                {
                    ComponentArt.Web.UI.MenuItem siteItem = new ComponentArt.Web.UI.MenuItem();
                    siteItem.Text = row.Name;
                    string url = GlobalVariable.GetVariable("url", row.SiteId);
                    siteItem.NavigateUrl = url;
                    siteItem.ClientTemplateId = "DescriptionTemplate";
                    siteItem.Attributes["Description"] = row.Description;
                    sitesRoot.Items.Add(siteItem);
                }

                SitesMenu.Items.Add(sitesRoot);
                SitesMenu.DataBind();
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindToolbar();
        }
    }
}