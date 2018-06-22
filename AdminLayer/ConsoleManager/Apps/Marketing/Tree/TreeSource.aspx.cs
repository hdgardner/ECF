using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.BaseClasses;

public partial class Apps_Marketing_Tree_TreeSource : BasePage
{
	private const string ModuleName = "Marketing";

    public enum TreeListType
    {
		None,
		Root,
		Policies,
		Expressions
    }

    /// <summary>
    /// Gets the type of the list.
    /// </summary>
    /// <value>The type of the list.</value>
    public TreeListType ListType
    {
        get
        {
			string nodeType = Request.Form["type"];

			if (String.IsNullOrEmpty(nodeType))
				return TreeListType.Root;

			TreeListType type = TreeListType.None;

			try
			{
				type = (TreeListType)Enum.Parse(typeof(TreeListType), nodeType, true);
			}
			catch
			{
				type = TreeListType.None;
			}

			return type;
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
        switch (ListType)
        {
			case TreeListType.Policies:
                if (ProfileContext.Current.CheckPermission("marketing:policies:mng:view"))
    				BindPolicies();
				break;
			case TreeListType.Expressions:
                if (ProfileContext.Current.CheckPermission("marketing:expr:mng:view"))
                    BindExpressions();
				break;
        }
    }

    /// <summary>
    /// Binds the policies.
    /// </summary>
	private void BindPolicies()
	{
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();

		// Policies child nodes
		foreach (string key in PromotionGroup.Groups.Keys)
			nodes.Add(JsonTreeNode.CreateNode(key, PromotionGroup.Groups[key], ModuleName, "Policy-List", "group=" + key, true));

		WriteArray(nodes);
	}

    /// <summary>
    /// Binds the expressions.
    /// </summary>
	private void BindExpressions()
	{
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();

		// Expressions child nodes
		foreach (string key in ExpressionCategory.Categories.Keys)
		    nodes.Add(JsonTreeNode.CreateNode(key, ExpressionCategory.Categories[key], ModuleName, "Expression-List", "group=" + key, true));

		WriteArray(nodes);
	}

    /// <summary>
    /// Writes the array.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
	private void WriteArray(List<JsonTreeNode> nodes)
    {
        string json = JsonSerializer.Serialize(nodes);
        Response.Write(json);
    }
}