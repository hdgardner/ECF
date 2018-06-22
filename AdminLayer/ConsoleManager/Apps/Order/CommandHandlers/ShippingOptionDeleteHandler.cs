using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
	public class ShippingOptionDeleteHandler : ICommand
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

		void ProcessDeleteCommand(string[] items)
		{
			// load all shipping options
			ShippingMethodDto dto = ShippingManager.GetShippingMethods(null, true);

			if (dto != null && dto.ShippingOption.Count > 0)
			{
				// delete selected
				for (int i = 0; i < items.Length; i++)
				{
					string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
					if (keys != null)
					{
						Guid id = new Guid(keys[0]);

						// delete selected shipping option

						ShippingMethodDto.ShippingOptionRow[] soRows = (ShippingMethodDto.ShippingOptionRow[])dto.ShippingOption.Select(String.Format("ShippingOptionId='{0}'", id));
						if (soRows != null && soRows.Length > 0)
							soRows[0].Delete();
					}
				}

				if (dto.HasChanges())
					ShippingManager.SaveShipping(dto);
			}
		}
	}
}