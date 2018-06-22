using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntryPricingEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
    {
        private const string _CatalogEntryDtoString = "CatalogEntryDto";
		private const string _SalePriceIdString = "SalePriceId";
		private const string _CatalogCurrencyString = "CatalogCurrency";
        
        private CatalogEntryDto _CatalogEntryDto = null;
		private CatalogDto _CatalogDto = null;

		List<GridItem> _removedItems = new List<GridItem>();

		#region Public Properties
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
        /// Gets the type of the entry.
        /// </summary>
        /// <value>The type of the entry.</value>
        public string EntryType
        {
            get
            {
                return Request.QueryString["type"];
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
			MaskedEditExtender1.CultureName = ManagementContext.Current.ConsoleUICulture.Name;
			bool catalogLoaded = false;
			if (_CatalogDto != null)
			{
				if (_CatalogDto.Catalog.Count > 0)
				{
					DisplayPriceCurrency.Text = _CatalogDto.Catalog[0].DefaultCurrency;
					catalogLoaded = true;
				}
			}
			if (!catalogLoaded)
				DisplayPriceCurrency.Text = String.Empty;

			if (!Page.IsPostBack)
				BindForm();

			SalePricesGrid.NeedRebind += new Grid.NeedRebindEventHandler(SalePricesGrid_NeedRebind);
			SalePricesGrid.NeedDataSource += new Grid.NeedDataSourceEventHandler(SalePricesGrid_NeedDataSource);
			SalePricesGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(SalePricesGrid_DeleteCommand);
			SalePricesGrid.PreRender += new EventHandler(SalePricesGrid_PreRender);
        }

		#region SalePricesGrid event handlers
        /// <summary>
        /// Handles the NeedDataSource event of the SalePricesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void SalePricesGrid_NeedDataSource(object sender, EventArgs e)
		{
			SalePricesGrid.DataSource = GetSalePricesDataSource();
		}

        /// <summary>
        /// Handles the NeedRebind event of the SalePricesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void SalePricesGrid_NeedRebind(object sender, EventArgs e)
		{
			SalePricesGrid.DataBind();
		}

        /// <summary>
        /// Handles the PreRender event of the SalePricesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void SalePricesGrid_PreRender(object sender, EventArgs e)
		{
			// Postback happens so the grid will be completely updated, make sure to save all the changes
			if (this.IsPostBack)
				ProcessSalePricesTableEvents(_CatalogEntryDto);
		}

        /// <summary>
        /// Handles the DeleteCommand event of the SalePricesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void SalePricesGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			_removedItems.Add(e.Item);
		}
		#endregion

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            BindLists();
            if (CatalogEntryId > 0)
            {
                CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto(_CatalogEntryDto.CatalogEntry[0].CatalogId);
                if (_CatalogEntryDto.CatalogEntry.Count > 0)
                {
                    if (_CatalogEntryDto.Variation.Count != 0)
                    {
                        CatalogEntryDto.VariationRow variationRow = _CatalogEntryDto.Variation[0];

						ListPrice.Text = variationRow.ListPrice.ToString("#0.00"); //"N2");
                        DisplayPriceCurrency.Text = catalogDto.Catalog[0].DefaultCurrency;
                        MinQty.Text = variationRow.MinQuantity.ToString();
                        MaxQty.Text = variationRow.MaxQuantity.ToString();
                        Weight.Text = variationRow.Weight.ToString();

                        if (!variationRow.IsMerchantIdNull())
                            ManagementHelper.SelectListItem(MerchantList, variationRow.MerchantId);

                        ManagementHelper.SelectListItem(PackageList, variationRow.PackageId);
                        ManagementHelper.SelectListItem(WarehouseList, variationRow.WarehouseId);
						ManagementHelper.SelectListItem(TaxList, variationRow.TaxCategoryId);

                        TrackInventory.IsSelected = variationRow.TrackInventory;
                    }

                    if (_CatalogEntryDto.Inventory.Count != 0)
                    {
                        CatalogEntryDto.InventoryRow inventoryRow = _CatalogEntryDto.Inventory[0];

                        ManagementHelper.SelectListItem(InventoryStatusList, inventoryRow.InventoryStatus);
                        InStockQty.Text = inventoryRow.InStockQuantity.ToString();
                        ReservedQty.Text = inventoryRow.ReservedQuantity.ToString();
                        ReorderMinQty.Text = inventoryRow.ReorderMinQuantity.ToString();
                        AllowPreorder.IsSelected = inventoryRow.AllowPreorder;
                        PreorderQty.Text = inventoryRow.PreorderQuantity.ToString();
                        PreorderAvail.Value = ManagementHelper.GetUserDateTime(inventoryRow.PreorderAvailabilityDate);
                        AllowBackorder.IsSelected = inventoryRow.AllowBackorder;
                        BackorderQty.Text = inventoryRow.BackorderQuantity.ToString();
                        BackorderAvail.Value = ManagementHelper.GetUserDateTime(inventoryRow.BackorderAvailabilityDate);
                    }

					// Bind SalePrices
					GridHelper.BindGrid(SalePricesGrid, "Catalog", "EntrySalePrice");
					BindSalePricesGrid();
                }
            }
            else // set defaults
            {
                InStockQty.Text = "10";
                ReservedQty.Text = "2";
                ReorderMinQty.Text = "1";
                AllowPreorder.IsSelected = false;
                PreorderQty.Text = "10";
                PreorderAvail.Value = ManagementHelper.GetUserDateTime(DateTime.Now.ToUniversalTime());
                AllowBackorder.IsSelected = false;
                BackorderQty.Text = "10";
                BackorderAvail.Value = ManagementHelper.GetUserDateTime(DateTime.Now.ToUniversalTime());
                MinQty.Text = "1";
                MaxQty.Text = "100";
                Weight.Text = Decimal.Parse("1.0", System.Globalization.CultureInfo.InvariantCulture).ToString();

				GridHelper.BindGrid(SalePricesGrid, "Catalog", "EntrySalePrice");
            }
        }

        /// <summary>
        /// Binds the lists.
        /// </summary>
		private void BindLists()
		{
			// bind shipment packages
			if (PackageList.Items.Count <= 1)
			{
				ShippingMethodDto shippingDto = ShippingManager.GetShippingPackages();
				if (shippingDto.Package != null)
					foreach (ShippingMethodDto.PackageRow row in shippingDto.Package.Rows)
						PackageList.Items.Add(new ListItem(row.Name, row.PackageId.ToString()));
				PackageList.DataBind();
			}

			// bind warehouses
			if (WarehouseList.Items.Count <= 1)
			{
				WarehouseDto dto = WarehouseManager.GetWarehouseDto();
				if (dto.Warehouse != null)
				{
					foreach (WarehouseDto.WarehouseRow row in dto.Warehouse.Rows)
					{
						WarehouseList.Items.Add(new ListItem(row.Name, row.WarehouseId.ToString()));
					}
				}

				WarehouseList.DataBind();
			}

			// bind merchants
			if (MerchantList.Items.Count <= 1)
			{
				CatalogEntryDto merchants = CatalogContext.Current.GetMerchantsDto();
				if (merchants.Merchant != null)
					foreach (CatalogEntryDto.MerchantRow row in merchants.Merchant.Rows)
						MerchantList.Items.Add(new ListItem(row.Name, row.MerchantId.ToString()));
				MerchantList.DataBind();
			}

			// bind tax categories
			if (TaxList.Items.Count <= 1)
			{
				CatalogTaxDto taxes = CatalogTaxManager.GetTaxCategories();
				if (taxes.TaxCategory != null)
					foreach (CatalogTaxDto.TaxCategoryRow row in taxes.TaxCategory.Rows)
						TaxList.Items.Add(new ListItem(row.Name, row.TaxCategoryId.ToString()));
				TaxList.DataBind();
			}
		}

        /// <summary>
        /// Binds the sale prices grid.
        /// </summary>
		private void BindSalePricesGrid()
		{
			object dataSource = GetSalePricesDataSource();
			if (dataSource != null)
			{
				SalePricesGrid.DataSource = dataSource;
				SalePricesGrid.DataBind();
			}
		}

        /// <summary>
        /// Gets the sale prices data source.
        /// </summary>
        /// <returns></returns>
		private object GetSalePricesDataSource()
		{
			if (_CatalogEntryDto != null && _CatalogEntryDto.SalePrice.Count > 0)
				return _CatalogEntryDto.SalePrice;
			else
				return null;
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();
            BindForm();
        }

        /// <summary>
        /// Processes the sale prices table events.
        /// </summary>
        /// <param name="dto">The dto.</param>
		private void ProcessSalePricesTableEvents(CatalogEntryDto dto)
		{
			if (dto != null)
				foreach (GridItem item in _removedItems)
				{
					int id = 0;
					if (item[_SalePriceIdString] != null && Int32.TryParse(item[_SalePriceIdString].ToString(), out id))
					{
						// find the existing one
						CatalogEntryDto.SalePriceRow row = dto.SalePrice.FindBySalePriceId(id);
						if (row != null && row.RowState != DataRowState.Deleted)
							row.Delete();
					}
				}

			_removedItems.Clear();
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            CatalogEntryDto dto = (CatalogEntryDto)context[_CatalogEntryDtoString];

			ProcessSalePricesTableEvents(_CatalogEntryDto);

            CatalogEntryDto.VariationRow variationRow = null;
            CatalogEntryDto.InventoryRow inventoryRow = null;

            CatalogDto catalogDto = null;

            // get catalog current entry belongs to
            if (CatalogEntryId > 0)
                catalogDto = CatalogContext.Current.GetCatalogDto(dto.CatalogEntry[0].CatalogId);
            else
                catalogDto = CatalogContext.Current.GetCatalogDto(ParentCatalogId);

            if (dto.Variation == null || dto.Variation.Count == 0)
                variationRow = dto.Variation.NewVariationRow();
            else
                variationRow = dto.Variation[0];

            if (dto.Inventory == null || dto.Inventory.Count == 0)
            {
                inventoryRow = dto.Inventory.NewInventoryRow();
                inventoryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            }
            else
                inventoryRow = dto.Inventory[0];

            // Update Variation
            variationRow.ListPrice = Decimal.Parse(ListPrice.Text);
            variationRow.MinQuantity = Decimal.Parse(MinQty.Text);
            variationRow.MaxQuantity = Decimal.Parse(MaxQty.Text);
            variationRow.CatalogEntryId = dto.CatalogEntry[0].CatalogEntryId;
            variationRow.TaxCategoryId = Int32.Parse(TaxList.SelectedValue);

            if(!String.IsNullOrEmpty(MerchantList.SelectedValue))
                variationRow.MerchantId = new Guid(MerchantList.SelectedValue);

            variationRow.Weight = Double.Parse(Weight.Text);

            if(!String.IsNullOrEmpty(PackageList.SelectedValue))
                variationRow.PackageId = Int32.Parse(PackageList.SelectedValue);

            if(!String.IsNullOrEmpty(WarehouseList.SelectedValue))
                variationRow.WarehouseId = Int32.Parse(WarehouseList.SelectedValue);

            variationRow.TrackInventory = TrackInventory.IsSelected;

            // Update Inventory
            inventoryRow.InventoryStatus = Int32.Parse(InventoryStatusList.SelectedValue);
            inventoryRow.InStockQuantity = Decimal.Parse(InStockQty.Text);
            inventoryRow.ReservedQuantity = Decimal.Parse(ReservedQty.Text);
            inventoryRow.ReorderMinQuantity = Decimal.Parse(ReorderMinQty.Text);
            inventoryRow.AllowPreorder = AllowPreorder.IsSelected;
            inventoryRow.PreorderQuantity = Decimal.Parse(PreorderQty.Text);
			inventoryRow.PreorderAvailabilityDate = PreorderAvail.Value != DateTime.MinValue ? PreorderAvail.Value.ToUniversalTime() : DateTime.UtcNow;
            inventoryRow.AllowBackorder = AllowBackorder.IsSelected;
            inventoryRow.BackorderQuantity = Decimal.Parse(BackorderQty.Text);
			inventoryRow.BackorderAvailabilityDate = BackorderAvail.Value != DateTime.MinValue ? BackorderAvail.Value.ToUniversalTime() : DateTime.UtcNow;
            inventoryRow.SkuId = dto.CatalogEntry[0].Code;

			// Update SalePrice
			dto.SalePrice.Merge(_CatalogEntryDto.SalePrice, false);
			foreach (CatalogEntryDto.SalePriceRow row in dto.SalePrice.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				if (row.RowState == DataRowState.Added || String.Compare(dto.CatalogEntry[0].Code, row.ItemCode, true) != 0)
				{
					row.ItemCode = dto.CatalogEntry[0].Code;
					if (String.IsNullOrEmpty(row.Currency))
						row.Currency = catalogDto.Catalog[0].DefaultCurrency;
				}
			}

            // Make sure to attach new rows
            if (variationRow.RowState == DataRowState.Detached)
                dto.Variation.Rows.Add(variationRow);

            if (inventoryRow.RowState == DataRowState.Detached)
                dto.Inventory.Rows.Add(inventoryRow);
        }
        #endregion

        #region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _CatalogEntryDto = (CatalogEntryDto)context[_CatalogEntryDtoString];

			if (ParentCatalogId > 0)
				_CatalogDto = CatalogManager.GetCatalogDto(ParentCatalogId, new CatalogResponseGroup(CatalogResponseGroup.ResponseGroup.CatalogInfo));

			if (_CatalogDto != null && _CatalogDto.Catalog.Count > 0)
				context[_CatalogCurrencyString] = _CatalogDto.Catalog[0].DefaultCurrency;

			SalePriceEditDialog.LoadContext(context);
        }
        #endregion
    }
}