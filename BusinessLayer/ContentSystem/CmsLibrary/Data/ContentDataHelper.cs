using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Cms.Data
{
	/// <summary>
	/// Helper class for cms data and data commands
	/// </summary>
	public static class ContentDataHelper
	{
		/// <summary>
		/// Creates the data command.
		/// </summary>
		/// <returns></returns>
		public static DataCommand CreateDataCommand()
		{
			DataCommand cmd = new DataCommand();
			cmd.ConnectionString = CmsConfiguration.Instance.Connection.AppDatabase;
			return cmd;
		}

		/// <summary>
		/// Creates the data command.
		/// </summary>
		/// <param name="commandText">The command text.</param>
		/// <returns></returns>
		public static DataCommand CreateDataCommand(string commandText)
		{
			DataCommand cmd = new DataCommand();
			cmd.ConnectionString = CmsConfiguration.Instance.Connection.AppDatabase;
			cmd.CommandText = commandText;
			return cmd;
		}
	}
}