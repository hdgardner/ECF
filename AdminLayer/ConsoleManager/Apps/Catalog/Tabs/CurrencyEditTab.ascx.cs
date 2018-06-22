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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class CurrencyEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _CurrencyIdString = "currencyid";
		private const string _CurrencyDtoContextString = "CurrencyDto";
		private const string _CurrencyIdContextString = "CurrencyId";

		private CurrencyDto _Currency = null;

		/// <summary>
		/// Gets the Currency id.
		/// </summary>
		/// <value>The Currency id.</value>
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
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (CurrencyId > 0)
			{
				bool found = false;
				if (_Currency.Currency.Count > 0)
				{
					CurrencyDto.CurrencyRow row = _Currency.Currency.FindByCurrencyId(CurrencyId);
					if (row != null)
					{
						this.tbCurrencyName.Text = row.Name;
						this.CodeText.Text = row.CurrencyCode;
						if (!row.IsModifiedDateNull())
							this.ModifiedDateLabel.Text = ManagementHelper.GetUserDateTime(row.ModifiedDate).ToString();

						found = true;
					}
				}
				if (!found)
				{
					DisplayErrorMessage(String.Format("Currency with id={0} not found.", CurrencyId));
					return;
				}
			}
		}

		/// <summary>
		/// Checks if entered currency code is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void CurrencyCodeCheck(object sender, ServerValidateEventArgs args)
		{
			// load currency by code
			CurrencyDto dto = CurrencyManager.GetCurrencyByCurrencyCode(args.Value);

			// check if currency with specified code is loaded
			if (dto != null && dto.Currency.Count > 0 &&
				dto.Currency[0].CurrencyId != CurrencyId)
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Currency = (CurrencyDto)context[_CurrencyDtoContextString];
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CurrencyDto dto = (CurrencyDto)context[_CurrencyDtoContextString];

			CurrencyDto.CurrencyRow currencyRow = null;

			if (dto == null)
				// dto must be created in base control that holds tabs
				return;

			currencyRow = dto.Currency.FindByCurrencyId(CurrencyId);

			if (currencyRow == null)
			{
				currencyRow = dto.Currency.NewCurrencyRow();
				currencyRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
			}

			currencyRow.Name = tbCurrencyName.Text;
			currencyRow.CurrencyCode = CodeText.Text;
			currencyRow.ModifiedDate = DateTime.UtcNow;

			if (currencyRow.RowState == DataRowState.Detached)
				dto.Currency.Rows.Add(currencyRow);
		}
		#endregion
	}
}