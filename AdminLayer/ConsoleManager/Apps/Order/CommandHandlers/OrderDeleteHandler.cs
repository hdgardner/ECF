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

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
    public class OrderDeleteHandler : ICommand
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
                        ProcessDeleteCommand(items, ManagementHelper.GetValueFromQueryString("class", ""));

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
        /// <param name="type">The type.</param>
        void ProcessDeleteCommand(string[] items, string type)
        {
            for (int i = 0; i < items.Length; i++)
            {
                string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
                if (keys != null)
                {
                    int id = Int32.Parse(keys[0]);
                    Guid customer = new Guid(keys[1]);

                    OrderGroup order = null;
                    if (type == "ShoppingCart")
                        order = (OrderGroup)OrderContext.Current.GetCart(customer, id);
                    else if (type == "PaymentPlan")
                        order = (OrderGroup)OrderContext.Current.GetPaymentPlan(customer, id);
                    else
                        order = (OrderGroup)OrderContext.Current.GetPurchaseOrder(customer, id);

                    if (order != null)
                    {
                        order.Delete();
                        order.AcceptChanges();
                    }
                }
            }
        }
    }
}