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

using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Controls
{
    public partial class Login : BaseStoreUserControl
    {
        /// <summary>
        /// Gets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        public string CustomerLogin
        {
            get
            {
                object obj = Request.QueryString["customer"];
                if (obj != null)
                    return obj.ToString();
                else
                    return null;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
                DataBind();
        }

        /// <summary>
        /// Called when [logging in].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.LoginCancelEventArgs"/> instance containing the event data.</param>
        protected void OnLoggingIn(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when [logged in].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void OnLoggedIn(object sender, EventArgs e)
        {
            // Create cookie manualy when logging in as someone else
            if (!String.IsNullOrEmpty(CustomerLogin) && Roles.IsUserInRole(LoginForm.UserName, AppRoles.AdminRole))
            {
                // Checkif specified account actually exists
                CommonHelper.CreateAuthenticationCookie(CustomerLogin, LoginForm.RememberMeSet);
            }

            // Redirect
            string url = Request.QueryString["ReturnUrl"];

            if (!String.IsNullOrEmpty(url))
                LoginForm.DestinationPageUrl = url;
        }

		protected void LoginForm_Authenticate(object sender, AuthenticateEventArgs e)
		{
			// check user login/password
			if (!Membership.ValidateUser(LoginForm.UserName, LoginForm.Password))
			{
				e.Authenticated = false;
				return;
			}

			// check additional user properties
			bool enabled = true;
			MembershipUser user = Membership.GetUser(LoginForm.UserName);
			if (user != null)
			{
                Mediachase.Commerce.Profile.Account account = Mediachase.Commerce.Profile.ProfileContext.Current.GetAccount(user.ProviderUserKey.ToString());
                if (account == null)
                {
                    account = Mediachase.Commerce.Profile.ProfileContext.Current.CreateAccountForUser(user);
                }

                int accountState = account.State;
				if (accountState == 1 || accountState == 3)
					enabled = false;

				e.Authenticated = enabled;
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
                divViewer.InnerHtml = "Login Control";
        }
    }
}