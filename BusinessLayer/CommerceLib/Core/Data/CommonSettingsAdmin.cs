using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Core.Data
{
	/// <summary>
	/// Implements the methods for and represents the settings administrator.
	/// </summary>
	public class CommonSettingsAdmin
	{
		private SettingsDto _DataSet;

		/// <summary>
		/// Gets the current dto.
		/// </summary>
		/// <value>The current dto.</value>
		internal SettingsDto CurrentDto
		{
			get
			{
				return _DataSet;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonSettingsAdmin"/> class.
		/// </summary>
		internal CommonSettingsAdmin()
		{
			_DataSet = new SettingsDto();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommonSettingsAdmin"/> class.
		/// </summary>
		/// <param name="dto">The dto.</param>
		internal CommonSettingsAdmin(SettingsDto dto)
		{
			_DataSet = dto;
		}

		/// <summary>
		/// Loads the settings.
		/// </summary>
		internal void Load()
		{
			DataCommand cmd = CoreDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_Settings";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.DataSet = CurrentDto;
			cmd.TableMapping = DataHelper.MapTables("CommonSettings");

			DataService.LoadDataSet(cmd);
		}

		/// <summary>
		/// Loads by the setting id.
		/// </summary>
		/// <param name="id">The id.</param>
		internal void LoadBySettingId(int id)
		{
			DataCommand cmd = CoreDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_Setting_SettingId");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("SettingId", id, DataParameterType.Int));
			cmd.DataSet = CurrentDto;
			cmd.TableMapping = DataHelper.MapTables("CommonSettings");

			DataService.LoadDataSet(cmd);
		}

		/// <summary>
		/// Loads by the application name.
		/// </summary>
		/// <param name="name">The name.</param>
		internal void LoadBySettingName(string name)
		{
			DataCommand cmd = CoreDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_Setting_Name");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 100));
			cmd.DataSet = CurrentDto;
			cmd.TableMapping = DataHelper.MapTables("CommonSettings");

			DataService.LoadDataSet(cmd);
		}

		/// <summary>
		/// Updates the CommonSettings.
		/// </summary>
		internal void Save()
		{
			DataCommand cmd = CoreDataHelper.CreateDataCommand();
			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(null, cmd, CurrentDto, "CommonSettings");
				scope.Complete();
			}
		}
	}
}
