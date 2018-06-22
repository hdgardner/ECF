using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Engine.Template;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Core.Managers;

namespace MetaDataTest.Common
{
    public class OrderHelper
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="template">The template.</param>
        public static void SendEmail(OrderGroup order, string template)
        {
            // Add input parameter
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderGroup", order);

            // Execute template processor
            string body = TemplateService.Process(template, Thread.CurrentThread.CurrentCulture, dic);

            // Send out emails
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("store@mediachase.com", "UNIT TEST: Mediachase Store");
            msg.To.Add(new MailAddress(order.OrderAddresses[0].Email, "UNIT TEST: " + order.Name));
            msg.Subject = "UNIT TEST: Mediachase Store: Order Notification";
            msg.Body = body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Send(msg);
        }

        /// <summary>
        /// Creates the cart simple.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static Cart CreateCartSimple(Guid customerId)
        {
            Cart cart = CreateCartSimple(customerId, Cart.DefaultName);
            return cart;
        }

        /// <summary>
        /// Creates a simple cart for a named cart.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static Cart CreateCartSimple(Guid customerId, string cartName)
        {
            Cart cart = OrderContext.Current.GetCart(cartName, customerId);
            cart.CustomerName = "John Doe";
			cart.BillingCurrency = CommonSettingsManager.GetDefaultCurrency();
            cart.OrderAddresses.Add(CreateAddress());
            cart.OrderForms.Add(CreateOrderForm("default"));
            cart.OrderForms[0].BillingAddressId = cart.OrderAddresses[0].Name;
            return cart;
        }

        /// <summary>
        /// Creates the cart.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static Cart CreateCart(Guid customerId)
        {
            Cart cart = CreateCart(customerId, Cart.DefaultName);
            return cart;
        }

        /// <summary>
        /// Creates the cart with a given name.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static Cart CreateCart(Guid customerId, string cartName)
        {
            Cart cart = OrderContext.Current.GetCart(cartName, customerId);
            cart.OrderAddresses.Add(CreateAddress());
            cart.OrderForms.Add(CreateOrderForm("default"));
            cart.OrderForms.Add(CreateOrderForm("default1"));
            cart.OrderForms.Add(CreateOrderForm("default2"));
            cart.OrderForms[0].BillingAddressId = cart.OrderAddresses[0].Name;
            cart.OrderForms[1].BillingAddressId = cart.OrderAddresses[0].Name;
            cart.OrderForms[2].BillingAddressId = cart.OrderAddresses[0].Name;
            return cart;
        }

        public static Cart CreateRetrieveOneLineItemCart(Guid customerId)
        {
            Cart cart = OrderHelper.CreateCartSimple(customerId);

            //delete extra line items
            for (int x = cart.OrderForms[0].LineItems.Count - 1; x > 0; x--)
                cart.OrderForms[0].LineItems[x].Delete();

            return cart;
        }

        /// <summary>
        /// Creates the order form.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static OrderForm CreateOrderForm(string name)
        {
            OrderForm orderForm = new OrderForm();

            orderForm.Name = name;

            // Add line items
            orderForm.LineItems.Add(CreateLineItem());
            orderForm.LineItems.Add(CreateLineItem());
            orderForm.LineItems.Add(CreateLineItem());

            // Add shipments
            // orderForm.Shipments.Add(CreateShipment());

            // Add payments
            orderForm.Payments.Add(CreateCreditCardPayment());

            // Add discounts
            orderForm.Discounts.Add(CreateOrderFormDiscount());

            // Return
            return orderForm;
        }

        /// <summary>
        /// Creates the order form discount.
        /// </summary>
        /// <returns></returns>
        public static OrderFormDiscount CreateOrderFormDiscount()
        {
            OrderFormDiscount discount = new OrderFormDiscount();
            discount.DisplayMessage = "Order Form Discount";
            discount.DiscountName = "@New Discount";
            discount.DiscountId = -1;
            discount.DiscountAmount = 12;
            discount.DiscountCode = "";
            return discount;
        }

