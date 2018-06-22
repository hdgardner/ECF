using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Catalog
{
	public partial class CurrencyEdit : CatalogBaseUserControl
	{
		private const string _CurrencyDtoEditSessionKey = "ECF.CurrencyDto.Edit";
		private const string _CurrencyIdString = "currencyid";
		private const string _CurrencyDtoContextString = "CurrencyDto";
		private const string _CurrencyIdContextString = "CurrencyId";

		/// <summary>
		/// Gets the currency id.
		/// </summary>
		/// <value>The currency id.</value>
		public int CurrencyId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_CurrencyIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadContext();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
		}

		/// <summary>
		/// Loads the fresh.
		/// </summary>
		/// <returns></returns>
		private CurrencyDto LoadFresh()
		{
			CurrencyDto currency = CurrencyManager.GetCurrencyDto();

			if (currency == null)
				currency = new CurrencyDto();

			currency.EnforceConstraints = false;

			// persist in session
			Session[_CurrencyDtoEditSessionKey] = currency;

			return currency;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			CurrencyDto currency = null;
			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
				currency = LoadFresh();
			else // load from session
			{
				currency = (CurrencyDto)Session[_CurrencyDtoEditSessionKey];

				if (currency == null)
					currency = LoadFresh();
			}

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_CurrencyDtoContextString, currency);
			dic.Add(_CurrencyIdContextString, CurrencyId);

			// Call tabs load context
			ViewControl.LoadContext(dic);
		}

		/// <summary>
		/// Handles the SaveChanges event of the EditSaveControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
		{
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			try
			{
				CurrencyDto currency = (CurrencyDto)Session[_CurrencyDtoEditSessionKey];

				if (CurrencyId > 0 && currency == null)
					currency = CurrencyManager.GetCurrencyDto();
				else if (CurrencyId == 0)
					currency = LoadFresh();

				IDictionary context = new ListDictionary();
				context.Add(_CurrencyDtoContextString, currency);
				context.Add(_CurrencyIdContextString, CurrencyId);

				ViewControl.SaveChanges(context);

				if (currency.HasChanges())
				{
					CurrencyDto.CurrencyRow currentCurrencyRow = null;

					// search for the row that has just been added
					CurrencyDto.CurrencyRow[] tempCurrencyRows = (CurrencyDto.CurrencyRow[])currency.Currency.Select("", "", DataViewRowState.Added);
					if (tempCurrencyRows != null && tempCurrencyRows.Length > 0)
						currentCurrencyRow = tempCurrencyRows[0];

					if (currentCurrencyRow != null)
					{
						int cId = currentCurrencyRow.CurrencyId;

						// if new currency has just been created, update currencyid for CurrencyRate rows
						CurrencyDto.CurrencyRateRow[] rateRows = (CurrencyDto.CurrencyRateRow[])currency.CurrencyRate.Select("", "", DataViewRowState.Added | DataViewRowState.ModifiedCurrent);
						if (rateRows != null && rateRows.Length > 0)
							foreach (CurrencyDto.CurrencyRateRow row in rateRows)
								row.FromCurrencyId = cId;
					}

					currency.EnforceConstraints = true;

					// commit changes to the db
					CurrencyManager.SaveCurrency(currency);
				}
			}
			catch (Exception ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
			finally
			{
				// we don't need to store Dto in session any more
				Session.Remove(_CurrencyDtoEditSessionKey);
			}
		}
	}
}