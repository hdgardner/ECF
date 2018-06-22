using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace OakTree.Web.UI {
	public class ControlHelper {
		
		/// <summary>
		/// Enables you to find a client control by id from the same page as the control passed
		/// </summary>
		/// <param name="controlID">The ID of the control to find.</param>
		/// <param name="control">The control from which you are looking.</param>
		/// <param name="searchNamingContainers">Whether to climb up the control tree and search.</param>
		/// <returns></returns>
		public static Control FindTargetControl(string ControlID, Control Control, bool SearchNamingContainers) {
			if (SearchNamingContainers) {
				Control namingContainer;
				Control control2 = null;
				if (Control is INamingContainer) {
					namingContainer = Control;
				} else {
					namingContainer = Control.NamingContainer;
				}
				do {
					control2 = namingContainer.FindControl(ControlID);
					namingContainer = namingContainer.NamingContainer;
					if (control2 != null) {
						return control2;
					}
				}
				while (namingContainer != null);
				return control2;
			}
			return Control.FindControl(ControlID);
		}

		/// <summary>
		/// Registers informationa bout a server-side control into the OakTree.Web.UI.WebControls JavaScript namespace
		/// </summary>
		/// <param name="ClientScriptManager"></param>
		/// <param name="Control">The control to register</param>
		public static void RegisterControlInClientScript(ClientScriptManager ClientScriptManager, Control Control) {
			ControlHelper.RegisterControlInClientScript(ClientScriptManager, Control, string.Empty);
		}
		
		/// <summary>
		/// Registers informationa bout a server-side control into the OakTree.Web.UI.WebControls JavaScript namespace
		/// </summary>
		/// <param name="ClientScriptManager"></param>
		/// <param name="Control">The control to register</param>
		/// <param name="Data">Any additional data you'd like to provide to the client script. This should be in JSON format</param>
		public static void RegisterControlInClientScript(ClientScriptManager ClientScriptManager, Control Control, string Data) {
			RegisterControlInClientScript(ClientScriptManager, Control, Control.GetType().Name, Data);
		}

		/// <summary>
		/// Registers informationa bout a server-side control into the OakTree.Web.UI.WebControls JavaScript namespace
		/// </summary>
		/// <param name="ClientScriptManager"></param>
		/// <param name="Control">The control to register</param>
		/// <param name="ControlTypeName">The typename of the control</param>
		/// <param name="Data">Any additional data you'd like to provide to the client script. This should be in JSON format</param>
		public static void RegisterControlInClientScript(ClientScriptManager ClientScriptManager, Control Control, string ControlTypeName, string Data) {
			ClientScriptManager.RegisterClientScriptResource(typeof(ControlResources), ControlResources.OAKTREE_WEB_UI_WEBCONTROLS_JS);
			int indexOfBracket = Data.IndexOf('{');
			if (indexOfBracket != -1) {
				Data = Data.Substring(indexOfBracket + 1);
				Data = "{controlID:'" + Control.ClientID + "'," + Data;
			} else {
				Data = "{controlID:'" + Control.ClientID + "'}";
			}

			string script = string.Format("OakTree.Web.UI.WebControls.registerControl('{0}',{1});", ControlTypeName, Data);
			ClientScriptManager.RegisterClientScriptBlock(typeof(ControlResources), "REGISTERED_CONTROL_" + Control.ID, script, true);

		}

		/// <summary>
		/// Returns a site relative HTTP path from a partial path starting out with a ~.
		/// Same syntax that ASP.Net internally supports but this method can be used
		/// outside of the Page framework.
		/// 
		/// Works like Control.ResolveUrl including support for ~ syntax
		/// but returns an absolute URL.
		/// </summary>
		/// <param name="originalUrl">Any Url including those starting with ~</param>
		/// <returns>relative url</returns>
		public static string ResolveUrl(string originalUrl) {

			if (originalUrl == null)

				return null;



			// *** Absolute path - just return

			if (originalUrl.IndexOf("://") != -1)

				return originalUrl;



			// *** Fix up image path for ~ root app dir directory
			if (originalUrl.StartsWith("~"))
				return VirtualPathUtility.ToAbsolute(originalUrl);



			return originalUrl;

		}

 
		/// <summary>
		/// This method returns a fully qualified absolute server Url which includes
		/// the protocol, server, port in addition to the server relative Url.
		/// 
		/// Works like Control.ResolveUrl including support for ~ syntax
		/// but returns an absolute URL.
		/// </summary>
		/// <param name="ServerUrl">Any Url, either App relative or fully qualified</param>
		/// <param name="forceHttps">if true forces the url to use https</param>
		/// <returns></returns>
		public static string ResolveServerUrl(string serverUrl, bool forceHttps) {
			// *** Is it already an absolute Url?
			if (serverUrl.IndexOf("://") > -1)
				return serverUrl;

			// *** Start by fixing up the Url an Application relative Url

			string newUrl = ResolveUrl(serverUrl);
			Uri originalUri = HttpContext.Current.Request.Url;
			newUrl = (forceHttps ? "https" : originalUri.Scheme) +

					 "://" + originalUri.Authority + newUrl;
			return newUrl;
		}

		/// <summary>
		/// This method returns a fully qualified absolute server Url which includes
		/// the protocol, server, port in addition to the server relative Url.
		/// 
		/// It work like Page.ResolveUrl, but adds these to the beginning.
		/// This method is useful for generating Urls for AJAX methods
		/// </summary>
		/// <param name="ServerUrl">Any Url, either App relative or fully qualified</param>
		/// <returns></returns>

		public static string ResolveServerUrl(string serverUrl) {
			return ResolveServerUrl(serverUrl, false);
		}
	
	}
}
