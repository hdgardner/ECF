using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using System.Configuration;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class SalePriceEditPopup : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _CatalogEntryDtoString = "CatalogEntryDto";
		private const string _CatalogCurrencyString = "CatalogCurrency";

		int _SalePriceId = 0;
		CatalogEntryDto _CatalogEntryDto = null;
		string _CatalogCurrency = null;

        /// <summary>
        /// Gets or sets the sale price id.
        /// </summary>
        /// <value>The sale price id.</value>
		public int SalePriceId
		{
			get
			{
				if (_SalePriceId == 0)
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
					{
						int index = this.DialogTrigger.Value.IndexOf('|');
						string idString = this.DialogTrigger.Value;

						if (index > 0)
							// cut off CatalogEntryId
							idString = this.DialogTrigger.Value.Substring(0, index);

						_SalePriceId = Int32.Parse(idString);
					}
				}

				return _SalePriceId;
			}
			set
			{
				_SalePriceId = value;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindForm(false);
			DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

        /// <summary>
        /// Handles the ValueChanged event of the DialogTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void DialogTrigger_ValueChanged(object sender, EventArgs e)
		{
			BindForm(true);
		}

        /// <summary>
        /// Handles the Click event of the SaveChangesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveChangesButton_Click(object sender, EventArgs e)
		{
			CatalogEntryDto.SalePriceRow row = null;

			if (SalePriceId != 0) // find existing
				row = _CatalogEntryDto.SalePrice.FindBySalePriceId(SalePriceId);

			// if not found, create new
			if (row == null)
			{
				row = _CatalogEntryDto.SalePrice.NewSalePriceRow();
				row.Currency = "";
				//row.ItemCode = "";
			}

			// update SalePrice fields
			if (row != null)
			{
				row.SaleCode = SaleCode.Text;

				decimal price = Decimal.Parse(UnitPrice.Text);
				row.UnitPrice = price;

				ListItem selectedItem = SaleTypeFilter.Items.FindByValue(SelectedSaleTypeField.Value);
				if (selectedItem != null)
				{
					row.SaleType = Int32.Parse(selectedItem.Value);
					ManagementHelper.SelectListItem(SaleTypeFilter, row.SaleType);
				}

				selectedItem = CurrencyFilter.Items.FindByValue(SelectedCurrencyField.Value);
				if (selectedItem != null)
				{
					row.Currency = selectedItem.Value;
					ManagementHelper.SelectListItem(CurrencyFilter, row.Currency);
				}

				row.MinQuantity = Decimal.Parse(MinQuantity.Text);
				row.StartDate = StartDate.Value.ToUniversalTime();
				row.EndDate = EndDate.Value.ToUniversalTime();

				if (row.RowState == DataRowState.Detached)
					_CatalogEntryDto.SalePrice.Rows.Add(row);
			}

			ScriptManager.RegisterStartupScript(MetaDataTab, typeof(SalePriceEditPopup), "DialogClose", "SalePriceEdit_CloseDialog();", true);
		}

        /// <summary>
        /// Binds the sale types list.
        /// </summary>
		private void BindSaleTypesList()
		{
			SaleTypeFilter.Items.Clear();

            foreach (SalePriceTypeDefinition element in CatalogConfiguration.Instance.SalePriceTypes)
            {
                ListItem li = new ListItem(UtilHelper.GetResFileString(element.Description), element.Value.ToString());
                SaleTypeFilter.Items.Add(li);
            }

            /*
			foreach (int key in SaleType.SaleTypes.Keys)
			{
				ListItem li = new ListItem(SaleType.SaleTypes[key], key.ToString());
				SaleTypeFilter.Items.Add(li);
			}
             * */
		}

        /// <summary>
        /// Binds the currencies list.
        /// </summary>
		private void BindCurrenciesList()
		{
			CurrencyDto dto = CatalogContext.Current.GetCurrencyDto();
			CurrencyFilter.DataSource = dto.Currency;
			CurrencyFilter.DataBind();
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
		private void BindForm(bool reset)
		{
			if (SaleTypeFilter.Items.Count == 0)
				BindSaleTypesList();
			if (CurrencyFilter.Items.Count == 0)
			{
				BindCurrenciesList();
				// select default currency
				ManagementHelper.SelectListItemIgnoreCase(CurrencyFilter, _CatalogCurrency);
			}

			if (String.IsNullOrEmpty(SelectedSaleTypeField.Value))
				SelectedSaleTypeField.Value = SaleTypeFilter.SelectedValue;

			if (String.IsNullOrEmpty(SelectedCurrencyField.Value))
				SelectedCurrencyField.Value = CurrencyFilter.SelectedValue;

			CatalogEntryDto.SalePriceRow selectedSalePriceRow = null;
			if (SalePriceId != 0)
				selectedSalePriceRow = _CatalogEntryDto.SalePrice.FindBySalePriceId(SalePriceId);

			if (selectedSalePriceRow != null)
			{
				if (reset)
					SetFormFieldsValues(selectedSalePriceRow.SaleCode,
							selectedSalePriceRow.UnitPrice,
							selectedSalePriceRow.MinQuantity,
							selectedSalePriceRow.StartDate,
							selectedSalePriceRow.EndDate,
							selectedSalePriceRow.SaleType,
							selectedSalePriceRow.Currency);

				ManagementHelper.SelectListItem2(SaleTypeFilter, selectedSalePriceRow.SaleType);
				ManagementHelper.SelectListItem2(CurrencyFilter, selectedSalePriceRow.Currency);
			}
			else if (reset)
			{
				if (_CatalogEntryDto != null)
				{
					SetFormFieldsValues("",
						0m,
						0m,
						DateTime.UtcNow,
						DateTime.UtcNow.AddYears(1),
						-1, _CatalogCurrency);
				}
				else
					SetFormFieldsValues("", 0m, 0m, DateTime.UtcNow, DateTime.UtcNow.AddYears(1), -1, _CatalogCurrency);

				ManagementHelper.SelectListItem2(CurrencyFilter, _CatalogCurrency);
			}
		}

        /// <summary>
        /// Sets the form fields values.
        /// </summary>
        /// <param name="saleCode">The sale code.</param>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="selectedSaleTypeId">The selected sale type id.</param>
        /// <param name="selectedCurrency">The selected currency.</param>
		private void SetFormFieldsValues(string saleCode, decimal unitPrice, decimal quantity, DateTime startDate, DateTime endDate, int selectedSaleTypeId, string selectedCurrency)
		{
			string formatString = "#0.00";

			SaleCode.Text = saleCode;
			UnitPrice.Text = unitPrice.ToString(formatString);
			MinQuantity.Text = quantity.ToString(formatString);
			StartDate.Value = ManagementHelper.GetUserDateTime(startDate);
			EndDate.Value = ManagementHelper.GetUserDateTime(endDate);
			if (selectedSaleTypeId >= 0)
				SelectedSaleTypeField.Value = selectedSaleTypeId.ToString();
			//if (!String.IsNullOrEmpty(selectedCurrency))
				SelectedCurrencyField.Value = selectedCurrency;
		}

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CatalogEntryDto dto = (CatalogEntryDto)context[_CatalogEntryDtoString];
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

			_CatalogCurrency = (string)context[_CatalogCurrencyString];
			if (String.IsNullOrEmpty(_CatalogCurrency))
				_CatalogCurrency = CommonSettingsManager.GetDefaultCurrency();
			_CatalogCurrency = _CatalogCurrency.ToUpper();
		}
		#endregion
	}
}