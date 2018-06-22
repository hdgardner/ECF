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
    public class Configuration
    {
        //private Guid _Guid;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
        }


        /// <summary>
        /// Initializes the specified test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]        
        public static void Initialize(TestContext testContext) 
        {
        }

        #region Test Methods
        /// <summary>
        /// Inits the test.
        /// </summary>
        [TestInitialize]
        public void InitTest()
        {
        }

        /*
        /// <summary>
        /// Recreates the meta data.
        /// </summary>
        [Description("fsdfsd"), TestMethod]
        public void RecreateMetaData()
        {
            RecreateMetaDataCompletely();
        }
         * */

        /// <summary>
        /// Cleanups the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
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
                field = MetaField.Create(OrderContext.MetaDataContext, "Mediachase.Commerce.Orders", "TrackingNumber", "Tracking Number", "Allows one to specify tracking number for the order", MetaDataType.ShortString, 100, true, false, false, true, false);
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
