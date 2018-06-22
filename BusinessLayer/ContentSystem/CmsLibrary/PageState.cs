using System;
using System.Data;
using Mediachase.Data.Provider;
using Mediachase.Cms.Data;

namespace Mediachase.Cms
{
	public static class PageState
	{
		#region GetById
        /// <summary>
        /// Gets page state the by StateId.
        /// </summary>
        /// <param name="stateId">StateId.</param>
        /// <returns>IDataReader</returns>
		public static IDataReader GetById(int stateId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_PageStateGetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		} 
		#endregion

		#region GetAll
        /// <summary>
        /// Get all page states.
        /// </summary>
        /// <returns>IDataReader</returns>
		public static IDataReader GetAll()
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_PageStateGetAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion
	}
}