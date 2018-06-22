using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Mediachase.Commerce.Engine.Template;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using MetaDataTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.OrderSystem
{
    /// <summary>
    /// Test class for payment plans
    /// </summary>
    [TestClass]
    public class OrderSystem_PaymentPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSystem_PaymentPlan"/> class.
        /// </summary>
        public OrderSystem_PaymentPlan()
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
        /// Payments the plan_ create from order_ first purchase order.
        /// </summary>
        [TestMethod]
        public void OrderSystem_PaymentPlan_CreateFromOrder_FirstPurchaseOrder()
        {
			Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());
			cart.OrderForms[0].Payments.Clear();
			cart.OrderForms[0].Payments.Add(OrderHelper.CreateCreditCardPayment());
			cart.AcceptChanges();
			cart.RunWorkflow("CartValidate");
			cart.RunWorkflow("CartPrepare");
			cart.OrderForms[0].Payments[0].Amount = cart.Total;
			//cart.RunWorkflow("CartCheckout");
			cart.AcceptChanges();

			// Create payment plan
			PaymentPlan plan = cart.SaveAsPaymentPlan();

            // Set some payment plan values
            // Monthly subscription for a year
            plan.CycleMode = PaymentPlanCycle.Months;
            plan.CycleLength = 1;
            plan.MaxCyclesCount = 12;
            plan.StartDate = DateTime.UtcNow;
            plan.EndDate = DateTime.UtcNow.AddMonths(13);
            plan.LastTransactionDate = DateTime.UtcNow;

            // Send emails
            OrderHelper.SendEmail(plan, "order-paymentplan-confirm");
            OrderHelper.SendEmail(plan, "order-paymentplan-notify");

            // Now create first PO from the payment plan
            PurchaseOrder po = plan.SaveAsPurchaseOrder();

            // Validate
            Assert.AreEqual(po.OrderForms[0].Payments.Count, 1);
        }
    }
}