using System;
using System.Threading;
using System.Web;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Profile;
using Mediachase.Licensing;
using System.Drawing;
using System.ComponentModel;

namespace Mediachase.Commerce.Manager.Apps.Shell.Modules
{
	public partial class UpTemplate : BaseUserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            LicenseManager.Validate(typeof(Mediachase.MetaDataPlus.MetaObject), null);

			Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "UpTemplate_SetPageTitle", String.Format("CSManagementClient.PageTitleControlId = '{0}';", this.lblPageTitle.ClientID), true);

			string userName = String.Empty;

			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
			{
                ProfileContext context = FrameworkContext.Current.Profile;
                if (context != null && context.Profile != null && context.Profile.Account != null)
                    userName = context.Profile.Account.Name;

                if(String.IsNullOrEmpty(userName))
				    userName = HttpContext.Current.User.Identity.Name; //FrameworkContext.Current.Profile;

				lblUser.Text = String.Format("{0}, {1}", RM.GetString("UPTEMPLATE_WELCOME"), userName);
			}
			else
				lblUser.Text = String.Empty;
			
            
            lblUser.EnableViewState = false;

            // Remove any cached info
            CommerceLicenseInfo[] licenseInfo = CommerceLicensing.GetLicenseInfo();
            if (licenseInfo == null || licenseInfo.Length == 0)
            {
                lblLicenseInfo.Text = "unlicensed version";
                lblLicenseInfo.ForeColor = Color.Red;
                lblLicenseInfo.Font.Bold = true;
            }
            else
            {
                lblLicenseInfo.Text = String.Format("{0} ({1})", licenseInfo[0].Edition, licenseInfo[0].Company);
            }
		}
	}
}