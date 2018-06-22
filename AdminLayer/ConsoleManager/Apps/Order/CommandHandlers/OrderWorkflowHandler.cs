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
using System.Web.Management;

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
    public class OrderWorkflowHandler : ICommand
    {
        private const string _OrderGroupDtoSessionKey = "ECF.OrderGroup.Edit";

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
                WorkflowDefinition work = null;
                int error = 0;
                string errorMessage = String.Empty;
                try
                {
                    work = WorkflowConfiguration.Instance.GetWorkflow(cp.CommandName);
                    Guid customerGuid = ManagementHelper.GetGuidFromQueryString("customerid");
                    int orderid = ManagementHelper.GetIntFromQueryString("id");
                    ProcessWorkflowCommand(cp.CommandName, orderid, customerGuid, ManagementHelper.GetValueFromQueryString("class", ""));
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
                        String.Format("CSManagementClient.SetSaveStatus('Failed to process workflow command \"{0}\". Error: {1}');", work.DisplayName, errorMessage), true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                        String.Format("CSManagementClient.SetSaveStatus('Successfully completed \"{0}\". Make sure to save the order before proceeding.');", work.DisplayName), true);
                }
            }
        }
        #endregion

        /// <summary>
        /// Processes the workflow command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="id">The id.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        void ProcessWorkflowCommand(string commandName, int id, Guid customer, string type)
        {
            if (type == "PurchaseOrder")
            {
                object order = HttpContext.Current.Session[_OrderGroupDtoSessionKey];

                PurchaseOrder po = null;
                if (order != null && order is PurchaseOrder)
                {
                    po = (PurchaseOrder)order;
                }

                if(po == null)
                    po = OrderContext.Current.GetPurchaseOrder(customer, id);

                if (po != null)
                {
                    po.RunWorkflow(commandName, true);

                    if (HttpContext.Current != null)
                        HttpContext.Current.Items.Add("Order-" + id.ToString(), String.Empty);

                    //po.AcceptChanges();
                }
            }
        }
    }
}