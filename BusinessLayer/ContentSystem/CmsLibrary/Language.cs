using System;
using System.Data;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class Language
    {
        #region AddLanguage
        /// <summary>
        /// Adds the language.
        /// </summary>
        /// <param name="langName">Name of the lang.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns></returns>
        public static int AddLanguage(string langName, string friendlyName, bool isDefault)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfo_Add]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("LangName", langName, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
            cmd.Parameters.Add(new DataParameter("ApplicationId", CMSContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region GetAllLanguages
        /// <summary>
        /// LangId, LangName, IsDefault
        /// </summary>
        /// <returns></returns>
        public static IDataReader GetAllLanguages()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfoLoadAll]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CMSContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// LangId, LangName, IsDefault
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllLanguagesDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfoLoadAll]");
            cmd.Parameters.Add(new DataParameter("ApplicationId", CMSContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region LoadLangByName()
        /// <summary>
        /// LangId, LangName, IsDefault
        /// </summary>
        /// <param name="langName">Name of the lang.</param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetLangByName(string langName)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfo_GetByLangName]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("LangName", langName, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("ApplicationId", CMSContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadLanguage()
        /// <summary>
        /// Loads the language.
        /// </summary>
        /// <param name="langId">The lang id.</param>
        /// <returns></returns>
        public static IDataReader LoadLanguage(int langId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfo_LoadById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

		#region UpdateLanguage
        /// <summary>
        /// Updates the language.
        /// </summary>
        /// <param name="langId">The lang id.</param>
        /// <param name="langName">Name of the lang.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
		public static void UpdateLanguage(int langId, string langName, string friendlyName, bool isDefault)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfo_Update]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangName", langName, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
            cmd.Parameters.Add(new DataParameter("ApplicationId", CMSContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			DataService.Run(cmd);
		}
		#endregion

		#region DeleteLanguage
        /// <summary>
        /// Deletes the language.
        /// </summary>
        /// <param name="langId">The lang id.</param>
		public static void DeleteLanguage(int langId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_LanguageInfo_Delete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			DataService.Run(cmd);
		}
		#endregion
    }
}
