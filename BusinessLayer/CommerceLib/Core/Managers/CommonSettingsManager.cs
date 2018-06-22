using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Core.Managers
{
	/// <summary>
	/// Implements operations for the settings manager.
	/// </summary>
	public static class CommonSettingsManager
	{
		public enum SettingsNames
		{
			DefaultLanguage,
			DefaultCurrency,
			DefaultWeight,
			DefaultLength,
			DefaultCustomer,
			DefaultCatalog,
			FullText
		}

		#region Settings Functions
		/// <summary>
		/// Gets the settings dto.
		/// </summary>
		/// <returns></returns>
		public static SettingsDto GetSettingsDto()
		{
			SettingsDto dto = null;

			// Load the object
			if (dto == null)
			{
				CommonSettingsAdmin admin = new CommonSettingsAdmin();
				admin.Load();
				dto = admin.CurrentDto;
			}

			dto.AcceptChanges();

			return dto;
		}

		/// <summary>
		/// Gets the setting by id.
		/// </summary>
		/// <returns></returns>
		public static SettingsDto GetSettingById(int settingId)
		{
			SettingsDto dto = null;

			// Load the object
			if (dto == null)
			{
				CommonSettingsAdmin admin = new CommonSettingsAdmin();
				admin.LoadBySettingId(settingId);
				dto = admin.CurrentDto;
			}

			dto.AcceptChanges();

			return dto;
		}

		/// <summary>
		/// Gets the setting by name.
		/// </summary>
		/// <returns></returns>
		public static SettingsDto GetSettingByName(SettingsNames settingName)
		{
			SettingsDto dto = null;

			// Load the object
			if (dto == null)
			{
				CommonSettingsAdmin admin = new CommonSettingsAdmin();
				admin.LoadBySettingName(settingName.ToString());
				dto = admin.CurrentDto;
			}

			dto.AcceptChanges();

			return dto;
		}

		public static string GetDefaultLanguage()
		{
			SettingsDto dto = GetSettingByName(SettingsNames.DefaultLanguage);
			if (dto.CommonSettings.Count > 0)
				return dto.CommonSettings[0].Value;
			else
				return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
		}

		public static string GetDefaultCurrency()
		{
			string currency = String.Empty;
			SettingsDto dto = GetSettingByName(SettingsNames.DefaultCurrency);
			if (dto.CommonSettings.Count > 0)
				currency = dto.CommonSettings[0].Value;
			else
			{
				CurrencyDto cDto = CurrencyManager.GetCurrencyDto();
				if (cDto != null && cDto.Currency.Count > 0)
					currency = cDto.Currency[0].CurrencyCode;
				else
					currency = new RegionInfo(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID).ISOCurrencySymbol;
			}

			return currency;
		}
		#endregion

		#region Edit Application Functions
		/// <summary>
		/// Saves the settings.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SaveSettings(SettingsDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("SettingsDto can not be null"));

			CommonSettingsAdmin admin = new CommonSettingsAdmin(dto);
			admin.Save();
		}
		#endregion
	}
}