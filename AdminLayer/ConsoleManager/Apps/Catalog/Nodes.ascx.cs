using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Config;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class Nodes : CatalogBaseUserControl
    {
        private static readonly string _CommandName = "CommandName";
        private static readonly string _CommandArguments = "CommandArguments";
        private static readonly string _MoveCopyDialogCommand = "MoveCopyDialogCommand";

        private int _TotalRecords = 0;
        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>The total records.</value>
        private int TotalRecords
        {
            get
            {
                return _TotalRecords;
            }
            set
            {
                _TotalRecords = value;
            }
        }

        /// <summary>
        /// Gets the parent catalog id.
        /// </summary>
        /// <value>The parent catalog id.</value>
        public int ParentCatalogId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("catalogid");
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
                return ManagementHelper.GetIntFromQueryString("catalognodeid");
            }
        }

		int _StartRowIndex = 0;

		protected int GetMaximumRows()
		{
			return EcfListView.GetSavedPageSize(this.Page, MyListView2.ViewId, EcfListView.DefaultPageSize);
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                if (!IsPostBack)
                {
                    MyListView2.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("ID", "Type");
                }

				//MyListView2.CurrentListView.CurrentPageSize = GetMaximumRows();
				InitDataSource(_StartRowIndex, GetMaximumRows(), true, ""/*EcfListView.MakeSortExpression(EcfListView.GetSavedSorting(this.Page, MyListView2.ViewId, 
					new SortViewState("Type", SortDirection.Ascending)))*/);
                DataBind();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
            // change timeout since it is possible for us to have long running processes here
            ScriptManager.GetCurrent(this.Page).AsyncPostBackTimeout = 32000;

			MyListView2.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView2.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);
			MyListView2.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);

            Page.LoadComplete += new EventHandler(Page_LoadComplete);

            base.OnInit(e);
		}

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                object objArgs = Request.Form["__EVENTARGUMENT"];
                if (objArgs != null)
                {
                    Dictionary<string, object> cmd = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(objArgs.ToString()) as Dictionary<string, object>;
                    if (cmd != null && cmd.Count > 1)
                    {
                        object cmdName = cmd[_CommandName];
                        if (String.Compare((string)cmdName, _MoveCopyDialogCommand, true) == 0)
                        {
                            // reset start index
                            _StartRowIndex = 0;

                            // process move/copy command
                            Dictionary<string, object> args = cmd[_CommandArguments] as Dictionary<string, object>;
                            if (args != null)
                            {
                                ProcessMoveCopyCommand(args);
                                ManagementHelper.SetBindGridFlag(MyListView2.CurrentListView.ID);
                            }
                        }
                    }
                }
            }

            if (ManagementHelper.GetBindGridFlag(MyListView2.CurrentListView.ID))
            {
                // reset start index
                _StartRowIndex = 0;

				InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView2.CurrentListView.SortExpression);
                DataBind();
                MyListView2.MainUpdatePanel.Update();
            }
        }

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
		void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
		{
			AdminView view = MyListView2.CurrentListView.GetAdminView();
			foreach(ViewColumn column in view.Columns)
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// reset start index
					_StartRowIndex = 0;

					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					InitDataSource(_StartRowIndex, GetMaximumRows(), true, sortExpression);
				}
		}

        /// <summary>
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
		private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount, string orderByClause)
		{
            MyListView2.DataMember = Mediachase.Commerce.Catalog.DataSources.CatalogItemsDataSource.CatalogNodesDataSourceView.CatalogNodesTreeViewName;
            CatalogItemsDataSource.Parameters.CatalogId = ParentCatalogId;
            CatalogItemsDataSource.Parameters.ParentNodeId = ParentCatalogNodeId;
			CatalogItemsDataSource.Parameters.RecordsToRetrieve = recordsCount;
			CatalogItemsDataSource.Parameters.StartingRecord = startRowIndex;
            
            if(String.IsNullOrEmpty(orderByClause))
                orderByClause = "Type ASC, [Name] ASC";

			CatalogItemsDataSource.Parameters.OrderByClause = orderByClause;
			CatalogItemsDataSource.Parameters.ReturnInactive = true;
			CatalogItemsDataSource.Parameters.ReturnTotalCount = true;
		}

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
			//_MaximumRows = e.MaximumRows;
			_StartRowIndex = e.StartRowIndex;
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView2.CurrentListView.SortExpression);
		}

        /// <summary>
        /// Processes the move copy command.
        /// </summary>
        /// <param name="args">The args.</param>
		void ProcessMoveCopyCommand(Dictionary<string, object> args)
        {
			if (args["cmd"] != null)
			{
				string command = args["cmd"].ToString();
				if (String.Compare(command, "move", true) == 0 || String.Compare(command, "copy", true) == 0)
				{
					string folder = args["folder"] as string;
					if (folder == null)
						return;

					string[] target = folder.Split('-');

					int targetCatalogId = 0;
					int targetCatalogNodeId = 0;

					if (folder.StartsWith("Catalog-"))
					{
						targetCatalogId = Int32.Parse(target[1]);

						if (target.Length > 2)
                            Int32.TryParse(target[2], out targetCatalogNodeId);
					}
					else
					{
						targetCatalogId = Int32.Parse(target[0]);

                        if (target.Length > 1)
                            Int32.TryParse(target[1], out targetCatalogNodeId);
					}

					string[] items = MyListView2.CurrentListView.GetCheckedCollection();

					if (items != null)
					{
						for (int i = 0; i < items.Length; i++)
						{
							string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
							if (keys != null)
							{
								int id = Int32.Parse(keys[0]);
								string type = keys[1];

								if (String.Compare(command, "move", true) == 0)
								{
									if (type == "Node")
										MoveCatalogNode(ParentCatalogId, id, targetCatalogId, targetCatalogNodeId);
									else
										MoveNodeEntry(ParentCatalogId, ParentCatalogNodeId, id, targetCatalogId, targetCatalogNodeId);
								}
								else if (String.Compare(command, "copy", true) == 0)
								{
									if (type == "Node")
										CopyCatalogNode(ParentCatalogId, id, targetCatalogId, targetCatalogNodeId);
									else
										CopyNodeEntry(ParentCatalogId, ParentCatalogNodeId, id, targetCatalogId, targetCatalogNodeId);
								}
							}
						}
					}
				}
			}
        }

		#region Move/Copy
        /// <summary>
        /// Moves the node entry.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="targetCatalogId">The target catalog id.</param>
        /// <param name="targetCatalogNodeId">The target catalog node id.</param>
		private void MoveNodeEntry(int catalogId, int catalogNodeId, int catalogEntryId, int targetCatalogId, int targetCatalogNodeId)
        {
            if (catalogId != targetCatalogId)
            {
                CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(catalogEntryId);
                if (catalogEntryDto.CatalogEntry.Count > 0)
                {
                    catalogEntryDto.CatalogEntry[0].CatalogId = targetCatalogId;
                    CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);
                }
            }

            bool addNewRow = false;

            if (catalogId != targetCatalogId || catalogNodeId != targetCatalogNodeId)
            {
                if (catalogNodeId > 0)
                {
                    CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(catalogId, catalogNodeId, catalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
                    if (catalogRelationDto.NodeEntryRelation.Count > 0)
                    {
                        if (targetCatalogNodeId > 0)
                        {
                            catalogRelationDto.NodeEntryRelation[0].CatalogId = targetCatalogId;
                            catalogRelationDto.NodeEntryRelation[0].CatalogNodeId = targetCatalogNodeId;
                        }
                        else
                        {
                            catalogRelationDto.NodeEntryRelation[0].Delete();
                        }
                        CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
                    }
                    else if (targetCatalogNodeId > 0)
                    {
                        addNewRow = true;
                    }

                }
                else if (targetCatalogNodeId > 0)
                {
                    addNewRow = true;
                }

                if (addNewRow)
                {
                    CatalogRelationDto newCatalogRelationDto = new CatalogRelationDto();
                    CatalogRelationDto.NodeEntryRelationRow row = newCatalogRelationDto.NodeEntryRelation.NewNodeEntryRelationRow();
                    row.CatalogId = targetCatalogId;
                    row.CatalogNodeId = targetCatalogNodeId;
                    row.CatalogEntryId = catalogEntryId;
                    row.SortOrder = 0;
                    newCatalogRelationDto.NodeEntryRelation.AddNodeEntryRelationRow(row);
                    CatalogContext.Current.SaveCatalogRelationDto(newCatalogRelationDto);
                }
            }
        }

        /// <summary>
        /// Moves the catalog node.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="targetCatalogId">The target catalog id.</param>
        /// <param name="targetCatalogNodeId">The target catalog node id.</param>
        private void MoveCatalogNode(int catalogId, int catalogNodeId, int targetCatalogId, int targetCatalogNodeId)
        {
            if (catalogId != targetCatalogId || catalogNodeId != targetCatalogNodeId)
            {
                if (catalogNodeId > 0)
                {
                    CatalogNodeDto catalogNodeDto = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId);
                    if (catalogNodeDto.CatalogNode.Count > 0)
                    {
                        catalogNodeDto.CatalogNode[0].CatalogId = targetCatalogId;
                        catalogNodeDto.CatalogNode[0].ParentNodeId = targetCatalogNodeId;
                        CatalogContext.Current.SaveCatalogNode(catalogNodeDto);

                        CatalogNodeDto childCatalogNodesDto = CatalogContext.Current.GetCatalogNodesDto(catalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId);
                        if (childCatalogNodesDto.CatalogNode.Count > 0)
                        {
                            for (int i = 0; i < childCatalogNodesDto.CatalogNode.Count; i++)
                            {
                                MoveCatalogNode(catalogId, childCatalogNodesDto.CatalogNode[i].CatalogNodeId, targetCatalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId);
                            }
                        }

                        CatalogEntryDto catalogEntriesDto = CatalogContext.Current.GetCatalogEntriesDto(catalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId);
                        if (catalogEntriesDto.CatalogEntry.Count > 0)
                        {
                            for (int i = 0; i < catalogEntriesDto.CatalogEntry.Count; i++)
                            {
                                MoveNodeEntry(catalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId, catalogEntriesDto.CatalogEntry[i].CatalogEntryId, targetCatalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies the node entry.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="targetCatalogId">The target catalog id.</param>
        /// <param name="targetCatalogNodeId">The target catalog node id.</param>
        private void CopyNodeEntry(int catalogId, int catalogNodeId, int catalogEntryId, int targetCatalogId, int targetCatalogNodeId)
        {
            if (catalogId != targetCatalogId || catalogNodeId != targetCatalogNodeId)
            {
                CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(targetCatalogId, targetCatalogNodeId, catalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
                if (catalogRelationDto.NodeEntryRelation.Count == 0)
                {
                    CatalogRelationDto.NodeEntryRelationRow row = catalogRelationDto.NodeEntryRelation.NewNodeEntryRelationRow();
                    row.CatalogId = targetCatalogId;
                    row.CatalogNodeId = targetCatalogNodeId;
                    row.CatalogEntryId = catalogEntryId;
                    row.SortOrder = 0;
                    catalogRelationDto.NodeEntryRelation.AddNodeEntryRelationRow(row);
                    CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
                }
            }
        }

        /// <summary>
        /// Copies the catalog node.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="targetCatalogId">The target catalog id.</param>
        /// <param name="targetCatalogNodeId">The target catalog node id.</param>
        private void CopyCatalogNode(int catalogId, int catalogNodeId, int targetCatalogId, int targetCatalogNodeId)
        {
            if (catalogId != targetCatalogId || catalogNodeId != targetCatalogNodeId)
            {
                if (catalogNodeId > 0)
                {
                    CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(0, 0, 0, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogNode));
                    DataView dv = catalogRelationDto.CatalogNodeRelation.DefaultView;
                    dv.RowFilter = String.Format("CatalogId = {0} AND ParentNodeId = {1} AND ChildNodeId = {2}", targetCatalogId, targetCatalogNodeId, catalogNodeId);
                    if (dv.Count == 0)
                    {
                        CatalogRelationDto.CatalogNodeRelationRow row = catalogRelationDto.CatalogNodeRelation.NewCatalogNodeRelationRow();
                        row.CatalogId = targetCatalogId;
                        row.ParentNodeId = targetCatalogNodeId;
                        row.ChildNodeId = catalogNodeId;
                        row.SortOrder = 0;
                        catalogRelationDto.CatalogNodeRelation.AddCatalogNodeRelationRow(row);
                        CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
                    }
                }
            }
		}
		#endregion
	}
}