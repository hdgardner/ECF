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
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable admin tools.
    /// </summary>
    public class OrderSchemaEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("order:admin:payments:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("order:admin:shipping:jur:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("order:admin:shipping:providers:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("order:admin:shipping:packages:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("order:admin:shipping:methods:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("order:admin:taxes:mng:view"))
                    return true;
                
                if (ProfileContext.Current.CheckPermission("order:admin:meta:cls:mng:view"))
                    return true;
                
                if (ProfileContext.Current.CheckPermission("order:admin:meta:fld:mng:view"))
                    return true;
                
                return false;
            }
            
            bool enable = false;
            if(SecurityManager.CheckPermission(
                new string[] {OrderRoles.AdminRole,
                              OrderRoles.SchemaManagerRole}, false))
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
    public class OrderTabEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("order:mng:view"))
                    return true;

                return false;
            }

            bool enable = false;
            if (SecurityManager.CheckPermission(
                new string[] {OrderRoles.AdminRole,
                              OrderRoles.ManagerRole,
                              OrderRoles.ViewerRole}, false))
            {
                enable = true;
            }

            return enable;
        }
        #endregion
    }
}