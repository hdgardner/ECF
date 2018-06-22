using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Cms;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Shared;

using ca = ComponentArt.Web.UI;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Controls
{
    public partial class ToolbarControl : Mediachase.Web.UI.BaseLocalizableControl
    {
        #region property: VersionId
        /// <summary>
        /// Gets the version id.
        /// </summary>
        /// <value>The version id.</value>
        public int VersionId
        {
            get { return CMSContext.Current.VersionId; }
            //set { versionId = value; }
        }
        #endregion

        #region Page_Load
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            CommonHelper.AddLinkedStyleSheet(this.Page, "~/Structure/Base/Controls/Toolbar/styles/LoginMenuStyle.css");
            CommonHelper.AddLinkedStyleSheet(this.Page, "~/Structure/Base/Controls/Toolbar/styles/Toolbars.css");
            CommonHelper.AddLinkedStyleSheet(this.Page, "~/Structure/Base/Controls/Toolbar/styles/ToolbarMenuStyle.css");
            CommonHelper.AddLinkedStyleSheet(this.Page, "~/Structure/Base/Controls/Toolbar/styles/ToolbarStyle.css");
            CommonHelper.AddLinkedStyleSheet(this.Page, "~/Structure/Base/Controls/Toolbar/styles/ToolbarDrag.css");

            // Register Javascripts
            Page.ClientScript.RegisterClientScriptInclude(typeof(ToolbarControl), "ToolbarClient", Page.ResolveClientUrl("~/Structure/Base/Controls/Toolbar/Scripts/ToolbarClient.js"));
            //Page.ClientScript.RegisterClientScriptInclude(typeof(ToolbarControl), "TbSettings", Page.ResolveClientUrl("~/Structure/Base/Controls/Toolbar/Scripts/TbSettings.js"));
            //Page.ClientScript.RegisterClientScriptInclude(typeof(ToolbarControl), "ToolBar", Page.ResolveClientUrl("~/Structure/Base/Controls/Toolbar/Scripts/ToolBar.js"));
            //Page.ClientScript.RegisterClientScriptInclude(typeof(ToolbarControl), "ToolBox", Page.ResolveClientUrl("~/Structure/Base/Controls/Toolbar/Scripts/CMS_ToolBox.js"));
			Page.ClientScript.RegisterClientScriptInclude(typeof(ToolbarControl), "DragToolBox", Page.ResolveClientUrl("~/Structure/Base/Controls/Toolbar/Scripts/Drag.js"));

            if (CMSContext.Current.IsDesignMode)
                TopToolBar.SelectedIndex = 1;
            else
                TopToolBar.SelectedIndex = 0;

            BindToolBar();

            if (!this.IsPostBack)
            {
                BindSites();
                LoginMenu.Items[0].Items[0].NavigateUrl = ResolveUrl("~/login.aspx");
                LoginMenu.Items[0].Items[1].NavigateUrl = ResolveUrl("~/logout.aspx");
                LoginMenu.DataBind();
            }
        }

        /// <summary>
        /// Handles the Error event of the Instance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Mediachase.Cms.WebUtility.UI.ErrorEventArgs"/> instance containing the event data.</param>
        private void Instance_Error(object sender, ErrorEventArgs e)
        {
            ErrorCtrl.InnerHtml = e.Message;
            ErrorCtrl.Visible = true;
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance can edit.
        /// </summary>
        /// <value><c>true</c> if this instance can edit; otherwise, <c>false</c>.</value>
        private bool CanEdit
        {
            get
            {
                if (!Page.User.Identity.IsAuthenticated)
                    return false;

                bool enable = false;
                if (ProfileConfiguration.Instance.EnablePermissions)
                {
                    enable = ProfileContext.Current.CheckPermission("content:site:nav:mng:design");
                }
                else
                {
                    enable = SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole, CmsRoles.ManagerRole, CmsRoles.EditorRole });
                }

                return enable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can admin.
        /// </summary>
        /// <value><c>true</c> if this instance can admin; otherwise, <c>false</c>.</value>
        private bool CanAdmin
        {
            get
            {
                if (!Page.User.Identity.IsAuthenticated)
                    return false;

                bool enable = false;
                if (ProfileConfiguration.Instance.EnablePermissions)
                {
                    enable = ProfileContext.Current.CheckPermission("content:site:mng:view") && ProfileContext.Current.CheckPermission("core:mng:login");
                }
                else
                {
                    enable = SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole, CmsRoles.ManagerRole });
                }

                return enable;
            }
        }

        #region BindToolBar
        /// <summary>
        /// Binds the tool bar.
        /// </summary>
        private void BindToolBar()
        {
            int folderId = 0;
            using (IDataReader reader = FileTreeItem.LoadParent(CMSContext.Current.PageId))
            {
                if (reader.Read())
                    folderId = reader.GetInt32(0);

                reader.Close();
            }

            if (CanEdit)
            {
                Tabs.Tabs[1].Visible = true;
            }
            else
            {
                Tabs.Tabs[1].Visible = false;
            }

            if (CanAdmin)
            {
                string adminUrl = GlobalVariable.GetVariable("cm_url", CMSContext.Current.SiteId);
                if (!String.IsNullOrEmpty(adminUrl))
                {
                    string url = String.Format("{0}/default.aspx?_a=Content&_v=Folder-List&folderid={1}&siteid={2}", adminUrl, folderId.ToString(), CMSContext.Current.SiteId);//, //Membership.GetUser(Page.User.Identity.Name).ProviderUserKey.ToString());
                    Tabs.Tabs[2].NavigateUrl = url;
                }
                else
                {
                    Tabs.Tabs[2].Visible = false;
                }
            }
            else
            {
                Tabs.Tabs[2].Visible = false;
            }
        }
        #endregion

        /// <summary>
        /// Binds the sites.
        /// </summary>
        private void BindSites()
        {
            SiteDto sites = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId);

            if(sites.Site.Count > 1)
            {
                ca.MenuItem sitesRoot = new ca.MenuItem();
                sitesRoot.Text = "Sites";
                sitesRoot.LookId = "TopItemLook";

                foreach(SiteDto.SiteRow row in sites.Site.Rows)
                {
                    ca.MenuItem siteItem = new ca.MenuItem();
                    siteItem.Text = row.Name;
                    string url = GlobalVariable.GetVariable("url", row.SiteId);
                    siteItem.NavigateUrl = url;                    
                    siteItem.ClientTemplateId = "DescriptionTemplate";
                    siteItem.Attributes["Description"] = row.Description;
                    sitesRoot.Items.Add(siteItem);
                }

                LoginMenu.Items.Insert(1, sitesRoot);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            ErrorManager.Instance.Error += new ErrorEventHandler(Instance_Error);
            base.OnInit(e);            
        }


        #region  Event Handlers (DesignMode)
        #region btnSave_Click
        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnSave_Click(object sender, EventArgs e)
        {
        }
        #endregion

        #endregion

        #region DeleteInvisible_Click
        /// <summary>
        /// Handles the Click event of the Logout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Logout_Click(object sender, EventArgs e)
        {
            System.Web.Security.FormsAuthentication.SignOut();
            if (Request.QueryString["CloseWindow"] != null)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script>window.close();</script>");
            }
            else
            {
                Response.Redirect("~" + CMSContext.Current.Outline);
            }
        }
        #endregion

    }
}