using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mediachase.Cms.Providers;
using Mediachase.Cms.WebUtility;


namespace Mediachase.Cms.Website.Templates.NWTD.Menu {

	public partial class SubMenu : BaseStoreUserControl {

		#region Fields

		private string _menuName;
		private SiteMapNode _parentMenuNode;
		private SiteMapNode _currentNode;

		#endregion

		#region Properties

		protected SiteMapNode ParentMenuNode {
			get {
				if (this._parentMenuNode == null) {
					if (this.CurrentMenuNode != null && this.CurrentMenuNode.ParentNode != null)

						//if we are at the top level already, we need to make the currentnode and the parent node the same.
						if (this.CurrentMenuNode.RootNode.Key.Equals(this.CurrentMenuNode.ParentNode.Key)) this._parentMenuNode = this.CurrentMenuNode;
						else this._parentMenuNode = this.CurrentMenuNode.ParentNode;

				}

				return this._parentMenuNode;
			}
			set {
				this.ParentMenuNode = value;
			}
		}

		protected SiteMapNode CurrentMenuNode {
			get {
				if (this._currentNode == null) {
					this.IntitializeProvider();
					foreach (SiteMapNode node in MenuDataSource.Provider.RootNode.ChildNodes) {
						if(this._currentNode != null) continue;

						//if we hit a top-level node, we want to show it, and its sub-children
						if (node.Url.Equals(Cms.CMSContext.Current.CurrentUrl,StringComparison.InvariantCultureIgnoreCase)) {
							this._currentNode = node;
						}
						
						foreach (SiteMapNode subnode in node.ChildNodes) {
							if (this._currentNode != null) continue;  //continue through the loop until we find what we're looking for
							if (subnode.Url.Equals(Cms.CMSContext.Current.CurrentUrl,StringComparison.InvariantCultureIgnoreCase)) this._currentNode = subnode;
						}
					}

				}

				return this._currentNode;
			}
		
		}

		/// <summary>
		/// The name of the top level menu to search
		/// </summary>
		public string MenuName {
			get { return this._menuName; }
			set { this._menuName = value; }
		}


		#endregion

		#region Methods

		private void IntitializeProvider() {
			// Initialize provider
			if (!String.IsNullOrEmpty(_menuName)) {

				int menuId = 0;
				IDataReader reader = MenuItem.LoadAllRoot(CMSContext.Current.SiteId);
				while (reader.Read()) {
					if (reader["Text"].Equals(this._menuName)) int.TryParse(reader["MenuId"].ToString(), out menuId);
				}
				reader.Close();

				((CmsSiteMapProvider)MenuDataSource.Provider).ActiveTopLevelMenuId = menuId;
			}
		}
		
		/// <summary>
		/// Gets the menu items.
		/// </summary>
		/// <returns></returns>
		private void BindMenuItems() {
			List<System.Web.UI.WebControls.MenuItem> menuItems  = new List<System.Web.UI.WebControls.MenuItem>();

			if (MenuDataSource.Provider.RootNode != null) {
				//List<SiteMapNode> nodes = new List<SiteMapNode>();

				//now loop through the current page's parent if we found it:
				if (this.ParentMenuNode != null) {
					foreach (SiteMapNode childNode in this.ParentMenuNode.ChildNodes) {
						if (!Boolean.Parse(childNode["visible"]))
							continue;
						System.Web.UI.WebControls.MenuItem item = new System.Web.UI.WebControls.MenuItem(childNode.Title, childNode.Key);
						item.NavigateUrl = childNode.Url;
						menuItems.Add(item);
						//nodes.Add(childNode);
					}
			
				}

			}

			this.SiteMenu.DataSource = menuItems;
			this.SiteMenu.DataBind();
		}

		#endregion


		#region Event Handlers

		protected void Page_Load(object sender, EventArgs e) {
			if (this.ParentMenuNode != null) {
				this.hlSubMenuHeading.Text = this.ParentMenuNode.Title;
				this.hlSubMenuHeading.NavigateUrl = this.ParentMenuNode.Url;
			}
			this.BindMenuItems();
			
		}

		protected void SubMenu_ItemDatabound(object sender, RepeaterItemEventArgs e) {
			HyperLink itemLink = e.Item.FindControl("itemLink") as HyperLink;
			if (itemLink != null) {
				System.Web.UI.WebControls.MenuItem item = e.Item.DataItem as System.Web.UI.WebControls.MenuItem;
				if (item.NavigateUrl.Equals(this.CurrentMenuNode.Url)) itemLink.CssClass = itemLink.CssClass + " selected";
			}
		}

		#endregion









  
    }
}