using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Ibn.Library
{
	/// <summary>
	/// Implements methods for the asset context.
	/// </summary>
	public class AssetContext
	{
		static AssetContext _Instance;

		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>The current.</value>
		public static AssetContext Current
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new AssetContext();
				}

				return _Instance;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetContext"/> class.
		/// </summary>
		AssetContext()
		{
		}

		#region Public Methods
		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetAssetSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataSet result = SqlHelper.ExecuteDataset(Mediachase.Ibn.Data.DataContext.Current.SqlContext.ConnectionString, 
				"GetBusinessFoundationSchemaVersionNumber");
			if (result != null)
			{
				if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
				{
					DataRow row = result.Tables[0].Rows[0];
					major = (int)row["Major"];
					minor = (int)row["Minor"];
					patch = (int)row["Patch"];
					installDate = (DateTime)row["InstallDate"];
				}
			}

			return retval;
		}
		#endregion
	}
}