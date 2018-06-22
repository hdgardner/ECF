using System;
using System.Data;
using System.Globalization;
using System.Text;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class FileTreeItem
	{
		private static readonly CultureInfo _CultureInfo = CultureInfo.InvariantCulture;

        #region AddFileItem
        /// <summary>
        /// Adds the file item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="isFolder">if set to <c>true</c> [is folder].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="masterPage">The master page.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static int AddFileItem(string name, bool isPublic, bool isFolder, bool isDefault, string masterPage, Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeAdd]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("IsPublic", isPublic, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsFolder", isFolder, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("MasterPage", masterPage, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region UpdateFileItem
        /// <summary>
        /// Updates the file item.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="name">The name.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="isFolder">if set to <c>true</c> [is folder].</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="masterPage">The master page.</param>
        /// <param name="siteId">The site id.</param>
        public static void UpdateFileItem(int pageId, string name, bool isPublic, bool isFolder, bool isDefault, string masterPage, Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("IsPublic", isPublic, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsFolder", isFolder, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("MasterPage", masterPage, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			DataService.Run(cmd);
        }
        #endregion

        #region DeleteFileItem
        /// <summary>
        /// Deletes the file item.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        public static void DeleteFileItem(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_FileTreeDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			DataService.Run(cmd);
        }
        #endregion

        #region GetFileTreeItemAll
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetFileTreeItemAll(Guid siteId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadAllFolders()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static IDataReader LoadAllFolders(Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadAllFolders]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadAllFoldersDT()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static DataTable LoadAllFoldersDT(Guid siteId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadAllFolders]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region LoadItemByFolderId()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <returns></returns>
        public static IDataReader LoadItemByFolderId(int folderId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadByFolderId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FolderId", folderId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadItemByFolderIdDT()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <returns></returns>
        public static DataTable LoadItemByFolderIdDT(int folderId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadByFolderId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FolderId", folderId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region GetItemById
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader GetItemById(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

		/// <summary>
		/// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
		/// </summary>
		/// <param name="pageId">The page id.</param>
		/// <returns></returns>
		public static DataTable GetItemByIdDT(int pageId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
		}
        #endregion

        #region GetItemByOutline
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static IDataReader GetItemByOutline(string outline, Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadByOutline]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Outline", outline, DataParameterType.NVarChar, 2048));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetItemByOutlineAll
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static IDataReader GetItemByOutlineAll(string outline, Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadByOutlineAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Outline", outline, DataParameterType.NVarChar, 2048));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Gets the item by outline all.
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static DataTable GetItemByOutlineAllDT(string outline, Guid siteId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadByOutlineAll]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("Outline", outline, DataParameterType.NVarChar, 2048));
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
            return DataService.LoadTable(cmd);
        }
        #endregion

        #region GetFolderDefaultPage
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <returns></returns>
        public static IDataReader GetFolderDefaultPage(int folderId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_FileTreeLoadFolderDefaultPage]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FolderId", folderId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetFolderWithContent
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <returns></returns>
        public static IDataReader GetFolderWithContent(int folderId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_FileTreeLoadFolderWithContent]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FolderId", folderId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadParent()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader LoadParent(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadParent]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region  LoadParentAll()
        /// <summary>
        /// PageId, Name, Outline, OutlineLevel, IsFolder, IsDefault, Order, IsPublic, MasterPage
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader LoadParentAll(int pageId)
        {
          	DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeLoadParentAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region MoveTo
        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="newParentId">The new parent id.</param>
        public static void MoveTo(int pageId, int newParentId)
        {
            if (pageId != newParentId)
            {
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_FileTreeMoveTo]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NewParentId", newParentId, DataParameterType.Int));
				DataService.Run(cmd);
            }
        }
        #endregion

        /*
        #region IsVirtual
        public static bool IsVirtual(string outline)
        {
			string tempLang = !String.IsNullOrEmpty(outline) ? outline.Substring(outline.IndexOf('.') + 1) : String.Empty;
            string tmpOutline = String.Empty;
            string lang = String.Empty;
            int langId = -1;
            int pageId = -1;

			if (tempLang.Contains("."))
				lang = tempLang.Substring(0, tempLang.IndexOf('.'));

            if (lang.Length > 0)
				tmpOutline = !String.IsNullOrEmpty(outline) ? outline.Replace("." + lang, string.Empty) : String.Empty;
            else 
				tmpOutline = outline;

            foreach (DataRow row in Language.GetAllLanguagesDT().Rows)
            {
				if (String.Compare(row["LangName"].ToString(), lang, true, _CultureInfo) == 0)
					langId = Convert.ToInt32(row["LangId"].ToString());
            }
            if (!String.IsNullOrEmpty(lang) && langId == -1) 
				return false;

            using (IDataReader reader = GetItemByOutlineAll(tmpOutline, CMSContext.Current.SiteId))
            {
                if (reader.Read())
                {
					if (String.Compare(tmpOutline, reader.GetString(2), true, _CultureInfo) == 0)
						pageId = reader.GetInt32(0);
                }
            }

			return pageId == -1 ? false : true;
        }
        #endregion
         * */

        #region --- PageAccess ---

        #region AddPageAccess
        /// <summary>
        /// Adds the page access.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static int AddPageAccess(string roleId, int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_Add]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.RunReturnInteger(cmd);
        } 
        #endregion

        #region DeletePageAccess
        /// <summary>
        /// Deletes the page access.
        /// </summary>
        /// <param name="pageAccessId">The page access id.</param>
        public static void DeletePageAccess(int pageAccessId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_Delete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageAccessId", pageAccessId, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region UpdatePageAccess
        /// <summary>
        /// Updates the page access.
        /// </summary>
        /// <param name="pageAccessId">The page access id.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="pageId">The page id.</param>
        public static void UpdatePageAccess(int pageAccessId, string roleId, int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_Update]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageAccessId", pageAccessId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region PageAccessGetById
        /// <summary>
        /// PageAccessId, RoleId, PageId
        /// </summary>
        /// <param name="pageAccessId">The page access id.</param>
        /// <returns></returns>
        public static IDataReader PageAccessGetById(int pageAccessId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_GetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageAccessId", pageAccessId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        } 
        #endregion

        #region PageAccessGetByPageId
        /// <summary>
        /// PageAccessId, RoleId, PageId
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader PageAccessGetByPageId(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_GetByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        } 
        #endregion

        #region PageAccessGetAll
        /// <summary>
        /// PageAccessId, RoleId, PageId
        /// </summary>
		/// <param name="siteId"></param>
        /// <returns></returns>
        public static IDataReader PageAccessGetAll(Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_GetAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region PageAccessGetAllDT
        /// <summary>
        /// PageAccessId, RoleId, PageId
        /// </summary>
		/// <param name="siteId"></param>
        /// <returns></returns>
		public static DataTable PageAccessGetAllDT(Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_GetAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        } 
        #endregion
        
        #region PageAccessGetByRoleIdPageId
        /// <summary>
        /// PageAccessId, RoleId, PageId
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader PageAccessGetByRoleIdPageId(string roleId, int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageTreeAccess_GetByRoleIdPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion
        #endregion

        #region GetRoot()
        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static int GetRoot(Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_fs_FileTreeGetRoot]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion
    }
}
