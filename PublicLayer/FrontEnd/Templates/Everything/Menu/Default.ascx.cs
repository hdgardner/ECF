using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility;
using System.Collections;
using Mediachase.Cms.Providers;

namespace Mediachase.Cms.Website.Templates.Everything.Menu
{
    public partial class Default : BaseStoreUserControl
    {
        string _DataSource = String.Empty;
        string _DataMember = String.Empty;
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Loads the context. Implements interface IContextUserControl.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void LoadContext(IDictionary context)
        {
            if (context["DataSource"] != null)
                _DataSource = context["DataSource"].ToString();

            if (context["DataMember"] != null)
                _DataMember = context["DataMember"].ToString();
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                BindMenu();
                base.CreateChildControls();
                ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Binds the menu.
        /// </summary>
        void BindMenu()
        {
            List<System.Web.UI.WebControls.MenuItem> menuItems = null;

            if (_DataSource.Equals("catalog"))
            {
                menuItems = GetCatalogItems();
            }
            else
            {
                menuItems = GetMenuItems();
            }

            SiteMenu.DataSource = menuItems;
            SiteMenu.DataBind();
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <returns></returns>
        private List<System.Web.UI.WebControls.MenuItem> GetCatalogItems()
        {
            // Initialize provider
            if (!String.IsNullOrEmpty(_DataMember))
                ((CatalogSiteMapProvider)CatalogDataSource.Provider).CatalogName = _DataMember;

            List<System.Web.UI.WebControls.MenuItem> menuItems = new List<System.Web.UI.WebControls.MenuItem>();
            if (CatalogDataSource.Provider.RootNode != null)
            {
                List<SiteMapNode> nodes = new List<SiteMapNode>();

                foreach (SiteMapNode node in CatalogDataSource.Provider.RootNode.ChildNodes)
                {
                    nodes.Add(node);
                }

                menuItems = SetMenuItemsCollection(nodes);
            }

            return menuItems;
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <returns></returns>
        private List<System.Web.UI.WebControls.MenuItem> GetMenuItems()
        {
            int menuId;
            List<System.Web.UI.WebControls.MenuItem> menuItems;

            // Initialize provider
            if (!String.IsNullOrEmpty(_DataMember))
            {
                int.TryParse(_DataMember, out menuId);
                ((CmsSiteMapProvider)MenuDataSource.Provider).ActiveTopLevelMenuId = menuId;
            }

            menuItems = new List<System.Web.UI.WebControls.MenuItem>();
            if (MenuDataSource.Provider.RootNode != null)
            {
                List<SiteMapNode> nodes = new List<SiteMapNode>();

                foreach (SiteMapNode node in MenuDataSource.Provider.RootNode.ChildNodes)
                {
                    if (!Boolean.Parse(node["visible"]))
                        continue;

                    nodes.Add(node);
                }

                menuItems = SetMenuItemsCollection(nodes);
            }

            return menuItems;
        }

        private List<System.Web.UI.WebControls.MenuItem> SetMenuItemsCollection(List<SiteMapNode> nodes)
        {
            List<System.Web.UI.WebControls.MenuItem> menuItems = new List<System.Web.UI.WebControls.MenuItem>();

            for (int index = 0; index < nodes.Count; index++)
            {
                SiteMapNode node = nodes[index];

                if (index < nodes.Count - 1)
                {
                    System.Web.UI.WebControls.MenuItem item = new System.Web.UI.WebControls.MenuItem(node.Title, node.Key);
                    item.NavigateUrl = node.Url;
                    menuItems.Add(item);
                }
                else
                {
                    //bind the last item to a separate div without a right hand boundary
                    TopMenuLastColumn.HRef = node.Url;
                    TopMenuLastColumn.InnerText = node.Title;
                }
            }

            return menuItems;
        }    
    }
}