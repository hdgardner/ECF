using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.MasterPages
{
	public partial class DialogMasterPage : System.Web.UI.MasterPage
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "main_js", "<script language=\"javascript\" type=\"text/javascript\" src=\"" + CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/main.js") + "\"></script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "common_js", "<script language=\"javascript\" type=\"text/javascript\" src=\"" + CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/common.js") + "\"></script>");
		}

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.Title.ToLower() == "untitled page")
				Page.Title = CHelper.GetFullPageTitle("");
			else
				Page.Title = CHelper.GetFullPageTitle(Page.Title);
		}
	}
}