using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Collections;
using Mediachase.Commerce.Orders;
using System.Xml;
using System.IO;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using MetaDataTest.Common;

namespace UnitTests.OrderSystem
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OrderSystemUnitTests
    {
        private Guid _Guid;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSystemUnitTests"/> class.
        /// </summary>
        public OrderSystemUnitTests()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
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

        
        public bool ResetDataBase = false;

        /// <summary>
        /// Initializes the specified test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]        
        public static void Initialize(TestContext testContext) 
        {
            object[] attributes = typeof(OrderSystemUnitTests).GetMethod("CartModifyProperties").GetCustomAttributes(typeof(TestPropertyAttribute), false);
            bool createDatabase = false;
            for (int index = 0; index < attributes.Length; index++)
            {
                if (((TestPropertyAttribute)attributes[index]).Name == "ResetDatabase")
                    createDatabase = Boolean.Parse(((TestPropertyAttribute)attributes[index]).Value);
            }

            /*
            if(createDatabase)
                RecreateMetaDataCompletely();
             * */            
        }

        #region Test Methods
        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitTest()
        {
            //_Guid = Guid.NewGuid();
            //OrderHelper.CreateCart(_Guid);
        }

        /*
        /// <summary>
        /// Recreates the order meta data.
        /// </summary>
        [TestMethod]
        public void RecreateOrderMetaData()
        {
            RecreateMetaDataCompletely();
        }

        /// <summary>
        /// Modifies cart properties.
        /// </summary>
        [TestMethod]
        [TestProperty("ResetDatabase", "true")]
        public void CartModifyProperties()
        {
            // Retrive cart and modify some values
            Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, _Guid);
            cart.OrderAddresses[0].CountryName = "Russian Federation";
            cart.AcceptChanges();
        }

        /// <summary>
        /// Serializes cart.
        /// </summary>
        [TestMethod]
        public void CartSerialization()
        {
            Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, _Guid);

            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            OrderContext.Serialize(writer, cart);

            writer.BaseStream.Position = 0;
            string xml = new StreamReader(writer.BaseStream).ReadToEnd();
            Console.Write(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            Cart cartNew = (Cart)OrderContext.Deserialize(doc.DocumentElement, typeof(Cart));
        }

        /// <summary>
        /// Carts to purchase order.
        /// </summary>
        [TestMethod]
        public void CartToPurchaseOrder()
        {
            Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, _Guid);
            cart.RunWorkflow("CartValidate");
            //cart.RunPipeline("CartPrepare");
            //cart.RunPipeline("CartCheckout");           
            PurchaseOrder po = cart.SaveAsPurchaseOrder();
        }
         * 
         * */

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            /*
            // Remove shopping cart
            Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, _Guid);
            if (cart != null)
            {
                cart.Delete();
                cart.AcceptChanges();
            }
             * */
        }
        #endregion

        #region Cleanup Help Methods

        /// <summary>
        /// Recreates the meta data completely.
        /// </summary>
        private static void RecreateMetaDataCompletely()
        {
            RemoveMetadata();
            RestoreMetaData();
        }

        /// <summary>
        /// Removes the metadata.
        /// </summary>
        private static void RemoveMetadata()
        {
            foreach (MetaClass cl in MetaClass.GetList(OrderContext.MetaDataContext, "Mediachase.Commerce.Orders", true))
            {
                if (!cl.IsSystem)
                    MetaClass.Delete(OrderContext.MetaDataContext, cl.Id);
            }

            foreach (MetaClass cl in MetaClass.GetList(OrderContext.MetaDataContext, "Mediachase.Commerce.Orders", true))
            {
                MetaClass.Delete(OrderContext.MetaDataContext, cl.Id);
            }
        }

        /// <summary>
        /// Restores the meta data.
        /// </summary>
        private static void RestoreMetaData()
        {
            OrderConfiguration.ConfigureMetaData();
        }

        /// <summary>
        /// Backups the meta data.
        /// </summary>
        private static void BackupMetaData()
        {
            ArrayList list = new ArrayList();
            
            foreach (MetaClass cl in MetaClass.GetList(OrderContext.MetaDataContext))
                list.Add(cl);

            foreach (MetaField field in MetaField.GetList(OrderContext.MetaDataContext))
            {
                if(!field.IsSystem)
                    list.Add(field);
            }

            MetaInstaller.Backup("OrderSystem.mdp", list.ToArray() );
        }

        /// <summary>
        /// Creates the classes.
        /// </summary>
        private static void CreateClasses()
        {
            // Create main class
            MetaClass metaClass = MetaClass.Load(OrderContext.MetaDataContext, "OrderGroup");
            if (metaClass == null)
            {
                metaClass = MetaClass.Create(OrderContext.MetaDataContext, "OrderGroup", "Order Group", "OrderGroup", 0, true, "OrderGroup Class");
            }

            // create purchase order class
            MetaClass purchaseOrderClass = MetaClass.Load(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.PurchaseOrderClass.Name);
            if (purchaseOrderClass == null)
            {
                purchaseOrderClass = MetaClass.Create(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.PurchaseOrderClass.Name, "Purchase Order Class", "OrderGroup_PurchaseOrder", metaClass, MetaClassType.User, "Purchase Order Class");
            }

            MetaField field = MetaField.Load(OrderContext.MetaDataContext, "TrackingNumber");
            if (field == null)
            {
                field = MetaField.Create(OrderContext.MetaDataContext, "Mediachase.Commerce.Orders", "TrackingNumber", "Tracking Number", "Allows one to specify tracking number for the order", MetaDataType.ShortString, 50, true, false, false, true, false);
            }

            if (!purchaseOrderClass.UserMetaFields.Contains(field))
                purchaseOrderClass.AddField(field);


            // create cart class
            MetaClass cartOrderClass = MetaClass.Load(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.ShoppingCartClass.Name);
            if (cartOrderClass == null)
            {
                cartOrderClass = MetaClass.Create(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.ShoppingCartClass.Name, "Cart Order Class", "OrderGroup_ShoppingCart", metaClass, MetaClassType.User, "Cart Order Class");
            }

            // Create System Order Info class
            MetaClass orderFormClass = MetaClass.Load(OrderContext.MetaDataContext, "OrderForm");
            if (orderFormClass == null)
            {
                orderFormClass = MetaClass.Create(OrderContext.MetaDataContext, "OrderForm", "Order Form", "OrderForm", 0, true, "OrderForm Class");
            }

            // create purchase order class
            MetaClass orderFormExClass = MetaClass.Load(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.OrderFormClass.Name);
            if (orderFormExClass == null)
            {
                purchaseOrderClass = MetaClass.Create(OrderContext.MetaDataContext, OrderConfiguration.Instance.MetaClasses.OrderFormClass.Name, "Order Form Ex Class", "OrderFormEx", orderFormClass, MetaClassType.User, "Order Form Ex Class");
            }

            BackupMetaData();
        }
        #endregion
    }
}
