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

public partial class Login : System.Web.UI.Page
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        LogOn.Focus();
		LogOn.Authenticate += new AuthenticateEventHandler(LogOn_Authenticate);
    }

	void LogOn_Authenticate(object sender, AuthenticateEventArgs e)
	{
		bool enabled = true;
		Mediachase.Commerce.Profile.CustomerProfile profile = Mediachase.Commerce.Profile.ProfileContext.Current.Profile;
		if (profile != null && profile.Account != null)
		{
			if (profile.Account.State == 1 || profile.Account.State == 3)
				enabled = false;
		}

		if (!enabled)
		{
			e.Authenticated = false;
		}
	}
}
