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
using Mediachase.Cms;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable application menu tab.
    /// </summary>
    public class ContentEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("content:site:mng:view") || 
                    ProfileContext.Current.CheckPermission("content:site:nav:mng:view") ||
                    ProfileContext.Current.CheckPermission("content:admin:mng:view") ||
                    ProfileContext.Current.CheckPermission("content:admin:workflow:mng:view") ||
                    ProfileContext.Current.CheckPermission("content:admin:templates:mng:view"))
                    return true;
                else
                    return false;
            } 
            
            bool enable = false;
            if(SecurityManager.CheckPermission(
                new string[] {CmsRoles.AdminRole,
                              CmsRoles.EditorRole, 
                              CmsRoles.ManagerRole, 
                              CmsRoles.ViewerRole}, false))
            {
                enable = true;
            }

			return enable;
		}
		#endregion
	}
}