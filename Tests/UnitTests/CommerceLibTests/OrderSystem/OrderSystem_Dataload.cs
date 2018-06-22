using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Orders;
using MetaDataTest.Common;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using Mediachase.Commerce.Core.Managers;

namespace UnitTests.OrderSystem
{
    [TestClass()]
    public class OrderSystem_Dataload
    {
        private String[] _customerNames;
        Random _random;
        private int[] _statusWeight = { 20, 5, 5, 10, 2, 58 };
        private String[] _status = { "NewOrder", "Submitted", "Processing", "Cancelled", "Rejected", "Shipped" };

        public OrderSystem_Dataload()
        {
            _random = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
            getCustomerNames();
        }

        /// <summary>
        /// Gets the customer names from XML file and randomly shuffles them.
        /// </summary>
        private void getCustomerNames()
        {
            // Overwrite default customer info
            XmlDocument doc = new XmlDocument();
            //doc.Load("C:\\ECF5\\main\\src\\Tests\\UnitTests\\CommerceLibTests\\OrderSystem\\CustomerInfo.xml");
            doc.Load("CustomerInfo.xml");

            XmlNodeList nodeList;
            XmlNode root = doc.DocumentElement;
            nodeList = root.SelectNodes("/CustomerInfo/customer/name");

            String[] names = new String[nodeList.Count];
            // Do something with each
            XmlNode customer;
            for (int i = 0; i < nodeList.Count; i++)
            {
                customer = nodeList[i];
                names[i] = customer.InnerText;
            }
            _customerNames = randomizeStrings(names);
        }

        /// <summary>
        /// Unit test to create random order.
        /// Shipping and handling costs do not seem to be calculated correctly at CartPrepare workflow.
        /// </summary>
        [TestMethod()]
        public void OrderSystem_CreateRandomOrder()
        {
            Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, Guid.NewGuid());

            String customerFullName = _customerNames[_random.Next(0, _customerNames.Length - 1)];
            int space = customerFullName.IndexOf(' ');
            String customerFirstName = customerFullName.Substring(0, space);
            String customerLastName = customerFullName.Substring(space + 1);
            //String customerHomeId = customerFullName + "\'s " + "Home";

            cart.CustomerName = customerFullName;

            cart.BillingCurrency = CommonSettingsManager.GetDefaultCurrency();
            cart.OrderAddresses.Add(OrderHelper.CreateAddress());

            cart.OrderAddresses[0].Name = "Home";
            cart.OrderAddresses[0].FirstName = customerFirstName;
            cart.OrderAddresses[0].LastName = customerLastName;

            OrderForm orderForm = new OrderForm();

            orderForm.Name = "default";

            // Randomly pick a shipping method.
            Mediachase.Commerce.Orders.Dto.ShippingMethodDto smd = Mediachase.Commerce.Orders.Managers.ShippingManager.GetShippingMethods("en-us");
            int shippingMethod = _random.Next(smd.ShippingMethod.Count);
            Guid shippingMethodId = smd.ShippingMethod[shippingMethod].ShippingMethodId;
            String shippingMethodName = smd.ShippingMethod[shippingMethod].Name;
            
            // Add line items
            // Random number of line items in an order
            int itemNum = _random.Next(3, 10);
            for (int i = 0; i < itemNum; i++)
            {
                orderForm.LineItems.Add(createLineItem(_random, shippingMethodId, shippingMethodName));
            }

            // Add payments
            // Pay by phone
            orderForm.Payments.Add(OrderHelper.CreateInvoicePayment());
            // Pay by credit card (sends out emails)
            //orderForm.Payments.Add(OrderHelper.CreateCreditCardPayment());

            // Add discounts
            orderForm.Discounts.Add(OrderHelper.CreateOrderFormDiscount());

            cart.OrderForms.Add(orderForm);
            cart.OrderForms[0].BillingAddressId = cart.OrderAddresses[0].Name;

            
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");

