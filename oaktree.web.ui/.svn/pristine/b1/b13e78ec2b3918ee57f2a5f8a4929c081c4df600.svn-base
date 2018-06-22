using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;

namespace OakTree.Web.UI.WebControls {
	[DefaultProperty("ScriptText"),ParseChildren(ChildrenAsProperties=true,DefaultProperty="ScriptText")]
	public class JavaScriptInclude: WebControl {

		public enum ScriptLibraryOption { None, jQuery, jQueryTools }

		[Category("Behaviour"), Description("The script library to Include")]
		public ScriptLibraryOption ScriptLibrary { get; set; }
		
		[UrlProperty, Category("Behaviour"), 
			DefaultValue((string)null), 
			Description("The Url of the script to include. This is only applicable if ScriptLibrary is set to None")]
		public string ScriptFile { get; set; }

		[Category("Behaviour")]
		[Description("The script text. This is only appplicable if ScriptLibrary is None and there is no ScriptFile ")]
		[DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerDefaultProperty), Browsable(false)]
		public String ScriptText { get; set; }


		protected override void OnPreRender(EventArgs e) {
			switch (this.ScriptLibrary) { 
				case ScriptLibraryOption.None:
					if (!string.IsNullOrEmpty(this.ScriptFile)) {
						this.Page.ClientScript.RegisterClientScriptInclude(typeof(ControlResources), "CLIENT_SCRIPT_INCLUDE_" + this.ScriptFile,ControlHelper.ResolveServerUrl( this.ScriptFile));

					} else if(!string.IsNullOrEmpty(this.ScriptText )){
						this.Page.ClientScript.RegisterClientScriptBlock(typeof(ControlResources), "CLIENT_SCRIPT_BLOCK_" + this.ID,  this.ScriptText.Trim(),true);
					}
					break;
				case ScriptLibraryOption.jQuery:
					this.Page.ClientScript.RegisterClientScriptResource( typeof(ControlResources),ControlResources.JQUERY_JS);
					break;
				case ScriptLibraryOption.jQueryTools: {
						this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);
						this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_TOOLS_JS);	
					break;
					
				}
			}
			base.OnPreRender(e);
		}

		protected override void CreateChildControls() {
			//base.CreateChildControls();
		}

		protected override void Render(HtmlTextWriter writer) {
			//base.Render(writer);
		}
	}
}
