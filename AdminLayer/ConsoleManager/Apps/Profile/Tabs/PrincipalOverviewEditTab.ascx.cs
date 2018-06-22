using System;
using System.Collections;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.Commerce.Manager.Profile.Tabs
{
    public partial class PrincipalOverviewEditTab : ProfileBaseUserControl, IAdminTabControl
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
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public Guid UserId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["mode"]) && String.Compare(Request.QueryString["mode"].ToString(), "myprofile", true) == 0)
                {
                    return ProfileContext.Current.UserId;
                }
                else
                {
                    if (Request.QueryString["userid"] != null)
                        return new Guid(Request.QueryString["userid"]);

                    return Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //first check permissions;if permissions not present, deny
            if (PrincipalId == Guid.Empty)
                SecurityManager.CheckRolePermission("profile:acc:mng:create");
            else
                SecurityManager.CheckRolePermission("profile:acc:mng:edit");

			PasswordCtrl.ChangedPassword += new EventHandler(PasswordCtrl_ChangedPassword);

            OrganizationFilter.DataRequested += new ComboBox.DataRequestedEventHandler(OrganizationFilter_DataRequested);
            if (!Page.IsPostBack && !OrganizationFilter.CausedCallback)
            {
                LoadOrganizationItems(0, OrganizationFilter.DropDownPageSize * 2, "");
                BindForm();
            }
        }

		void PasswordCtrl_ChangedPassword(object sender, EventArgs e)
		{
			if (String.Compare(PasswordCtrl.UserName, ProfileContext.Current.UserName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				// re-set authentication cookie
				if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
				{
					FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
					CHelper.CreateAuthenticationCookie(ticket.Name, Membership.Provider.ApplicationName, ticket.IsPersistent);
				}
			}
		}

        /// <summary>
        /// Handles the DataRequested event of the OrganizationFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void OrganizationFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadOrganizationItems(args.StartIndex, args.NumItems, args.Filter);
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (!ProfileContext.Current.CheckPermission("profile:acc:mng:roles"))
                RolesList.Enabled = false;
            else
                RolesList.Enabled = true;

            if (PrincipalId != Guid.Empty)
            {
                Account account = ProfileContext.Current.GetAccount(PrincipalId);

                if (account != null)
                {
                    Name.Text = account.Name;
                    Description.Text = account.Description;
                    CustomerGroup.Text = account.CustomerGroup;

					ManagementHelper.SelectListItem(AccountState, account.State);

                    ComboBoxItem item = OrganizationFilter.Items.FindByValue(account.OrganizationId.ToString());
                    if (item != null)
                        OrganizationFilter.SelectedItem = item;

                    if (!String.IsNullOrEmpty(account.ProviderKey))
                        BindMembershipForm(new Guid(account.ProviderKey));
                }
                else
                {
                    DisplayErrorMessage("Specified account was not found, use this form to create a new account.");
                    ActionTd.Visible = false;
                    BindMembershipForm(null);
                }
            }
            else if (UserId != Guid.Empty)
            {
                Account account = ProfileContext.Current.GetAccount(UserId.ToString());
                if (account != null)
                {
                    Name.Text = account.Name;
                    Description.Text = account.Description;
                    CustomerGroup.Text = account.CustomerGroup;

                    ManagementHelper.SelectListItem(AccountState, account.State);
                    ComboBoxItem item = OrganizationFilter.Items.FindByValue(account.OrganizationId.ToString());
                    if (item != null)
                        OrganizationFilter.SelectedItem = item;
                }

                BindMembershipForm(UserId);
            }
            else
            {
                ActionTd.Visible = false;
                BindMembershipForm(null);
            }
        }

        /// <summary>
        /// Binds the membership form.
        /// </summary>
        /// <param name="id">The id.</param>
        private void BindMembershipForm(object id)
        {
            RolesList.DataSource = Roles.GetAllRoles();
            RolesList.DataBind();

			// Always enable everyone role
			ListItem item = RolesList.Items.FindByValue(AppRoles.EveryoneRole);
			if (item != null)
			{
				item.Selected = true;
				item.Enabled = false;
			}

			// Always enable registered role
			item = RolesList.Items.FindByValue(AppRoles.RegisteredRole);
			if (item != null)
			{
				item.Selected = true;
				item.Enabled = false;
			}

            MembershipUser user = null;
            if(id != null)
                user = Membership.GetUser((Guid)id);
            if (user != null)
            {
                ActionTd.Visible = true;
                PasswordTr.Visible = false;
                RecoveryCtrl.UserName = user.UserName;
                PasswordCtrl.UserName = user.UserName;
				IsApproved.IsSelected = user.IsApproved;
                IsLockedOut.IsSelected = user.IsLockedOut;
				LastActivityDate.Text = ManagementHelper.GetUserDateTime(user.LastActivityDate).ToString();
                LastLockoutDate.Text = ManagementHelper.GetUserDateTime(user.LastLockoutDate).ToString();
                LastLoginDate.Text = ManagementHelper.GetUserDateTime(user.LastLoginDate).ToString();
                LastPasswordChangedDate.Text = ManagementHelper.GetUserDateTime(user.LastPasswordChangedDate).ToString();
                UserNameTextBox.Text = user.UserName;
                UserNameTextBox.Enabled = false;
                EmailText.Text = user.Email;
                IsLockedOut.Enabled = false;
                CommentTextBox.Text = user.Comment;
                string[] roles = Roles.GetRolesForUser(user.UserName);
                if (roles != null)
                {
                    foreach (string role in roles)
                    {
                        ListItem listItem = RolesList.Items.FindByValue(role);
						if (listItem != null)
							listItem.Selected = true;
                    }
                }

                // Bind Login on behalf
                // Find out site url
                SiteDto.SiteDataTable sites = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId).Site;
                SiteDto.SiteRow site = null;
                if (sites != null && sites.Rows.Count > 0)
                {
                    foreach (SiteDto.SiteRow siteRow in sites.Rows)
                    {
                        if (site == null && siteRow.IsActive)
                            site = siteRow;

                        if (siteRow.IsDefault && siteRow.IsActive)
                            site = siteRow;
                    }
                }

                if (site != null)
                {
                    LoginOnBehalf.Visible = true;
                    LoginOnBehalf.NavigateUrl = String.Format("{0}/login.aspx?customer={1}", GlobalVariable.GetVariable("url", site.SiteId), user.UserName);
                }
                else
                    LoginOnBehalf.Visible = false;

            }
            else
            {
                UserNameTextBox.Enabled = true;
                IsLockedOut.Enabled = false;
            }
        }

        /// <summary>
        /// Loads the organization items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadOrganizationItems(int iStartIndex, int iNumItems, string sFilter)
        {
            int total = 0;

            ProfileSearchParameters pars = new ProfileSearchParameters();
            ProfileSearchOptions options = new ProfileSearchOptions();
            pars.FreeTextSearchPhrase = sFilter;
            options.Namespace = "Mediachase.Commerce.Profile";
            options.Classes.Add("Organization");

            Organization[] orgs = ProfileContext.Current.FindOrganizations(pars, options, out total);
            total = orgs.Length;

            OrganizationFilter.Items.Clear();

            foreach (Organization org in orgs)
            {
                ComboBoxItem item = new ComboBoxItem(org.Name);
                item.Value = org.Id.ToString();
                OrganizationFilter.Items.Add(item);
            }

            OrganizationFilter.ItemCount = total;
        }

		/// <summary>
		/// Entries the code check.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void CustomerNameCheck(object sender, ServerValidateEventArgs args)
		{
			args.IsValid = true;
		}

		/// <summary>
		/// Entries the code check.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void UserNameCheck(object sender, ServerValidateEventArgs args)
		{
			MembershipUser user = Membership.GetUser(UserNameTextBox.Text);
			if (user == null || (user != null && !UserNameTextBox.Enabled))
				args.IsValid = true;
			else
				args.IsValid = false;
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            Account account = null;

            if (PrincipalId != Guid.Empty)
                account = ProfileContext.Current.GetAccount(PrincipalId);
            else if (UserId != Guid.Empty)
                account = ProfileContext.Current.GetAccount(UserId.ToString());

            if (account == null)
            {
                account = new Account();
            }

            //MembershipUser user = ProfileContext.Current.User;
            //if (user != null)
            //    account.ProviderKey = user.ProviderUserKey;
            account.Name = Name.Text;
            account.Description = Description.Text;
            account.CustomerGroup = CustomerGroup.Text;

            int selectedValue = 0;

            if (Int32.TryParse(AccountState.SelectedValue, out selectedValue) && selectedValue != 0)
                account.State = selectedValue;

            /*
            MembershipUser user = Membership.GetUser(MembershipFilter.SelectedValue);
            if (user != null)
                account.ProviderKey = user.ProviderUserKey.ToString();
             * */

            selectedValue = 0;
            if (Int32.TryParse(OrganizationFilter.SelectedValue, out selectedValue) && selectedValue != 0)
                account.OrganizationId = selectedValue;

            // Save membership user
            MembershipUser user = null;

            if (!String.IsNullOrEmpty(account.ProviderKey))
                user = Membership.GetUser(new Guid(account.ProviderKey));
            else if (UserId != Guid.Empty)
                user = Membership.GetUser(UserId);
            else if (!String.IsNullOrEmpty(UserNameTextBox.Text))
                user = Membership.GetUser(UserNameTextBox.Text);


            if (user != null)
            {
                user.Comment = CommentTextBox.Text;
                user.Email = EmailText.Text;
                user.IsApproved = IsApproved.IsSelected;
                account.ProviderKey = user.ProviderUserKey.ToString();
                Membership.UpdateUser(user);
            }
            else // create new account
            {
                user = Membership.CreateUser(UserNameTextBox.Text, PasswordTextBox.Text, EmailText.Text);
                account.ProviderKey = user.ProviderUserKey.ToString();
                user.Comment = CommentTextBox.Text;
                user.Email = EmailText.Text;
                user.IsApproved = IsApproved.IsSelected;
                Membership.UpdateUser(user);
            }

            // Update user roles
            foreach (ListItem item in RolesList.Items)
            {
                if (item.Selected)
                {
                    if (!Roles.IsUserInRole(user.UserName, item.Value))
                        Roles.AddUserToRole(user.UserName, item.Value);
                }
                else
                {
                    if (Roles.IsUserInRole(user.UserName, item.Value))
                        Roles.RemoveUserFromRole(user.UserName, item.Value);
                }
            }

            ProfileContext.MetaDataContext.UseCurrentUICulture = false;
            account.AcceptChanges();
        }

        #endregion
    }
}
