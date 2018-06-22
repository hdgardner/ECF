using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Content.Site
{
    public partial class SiteList : BaseUserControl
    {
        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get
            {
                return new Guid(Request.QueryString["SiteId"].ToString());
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
        }

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
            MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);
			base.OnInit(e);
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind(string sortExpression)
        {
			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("SiteId", "Name");
            MyListView.DataSource = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId, true).Site;
            DataBind();
        }

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}