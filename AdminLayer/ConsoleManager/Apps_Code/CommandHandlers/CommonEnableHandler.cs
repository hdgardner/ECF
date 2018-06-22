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
	/// <summary>
	/// Used to enable/disable menu item depending whether grid is empty or not
	/// </summary>
	public class CommonEnableHandler : ICommandEnableHandler
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
			bool enable = false;
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;

				string gridId = cp.CommandArguments["GridId"];

				// find grid
				EcfListView grid = ManagementHelper.GetControlFromCollection<EcfListView>(((Control)sender).Page.Controls, gridId);

				// get EcfListViewControl which contains the specified grid
				EcfListViewControl lv = ManagementHelper.GetParentControl<EcfListViewControl>(grid);

				if (lv != null)
					enable = lv.CurrentListView.Items.Count > 0;
				else
					enable = false;

                // Check permissions too
                if (enable)
                {
                    if (ProfileConfiguration.Instance.EnablePermissions)
                    {
                        string permissions = cp.CommandArguments["permissions"];

                        if (permissions != string.Empty)
                        {
                            string[] permissionsArray = permissions.Split(new char[] { ',' });

                            foreach (string permission in permissionsArray)
                            {
                                if (!ProfileContext.Current.CheckPermission(permission))
                                    return false;
                            }
                        }
                    }
                }
			}
			return enable;
		}
		#endregion
	}
}