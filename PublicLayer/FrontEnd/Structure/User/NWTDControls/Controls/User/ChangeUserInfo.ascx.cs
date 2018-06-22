using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Cms.WebUtility.UI;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	
	/// <summary>
	/// This control allows a user to change person information.
	/// </summary>
	public partial class ChangeUserInfo : System.Web.UI.UserControl {
		/// <summary>
		/// When the page loads, user information is bount to the various form fields.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
            this.litUserName.Text = Mediachase.Commerce.Profile.ProfileContext.Current.UserName;
            this.litUserState.Text = string.IsNullOrEmpty( NWTD.Profile.BusinessPartnerState )? "N/A" : NWTD.Profile.BusinessPartnerState ;
            
                this.litUserDistrict.Text = (NWTD.Profile.BusinessPartner != null) ? NWTD.Profile.BusinessPartner.Name : "N/A";
            

			if (!IsPostBack) {

				if(ProfileContext.Current.User != null)
					this.tbEmail.Text = ProfileContext.Current.User.Email;

				this.tbFirstName.Text = Mediachase.Commerce.Profile.ProfileContext.Current.Profile.FirstName;
				this.tbLastName.Text = Mediachase.Commerce.Profile.ProfileContext.Current.Profile.LastName;
				this.tbPhone.Text = Mediachase.Commerce.Profile.ProfileContext.Current.Profile["Phone"].ToString();
				DataBind();
			}
		}

		/// <summary>
		/// When the save button is clicked, the information is stored, and the user receives a mesage.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSaveChanges_Click(object sender, EventArgs e) {
			Mediachase.Commerce.Profile.ProfileContext.Current.User.Email = this.tbEmail.Text;
			
			try {
				System.Web.Security.Membership.UpdateUser(Mediachase.Commerce.Profile.ProfileContext.Current.User);
			}
			catch (Exception ex) {
				ErrorManager.GenerateError(ex.Message);
			}
			Mediachase.Commerce.Profile.ProfileContext.Current.Profile.FirstName = this.tbFirstName.Text;
			Mediachase.Commerce.Profile.ProfileContext.Current.Profile.LastName = this.tbLastName.Text;
			Mediachase.Commerce.Profile.ProfileContext.Current.Profile["Phone"] = this.tbPhone.Text;
			Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Save();
			this.lblMessage.Visible = true;
			this.lblMessage.Text = "Your personal information has been saved.";

		}
	}
}