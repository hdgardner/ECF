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
using Mediachase.Web.Console.Common;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;
using Mediachase.Web.Console.BaseClasses;

public partial class Apps_Dashboard_Tree_TreeSource : BasePage
{
    private class TreeNode
    {
        public string id;
        public string text;
        public string app;
        public string viewid;
        public List<TreeNode> children;
        public string icon;
        public string parameters;
        public bool leaf = false;
    }

    /// <summary>
    /// Gets the app.
    /// </summary>
    /// <value>The app.</value>
	public string App
	{
		get
		{
			return ManagementHelper.GetValueFromQueryString("app", String.Empty);
		}
	}

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BindList();
    }

    /// <summary>
    /// Binds the list.
    /// </summary>
    private void BindList()
    {
        SiteMapNodeCollection siteNodes = SiteMap.RootNode.ChildNodes;

        List<TreeNode> nodes = new List<TreeNode>();

        foreach (SiteMapNode siteNode in siteNodes)
        {
            if (siteNode["App"].ToLower() == App.ToLower())
                BindRecursive(ref nodes, siteNode);
        }

        WriteArray(nodes);
    }

    /// <summary>
    /// Binds the recursive.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="rootNode">The root node.</param>
    private void BindRecursive(ref List<TreeNode> nodes, SiteMapNode rootNode)
    {
        foreach (SiteMapNode siteNode in rootNode.ChildNodes)
        {
            TreeNode node = new TreeNode();
            node.text = siteNode.Title;
            node.id = siteNode.Key;
            node.app = siteNode["App"];
            node.viewid = siteNode["ViewId"];
            node.parameters = siteNode["parameters"];
            string imageUrl = String.Format("~/apps/{0}/images/{1}.png", siteNode["App"], siteNode.Key);
            if (File.Exists(Server.MapPath(imageUrl)))
                node.icon = Page.ResolveUrl(imageUrl);

            nodes.Add(node);
            List<TreeNode> children = new List<TreeNode>();
            BindRecursive(ref children, siteNode);
            if (children.Count > 0)
                node.children = children;
            else
                node.leaf = true;
        }
    }

    /// <summary>
    /// Writes the array.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    private void WriteArray(List<TreeNode> nodes)
    {
        string json = JsonSerializer.Serialize(nodes);
        Response.Write(json);
    }
}
