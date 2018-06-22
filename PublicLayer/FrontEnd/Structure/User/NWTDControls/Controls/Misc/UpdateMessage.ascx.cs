using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Misc {
	public partial class UpdateMessage : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {
			this.MessageImage.ImageUrl = this.ImageUrl;
			this.pnlUpdateMessage.CssClass = this.CssClass;
		}
		[UrlProperty]
		public string ImageUrl{get;set;}
		public string Message{get;set;}
		public string CssClass { get; set; }
	}


}