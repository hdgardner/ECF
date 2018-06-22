using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.Website.Templates.NWTD {
	public partial class SearchLink : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {
			this.Visible = ! global::NWTD.Profile.CurrentUserLevel.Equals(global::NWTD.UserLevel.ANONYMOUS);
		}
	}
}