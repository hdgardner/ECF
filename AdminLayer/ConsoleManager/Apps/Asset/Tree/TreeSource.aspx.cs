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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Web.Console.Common;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;

public partial class Apps_Asset_Tree_TreeSource : BasePage
{
	private const string ModuleName = "Asset";

    /// <summary>
    /// Gets the node id.
    /// </summary>
    /// <value>The node id.</value>
    public int NodeId
    {
        get
        {
            int retval = 0;
            if (Request.Form["itemid"] != null)
                Int32.TryParse(Request.Form["itemid"].ToString(), out retval);
            
            return retval;
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
		List<JsonTreeNode> nodes = new List<JsonTreeNode>();
        Mediachase.Ibn.Data.Services.TreeNode[] treeNodes;

        if (NodeId > 0)
            treeNodes = TreeManager.GetChildNodes(Folder.GetAssignedMetaClass(), NodeId);
        else
            treeNodes = TreeManager.GetRootNodes(Folder.GetAssignedMetaClass());

		foreach (Mediachase.Ibn.Data.Services.TreeNode row in treeNodes)
			nodes.Add(JsonTreeNode.CreateNode(row.ObjectId.ToString(), row.ObjectId.ToString(), row.Title, ModuleName,
				"Asset-List", "id=" + row.ObjectId.ToString(), String.Empty, !row.HasChildren));

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
