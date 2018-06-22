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
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Profile.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable admin tools.
    /// </summary>
    public class ProfileSchemaEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("profile:admin:meta:cls:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("profile:admin:meta:fld:mng:view"))
                    return true;

                return false;
            }
            
            bool enable = false;
            if(SecurityManager.CheckPermission(
                new string[] {ProfileRoles.AdminRole,
                              ProfileRoles.SchemaManagerRole}, false))
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
    public class ProfileTabEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("profile:acc:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("profile:org:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("profile:roles:mng:view"))
                    return true;

                return false;
            }

            
            bool enable = false;
            if (SecurityManager.CheckPermission(
                new string[] {
                    AppRoles.ManagerUserRole,
                    ProfileRoles.AdminRole,
                    ProfileRoles.ManagerRole, 
                    ProfileRoles.ViewerRole}, false))
            {
                enable = true;
            }

            return enable;
        }
        #endregion
    }
}