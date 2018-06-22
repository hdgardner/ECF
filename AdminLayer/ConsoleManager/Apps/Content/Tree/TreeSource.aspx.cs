using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using mc = Mediachase.Cms;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

public partial class Apps_Content_Tree_TreeSource : BasePage
{
	private const string ModuleName = "Content";

    public enum TreeListType
    {
        None,
        Root,
        MyWork,
        Sites,
        Folders,
        Menus,

		Templates
	}

	#region Properties
	private string userId = string.Empty;
    /// <summary>
    /// Gets the user id.
    /// </summary>
    /// <value>The user id.</value>
	public string UserId
	{
		get
		{
			userId = Membership.GetUser(Page.User.Identity.Name).ProviderUserKey.ToString();
			return userId;
		}
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
    /// Gets the site id.
    /// </summary>
    /// <value>The site id.</value>
    public Guid SiteId
    {
        get
        {
            return new Guid(Request.Form["itemid"].ToString());
        }
	}
	#endregion

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
			//case TreeListType.Root:
			//    BindRoot();
			//    break;
            case TreeListType.MyWork:
                BindMyWork();
                break;
            case TreeListType.Sites:
                BindSite();
                break;
            case TreeListType.Folders:
                BindFolders();
                break;
            case TreeListType.Menus:
                BindMenu();
                break;
			case TreeListType.Templates:
				BindTemplates();
				break;
        }
    }

    /// <summary>
    /// Makes the node id.
    /// </summary>
    /// <param name="baseId">The base id.</param>
    /// <returns></returns>
	private string MakeNodeId(string baseId)
	{
		return String.Concat(ModuleName, "_", baseId);
	}

    /// <summary>
    /// Binds the root.
    /// </summary>
    private void BindRoot()
    {
        List<JsonTreeNode> nodes = new List<JsonTreeNode>();

        //check permissions before adding the nodes
        if (ProfileContext.Current.CheckPermission("content:admin:workflow:mng:edit"))
    		nodes.Add(JsonTreeNode.CreateNode(MakeNodeId("MyWork"), String.Empty, "My Work", ModuleName, String.Empty, String.Empty, TreeListType.MyWork.ToString()));

        if (ProfileContext.Current.CheckPermission("content:site:mng:view"))
            nodes.Add(JsonTreeNode.CreateNode(MakeNodeId("Sites"), String.Empty, "Sites", ModuleName, "Site-List", String.Empty, TreeListType.Sites.ToString()));
        WriteArray(nodes);
    }

    #region Bind Menus
    /// <summary>
    /// Binds the menu.
    /// </summary>
    private void BindMenu()
    {
        List<JsonTreeNode> nodes = new List<JsonTreeNode>();
        IDataReader reader = mc.MenuItem.LoadAllRoot(SiteId);
        while (reader.Read())
        {
            nodes.Add(BindRootMenuNode(reader, true));
        }
        reader.Close();

        WriteArray(nodes);
    }

    /// <summary>
    /// Binds the root menu node.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="isroot">if set to <c>true</c> [isroot].</param>
    /// <returns></returns>
    private JsonTreeNode BindRootMenuNode(IDataReader reader, bool isroot)
    {
		string mainMenuItemId = reader["MenuItemId"].ToString();

        JsonTreeNode newNode = new JsonTreeNode();
        newNode.text = reader["Text"].ToString();
        newNode.id = String.Format("Root-{0}", reader["MenuItemId"].ToString());
		newNode.itemid = mainMenuItemId;

		SiteDto siteDto = SiteManager.GetSite(SiteId);
		if (siteDto != null && siteDto.SiteLanguage.Count > 0)
		{
			EnumerableRowCollection<SiteDto.SiteLanguageRow> siteLangRows = siteDto.SiteLanguage.AsEnumerable();

			// add site's languages
			using (IDataReader langReader = mc.Language.GetAllLanguages())
			{
				while (langReader.Read())
				{
					string langName = langReader["LangName"].ToString();
					var query = from row in siteLangRows
								where String.Compare(row.Field<string>("LanguageCode"), langName) == 0
								select row;

					// skip language, if it is not available for the site
					if (query.Count() == 0)
						continue;

					int langId = Int32.Parse(langReader["LangId"].ToString());
					CultureInfo culture = CultureInfo.CreateSpecificCulture(langName);
					JsonTreeNode langNode = new JsonTreeNode();
					langNode.text = culture.DisplayName;
					langNode.id = String.Format("menu-{0}-{1}", mainMenuItemId, langName);
					langNode.app = ModuleName;
					langNode.viewid = "MenuItem-List";
					langNode.parameters = String.Format("menuitemid={0}&parentid={1}&langid={2}&siteid={3}", mainMenuItemId, mainMenuItemId, langId, SiteId.ToString());

					if (newNode.children == null)
						newNode.children = new List<JsonTreeNode>();

					newNode.children.Add(langNode);

					DataTable MenuTable = mc.MenuItem.LoadSubMenuDT(Int32.Parse(newNode.itemid), langId);
					foreach (DataRow row in MenuTable.Rows)
					{
						int menuItemId = (int)row["MenuItemId"];

						using (IDataReader reader2 = mc.MenuItem.LoadById(menuItemId, langId))
						{
							if (reader2.Read())
							{
								if (langNode.children == null)
									langNode.children = new List<JsonTreeNode>();

								langNode.children.Add(BindMenuNode(reader2, menuItemId, langId));
							}
                            reader2.Close();
						}
					}
				}
                langReader.Close();
			}
		}

        return newNode;
    }

    /// <summary>
    /// Binds the menu node.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="langId">The lang id.</param>
    /// <returns></returns>
    private JsonTreeNode BindMenuNode(IDataReader reader, int parentItemId, int langId)
    {
        JsonTreeNode newNode = new JsonTreeNode();
        newNode.text = reader["Text"].ToString();
        newNode.id = reader["MenuItemId"].ToString();
        newNode.app = ModuleName;
        newNode.viewid = "MenuItem-List";
        newNode.parameters = String.Format("menuitemid={0}&parentid={1}&langid={2}&siteid={3}", reader["MenuItemId"].ToString(), parentItemId, langId, SiteId.ToString());

        DataTable MenuTable = mc.MenuItem.LoadSubMenuDT(Int32.Parse(newNode.id));
        foreach (DataRow row in MenuTable.Rows)
        {
            int menuItemId = (int)row["MenuItemId"];

            using (IDataReader reader2 = mc.MenuItem.LoadById(menuItemId, langId))
            {
                if (reader2.Read())
                {
                    if (newNode.children == null)
                        newNode.children = new List<JsonTreeNode>();

                    newNode.children.Add(BindMenuNode(reader2, menuItemId, langId));
                }
                reader2.Close();
            }
        }

        if (newNode.children == null)
            newNode.leaf = true;

        return newNode;
    }
    #endregion

    #region Bind Folders
    /// <summary>
    /// Binds the folders.
    /// </summary>
    private void BindFolders()
    {
        List<JsonTreeNode> nodes = new List<JsonTreeNode>();
        using (IDataReader reader = FileTreeItem.GetItemById(FileTreeItem.GetRoot(SiteId)))
        {
            if (reader.Read())
            {
				string nodeId = MakeNodeId("folder-" + reader["PageId"].ToString());
                JsonTreeNode node = JsonTreeNode.CreateNode(nodeId, reader["PageId"].ToString(), "/", "Content", "Folder-List", String.Format("folderid={0}&siteid={1}", reader["PageId"].ToString(), SiteId.ToString()), TreeListType.Folders.ToString());
                nodes.Add(node);
                BindFolders(node, (int)reader["PageId"]);
            }
            reader.Close();
        }

        WriteArray(nodes);
    }

    /// <summary>
    /// Binds the folders.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <param name="folderId">The folder id.</param>
    private void BindFolders(JsonTreeNode root, int folderId)
    {
        using (IDataReader reader = FileTreeItem.LoadItemByFolderId(folderId))
        {
            while (reader.Read())
            {
                if ((bool)reader["IsFolder"])
                {
					string nodeId = MakeNodeId("folder-" + reader["PageId"].ToString());
                    JsonTreeNode node = JsonTreeNode.CreateNode(nodeId, reader["PageId"].ToString(), (string)reader["Name"], "Content", "Folder-List", String.Format("folderid={0}&siteid={1}", reader["PageId"].ToString(), SiteId.ToString()), TreeListType.Folders.ToString());
                    
                    if (root.children == null)
                        root.children = new List<JsonTreeNode>();

                    root.children.Add(node);
                    BindFolders(node, (int)reader["PageId"]);
                }
            }
            reader.Close();
        }
    }
    #endregion

    #region Bind Work Review/Approval
    /// <summary>
    /// Binds my work.
    /// </summary>
    private void BindMyWork()
    {

        //don't load "my work" subnodes if permissions not allocated
        if (!ProfileContext.Current.CheckPermission("content:admin:workflow:mng:view"))
            return;

        List<JsonTreeNode> nodes = new List<JsonTreeNode>();

        DataTable dtPV = PageVersion.GetWorkVersionByUserId(Page.User.Identity.Name);

        if (dtPV != null && dtPV.Rows.Count > 0)
        {
            DataView dv = dtPV.DefaultView;

            //APPLY FILTER
            int defaultWorkflowId = -1;
            using (IDataReader reader = Workflow.LoadDefault())
            {
                if (reader.Read())
                {
                    defaultWorkflowId = (int)reader["WorkflowId"];
                }
                reader.Close();
            }

            int PublishId = WorkflowStatus.GetLastByWorkflowId(defaultWorkflowId);
            int DraftId = WorkflowStatus.DraftId;

            dv.RowFilter = String.Format("StateId = 1 AND StatusId NOT IN (-1, {0}, {1})", PublishId, DraftId);
            if (dv.Count > 0)
            {
				nodes.Add(JsonTreeNode.CreateNode(String.Format("{0} <b>({1})</b>", Resources.Admin.ForApprove, dv.Count), ModuleName, "Approve-List", "filter=approve", true));
            }
            else
            {
				nodes.Add(JsonTreeNode.CreateNode(Resources.Admin.ForApprove, ModuleName, "Approve-List", "approve", true));
            }

            dv.RowFilter = String.Format("StateId = 2 AND StatusId NOT IN (-1, {0})", PublishId); 
            if (dv.Count > 0)
            {
				nodes.Add(JsonTreeNode.CreateNode(String.Format("{0} <b>({1})</b>", Resources.Admin.Rollbacked, dv.Count), ModuleName, "Reject-List", "filter=reject", true));
            }
            else
            {
				nodes.Add(JsonTreeNode.CreateNode(Resources.Admin.Rollbacked, ModuleName, "Reject-List", "filter=reject", true));
            }

            dv.RowFilter = String.Format("StateId = 1 AND StatusId = {0}", DraftId);
            if (dv.Count > 0)
            {
				nodes.Add(JsonTreeNode.CreateNode(String.Format("{0} <b>({1})</b>", Resources.Admin.Drafts, dv.Count), ModuleName, "Draft-List", "filter=draft", true));
            }
            else
            {
				nodes.Add(JsonTreeNode.CreateNode(Resources.Admin.Drafts, ModuleName, "Draft-List", "filter=draft", true));
            }
        }

        WriteArray(nodes);
    }
    #endregion

    #region Bind Sites
    /// <summary>
    /// Binds the site.
    /// </summary>
    private void BindSite()
    {
        List<JsonTreeNode> nodes = new List<JsonTreeNode>();

        SiteDto dto = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId, true);

        foreach (SiteDto.SiteRow row in dto.Site.Rows)
        {
			JsonTreeNode newNode = JsonTreeNode.CreateNode(MakeNodeId(row.SiteId.ToString()), row.SiteId.ToString(), row.Name, ModuleName, String.Empty, String.Format("siteid={0}", row.SiteId), TreeListType.None.ToString());

			string treeLoader = Request.Url.AbsoluteUri;
			if(!row.IsActive)
				newNode.icon = Page.ResolveUrl("~/Apps/Content/images/folder-disabled.gif");

            newNode.children = new List<JsonTreeNode>();

            //check permissions first
            if (ProfileContext.Current.CheckPermission("content:site:nav:mng:view"))
            {
                JsonTreeNode foldersNode = JsonTreeNode.CreateNode(MakeNodeId(row.SiteId.ToString() + "FoldersAndPages"), row.SiteId.ToString(), "Folders & Pages", ModuleName, String.Empty, String.Format("siteid={0}", row.SiteId), TreeListType.Folders.ToString());
                foldersNode.icon = Page.ResolveUrl("~/Apps/Content/images/Folder-Documents.png");
                foldersNode.treeLoader = treeLoader; // set current page as tree toader for new nodes
                newNode.children.Add(foldersNode);
            }

            //check permissions first
            if (ProfileContext.Current.CheckPermission("content:site:menu:mng:view"))
            {
                JsonTreeNode menusNode = JsonTreeNode.CreateNode(MakeNodeId(row.SiteId.ToString() + "Menus"), row.SiteId.ToString(), "Menus", ModuleName, "Menu-List", String.Format("siteid={0}", row.SiteId), TreeListType.Menus.ToString());
                menusNode.icon = Page.ResolveUrl("~/Apps/Content/images/Menus.png");
                menusNode.treeLoader = treeLoader;
                newNode.children.Add(menusNode);
            }

            nodes.Add(newNode);
        }

        WriteArray(nodes);
    }
    #endregion

    /// <summary>
    /// Binds the templates.
    /// </summary>
	private void BindTemplates()
	{
        SecurityManager.CheckRolePermission("content:admin:templates:mng:view");

		List<JsonTreeNode> nodes = new List<JsonTreeNode>();

		using (IDataReader langReader = Language.GetAllLanguages())
		{
			while (langReader.Read())
			{
				int langId = Int32.Parse(langReader["LangId"].ToString());
				CultureInfo culture = CultureInfo.CreateSpecificCulture(langReader["LangName"].ToString());
				string langName = culture.Name;
				nodes.Add(JsonTreeNode.CreateNode("TemplateLanguage-" + langId, culture.DisplayName, ModuleName, "Templates-List", String.Format("lang={0}", langName.ToLower()), true));
			}
            langReader.Close();
		}

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
