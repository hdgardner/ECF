using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MetaDataTest.Common;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;

namespace UnitTests.OrderSystem
{
    /// <summary>
    /// Summary description for OrderSystem_Perf
    /// </summary>
    [TestClass]
    public class OrderSystem_Perf
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSystem_Perf"/> class.
        /// </summary>
        public OrderSystem_Perf()
        {
            //
            // TODO: Add constructor logic here
            //
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
        /// Creates the cart and purchase order with workflow.
        /// </summary>
        [TestMethod]
        public void OrderSystem_UnitTest_CreateCartAndPurchaseOrderWithWorkflowSucceed()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());            

            // No need to save cart before executing the workflows, they can be executed to the objects in memory
            // cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            //cart.AcceptChanges();
            PurchaseOrder po = cart.SaveAsPurchaseOrder();
            // Delete original cart, no need since it was never saved
            // cart.Delete();
            // cart.AcceptChanges();            
        }
    }
}