using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace OakTree.Web.UI.WebControls {
	public class TabbedControl:CompositeControl {

		#region protected and private memebers
		private List<Tab> _tabs;
		private HtmlGenericControl _tabsUl;
		protected HtmlGenericControl TabsUl {
			get {
				if (this._tabsUl == null) {
					this._tabsUl = new HtmlGenericControl("ul");
					this._tabsUl.Attributes.Add("class", "oakTree-tabList");
				}
				return this._tabsUl;
			}
		}
		#endregion

		#region public properties

		[Description("The tabs in the control")]
		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<Tab> Tabs { 
			get {
				if (this._tabs == null) this._tabs = new List<Tab>();
				return this._tabs;
			} 
		}

		[Category("Behaviour")]
		[Description("Whether to persist the state of the tabs on page refreshes.")]
		public bool PersistSelectedTab { get; set; }
		
		[Category("Behaviour"),Description("The tab that will be selected by default. This is overridden if PersistSelctedTab is set to True")]
		public Tab SelectedTab { 
			get{
				return this.Tabs.SingleOrDefault(tab => tab.Selected == true);
			}
			set {
				foreach (Tab tab in this.Tabs) {
					if (tab.Equals(value)) tab.Selected = true;
					else tab.Selected = false;
				}
			}
		}

		#endregion

		#region method and property overrides

		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}


		protected override void OnPreRender (EventArgs e) {
			foreach (Tab tab in this.Tabs) {
				this.TabsUl.Controls.Add(tab.Li);
			}

		
			
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.JQUERY_JS);
			this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.TABBED_CONTROL_JS);
			if(this.PersistSelectedTab){
				this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.OAKTREE_UTILITIES_JS);
				this.Page.ClientScript.RegisterClientScriptResource(typeof(ControlResources), ControlResources.OAKTREE_WEB_UI_COOKIE);
				ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, "{persistSelectedTab:true}");
			}
			else ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this);
			base.OnPreRender(e);
		}

		protected override void CreateChildControls() {
			this.Controls.Add(this.TabsUl);		
			foreach(Tab tab in this.Tabs){
				this.Controls.Add(tab);
			}
			if (this.SelectedTab == null) this.SelectedTab = this.Tabs[0];
		}

		#endregion
	}
	
	[TypeConverter(typeof(ExpandableObjectConverter)),ToolboxItem(false)]
	public class Tab:Panel {

		#region protected and private members
		private bool _selected;
		private HtmlGenericControl _li;
		private HtmlAnchor _a;
		protected HtmlAnchor A {
			get { 
				if (this._a == null) this._a = new HtmlAnchor();
				return this._a;
			}

		}
		#endregion

		#region public properties

		[EditorBrowsable(EditorBrowsableState.Never)]
		public HtmlGenericControl Li {
			get {
				if (this._li == null) {
					this._li = new HtmlGenericControl("li");
					this._li.Attributes.Add("class", "oakTree-tabListItem");
				}
				return this._li;
			}
		}

		[Category("Appearance"), Description("The Text that will appear in the Tab menu")]
		public string Title { get; set; }
		
		[Category("Behaviour"), Description("Whether this is the selected tab")]
		public bool Selected{
			get {
				return this._selected;
			}
			set {
				if (value) {
					this.Style.Remove("display");
					this.A.Attributes["class"] = "selectedTab";
				} else {
					this.Style.Add("display", "none");
					this.A.Attributes.Remove("class");
				}
				this._selected = value;
			}

		}

		#endregion

		#region overrides

		protected override void OnPreRender(EventArgs e) {
			this.A.InnerText = this.Title;
			this.A.HRef = "#" + this.ClientID;
			Li.Controls.Add(this._a);

			if(!this._selected) this.Style.Add("display", "none");

			base.OnPreRender(e);
		}
		

		protected override void OnLoad(EventArgs e) {
			if (string.IsNullOrEmpty(this.CssClass)) this.CssClass = "oakTree-tabContainer";
			else this.CssClass += " oakTree-tabContainer";
			base.OnLoad(e);
		}

		#endregion
	}
}
