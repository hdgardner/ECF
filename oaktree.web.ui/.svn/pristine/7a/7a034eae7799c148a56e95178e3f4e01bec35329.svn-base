using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using System.Text;

namespace OakTree.Web.UI.WebControls {
	/// <summary>
	/// Summary description for SWFObject
	/// </summary>
	/// 
	[ToolboxData("<{0}:SWFObject runat=\"server\"><AlternateContent>Enter Your Alternate Content</AlternateContent></{0}:SWFObject>"), DefaultProperty("Src")]
	public class SWFObject : System.Web.UI.WebControls.CompositeControl {
		
		private List<SWFParam> _parameters;
		private List<SWFFlashVar> _vars;
		private string _flashPlayerVersion = "7.0.0";
		private string _width = "200";
		private string _height = "200";

		protected const string ClientScriptKey = "SWFObject_javascript";
		//render this in a DIV
		protected override System.Web.UI.HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected string ClientScript {
			get {
				string expressUrl;
				if (string.IsNullOrEmpty(this.ExpressInstallSwfUrl)) expressUrl = "false";
				else expressUrl = "'" + this.Page.ResolveUrl(this.ExpressInstallSwfUrl) + "'";

				StringBuilder builder = new StringBuilder(string.Format("<script type=\"text/javascript\"> swfobject.embedSWF('{0}', '{1}', '{2}', '{3}', '{4}', {5}, ",
					this.Page.ResolveUrl(this.Src),
					this.ClientID,
					this.SWFWidth,
					this.SWFHeight,
					this.FlashPlayerVersion,
					expressUrl
				));

				builder.Append("{");

				foreach (SWFFlashVar var in this.FlashVars) {
					builder.Append(string.Format("{0}:'{1}'", var.Name, var.Value));
					if (this.FlashVars.IndexOf(var) != FlashVars.Count - 1) builder.Append(",");
				}

				builder.Append("},{");
				foreach (SWFParam param in this.Parameters) {
					builder.Append(string.Format("{0}:'{1}'", param.Name.ToString(), param.Value));
					if (this.Parameters.IndexOf(param) != Parameters.Count - 1) builder.Append(",");
				}

				builder.Append("},");
				builder.Append("{");
				builder.Append(string.Format("align:'{0}',", this.Align.ToString()));
				builder.Append(string.Format("name:'{0}'", this.Name));
				builder.Append("}");
				builder.Append(");</script>");
				return builder.ToString();
			}
		}

		public enum AlignOption { Middle, Left, Right }

		#region public properties

		[Category("Appearance")]
		[DefaultValue("200")]
		public string SWFHeight { get { return this._height; } set { this._height = value; } }

		[Category("Appearance")]
		[DefaultValue("200")]
		public string SWFWidth { get { return this._width; } set { this._width = value; } }

		[Category("Appearance")]
		public AlignOption Align { get; set; }
		
		public string Name { get; set; }

		[Category("Behaviour")]
		public string Src { get; set; }

		[Category("Behaviour")]
		[DefaultValue("7.0.0")]
		public string FlashPlayerVersion { get { return this._flashPlayerVersion; } set { this._flashPlayerVersion = value; } }

		[Category("Behaviour")]
		public string ExpressInstallSwfUrl { get; set; }

		[Category("Behaviour")]
		[Description("The alternate content to supply to browsers without flash support")]
		[DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateInstance(TemplateInstance.Single)]
		public virtual ITemplate AlternateContent { get; set; }

		[Category("Behaviour")]
		[Description("Flashvars to be passed to the SWF")]
		[PersistenceMode(PersistenceMode.InnerProperty), TemplateInstance(TemplateInstance.Single), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual List<SWFFlashVar> FlashVars {
			get {
				if (this._vars == null) this._vars = new List<SWFFlashVar>();
				return this._vars;
			}
		}

		[Category("Behaviour")]
		[Description("The parameters for the swf")]
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual List<SWFParam> Parameters {
			get {
				if (this._parameters == null) this._parameters = new List<SWFParam>();
				return this._parameters;
			}
		}

		#endregion

		#region method overrides
		
		protected override void CreateChildControls() {
			if (this.AlternateContent != null) {
				this.AlternateContent.InstantiateIn(this);
			} 
			base.CreateChildControls();
		}
		protected override void OnLoad(EventArgs e) {
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.SWFOBJECT_JS);
			this.Page.ClientScript.RegisterStartupScript(typeof(ControlResources), SWFObject.ClientScriptKey + this.ClientID, this.ClientScript, false);
			base.OnLoad(e);
		}

		#endregion
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SWFParam {

		public enum SWFParamName { play, loop, menu, quality, scale, salign, wmode,bgcolor, @base, swliveconnect, flashvars,devicesfont,allowscriptaccess,seamlesstabbing,allowfullscreen,allownetworking  }

		[DefaultValue(""), NotifyParentProperty(true)]
		public SWFParamName Name { get; set; }
		[DefaultValue(""), NotifyParentProperty(true)]
		public string Value { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SWFFlashVar {
		[DefaultValue(""), NotifyParentProperty(true)]
		public string Name { get; set; }
		[DefaultValue(""), NotifyParentProperty(true)]
		public string Value { get; set; }
	}


}