            // Last step
            PurchaseOrder po = cart.SaveAsPurchaseOrder();
            // Randomize created date and time
            po.OrderForms[0].Created = randomDate();
            // Randomize order status
            po.Status = randomStatus();

            po.AcceptChanges();
        }

        /// <summary>
        /// Creates a line item.
        /// Code copied from OrderHelper.cs.
        /// </summary>
        /// <param name="random">A Random object seeded from the start of test method.</param>
        /// <returns></returns>
        private LineItem createLineItem(Random random, Guid shippingMethod, String shippingMethodName)
        {
            CatalogDto catalogs = CatalogContext.Current.GetCatalogDto();
            CatalogEntryDto.CatalogEntryRow entry = null;
            bool found = false;
            string catalogName = String.Empty;

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
            lineItem.ShippingMethodId = shippingMethod;
            lineItem.ShippingMethodName = shippingMethodName;
            lineItem.ShippingAddressId = "Home";
            // Choose a random quantity for chosen product.
            int quantity = random.Next(1, 7);
            lineItem.Quantity = quantity;
            lineItem.CatalogNode = catalogName;
            lineItem.Discounts.Add(OrderHelper.CreateLineItemDiscount());
            return lineItem;
        }

        private void cleanerTest() // I think
        {
            // Create cart
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());

            getCustomerNames();

            Random random = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);

            String customerFullName = _customerNames[random.Next(0, _customerNames.Length - 1)];
            int space = customerFullName.IndexOf(' ');
            String customerFirstName = customerFullName.Substring(0, space);
            String customerLastName = customerFullName.Substring(space + 1);
            String customerHomeId = customerFullName + "\'s " + "Home";

            cart.CustomerName = customerFullName;
            cart.OrderAddresses[0].Name = customerHomeId;
            cart.OrderAddresses[0].FirstName = customerFirstName;
            cart.OrderAddresses[0].LastName = customerLastName;

            #region Add items to cart

            // Random number of line items in an order
            int itemNum = random.Next(3, 10);
            // Add remaining items since default cart creates 3 line items
            for (int i = 3; i < itemNum; i++)
            {
                cart.OrderForms[0].LineItems.Add(OrderHelper.CreateLineItem());

            }
            // Overwrite default line item info
            for (int i = 0; i < itemNum; i++)
            {
                int quantity = random.Next(1, 10);
                cart.OrderForms[0].LineItems[i].Quantity = quantity;
                //cart.OrderForms[0].LineItems[i].ListPrice = 
            }
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateCreditCardPayment());
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");

            #endregion
            // Last step
            cart.SaveAsPurchaseOrder();
        }

        /// <summary>
        /// Return randomized version of the string array
        /// </summary>
        private string[] randomizeStrings(string[] arr)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            // Add all strings from array
            // Add new random int each time
            foreach (string s in arr)
            {
                list.Add(new KeyValuePair<int, string>(_random.Next(), s));
            }
            // Sort the list by the random number
            var sorted = from item in list
                         orderby item.Key
                         select item;
            // Allocate new string array
            string[] result = new string[arr.Length];
            // Copy values to array
            int index = 0;
            foreach (KeyValuePair<int, string> pair in sorted)
            {
                result[index] = pair.Value;
                index++;
            }
            // Return copied array
            return result;
        }

        private DateTime randomDate()
        {
            DateTime today = DateTime.Today;
            DateTime oneYearAgo = DateTime.Today.AddDays(-365);

            DateTime day = oneYearAgo.AddDays(_random.Next(365));
            day = day.AddHours(_random.Next(24));
            day = day.AddMinutes(_random.Next(60));
            day = day.AddSeconds(_random.Next(60));
            return day;
        }

        private string randomStatus()
        {
            String[] weightedStatus = new String[100];
            int count = 0;
            for (int i = 0; i < _statusWeight.Length; i++)
            {
                for (int j = 0; j < _statusWeight[i]; j++)
                {
                    weightedStatus[count] = _status[i];
                    count++;
                }
            }
            return weightedStatus[_random.Next(100)];
        }
    }
}
