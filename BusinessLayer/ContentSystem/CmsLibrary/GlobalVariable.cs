using System;
using System.Data;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	/// <summary>
	/// Summary description for GlobalVariable.
	/// </summary>
	public static class GlobalVariable
	{
        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
		public static string GetVariable(string key, Guid siteId)
		{
            string cacheKey = CmsCache.CreateCacheKey("var", key, siteId.ToString());

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
                return (string)cachedObject;

            string val = String.Empty;
			using (IDataReader reader = GetGlobalVariablesReader(key, siteId))
            {
                if (reader.Read())
                    val = (string)reader["Value"];

                reader.Close();
            }

            CmsCache.Insert(cacheKey, val, CmsConfiguration.Instance.Cache.SiteVariablesTimeout);
            return val;
		}

        /// <summary>
        /// Sets the variable.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        /// <param name="siteId">The site id.</param>
        public static void SetVariable(string key, string val, Guid siteId)
		{
			using (IDataReader reader = GetGlobalVariablesReader(key, siteId))
            {
				DataCommand cmd = ContentDataHelper.CreateDataCommand();

                if (reader.Read())
                {
					cmd.CommandText = String.Format("[cms_GlobalVariablesUpdate]");
					cmd.Parameters = new DataParameters();
					cmd.Parameters.Add(new DataParameter("GlobalVariableId", (int)reader["GlobalVariableId"], DataParameterType.Int));
					cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 250));
					cmd.Parameters.Add(new DataParameter("Value", val, DataParameterType.NVarChar, 1024));
					cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
					DataService.Run(cmd);
                }
                else
                {
					cmd.CommandText = String.Format("[cms_GlobalVariablesAdd]");
					cmd.Parameters = new DataParameters();
					cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 250));
					cmd.Parameters.Add(new DataParameter("Value", val, DataParameterType.NVarChar, 1024));
					cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
					DataService.RunReturnInteger(cmd);
                }
                reader.Close();
            }
		}

        /// <summary>
        /// Gets the global variables reader.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
		private static IDataReader GetGlobalVariablesReader(string key, Guid siteId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_GlobalVariablesLoadByKey]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
		}
	}
}
