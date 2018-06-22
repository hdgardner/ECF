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
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Core;

namespace Mediachase.Cms.Controls
{
    public partial class Register : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                DataBind();
            }
        }

        /// <summary>
        /// Createds the user.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        public void CreatedUser(object sender, EventArgs e)
        {
            bool emailValidation = false;
            /*
            SiteListItem[] items = ClientList.GetSettings();
            foreach (SiteListItem item in items)
            {
                if (item.Name.CompareTo("EmailValidation") == 0)
                    emailValidation = bool.Parse(ClientHelper.GetAttributeByName("Value", item.ItemAttributes).Value[0]);
            }
             * */

            Label lbl1 = CompleteWizardStep1.ContentTemplateContainer.FindControl("AccountCreated1") as Label;
            if (lbl1 != null) lbl1.Visible = !emailValidation;
            Label lbl2 = CompleteWizardStep1.ContentTemplateContainer.FindControl("AccountCreatedEmailValidation1") as Label;
            if (lbl2 != null) lbl2.Visible = emailValidation;
            Label lbl3 = CompleteWizardStep1.ContentTemplateContainer.FindControl("AccountCreated2") as Label;
            if (lbl3 != null) lbl3.Visible = !emailValidation;
            Label lbl4 = CompleteWizardStep1.ContentTemplateContainer.FindControl("AccountCreatedEmailValidation2") as Label;
            if (lbl4 != null) lbl4.Visible = emailValidation;

            if (emailValidation)
            {
                CreateUserForm.ContinueDestinationPageUrl = ResolveUrl("~/");
            }

            MembershipUser user = Membership.GetUser(CreateUserForm.UserName);

            // Add user to everyone role
            // Check if such role exists
            if (RoleExists(AppRoles.EveryoneRole))
                Roles.AddUserToRole(user.UserName, AppRoles.EveryoneRole);

            if (RoleExists(AppRoles.RegisteredRole))
                Roles.AddUserToRole(user.UserName, AppRoles.RegisteredRole);

            // Now create an account in the ECF 
			ProfileContext.Current.CreateAccountForUser(user);

            /*
            if (!ClientCustomer.AssociateLatestOrder())
                DisplayErrorMessage("Could not associate customer with the order");
             * */
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        private bool RoleExists(string roleName)
        {
            foreach (string role in Roles.GetAllRoles())
            {
                if (role.Equals(roleName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (CMSContext.Current.IsDesignMode)
                divViewer.InnerHtml = "Register Control";
        }
    }
}