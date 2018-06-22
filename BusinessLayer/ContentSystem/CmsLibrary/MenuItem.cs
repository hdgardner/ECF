using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class MenuItem
	{
		#region Add
        /// <summary>
        /// Adds the specified menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <param name="text">The text.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <returns></returns>
		public static int Add(int menuId, string text, string commandText, int commandType, bool isActive, int sortOrder)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemAdd]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Text", text, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("CommandText", commandText, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("CommandType", commandType, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LeftImageUrl", String.Empty, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("RightImageUrl", String.Empty, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("IsVisible", isActive, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsInherits", false, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("Order", sortOrder, DataParameterType.Int));
			return DataService.RunReturnInteger(cmd);
		}

		#endregion

		#region Delete
        /// <summary>
        /// Deletes the specified menu item id.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
		public static void Delete(int menuItemId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			DataService.Run(cmd);
		}
		#endregion

		#region Update
        /// <summary>
        /// Updates the specified menu item id.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="text">The text.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="leftImageUrl">The left image URL.</param>
        /// <param name="rightImageUrl">The right image URL.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="isInherits">if set to <c>true</c> [is inherits].</param>
		/// <param name="sortOrder"></param>
		public static void Update(int menuItemId, string text, string commandText, int commandType, string leftImageUrl, string rightImageUrl, bool isVisible, bool isInherits, int sortOrder)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Text", text, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("CommandText", commandText, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("CommandType", commandType, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LeftImageUrl", leftImageUrl, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("RightImageUrl", rightImageUrl, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("IsVisible", isVisible, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("IsInherits", isInherits, DataParameterType.Bit));
			cmd.Parameters.Add(new DataParameter("Order", sortOrder, DataParameterType.Int));
			DataService.Run(cmd);
		}

		/// <summary>
		/// Updates the specified menu item id.
		/// </summary>
		/// <param name="menuItemId">The menu item id.</param>
		/// <param name="text">The text.</param>
		/// <param name="commandText">The command text.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="isVisible">if set to <c>true</c> [is visible].</param>
		public static void Update(int menuItemId, string text, string commandText, int commandType, bool isVisible, int sortOrder)
		{
			Update(menuItemId, text, commandText, commandType, "", "", isVisible, false, sortOrder);
		}

		public static void UpdateSortOrder(int menuItemId, int sortOrder)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemUpdateSortOrder]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Order", sortOrder, DataParameterType.Int));
			DataService.Run(cmd);
		}
		#endregion

		#region LoadById
		/// <summary>
		/// Loads menu item the by id.
		/// </summary>
		/// <param name="menuItemId">The menu item id.</param>
		/// <returns></returns>
		public static IDataReader LoadById(int menuItemId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetByMenuItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

        #region LoadByIdAndLanguageId
        /// <summary>
        /// Loads menu item the by id.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
		/// <param name="languageId">The language id.</param>
        /// <returns></returns>
        public static IDataReader LoadById(int menuItemId,int languageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetByMenuItemIdAndLanguageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

		#region LoadByMenuId
        /// <summary>
        /// Loads the by menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
		public static IDataReader LoadByMenuId(int menuId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetByMenuId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

        #region LoadByMenuIdAndLanguageId
        /// <summary>
        /// Loads the by menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
        public static IDataReader LoadByMenuId(int menuId, int languageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetByMenuIdLanguageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

		#region LoadSubMenuByMenuItemId
		/// <summary>
		/// Loads the sub menu.
		/// </summary>
		/// <param name="menuItemId">The menu item id.</param>
		/// <returns></returns>
		public static IDataReader LoadSubMenu(int menuItemId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetSubMenuByMenuItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}

        /// <summary>
        /// Loads the sub menu DT.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <returns></returns>
		public static DataTable LoadSubMenuDT(int menuItemId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetSubMenuByMenuItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
		}
		#endregion

        #region LoadSubMenuByMenuItemIdAndLanguageId
        /// <summary>
        /// Loads the sub menu.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
		/// <param name="languageId">The language id.</param>
        /// <returns></returns>
        public static IDataReader LoadSubMenu(int menuItemId,int languageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetSubMenuByMenuItemIdAndLanguageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Loads the sub menu DT.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
		public static DataTable LoadSubMenuDT(int menuItemId, int languageId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetSubMenuByMenuItemIdAndLanguageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }
        #endregion

		#region LoadAllChild
        /// <summary>
        /// Loads all child.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <returns></returns>
		public static IDataReader LoadAllChild(int menuItemId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetAllChild]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}

        /// <summary>
        /// Loads all child DT.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <returns></returns>
        public static DataTable LoadAllChildDT(int menuItemId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetAllChild]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
            return DataService.LoadTable(cmd);
        }
		#endregion

		#region LoadParent
		/// <summary>
		/// Loads the parent.
		/// </summary>
		/// <param name="menuItemId">The menu item id.</param>
		/// <returns></returns>
		public static IDataReader LoadParent(int menuItemId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetParentByMenuItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

		#region LoadPath
		/// <summary>
		/// Loads the path.
		/// </summary>
		/// <param name="menuItemId">The menu item id.</param>
		/// <returns>Path to element in IDataReader</returns>
		public static IDataReader LoadPath(int menuItemId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetPathByMenuItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

		#region InserAfter
        /// <summary>
        /// Inserts the after.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="targetMenuItemId">The target menu item id.</param>
		public static void InsertAfter(int menuItemId, int targetMenuItemId)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				MoveTo(menuItemId, targetMenuItemId, 2);
				scope.Complete();
			}
		}
		#endregion

		#region InsertBefore
        /// <summary>
        /// Inserts the before.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="targetMenuItemId">The target menu item id.</param>
		public static void InsertBefore(int menuItemId, int targetMenuItemId)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				MoveTo(menuItemId, targetMenuItemId, 1);
				scope.Complete();
			}
		}
		#endregion

		#region MoveTo
        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="targetMenuItemId">The target menu item id.</param>
		public static void MoveTo(int menuItemId, int targetMenuItemId)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				MoveTo(menuItemId, targetMenuItemId, 3);
				scope.Complete();
			}
		}

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="targetMenuItemId">The target menu item id.</param>
        /// <param name="moveToMode">The move to mode. Can be one of the following:
		/// --  1 moved item will be the first among the siblings
		/// --  2 moved item will be the last among the siblings
		///	--  3 sort order will not be changed
		/// </param>
        private static void MoveTo(int menuItemId, int targetMenuItemId, int moveToMode)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemMoveTo]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("TargetMenuItemId", targetMenuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("MoveToMode", moveToMode, DataParameterType.Int));
			DataService.Run(cmd);
        }
		#endregion

		#region LoadAllRoot
        /// <summary>
        /// Loads all root.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
		public static IDataReader LoadAllRoot(Guid siteId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuItemGetAllRoot]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

        /// <summary>
        /// Loads all DT.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
        public static DataTable LoadAllDT(Guid siteId, int languageId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetBySiteIdLanguageId]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
            return DataService.LoadTable(cmd);
        }

        #region LoadRootId()
        /// <summary>
        /// Loads the root id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static int LoadRootId(int menuId, Guid siteId)
        {
            using (IDataReader reader = LoadAllRoot(siteId))
            {
                while (reader.Read())
                {
                    if ((int)reader["MenuId"] == menuId)
                    {
                        return (int)reader["MenuItemId"];
                    }
                }

                reader.Close();
            }
            return -1;
        }
        #endregion

        #region AddResource
        /// <summary>
        /// Adds the resource.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="languageId">The language id.</param>
        /// <param name="text">The text.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <returns></returns>
        public static int AddResource(int menuItemId, int languageId, string text, string toolTip)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItem_ResourcesAdd]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Text", text, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("ToolTip", toolTip, DataParameterType.NVarChar, 250));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region UpdateResource
        /// <summary>
        /// Updates the resource.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="languageId">The language id.</param>
        /// <param name="text">The text.</param>
        /// <param name="toolTip">The tool tip.</param>
        public static void UpdateResource(int menuItemId, int languageId, string text, string toolTip)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItem_ResourcesUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Text", text, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("ToolTip", toolTip, DataParameterType.NVarChar, 250));
			DataService.Run(cmd);
        }
        #endregion

        #region DeleteResource
        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <param name="languageId">The language id.</param>
        public static void DeleteResource(int menuItemId, int languageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItem_ResourcesDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageId, DataParameterType.Int));
			DataService.Run(cmd);
        }
        #endregion

        #region DeleteAllResource
        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        public static void DeleteResource(int menuItemId)
        {
            DeleteResource(menuItemId, 0);
        }
        #endregion

        #region 2Delete LoadAvaliableLanguages
		/*
        public static ICollection<Int32> LoadAvailableLanguages(int menuItemId)
        {
            List<int> avaliableLanguages = new List<int>();

			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItem_ResourcesGetAvaliableLanguage]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));

			using (IDataReader reader = DataService.LoadReader(cmd).DataReader)
            {
                while (reader.Read())
                    avaliableLanguages.Add((int)reader["LanguageId"]);
            }
            return avaliableLanguages;
        }*/
        #endregion

        /// <summary>
        /// Loads the menu root.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
		public static IDataReader LoadMenuRoot(int menuId)
		{
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_MenuItemGetRootByMenuId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
    }
}
