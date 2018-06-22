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
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Data.Provider;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager.Marketing.CommandHandlers
{
	public class PolicyDeleteHandler : ICommand
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
						ProcessDeleteCommand(items);

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
							String.Format("alert('{0}{1}');", "Failed to delete item(s). Error: ", errorMessage), true);
					}
				}
				else
					return;
			}
		}
		#endregion

        /// <summary>
        /// Processes the delete command.
        /// </summary>
        /// <param name="items">The items.</param>
		void ProcessDeleteCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int id = Int32.Parse(keys[0]);

					PolicyDto dto = PolicyManager.GetPolicyDto(id);
					if (dto.Policy.Count > 0)
					{
						dto.Policy[0].Delete();
						PolicyManager.SavePolicy(dto);
					}
				}
			}
		}
	}
}