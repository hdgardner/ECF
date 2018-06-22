using System;
using System.Xml;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Storage;
using Mediachase.Ibn.Core;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using MetaDataTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for OrderSystem_OrderContextTest and is intended
    ///to contain all OrderSystem_OrderContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderSystem_UnitTests
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void OrderSystem_SerializeTest()
        {
            XmlTextWriter writer = null; // TODO: Initialize to an appropriate value
            OrderGroup orderGroup = null; // TODO: Initialize to an appropriate value
            OrderContext.Serialize(writer, orderGroup);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetPurchaseOrders
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetPurchaseOrdersTest()
        {

        }

        /// <summary>
        ///A test for GetPurchaseOrder
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetPurchaseOrderTest()
        {
            Cart cart = CreateShoppingCart();

            cart.OrderForms[0].LineItems[0].Discounts.Add(OrderHelper.CreateLineItemDiscount());

            ShipmentDiscount discount = new ShipmentDiscount();
            discount.DiscountAmount = 10;
            discount.DiscountId = 1;
            discount.DiscountName = "asas";
            discount.DiscountValue = 10;
            discount.DisplayMessage = "asdasd";


            ShipmentDiscount discount2 = new ShipmentDiscount();
            discount2.DiscountAmount = 10;
            discount2.DiscountId = 2;
            discount2.DiscountName = "asas";
            discount2.DiscountValue = 10;
            discount2.DisplayMessage = "asdasd";


            cart.OrderForms[0].Shipments[0].Discounts.Add(discount);
            cart.OrderForms[0].Shipments[0].Discounts.Add(discount2);

            int cartLineItemDiscountCount = cart.OrderForms[0].LineItems[0].Discounts.Count;

            PurchaseOrder po = cart.SaveAsPurchaseOrder();
            po.AcceptChanges();

            // Reload cart from database
            PurchaseOrder po2 = OrderContext.Current.GetPurchaseOrder(po.CustomerId, po.OrderGroupId);

            int po1ShipmentDiscountsCount = po.OrderForms[0].Shipments[0].Discounts.Count;
            int po2ShipmentDiscountsCount = po2.OrderForms[0].Shipments[0].Discounts.Count;
            int po2LineItemDiscountCount = po2.OrderForms[0].LineItems[0].Discounts.Count;

            // Now remove discounts and add them again
            foreach (ShipmentDiscount dis in po2.OrderForms[0].Shipments[0].Discounts)
            {
                dis.Delete();
            }

            po2.OrderForms[0].Shipments[0].Discounts.Add(discount);
            po2.OrderForms[0].Shipments[0].Discounts.Add(discount2);
            po2.AcceptChanges();

            // Remove created stuff
            cart.Delete();
            cart.AcceptChanges();
            po2.Delete();
            po2.AcceptChanges();

            Assert.AreEqual(po1ShipmentDiscountsCount, po2ShipmentDiscountsCount);
            Assert.AreEqual(cartLineItemDiscountCount, po2LineItemDiscountCount);
        }

        /// <summary>
        ///A test for GetPaymentPlans
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetPaymentPlansTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for GetPaymentPlan
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetPaymentPlanTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for GetCart
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetCartTest1()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for GetCart
        ///</summary>
        [TestMethod()]
        public void OrderSystem_GetCartTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindPurchaseOrders
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindPurchaseOrdersTest1()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindPurchaseOrders
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindPurchaseOrdersTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindPaymentPlans
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindPaymentPlansTest1()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindPaymentPlans
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindPaymentPlansTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindCarts
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindCartsTest1()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for FindCarts
        ///</summary>
        [TestMethod()]
        public void OrderSystem_FindCartsTest()
        {
            // Creation of the private accessor for 'Microsoft.VisualStudio.TestTools.TypesAndSymbols.Assembly' failed
            Assert.Inconclusive("Creation of the private accessor for \'Microsoft.VisualStudio.TestTools.TypesAndSy" +
                    "mbols.Assembly\' failed");
        }

        /// <summary>
        ///A test for Deserialize
        ///</summary>
        [TestMethod()]
        public void OrderSystem_DeserializeTest()
        {
            XmlNode node = null; // TODO: Initialize to an appropriate value
            Type type = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = OrderContext.Deserialize(node, type);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateCart
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mediachase.Commerce.dll")]
        public void OrderSystem_CreateCartTest()
        {
            Guid newCustomer = Guid.NewGuid();
            Cart cart = OrderContext.Current.GetCart("myname", newCustomer);
            cart.CustomerName = "some name";
            cart.AcceptChanges();

            Cart newCart = OrderContext.Current.GetCart("myname", newCustomer);

            Assert.AreEqual(cart.CustomerName, newCart.CustomerName);
        }

        [TestMethod()]
        public void OrderSystem_ShoppingCartMerge()
        {
            Cart cart = CreateShoppingCart();
            cart.AcceptChanges();
            Cart cart2 = CreateShoppingCart();
            cart2.AcceptChanges();

            int lineItemCount = 0;
            // Count number of items
            foreach (OrderForm form in cart.OrderForms)
            {
                lineItemCount += form.LineItems.Count;
            }

            // Perform merge
            cart.Add(cart2);
            cart.AcceptChanges();

            // Clean up
            cart2.Delete();
            cart2.AcceptChanges();

            // Reload the cart
            cart = OrderContext.Current.GetCart(cart.CustomerId, cart.OrderGroupId);

            int lineItemCount2 = 0;
            // Count number of items
            foreach (OrderForm form in cart.OrderForms)
            {
                lineItemCount2 += form.LineItems.Count;
            }

            // Number should be double
            Assert.AreEqual(lineItemCount * 2, lineItemCount2);

            cart.Delete();
            cart.AcceptChanges();
        }

        [TestMethod()]
        public void OrderSystem_LineItemMetaDataSave()
        {
            bool metaFieldExists = false;
            MetaField brandField = null;
            string fieldName = "Brand";
            bool metaFieldAdded = false;
            string testForValue = "Test";
            string returnedValue = "";
            Cart cart = null;
            Cart newCart = null;
            Cart retrieveTestCart = null;
            Guid customerId = Guid.NewGuid();

            //Create cart. This cart is just used to get the lineitem meta class information
            try
            {
                cart = OrderHelper.CreateCartSimple(customerId);
            }
            catch (Exception exc)
            {
                if (exc.Message == "'maxValue' must be greater than zero.\r\nParameter name: maxValue")
                    Assert.Fail("Check your ApplicationId");
                else
                    throw exc;
            }

            //first make sure the meta field exists in the collection for the LineItem  collection
            MetaDataContext context = OrderContext.MetaDataContext;
            MetaClass mc = cart.OrderForms[0].LineItems[0].MetaClass;
            
            for(int i = 0; i < mc.MetaFields.Count; i++)
            {
                if (mc.MetaFields[i].Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    brandField = mc.MetaFields[i];
                    metaFieldExists = true;
                    break;
                }
            }

            if (!metaFieldExists)
            {
                brandField = MetaField.Load(context, fieldName);

                if (brandField == null)
                {
                    brandField = MetaField.Create(context, "Mediachase.Commerce.Orders.System", fieldName, fieldName, "", MetaDataType.ShortString, 100, true, false, false, false, false);
                    metaFieldAdded = true;
                }
                mc.AddField(brandField);                
            }

            //use a new customer id for the new cart
            customerId = Guid.NewGuid();

            //Create a new cart that will be used to add meta data to and save
            newCart = OrderHelper.CreateRetrieveOneLineItemCart(customerId);

            //now add a value for this new metafield in the first lineitem in the cart
            newCart.OrderForms[0].LineItems[0][fieldName] = testForValue;
            newCart.AcceptChanges();

            //now retrieve the cart anew and test 
            retrieveTestCart = Cart.LoadByCustomerAndName(newCart.CustomerId, newCart.Name);
            
            //check for the value
            if (retrieveTestCart.OrderForms[0].LineItems[0][fieldName] != null)
                returnedValue = retrieveTestCart.OrderForms[0].LineItems[0][fieldName].ToString();

            if (!metaFieldExists)
                mc.DeleteField(brandField);

            //delete the field if added
            if (metaFieldAdded)
                MetaField.Delete(context, brandField.Id);

            if (testForValue != returnedValue)
                Assert.Fail("Value was not saved");
        }

        [TestMethod()]
        public void OrderSystem_GetCartsForACustomer()
        {
            Cart firstCart = null;
            Cart secondCart = null;
            MetaStorageCollectionBase<Cart> carts = null;
            Guid customerId;

            //first add fictitious carts into the db
            customerId = Guid.NewGuid();

            //Create cart 1. This cart is just used to get the lineitem meta class information
            try
            {
                firstCart = OrderHelper.CreateCart(customerId, "FirstTestCart");
                firstCart.AcceptChanges();
            }
            catch (Exception exc)
            {
                if (exc.Message == "'maxValue' must be greater than zero.\r\nParameter name: maxValue")
                    Assert.Fail("Check your ApplicationId");
                else
                    throw exc;
            }

            //Create cart 1. This cart is just used to get the lineitem meta class information
            try
            {
                secondCart = OrderHelper.CreateCart(customerId, "SecondTestCart");
                secondCart.AcceptChanges();
            }
            catch (Exception exc)
            {
                if (exc.Message == "'maxValue' must be greater than zero.\r\nParameter name: maxValue")
                    Assert.Fail("Check your ApplicationId");
                else
                    throw exc;
            }

            try
            {
                carts = Cart.LoadByCustomer(customerId);
            }
            catch (Exception exc)
            {
                Assert.Fail("Error calling Cart.LoadByCustomer method : " + exc.Message);
            }

            if (carts.Count != 2)
                Assert.Fail("Incorrect number of carts found by Cart.LoadByCustomer(). Found: " + carts.Count);

            //cleanup
            firstCart.Delete();
            firstCart.AcceptChanges();
            secondCart.Delete();
            secondCart.AcceptChanges();
        }

        #region Shared Code

        private Cart CreateShoppingCart()
        {
            Cart cart = OrderHelper.CreateCartSimple(Guid.NewGuid());

            // No need to save cart before executing the workflows, they can be executed to the objects in memory
            // cart.AcceptChanges();
            cart.RunWorkflow("CartValidate");
            cart.RunWorkflow("CartPrepare");
            cart.OrderForms[0].Payments[0].Amount = cart.Total;
            cart.RunWorkflow("CartCheckout");
            return cart;
        }
        #endregion
    }
}
