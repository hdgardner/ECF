using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.Config;
using System.Collections.Generic;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Controls;
using Mediachase.Cms;
using System.Globalization;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Core.Managers;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class CatalogBatchUpdate : CatalogBaseUserControl
    {
		int _MaximumRows = 20;
		int _StartRowIndex = 0;

        /// <summary>
        /// Gets the catalog node code.
        /// </summary>
        public string CatalogNodeCode
        {
            get
            {
                if (ListCatalogs.SelectedIndex > 0)// selected catalogs
                {
                    string[] values = ListCatalogs.SelectedValue.Split(';');

                    if (values.Length > 1)
                        return values[1];

                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the catalog node id.
        /// </summary>
        public int CatalogNodeId
        {
            get
            {
                if (ListCatalogs.SelectedIndex > 0)// selected catalogs
                {
                    string[] values = ListCatalogs.SelectedValue.Split(';');

                    if (values.Length > 2)
                        return Int32.Parse(values[2]);

                }
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
            if (!IsPostBack) 
			{
				MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("CatalogEntryId", "ClassTypeId");

                LoadDataAndDataBind();

                InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                DataBind();
			}

            if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                DataBind();
            }

            //Update column header
            HtmlTableRow headerTRow = MyListView.InnerListViewTableHeader;
            if (headerTRow != null)
            {
                List<Label> labels = ManagementHelper.CollectControls<Label>(headerTRow);
                foreach (Label label in labels)
                {
                    if (ddlFieldList.Items.Count > 0)
                        label.Text = ddlFieldList.SelectedItem == null ? ddlFieldList.Items[0].Text : ddlFieldList.SelectedItem.Text;
                }
            }
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
			BindLanguagesList();
			ManagementHelper.SelectListItem(ListLanguages, CommonSettingsManager.GetDefaultLanguage());

			BindCatalogsList();
            BindMetaclassList();

			StringBuilder script = new StringBuilder("this.disabled = true;\r\n");
			script.AppendFormat("__doPostBack('{0}', '');", btnSearch.UniqueID);
			btnSearch.OnClientClick = script.ToString();
        }

		/// <summary>
		/// Binds the languages.
		/// </summary>
		private void BindLanguagesList()
		{
			ListLanguages.DataValueField = "LangId";
			ListLanguages.DataTextField = "LangName";

			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
				ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
				ListLanguages.Items.Add(item);
			}
		}

        /// <summary>
        /// Binds the meta classes.
        /// </summary>
        private void BindMetaclassList()
        {
            // Bind Meta classes
            MetaClass catalogEntry = MetaClass.Load(CatalogContext.MetaDataContext, "CatalogEntry");
            ddlMetaClassList.Items.Clear();
            if (catalogEntry != null)
            {
                //ddlMetaClassList.Items.Add(new ListItem("", "[all]"));
                MetaClassCollection metaClasses = catalogEntry.ChildClasses;
                foreach (MetaClass metaClass in metaClasses)
                {
                    ddlMetaClassList.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Id.ToString()));
                }
                ddlMetaClassList.DataBind();
            }

            BindFieldList();
        }

        /// <summary>
        /// Binds the meta fields.
        /// </summary>
        private void BindFieldList()
        {
            ddlFieldList.Items.Clear();

            ddlFieldList.Items.Add(new ListItem("Entry : Name", String.Format("{0}${1}", "Name", false)));
            ddlFieldList.Items.Add(new ListItem("Entry : Available from", String.Format("{0}${1}", "StartDate", false)));
            ddlFieldList.Items.Add(new ListItem("Entry : Expires on", String.Format("{0}${1}", "EndDate", false)));
            ddlFieldList.Items.Add(new ListItem("Entry : Display Template", String.Format("{0}${1}", "TemplateName", false)));
            ddlFieldList.Items.Add(new ListItem("Entry : Code", String.Format("{0}${1}", "Code", false)));
            
            if (CatalogNodeId > 0)
                ddlFieldList.Items.Add(new ListItem("Node Relation : SortOrder", String.Format("{0}${1}", "SortOrder", false)));

            ddlFieldList.Items.Add(new ListItem("Entry : Available", String.Format("{0}${1}", "IsActive", false)));

            if (ListTypes.SelectedValue.Equals("Variation") || ListTypes.SelectedValue.Equals("Package"))
            {
                ddlFieldList.Items.Add(new ListItem("Variation : Display Price", String.Format("{0}${1}", "ListPrice", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Tax Category", String.Format("{0}${1}", "TaxCategoryId", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Track Inventory", String.Format("{0}${1}", "TrackInventory", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Merchant", String.Format("{0}${1}", "MerchantId", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Warehouse", String.Format("{0}${1}", "WarehouseId", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Weight", String.Format("{0}${1}", "Weight", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Package", String.Format("{0}${1}", "PackageId", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Min. Quantity", String.Format("{0}${1}", "MinQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Variation : Max. Quantity", String.Format("{0}${1}", "MaxQuantity", false)));

                ddlFieldList.Items.Add(new ListItem("Inventory : In Stock", String.Format("{0}${1}", "InStockQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Reserved", String.Format("{0}${1}", "ReservedQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Reorder Min. Qty", String.Format("{0}${1}", "ReorderMinQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Preorder Qty", String.Format("{0}${1}", "PreorderQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Backorder Qty", String.Format("{0}${1}", "BackorderQuantity", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Allow Backorder", String.Format("{0}${1}", "AllowBackorder", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Allow Preorder", String.Format("{0}${1}", "AllowPreorder", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Inventory Status", String.Format("{0}${1}", "InventoryStatus", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Preorder Avail.", String.Format("{0}${1}", "PreorderAvailabilityDate", false)));
                ddlFieldList.Items.Add(new ListItem("Inventory : Backorder Avail.", String.Format("{0}${1}", "BackorderAvailabilityDate", false)));
            }

            if (ddlMetaClassList.Items.Count > 1 && ddlMetaClassList.SelectedValue != "[all]")
            {
                int SelectedClassId = int.Parse(ddlMetaClassList.SelectedValue);
                MetaClass mclass = MetaClass.Load(CatalogContext.MetaDataContext, SelectedClassId);
                if (mclass != null)
                {
                    foreach (MetaField mfc in mclass.UserMetaFields)
                    {
                        if (mfc.DataType == MetaDataType.ImageFile || mfc.DataType == MetaDataType.File)
                            continue;
                        ddlFieldList.Items.Add(new ListItem(String.Concat("Meta : ", mfc.FriendlyName), String.Format("{0}${1}", mfc.Name, true)));
                    }
                }
            }
            else
                ddlFieldList.Items.Clear();
        }

        /// <summary>
        /// Binds the catalogs list.
        /// </summary>
		private void BindCatalogsList()
		{
			CatalogDto dto = CatalogContext.Current.GetCatalogDto();

            ListCatalogs.Items.Add(new ListItem("all catalogs", "[all]"));
            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                ListItem item = new ListItem(row.Name, row.Name);
                ListCatalogs.Items.Add(item);

                CatalogNodeDto dtoNode = CatalogContext.Current.GetCatalogNodesDto(row.CatalogId);
                DataView dvNode = dtoNode.CatalogNode.DefaultView;
                PopulateSubTree(dvNode, 0, row.Name, "-- ");
            }
            ListCatalogs.Items[0].Selected = true;
			ListCatalogs.DataBind();
		}

        private void PopulateSubTree(DataView dvNode, int parentNodeId, string catalogName, string space)
        {
            dvNode.RowFilter = String.Format("ParentNodeId = {0}", parentNodeId);
            foreach (DataRowView drv in dvNode)
            {
                ListItem itemNode = new ListItem(String.Format("{0}{1}", space, drv["Name"]), String.Format("{0};{1};{2}", catalogName, drv["Code"], drv["CatalogNodeId"]));
                ListCatalogs.Items.Add(itemNode);
                PopulateSubTree(dvNode, (int)drv["CatalogNodeId"], catalogName, space + "-- ");
            }
        }

        public void ddlMetaClassList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSelectedIndex = 0;
            if (!bool.Parse(ddlFieldList.SelectedValue.Split('$')[1]))
                iSelectedIndex = ddlFieldList.SelectedIndex;
            BindFieldList();
            ddlFieldList.SelectedIndex = iSelectedIndex;
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            DataBind();
        }

        public void ddlCategoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSelectedIndex = 0;
            if (!bool.Parse(ddlFieldList.SelectedValue.Split('$')[1]))
                iSelectedIndex = ddlFieldList.SelectedIndex;
            BindFieldList();
            ddlFieldList.SelectedIndex = iSelectedIndex;
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            DataBind();
        }

        public void ListTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSelectedIndex = 0;
            if (!bool.Parse(ddlFieldList.SelectedValue.Split('$')[1]))
                iSelectedIndex = ddlFieldList.SelectedIndex;
            BindFieldList();
            ddlFieldList.SelectedIndex = iSelectedIndex;
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            DataBind();
        }

        public void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
            MyListView.CurrentListView.ItemCreated += new EventHandler<ListViewItemEventArgs>(CurrentListView_ItemCreated);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

            btnSearch.Click += new EventHandler(btnSearch_Click);

            base.OnInit(e);
        }

        void CurrentListView_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                List<TemplateControl> ctrls = ManagementHelper.CollectControls<TemplateControl>(e.Item);
                foreach (Control ctrl in ctrls)
                {
                    IBatchUpdateControl buctrl = ctrl as IBatchUpdateControl;
                    if (buctrl != null)
                    {
                        buctrl.FieldName = ddlFieldList.SelectedValue.Split('$')[0];
                        buctrl.IsMetaField = bool.Parse(ddlFieldList.SelectedValue.Split('$')[1]);
                        buctrl.LanguageCode = ListLanguages.SelectedValue;
                        buctrl.CatalogNodeId = CatalogNodeId;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
            if (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
            {
                // reset start index
                _StartRowIndex = 0;

                InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                DataBind();
                MyListView.MainUpdatePanel.Update();
            }
		}

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
		void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
		{
			AdminView view = MyListView.CurrentListView.GetAdminView();
			foreach (ViewColumn column in view.Columns)
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// reset start index
					_StartRowIndex = 0;

					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					InitDataSource(_StartRowIndex, _MaximumRows, true, sortExpression);
                    DataBind();
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
            MyListView.DataSourceID = CatalogSearchDataSource1.ID;

			// fill in search parameters
			CatalogSearchDataSource1.Options.Namespace = String.Empty;

			// language filter
			CatalogSearchDataSource1.Parameters.Language = ListLanguages.SelectedValue;

			// catalog filter
            CatalogSearchDataSource1.Parameters.CatalogNames.Clear();
            CatalogSearchDataSource1.Parameters.CatalogNodes.Clear();

            if (ListCatalogs.SelectedValue == "[all]")
            {
                foreach (ListItem item in ListCatalogs.Items)
                {
                    if (item.Value == "[all]")
                        continue;

                    string[] values = item.Value.Split(';');
                    if(values.Length == 1)
                        CatalogSearchDataSource1.Parameters.CatalogNames.Add(item.Value);
                }
            }
            else if (ListCatalogs.SelectedIndex > 0)// selected catalogs
			{
                string[] values = ListCatalogs.SelectedValue.Split(';');
                CatalogSearchDataSource1.Parameters.CatalogNames.Add(values[0]);

                if (values.Length > 1)
                    CatalogSearchDataSource1.Parameters.CatalogNodes.Add(values[1]);

			}

            StringBuilder sqlWhereClause = new StringBuilder("1=1");

            if (String.Compare(ListTypes.SelectedValue, "[all]", false) != 0)
                sqlWhereClause.AppendFormat(" AND ClassTypeId LIKE '{0}'", ListTypes.SelectedValue);
            
            if(String.Compare(ddlMetaClassList.SelectedValue, "[all]", false) != 0)
                sqlWhereClause.AppendFormat(" AND MetaClassId LIKE {0}", ddlMetaClassList.SelectedValue);

            if (String.Compare(sqlWhereClause.ToString(), "1=1", false) != 0)
                CatalogSearchDataSource1.Parameters.SqlWhereClause = sqlWhereClause.ToString();

			CatalogSearchDataSource1.Parameters.OrderByClause = orderByClause;
            CatalogSearchDataSource1.Parameters.FreeTextSearchPhrase = tbKeywords.Text;

			CatalogSearchDataSource1.Options.RecordsToRetrieve = recordsCount;
			CatalogSearchDataSource1.Options.StartingRecord = startRowIndex;
			CatalogSearchDataSource1.Options.ReturnTotalCount = true;
		}

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
			_MaximumRows = e.MaximumRows;
			_StartRowIndex = e.StartRowIndex;
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
            DataBind();
		}

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnSearch_Click(object sender, EventArgs e)
        {
			_StartRowIndex = 0;
			InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
			MyListView.ResetPageNumber();
			DataBind();
			MyListView.MainUpdatePanel.Update();

			btnSearch.Enabled = true;
			upSearchButton.Update();
        }
    }
}