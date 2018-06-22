using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Commerce.Catalog.Search;

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
{
	public class CurrencyDeleteHandler : ICommand
	{
		#region ICommand Members
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
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int id = Int32.Parse(keys[0]);

					// TODO: implement delete selected currency (maybe set currency to inactive instead of deleting)
					CurrencyDto dto = CurrencyManager.GetCurrencyByCurrencyId(id);
					if (dto != null && dto.Currency.Count > 0)
					{
						dto.Currency[0].Delete();
						CurrencyManager.SaveCurrency(dto);
					}
				}
			}
		}
	}
}