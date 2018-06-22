using System;
using System.Collections;
using System.Data;
using Mediachase.Data.Provider;
using Mediachase.Cms.Data;

namespace Mediachase.Cms
{
	/// <summary>
	/// Summary description for Menu.
	/// </summary>
	public static class Menu
	{
		#region Add
        /// <summary>
        /// Adds the specified friendly name.
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
		public static int Add(string friendlyName, Guid siteId)
		{
            // TODO: check security

			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuAdd]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
		}
		#endregion

		#region Delete
        /// <summary>
        /// Deletes the specified menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
		public static void Delete(int menuId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			DataService.Run(cmd);
		}

        /// <summary>
        /// Delete all of the menus associated with a site
        /// </summary>
        /// <param name="siteId">Site unique identifer</param>
        /// Added for unit test purposes
        public static void DeleteBySiteId(Guid siteId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("cms_menu_MenuDeleteBySiteId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
            DataService.Run(cmd);
        }
		#endregion

		#region Update
        /// <summary>
        /// Updates the specified menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
		public static void Update(int menuId, string friendlyName)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
			DataService.Run(cmd);
		}
		#endregion

		#region LoadById
        /// <summary>
        /// Loads the by id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
		public static IDataReader LoadById(int menuId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuGetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion

		#region LoadByName
		/// <summary>
		/// Loads menu by name.
		/// </summary>
		/// <param name="name">The menu name.</param>
		/// <param name="siteId">The site id.</param>
		/// <returns>[MenuId], [FriendlyName]</returns>
		public static IDataReader LoadByName(string name, Guid siteId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_MenuGetByName]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FriendlyName", name, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
		}
		#endregion
	}
}
