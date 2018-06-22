using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MetaDataTest.Common;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Engine.Template;
using System.Threading;
using System.Net.Mail;

namespace UnitTests.OrderSystem
{
    /// <summary>
    /// Test class for different notifications related to order system.
    /// </summary>
    [TestClass]
    public class OrderSystem_Notifications
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSystem_Notifications"/> class.
        /// </summary>
        public OrderSystem_Notifications()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        /// <summary>
        /// Initialize performance.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]
        public static void PerformanceInitialize(TestContext testContext) 
        {
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Send email notifications to the credit card customer.
        /// </summary>
        [TestMethod]
        public void OrderSystem_Notifications_CreditCard_CustomerEmail()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
            cart.OrderForms[0].Payments.Clear();
            cart.OrderForms[0].Payments.Add(OrderHelper.CreateCreditCardPayment());
            cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            //cart.RunWorkflow("CartCheckout");
            cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();

            po = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            // Send emails
            OrderHelper.SendEmail(po, "order-purchaseorder-confirm");
            OrderHelper.SendEmail(po, "order-purchaseorder-notify");

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }

    }
}