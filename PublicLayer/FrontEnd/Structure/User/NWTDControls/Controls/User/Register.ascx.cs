using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	/// <summary>
	/// This control provides signup functionality for NWTD sites.
	/// </summary>
	public partial class Register : NWTD.Controls.DepositoryControl {
        
//        private string _descriptionText =
//                @"Welcome to the new user registration page!&nbsp; Our web based services are available to districts, schools, and teachers located in {0}.&nbsp; 
//                <b>Individuals wishing to place an order should call 800-676-6630.</b> <br>
//				<br>
//				Note - If your school is part of a public school district, please select your district from the pull down list on the left.&nbsp;  
//                Private or Charter Schools, please select your school from the pull down list on the right. <br>
//				<br>";

        //  On 08/02/17, Heath replaced the the above text with the following per Scott Seeley & NWTD customer service's request
        private string _descriptionText =
                @"Welcome to the new user registration page!&nbsp; Our web based services are available to districts, schools, and teachers located in {0}.&nbsp; 
                <br>
				<br>
				If your school is part of a public school district, please select your district from the pull down list on the left.&nbsp;  
                Private or Charter Schools, please select your school from the pull down list on the right.&nbsp;
                If you do not see your school listed, please call {1} for additional information.&nbsp; 
                <br>
				<br>
                Parents, students, and non-school customers please call for instructions.&nbsp;
                <br>
                <br>";

		/// <summary>
		/// The decription to be added above the form. By default it will show different text depending on the depository.
		/// </summary>
		public string DescriptionText {
			get {

                //return string.Format(this._descriptionText, this.Depository.Equals(NWTD.Depository.MSSD) ? "Nevada and Utah" : "Oregon, Washington, or Alaska");
                
                //On 08/02/17, Heath replaced the above string format with the below per Scott Seeley & NWTD customer service's request
				if (this.Depository.Equals(NWTD.Depository.MSSD))
                    return string.Format(this._descriptionText, "Nevada and Utah", "801-773-3200");
                else
                    return string.Format(this._descriptionText, "Oregon, Washington, or Alaska", "800-676-6630");
			}
			set {

				this._descriptionText = value;
			}
		}


		protected void Page_Load(object sender, EventArgs e) {
            DistrictSelector districtSelector = this.CreateUserWizardStep1.ContentTemplateContainer.FindControl("dsDistricts") as DistrictSelector;
            districtSelector.Depository = this.Depository;
			this.cuwRegister.ContinueDestinationPageUrl = NavigationManager.GetUrl("Home");

		}

		/// <summary>
		/// When the user is crated, the user is added to the "everyone" role, meta fields are set, and the user's organization is set.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void cuwRegister_CreatedUser(object sender, EventArgs e) {
			MembershipUser user = Membership.GetUser(this.cuwRegister.UserName);

			TextBox tbPhone = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Phone") as TextBox;
			TextBox tbFirstName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("FirstName") as TextBox;
			TextBox tbLastName = CreateUserWizardStep1.ContentTemplateContainer.FindControl("LastName") as TextBox;
			DistrictSelector districtSelector = this.CreateUserWizardStep1.ContentTemplateContainer.FindControl("dsDistricts") as DistrictSelector; 


			// Add user to everyone role
			// Check if such role exist
			if (RoleExists(AppRoles.EveryoneRole))
				Roles.AddUserToRole(user.UserName, AppRoles.EveryoneRole);

			if (RoleExists(AppRoles.RegisteredRole))
				Roles.AddUserToRole(user.UserName, AppRoles.RegisteredRole);

			// Now create an account in the ECF 
			ProfileContext.Current.CreateAccountForUser(user);
			


			// and set our custom profile properties
			CustomerProfile profile = CustomerProfile.Create(user.UserName) as CustomerProfile;


			profile.FirstName = tbFirstName.Text;
			profile.LastName = tbLastName.Text;
			profile.FullName = tbFirstName.Text + " " + tbLastName.Text;
			profile.Account.Name = profile.FullName;
			profile["Phone"] = tbPhone.Text;
			profile.Account.OrganizationId = int.Parse(districtSelector.OrganizationID);
			profile.Account.AcceptChanges();
			profile.Save();


			this.cuwRegister.ActiveStepIndex = 1; //Make sure the first step is hidden
            this.pnlError.Visible = false;


		}

		/// <summary>
		/// Roles the exists.
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		private bool RoleExists(string roleName) {
			foreach (string role in Roles.GetAllRoles()) {
				if (role.Equals(roleName))
					return true;
			}

			return false;
		}

		/// <summary>
		/// This is the event handler for errors during user creation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void cuwRegister_CreateUserError(object sender, CreateUserErrorEventArgs e) {
			this.pnlError.Visible = true;

            MembershipProvider provider = Membership.Provider;

			switch (e.CreateUserError.ToString()){
					
				case "DuplicateUserName":
					this.litError.Text = "A user with that user name already exists. Please use a different user name";
					break;
				case  "DuplicateEmail":
					this.litError.Text = "A user with that email address already exists. Please use a different email address";
					break;
                case "InvalidPassword":
                    this.litError.Text = string.Format(this.cuwRegister.InvalidPasswordErrorMessage, provider.MinRequiredPasswordLength, provider.MinRequiredNonAlphanumericCharacters);
                    break;
				default:
					this.litError.Text = string.Format("There was an error creating a user: {0}", e.CreateUserError.ToString());
					break;
			
			}
			
			if (e.CreateUserError.ToString() == "DuplicateEmail") {
				this.pnlError.Visible = true;
				this.litError.Text = "A user with that email address already exists. Please use a different email address";
			}
		}

		protected void cuwRegister_CreatingUser(object sender, LoginCancelEventArgs e) {

		}

	}
}