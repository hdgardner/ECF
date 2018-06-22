using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class ItemAssetsEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
    {
        private DataTable _AssetTable = null;
        private const string _CatalogEntryDtoString = "CatalogEntryDto";
        private const string _CatalogNodeDtoString = "CatalogNodeDto";
        List<GridItem> _addedItems = new List<GridItem>();
        List<GridItem> _removedItems = new List<GridItem>();

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !AssetItemsFilter.CausedCallback)
            {
                LoadItems(0, AssetItemsFilter.DropDownPageSize * 2, "");
				BindForm();
            }
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadItems(int iStartIndex, int iNumItems, string sFilter)
        {
            // declare collections
            Folder[] folders = null;
            Mediachase.Ibn.Data.Services.TreeNode[] nodes = null;
            FolderElement[] elements = null;
            string path = sFilter;
            int totalItemsCount = 0;

            // Load all elements if filter is empty
            if (String.IsNullOrEmpty(sFilter))
            {
                int folderCount = Folder.GetTotalCount(Folder.GetAssignedMetaClass());
                int elementCount = FolderElement.GetTotalCount(FolderElement.GetAssignedMetaClass());
                bool skipFolders = false;
                if (elementCount + folderCount <= iStartIndex)
                {
                    iStartIndex = 0;
                    iNumItems = 20;
                }
                else if (folderCount < iStartIndex)
                {
                    skipFolders = true;
                    iStartIndex = iStartIndex - folderCount;
                }

                if (!skipFolders)
                    folders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElementCollection(), new SortingElementCollection(), iStartIndex, iNumItems);

                elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElementCollection(), new SortingElementCollection(), iStartIndex, iNumItems);

                totalItemsCount = elementCount + folderCount;
            }
            else
            {
                // Calculate outline here
                string outline = String.Empty;
                string[] outlineArray = sFilter.Split(new char[] { '/' });


                if (outlineArray.Length > 1)
                {
                    int index = 0;
                    while (index < outlineArray.Length - 1)
                    {
                        Folder[] outlineFolders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("Name", FilterElementType.Equal, outlineArray[index]) });
                        if (outlineFolders.Length > 0)
                        {
                            if (String.IsNullOrEmpty(outline))
                                outline = outlineFolders[0].PrimaryKeyId.ToString();
                            else
                                outline += "." + outlineFolders[0].PrimaryKeyId.ToString();
                        }
                        index++;
                    }
                }

                // Must be first element
                if (String.IsNullOrEmpty(outline))
                {
                    if (String.IsNullOrEmpty(sFilter))
                        nodes = TreeManager.GetRootNodes(Folder.GetAssignedMetaClass());
                    else
                        folders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("Name", FilterElementType.StartsWith, sFilter) });
                }
                else
                {
                    Mediachase.Ibn.Data.Services.TreeNode node = TreeManager.GetNodeByOulineNumber(Folder.GetAssignedMetaClass(), outline);
                    nodes = TreeManager.GetChildNodes(Folder.GetAssignedMetaClass(), node.ObjectId);
                }
                
                if (!String.IsNullOrEmpty(path))
                {
                    if (path.Contains("/"))
                    {
                        path = path.Substring(0, path.LastIndexOf('/'));
                    }

                    elements = FolderElement.GetElementsByPath(path);
                }
                else
                    path = String.Empty;

                // Do blank keyword search
                if (elements == null && !sFilter.Contains("/"))
                    elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("Name", FilterElementType.Contains, sFilter) });

                if (elements == null)
                    elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("ParentId", FilterElementType.Equal, 0) });


                // Count all returned items
                totalItemsCount = elements.Length;

                if (nodes != null)
                    totalItemsCount += nodes.Length;

                if (folders != null)
                    totalItemsCount += folders.Length;
             
            }

            AssetItemsFilter.Items.Clear();



            AddItems(folders);
            AddItems(nodes);
            AddItems(elements);

            int iEndIndex = Math.Min(iStartIndex + iNumItems, totalItemsCount);
            AssetItemsFilter.ItemCount = Math.Min(totalItemsCount, iEndIndex + AssetItemsFilter.DropDownPageSize);
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="folders">The folders.</param>
        private void AddItems(Folder[] folders)
        {
            if (folders == null)
                return;

            foreach (Folder folder in folders)
            {
                ComboBoxItem item = new ComboBoxItem(BuildFullPath(folder) + folder.Name + "/");
                item.Value = folder.PrimaryKeyId.ToString();
                item["icon"] = Page.ResolveClientUrl("~/App_Themes/Default/images/icons/Node.gif");
                AssetItemsFilter.Items.Add(item);
            }
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="elements">The elements.</param>
        private void AddItems(FolderElement[] elements)
        {
            if (elements == null)
                return;

            foreach (FolderElement element in elements)
            {
                ComboBoxItem item = new ComboBoxItem(BuildFullPath(element) + element.Name);
                item.Value = element.PrimaryKeyId.ToString();
                item["icon"] = Page.ResolveClientUrl(CHelper.GetIcon(element.Name));
                AssetItemsFilter.Items.Add(item);
            }
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        private void AddItems(Mediachase.Ibn.Data.Services.TreeNode[] nodes)
        {
            if (nodes == null)
                return;

            foreach (Mediachase.Ibn.Data.Services.TreeNode node in nodes)
            {
                ComboBoxItem item = new ComboBoxItem(BuildFullPath(Folder.GetAssignedMetaClass(), node) + node.Title + "/");
                item.Value = node.ObjectId.ToString();
                item["icon"] = Page.ResolveClientUrl("~/App_Themes/Default/images/icons/Node.gif");
                AssetItemsFilter.Items.Add(item);
            }
        }

        /// <summary>
        /// Builds the full path.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private string BuildFullPath(FolderElement element)
        {
            Folder currentFolder;
            string path = String.Empty;

            object parentId = null;
            
            parentId = element.Properties["ParentId"].Value;

            if(parentId == null)
                return String.Empty;

            do
            {
                Folder[] folders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderId", FilterElementType.Equal, parentId) });
                path = folders[0].Name + "/" + path;
                currentFolder = folders[0];
            } while ((parentId = currentFolder.Properties["ParentId"].Value) != null);
               
            return path;
        }

        /// <summary>
        /// Builds the full path.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        private string BuildFullPath(Folder folder)
        {
            Folder currentFolder = folder;
            string path = String.Empty;

            object parentId = null;
            while ((parentId = currentFolder.Properties["ParentId"].Value) != null)
            {
                Folder[] folders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderId", FilterElementType.Equal, parentId) });

                if (folders != null && folders.Length > 0)
                {
                    path = folders[0].Name + "/" + path;
                    currentFolder = folders[0];
                }
                else
                {
                    break;
                }
            }

            return path;
        }

        /// <summary>
        /// Builds the full path.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private string BuildFullPath(Mediachase.Ibn.Data.Meta.Management.MetaClass metaClass, Mediachase.Ibn.Data.Services.TreeNode node)
        {
            string path = String.Empty;
            Mediachase.Ibn.Data.Services.TreeNode[] nodes = TreeManager.GetPathToNode(metaClass, node);
            if (nodes.Length > 0)
            {
                foreach (Mediachase.Ibn.Data.Services.TreeNode treeNode in nodes)
                {
                    path += treeNode.Title + "/";
                }
            }

            return path;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            AssetsDefaultGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssetsDefaultGrid_DeleteCommand);
            AssetsDefaultGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssetsDefaultGrid_InsertCommand);
            AssetsDefaultGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssetsDefaultGrid_InsertCommand);

            AssetItemsFilter.DataRequested += new ComboBox.DataRequestedEventHandler(AssetItemsFilter_DataRequested);

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the DataRequested event of the AssetItemsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void AssetItemsFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadItems(args.StartIndex, args.NumItems, args.Filter);  
        }

        /// <summary>
        /// Handles the DeleteCommand event of the AssetsDefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void AssetsDefaultGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {            
            int id = Int32.Parse(e.Item["ID"].ToString());

            foreach (GridItem item in _addedItems)
            {
                if (Int32.Parse(item["ID"].ToString()) == id)
                {
                    _addedItems.Remove(item);
                    break;
                }
            }

            _removedItems.Add(e.Item);
        }

        /// <summary>
        /// Handles the InsertCommand event of the AssetsDefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void AssetsDefaultGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            int id = Int32.Parse(e.Item["ID"].ToString());

            foreach (GridItem item in _removedItems)
            {
                if (Int32.Parse(item["ID"].ToString()) == id)
                {
                    _removedItems.Remove(item);
                    break;
                }
            }

            _addedItems.Add(e.Item);
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(AssetsDefaultGrid, "Catalog", "Assets-List");

            DataTable table = new DataTable();

            table.Columns.Add(new DataColumn("ID", typeof(int)));
            table.Columns.Add(new DataColumn("AssetType", typeof(string)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("GroupName", typeof(string)));
            table.Columns.Add(new DataColumn("SortOrder", typeof(int)));

            foreach (DataRow row in _AssetTable.Rows)
            {
                DataRow newRow = table.NewRow();

                newRow["ID"] = row["AssetKey"];
                newRow["AssetType"] = row["AssetType"];
                if (row["AssetType"].Equals("folder"))
                {
                    Folder[] folders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderId", FilterElementType.Equal, row["AssetKey"]) });
                    if (folders != null && folders.Length > 0)
                    {
                        newRow["Name"] = BuildFullPath(folders[0]) + folders[0].Name;
                    }
                }
                else
                {
                    FolderElement[] elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderElementId", FilterElementType.Equal, row["AssetKey"]) });
                    if (elements != null && elements.Length > 0)
                    {
                        newRow["Name"] = BuildFullPath(elements[0]) + elements[0].Name;
                    }
                }
                newRow["GroupName"] = row["GroupName"];
                newRow["SortOrder"] = row["SortOrder"];

                table.Rows.Add(newRow);
            }

            AssetsDefaultGrid.DataSource = table;

            AssetsDefaultGrid.DataBind();
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();            
            BindForm();
        }
		
        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            int? catalogEntryId = null;
            int? catalogNodeId = null;
            if (context.Contains(_CatalogEntryDtoString))
            {
                CatalogEntryDto dto = (CatalogEntryDto)context[_CatalogEntryDtoString];
                _AssetTable = dto.CatalogItemAsset;
                catalogEntryId = dto.CatalogEntry[0].CatalogEntryId;
            }
            else if (context.Contains(_CatalogNodeDtoString))
            {
                CatalogNodeDto dto = (CatalogNodeDto)context[_CatalogNodeDtoString];
                _AssetTable = dto.CatalogItemAsset;
                catalogNodeId = dto.CatalogNode[0].CatalogNodeId;
            }

            DataTable tbl = _AssetTable;

            foreach (GridItem item in _addedItems)
            {
                string id = item["ID"].ToString();
                string assetType = item["AssetType"].ToString();
                string groupName = item["GroupName"].ToString();
                int sortOrder = 0;
                Int32.TryParse(item["SortOrder"].ToString(), out sortOrder);

                DataRow row = null;
                if (tbl.Rows.Count > 0)
                {
                    // find the existing one
                    foreach (DataRow nrow in tbl.Rows)
                    {
                        if (nrow["AssetKey"].ToString().Equals(id) && nrow["AssetType"].ToString().Equals(assetType))
                        {
                            row = nrow;
                            break;
                        }
                    }
                }

                if(row == null)
                    row = tbl.NewRow();

                if (catalogEntryId != null)
                    row["CatalogEntryId"] = catalogEntryId;

                if (catalogNodeId != null)
                    row["CatalogNodeId"] = catalogNodeId;

                row["AssetKey"] = id;
                row["AssetType"] = assetType;
                row["GroupName"] = groupName;
                row["SortOrder"] = sortOrder;

                if (row.RowState == DataRowState.Detached)
                {
                    tbl.Rows.Add(row);
                }
            }

			// delete removed items from the table
            foreach (GridItem item in _removedItems)
            {
                if (tbl.Rows.Count > 0)
                {
                    string id = item["ID"].ToString();
                    string assetType = item["AssetType"].ToString();
                    // find the existing one
                    foreach (DataRow row in tbl.Rows)
					{
						// if row has been deleted during previous cycle step(s), skip it
						if (row.RowState == DataRowState.Deleted)
							continue;

						// delete the row
                        if (row["AssetKey"].Equals(id) && row["AssetType"].Equals(assetType))
						{
							row.Delete();
							break;
						}
                    }
                }
            }
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            if (context.Contains(_CatalogEntryDtoString))
                _AssetTable = ((CatalogEntryDto)context[_CatalogEntryDtoString]).CatalogItemAsset;
            else if (context.Contains(_CatalogNodeDtoString))
                _AssetTable = ((CatalogNodeDto)context[_CatalogNodeDtoString]).CatalogItemAsset;
        }

        #endregion
    }
}