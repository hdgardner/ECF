using System;
using System.Collections.Generic;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Apps.Content.Site.Tabs
{
    public partial class SiteImportConfirmTab : BaseUserControl
    {
        private Guid SiteId
        {
            get
            {
                string _siteId = (string)Request.QueryString["SiteId"];
                if (!String.IsNullOrEmpty(_siteId))
                {
                    return new Guid(_siteId);
                }

                return Guid.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SiteDto sites = CMSContext.Current.GetSitesDto(GetCurrentApplicationId());
                ddlExistingSite.DataValueField = "SiteId";
                ddlExistingSite.DataTextField = "Name";
                ddlExistingSite.DataSource = sites.Site;
                ddlExistingSite.DataBind();

                if (!SiteId.Equals(Guid.Empty))
                {
                    for (int i = 0; i < ddlExistingSite.Items.Count; i++)
                    {
                        if (ddlExistingSite.Items[i].Value.Equals(SiteId.ToString()))
                        {
                            ddlExistingSite.SelectedIndex = i;
                            rbExistingSite.Checked = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current application id.
        /// </summary>
        /// <returns></returns>
        private Guid GetCurrentApplicationId()
        {
            return CmsConfiguration.Instance.ApplicationId;
        }

        public void btnDoImport_Click(object sender, EventArgs e)
        {
            CommandParameters cp = new CommandParameters("cmdDoImport");
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (rbExistingSite.Checked)
                dic["SiteId"] = ddlExistingSite.SelectedValue;
            else
                dic["SiteId"] = Guid.Empty.ToString();

            cp.CommandArguments = dic;

            CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
        }

    }
}