        /// <summary>
        /// Creates the line item discount.
        /// </summary>
        /// <returns></returns>
        public static LineItemDiscount CreateLineItemDiscount()
        {
            LineItemDiscount discount = new LineItemDiscount();
            discount.DisplayMessage = "Line Item Discount";
            discount.DiscountName = "@New Discount";
            discount.DiscountId = -1;
            discount.DiscountAmount = 12;
            discount.DiscountCode = "";
            return discount;
        }

        /// <summary>
        /// Creates the shipment discount.
        /// </summary>
        /// <returns></returns>
        public static ShipmentDiscount CreateShipmentDiscount()
        {
            ShipmentDiscount discount = new ShipmentDiscount();
            discount.DisplayMessage = "Shipment Discount";
            discount.DiscountName = "@New Discount";
            discount.DiscountId = -1;
            discount.DiscountAmount = 12;
            discount.DiscountCode = "";
            return discount;
        }

        /// <summary>
        /// Creates the line item.
        /// </summary>
        /// <returns></returns>
        public static LineItem CreateLineItem()
        {
            CatalogDto catalogs = CatalogContext.Current.GetCatalogDto();
            CatalogEntryDto.CatalogEntryRow entry = null;
            bool found = false;
            string catalogName = String.Empty;
            Random random = new Random();

            int seed = 0;

            while (!found)
            {
                seed = random.Next(catalogs.Catalog.Count - 1);
                CatalogDto.CatalogRow catalog = catalogs.Catalog[seed];
                catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogNodeDto nodes = CatalogContext.Current.GetCatalogNodesDto(catalogName);

                // Pick random node
                if (nodes.CatalogNode.Count > 0)
                {
                    seed = random.Next(nodes.CatalogNode.Count - 1);

                    CatalogNodeDto.CatalogNodeRow node = nodes.CatalogNode[seed];

                    CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntriesDto(catalogName, node.CatalogNodeId);

                    if (entryDto.CatalogEntry.Count > 0)
                    {
                        seed = random.Next(entryDto.CatalogEntry.Count - 1);
                        entry = entryDto.CatalogEntry[seed];
                        if (entry.IsActive)
                            found = true;
                    }
                }
            }

            LineItem lineItem = new LineItem();
            lineItem.DisplayName = entry.Name;
            lineItem.CatalogEntryId = entry.Code;
            lineItem.ShippingMethodId = new Guid("17995798-a2cc-43ad-81e8-bb932f6827e4");
            lineItem.ShippingMethodName = "Online Download";
            lineItem.ShippingAddressId = "Home";
            lineItem.ListPrice = 100;
            lineItem.Quantity = 2;
            lineItem.CatalogNode = catalogName;
            lineItem.Discounts.Add(CreateLineItemDiscount());
            return lineItem;
        }

        /// <summary>
        /// Creates the shipment.
        /// </summary>
        /// <returns></returns>
        public static Shipment CreateShipment()
        {
            Shipment shipment = new Shipment();
            shipment.ShipmentTrackingNumber = "My tracking number";
            shipment.Discounts.Add(CreateShipmentDiscount());
            return shipment;
        }

        /// <summary>
        /// Creates the address.
        /// </summary>
        /// <returns></returns>
        public static OrderAddress CreateAddress()
        {
            OrderAddress address = new OrderAddress();
            address.Name = "Home";
            address.Organization = "Mediachase";
            address.CountryName = "United State";
            address.CountryCode = "USA";
            address.DaytimePhoneNumber = "323 28742839";
            address.Email = "store@mediachase.com";
            address.EveningPhoneNumber = "323 98347294";
            address.FaxNumber = "323 346767293";
            address.FirstName = "John";
            address.LastName = "Doe";
            address.Line1 = "734 Beverly Blvd";
            address.Line2 = "Suite 3434";
            address.PostalCode = "90000";
            address.RegionCode = "LA";
            address.RegionName = "Los Angeles County";
            address.State = "CA";               
            return address;
        }

