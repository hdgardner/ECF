using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OakTree.Web.UI.WebControls{
	[DefaultProperty("FLVUrl")]
	[ToolboxData("<{0}:FlowPlayer runat=server></{0}:FlowPlayer>")]
	public class FlowPlayer : CompositeControl {

		private string _playerUrl;
		private string _controlsUrl;
		private List<Trigger> _triggers;

		[Bindable(true)]
		[UrlProperty]
		[Category("Behaviour")]
		[DefaultValue("")]
		public string PlayerUrl {
			get {
				//we have an embedded flowplayer. Unfortunately it's buggy when it's embedded...
				if (string.IsNullOrEmpty(this._playerUrl)) this._playerUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(ControlResources), ControlResources.FLOWPLAYER_SWF);
				return this._playerUrl; 
			}
			set { this._playerUrl = value; }
		
		}

		//[Bindable(true)]
		//[UrlProperty]
		//[Category("Behaviour")]
		//[DefaultValue("")]
		//public string ControlsURL {
		//    get {
		//        //we have an embedded flowplayer. Unfortunately it's buggy when it's embedded...
		//        if (string.IsNullOrEmpty(this._controlsUrl)) this._controlsUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(ControlResources), ControlResources.FLOWPLAYER_CONTROLS_SWF);
		//        return this._controlsUrl;
		//    }
		//    set { this._controlsUrl = value; }

		//}

		[Bindable(true)]
		[UrlProperty]
		[Category("Behaviour")]
		[DefaultValue("")]
		public string FLVUrl { get; set; }

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("")]
		[Description("A JavaScript Object Literal that represents the player's options. See the flowplayer API for more info: http://flowplayer.org")]
		public string PlayerOptions { get; set; }


		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("")]
		[Description("A JavaScript Object Literal that represents the modal dialog's options. See the overlay API for more info: http://flowplayer.org/tools/overlay.html")]
		public string OverlayOptions { get; set; }

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("")]
		public bool UseModal { get; set; }

		[Category("Behaviour")]
		[Description("Triggers that display the modal dialog. If the value can either be a control ID, or any valid CSS selector. (eg. \"a.trigger, img#myImage\"")]
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual List<Trigger> Triggers {
			get {
				if (this._triggers == null) this._triggers = new List<Trigger>();
				return this._triggers;
			}
		}

		protected override System.Web.UI.HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}

		protected override void OnLoad(EventArgs e) {
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_TOOLS_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.FLOWPLAYER_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.FLOWPLAYER_IMPLEMENTATION);		
			
			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e) {
			List<string> jsProperties = new List<string>();
			
			//triggers
			if (this.Triggers.Count > 0) {
				List<string> triggerSelectors = new List<string>();
				foreach (Trigger trigger in this.Triggers) {
					if (trigger.TriggerType == Trigger.TriggerTypeOption.ControlID) {
						Control triggerControl = ControlHelper.FindTargetControl(trigger.Value, this, true);
						if (triggerControl != null) triggerSelectors.Add(string.Format("#{0}", triggerControl.ClientID));
					}
					if (trigger.TriggerType == Trigger.TriggerTypeOption.CSSSelector) {
						triggerSelectors.Add(trigger.Value);
					}
				}
				jsProperties.Add(string.Format("triggers:'{0}'", string.Join(",", triggerSelectors.ToArray())));
			}

			//player options
			if (!string.IsNullOrEmpty(this.PlayerOptions)) { 
				jsProperties.Add( string.Format("playerOptions:{0}",PlayerOptions));
			}
			
			//overlay options
			if (!string.IsNullOrEmpty(this.OverlayOptions)) {
				jsProperties.Add( string.Format("overlayOptions:{0}", OverlayOptions));
			}

			//whether to use overlay
			if (this.UseModal) {
				this.Style["display"] = "none";
				jsProperties.Add("usemodal:true");
			}


			jsProperties.Add( 
				string.Format("overlayID:'{0}', flv:'{1}', swf:'{2}'", 
					this.ClientID, 
					ControlHelper.ResolveServerUrl(this.FLVUrl), 
					ControlHelper.ResolveServerUrl(this.PlayerUrl)
	
				)
			);

			string script = "{" + string.Join(",", jsProperties.ToArray())  + "}";


			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);

			ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, script);

			base.OnPreRender(e);
		}

	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Trigger {
		public enum TriggerTypeOption { ControlID , CSSSelector}

		[Description("Either a Control ID or CSS Selector, depending on the TriggerType. CSS selectors can be any valid CSS selctor, including comma-separated selectors.")]
		[DefaultValue(""), NotifyParentProperty(true)]
		public string Value { get; set; }

		[Description("Whether to use a Control ID for a trigger, or a CSS selector")]
		[DefaultValue(""), NotifyParentProperty(true)]
		public TriggerTypeOption TriggerType { get; set; }

	}

}
