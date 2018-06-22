using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	
	/// <summary>
	/// This is a control for displaying schools from whicha user can choose.
	/// The schools from which a user can choose will be filtered by the value of the District and State property of this control.
	/// </summary>
	public partial class SchoolSelector : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {

		}

		protected void ddlDistricts_DataBound(object sender, EventArgs e) {
			ddlSchools.DataBind();
			udpSchools.Update();
		}

		/// <summary>
		/// The selecte school
		/// </summary>
		public string School {
			get { return this.ddlSchools.SelectedValue; }
		}
		/// <summary>
		/// The selected discrict
		/// </summary>
		public string District {
			get { return this.ddlDistricts.SelectedItem.Text; }
		}
		/// <summary>
		/// The selected state
		/// </summary>
		public string State {
			get { return this.ddlState.SelectedValue; }
		}
	}
}