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
using Mediachase.Ibn.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Catalog.Tree
{
	/// <summary>
	/// Used for building tree in move/copy dialog
	/// </summary>
	public partial class MoveCopyDialogTreeSource : BasePage
	{
		private const string ModuleName = "Catalog";

		public enum TreeListType
		{
			None,
			Root,
			Catalogs,
			Categories
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
					type = TreeListType.Root;
				}

				return type;
			}
		}

		private string[] _IdsList = null;
		private bool _IdsListInitialized = false;

        /// <summary>
        /// Gets the parent catalog id.
        /// </summary>
        /// <value>The parent catalog id.</value>
		public int ParentCatalogId
		{
			get
			{
				int id = 0;
				string strId = "";
				if (!_IdsListInitialized)
					GetCatalogIds(Request.Form["itemid"]);

				strId = _IdsList[0];

				if (Int32.TryParse(strId, out id))
					return id;
				else
					return 0;
			}
		}

        /// <summary>
        /// Gets the parent catalog node id.
        /// </summary>
        /// <value>The parent catalog node id.</value>
		public int ParentCatalogNodeId
		{
			get
			{
				int id = 0;
				string strId = "";
				if (!_IdsListInitialized)
					GetCatalogIds(Request.Form["itemid"]);

				strId = _IdsList[1];

				if (Int32.TryParse(strId, out id))
					return id;
				else
					return 0;
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
				case TreeListType.None:
				case TreeListType.Root:
					BindCatalogs();
					break;
				case TreeListType.Catalogs:
					BindCategories();
					break;
				case TreeListType.Categories:
					BindCategories();
					break;
			}
		}

        /// <summary>
        /// returns array of 2 ids - catalogId and catalogNodeId
        /// </summary>
        /// <param name="itemId">The item id.</param>
		private void GetCatalogIds(string itemId)
		{
			string[] ids = new string[2] { "", "" };
			if (!String.IsNullOrEmpty(itemId))
			{
				string[] tempIds = itemId.Split('-');

				ids[0] = tempIds[0];
				if (tempIds.Length >= 2)
					ids[1] = tempIds[1];
			}

			_IdsList = ids;
			_IdsListInitialized = true;
		}

        /// <summary>
        /// Makes the item id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
		private string MakeItemId(string catalogId, string catalogNodeId)
		{
			return String.Concat(catalogId, "-", catalogNodeId);
		}

        /// <summary>
        /// Binds the catalogs.
        /// </summary>
		private void BindCatalogs()
		{
			CatalogDto dto = CatalogContext.Current.GetCatalogDto();

			List<JsonTreeNode> nodes = new List<JsonTreeNode>();
			foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
			{
				JsonTreeNode cNode = JsonTreeNode.CreateNode(String.Format("c_{0}", row.CatalogId.ToString()), MakeItemId(row.CatalogId.ToString(), ""),
					row.Name, ModuleName, "Node-List", String.Format("catalogid={0}&catalognodeid={1}", row.CatalogId, ""),
					TreeListType.Catalogs.ToString());
				cNode.treeLoader = Request.Url.AbsoluteUri;
				nodes.Add(cNode);
			}

			WriteArray(nodes);
		}

        /// <summary>
        /// Binds the categories.
        /// </summary>
		private void BindCategories()
		{
			CatalogNodeDto dto = CatalogContext.Current.GetCatalogNodesDto(ParentCatalogId, ParentCatalogNodeId);
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
			{
				JsonTreeNode cNode = JsonTreeNode.CreateNode(String.Format("cn_{0}", row.CatalogNodeId.ToString()),
					MakeItemId(row.CatalogId.ToString(), row.CatalogNodeId.ToString()),
					row.Name, ModuleName, String.Empty,
					String.Format("catalogid={0}&catalognodeid={1}", row.CatalogId, row.CatalogNodeId),
					TreeListType.Categories.ToString());

				cNode.treeLoader = Request.Url.AbsoluteUri;
				nodes.Add(cNode);
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
}