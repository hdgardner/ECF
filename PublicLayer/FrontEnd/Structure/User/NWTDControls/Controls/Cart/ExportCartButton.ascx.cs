using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// Unfinished control that could be used for exporting a cart
	/// </summary>
	public partial class ExportCartButton : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {

		}

		public string CartName{get;set;}

		protected void Export_Click(object sender, EventArgs e) {
			CartHelper helper = new CartHelper(this.CartName, ProfileContext.Current.UserId);
			//to the exporting here
		}



	}
}