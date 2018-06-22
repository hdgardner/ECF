using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace OakTree.Web.UI.WebControls {
	public class FacebookShareLink:HyperLink {

		public const string FACEBOOK_SHARE_URL = "http://www.facebook.com/share.php";
		public const string FACEBOOK_ICON_URL = "http://static.ak.fbcdn.net/images/share/facebook_share_icon.gif";

		public bool ShowIcon { get; set; }
		protected override void OnPreRender(EventArgs e) {
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.FACEBOOK_JS);
			base.OnPreRender(e);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			if (this.Enabled && !base.IsEnabled) {
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
			}
			
			string navigateUrl = this.NavigateUrl;
			if ((navigateUrl.Length > 0) && base.IsEnabled) {
				string fullUrl = ControlHelper.ResolveServerUrl(navigateUrl);
				writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return OakTree.Web.Facebook.share('{0}');",fullUrl));
				writer.AddAttribute(HtmlTextWriterAttribute.Href, FACEBOOK_SHARE_URL);
			}
			string target = this.Target;
			if (navigateUrl.Length > 0) {
				writer.AddAttribute(HtmlTextWriterAttribute.Target, target);
			}
			base.AddAttributesToRender(writer);
			
		}

		protected override void RenderContents(HtmlTextWriter writer) {
			if (this.ShowIcon) {
				Image facebookIcon = new Image();
				facebookIcon.ImageUrl = FACEBOOK_ICON_URL;
				facebookIcon.AlternateText = FACEBOOK_ICON_URL;
				facebookIcon.RenderControl(writer);
			}
			base.RenderContents(writer);
		}
	}
}
