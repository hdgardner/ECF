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
    public partial class Hierarchical : BaseStoreUserControl
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
            int menuId = 0;
            SiteMapDataSource source = null;

            if (_DataSource.Equals("catalog"))
            {
                source = this.CatalogDataSource;
                if (!String.IsNullOrEmpty(_DataMember))
                    ((CatalogSiteMapProvider)source.Provider).CatalogName = _DataMember;
            }
            else
            {
                source = this.MenuDataSource;
                // Initialize provider
                if (!String.IsNullOrEmpty(_DataMember))
                {
                    int.TryParse(_DataMember, out menuId);
                    ((CmsSiteMapProvider)MenuDataSource.Provider).ActiveTopLevelMenuId = menuId;
                }
            }

            BindMenuFromDataSource(SiteMenu, source);
        }

        /// <summary>
        /// Binds the menu.
        /// </summary>
        private void BindMenuFromDataSource(ComponentArt.Web.UI.Menu menu, SiteMapDataSource source)
        {
            //menu.Items.Add(CreateItem("StartLook"));

            int index = 0;
            foreach (SiteMapNode node in source.Provider.RootNode.ChildNodes)
            {
                // Check if item is visible
                if (!String.IsNullOrEmpty(node["visible"]) && !Boolean.Parse(node["visible"]))
                    continue;

                if (index > 0)
                    SiteMenu.Items.Add(CreateItem("BreakLook"));

                ComponentArt.Web.UI.MenuItem menuItem = new ComponentArt.Web.UI.MenuItem();
                menuItem.ID = node.Key;
                menuItem.Text = node.Title;
                menuItem.NavigateUrl = node.Url;
                menuItem.LookId = "TopItemLook";

                menu.Items.Add(menuItem);

                index++;

                LoadFromNodeRecursive(node, menuItem);
            }

            //menu.Items.Add(CreateItem("EndLook"));
            menu.DataBind();
        }

        /// <summary>
        /// Loads from node recursive.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="item">The item.</param>
        private int LoadFromNodeRecursive(SiteMapNode root, ComponentArt.Web.UI.MenuItem item)
        {
            int index = 0;
            foreach (SiteMapNode node in root.ChildNodes)
            {
                // Check if item is visible
                if (!String.IsNullOrEmpty(node["visible"]) && !Boolean.Parse(node["visible"]))
                    continue;

                ComponentArt.Web.UI.MenuItem menuItem = new ComponentArt.Web.UI.MenuItem();
                menuItem.ID = node.Key;
                menuItem.Text = node.Title;
                menuItem.NavigateUrl = node.Url;
                menuItem.LookId = "ChildItemLook";

                item.Items.Add(menuItem);

                index++;

                if (LoadFromNodeRecursive(node, menuItem) > 0)
                {
                    menuItem.Look.RightIconUrl = "arrow.gif";
                    menuItem.Look.RightIconWidth = 20;
                }
            }

            return index;
        }

        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="lookId">The look id.</param>
        /// <returns></returns>
        private ComponentArt.Web.UI.MenuItem CreateItem(string lookId)
        {
            ComponentArt.Web.UI.MenuItem breakItem = new ComponentArt.Web.UI.MenuItem();
            breakItem.LookId = lookId;
            return breakItem;
        }
    }
}