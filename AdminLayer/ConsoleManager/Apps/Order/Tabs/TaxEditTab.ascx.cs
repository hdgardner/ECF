using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	/// <summary>
	///		Summary description for TaxEditTab.
	/// </summary>
	public partial class TaxEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _TaxIdString = "taxid";
		private const string _TaxDtoString = "TaxDto";

		private TaxDto _TaxDto = null;

		/// <summary>
		/// Gets the Tax id.
		/// </summary>
		/// <value>The Tax id.</value>
		public int TaxId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_TaxIdString);
			}
		}

		private CultureInfo GetCurrentCulture()
		{
			return ManagementContext.Current.ConsoleUICulture;
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
			CultureInfo ci = GetCurrentCulture();

			if (TaxId > 0)
			{
				if (_TaxDto.Tax.Count > 0)
				{
					this.tbName.Text = _TaxDto.Tax[0].Name;
					this.tbSortOrder.Text = _TaxDto.Tax[0].SortOrder.ToString();

					ManagementHelper.SelectListItem2(TaxTypeList, _TaxDto.Tax[0].TaxType.ToString());
				}
				else
				{
					DisplayErrorMessage(String.Format("Tax with id={0} not found.", TaxId));
					return;
				}
			}
			else
			{
				this.tbSortOrder.Text = "0";
			}

			BindTaxLanguagesList();
		}

		/// <summary>
		/// Binds the languages repeater.
		/// </summary>
		/// <returns></returns>
		private void BindTaxLanguagesList()
		{
			DataTable sourceTable = new DataTable();
			sourceTable.Columns.AddRange(new DataColumn[] { 
				new DataColumn("LanguageCode", typeof(string)),
				new DataColumn("FriendlyName", typeof(string)),
				new DataColumn("DisplayName", typeof(string))
					});

			DataTable dtLanguages = Language.GetAllLanguagesDT();

			if (TaxId > 0 && _TaxDto.Tax.Count > 0)
			{
				foreach (DataRow row in dtLanguages.Rows)
				{
					string langCode = row["LangName"].ToString();

					TaxDto.TaxLanguageRow taxLanguageRow = null;

					// check if record for the current language already exists in TaxLanguage table
					TaxDto.TaxLanguageRow[] taxLanguageRows = (TaxDto.TaxLanguageRow[])_TaxDto.TaxLanguage.Select(String.Format("LanguageCode='{0}'", langCode));
					if (taxLanguageRows != null && taxLanguageRows.Length > 0)
					{
						taxLanguageRow = taxLanguageRows[0];
					}
					else
					{
						taxLanguageRow = _TaxDto.TaxLanguage.NewTaxLanguageRow();
						taxLanguageRow.LanguageCode = langCode;
						taxLanguageRow.TaxId = _TaxDto.Tax[0].TaxId;
						taxLanguageRow.DisplayName = String.Empty;
					}

					// add taxLanguage to the source table
					DataRow sourceTableRow = sourceTable.NewRow();
					sourceTableRow["LanguageCode"] = taxLanguageRow.LanguageCode;
					sourceTableRow["FriendlyName"] = (string)row["FriendlyName"];
					sourceTableRow["DisplayName"] = taxLanguageRow.DisplayName;
					sourceTable.Rows.Add(sourceTableRow);
				}
			}
			else
			{
				// this is a new tax, bind empty table
				foreach (DataRow row in dtLanguages.Rows)
				{
					string langCode = row["LangName"].ToString();

					DataRow sourceTableRow = sourceTable.NewRow();
					sourceTableRow["LanguageCode"] = langCode;
					sourceTableRow["FriendlyName"] = (string)row["FriendlyName"];
					sourceTableRow["DisplayName"] = String.Empty;
					sourceTable.Rows.Add(sourceTableRow);
				}
			}

			LanguagesList.DataSource = sourceTable;
			LanguagesList.DataBind();
		}

		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			// load tax by name
			TaxDto dto = TaxManager.GetTax(args.Value);

			// check if jurisdiction with specified code is loaded
			if (dto != null && dto.Tax.Count > 0 &&
				dto.Tax[0].TaxId != TaxId &&
				String.Compare(dto.Tax[0].Name, args.Value, StringComparison.CurrentCultureIgnoreCase) == 0)
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
			_TaxDto = (TaxDto)context[_TaxDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CultureInfo ci = GetCurrentCulture();

			TaxDto dto = (TaxDto)context[_TaxDtoString];
			TaxDto.TaxRow taxRow = null;

			if (dto == null)
				// dto must be created in base control that holds tabs
				return;

			if (dto.Tax.Count > 0)
			{
				taxRow = dto.Tax[0];
			}
			else
			{
				taxRow = dto.Tax.NewTaxRow();
				taxRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
			}

			taxRow.Name = tbName.Text;
			taxRow.SortOrder = Int32.Parse(tbSortOrder.Text);
			taxRow.TaxType = Int32.Parse(TaxTypeList.SelectedValue);

			if (taxRow.RowState == DataRowState.Detached)
				dto.Tax.Rows.Add(taxRow);

			// save localized values
			foreach (RepeaterItem item in LanguagesList.Items)
			{
				HiddenField hfCtrl = item.FindControl("hfLangCode") as HiddenField;
				TextBox tbCtrl = item.FindControl("tbDisplayName") as TextBox;

				if (hfCtrl != null && tbCtrl != null)
				{
					TaxDto.TaxLanguageRow[] taxLanguageRows = (TaxDto.TaxLanguageRow[])dto.TaxLanguage.Select(String.Format("LanguageCode='{0}'", hfCtrl.Value));
					TaxDto.TaxLanguageRow taxLanguageRow = null;

					if (taxLanguageRows != null && taxLanguageRows.Length > 0)
						taxLanguageRow = taxLanguageRows[0];
					else
					{
						// add a new record for the current language
						taxLanguageRow = dto.TaxLanguage.NewTaxLanguageRow();
						taxLanguageRow.TaxId = taxRow.TaxId;
						taxLanguageRow.LanguageCode = hfCtrl.Value;
					}

					taxLanguageRow.DisplayName = tbCtrl.Text;

					if (taxLanguageRow.RowState == DataRowState.Detached)
						dto.TaxLanguage.Rows.Add(taxLanguageRow);
				}
			}
		}
		#endregion
	}
}
