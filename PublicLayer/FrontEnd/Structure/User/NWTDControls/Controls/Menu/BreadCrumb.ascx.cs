using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms;
namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Menu {
	public partial class BreadCrumb : BaseStoreUserControl, ICmsDataAdapter {
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e) {
			DataBind();

			if (CatalogSiteMap.Provider.CurrentNode != CatalogSiteMap.Provider.RootNode) {
				StoreSiteMap.Visible = false;
				CatalogSiteMap.Visible = true;
			} else {
				StoreSiteMap.Visible = true;
				CatalogSiteMap.Visible = false;
			}

			if (StoreSiteMap.Provider.CurrentNode == StoreSiteMap.Provider.RootNode)
				StoreSiteMap.Visible = false;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
		}

		#region ICmsDataAdapter Members
		public void SetParamInfo(object param) {
		}
		#endregion
	}
}