using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Ibn.Data;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Cms;
using Mediachase.Ibn.Library;

namespace Mediachase.Commerce.Manager.Core.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable admin tools.
    /// </summary>
    public class CoreEnableHandler : ICommandEnableHandler
    {
        #region ICommandEnableHandler Members

        /// <summary>
        /// Returns true, if user in the correct roles.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public bool IsEnable(object sender, object element)
		{
            //for now, nothing will prevent these nodes from appearing
            return true;
        }
		#endregion
	}

    /// <summary>
    /// Used to enable/disable Business Foundation link.
    /// </summary>
    public class CoreBusinessFoundationEnableHandler : ICommandEnableHandler
    {
        #region ICommandEnableHandler Members

        /// <summary>
        /// Returns true, if user in the correct roles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsEnable(object sender, object element)
        {
            if (SecurityManager.CheckPermission(new string[] { AppRoles.AdminRole }, false))
                return true;
            else
                return false;

        }
        #endregion
    }

    /// <summary>
    /// Used to enable/disable Business Foundation link.
    /// </summary>
    public class CoreAdministrationTabEnableHandler : ICommandEnableHandler
    {
        #region ICommandEnableHandler Members

        /// <summary>
        /// Returns true, if user in the correct roles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsEnable(object sender, object element)
        {
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                if (ProfileContext.Current.CheckPermission("core:mng:settings") ||
                    ProfileContext.Current.CheckPermission("asset:mng:view") ||
                    ProfileContext.Current.CheckPermission("asset:admin:mng:view") ||
                    ProfileContext.Current.CheckPermission("core:mng:licensing") ||
                    ProfileContext.Current.CheckPermission("core:mng:search") ||
                    ProfileContext.Current.CheckPermission("content:admin:templates:mng:view") ||
                    ProfileContext.Current.CheckPermission("content:site:mng:view") ||
                    ProfileContext.Current.CheckPermission("content:admin:workflow:mng:view"))
                    return true;
                else
                    return false;
            }
            
            bool enable = false;

            if (SecurityManager.CheckPermission(
                new string[] {
                    AssetRoles.AdminRole,
                    AssetRoles.SchemaManagerRole,
                    CatalogRoles.CatalogAdminRole,
                    CatalogRoles.CatalogSchemaManagerRole,
                    CmsRoles.AdminRole,
                    CmsRoles.EditorRole,
                    OrderRoles.AdminRole,
                    OrderRoles.SchemaManagerRole,
                    ProfileRoles.AdminRole,
                    ProfileRoles.SchemaManagerRole}, false))
            {
                enable = true;
            }

            return enable;
        }
        #endregion
    }

    /// <summary>
    /// Used to enable/disable Business Foundation link.
    /// </summary>
    public class DictionaryTabEnableHandler : ICommandEnableHandler
    {
        #region ICommandEnableHandler Members

        /// <summary>
        /// Returns true, if user in the correct roles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsEnable(object sender, object element)
        {
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                if (ProfileContext.Current.CheckPermission("content:site:mng:edit") ||
                    ProfileContext.Current.CheckPermission("catalog:admin:country:mng:view"))
                    return true;
                else
                    return false;
            }

            bool enable = false;

            if (SecurityManager.CheckPermission(
                new string[] {
                    AssetRoles.AdminRole,
                    AssetRoles.SchemaManagerRole,
                    CatalogRoles.CatalogAdminRole,
                    CatalogRoles.CatalogSchemaManagerRole,
                    CmsRoles.AdminRole,
                    CmsRoles.EditorRole,
                    OrderRoles.AdminRole,
                    OrderRoles.SchemaManagerRole,
                    ProfileRoles.AdminRole,
                    ProfileRoles.SchemaManagerRole}, false))
            {
                enable = true;
            }

            return enable;
        }
        #endregion
    }

}