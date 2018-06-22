using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Mediachase.Cms.Website.Templates.NWTD.NodeTemplates {
	public partial class EntriesFiltered : Mediachase.Cms.WebUtility.BaseControls.BaseNodeTemplate {
		protected void Page_Load(object sender, EventArgs e) {

		}
		public override void LoadContext(IDictionary context) {
			base.LoadContext(context);
			SearchControl1.LoadContext(context);
		}
	}

}