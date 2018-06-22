using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.Website.Templates.NWTD.NodeTemplates {
	public partial class NodeInfo : Mediachase.Cms.WebUtility.BaseControls.BaseNodeTemplate {
		protected void Page_Load(object sender, EventArgs e) {

		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			BindFields();
		}

		private void BindFields() {
			DataBind();
		}


	}
}