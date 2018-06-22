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
using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog.Search;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class EntryAssociationEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _CatalogAssociationDtoViewStateString = "CatalogAssociationDto_AssociationsEdit";
		private const string _CatalogAssociationDtoString = "CatalogAssociationDto";

		private const string _CatalogAssociationIdString = "CatalogAssociationId";
		private const string _CatalogEntryIdString = "CatalogEntryId";
		private const string _SortOrderString = "SortOrder";
		private const string _AssociationTypeIdString = "AssociationTypeId";

		List<GridItem> _addedItems = new List<GridItem>();
        List<GridItem> _removedItems = new List<GridItem>();

		CatalogAssociationDto _CatalogAssociationDto;

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
        /// Gets the selected association id.
        /// </summary>
        /// <value>The selected association id.</value>
		public int SelectedAssociationId
		{
			get
			{
				string strId = AssociationsFilter.SelectedValue;
				int id = 0;
				if (!String.IsNullOrEmpty(strId) && Int32.TryParse(strId, out id))
					return id;
				return 0;
			}
		}

        /// <summary>
        /// Gets the current catalog association dto.
        /// </summary>
        /// <value>The current catalog association dto.</value>
		protected CatalogAssociationDto CurrentCatalogAssociationDto
		{
			get
			{
				/*CatalogAssociationDto catalogAssociationDto = ViewState[_CatalogAssociationDtoViewStateString] as CatalogAssociationDto;
				if (catalogAssociationDto == null)
				{
					catalogAssociationDto = CatalogContext.Current.GetCatalogAssociationDtoByEntryId(CatalogEntryId);
					ViewState[_CatalogAssociationDtoViewStateString] = catalogAssociationDto;
				}
				
				return catalogAssociationDto;*/
				return _CatalogAssociationDto;
			}
			/*set
			{
				ViewState[_CatalogAssociationDtoViewStateString] = value;
			}*/
		}

        /// <summary>
        /// Checks the post back.
        /// </summary>
        /// <returns></returns>
		private bool CheckPostBack()
		{
			return !Page.IsPostBack && !AssociationsFilter.CausedCallback && 
				!AssociataionItemsFilter.CausedCallback &&
				!AssociationItemsGrid.CausedCallback ? false : true;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!CheckPostBack())
				BindForm();

			//string selectedAssociation = AssociationsFilter.SelectedValue;
			//LoadItems(0, AssociationsFilter.DropDownPageSize * 2, "");
			//if (!String.IsNullOrEmpty(selectedAssociation))
			//    AssociationsFilter.SelectedValue = selectedAssociation;

            if (!AssociataionItemsFilter.CausedCallback)
			    LoadAssociationItems(0, AssociataionItemsFilter.DropDownPageSize * 2, "");

			pnlSelectedAssociation.Visible = SelectedAssociationId != 0 ? true : false;

			AssociationItemsGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssociationItemsGrid_DeleteCommand);
			AssociationItemsGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssociationItemsGrid_InsertCommand);
			AssociationItemsGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(AssociationItemsGrid_InsertCommand);
			AssociationItemsGrid.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(AssociationItemsGrid_OnNeedRebind);
			AssociationItemsGrid.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(AssociationItemsGrid_OnNeedDataSource);
			AssociationItemsGrid.PreRender += new EventHandler(AssociationItemsGrid_PreRender);
        }

		#region Bind Methods
        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			if (CurrentCatalogAssociationDto != null)
			{
				BindGrid();
				//BindAssociationsFilter();
				LoadItems(0, AssociationsFilter.DropDownPageSize * 2, "");
				AssociationItemAddButton.OnClientClick = String.Format("AssociationItemsGrid_AddRow('{0}');return;", GetDefaultAssociationTypeId());
			}
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			base.DataBind();
			if (!this.IsPostBack)
				GridHelper.BindGrid(AssociationItemsGrid, "Catalog", "ProductAssociations");
		}

        /// <summary>
        /// Binds the grid.
        /// </summary>
		private void BindGrid()
		{
			GridHelper.BindGrid(AssociationItemsGrid, "Catalog", "ProductAssociations");
			SetAssociationItemsGridDataSource(SelectedAssociationId);
			AssociationItemsGrid.DataBind();
		}

        /// <summary>
        /// Binds the associations filter.
        /// </summary>
		private void BindAssociationsFilter()
		{
			//AssociationsFilter.Items.Clear();
			//CatalogAssociationDto.CatalogAssociationRow[] rows = (CatalogAssociationDto.CatalogAssociationRow[])CurrentCatalogAssociationDto.CatalogAssociation.Select("", "", DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Unchanged);
			//AssociationsFilter.DataSource = rows.Length > 0 ? rows : null; // do not load deleted rows
			//AssociationsFilter.DataBind();

			//AssociationsFilter.SelectedIndex = -1;
			//AssociationsFilter.Text = "";
		}

        /// <summary>
        /// Sets the association items grid data source.
        /// </summary>
        /// <param name="associationId">The association id.</param>
		private void SetAssociationItemsGridDataSource(int associationId)
		{
			CatalogAssociationDto dto = CurrentCatalogAssociationDto;

			CatalogAssociationDto.CatalogAssociationRow row = GetAssociationRow(associationId);
			
			if (row != null)
			{
				CatalogAssociationDto.CatalogEntryAssociationRow[] entryAssociationRows = row.GetCatalogEntryAssociationRows();
				DataTable dt = new DataTable("CatalogEntryAssociation");
				dt.Columns.AddRange(new DataColumn[5] { new DataColumn(_CatalogAssociationIdString, typeof(int)), 
				new DataColumn(_CatalogEntryIdString, typeof(int)),
				new DataColumn("EntryName", typeof(string)),
				new DataColumn(_SortOrderString, typeof(int)),
				new DataColumn(_AssociationTypeIdString, typeof(string))});
				foreach (CatalogAssociationDto.CatalogEntryAssociationRow entryRow in entryAssociationRows)
				{
					DataRow dr = dt.NewRow();
					dr[_CatalogAssociationIdString] = entryRow.CatalogAssociationId;
					dr[_CatalogEntryIdString] = entryRow.CatalogEntryId;
					dr["EntryName"] = GetEntryNameById(entryRow.CatalogEntryId);
					dr[_SortOrderString] = entryRow.SortOrder;
					dr[_AssociationTypeIdString] = entryRow.AssociationTypeId;
					dt.Rows.Add(dr);
				}

				CatalogAssociationDto.AssociationTypeDataTable associationTypeTable = new CatalogAssociationDto.AssociationTypeDataTable();
				List<CatalogAssociationDto.AssociationTypeRow> associationTypeRows = new List<CatalogAssociationDto.AssociationTypeRow>();

				foreach (CatalogAssociationDto.AssociationTypeRow associationTypeRow in dto.AssociationType.Rows)
					associationTypeTable.ImportRow(associationTypeRow);

				DataSet dsSrc = new DataSet();
				dsSrc.Tables.Add(dt);
				dsSrc.Tables.Add(associationTypeTable);

				dsSrc.Relations.Add(dsSrc.Tables["AssociationType"].Columns["AssociationTypeId"], dsSrc.Tables["CatalogEntryAssociation"].Columns["AssociationTypeId"]);

				AssociationItemsGrid.DataSource = dsSrc;
			}
			else
				AssociationItemsGrid.DataSource = null;
			
		}

        /// <summary>
        /// Loads the items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
		private void LoadItems(int iStartIndex, int iNumItems, string sFilter)
        {
			AssociationsFilter.Items.Clear();

			CatalogAssociationDto dto = CurrentCatalogAssociationDto;
			CatalogAssociationDto.CatalogAssociationDataTable dtAssociation = dto.CatalogAssociation;

			foreach (CatalogAssociationDto.CatalogAssociationRow assocRow in dto.CatalogAssociation)
			{
				if ((assocRow.RowState & (DataRowState.Added | DataRowState.Modified | DataRowState.Unchanged)) > 0 &&
					(String.IsNullOrEmpty(sFilter) || assocRow.AssociationName.Contains(sFilter)))
				{
					ComboBoxItem item = new ComboBoxItem(assocRow.AssociationName);
					item.Value = assocRow.CatalogAssociationId.ToString();
					AssociationsFilter.Items.Add(item);
				}
			}

			AssociationsFilter.ItemCount = dto.CatalogAssociation.Count;
        }

        /// <summary>
        /// Loads the association items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
		private void LoadAssociationItems(int iStartIndex, int iNumItems, string sFilter)
		{
			AssociataionItemsFilter.Items.Clear();

            // Changed to return all entries here
            string entryType = String.Empty; //Mediachase.Commerce.Catalog.Objects.EntryType.Product.ToString();

            CatalogSearchParameters pars = new CatalogSearchParameters();
            CatalogSearchOptions options = new CatalogSearchOptions();

            options.RecordsToRetrieve = iNumItems;
            options.Namespace = "Mediachase.Commerce.Catalog";
            options.StartingRecord = iStartIndex;
            options.ReturnTotalCount = true;
            pars.SqlWhereClause = String.Format("[CatalogEntry].Name like '%{0}%'", sFilter);

            // Add catalogs
            CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
            {
                pars.CatalogNames.Add(catalogRow.Name);
            }

            int totalRecords = 0;
            CatalogEntryDto dto = CatalogContext.Current.FindItemsDto(pars, options, ref totalRecords);

			//CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntriesDto(String.Format("%{0}%", sFilter), entryType);
			
			foreach (CatalogEntryDto.CatalogEntryRow entryRow in dto.CatalogEntry)
			{
				ComboBoxItem item = new ComboBoxItem(entryRow.Name);
				item.Value = entryRow.CatalogEntryId.ToString();
                item["icon"] = Page.ResolveClientUrl(String.Format("~/app_themes/Default/images/icons/{0}.gif", entryRow.ClassTypeId));
				AssociataionItemsFilter.Items.Add(item);
			}

            AssociataionItemsFilter.ItemCount = totalRecords;// dto.CatalogEntry.Count;
		}
		#endregion

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
        {
            AssociationsFilter.DataRequested += new ComboBox.DataRequestedEventHandler(AssociationsFilter_DataRequested);
			AssociataionItemsFilter.DataRequested += new ComboBox.DataRequestedEventHandler(AssociataionItemsFilter_DataRequested);
			btnDelete.Click += new EventHandler(btnDelete_Click);
			ItemsPanelTrigger.ValueChanged += new EventHandler(ItemsPanelTrigger_ValueChanged);

			AssociationsFilter.SelectedIndexChanged += new ComboBox.SelectedIndexChangedEventHandler(AssociationsFilter_SelectedIndexChanged);

            base.OnInit(e);
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the AssociationsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void AssociationsFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = AssociationsFilter.SelectedIndex;
		}

        /// <summary>
        /// Handles the ValueChanged event of the ItemsPanelTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ItemsPanelTrigger_ValueChanged(object sender, EventArgs e)
		{
			int id = 0;
			if (Int32.TryParse(ItemsPanelTrigger.Value, out id) && id != 0)
			{
				pnlSelectedAssociation.Visible = true;

				CatalogAssociationDto.CatalogAssociationRow row = GetAssociationRow(id); //SelectedAssociationId);
				if (row != null)
				{
					lblAssociationName.Text = row.AssociationName;
					lblAssociationDescription.Text = row.AssociationDescription;
					BindGrid();
				}
			}
			else
				pnlSelectedAssociation.Visible = false;
		}

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnDelete_Click(object sender, EventArgs e)
		{
			CatalogAssociationDto dto = CurrentCatalogAssociationDto;
			if (SelectedAssociationId != 0 && dto != null)
			{
				CatalogAssociationDto.CatalogAssociationRow row = dto.CatalogAssociation.FindByCatalogAssociationId(SelectedAssociationId);
				if (row != null)
				{
					/*
					// delete asosciation items since there's no cascade delete in this relationship
					DeleteAssociationItems(row);
					 * */
					// delete association
					row.Delete();
				}
			}
			AssociationsFilter.SelectedIndex = -1;
			AssociationsFilter.Text = "";
			BindForm();
			pnlSelectedAssociation.Visible = false;
		}

        /// <summary>
        /// Deletes the association items.
        /// </summary>
        /// <param name="catalogAssociation">The catalog association.</param>
		private void DeleteAssociationItems(CatalogAssociationDto.CatalogAssociationRow catalogAssociation)
		{
			if (catalogAssociation != null)
			{
				CatalogAssociationDto.CatalogEntryAssociationRow[] entryRows = catalogAssociation.GetCatalogEntryAssociationRows();
				foreach (CatalogAssociationDto.CatalogEntryAssociationRow entryRow in entryRows)
					entryRow.Delete();
			}
		}

        /// <summary>
        /// Handles the PreRender event of the AssociationItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void AssociationItemsGrid_PreRender(object sender, EventArgs e)
		{
			//BindGrid();
		}

        /// <summary>
        /// Gets the association row.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        /// <returns></returns>
		private CatalogAssociationDto.CatalogAssociationRow GetAssociationRow(int catalogAssociationId)
		{
			// load selected item
			CatalogAssociationDto dto = CurrentCatalogAssociationDto;
			CatalogAssociationDto.CatalogAssociationRow row = null;
			if (dto != null)
				row = CurrentCatalogAssociationDto.CatalogAssociation.FindByCatalogAssociationId(catalogAssociationId);
			return row;
		}

		#region ComponentArt ComboBoxes' Methods
        /// <summary>
        /// Handles the DataRequested event of the AssociationsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
		void AssociationsFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadItems(args.StartIndex, args.NumItems, args.Filter);  
        }

        /// <summary>
        /// Handles the DataRequested event of the AssociataionItemsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
		void AssociataionItemsFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
		{
			LoadAssociationItems(args.StartIndex, args.NumItems, args.Filter);
		}
		#endregion

		#region AssociationItemsGrid Methods
        /// <summary>
        /// Handles the DeleteCommand event of the AssociationItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void AssociationItemsGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
			int id = Int32.Parse(e.Item[_CatalogAssociationIdString].ToString());
			int entryId = Int32.Parse(e.Item[_CatalogEntryIdString].ToString());

			if (CurrentCatalogAssociationDto != null)
			{
				CatalogAssociationDto.CatalogEntryAssociationRow entryRow = CurrentCatalogAssociationDto.CatalogEntryAssociation.FindByCatalogAssociationIdCatalogEntryId(id, entryId);
				if (entryRow != null)
					entryRow.Delete();
			}
        }

        /// <summary>
        /// Handles the InsertCommand event of the AssociationItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void AssociationItemsGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
			int id = Int32.Parse(e.Item[_CatalogAssociationIdString].ToString());
			int entryId = Int32.Parse(e.Item[_CatalogEntryIdString].ToString());

			if (CurrentCatalogAssociationDto != null)
			{
				CatalogAssociationDto.CatalogEntryAssociationRow entryRow = CurrentCatalogAssociationDto.CatalogEntryAssociation.FindByCatalogAssociationIdCatalogEntryId(id, entryId);
				if (entryRow == null)
				{
					entryRow = CurrentCatalogAssociationDto.CatalogEntryAssociation.NewCatalogEntryAssociationRow();
					entryRow.CatalogAssociationId = id;
					entryRow.CatalogEntryId = entryId;
				}

				entryRow.SortOrder = Int32.Parse(e.Item[_SortOrderString].ToString());
				entryRow.AssociationTypeId = e.Item[_AssociationTypeIdString].ToString();

				if (entryRow.RowState == DataRowState.Detached)
					CurrentCatalogAssociationDto.CatalogEntryAssociation.Rows.Add(entryRow);
			}
		}

        /// <summary>
        /// Handles the OnNeedDataSource event of the AssociationItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void AssociationItemsGrid_OnNeedDataSource(object sender, EventArgs oArgs)
		{
			SetAssociationItemsGridDataSource(SelectedAssociationId);
		}

        /// <summary>
        /// Handles the OnNeedRebind event of the AssociationItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void AssociationItemsGrid_OnNeedRebind(object sender, System.EventArgs oArgs)
		{
			AssociationItemsGrid.DataBind();
		}
		#endregion

        /// <summary>
        /// Gets the entry name by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
		private string GetEntryNameById(int id)
		{
			string name = String.Empty;
			bool found = false; // shows whether name was found

			// try to look up name using items in AssociationItemsFilter dropdown; if item with specified id is not there, get it from the db
			if (AssociataionItemsFilter.ItemCount > 0)
			{
				ComboBoxItem item = AssociataionItemsFilter.Items.FindByValue(id.ToString());
				if (item != null)
				{
					name = item.Text;
					found = true;
				}
			}
			if(!found)
			{
				CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntryDto(id);
				if (dto.CatalogEntry.Rows.Count > 0)
					name = ((CatalogEntryDto.CatalogEntryRow)dto.CatalogEntry.Rows[0]).Name;
			}
			return name;
		}

        /// <summary>
        /// Gets the default association type id.
        /// </summary>
        /// <returns></returns>
		protected string GetDefaultAssociationTypeId()
		{
			CatalogAssociationDto dto = CurrentCatalogAssociationDto;
			if (dto != null && dto.AssociationType.Rows.Count > 0)
				return ((CatalogAssociationDto.AssociationTypeRow)dto.AssociationType.Rows[0]).AssociationTypeId;
			return String.Empty;
		}
		
        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogAssociationDto dto = (CatalogAssociationDto)context[_CatalogAssociationDtoString];

			/*if (CatalogEntryId > 0 && dto == null)
				dto = CatalogContext.Current.GetCatalogAssociationDtoByEntryId(CatalogEntryId);
			else if (CatalogEntryId == 0)
				dto = new CatalogAssociationDto();*/

			dto.CatalogAssociation.Merge(CurrentCatalogAssociationDto.CatalogAssociation, false);
			dto.CatalogEntryAssociation.Merge(CurrentCatalogAssociationDto.CatalogEntryAssociation, false);
		}
        #endregion

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_CatalogAssociationDto = (CatalogAssociationDto)context[_CatalogAssociationDtoString];
			EntryAssociationEditDialog.LoadContext(context);
		}
		#endregion
	}
}