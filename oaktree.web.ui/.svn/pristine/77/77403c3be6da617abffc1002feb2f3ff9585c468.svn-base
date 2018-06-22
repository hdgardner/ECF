using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;

namespace OakTree.Web.UI.WebControls {
	public class Overlay:CompositeControl {

		#region Private Members

		private List<Trigger> _triggers;

		#endregion

		#region Public Properties

		[Bindable(true)]
		[Category("Behaviour")]
		[DefaultValue("")]
		[Description("A JavaScript Object Literal that represents the modal dialog's options. See the overlay API for more info: http://flowplayer.org/tools/overlay.html")]
		public string OverlayOptions { get; set; }

		[Category("Behaviour")]
		[Description("Triggers that display the modal dialog. If the value can either be a control ID, or any valid CSS selector. (eg. \"a.trigger, img#myImage\"")]
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual List<Trigger> Triggers {
			get {
				if (this._triggers == null) this._triggers = new List<Trigger>();
				return this._triggers;
			}
		}

		[Category("Behaviour")]
		[Description("The the content of the modal window")]
		[DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateInstance(TemplateInstance.Single)]
		public virtual ITemplate Content { get; set; }

		#endregion

		#region Overrides

		protected override System.Web.UI.HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}

		protected override void OnLoad(EventArgs e) {
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_TOOLS_JS);


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


			//overlay options
			if (!string.IsNullOrEmpty(this.OverlayOptions)) {
				jsProperties.Add(string.Format("overlayOptions:{0}", OverlayOptions));
			}


			string script = "{" + string.Join(",", jsProperties.ToArray()) + "}";


			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);

			ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, script);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.OVERLAY_JS);


			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e) {


			base.OnPreRender(e);
		}

		protected override void CreateChildControls() {
			if (this.Content != null) {
				this.Content.InstantiateIn(this);
			}
			base.CreateChildControls();
		}

		#endregion
	}
}
