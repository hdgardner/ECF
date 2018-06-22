using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class CurrencyRateEditPopup : CatalogBaseUserControl, IAdminContextControl
	{
		private const string _CurrencyIdString = "currencyid";
		private const string _CurrencyDtoContextString = "CurrencyDto";
		private const string _CurrencyIdContextString = "CurrencyId";

		private CurrencyDto _Currency = null;

		int _currencyRateId = 0;
		int _fromCurrencyId = 0;

		/// <summary>
		/// Gets or sets the currency rate id.
		/// </summary>
		/// <value>The currency rate id.</value>
		private int CurrencyRateId
		{
			get
			{
				if (_currencyRateId == 0)
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
						Int32.TryParse(this.DialogTrigger.Value, out _currencyRateId);
				}

				return _currencyRateId;
			}
			set
			{
				_currencyRateId = value;
			}
		}

		private int CurrencyId
		{
			get
			{
				return _fromCurrencyId;
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
			CurrencyDto.CurrencyRateRow rateRow = null;

			if (CurrencyRateId != 0) // find existing
			{
				CurrencyDto.CurrencyRateRow[] rateRows = (CurrencyDto.CurrencyRateRow[])_Currency.CurrencyRate.Select(String.Format("CurrencyRateId={0}", CurrencyRateId));
				if (rateRows != null && rateRows.Length > 0)
					rateRow = rateRows[0];
			}

			// if not found, create new
			if (rateRow == null)
			{
				rateRow = _Currency.CurrencyRate.NewCurrencyRateRow();
				rateRow.FromCurrencyId = CurrencyId;
				rateRow.ToCurrencyId = Int32.Parse(SelectedCurrencyField.Value);
			}

			// update lineItem fields
			if (rateRow != null)
			{
				rateRow.EndOfDayRate = Double.Parse(tbRate.Text);
				rateRow.AverageRate = Double.Parse(tbAverageRate.Text);
				rateRow.CurrencyRateDate = CurrencyRateDate.Value; //.ToUniversalTime(); - don't need to convert to Utc here since we need only Date, without time
				rateRow.ModifiedDate = DateTime.UtcNow;
			}

			if (rateRow.RowState == DataRowState.Detached)
				_Currency.CurrencyRate.AddCurrencyRateRow(rateRow);

			ScriptManager.RegisterStartupScript(SaveChangesButton, typeof(CurrencyRateEditPopup), "DialogClose", "CurrencyRateDialog_CloseDialog();", true);
		}

		/// <summary>
		/// Binds the currencies list.
		/// </summary>
		private void BindCurrenciesList()
		{
			CurrencyDto dto = CurrencyManager.GetCurrencyDto();

			if (CurrencyId > 0)
			{
				// remove row with current currency
				CurrencyDto.CurrencyRow[] rows = (CurrencyDto.CurrencyRow[])dto.Currency.Select(String.Format("CurrencyId={0}", CurrencyId));
				if (rows != null && rows.Length > 0)
					dto.Currency.Rows.Remove(rows[0]);
			}

			ToCurrenciesList.DataSource = dto.Currency.DefaultView;
			ToCurrenciesList.DataBind();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		/// <param name="reset">if set to <c>true</c> [reset].</param>
		private void BindForm(bool reset)
		{
			if (CurrencyRateId == 0)
			{
				ToCurrencyLabel.Visible = false;
				ToCurrenciesList.Visible = true;
				ToCurrencyRequiredValidator.Enabled = true;

				BindCurrenciesList();

				if (String.IsNullOrEmpty(SelectedCurrencyField.Value) || SelectedCurrencyField.Value == "0")
					SelectedCurrencyField.Value = ToCurrenciesList.SelectedValue;
			}
			else
			{
				ToCurrencyLabel.Visible = true;
				ToCurrenciesList.Visible = false;
				ToCurrencyRequiredValidator.Enabled = false;
			}

			CurrencyDto.CurrencyRateRow selectedRow = null;
			if (CurrencyRateId != 0)
				selectedRow = _Currency.CurrencyRate.FindByCurrencyRateId(CurrencyRateId);

			if (selectedRow != null)
			{
				if (reset)
					SetFormFieldsValues(selectedRow.FromCurrencyId,
							selectedRow.EndOfDayRate,
							selectedRow.AverageRate,
							selectedRow.CurrencyRateDate,
							ManagementHelper.GetUserDateTime(selectedRow.ModifiedDate),
							selectedRow.ToCurrencyId);

				// set ToCurrencyLabel text
				string toCurrencyLabelText = String.Empty;

				CurrencyDto.CurrencyRow toCurrencyRow = _Currency.Currency.FindByCurrencyId(selectedRow.ToCurrencyId);
				if (toCurrencyRow != null)
					toCurrencyLabelText = MakeCurrencyText(toCurrencyRow);
				ToCurrencyLabel.Text = toCurrencyLabelText;
			}
			else if (reset)
			{
				SetFormFieldsValues(CurrencyId, 0, 0, DateTime.Now, DateTime.Now, 0);
			}
		}

		/// <summary>
		/// Sets the form fields values.
		/// </summary>
		private void SetFormFieldsValues(int fromCurrencyId, Double rate, Double avgRate, DateTime rateDate, DateTime modificationDate, int selectedCurrencyId)
		{
			string formatString = "#0.00";

			if (fromCurrencyId > 0 && _Currency!=null)
			{
				CurrencyDto.CurrencyRow currencyRow = _Currency.Currency.FindByCurrencyId(fromCurrencyId);
				if (currencyRow != null)
					FromCurrencyLabel.Text = MakeCurrencyText(currencyRow);
				else
					FromCurrencyLabel.Text = String.Empty;
			}

			tbRate.Text = rate.ToString(formatString);
			tbAverageRate.Text = avgRate.ToString(formatString);
			CurrencyRateDate.Value = rateDate;
			ModificationDate.Text = modificationDate.ToString();
			SelectedCurrencyField.Value = selectedCurrencyId.ToString();
		}

		private string MakeCurrencyText(CurrencyDto.CurrencyRow currency)
		{
			return String.Format("{0} ({1})", currency.Name, currency.CurrencyCode);
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Currency = (CurrencyDto)context[_CurrencyDtoContextString];
			_fromCurrencyId = (int)context[_CurrencyIdContextString];
		}
		#endregion
	}
}