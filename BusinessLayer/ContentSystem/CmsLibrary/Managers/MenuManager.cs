using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Data;

namespace Mediachase.Cms.Managers
{
    public static class MenuManager
    {
        /// <summary>
        /// Gets the menu dto.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
        public static MenuDto GetMenuDto(int menuId)
        {
            // TODO: add caching
            MenuAdmin admin = new MenuAdmin();
            admin.Load(menuId);
            return admin.CurrentDto;
        }

        /// <summary>
        /// Gets the menu by menu item dto.
        /// </summary>
        /// <param name="menuItemId">The menu item id.</param>
        /// <returns></returns>
        public static MenuDto GetMenuByMenuItemDto(int menuItemId)
        {
            // TODO: add caching
            MenuAdmin admin = new MenuAdmin();
            admin.LoadByMenuItemId(menuItemId);
            return admin.CurrentDto;
        }


        /// <summary>
        /// Gets the menu dto.
        /// </summary>
        /// <returns></returns>
        public static MenuDto GetMenuDto()
        {
            // TODO: add caching
            MenuAdmin admin = new MenuAdmin();
            admin.Load();
            return admin.CurrentDto;
        }

        /// <summary>
        /// Gets the menu dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public static MenuDto GetMenuDto(Guid siteId)
        {
            // TODO: add caching
            MenuAdmin admin = new MenuAdmin();
            admin.Load(siteId);
            return admin.CurrentDto;
        }

        /// <summary>
        /// Saves the menu dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveMenuDto(MenuDto dto)
        {
            MenuAdmin admin = new MenuAdmin(dto);
            admin.Save();
        }
    }
}
