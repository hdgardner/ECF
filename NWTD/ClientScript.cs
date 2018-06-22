using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Shared;

namespace NWTD.Web.UI {

	/// <summary>
	/// A Utility class for adding all the scripts reuqired for NWTD to run.
	/// </summary>
	public class ClientScript {

		//TODO: these should be resources living in THIS project
		/// <summary>
		/// Adds all the required client script to a page at runtime
		/// </summary>
		/// <param name="Page"></param>
		public static void AddRequiredScripts(System.Web.UI.Page Page){

			string script = "var NWTD = NWTD||{};";
			script += string.Format("NWTD.BaseURL='{0}';", System.Web.HttpUtility.UrlEncode( Page.ResolveUrl("~/")));
			script += string.Format("NWTD.CurrentUserID='{0}';", Mediachase.Commerce.Profile.ProfileContext.Current.UserId.ToString());

			Page.ClientScript.RegisterClientScriptResource(typeof(OakTree.Web.UI.ControlResources), OakTree.Web.UI.ControlResources.JQUERY_JS);
			Page.ClientScript.RegisterClientScriptResource(typeof(OakTree.Web.UI.ControlResources), OakTree.Web.UI.ControlResources.OAKTREE_UTILITIES_JS);
			Page.ClientScript.RegisterClientScriptResource(typeof(OakTree.Web.UI.ControlResources), OakTree.Web.UI.ControlResources.OAKTREE_WEB_UI_COOKIE);
			Page.ClientScript.RegisterClientScriptResource(typeof(OakTree.Web.UI.ControlResources), OakTree.Web.UI.ControlResources.OAKTREE_WEB_UI_WEBCONTROLS_JS);

			Page.ClientScript.RegisterClientScriptInclude(typeof(OakTree.Web.UI.ControlResources), "NWTD_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/NWTD.js"));
			Page.ClientScript.RegisterClientScriptBlock(typeof(OakTree.Web.UI.ControlResources), "NWTD_global_js", script, true);
		}	
	}
}
