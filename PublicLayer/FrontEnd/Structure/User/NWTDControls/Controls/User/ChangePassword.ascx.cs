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
using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Cms.Dto;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	/// <summary>
	/// This control allows a user to change his or her passowrd. It was, for the most part copied from the ECF-supplied control
	/// </summary>
	public partial class ChangePassword : BaseStoreUserControl
	{
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				ChangePwd.ContinueDestinationPageUrl = ResolveUrl("~/Profile/AccountInfo.aspx");
			}
		}

		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			if (CMSContext.Current.IsDesignMode)
				divViewer.InnerHtml = "Change Password Control";
		}

		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}
	}
}