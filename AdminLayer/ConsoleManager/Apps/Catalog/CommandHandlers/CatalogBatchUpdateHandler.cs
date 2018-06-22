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
using System.Web.UI.WebControls;
using Mediachase.Web.Console.Interfaces;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
{
	public class CatalogBatchUpdateHandler : ICommand
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
                EcfListView ecfLV = ManagementHelper.GetControlFromCollection<EcfListView>(((Control)sender).Page.Controls, gridId);

                if (ecfLV != null)
                {
                    int error = 0;
                    string errorMessage = String.Empty;

                    try
                    {
                        ProcessUpdateCommand(ecfLV.Items);
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
                            String.Format("alert('{0}{1}');", "Failed to update item(s). Error: ", errorMessage), true);
                    }
                }
			}
		}
		#endregion

        /// <summary>
        /// Processes the update command.
        /// </summary>
        /// <param name="items">The items.</param>
        void ProcessUpdateCommand(IList<ListViewDataItem> items)
		{
            foreach(ListViewItem item in items)
            {
                ProcessItemUpdateCommand(item);
            }
		}

        /// <summary>
        /// Processes the item update command.
        /// </summary>
        /// <param name="items">The items.</param>
        void ProcessItemUpdateCommand(ListViewItem item)
        {
            List<TemplateControl> ctrls = ManagementHelper.CollectControls<TemplateControl>(item);
            foreach (Control ctrl in ctrls)
            {
                IBatchUpdateControl buctrl = ctrl as IBatchUpdateControl;
                if (buctrl != null)
                {
                    buctrl.Update();
                }
            }
        }
	}
}