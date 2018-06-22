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
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Profile;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Profile.Tabs
{
    public partial class OrgOverviewEditTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the principal id.
        /// </summary>
        /// <value>The principal id.</value>
        public Guid PrincipalId
        {
            get
            {
                if (Request.QueryString["id"] != null)
                    return new Guid(Request.QueryString["id"]);
                else
                    return Guid.Empty;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindForm();
            }
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {

            if (PrincipalId != Guid.Empty)
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("profile:org:mng:edit");

                Organization org = ProfileContext.Current.GetOrganization(PrincipalId);

                if (org != null)
                {
                    Name.Text = org.Name;
                    Description.Text = org.Description;

                    ProfileSearchParameters pars = new ProfileSearchParameters();
                    ProfileSearchOptions options = new ProfileSearchOptions();
                    pars.SqlMetaWhereClause = String.Format("OrganizationId = {0}", org.Id);
                    options.Namespace = "Mediachase.Commerce.Profile";
                    options.Classes.Add("Account");

                    Account[] accounts = ProfileContext.Current.FindAccounts(pars, options);
                    OrgMemberList.DataSource = accounts;
                    OrgMemberList.DataBind();

                    ManagementHelper.SelectListItem(AccountState, org.State);
                }
            }
            else
            {
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("profile:org:mng:create");
            }
        }

        /// <summary>
        /// Handles the Command event of the DeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void DeleteButton_Command(object sender, CommandEventArgs e)
        {
            Guid principalId = new Guid(e.CommandArgument.ToString());
            Account acc = ProfileContext.Current.GetAccount(principalId);
            if (acc != null)
            {
                acc.OrganizationId = 0;
                acc.AcceptChanges();
            }

            BindForm();
        }


        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            Organization org = ProfileContext.Current.GetOrganization(PrincipalId);

            if (org == null)
            {
                org = new Organization();
            }

            org.Name = Name.Text;
            org.Description = Description.Text;
            org.State = Int32.Parse(AccountState.SelectedValue);

            org.AcceptChanges();
        }

        #endregion
    }
}