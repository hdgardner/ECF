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
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog.Search;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntryRelationEditTab : CatalogBaseUserControl, IAdminTabControl
    {
        List<GridItem> _addedItems = new List<GridItem>();
        List<GridItem> _removedItems = new List<GridItem>();

        string _RelationTypeId = EntryRelationType.ProductVariation;
        /// <summary>
        /// Gets or sets the relation type id.
        /// </summary>
        /// <value>The relation type id.</value>
        public string RelationTypeId
        {
            get
            {
                return _RelationTypeId;
            }
            set
            {
                _RelationTypeId = value;
            }
        }

        string _ServiceMethod = "GetVariationList";
        /// <summary>
        /// Gets or sets the service method.
        /// </summary>
        /// <value>The service method.</value>
        public string ServiceMethod
        {
            get
            {
                return _ServiceMethod;
            }
            set
            {
                _ServiceMethod = value;
            }
        }

        /// <summary>
        /// Gets the catalog entry id.
        /// </summary>
        /// <value>The catalog entry id.</value>
		public int CatalogEntryId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("catalogentryid");
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
        /// Gets the type of the current entry.
        /// </summary>
        /// <value>The type of the current entry.</value>
        public string CurrentEntryType
        {
            get
            {
                return Request.QueryString["type"];
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !ItemsFilter.CausedCallback)
            {
                LoadItems(0, ItemsFilter.DropDownPageSize * 2, "");
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
            // This method filters which entries to display in the search results. 
            // By default the system displays only variations for product variations. So for instance
            // product can not have sub products and any entry for all other types of products. So
            // for instance package can have other packages and related items.
            // This behaviour can be customized by modifying the following IF statement.
            string entryType = EntryType.Variation.ToString();

            if (RelationTypeId == EntryRelationType.ProductVariation.ToString())
                entryType = EntryType.Variation.ToString();
            else
                entryType = String.Empty;

            CatalogSearchParameters pars = new CatalogSearchParameters();
            CatalogSearchOptions options = new CatalogSearchOptions();

            options.RecordsToRetrieve = iNumItems;
            options.Namespace = "Mediachase.Commerce.Catalog";
            options.StartingRecord = iStartIndex;
            options.ReturnTotalCount = true;
            pars.SqlWhereClause = String.Format("[CatalogEntry].Name like '%{0}%' OR [CatalogEntry].Code like '%{0}%'", sFilter);
            if (!String.IsNullOrEmpty(entryType))
                pars.SqlWhereClause = pars.SqlWhereClause + String.Format(" AND ClassTypeId='{0}'", entryType);

            // Add catalogs
            CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
            {
                pars.CatalogNames.Add(catalogRow.Name);
            }

            int totalRecords = 0;
            CatalogEntryDto dto = CatalogContext.Current.FindItemsDto(pars, options, ref totalRecords);

            //CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntriesDto(String.Format("%{0}%", sFilter), entryType);

            ItemsFilter.Items.Clear();

            foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry)
            {
                ComboBoxItem item = new ComboBoxItem(row.Name);
                item.Value = row.CatalogEntryId.ToString();
                item["icon"] = Page.ResolveClientUrl(String.Format("~/app_themes/Default/images/icons/{0}.gif", row.ClassTypeId));
                ItemsFilter.Items.Add(item);
            }

            ItemsFilter.ItemCount = totalRecords;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            EntryRelationDefaultGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(EntryRelationDefaultGrid_DeleteCommand);
            EntryRelationDefaultGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(EntryRelationDefaultGrid_InsertCommand);
            EntryRelationDefaultGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(EntryRelationDefaultGrid_InsertCommand);

            ItemsFilter.DataRequested += new ComboBox.DataRequestedEventHandler(ItemsFilter_DataRequested);

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the DataRequested event of the ItemsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void ItemsFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadItems(args.StartIndex, args.NumItems, args.Filter);  
        }

        /// <summary>
        /// Handles the DeleteCommand event of the EntryRelationDefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void EntryRelationDefaultGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
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
        /// Handles the InsertCommand event of the EntryRelationDefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void EntryRelationDefaultGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
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
            //autoComplete.ServiceMethod = this.ServiceMethod;
            GridHelper.BindGrid(EntryRelationDefaultGrid, "Catalog", Request.QueryString["_v"].ToString());

            if (CatalogEntryId > 0)
            {
                CatalogRelationDto relation = CatalogContext.Current.GetCatalogRelationDto(ParentCatalogId, ParentCatalogNodeId, CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
                DataTable table = new DataTable();

                table.Columns.Add(new DataColumn("ID", typeof(int)));
                table.Columns.Add(new DataColumn("Name", typeof(string)));
                table.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
                table.Columns.Add(new DataColumn("GroupName", typeof(string)));                
                table.Columns.Add(new DataColumn("SortOrder", typeof(int)));

                foreach (CatalogRelationDto.CatalogEntryRelationRow row in relation.CatalogEntryRelation)
                {
                    DataRow newRow = table.NewRow();

                    newRow["ID"] = row.ChildEntryId;
                    newRow["Name"] = CatalogContext.Current.GetCatalogEntryDto(row.ChildEntryId).CatalogEntry[0].Name;
                    newRow["Quantity"] = row.Quantity;
                    newRow["GroupName"] = row.GroupName;
                    newRow["SortOrder"] = row.SortOrder;

                    table.Rows.Add(newRow);
                }

                EntryRelationDefaultGrid.DataSource = table;
            }

            EntryRelationDefaultGrid.DataBind();
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
            CatalogEntryDto dto = (CatalogEntryDto)context["CatalogEntryDto"];

			/*if (CatalogEntryId > 0 && dto == null)
				dto = CatalogContext.Current.GetCatalogEntryDto(CatalogEntryId);
			else if (CatalogEntryId == 0 && dto == null)
				dto = new CatalogEntryDto();*/

            CatalogRelationDto relations = (CatalogRelationDto)context["CatalogRelationDto"];

			/*if (CatalogEntryId > 0 && relations == null)
				relations = CatalogContext.Current.GetCatalogRelationDto(ParentCatalogId, ParentCatalogNodeId, CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
			else if (CatalogEntryId == 0 && relations == null)
				relations = new CatalogRelationDto();*/

            CatalogRelationDto.CatalogEntryRelationDataTable tbl = relations.CatalogEntryRelation;

            foreach (GridItem item in _addedItems)
            {
                int id = Int32.Parse(item["ID"].ToString());
                decimal qty = 0;
                Decimal.TryParse(item["Quantity"].ToString(), out qty);
                string groupName = item["GroupName"].ToString();
                int sortOrder = 0;
                Int32.TryParse(item["SortOrder"].ToString(), out sortOrder);

                CatalogRelationDto.CatalogEntryRelationRow row = null;
                if (tbl.Count > 0)
                {
                    // find the existing one
                    foreach (CatalogRelationDto.CatalogEntryRelationRow nrow in tbl.Rows)
                    {
                        if (nrow.ChildEntryId == id)
                        {
                            row = nrow;
                            break;
                        }
                    }
                }

                if(row == null)
                    row = tbl.NewCatalogEntryRelationRow();

                row.ParentEntryId = dto.CatalogEntry[0].CatalogEntryId;

                //dto.CatalogEntry[0].ClassTypeId
                row.RelationTypeId = RelationTypeId;
                row.ChildEntryId = id;
                row.Quantity = qty;
                row.GroupName = groupName;
                row.SortOrder = sortOrder;

                if (row.RowState == DataRowState.Detached)
                {
                    tbl.Rows.Add(row);
                }
            }

			// delete removed items from the table
            foreach (GridItem item in _removedItems)
            {
                if (tbl.Count > 0)
                {
                    int id = Int32.Parse(item["ID"].ToString());
                    // find the existing one
                    foreach (CatalogRelationDto.CatalogEntryRelationRow row in tbl.Rows)
					{
						// if row has been deleted during previous cycle step(s), skip it
						if (row.RowState == DataRowState.Deleted)
							continue;

						// delete the row
						if (row.ChildEntryId == id)
						{
							row.Delete();
							break;
						}
                    }
                }
            }
        }
        #endregion
    }
}