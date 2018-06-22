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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable admin tools.
    /// </summary>
    public class CatalogSchemaEnableHandler : ICommandEnableHandler
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
			bool enable = false;
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                if (ProfileContext.Current.CheckPermission("catalog:admin:warehouses:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:admin:meta:cls:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:admin:meta:fld:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:ctlg:entries:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:ctlg:nodes:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:ctlg:mng:view"))
                    return true;
                else
                    return false;
            }

            if(SecurityManager.CheckPermission(
                new string[] {CatalogRoles.CatalogAdminRole,
                              CatalogRoles.CatalogSchemaManagerRole}, false))
            {
                enable = true;
            }

			return enable;
		}
		#endregion
	}

    /// <summary>
    /// Used to enable/disable application menu tab.
    /// </summary>
    public class CatalogTabEnableHandler : ICommandEnableHandler
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
            // Check permissions
            if (ProfileConfiguration.Instance.EnablePermissions)
            {
                if (ProfileContext.Current.CheckPermission("catalog:ctlg:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:ctlg:nodes:mng:view") ||
                    ProfileContext.Current.CheckPermission("catalog:ctlg:entries:mng:view"))
                    return true;
                else
                    return false;
            }

            bool enable = false;

            if (SecurityManager.CheckPermission(
                new string[] {CatalogRoles.CatalogAdminRole,
                              CatalogRoles.CatalogManagerRole,                                
                              CatalogRoles.CatalogViewerRole}, false))
            {
                enable = true;
            }

            return enable;
        }
        #endregion
    }
}