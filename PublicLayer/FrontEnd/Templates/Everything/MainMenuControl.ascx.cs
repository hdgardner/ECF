using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Mediachase.Cms.Templates.Everything
{
    public partial class MainMenuControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

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
            //SiteMapNode currentNode = SiteMap.CurrentNode;
            List<System.Web.UI.WebControls.MenuItem> menuItems = new List<System.Web.UI.WebControls.MenuItem>();
            if (SiteMap.RootNode != null)
            {
                List<SiteMapNode> nodes = new List<SiteMapNode>();
                foreach (SiteMapNode node in SiteMap.RootNode.ChildNodes)
                {
                    if (!Boolean.Parse(node["visible"]))
                        continue;

                    nodes.Add(node);
                }

                for (int index = 0; index < nodes.Count; index++ )
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
            }

            SiteMenu.DataSource = menuItems;
            SiteMenu.DataBind();
        }
    }
}