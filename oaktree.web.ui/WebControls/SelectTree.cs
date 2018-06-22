using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;


namespace OakTree.Web.UI.WebControls {
	public class SelectTree:CompositeControl, ITextControl {

		private HiddenField _resultsValue;
		private HyperLink _addButton;
		private ListBox _parentSelect;
		private ListBox _childSelect;

		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBox ParentSelect {
			get { 
				if (this._parentSelect == null) this._parentSelect = new ListBox();
				return this._parentSelect;
			} 
		}
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBox ChildSelect {
			get {
				if (this._childSelect == null) this._childSelect = new ListBox();
				return this._childSelect;
			}
		}

		private string _clientNameKey = "key";
		private string _clientValuesKey = "values";

		public string ClientNameKey {
			get {return this._clientNameKey;}
			set {this._clientNameKey = value;}
		}

		public string ClientValuesKey {
			get {return this._clientValuesKey;}
			set { this._clientValuesKey = value; }
		}


		public HiddenField ResultsValue {
			get {
				if (this._resultsValue == null) this._resultsValue = new HiddenField();
				return this._resultsValue;
			}
		}
		public HyperLink AddButton {
			get {
				if (this._addButton == null) {
					this._addButton = new HyperLink();
					this._addButton.NavigateUrl = "#";
					this._addButton.CssClass = "addButton";
				}
				return this._addButton;
			}
		}

		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void  OnPreRender(EventArgs e){
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.SELECT_TREE_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.OAKTREE_UTILITIES_JS);
			ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, string.Format("{{keyName:'{0}',valuesName:'{1}'}}",this.ClientNameKey,this.ClientValuesKey));
			base.OnInit(e);
		}		

		protected override void CreateChildControls() {
			this.Controls.Add(this.ResultsValue);
			HtmlGenericControl resultsList = new HtmlGenericControl("ul");
			resultsList.Attributes.Add("class", "resultsList");

			this.Controls.Add(this.ParentSelect);
			this.Controls.Add(this.ChildSelect);

			this.AddButton.Text = "Add";

			this.Controls.Add(this.AddButton);
			this.Controls.Add(resultsList);
		}


		#region ITextControl Members

		public string Text {
			get {
				this.EnsureChildControls();
				return this.ResultsValue.Value;
			}
			set {
				this.ResultsValue.Value = value;
			}
		}

		#endregion
	}
}
