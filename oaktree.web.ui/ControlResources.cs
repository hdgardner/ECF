using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Reflection;

[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.XML.States.xml", "text/xml")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.SWF.FlowPlayer.flowplayer-3.1.3.swf", "application/x-shockwave-flash")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.SWF.flowplayer.controls-3.1.3.swf", "application/x-shockwave-flash")]

//javascript
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.flowplayer-3.1.4.min.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.jquery.tools.min.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.FlowPlayer_Implementation.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.swfobject.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.jquery-1.3.2.min.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.jquery-1.4.1.min.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.jquery-ui-1.7.2.custom.min.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.OakTree_Web_UI_WebControls.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.TabbedControl.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.OakTree_Web_UI_Cookie.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.OakTree_Utilities.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.Facebook.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.SelectTree.js", "application/x-javascript")]
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.ClientScript.overlay.js", "application/x-javascript")]
//[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.Styles.OerlayDefault.css", "text/css")]
// On 08/02/17, Heath replaced the mispelled 'OerlayDefault.css' above with the following correctly spelled 'OverlayDefault.css'
[assembly: WebResourceAttribute("OakTree.Web.UI.Resources.Styles.OverlayDefault.css", "text/css")]

namespace OakTree.Web.UI {

	/// <summary>
	/// A control that helps store and provide embedded resrouces. 
	public class ControlResources {
		public const string FLOWPLAYER_JS = "OakTree.Web.UI.Resources.ClientScript.flowplayer-3.1.4.min.js";

		public const string JQUERY_TOOLS_JS = "OakTree.Web.UI.Resources.ClientScript.jquery.tools.min.js";
		public const string STATES_XML = "OakTree.Web.UI.Resources.XML.States.xml";
		public const string FLOWPLAYER_SWF = "OakTree.Web.UI.Resources.SWF.FlowPlayer.flowplayer-3.1.3.swf";
		public const string FLOWPLAYER_CONTROLS_SWF = "OakTree.Web.UI.Resources.SWF.flowplayer.controls-3.1.3.swf";
		public const string SWFOBJECT_JS = "OakTree.Web.UI.Resources.ClientScript.swfobject.js";
		public const string JQUERY_JS_1_3_2 = "OakTree.Web.UI.Resources.ClientScript.jquery-1.3.2.min.js";
		public const string JQUERY_JS = "OakTree.Web.UI.Resources.ClientScript.jquery-1.4.1.min.js";
		public const string JQUERY_UI_JS = "OakTree.Web.UI.Resources.ClientScript.jquery-ui-1.7.2.custom.min.js";
		public const string OAKTREE_WEB_UI_WEBCONTROLS_JS = "OakTree.Web.UI.Resources.ClientScript.OakTree_Web_UI_WebControls.js";
		public const string OAKTREE_UTILITIES_JS = "OakTree.Web.UI.Resources.ClientScript.OakTree_Utilities.js";
		public const string OAKTREE_WEB_UI_COOKIE = "OakTree.Web.UI.Resources.ClientScript.OakTree_Web_UI_Cookie.js";
		public const string FLOWPLAYER_IMPLEMENTATION = "OakTree.Web.UI.Resources.ClientScript.FlowPlayer_Implementation.js";
		public const string TABBED_CONTROL_JS = "OakTree.Web.UI.Resources.ClientScript.TabbedControl.js";
		public const string FACEBOOK_JS = "OakTree.Web.UI.Resources.ClientScript.Facebook.js";
		public const string SELECT_TREE_JS = "OakTree.Web.UI.Resources.ClientScript.SelectTree.js";
		public const string OVERLAY_JS = "OakTree.Web.UI.Resources.ClientScript.overlay.js";


		/// <summary>
		/// Gets the resource by name
		/// </summary>
		/// <param name="ResourceName"></param>
		/// <returns></returns>
		public static Stream GetResourceStream(string ResourceName) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream(ResourceName);
			if (stream == null) {
				throw new FileNotFoundException("Cannot find mappings file.", ResourceName);
			}
			return stream;
		}

	}
}
