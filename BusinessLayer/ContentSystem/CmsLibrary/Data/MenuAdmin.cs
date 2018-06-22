using System;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Cms.Data
{
    public class MenuAdmin
    {
        private MenuDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public MenuDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuAdmin"/> class.
        /// </summary>
        internal MenuAdmin()
        {
            _DataSet = new MenuDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal MenuAdmin(MenuDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        /// <value>The mapping.</value>
        private System.Data.Common.DataTableMapping[] Mapping
        {
            get
            {
				return DataHelper.MapTables("Menu", "MenuItem", "MenuItem_Resources");
            }
        }

		/// <summary>
		/// Gets the mapping. Used for saving changes to the dataset.
		/// </summary>
		/// <value>The mapping.</value>
		private System.Data.Common.DataTableMapping[] Mapping2
		{
			get
			{
				return DataHelper.MapTables2(
					new string[] { "main_Menu", "main_MenuItem", "main_MenuItem_Resources" },
					new string[] { "Menu", "MenuItem", "MenuItem_Resources" });
			}
		}

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand();
			cmd.TableMapping = Mapping2;

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(null, cmd, CurrentDto, "Menu", "MenuItem", "MenuItem_Resources");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified menu id.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        internal void Load(int menuId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_LoadById]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("MenuId", menuId, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            /*DataResult results =*/ DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified site id.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        internal void Load(Guid siteId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_LoadBySiteId]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by menu item id.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        internal void LoadByMenuItemId(int menuItemId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_menu_LoadByMenuItemId]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("MenuItemId", menuItemId, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            /*DataResult results =*/ DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        internal void Load()
        {
            Load(0);
        }
	}
}
