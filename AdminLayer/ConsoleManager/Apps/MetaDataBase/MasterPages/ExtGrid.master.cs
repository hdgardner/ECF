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

using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.MasterPages
{
	public partial class ExtGrid : System.Web.UI.MasterPage
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			//ScriptManager1.EnablePageMethods = true;
			ScriptManager1.Services.Add(new ServiceReference("~/WebServices/ListHandler.asmx"));

			if (!IsPostBack)
			{
				//bindMenu();
			}

            
		}


		#region Page_PreRender
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			RegisterScriptTags();
		}

        /// <summary>
        /// Registers the script tags.
        /// </summary>
		private void RegisterScriptTags()
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
                    CHelper.GetAbsolutePath("/Apps/MetaDataBase/styles/mcBlockMenu.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
                    CHelper.GetAbsolutePath("/Apps/MetaDataBase/styles/ibn.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
                    CHelper.GetAbsolutePath("/Apps/MetaDataBase/styles/Theme.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
                    CHelper.GetAbsolutePath("/Apps/MetaDataBase/styles/windows.css")));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("<link rel='stylesheet' type='text/css' href='{0}' />",
                    CHelper.GetAbsolutePath("/Apps/MetaDataBase/styles/menuStyle.css")));

			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/yui-utilities.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/ext-yui-adapter.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/ext8.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/ext-lang-ru.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/List2List.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/jquery-1.2.1.min.js"));
			Page.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString("N"),
                CHelper.GetAbsolutePath("/Apps/MetaDataBase/Scripts/common.js"));
		}
		#endregion
	}
}
