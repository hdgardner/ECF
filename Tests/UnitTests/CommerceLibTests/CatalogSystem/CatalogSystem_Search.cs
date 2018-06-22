using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Search.Extensions;
using Mediachase.Search;
using Mediachase.Commerce.Core;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Mediachase.Commerce;

namespace UnitTests.CatalogSystem
{
    [TestClass()]
    public class CatalogSystem_Search
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

        /// <summary>
        ///A test for Current
        ///</summary>
        [TestMethod()]
        public void Search_JoinTable()
        {
            ICatalogSystem system = CatalogContext.Current; 

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

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

                    pars.CatalogNames.Add(catalogName);
                    pars.CatalogNodes.Add(node.Code);
                    pars.JoinType = "inner join";
                    pars.Language = "en-us";
                    pars.JoinSourceTable = "CatalogEntry";
                    pars.JoinTargetQuery = "CatalogEntryEx";
                    pars.JoinSourceTableKey = "CatalogEntryId";
                    pars.JoinTargetTableKey = "CatalogEntryEx.ObjectId";
                    pars.OrderByClause = "CatalogEntryEx.DisplayName";

                    Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                }
            }  
        }

        /// <summary>
        /// Searches the join query.
        /// </summary>
        [TestMethod()]
        public void Search_JoinQuery()
        {
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogNodeDto nodes = system.GetCatalogNodesDto(catalogName);
                foreach (CatalogNodeDto.CatalogNodeRow node in nodes.CatalogNode)
                {
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();
                    options.CacheResults = true;

                    pars.CatalogNames.Add(catalogName);
                    pars.CatalogNodes.Add(node.Code);
                    pars.JoinType = "inner join";
                    pars.Language = "en-us";
                    pars.JoinSourceTable = "CatalogEntry";
                    pars.JoinTargetQuery = "(select distinct ObjectId, DisplayName from CatalogEntryEx) CatalogEntryEx";
                    pars.JoinSourceTableKey = "CatalogEntryId";
                    pars.JoinTargetTableKey = "CatalogEntryEx.ObjectId";
                    pars.OrderByClause = "CatalogEntryEx.DisplayName";

                    Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                }
            }
        }

        /// <summary>
        /// Searches the browse entries.
        /// </summary>
        [TestMethod()]
        public void Search_BrowseEntries()
        {
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogNodeDto nodes = system.GetCatalogNodesDto(catalogName);
                foreach (CatalogNodeDto.CatalogNodeRow node in nodes.CatalogNode)
                {
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();
                    options.CacheResults = true;

                    pars.CatalogNames.Add(catalogName);
                    pars.CatalogNodes.Add(node.Code);

                    Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                }
            }
        }

        /// <summary>
        /// Searches the FTS.
        /// </summary>
        [TestMethod()]
        public void Search_FTS()
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

                pars.FreeTextSearchPhrase = "policy";
                pars.CatalogNames.Add(catalogName);

                Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            }
        }

        /// <summary>
        /// Searches the advanced FTS.
        /// </summary>
        [TestMethod()]
        public void Search_AdvancedFTS()
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

                pars.AdvancedFreeTextSearchPhrase = "(\"sweet and savory\" NEAR sauces) OR (\"sweet and savory\" NEAR candies)";
                pars.CatalogNames.Add(catalogName);

                Entries entries = CatalogContext.Current.FindItems(pars, options, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            }
        }

        /// <summary>
        /// Performs simple lucene search.
        /// </summary>
        [TestMethod()]
        public void Search_Lucene_Simple()
        {
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria();
            criteria.SearchPhrase = "canon";
            SearchManager manager = new SearchManager(AppContext.Current.ApplicationName);
            SearchResults results = manager.Search(criteria);
        }

        /// <summary>
        /// Searches using Lucense engine within all catalogs sorting the entries by display name.
        /// </summary>
        [TestMethod()]
        public void Search_Lucene_WithinCatalogsWithSorting()
        {
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            // Create Entry Criteria
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria();

            // Bind default catalogs if none found
            if (criteria.CatalogNames.Count == 0)
            {
                if (catalogs.Catalog.Count > 0)
                {
                    foreach (CatalogDto.CatalogRow row in catalogs.Catalog)
                    {
                        if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
                            criteria.CatalogNames.Add(row.Name);
                    }
                }
            }
            
            // Define phrase we want to search
            criteria.SearchPhrase = "canon";

            // Create a manager
            SearchManager manager = new SearchManager(AppContext.Current.ApplicationName);

            SearchResults results = null;

            // Define sort parameter
            criteria.Sort = new SearchSort("DisplayName");

            // Perform search
            results = manager.Search(criteria);

            Assert.IsTrue(results.TotalCount > 0, "No hits were found in Lucene index.");

            // Get IDs we need
            int[] resultIndexes = results.GetIntResults(0, 10 + 5); // we add padding here to accomodate entries that might have been deleted since last indexing

            // Retrieve actual entry objects, with no caching
            Entries entries = CatalogContext.Current.GetCatalogEntries(resultIndexes, false, new TimeSpan(), new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            entries.TotalResults = results.TotalCount;

            Assert.IsTrue(entries.TotalResults > 0, "No entries were returned from the database.");
        }

        /// <summary>
        /// Performs fuzzy lucense search.
        /// </summary>
        [TestMethod()]
        public void Search_Lucene_Fuzzy()
        {
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria();
            criteria.SearchPhrase = "fanon";
            SearchManager manager = new SearchManager(AppContext.Current.ApplicationName);
            SearchResults results = manager.Search(criteria);

            if (results.TotalCount == 0)
            {
                criteria.IsFuzzySearch = true;
                criteria.FuzzyMinSimilarity = 0.7f;
                results = manager.Search(criteria);
            }

            Assert.IsTrue(results.TotalCount > 0, "No hits were found in Lucene index.");
        }
    }
}