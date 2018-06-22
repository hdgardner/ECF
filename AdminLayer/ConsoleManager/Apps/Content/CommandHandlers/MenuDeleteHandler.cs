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
using mc = Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
	public class MenuDeleteHandler : ICommand
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

				int error = 0;
				string errorMessage = String.Empty;

				try
				{
					string[] items = null;

					if (cp.CommandArguments.ContainsKey(EcfListView.GridCommandParameterName) &&
						Boolean.Parse(cp.CommandArguments[EcfListView.GridCommandParameterName]))
					{
						// process command from grid (delete only one item)
						string primaryKeyId = cp.CommandArguments["primaryKeyId"];
						items = new string[] { primaryKeyId };
					}
					else
					{
						// get checked items and process batch delete command (from toolbar)
						string gridId = cp.CommandArguments["GridId"];
						items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);
						ManagementHelper.SetBindGridFlag(gridId);
					}

					if (items != null)
						ProcessDeleteCommand(items);
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
		}
		#endregion

        /// <summary>
        /// Processes the delete command.
        /// </summary>
        /// <param name="items">The items.</param>
		void ProcessDeleteCommand(string[] items)
		{
            if (!ProfileContext.Current.CheckPermission("content:site:menu:mng:edit"))
                throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            
            for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int menuId = 0;
					int menuItemId = 0;
					bool isRoot = false;

					if (!Int32.TryParse(keys[0], out menuItemId) ||
						!Int32.TryParse(keys[1], out menuId) ||
						!Boolean.TryParse(keys[2], out isRoot))
						continue;

					// delete selected menus
					if (isRoot) // deleting root menu
					{
						MenuDto dto = MenuManager.GetMenuDto(menuId);
						if (dto.Menu.Count > 0)
						{
							dto.Menu.Rows[0].Delete();
							MenuManager.SaveMenuDto(dto);
						}
					}
					else
					{
						mc.MenuItem.Delete(menuItemId);
						mc.MenuItem.DeleteResource(menuItemId);
					}
				}
			}
		}
	}
}