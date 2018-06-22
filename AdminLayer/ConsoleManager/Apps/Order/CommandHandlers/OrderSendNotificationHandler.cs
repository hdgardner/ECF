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
using Common.Logging;

namespace Mediachase.Commerce.Manager.Order.CommandHandlers
{
    public class OrderSendNotificationHandler : ICommand
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
                int error = 0;
                int numEmails = 0;
                string errorMessage = String.Empty;
                try
                {
                    Guid customerGuid = ManagementHelper.GetGuidFromQueryString("customerid");
                    int orderid = ManagementHelper.GetIntFromQueryString("id");
                    numEmails = ProcessNotificationCommand(orderid, customerGuid, ManagementHelper.GetValueFromQueryString("class", ""));
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger(GetType()).Error("Order Notification Error.", ex);
                    error++;
                    errorMessage = ex.Message;
                }

                if (error > 0)
                {
                    errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
                    ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                        String.Format("CSManagementClient.SetSaveStatus('{0}{1} Check log for more details.');", "Failed to send notification. Error: ", errorMessage), true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                        String.Format("CSManagementClient.SetSaveStatus('Successfully sent {0} notifications');", numEmails.ToString()), true);
                }
            }
        }
        #endregion

        /// <summary>
        /// Processes the notification command.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="type">The type.</param>
        /// <returns>number of emails sent</returns>
        int ProcessNotificationCommand(int id, Guid customer, string type)
        {
            if (type == "PurchaseOrder")
            {
                object order = HttpContext.Current.Session[_OrderGroupDtoSessionKey];

                PurchaseOrder po = null;
                if (order != null && order is PurchaseOrder)
                {
                    po = (PurchaseOrder)order;
                }
                if (po == null)
                    po = OrderContext.Current.GetPurchaseOrder(customer, id);

                if (po != null)
                {
                    OrderAddress primaryAddress = null;
                    // Find primary address
                    foreach (OrderAddress address in po.OrderAddresses)
                    {
                        if (address.Name.Equals(po.OrderForms[0].BillingAddressId, StringComparison.OrdinalIgnoreCase))
                        {
                            primaryAddress = address;
                            break;
                        }
                    }

                    if (primaryAddress == null)
                        throw new ApplicationException("Primary billing address was not found");

                    // Send out emails
                    SendEmails(po, primaryAddress.Email);
                    return 2;
                }
            }

            return 0;
        }

        /// <summary>
        /// Sends the emails.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="email">The email.</param>
        private void SendEmails(PurchaseOrder order, string email)
        {
            string storeEmail = String.Empty;
            string storeTitle = String.Empty;

            // Find default site
            SiteDto sites = CMSContext.Current.GetSitesDto(AppContext.Current.ApplicationId);

            foreach (SiteDto.SiteRow site in sites.Site.Rows)
            {
                if (String.IsNullOrEmpty(storeEmail))
                {
                    storeTitle = GlobalVariable.GetVariable("title", site.SiteId);
                    storeEmail = GlobalVariable.GetVariable("email", site.SiteId);
                }

                if (site.IsDefault)
                {
                    storeTitle = GlobalVariable.GetVariable("title", site.SiteId);
                    storeEmail = GlobalVariable.GetVariable("email", site.SiteId);
                    break;
                }
            }

            // Add input parameter
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderGroup", order);

            // Send out emails
            // Create smtp client
            SmtpClient client = new SmtpClient();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(storeEmail, storeTitle);
            msg.IsBodyHtml = true;

            // Send confirmation email
            msg.Subject = String.Format("{1}: Order Confirmation for {0}", order.CustomerName, storeTitle);
            msg.To.Add(new MailAddress(email, order.CustomerName));
            msg.Body = TemplateService.Process("order-purchaseorder-confirm", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);

            msg = new MailMessage();
            msg.From = new MailAddress(storeEmail, storeTitle);
            msg.IsBodyHtml = true;

            // Send notify email
            msg.Subject = String.Format("{1}: Order Notification {0}", order.TrackingNumber, storeTitle);
            msg.To.Add(new MailAddress(storeEmail, storeTitle));
            msg.Body = TemplateService.Process("order-purchaseorder-notify", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);
        }
    }
}