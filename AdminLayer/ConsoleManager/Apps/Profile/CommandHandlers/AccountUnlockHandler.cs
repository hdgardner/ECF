using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Profile.CommandHandlers
{
    public class AccountUnlockHandler : ICommand
	{
		#region ICommand Members
        /// <summary>
        /// Invokes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;

				string gridId = cp.CommandArguments["GridId"];
				string[] items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);

				if (items != null)
				{
					int error = 0;
					string errorMessage = String.Empty;
					try
					{
                        ProcessUnlockCommand(items);

						ManagementHelper.SetBindGridFlag(gridId);
					}
					catch (Exception ex)
					{
						error++;
						errorMessage = ex.Message;
					}

					if (error > 0)
					{
						errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
						ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                            String.Format("alert('{0}{1}');", "Failed to unlock item(s). Error: ", errorMessage), true);
					}
				}
				else
					return;
			}
		}
		#endregion

        /// <summary>
        /// Processes the unlock command.
        /// </summary>
        /// <param name="items">The items.</param>
        void ProcessUnlockCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					string id = keys[0];

                    MembershipUser user = null;
                    if (id != null)
                        user = Membership.GetUser(new Guid(id));

                    if (user != null)
                    {
                        user.UnlockUser();
                    }
				}
			}
		}
	}
}