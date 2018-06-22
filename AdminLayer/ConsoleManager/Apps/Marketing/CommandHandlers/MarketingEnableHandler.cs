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
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Marketing.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable application menu tab.
    /// </summary>
    public class MarketingTabEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("marketing:campaigns:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("marketing:promotions:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("marketing:segments:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("marketing:policies:mng:view"))
                    return true;

                if (ProfileContext.Current.CheckPermission("marketing:expr:mng:view"))
                    return true;

                return false;
            }

            if(SecurityManager.CheckPermission(
                new string[] {MarketingRoles.AdminRole,
                              MarketingRoles.ManagerRole, 
                              MarketingRoles.ViewerRole}, false))
            {
                enable = true;
            }

			return enable;
		}
		#endregion
	}
}