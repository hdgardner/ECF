using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	public partial class UserInfo : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {
			this.litFirstName.Text = ProfileContext.Current.Profile["FirstName"].ToString();
			this.litDistrict.Text = ProfileContext.Current.Profile["District"].ToString();
			this.litPhone.Text =ProfileContext.Current.Profile["Phone"].ToString();
			this.litLastName.Text = ProfileContext.Current.Profile["LastName"].ToString();
			this.litSchool.Text = ProfileContext.Current.Profile["School"].ToString();
			this.litState.Text = ProfileContext.Current.Profile["State"].ToString();
			this.litUserName.Text = ProfileContext.Current.Profile.UserName;
		}

	}
}