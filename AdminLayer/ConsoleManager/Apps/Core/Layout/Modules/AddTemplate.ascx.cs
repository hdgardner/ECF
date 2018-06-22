using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Layout.Modules
{
	public partial class AddTemplate : System.Web.UI.UserControl
	{
		#region prop: OnClickCommandScript
		/// <summary>
		/// Gets or sets the on click command script.
		/// </summary>
		/// <value>The on click command script.</value>
		public string OnClickCommandScript
		{
			get
			{
				if (ViewState["_OnClickCommandScript"] != null)
					return ViewState["_OnClickCommandScript"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["_OnClickCommandScript"] = value;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters();
			cp.CommandName = "cmdCoreLayoutAddControl";
			
			System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();			
			dic.Add("ColumnId", "%columnId%");
			dic.Add("PageUid", "%pageUid%");
			
			cp.CommandArguments = dic;
			this.OnClickCommandScript = CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, "Core", "LayoutManage", cp);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddAttribute("class", "x-panel x-portlet"); //ibn-portlet-addButton");
			
			/*writer.AddAttribute("onmouseover", "this.className = this.className.replace(' ibn-portlet-addButton', '');");
			writer.AddAttribute("onmouseout", "this.className += ' ibn-portlet-addButton';");*/
			writer.AddAttribute("onclick_toExecute", this.OnClickCommandScript);
			
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			//writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
			//writer.AddStyleAttribute(HtmlTextWriterStyle.Filter, "alpha(opacity=45)");
			//writer.AddStyleAttribute("opacity", ".45");
			
			writer.RenderBeginTag("div");
			
			base.Render(writer);

			writer.RenderEndTag();
		}
	}
}