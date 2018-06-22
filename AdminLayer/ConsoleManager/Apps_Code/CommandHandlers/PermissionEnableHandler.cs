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
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.CommandHandlers
{
	public class PermissionEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		/// <summary>
		/// Returns true, if grid is not empty.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public bool IsEnable(object sender, object element)
		{
            if (!ProfileConfiguration.Instance.EnablePermissions)
                return true;

			bool enable = true;
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;

				string permissions = cp.CommandArguments["permissions"];

                string[] permissionsArray = permissions.Split(new char[] { ',' });

                if (permissionsArray.Length > 0)
                    enable = false;

                foreach (string permission in permissionsArray)
                {
                    bool required = false;
                    string newPermission = permission;
                    if (permission.StartsWith("*"))
                    {
                        required = true;
                        newPermission = permission.Substring(1);
                    }

                    if (!ProfileContext.Current.CheckPermission(newPermission))
                    {
                        if (required) // required permission, otherwise any of existing permission can be satisfied
                            return false;
                    }
                    else
                    {
                        enable = true;
                    }
                }
			}
			return enable;
		}
		#endregion
	}
}