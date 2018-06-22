using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace Mediachase.Web.Console.Controls
{
	/// <summary>
	/// Summary description for CustomImageButton.
	/// </summary>
	public class CustomImageButton : HtmlButton
	{
		#region IsDecline { get; set }
		[DefaultValueAttribute(false)]
		public bool IsDecline
		{
			get
			{
				bool retval = false;
				if (ViewState["isDecline"] != null)
					retval = bool.Parse(ViewState["isDecline"].ToString());
				return retval;
			}
			set
			{
				ViewState["isDecline"] = value;
			}
		}
		#endregion

		#region Text { get; set }
		[DefaultValueAttribute("")]
		[BindableAttribute(true)]
		public string Text
		{
			get
			{
				string retval = "";
				if (ViewState["text"] != null)
					retval = ViewState["text"].ToString();
				return retval;
			}
			set
			{
				ViewState["text"] = value;
			}
		}
		#endregion

		#region CustomImage { get; set }
		[DefaultValueAttribute("")]
		public string CustomImage
		{
			get
			{
				string retval = "";
				if (ViewState["customImage"] != null)
					retval = ViewState["customImage"].ToString();
				return retval;
			}
			set
			{
				ViewState["customImage"] = value;
			}
		}
		#endregion

		#region ImageWidth { get; set }
		[DefaultValueAttribute("16px")]
		public string ImageWidth
		{
			get
			{
				string retval = "16px";
				if (ViewState["imageWidth"] != null)
					retval = ViewState["imageWidth"].ToString();
				return retval;
			}
			set
			{
				ViewState["imageWidth"] = value;
			}
		}
		#endregion

		#region ImageHeight { get; set }
		[DefaultValueAttribute("16px")]
		public string ImageHeight
		{
			get
			{
				string retval = "16px";
				if (ViewState["imageHeight"] != null)
					retval = ViewState["imageHeight"].ToString();
				return retval;
			}
			set
			{
				ViewState["imageHeight"] = value;
			}
		}
		#endregion

		#region CommandArgument { get; set }
		[DefaultValueAttribute("")]
		public string CommandArgument
		{
			get
			{
				string retval = "";
				if (ViewState["commandArgument"] != null)
					retval = ViewState["commandArgument"].ToString();
				return retval;
			}
			set
			{
				ViewState["commandArgument"] = value;
			}
		}
		#endregion

		public CustomImageButton()
		{
		}

		#region override Render
		protected override void Render(HtmlTextWriter writer)
		{
			this.Attributes.Add("type", "button");
			if (String.IsNullOrEmpty(CustomImage))
			{
				string img = "accept.gif";
				if (IsDecline)
					img = "deny.gif";

				string url = "images/" + img;
				string theme = String.IsNullOrEmpty(Page.Theme) ? "Default" : Page.Theme;
				string path = this.Page.MapPath(String.Format("~/App_Themes/{0}/{1}", theme, url));

				if (System.IO.File.Exists(path)) // try current theme
					url = String.Format("~/App_Themes/{0}/{1}", theme, url);
				else // try default theme
					url = String.Format("~/App_Themes/{0}/{1}", "Default", url);

				base.InnerHtml = String.Format("<img src='{0}' border='0' width='16' height='16' align='absMiddle'>&nbsp;{1}", ResolveClientUrl(url), Text);
			}
			else
			{
				base.InnerHtml = String.Format("<img src='{0}' border='0' width='{2}' height='{3}' align='absMiddle'>&nbsp;{1}", CustomImage, Text, ImageWidth, ImageHeight);
			}

			base.RenderBeginTag(writer);
			base.RenderChildren(writer);
			RenderEndTag(writer);
		}
		#endregion
	}
}