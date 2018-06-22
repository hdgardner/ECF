using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Core.Tabs
{
	public partial class CommonSettingsTab : BaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _SettingsDtoString = "SettingsDto";

		private SettingsDto _SettingsDto = null;

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
			BindLanguagesDropDown();
			BindCurrenciesDropDown();
			BindLengthsDropDown();
			BindWeightsDropDown();

			if (_SettingsDto.CommonSettings.Count > 0)
			{
				SelectItemInDropDown(LanguagesList, CommonSettingsManager.SettingsNames.DefaultLanguage);
				SelectItemInDropDown(CurrenciesList, CommonSettingsManager.SettingsNames.DefaultCurrency);
				SelectItemInDropDown(LengthUnitList, CommonSettingsManager.SettingsNames.DefaultLength);
				SelectItemInDropDown(WeightUnitList, CommonSettingsManager.SettingsNames.DefaultWeight);
			}
		}

		private void BindLanguagesDropDown()
		{
			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				ListItem item = new ListItem(row["FriendlyName"].ToString(), row["LangName"].ToString());
				LanguagesList.Items.Add(item);
			}
		}

		private void BindCurrenciesDropDown()
		{
			CurrencyDto dto = CurrencyManager.GetCurrencyDto();

			CurrenciesList.DataValueField = "CurrencyCode";
			CurrenciesList.DataTextField = "Name";

			CurrenciesList.DataSource = dto.Currency;
			CurrenciesList.DataBind();
		}

		private void BindWeightsDropDown()
		{
			WeightUnitList.Items.Add(new ListItem("Kilograms", "KGS"));
			WeightUnitList.Items.Add(new ListItem("Pounds", "LBS"));
		}

		private void BindLengthsDropDown()
		{
			LengthUnitList.Items.Add(new ListItem("Meters", "M"));
			LengthUnitList.Items.Add(new ListItem("Feet", "FT"));
		}

		private void SelectItemInDropDown(DropDownList list, CommonSettingsManager.SettingsNames value)
		{
			SettingsDto.CommonSettingsRow[] rows = (SettingsDto.CommonSettingsRow[])_SettingsDto.CommonSettings.Select(String.Format("Name='{0}'", value));
			if (rows != null && rows.Length > 0)
				ManagementHelper.SelectListItemIgnoreCase(list, rows[0].Value);
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_SettingsDto = (SettingsDto)context[_SettingsDtoString];
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			SettingsDto dto = (SettingsDto)context[_SettingsDtoString];

			if (dto == null)
				// dto must be created in base control that holds tabs
				return;

			FillItemFromDropDown(LanguagesList, CommonSettingsManager.SettingsNames.DefaultLanguage, ref dto);
			FillItemFromDropDown(CurrenciesList, CommonSettingsManager.SettingsNames.DefaultCurrency, ref dto);
			FillItemFromDropDown(LengthUnitList, CommonSettingsManager.SettingsNames.DefaultLength, ref dto);
			FillItemFromDropDown(WeightUnitList, CommonSettingsManager.SettingsNames.DefaultWeight, ref dto);
		}
		#endregion

		private void FillItemFromDropDown(DropDownList list, CommonSettingsManager.SettingsNames setting, ref SettingsDto dto)
		{
			SettingsDto.CommonSettingsRow[] rows = (SettingsDto.CommonSettingsRow[])dto.CommonSettings.Select(String.Format("Name='{0}'", setting));
			if (rows != null && rows.Length > 0)
				rows[0].Value = list.SelectedValue;
			else
			{
				// create new row
				SettingsDto.CommonSettingsRow row = dto.CommonSettings.NewCommonSettingsRow();
				row.ApplicationId = AppContext.Current.ApplicationId;
				row.Name = setting.ToString();
				row.Value = list.SelectedValue;

				if (row.RowState == DataRowState.Detached)
					dto.CommonSettings.Rows.Add(row);
			}
		}
	}
}