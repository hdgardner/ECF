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
using Mediachase.Data.Provider;
using System.Collections.Generic;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Engine.Template;
using System.Net.Mail;
using Mediachase.Cms;
using System.Threading;
using Mediachase.Commerce.Core;
using Mediachase.Cms.Dto;

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
    public class CartCreatePurchaseOrderHandler : ICommand
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
                    Guid customerGuid = ManagementHelper.GetGuidFromQueryString("customerid");
                    int orderid = ManagementHelper.GetIntFromQueryString("id");
                    ProcessCreateCommand(orderid, customerGuid);
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
                        String.Format("CSManagementClient.SetSaveStatus('{0}{1}');", "Failed to create Purchase Order. Error: ", errorMessage), true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                        String.Format("CSManagementClient.SetSaveStatus('Successfully created purchase order');"), true);
                }
            }
        }
        #endregion

        /// <summary>
        /// Processes the create command.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        void ProcessCreateCommand(int id, Guid customer)
        {
            Cart order = OrderContext.Current.GetCart(customer, id);
            if (order != null)
            {
                order.SaveAsPurchaseOrder();
            }
        }
    }
}