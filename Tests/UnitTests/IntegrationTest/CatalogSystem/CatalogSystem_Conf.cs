using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.IO;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Data;

namespace UnitTests.CatalogSystem
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
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

        /// <summary>
        /// Initializes the specified test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]       
        public static void Initialize(TestContext testContext) 
        {
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
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

        /// <summary>
        /// Prints all catalogs.
        /// </summary>
        [TestMethod]
        public void PrintAllCatalogs()
        {
            CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
            {
                Console.WriteLine(catalogRow.Name);
            }
        }

        /// <summary>
        /// Cleanups the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
        }
        #endregion
    }
}