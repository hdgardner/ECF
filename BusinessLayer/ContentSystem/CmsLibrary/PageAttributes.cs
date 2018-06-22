using System;
using System.Data;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class PageAttributes
    {
        #region Add
        /// <summary>
        /// Adds the specified page id.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="title">The title.</param>
        /// <param name="metaKeys">The meta keys.</param>
        /// <param name="metaDescriptions">The meta descriptions.</param>
        /// <returns></returns>
        public static int Add(int pageId, string title, string metaKeys, string metaDescriptions)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesAdd]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Title", title, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("MetaKeys", metaKeys, DataParameterType.NVarChar, 4000));
			cmd.Parameters.Add(new DataParameter("MetaDescriptions", metaDescriptions, DataParameterType.NVarChar, 4000));
			return DataService.RunReturnInteger(cmd);
        }
 
	    #endregion   

        #region Delete
        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public static void Delete(int id)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region DeleteByPageId
        /// <summary>
        /// Deletes the by page id.
        /// </summary>
        /// <param name="pageId">The page id.</param>
		public static void DeleteByPageId(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesDeleteByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			DataService.Run(cmd);
        }
	    #endregion    

        #region Update
        /// <summary>
        /// Updates the specified id.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="title">The title.</param>
        /// <param name="metaKeys">The meta keys.</param>
        /// <param name="metaDescriptions">The meta descriptions.</param>
        public static void Update(int pageId, string title, string metaKeys, string metaDescriptions)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Title", title, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("MetaKeys", metaKeys, DataParameterType.NVarChar, 4000));
			cmd.Parameters.Add(new DataParameter("MetaDescriptions", metaDescriptions, DataParameterType.NVarChar, 4000));
			DataService.Run(cmd);
        } 
        #endregion

        #region GetById
        /// <summary>
        /// Id, PageId, Title, MetaKeys, MetaDescriptions
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IDataReader GetById(int id)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesGetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetByPageId
        /// <summary>
        /// Id, PageId, Title, MetaKeys, MetaDescriptions
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader GetByPageId(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageAttributesGetByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion
    }
}