		/// <summary>
		/// Creates the payment plan.
		/// </summary>
		/// <returns></returns>
		public static Cart CreateCartForPaymentPlan()
		{
			Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
			cart.OrderForms[0].Payments.Clear();
			cart.OrderForms[0].Payments.Add(OrderHelper.CreateCreditCardPayment());
			cart.OrderForms[0].Payments[0].BillingAddressId = cart.OrderForms[0].BillingAddressId;
			cart.AcceptChanges();
			cart.RunWorkflow("CartValidate");
			cart.RunWorkflow("CartPrepare");
			cart.OrderForms[0].Payments[0].Amount = cart.Total;
			cart.AcceptChanges();
			
			return cart;
		}

        //map the system keyword value of the payment method to the payment type
        /// <summary>
        /// Updates the payment method info.
        /// </summary>
        /// <param name="payment">The payment.</param>
        private static void UpdatePaymentMethodInfo(Payment payment)
        {
            payment.BillingAddressId = "Home";

            string paymentType = payment.GetType().Name;
            string matchingSystemKeyword = "";

            //first create an artificial payment method to payment type mapping
            switch (paymentType)
            {
                case "CreditCardPayment":
                    matchingSystemKeyword = "Authorize";
                    break;
                case "CashCardPayment":
                    matchingSystemKeyword = "ECheck";
                    break;
                case "GiftCardPayment":
                    matchingSystemKeyword = "ICharge";
                    break;
                case "InvoicePayment":
                    matchingSystemKeyword = "Generic";
                    break;
                case "OtherPayment":
                    matchingSystemKeyword = "Generic";
                    break;
            }

            //now retrieve the payment methods and set the payment properties from 
            //the payment method that 'matches'
            PaymentMethodDto paymentMethods = PaymentManager.GetPaymentMethods("en-us");
            
            foreach (PaymentMethodDto.PaymentMethodRow row in paymentMethods.PaymentMethod.Rows)
            {
                if (row.SystemKeyword == matchingSystemKeyword)
                {
                    payment.PaymentMethodId = row.PaymentMethodId;
                    payment.PaymentMethodName = row.Name;
                    break;
                }
            }
        }

        /// <summary>
        /// Creates the credit card payment.
        /// </summary>
        /// <returns></returns>
        public static Payment CreateCreditCardPayment()
        {
            CreditCardPayment payment = new CreditCardPayment();

            UpdatePaymentMethodInfo(payment);

			payment.CardType = "Visa";
			payment.CreditCardNumber = "4012888818888";
			payment.CreditCardSecurityCode = "123";
            payment.ExpirationMonth = 7;
            payment.ExpirationYear = DateTime.Now.AddYears(1).Year;
            payment.Amount = 10;
            return payment;
        }

        /// <summary>
        /// Creates the cash card payment.
        /// </summary>
        /// <returns></returns>
        public static Payment CreateCashCardPayment()
        {
            CashCardPayment payment = new CashCardPayment();
            UpdatePaymentMethodInfo(payment);

            payment.Amount = 10;
            return payment;
        }

        /// <summary>
        /// Creates the invoice payment.
        /// </summary>
        /// <returns></returns>
        public static Payment CreateInvoicePayment()
        {
            InvoicePayment payment = new InvoicePayment();
            UpdatePaymentMethodInfo(payment);

            payment.Amount = 10;
            payment.InvoiceNumber = Guid.NewGuid().ToString();
            return payment;
        }

        /// <summary>
        /// Creates the gift card payment.
        /// </summary>
        /// <returns></returns>
        public static Payment CreateGiftCardPayment()
        {
            GiftCardPayment payment = new GiftCardPayment();
            UpdatePaymentMethodInfo(payment);

            payment.Amount = 10;
            payment.ExpirationMonth = 7;
			payment.ExpirationYear = DateTime.Now.AddYears(1).Year;
            payment.GiftCardNumber = Guid.NewGuid().ToString();
            payment.GiftCardSecurityCode = "asds";
            return payment;
        }

        /// <summary>
        /// Creates the other payment.
        /// </summary>
        /// <returns></returns>
        public static Payment CreateOtherPayment()
        {
            OtherPayment payment = new OtherPayment();
            UpdatePaymentMethodInfo(payment);

            payment.Amount = 10;
            return payment;
        }
    }
}
