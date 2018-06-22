using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Cms.Objects;
using Mediachase.Cms.Dto;
using System.Collections.Specialized;
using System.Data;

namespace Mediachase.Cms.Managers
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Creates the site.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static CmsSite CreateSite(SiteDto.SiteRow row)
        {
            CmsSite site = new CmsSite();
            site.Description = row.Description;

            if (String.IsNullOrEmpty(row.Domain))
            {
                string rowDomain = row.Domain.Replace("\r\n", "\n");
                string[] domains = rowDomain.Split(new char[] { '\n' });
                site.Domains = domains;
            }

            site.ID = row.SiteId;
            site.Name = row.Name;

            // Add attributes
            site.Attributes = new StringDictionary();
            foreach (SiteDto.main_GlobalVariablesRow varRow in row.Getmain_GlobalVariablesRows())
            {
                site.Attributes.Add(varRow.KEY, varRow.VALUE);
            }

            return site;
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static SiteMenu CreateMenu(MenuDto menuDto, MenuDto.MenuRow row)
        {
            SiteMenu menu = new SiteMenu();
            menu.Name = row.FriendlyName;

            List<SiteMenuItem> menuItems = new List<SiteMenuItem>();

            /*
            // Load all menu item resources
            foreach (MenuDto.MenuItemRow itemRow in row.GetMenuItemRows())
            {
                // TODO: add item to a menu
                SiteMenuItem item = CreateMenuItem(itemRow);
                item.Menu
                menuItems.Add(item);
            }
             * */

            //menu.Items = menuItems.ToArray();
            return menu;
        }

        public static SiteMenuItem CreateMenuItem(MenuDto.MenuItemRow row)
        {
            SiteMenuItem item = new SiteMenuItem();
            item.CommandText = row.CommandText;
            item.CommandType = row.CommandType;
            item.IsVisible = row.IsVisible;
            item.SortOrder = row.Order;

            SiteMenuItemResources resources = new SiteMenuItemResources();
            List<SiteMenuItemResource> resourceList = new List<SiteMenuItemResource>();

            foreach (MenuDto.MenuItem_ResourcesRow itemRow in row.Getmain_MenuItem_ResourcesRows())
            {
                // TODO: add item to a menu
                resourceList.Add(CreateMenuItemResource(itemRow));
            }

            // Populate Children


            resources.Resource = resourceList.ToArray();           
            return item;
        }

        /// <summary>
        /// Creates the menu item resource.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static SiteMenuItemResource CreateMenuItemResource(MenuDto.MenuItem_ResourcesRow row)
        {
            SiteMenuItemResource item = new SiteMenuItemResource();
            item.Text = row.Text;
            item.Tooltip = row.ToolTip;
            item.LanguageCode = row.LanguageId.ToString();
            return item;
        }

    }
}
