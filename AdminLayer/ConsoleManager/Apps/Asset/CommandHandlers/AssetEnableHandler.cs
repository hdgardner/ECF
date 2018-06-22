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
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Library;

namespace Mediachase.Commerce.Manager.Asset.CommandHandlers
{
    /// <summary>
    /// Used to enable/disable application menu tab.
    /// </summary>
    public class AssetTabEnableHandler : ICommandEnableHandler
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
                if (ProfileContext.Current.CheckPermission("asset:mng:view"))
                    return true;

                return false;
            }

            if (SecurityManager.CheckPermission(
                new string[] {
                    AssetRoles.AdminRole,
                    AssetRoles.ManagerRole,
                    AssetRoles.ViewerRole}, false))
            {
                enable = true;
            }

			return enable;
		}
		#endregion
	}
}