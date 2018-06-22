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
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Controls
{
	public partial class ChangeEMail : BaseStoreUserControl
	{
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ErrorLabel.Text = "";
			if (!IsPostBack)
			{
				if (ProfileContext.Current.User != null)
					tbEMail.Text = ProfileContext.Current.User.Email;

				DataBind();
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
				divViewer.InnerHtml = "Change EMail Control";
		}

		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected void EMailButton_Click(object sender, EventArgs e)
		{
			bool error = false;

			MembershipUser user = ProfileContext.Current.User;
			if (user != null)
			{
				user.Email = tbEMail.Text;

				try
				{
					Membership.UpdateUser(user);
				}
				catch(Exception ex)
				{
					ErrorManager.GenerateError(ex.Message);
					error = true;
				}
			}
			else
			{
				ErrorManager.GenerateError("Current user not found!");
				error = true;
			}

			if (!error)
				ErrorLabel.Text = RM.GetString("ACCOUNT_EMAIL_SUCCESSFULLY_CHANGED");
		}
	}
}