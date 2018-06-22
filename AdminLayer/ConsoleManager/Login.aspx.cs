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
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.Commerce.Manager
{
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoginCtrl.Authenticate += new AuthenticateEventHandler(LoginCtrl_Authenticate);

			/*
            int total = 0;
            // Determine if there are users in the system
            MembershipUserCollection users = Membership.GetAllUsers(0, 1, out total);

            
            RegisterPanel.Visible = false;
            LoginPanel.Visible = true;
            if (users.Count == 0)
            {
                RegisterPanel.Visible = true;
                LoginPanel.Visible = false;
            }
             * */
        }

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreInit(object sender, EventArgs e)
		{
			//if (String.Compare(Request.Browser.Type, "IE8", StringComparison.OrdinalIgnoreCase) != 0)
			//    LoginCtrl.SkinID = "LoginSkinCommon";
			//else
			//    LoginCtrl.SkinID = "LoginSkinIE8";
		}

        /// <summary>
        /// Handles the Authenticate event of the LoginCtrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.AuthenticateEventArgs"/> instance containing the event data.</param>
        void LoginCtrl_Authenticate(object sender, AuthenticateEventArgs e)
        {
            string appName = ((TextBox)LoginCtrl.FindControl("Application")).Text;
            AppDto dto = AppContext.Current.GetApplicationDto(appName);

            // If application does not exists or is not activa, prevent login
            if (dto == null || dto.Application.Count == 0 || !dto.Application[0].IsActive)
            {
                LogManager.WriteLog("LOGIN", LoginCtrl.UserName, "login.aspx", "Commerce Manager", "SYSTEM", "Application name is incorrect.", false);
                LoginCtrl.FailureText = "Login failed. Please try again.";
                return;
            }

            Membership.Provider.ApplicationName = appName;

            if (Membership.ValidateUser(LoginCtrl.UserName, LoginCtrl.Password))
            {
                CHelper.CreateAuthenticationCookie(LoginCtrl.UserName, appName, LoginCtrl.RememberMeSet);
                string url = FormsAuthentication.GetRedirectUrl(LoginCtrl.UserName, LoginCtrl.RememberMeSet);
                LogManager.WriteLog("LOGIN", LoginCtrl.UserName, "login.aspx", "Commerce Manager", "SYSTEM", String.Empty, true);
                if (url.Contains(".axd") || url.Contains("/Apps/Core/Controls/Uploader/")) // prevent redirecting to report files
                    Response.Redirect("~/default.aspx");
                else
                    Response.Redirect(url);
            }
            else
            {
                LogManager.WriteLog("LOGIN", LoginCtrl.UserName, "login.aspx", "Commerce Manager", "SYSTEM", "Login or password are incorrect.", false);
                LoginCtrl.FailureText = "Login failed. Please try again.";
            }
        }
    }
}
