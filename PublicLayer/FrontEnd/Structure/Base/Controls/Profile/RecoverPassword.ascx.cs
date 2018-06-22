using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Mediachase.Cms.Dto;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Commerce.Engine.Template;
using System.Threading;
using System.Web.Security;

namespace Mediachase.Cms.Controls
{
    public partial class RecoverPassword : BaseStoreUserControl
    {
		private int _defaultRecoveryBody_PasswordStringIndex = 0;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            SiteDto dto = CMSContext.Current.GetSiteDto(CMSContext.Current.SiteId);
            PasswordRecovery1.MailDefinition.From = String.Format("\"{0}\"<{1}>", dto.Site[0].Name, GlobalVariable.GetVariable("email", CMSContext.Current.SiteId));

			// get index of string <% Password %>; will be used in PasswordRecovery1_SendingMail method for getting actual password
			System.Resources.ResourceManager rm = new System.Resources.ResourceManager("System.Web", typeof(System.Web.HttpApplication).Assembly);
			string s = rm.GetString("PasswordRecovery_DefaultBody");
			_defaultRecoveryBody_PasswordStringIndex = s.LastIndexOf("<%Password%>");
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

        /// <summary>
        /// Cancels the email.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.MailMessageEventArgs"/> instance containing the event data.</param>
        protected void CancelEmail(Object sender, MailMessageEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Sends the mail error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.SendMailErrorEventArgs"/> instance containing the event data.</param>
        protected void SendMailError(Object sender, SendMailErrorEventArgs e)
        {
            ErrorManager.GenerateError(e.Exception.Message);
            e.Handled = true;
        }

        /// <summary>
        /// Users the verify.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.LoginCancelEventArgs"/> instance containing the event data.</param>
        protected void UserVerify(Object sender, LoginCancelEventArgs e)
        {
            /*MembershipUser user = Membership.GetUser(PasswordRecovery1.UserName);
            if (user == null)
            {
                e.Cancel = true;
            }*/
            //PasswordRecovery1.UserName = Membership.GetUserNameByEmail(PasswordRecovery1.UserName);
        }

        /// <summary>
        /// Answers the verify.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.LoginCancelEventArgs"/> instance containing the event data.</param>
        protected void AnswerVerify(Object sender, LoginCancelEventArgs e)
        {
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
			PasswordRecovery1.SendingMail += new MailMessageEventHandler(PasswordRecovery1_SendingMail);
            base.OnInit(e);
        }

		void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
		{
			// Add input parameter
			Dictionary<string, object> dic = new Dictionary<string, object>();
			
			//string newPwd = e.Message.Body.Substring(e.Message.Body.LastIndexOf("\nPassword:") + "\nPassword:".Length).Trim().TrimEnd('\n');
			int dif = PasswordRecovery1.UserName.Length - "<%UserName%>".Length;
			string newPwd = e.Message.Body.Substring(_defaultRecoveryBody_PasswordStringIndex + dif).Trim().TrimEnd('\n');
			UserCredentials credentials = new UserCredentials() { User = PasswordRecovery1.UserName, Password = newPwd };
			dic.Add("credentials", credentials);

			e.Message.IsBodyHtml = true;
			e.Message.Body = TemplateService.Process("profile-remindpassword", Thread.CurrentThread.CurrentCulture, dic);
		}
        #endregion

		#region UserCredentials class
		public class UserCredentials
		{
			public string User;
			public string Password;
		}
		#endregion
	}
}