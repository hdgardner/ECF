using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Xml;

namespace OakTree.Web.UI.WebControls {

    [ToolboxData("<{0}:StateSelector runat=\"server\" />"), DefaultProperty("LabelText"), ValidationProperty("SelectedState")]
	public class StateSelector : FormFieldComposite {

		private StateDisplayCountry _country;

		private Label _label;
		private DropDownList _stateDropdown;

		protected override Label Label{
			get { 
				if (this._label == null) {
					this._label = new Label();
					this._label.AssociatedControlID = this.StateDropdown.ID;
				}
				return this._label;
			}
		}

		/// <summary>
		/// Binds the states from the embeded xml data source to the state dropdown. Don't call this method before all properties have been set (e.d. don't call this from a setter)
		/// </summary>
		protected void BindStates(){

			this.StateDropdown.Items.Clear();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(ControlResources.GetResourceStream(ControlResources.STATES_XML));
			XmlElement country = null;
			foreach (XmlElement element in xmlDoc.GetElementsByTagName("Country")) {
				if (element.Attributes["code"].Value == this.Country.ToString()) {
					country = element;
				}
			}

			if (this.IncludeEmptyItem) {

				this.StateDropdown.Items.Insert(0, new ListItem(this.EmptyItemText, string.Empty));
			}

			if (country != null) {
				foreach (XmlElement element in country.GetElementsByTagName("State")) {
					string textattr = (this.StateDisplaymode == StateDisplayModeOption.FULL_NAME) ? "name" : "code";
					ListItem item = new ListItem(element.Attributes[textattr].Value, element.Attributes["code"].Value);
					this.StateDropdown.Items.Add(item);
					if (this.DefaultState == item.Value) item.Selected = true;
				}
			}
		}

		public override void DataBind() {
			this.BindStates();
			base.DataBind();
		}

		public DropDownList StateDropdown {
			get {
				if (this._stateDropdown == null){
					this._stateDropdown = new DropDownList();
					this._stateDropdown.ID = new Guid().ToString();
					
				}
				return this._stateDropdown;
			}
		}

		public enum StateDisplayModeOption {FULL_NAME, ABBREVIATION }
		public enum StateDisplayCountry {US,FR,GB,NL,CA}

		[Category("Appearance")]
		[Description("Whether to include an empty item as the first item in the State DropdownList.")]
		public bool IncludeEmptyItem { get; set; }

		public string EmptyItemText { get; set; }

		[Category("Appearance")]
		[Description("Whether a state will be displayed using its full name or abbreviation.")]
		public StateDisplayModeOption StateDisplaymode { get; set; }

		[Category("Behaviour")]
		public StateDisplayCountry Country { 
			get { return this._country; }
			set {
				this._country = value;

			}
		}

		[Category("Behaviour")]
		[Description("The State that's selected in the Dropdown list when the control loads for the first time.")]
		public string DefaultState { get; set; }

		[Category("Behaviour")]
		[Description("The State that's currently selected in the dropdown list. Use the state's Abbreviation")]
		public string SelectedState {
			get{
				this.EnsureChildControls();
				return this.StateDropdown.SelectedValue; 
			}
			//set {
			//    this.EnsureChildControls();
			//    this.StateDropdown.SelectedValue = value; 
			//}
		}

		
		protected override void OnInit(EventArgs e) {
			this.EnsureChildControls();//we can't bind data until we have our child controls to bind them to
			this.DataBind();//binding data in the OnInit as opposed in a CreateChildControls makes sure all the properties have been set so we can read them.

			base.OnInit(e);

		}


		protected override void CreateChildControls() {
			if(!string.IsNullOrEmpty(this.LegendText) && this.ContainerType == FormFieldContainerType.FIELDSET){
				this.Legend.InnerText = this.LegendText;
				this.Controls.Add(this.Legend);
			}
			if (!string.IsNullOrEmpty(this.LabelText)) {
				this.Label.Text = this.LabelText;
				if (this.LabelPosition == FormFieldLabelPosition.LEFT) {
					this.Controls.Add(this.Label);
					this.Controls.Add(this.StateDropdown);
				} else {
					this.Controls.Add(this.StateDropdown);
					this.Controls.Add(this.Label);	
				}
			} else {
				this.Controls.Add(this.StateDropdown);
			}
			//this.BindStates();

		}

	}



}
