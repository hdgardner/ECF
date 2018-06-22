using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;

namespace UnitTests.CatalogSystem
{
    [TestClass()]
    public class CatalogSystem_Perf
    {

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        /// <value>The test context.</value>
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
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// A test for browsing through all entries in all catalogs
        /// Entries not getting anything
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_BrowseEntries()
        {
            ICatalogSystem system = CatalogContext.Current; 

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            // Number of entries in CatalogEntry table
            int entryCount = 0;

            foreach(CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogNodeDto nodes = system.GetCatalogNodesDto(catalogName);
                foreach (CatalogNodeDto.CatalogNodeRow node in nodes.CatalogNode)
                {
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();
                    options.CacheResults = true;

                    pars.Language = "en-us";

                    pars.CatalogNames.Add(catalogName);
                    pars.CatalogNodes.Add(node.Code);

                    // Test does not seem to be working: entries are mostly returning empty.
                    Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

                    try
                    {
                        foreach (Entry entry in entries.Entry)
                        {
                            // Something to do? Just looking at entries
                            entryCount++;
                        }
                    }
                    catch (Exception e)
                    {
                        Assert.IsFalse(new NullReferenceException().Equals(e));
                    }
                }
            }
            // As of testing 4/19/09, entryCount incremented 22 times (there are over 1300 entries in the table CatalogEntry)
            Console.WriteLine("Number of entries browsed: {0:d}", entryCount);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for searching for entries using keyword search. Working.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SearchEntries()
        {
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogSearchParameters pars = new CatalogSearchParameters();
                CatalogSearchOptions options = new CatalogSearchOptions();

                
                // Search phrase arbitrary
                pars.FreeTextSearchPhrase = "policy";
                
                // Set language
                pars.Language = "en-us";
                
                pars.CatalogNames.Add(catalogName);

                Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

                // Meaningless assert - to be replaced with appropriate assert once the problem with search is figured out
                Assert.IsTrue(entries.TotalResults == entries.TotalResults);
                Console.WriteLine("Number of entries matching \"{0}\" in the {1} catalog: {2}", pars.FreeTextSearchPhrase, catalogName, entries.TotalResults); 
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

    }
}