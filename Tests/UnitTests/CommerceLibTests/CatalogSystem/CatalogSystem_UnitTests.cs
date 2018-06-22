using System;
using System.Data;
using System.IO;
using Mediachase.Cms;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Impl;
using Mediachase.Commerce.Catalog.ImportExport;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Catalog.CSVImport;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;

namespace UnitTests.CatalogSystem
{

    /// <summary>
    ///This is a test class for CatalogSystem_CatalogContextImplTest and is intended
    ///to contain all CatalogSystem_CatalogContextImplTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CatalogSystem_UnitTests
    {
        public CatalogSystem_UnitTests()
        {
            // Disable caching
            CatalogConfiguration.Instance.Cache.IsEnabled = false;
            // Set index of added row to 0
            index = 0;
        }

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


        #region Helper methods for testing
        private string _testCategory;

        internal string TestCategory
        {
            get
            {
                return _testCategory;
            }
            set
            {
                _testCategory = value;
            }
        }

        /// <summary>
        /// Delivers the fail assert.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void deliverFailAssert(string message)
        {
            Assert.Fail(message + " Fail!");
        }

        /// <summary>
        /// Generate the fail message.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        internal void failMessage(string columnName)
        {
            deliverFailAssert(this.TestCategory + " " + columnName + " does not match.");
        }

        /// <summary>
        /// Generate the fail message.
        /// </summary>
        /// <param name="add">if set to <c>true</c> [add].</param>
        internal void failMessage(bool addRow)
        {
            string message = this.TestCategory;

            if (addRow)
                message += " row was not added.";
            else
                message += " row was not removed.";

            deliverFailAssert(message);
        }

        /// <summary>
        /// Handles the null exception.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        /// <param name="wantNull">if set to <c>true</c> [want null].</param>
        internal void handleNullException(Exception exception, bool addRow, bool wantNull)
        {
            if (exception.Message.Contains("Null") || exception.Message.Contains("null"))
            {
                if (wantNull)
                    Assert.IsTrue(true);
                else
                    failMessage(addRow);
            }
        }

        /// <summary>
        /// Handles the generic fail exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        internal void handleGenericFailException(Exception exception, bool addRow)
        {
            if (exception.Message.Contains("Fail!"))
                failMessage(addRow);
        }

        /// <summary>
        /// Handles the generic fail exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="messageParameter">The message parameter.</param>
        internal void handleGenericFailException(Exception exception, string messageParameter)
        {
            if (exception.Message.Contains("Fail!"))
                failMessage(messageParameter);
        }

        // If I need to use a random number
        Random rand = new Random();

        // Index for added row
        int index;

        // Generic counter
        int i;
        #endregion

        #region Catalog system save unit tests

        /// <summary>
        /// A test for SaveCurrency. Success
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCurrency()
        {
            ICatalogSystem target = CatalogContext.Current;

            CurrencyDto dto = new CurrencyDto(); // TODO: Initialize to an appropriate value

            // API Testing Note: Can't get DTO with the presence of both a currency row and a currency RATE row
            // Error message:  System.Data.ConstraintException: Failed to enable constraints. 
            //                  One or more rows contain values violating non-null, unique, or foreign-key constraints..

            // Set parameters, add row, and check row

            // Currency table
            // Make new currency
            CurrencyDto.CurrencyRow newRow = dto.Currency.NewCurrencyRow();
            string currencyCode = "JPT";
            string name = "Jupiter";
            DateTime modifiedDate = DateTime.Today;
            Guid applicationId = AppContext.Current.ApplicationId;
            newRow.CurrencyCode = currencyCode;
            newRow.Name = name;
            newRow.ModifiedDate = modifiedDate;
            newRow.ApplicationId = applicationId;
            dto.Currency.AddCurrencyRow(newRow);
            target.SaveCurrency(dto);

            // Make second new currency
            newRow = dto.Currency.NewCurrencyRow();
            string currencyCode1 = "SAT";
            string name1 = "Saturn";
            modifiedDate = DateTime.Today;
            Guid applicationId1 = AppContext.Current.ApplicationId;
            newRow.CurrencyCode = currencyCode1;
            newRow.Name = name1;
            newRow.ModifiedDate = modifiedDate;
            newRow.ApplicationId = applicationId1;
            dto.Currency.AddCurrencyRow(newRow);
            target.SaveCurrency(dto);

            // Currency rate table
            CurrencyDto rateDTO = target.GetCurrencyDto();
            CurrencyDto.CurrencyRateRow newRateRow = rateDTO.CurrencyRate.NewCurrencyRateRow();
            float averageRate = 1;
            float endOfDayRate = 1;
            DateTime rateModifiedDate = DateTime.Today;
            int fromCurrencyId = rateDTO.Currency.Select("CurrencyCode = 'JPT'")[0].Field<int>("CurrencyId");
            int toCurrencyId = rateDTO.Currency.Select("CurrencyCode = 'SAT'")[0].Field<int>("CurrencyId");
            DateTime currencyRateDate = DateTime.Today;
            newRateRow.AverageRate = averageRate;
            newRateRow.EndOfDayRate = endOfDayRate;
            newRateRow.ModifiedDate = rateModifiedDate;
            newRateRow.FromCurrencyId = fromCurrencyId;
            newRateRow.ToCurrencyId = toCurrencyId;
            newRateRow.CurrencyRateDate = currencyRateDate;
            rateDTO.CurrencyRate.AddCurrencyRateRow(newRateRow);
            target.SaveCurrency(rateDTO);

            CurrencyDto dto2 = target.GetCurrencyDto();

            this.TestCategory = "Currency";
            // Check first new row
            int index1 = -1;
            CurrencyDto.CurrencyRow row = null;
            try
            {
                for (i = 0; i < dto2.Currency.Count; i++)
                {
                    row = dto2.Currency[i];
                    if (row.Name.Equals("Jupiter"))
                    {
                        index1 = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.Currency.Count)
                failMessage(true);

            if (!row.CurrencyCode.Equals(currencyCode))
                failMessage("code");
            if (!row.ModifiedDate.Equals(modifiedDate))
                failMessage("modified date");
            if (!row.ApplicationId.Equals(applicationId))
                failMessage("application ID");

            // Check second new row
            row = null;
            int index2 = -1;
            try
            {
                for (i = 0; i < dto2.Currency.Count; i++)
                {
                    row = dto2.Currency[i];
                    if (row.Name.Equals("Saturn"))
                    {
                        index2 = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.Currency.Count)
                failMessage(true);

            if (!row.CurrencyCode.Equals(currencyCode1))
                failMessage("code");
            if (!row.ModifiedDate.Equals(modifiedDate))
                failMessage("modified date");
            if (!row.ApplicationId.Equals(applicationId1))
                failMessage("application ID");

            this.TestCategory = "Currency rate";
            // Check added currency rate
            CurrencyDto.CurrencyRateRow rateRow = null;
            int rateIndex = 0;
            try
            {
                for (i = 0; i < dto2.CurrencyRate.Count; i++)
                {
                    rateRow = dto2.CurrencyRate[i];
                    if (rateRow.FromCurrencyId == fromCurrencyId &&
                        rateRow.ToCurrencyId == toCurrencyId)
                    {
                        rateIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CurrencyRate.Count)
                failMessage(true);

            if (!rateRow.ModifiedDate.Equals(rateModifiedDate))
                failMessage("rate modified date");
            if (!rateRow.CurrencyRateDate.Equals(currencyRateDate))
                failMessage("rate date");

            // Remove added row and check for removal
            dto2.Currency[index1].Delete();
            dto2.Currency[index2].Delete();
            dto2.CurrencyRate[rateIndex].Delete();
            target.SaveCurrency(dto2);

            dto2 = target.GetCurrencyDto();

            this.TestCategory = "Currency";
            // Delete first added row
            CurrencyDto.CurrencyRow removed;
            try
            {
                for (i = 0; i < dto2.Currency.Count; i++)
                {
                    removed = dto2.Currency[i];
                    if (removed.Name.Equals(name))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.Currency.Count)
                Assert.IsTrue(true);

            // Delete second added row
            try
            {
                for (i = 0; i < dto2.Currency.Count; i++)
                {
                    removed = dto2.Currency[i];
                    if (removed.Name.Equals(name1))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.Currency.Count)
                Assert.IsTrue(true);

            this.TestCategory = "Currency rate";
            CurrencyDto.CurrencyRateRow rateRemoved;
            try
            {
                for (i = 0; i < dto2.CurrencyRate.Count; i++)
                {
                    rateRemoved = dto2.CurrencyRate[i];
                    if (rateRemoved.FromCurrencyId == fromCurrencyId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CurrencyRate.Count)
                Assert.IsTrue(true);
        }

        /// <summary>
        /// A test for SaveCatalogRelationDto. 
        /// (In principle dependent on CatalogEntryTest) Need entries in database.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCatalogRelationDto()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogRelationDto dto = new CatalogRelationDto(); // TODO: Initialize to an appropriate value

            // Set parameters
            CatalogRelationDto.CatalogEntryRelationRow newEntryRelation = dto.CatalogEntryRelation.NewCatalogEntryRelationRow();
            int parentEntryId = target.GetCatalogEntryDto("default test code").CatalogEntry[0].CatalogEntryId;
            int childEntryId = target.GetCatalogEntryDto("some code 1").CatalogEntry[0].CatalogEntryId;
            string relationTypeId = "Completely imaginary";
            decimal quantity = 10; // Arbitrarily chosen
            string groupName = "API Unit Tester Group";
            int entryRelationSortOrder = 5253; // Completely arbitrary again

            newEntryRelation.ParentEntryId = parentEntryId;
            newEntryRelation.ChildEntryId = childEntryId;
            newEntryRelation.RelationTypeId = relationTypeId;
            newEntryRelation.Quantity = quantity;
            newEntryRelation.GroupName = groupName;
            newEntryRelation.SortOrder = entryRelationSortOrder;

            CatalogRelationDto.CatalogNodeRelationRow newNodeRelation = dto.CatalogNodeRelation.NewCatalogNodeRelationRow();
            int catalogId = target.GetCatalogNodeDto("Default API Testing Code").CatalogNode[0].CatalogId;
            int parentNodeId = target.GetCatalogNodeDto("Default API Testing Code").CatalogNode[0].CatalogNodeId;
            // Child's ParentNodeId should = parentNodeId in CatalogNode table?
            int childNodeId = target.GetCatalogNodeDto("API Testing 9").CatalogNode[0].CatalogNodeId;
            int nodeRelationSortOrder = 0;

            newNodeRelation.CatalogId = catalogId;
            newNodeRelation.ParentNodeId = parentNodeId;
            newNodeRelation.ChildNodeId = childNodeId;
            newNodeRelation.SortOrder = nodeRelationSortOrder;

            CatalogRelationDto.CatalogItemAssetRow newItemAsset = dto.CatalogItemAsset.NewCatalogItemAssetRow();
            int itemAssetCatalogNodeId = 105;
            int itemAssetCatalogEntryId = 3705;        // Confirmed no FK constraints enforced on creation
            string assetType = "API Tester file";
            string assetKey = "API tester " + rand.Next(0, 12);
            string itemAssetGroupName = "AUI";
            int itemAssetSortOrder = 0;

            newItemAsset.CatalogNodeId = itemAssetCatalogNodeId;
            newItemAsset.CatalogEntryId = itemAssetCatalogEntryId;
            newItemAsset.AssetType = assetType;
            newItemAsset.AssetKey = assetKey;
            newItemAsset.GroupName = itemAssetGroupName;
            newItemAsset.SortOrder = itemAssetSortOrder;

            CatalogRelationDto.NodeEntryRelationRow newNodeEntryRelation = dto.NodeEntryRelation.NewNodeEntryRelationRow();
            int nodeEntryRelationCatalogId = target.GetCatalogDto(5).Catalog[0].CatalogId;                                               // PK, FK
            int nodeEntryRelationCatalogEntryId = target.GetCatalogEntryDto("default test code").CatalogEntry[0].CatalogEntryId;         // PK, FK
            int nodeEntryRelationCatalogNodeId = target.GetCatalogNodeDto("Default API Testing Code").CatalogNode[0].CatalogNodeId;      // PK, FK
            int nodeEntryRelationSortOrder = 0;

            newNodeEntryRelation.CatalogId = nodeEntryRelationCatalogId;
            newNodeEntryRelation.CatalogEntryId = nodeEntryRelationCatalogEntryId;
            newNodeEntryRelation.CatalogNodeId = nodeEntryRelationCatalogNodeId;
            newNodeEntryRelation.SortOrder = nodeEntryRelationSortOrder;

            // Add rows
            dto.CatalogEntryRelation.AddCatalogEntryRelationRow(newEntryRelation);
            dto.CatalogNodeRelation.AddCatalogNodeRelationRow(newNodeRelation);
            dto.CatalogItemAsset.AddCatalogItemAssetRow(newItemAsset);
            dto.NodeEntryRelation.AddNodeEntryRelationRow(newNodeEntryRelation);
            target.SaveCatalogRelationDto(dto);

            CatalogRelationDto dto2 = target.GetCatalogRelationDto(5, 105, 3705, groupName, new CatalogRelationResponseGroup());

            #region Catalog entry relation test
            this.TestCategory = "Catalog entry relation";
            CatalogRelationDto.CatalogEntryRelationRow catalogEntryRelationRow = null;
            int catalogEntryRelationIndex = 0;
            try
            {
                catalogEntryRelationRow = dto2.CatalogEntryRelation[0];
                for (i = 0; i < dto2.CatalogEntryRelation.Count; i++)
                {
                    if (catalogEntryRelationRow.ParentEntryId == parentEntryId)
                    {
                        catalogEntryRelationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogEntryRelation.Count)
                failMessage(true);

            if (catalogEntryRelationRow.ChildEntryId != childEntryId)
                failMessage("child entry ID");
            if (!catalogEntryRelationRow.RelationTypeId.Equals(relationTypeId))
                failMessage("relation type ID");
            if (catalogEntryRelationRow.Quantity != quantity)
                failMessage("quantity");
            if (!catalogEntryRelationRow.GroupName.Equals(groupName))
                failMessage("group name");
            if (catalogEntryRelationRow.SortOrder != entryRelationSortOrder)
                failMessage("sort order");

            dto2.CatalogEntryRelation[catalogEntryRelationIndex].Delete();
            target.SaveCatalogRelationDto(dto2);

            dto2 = target.GetCatalogRelationDto(5, 105, 3705, groupName, new CatalogRelationResponseGroup());
            try
            {
                for (i = 0; i < dto2.CatalogEntryRelation.Count; i++)
                {
                    catalogEntryRelationRow = dto2.CatalogEntryRelation[i];
                    if (catalogEntryRelationRow.ParentEntryId == parentEntryId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogEntryRelation.Count)
                Assert.IsTrue(true);
            #endregion

            #region Catalog node relation test
            this.TestCategory = "Catalog node relation";
            CatalogRelationDto.CatalogNodeRelationRow catalogNodeRelationRow = null;
            int catalogNodeRelationIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogNodeRelation.Count; i++)
                {
                    catalogNodeRelationRow = dto2.CatalogNodeRelation[i];
                    if (catalogNodeRelationRow.CatalogId == catalogId)
                    {
                        catalogNodeRelationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogNodeRelation.Count)
                failMessage(true);

            if (catalogNodeRelationRow.ParentNodeId != parentNodeId)
                failMessage("parent node ID");
            if (catalogNodeRelationRow.ChildNodeId != childNodeId)
                failMessage("child node ID");
            if (catalogNodeRelationRow.SortOrder != nodeRelationSortOrder)
                failMessage("sort order");

            dto2.CatalogNodeRelation[catalogNodeRelationIndex].Delete();
            target.SaveCatalogRelationDto(dto2);

            dto2 = target.GetCatalogRelationDto(5, 105, 3705, groupName, new CatalogRelationResponseGroup());
            try
            {
                for (i = 0; i < dto2.CatalogNodeRelation.Count; i++)
                {
                    catalogNodeRelationRow = dto2.CatalogNodeRelation[i];
                    if (catalogNodeRelationRow.CatalogId == catalogId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogNodeRelation.Count)
                Assert.IsTrue(true);
            #endregion

            #region Node entry relation test
            this.TestCategory = "Node-entry relation";
            CatalogRelationDto.NodeEntryRelationRow nodeEntryRelationRow = null;
            int nodeEntryRelationIndex = 0;
            try
            {
                for (i = 0; i < dto2.NodeEntryRelation.Count; i++)
                {
                    nodeEntryRelationRow = dto2.NodeEntryRelation[i];
                    if (nodeEntryRelationRow.CatalogId == nodeEntryRelationCatalogId)
                    {
                        nodeEntryRelationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.NodeEntryRelation.Count)
                failMessage(true);

            if (nodeEntryRelationRow.CatalogEntryId != nodeEntryRelationCatalogEntryId)
                failMessage("catalog entry ID");
            if (nodeEntryRelationRow.CatalogNodeId != nodeEntryRelationCatalogNodeId)
                failMessage("catalog node ID");
            if (nodeEntryRelationRow.SortOrder != nodeEntryRelationSortOrder)
                failMessage("sort order");

            dto2.NodeEntryRelation[nodeEntryRelationIndex].Delete();
            target.SaveCatalogRelationDto(dto2);

            dto2 = target.GetCatalogRelationDto(5, 105, 3705, groupName, new CatalogRelationResponseGroup());
            try
            {
                for (i = 0; i < dto2.NodeEntryRelation.Count; i++)
                {
                    nodeEntryRelationRow = dto2.NodeEntryRelation[i];
                    if (nodeEntryRelationRow.CatalogId == catalogId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.NodeEntryRelation.Count)
                Assert.IsTrue(true);
            #endregion

            #region Catalog item asset test
            this.TestCategory = "Catalog item asset";
            dto2 = target.GetCatalogRelationDto(assetKey);
            CatalogRelationDto.CatalogItemAssetRow catalogItemAssetRow = null;
            int catalogItemAssetIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogNodeId)
                    {
                        catalogItemAssetIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                failMessage(true);

            if (catalogItemAssetRow.CatalogEntryId != itemAssetCatalogEntryId)
                failMessage("catalog entry ID");
            if (!catalogItemAssetRow.AssetType.Equals(assetType))
                failMessage("asset type");
            if (!catalogItemAssetRow.AssetKey.Equals(assetKey))
                failMessage("asset key");
            if (!catalogItemAssetRow.GroupName.Equals(itemAssetGroupName))
                failMessage("group name");
            if (catalogItemAssetRow.SortOrder != itemAssetSortOrder)
                failMessage("sort order");

            dto2.CatalogItemAsset[catalogItemAssetIndex].Delete();
            target.SaveCatalogRelationDto(dto2);

            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogNodeId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                Assert.IsTrue(true);
            #endregion
        }

        /// <summary>
        /// A test for SaveCatalogNode. CatalogItemAsset and CatalogItemSeo dependent on CatalogEntry.
        /// As of 3/25/09 12:30 PM, CatalogEntry does not work and therefore cannot procede with this unit test
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCatalogNode()
        {
            ICatalogSystem target = CatalogContext.Current;
            //CatalogNodeDto dto = new CatalogNodeDto(); // TODO: Initialize to an appropriate value
            CatalogNodeDto dto = target.GetCatalogNodeDto(105);
            int existingCatalogId = 1;
            dto = target.GetCatalogNodesDto(existingCatalogId);

            // Set paraemeters
            // Catalog node
            // Catalog node needs 
            int catalogId = existingCatalogId;      //FK
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddHours(2.53);
            string name = "Testing API";
            string templateName = "NodeInfoTemplate";
            string code = "API Testing " + rand.Next(0, 20).ToString();
            int parentNodeId = 999; // Need to be existing nodes or 0?  Or can it be arbitrary?
            int metaClassId = 3;
            bool isActive = false;
            int sortOrder = 0;
            dto.CatalogNode.AddCatalogNodeRow
                (catalogId, start, end, name, templateName, code, parentNodeId, metaClassId, sortOrder, isActive, AppContext.Current.ApplicationId);
            target.SaveCatalogNode(dto);

            #region Catalog node test
            CatalogNodeDto dto2 = target.GetCatalogNodeDto(code);
            CatalogNodeDto.CatalogNodeRow catalogNodeRow = null;
            this.TestCategory = "Catalog node";
            try
            {
                for (i = 0; i < dto2.CatalogNode.Count; i++)
                {
                    catalogNodeRow = dto2.CatalogNode[i];
                    if (catalogNodeRow.Name.Equals(name))
                    {
                        index = i;
                        break;
                    }
                }

                if (i >= dto2.CatalogNode.Count)
                    failMessage(true);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Null") || e.Message.Contains("null"))
                    failMessage(true);
            }

            if (catalogNodeRow.CatalogId != catalogId)
                failMessage("catalog ID");
            if (!catalogNodeRow.StartDate.Equals(start))
                failMessage("start date");
            if (!catalogNodeRow.EndDate.Equals(end))
                failMessage("end date");
            if (!catalogNodeRow.Name.Equals(name))
                failMessage("name");
            if (!catalogNodeRow.TemplateName.Equals(templateName))
                failMessage("template name");
            if (!catalogNodeRow.Code.Equals(code))
                failMessage("code");
            if (catalogNodeRow.ParentNodeId != parentNodeId)
                failMessage("parent node ID");
            if (catalogNodeRow.MetaClassId != metaClassId)
                failMessage("meta class ID");
            if (catalogNodeRow.SortOrder != sortOrder)
                failMessage("sort order");
            if (catalogNodeRow.IsActive != isActive)
                failMessage("active status");

            #endregion

            // Catalog item asset
            int itemAssetCatalogNodeId = index;
            int itemAssetCatalogEntryId = 3705;
            string assetType = "API Tester file";
            string assetKey = "API tester " + rand.Next(0, 12);
            string itemAssetGroupName = "AUI";
            int itemAssetSortOrder = 0;

            CatalogNodeDto.CatalogItemAssetRow catalogItemAssetRow = dto.CatalogItemAsset.NewCatalogItemAssetRow();
            catalogItemAssetRow.AssetKey = assetKey;
            catalogItemAssetRow.AssetType = assetType;
            catalogItemAssetRow.CatalogEntryId = itemAssetCatalogEntryId;
            catalogItemAssetRow.CatalogNodeId = itemAssetCatalogNodeId;
            catalogItemAssetRow.SortOrder = itemAssetSortOrder;
            catalogItemAssetRow.GroupName = itemAssetGroupName;
            //catalogItemAssetRow.CatalogNodeRow = (CatalogNodeDto.CatalogNodeRow)target.GetCatalogNodeDto(105).CatalogNode.Rows[0];
            dto.CatalogItemAsset.AddCatalogItemAssetRow(catalogItemAssetRow);
            target.SaveCatalogNode(dto);

            #region Catalog item asset
            CatalogNodeResponseGroup response = new CatalogNodeResponseGroup();
            response.ResponseGroups = response.ResponseGroups | CatalogNodeResponseGroup.ResponseGroup.Assets;
            dto2 = target.GetCatalogNodeDto(itemAssetCatalogNodeId, response);
            int catalogItemAssetIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogNodeId)
                    {
                        catalogItemAssetIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                failMessage(true);

            if (catalogItemAssetRow.CatalogEntryId != itemAssetCatalogEntryId)
                failMessage("catalog entry ID");
            if (!catalogItemAssetRow.AssetType.Equals(assetType))
                failMessage("asset type");
            if (!catalogItemAssetRow.AssetKey.Equals(assetKey))
                failMessage("asset key");
            if (!catalogItemAssetRow.GroupName.Equals(itemAssetGroupName))
                failMessage("group name");
            if (catalogItemAssetRow.SortOrder != itemAssetSortOrder)
                failMessage("sort order");

            #endregion

            // Catalog item SEO
            string languageCode = "ff-ix";                              // PK
            int itemSeoCatalogNodeId = index;                             // FK
            int itemSeoCatalogEntryId = 3705;
            string uri = "ff-guide-complete.aspx";                      // PK
            string title = "Complete Final Fantasy SERIES Guide";
            string description = "The ultimate walkthrough for every Final Fantasy game to ever have existed!! PWNAGE!";
            string keywords = "The ultimate SEO optiming keywords";

            CatalogNodeDto.CatalogItemSeoRow catalogItemSeoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
            catalogItemSeoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            catalogItemSeoRow.CatalogEntryId = itemSeoCatalogEntryId;
            catalogItemSeoRow.CatalogNodeId = itemSeoCatalogNodeId;
            catalogItemSeoRow.Description = description;
            catalogItemSeoRow.Keywords = keywords;
            catalogItemSeoRow.LanguageCode = languageCode;
            catalogItemSeoRow.Title = title;
            catalogItemSeoRow.Uri = uri;
            dto.CatalogItemSeo.AddCatalogItemSeoRow(catalogItemSeoRow);
            target.SaveCatalogNode(dto);



            #region Catalog item SEO
            dto2 = target.GetCatalogNodeDto(uri, languageCode);
            int catalogItemSeoIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogItemSeo.Count; i++)
                {
                    catalogItemSeoRow = dto2.CatalogItemSeo[i];
                    if (catalogItemSeoRow.CatalogNodeId == itemSeoCatalogNodeId)
                    {
                        catalogItemSeoIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogItemSeo.Count)
                failMessage(true);

            if (catalogItemSeoRow.CatalogEntryId != itemSeoCatalogEntryId)
                failMessage("catalog entry ID");
            #endregion

            // Remove added row and check for removal
            dto2.CatalogNode[index].Delete();
            target.SaveCatalogNode(dto2);

            dto2 = target.GetCatalogNodeDto(code);
            try
            {
                for (i = 0; i < dto2.CatalogNode.Count; i++)
                {
                    catalogNodeRow = dto2.CatalogNode[i];
                    if (catalogNodeRow.Name.Equals(name))
                        failMessage(false);
                }

                if (i >= dto2.CatalogNode.Count)
                    Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Null") || e.Message.Contains("null"))
                    Assert.IsTrue(true);
                if (e.Message.Contains("Fail!"))
                    failMessage(false);
            }



            dto2.CatalogItemSeo[catalogItemSeoIndex].Delete();
            target.SaveCatalogNode(dto2);

            try
            {
                for (i = 0; i < dto2.CatalogItemSeo.Count; i++)
                {
                    catalogItemSeoRow = dto2.CatalogItemSeo[i];
                    if (catalogItemSeoRow.CatalogNodeId == itemSeoCatalogNodeId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogItemSeo.Count)
                Assert.IsTrue(true);


            dto2.CatalogItemAsset[catalogItemAssetIndex].Delete();
            target.SaveCatalogNode(dto2);

            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogNodeId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                Assert.IsTrue(true);
        }

        /// <summary>
        /// A test for SaveCatalogEntry. 
        /// Stored procedure error after deleting new row
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCatalogEntry()
        {

            ICatalogSystem target = CatalogContext.Current;

            CatalogEntryResponseGroup response = new CatalogEntryResponseGroup();
            response.ResponseGroups = response.ResponseGroups | CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull;

            /*
            CatalogSystem_UnitTest_CatalogEntry():
            - GetCatalogEntriesDto(existingCatalogId) returns either:
                - empty DTO or DTO containing single element added
                - When deleting the single item from DTO, in admin.Save(),
                    if CatalogEntry.Count == 0 (which is true after deleting row and accepting change),
                    then function returns immediately instead of actually making a change to the database
             */

            // Need prior knowledge of existing catalog to get catalog entries
            int existingCatalogId = Int32.MinValue;

            CatalogDto catalogs = target.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                existingCatalogId = catalog.CatalogId;
                break;
            }

            Assert.IsFalse(existingCatalogId == Int32.MinValue, "Could not retrieve catalog");

            CatalogEntryDto dto = target.GetCatalogEntriesDto(existingCatalogId, response);
            //CatalogEntryDto dto = new CatalogEntryDto();

            // Catalog entry
            int catalogId = existingCatalogId;
            string entryName = "API Tester";
            string templateName = "API Testing Template";
            string code = "some code 3";
            string classTypeID = "classTypeID";
            int metaClassId = 2; // Arbitrary
            bool active = false;
            // Add column data
            CatalogEntryDto.CatalogEntryRow catalogEntryRow = dto.CatalogEntry.NewCatalogEntryRow();
            catalogEntryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            catalogEntryRow.CatalogId = catalogId;
            catalogEntryRow.StartDate = DateTime.Today;
            catalogEntryRow.EndDate = DateTime.Today;
            catalogEntryRow.Name = entryName;
            catalogEntryRow.TemplateName = templateName;
            catalogEntryRow.Code = code;
            catalogEntryRow.ClassTypeId = classTypeID;
            catalogEntryRow.MetaClassId = metaClassId;
            catalogEntryRow.IsActive = active;
            //newRow.SerializedData = null; // <- NULL by default
            dto.CatalogEntry.AddCatalogEntryRow(catalogEntryRow);
            target.SaveCatalogEntry(dto);


            // Catalog association
            #region Set parameters other than Catalog Entry (commented out until entry works)
            /*
            int newEntryId = existingCatalogEntry;           // FK
            string newAssociationName = "API_Association";
            string newAssociationDescription = "API testing association";
            int newSortOrder = 1;

            CatalogEntryDto.CatalogAssociationRow catalogAssociationRow = dto.CatalogAssociation.NewCatalogAssociationRow();
            catalogAssociationRow.CatalogEntryId = newEntryId;
            catalogAssociationRow.AssociationName = newAssociationName;
            catalogAssociationRow.AssociationDescription = newAssociationDescription;
            catalogAssociationRow.SortOrder = newSortOrder;
            dto.CatalogAssociation.AddCatalogAssociationRow(catalogAssociationRow);
            target.SaveCatalogEntry(dto);

            // Catalog item asset
            int itemAssetCatalogNodeId = 105;
            int itemAssetCatalogEntryId = existingCatalogEntry;
            string assetType = "API Tester file";
            string assetKey = "API tester " + rand.Next(0, 12);
            string itemAssetGroupName = "AUI";
            int itemAssetSortOrder = 0;

            CatalogEntryDto.CatalogItemAssetRow catalogItemAssetRow = dto.CatalogItemAsset.NewCatalogItemAssetRow();
            catalogItemAssetRow.AssetKey = assetKey;
            catalogItemAssetRow.AssetType = assetType;
            catalogItemAssetRow.CatalogEntryId = itemAssetCatalogEntryId;
            catalogItemAssetRow.CatalogNodeId = itemAssetCatalogNodeId;
            catalogItemAssetRow.SortOrder = itemAssetSortOrder;
            catalogItemAssetRow.GroupName = itemAssetGroupName;
            dto.CatalogItemAsset.AddCatalogItemAssetRow(catalogItemAssetRow);
            target.SaveCatalogEntry(dto);

            // Catalog item SEO
            string languageCode = "ff-ix";                              // PK
            int itemSeoCatalogNodeId = 105;                             // FK
            int itemSeoCatalogEntryId = existingCatalogEntry;
            string uri = "ff-guide-complete.aspx";                      // PK
            string title = "Complete Final Fantasy SERIES Guide";
            string description = "The ultimate walkthrough for every Final Fantasy game to ever have existed!! PWNAGE!";
            string keywords = "The ultimate SEO optiming keywords";

            CatalogEntryDto.CatalogItemSeoRow catalogItemSeoRow = dto.CatalogItemSeo.NewCatalogItemSeoRow();
            catalogItemSeoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            catalogItemSeoRow.CatalogEntryId = itemSeoCatalogEntryId;
            catalogItemSeoRow.CatalogNodeId = itemSeoCatalogNodeId;
            catalogItemSeoRow.Description = description;
            catalogItemSeoRow.Keywords = keywords;
            catalogItemSeoRow.LanguageCode = languageCode;
            catalogItemSeoRow.Title = title;
            catalogItemSeoRow.Uri = uri;
            dto.CatalogItemSeo.AddCatalogItemSeoRow(catalogItemSeoRow);
            target.SaveCatalogEntry(dto);

            // Inventory
            string skuId = "Heaven's Cloud";
            decimal inStockQuantity = 10;
            decimal reservedQuantity = 0;
            decimal reOrderMinQuantity = 1;
            decimal preOrderQuantity = 1;
            decimal backOrderQuantity = 1;
            bool allowBackOrder = true;
            bool allowPreOrder = false;
            int inventoryStatus = 0;
            DateTime preOrderAvailabilityDate = DateTime.Today;
            DateTime backOrderAvailabilityDate = DateTime.Today;

            CatalogEntryDto.InventoryRow inventoryRow = dto.Inventory.NewInventoryRow();
            inventoryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
            inventoryRow.SkuId = skuId;
            inventoryRow.InStockQuantity = inStockQuantity;
            inventoryRow.ReservedQuantity = reservedQuantity;
            inventoryRow.ReorderMinQuantity = reOrderMinQuantity;
            inventoryRow.PreorderQuantity = preOrderQuantity;
            inventoryRow.BackorderQuantity = backOrderQuantity;
            inventoryRow.AllowBackorder = allowBackOrder;
            inventoryRow.AllowPreorder = allowPreOrder;
            inventoryRow.InventoryStatus = inventoryStatus;
            inventoryRow.PreorderAvailabilityDate = preOrderAvailabilityDate;
            inventoryRow.BackorderAvailabilityDate = backOrderAvailabilityDate;
            dto.Inventory.AddInventoryRow(inventoryRow);
            target.SaveCatalogEntry(dto);

            // Merchant
            Guid merchantId = new Guid("2038a8bc392034bcef9201a3b3e3e55f");        // PK. Must make it yourself
            string merchantName = "Weapons shop in Sector 7";
            Guid applicationId = AppContext.Current.ApplicationId;

            CatalogEntryDto.MerchantRow merchantRow = dto.Merchant.NewMerchantRow();
            merchantRow.MerchantId = merchantId;
            merchantRow.Name = merchantName;
            merchantRow.ApplicationId = applicationId;
            dto.Merchant.AddMerchantRow(merchantRow);
            target.SaveCatalogEntry(dto);

            // Sale price
            string itemCode = "Heaven's Cloud";     // PK, FK
            int saleType = 0;
            string saleCode = "Find it";
            DateTime startDate = DateTime.Today;
            string currency = "GIL";
            decimal salePriceMinQuantity = 1;
            decimal unitPrice = 30000;
            DateTime endDate = DateTime.Today;

            CatalogEntryDto.SalePriceRow salePriceRow = dto.SalePrice.NewSalePriceRow();
            salePriceRow.ItemCode = itemCode;
            salePriceRow.SaleType = saleType;
            salePriceRow.SaleCode = saleCode;
            salePriceRow.StartDate = startDate;
            salePriceRow.Currency = currency;
            salePriceRow.MinQuantity = salePriceMinQuantity;
            salePriceRow.UnitPrice = unitPrice;
            salePriceRow.EndDate = endDate;
            dto.SalePrice.AddSalePriceRow(salePriceRow);
            target.SaveCatalogEntry(dto);

            // Variation
            int variationCatalogEntryId = existingCatalogEntry;
            decimal listPrice = 20;
            int taxCategoryId = 0;
            bool trackInventory = false;
            Guid merchantIdFK = merchantId;     // FK
            int wareHouseId = 0;
            float weight = 10;
            int packageId = 0;
            decimal variationMinQuantity = 1;
            decimal variationMaxQuantity = 2;

            CatalogEntryDto.VariationRow variationRow = dto.Variation.NewVariationRow();
            variationRow.CatalogEntryId = variationCatalogEntryId;
            variationRow.ListPrice = listPrice;
            variationRow.TaxCategoryId = taxCategoryId;
            variationRow.TrackInventory = trackInventory;
            variationRow.MerchantId = merchantId;
            variationRow.WarehouseId = wareHouseId;
            variationRow.Weight = weight;
            variationRow.PackageId = packageId;
            variationRow.MinQuantity = variationMinQuantity;
            variationRow.MaxQuantity = variationMaxQuantity;
            dto.Variation.AddVariationRow(variationRow);
            target.SaveCatalogEntry(dto);

             */
            #endregion

            CatalogEntryDto dto2;

            #region Catalog entry test

            // API Testing Note: There is a missing stored procedure error when trying to save after deleting a row.

            this.TestCategory = "Catalog entry";
            //dto2 = target.GetCatalogEntryDto(code, response);
            dto2 = target.GetCatalogEntriesDto(existingCatalogId, response);
            //dto2 = target.GetCatalogEntriesDto(entryName, classTypeID);
            try
            {
                for (i = 0; i < dto2.CatalogEntry.Count; i++)
                {
                    catalogEntryRow = dto2.CatalogEntry[i];
                    if (catalogEntryRow.Name.Equals(entryName))
                    {
                        index = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogEntry.Count)
                failMessage(true);
            //dto2.CatalogEntry[0]
            if (catalogEntryRow.CatalogId != catalogId)
                failMessage("catalog ID");
            if (!catalogEntryRow.StartDate.Equals(DateTime.Today))
                failMessage("start date");
            if (!catalogEntryRow.EndDate.Equals(DateTime.Today))
                failMessage("end date");
            if (!catalogEntryRow.TemplateName.Equals(templateName))
                failMessage("template name");
            if (!catalogEntryRow.ClassTypeId.Equals(classTypeID))
                failMessage("class type ID");
            if (catalogEntryRow.MetaClassId != metaClassId)
                failMessage("meta class ID");
            if (catalogEntryRow.IsActive != active)
                failMessage("active status");
            try
            {
                if (catalogEntryRow.SerializedData.Length > 0)
                    failMessage("serialized data");
            }
            catch (Exception e)
            {
                // If it was a null exception, that's what should have happened anyways
                //  (given the serialized data was initialized to null)
                handleNullException(e, true, true);
                handleGenericFailException(e, "serialized data");
            }

            // Remove added row and check for removal
            dto2.CatalogEntry[index].Delete();
            // Stored procedure error at 2:40 PM, 4/14/09, test run on dev qa database
            // Could not find stored procedure 'mdpsp_avto_CatalogEntry_Get'
            // Fails at line 70 of Mediachase.Commerce.Storage.DataHelper
            // The parameters to the call are correct, however.
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntriesDto(existingCatalogId);
            CatalogEntryDto.CatalogEntryRow removed = null;
            try
            {
                for (i = 0; i < dto2.CatalogEntry.Count; i++)
                {
                    catalogEntryRow = dto2.CatalogEntry[i];
                    if (catalogEntryRow.Name.Equals(entryName))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogEntry.Count)
                Assert.IsTrue(true);

            #endregion

            #region Other stuff to test out after entry is good
            /*
            #region Catalog association test
            this.TestCategory = "Catalog association";
            dto2 = target.GetCatalogEntryDto(newEntryId, response);
            int catalogAssociationIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogAssociation.Count; i++)
                {
                    catalogAssociationRow = dto2.CatalogAssociation[i];
                    if (catalogAssociationRow.CatalogEntryId == newEntryId)
                    {
                        catalogAssociationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogAssociation.Count)
                failMessage(true);

            if (!catalogAssociationRow.AssociationName.Equals(newAssociationName))
                failMessage("association name");
            if (!catalogAssociationRow.AssociationDescription.Equals(newAssociationDescription))
                failMessage("association description");
            if (catalogAssociationRow.SortOrder != newSortOrder)
                failMessage("sort order");

            dto2.CatalogAssociation[catalogAssociationIndex].Delete();
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntryDto(newEntryId, response);
            try
            {
                for (i = 0; i < dto2.CatalogAssociation.Count; i++)
                {
                    catalogAssociationRow = dto2.CatalogAssociation[i];
                    if (catalogAssociationRow.CatalogEntryId != newEntryId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogAssociation.Count)
                Assert.IsTrue(true);
                
            #endregion

            #region Catalog item asset test
            this.TestCategory = "Category item asset";
            dto2 = target.GetCatalogEntryDto(itemAssetCatalogEntryId, response);
            int catalogItemAssetIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogNodeId)
                    {
                        catalogItemAssetIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                failMessage(true);

            if (!catalogItemAssetRow.AssetKey.Equals(assetKey))
                failMessage("asset key");
            if (!catalogItemAssetRow.AssetType.Equals(assetType))
                failMessage("asset type");
            if (catalogItemAssetRow.SortOrder != itemAssetSortOrder)
                failMessage("sort order");
            if (!catalogItemAssetRow.GroupName.Equals(itemAssetGroupName))
                failMessage("group name");

            dto2.CatalogItemAsset[catalogItemAssetIndex].Delete();
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntryDto(itemAssetCatalogEntryId, response);
            try
            {
                for (i = 0; i < dto2.CatalogItemAsset.Count; i++)
                {
                    catalogItemAssetRow = dto2.CatalogItemAsset[i];
                    if (catalogItemAssetRow.CatalogNodeId == itemAssetCatalogEntryId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogItemAsset.Count)
                Assert.IsTrue(true);

            #endregion

            #region Catalog item SEO test
            this.TestCategory = "Catalog item SEO";
            dto2 = target.GetCatalogEntryDto(itemSeoCatalogEntryId, response);
            int catalogItemSeoIndex = 0;
            try 
            {
                for (i = 0; i <= dto2.CatalogItemSeo.Count; i++)	
                {		
                    catalogItemSeoRow = dto2.CatalogItemSeo[i];
                    if (catalogItemSeoRow.CatalogNodeId == itemSeoCatalogNodeId)
                    {
                        catalogItemSeoIndex = i;
                        break;		
                    }	
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogItemSeo.Count)
                failMessage(true);

            if (!catalogItemSeoRow.LanguageCode.Equals(languageCode))
                failMessage("language code");
            if (!catalogItemSeoRow.Uri.Equals(uri))
                failMessage("URI");
            if (!catalogItemSeoRow.Title.Equals(title))
                failMessage("title");
            if (!catalogItemSeoRow.Description.Equals(description))
                failMessage("description");
            if (!catalogItemSeoRow.Keywords.Equals(keywords))
                failMessage("key words");

            dto2.CatalogItemSeo[catalogItemSeoIndex].Delete();
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntryDto(itemSeoCatalogEntryId, response);
            try
            {
                for (i = 0; i <= dto2.CatalogItemSeo.Count; i++)
                {
                    catalogItemSeoRow = dto2.CatalogItemSeo[i];
                    if (catalogItemSeoRow.CatalogNodeId == itemSeoCatalogNodeId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogItemSeo.Count)
                Assert.IsTrue(true);

            #endregion

            #region Inventory test
            this.TestCategory = "Inventory";
            dto2 = target.GetCatalogEntryDto(skuId, response);
            int inventoryIndex = 0;
            try {
                for (i = 0; i <= dto2.Inventory.Count; i++)
                {
                    inventoryRow = dto2.Inventory[i];
                    if (inventoryRow.SkuId.Equals(skuId))
                    {
                        inventoryIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.Inventory.Count)
                failMessage(true);
            
            if (inventoryRow.InStockQuantity != inStockQuantity)
                failMessage("in-stock quantity");
            if (inventoryRow.ReservedQuantity != reservedQuantity)
                failMessage("reserved quantity");
            if (inventoryRow.ReorderMinQuantity != reOrderMinQuantity)
                failMessage("re-order quantity");
            if (inventoryRow.PreorderQuantity != preOrderQuantity)
                failMessage("pre-order quantity");
            if (inventoryRow.BackorderQuantity != backOrderQuantity)
                failMessage("back-order quantity");
            if (inventoryRow.AllowBackorder != allowBackOrder)
                failMessage("status on allowing back-orders");
            if (inventoryRow.AllowPreorder != allowPreOrder)
                failMessage("status on allowing pre-orders");
            if (!inventoryRow.BackorderAvailabilityDate.Equals(backOrderAvailabilityDate))
                failMessage("back-order availability date");

            dto2.Inventory[inventoryIndex].Delete();
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntryDto(skuId, response);
            try
            {
                for (i = 0; i <= dto2.Inventory.Count; i++)
                {
                    inventoryRow = dto2.Inventory[i];
                    if (inventoryRow.SkuId.Equals(skuId))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.Inventory.Count)
                Assert.IsTrue(true);
            #endregion

            #region Merchant test (Variation test embedded)

            // API Testing Note: Deletion of a Variation row apparently also deletes the associated Merchant row to be deleted from
            //                      the DTO viewpoint, but the actual Merchant row remains in the table.

            this.TestCategory = "Merchant";
            dto2 = target.GetCatalogEntryDto(existingCatalogEntry, response);
            int merchantIndex = 0;
            try {
                for (i = 0; i < dto2.Merchant.Count; i++)
                {
                    merchantRow = dto2.Merchant[i];
                    if (merchantRow.MerchantId == merchantId)
                    {
                        merchantIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.Merchant.Count)
                failMessage(true);

            if (!merchantRow.Name.Equals(merchantName))
                failMessage("name");
            if (!merchantRow.ApplicationId.Equals(applicationId))
                failMessage("application ID");

            #region Variation test
            this.TestCategory = "Variation";
            CatalogEntryDto dto3 = target.GetCatalogEntryDto(variationCatalogEntryId, response);
            int variationIndex = 0;
            try
            {
                for (i = 0; i < dto3.Variation.Count; i++)
                {
                    variationRow = dto3.Variation[i];
                    if (variationRow.MerchantId.Equals(merchantIdFK))
                    {
                        variationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto3.Variation.Count)
                failMessage(true);

            if (variationRow.ListPrice != listPrice)
                failMessage("list price");
            if (variationRow.TaxCategoryId != taxCategoryId)
                failMessage("tax category ID");
            if (variationRow.TrackInventory != trackInventory)
                failMessage("decision on tracking inventory");
            if (variationRow.WarehouseId != wareHouseId)
                failMessage("warehouse ID");
            if (variationRow.Weight != weight)
                failMessage("weight");
            if (variationRow.PackageId != packageId)
                failMessage("package ID");
            if (variationRow.MinQuantity != variationMinQuantity)
                failMessage("minimum quantity");
            if (variationRow.MaxQuantity != variationMaxQuantity)
                failMessage("maximum quantity");

            dto3.Variation[variationIndex].Delete();
            target.SaveCatalogEntry(dto3);
            
            dto3 = target.GetCatalogEntryDto(variationCatalogEntryId, response);
            try
            {
                for (i = 0; i < dto3.Variation.Count; i++)
                {
                    variationRow = dto3.Variation[i];
                    if (variationRow.MerchantId.Equals(merchantIdFK))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto3.Variation.Count)
                Assert.IsTrue(true);
            #endregion

            dto2.Merchant[merchantIndex].Delete();
            target.SaveCatalogEntry(dto2);

            dto2 = target.GetCatalogEntryDto(existingCatalogEntry, response);

            try
            {
                for (i = 0; i < dto2.Merchant.Count; i++)
                {
                    merchantRow = dto2.Merchant[i];
                    if (merchantRow.MerchantId == merchantId)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.Merchant.Count)
                Assert.IsTrue(true);
            #endregion

            #region Sale price test
            this.TestCategory = "Sale price";
            dto2 = target.GetCatalogEntryDto(itemCode, response);
            int salePriceIndex = 0;
            try {
                for (i = 0; i < dto2.SalePrice.Count; i++)
                {
                    salePriceRow = dto2.SalePrice[i];
                    if (salePriceRow.ItemCode.Equals(itemCode))
                    {
                        salePriceIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.SalePrice.Count)
                failMessage(true);
            
            if (salePriceRow.SaleType != saleType)
                failMessage("sale type");
            if (!salePriceRow.SaleCode.Equals(saleCode))
                failMessage("sale code");
            if (!salePriceRow.StartDate.Equals(startDate))
                failMessage("start date");
            if (!salePriceRow.Currency.Equals(currency))
                failMessage("currency");
            if (salePriceRow.MinQuantity != salePriceMinQuantity)
                failMessage("minimumm quantity");
            if (salePriceRow.UnitPrice != unitPrice)
                failMessage("unit price");
            if (!salePriceRow.EndDate.Equals(endDate))
                failMessage("end date");

            dto2.SalePrice[salePriceIndex].Delete();
            target.SaveCatalogEntry(dto2);
            
            dto2 = target.GetCatalogEntryDto(itemCode, response);
            try
            {
                for (i = 0; i < dto2.SalePrice.Count; i++)
                {
                    salePriceRow = dto2.SalePrice[i];
                    if (salePriceRow.ItemCode.Equals(itemCode))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.SalePrice.Count)
                Assert.IsTrue(true);
            #endregion
            */
            #endregion
        }

        /// <summary>
        /// A test for SaveCatalogAssociation.
        /// Depends on CatalogEntry
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCatalogAssociation()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogAssociationDto dto = new CatalogAssociationDto();

            // Set parameters

            // Catalog association table

            // Add a Catalog entry
            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntriesDto(2);

            int[] catalogEntryId = { 3681, 3682, 3689, 3698 };
            int randNum = rand.Next() % catalogEntryId.Length;
            int newEntryId = catalogEntryId[randNum];           // FK
            string newAssociationName = "API_Association";
            string newAssociationDescription = "API testing association";
            int newSortOrder = 1;

            // Add new catalog association row in table
            CatalogAssociationDto.CatalogAssociationRow newRow =
                dto.CatalogAssociation.NewCatalogAssociationRow();
            newRow.CatalogEntryId = newEntryId;
            newRow.AssociationName = newAssociationName;
            newRow.AssociationDescription = newAssociationDescription;
            newRow.SortOrder = newSortOrder;
            dto.CatalogAssociation.AddCatalogAssociationRow(newRow);
            target.SaveCatalogAssociation(dto);

            #region Not actually executed - save new association type row (they are hard-coded)
            /*dto = target.GetCatalogAssociationDto(newAssociationName);
            CatalogAssociationDto.AssociationTypeRow newAssociationType = dto.AssociationType.NewAssociationTypeRow();
            newAssociationType.AssociationTypeId = "OPTIONAL";
            newAssociationType.Description = "API test data";
            dto.AssociationType.AddAssociationTypeRow(newAssociationType);
            target.SaveCatalogAssociation(dto);*/
            #endregion

            dto = target.GetCatalogAssociationDto(newAssociationName);
            // Catalog entry association table
            int catalogAssociationId = dto.CatalogAssociation[0].CatalogAssociationId;         // PK, FK
            int catalogentryId_CEA = dto.CatalogAssociation[0].CatalogEntryId;                 // PK, FK
            int sortOrder_CEA = 1;
            string associationTypeId = "OPTIONAL";                                             // FK

            // Add new catalog entry association row in table
            CatalogAssociationDto.CatalogEntryAssociationRow newCatalogEntryAssociationRow =
                dto.CatalogEntryAssociation.NewCatalogEntryAssociationRow();
            newCatalogEntryAssociationRow.CatalogAssociationId = catalogAssociationId;
            newCatalogEntryAssociationRow.CatalogEntryId = catalogentryId_CEA;
            newCatalogEntryAssociationRow.SortOrder = sortOrder_CEA;
            newCatalogEntryAssociationRow.AssociationTypeId = associationTypeId;
            dto.CatalogEntryAssociation.AddCatalogEntryAssociationRow(newCatalogEntryAssociationRow);
            target.SaveCatalogAssociation(dto);

            // Check that added catalog association row exists and is as was specified
            this.TestCategory = "Catalog association";
            CatalogAssociationDto dto2 = target.GetCatalogAssociationDto(newAssociationName);
            CatalogAssociationDto.CatalogAssociationRow row = null;
            try
            {
                for (i = 0; i < dto2.CatalogAssociation.Count; i++)
                {
                    row = dto2.CatalogAssociation[i];
                    if (row.AssociationName.Equals(newAssociationName))
                    {
                        index = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Null") || e.Message.Contains("null"))
                    failMessage(true);
            }
            if (i >= dto2.CatalogAssociation.Count)
                failMessage(true);

            if (row.CatalogEntryId != newEntryId)
                failMessage("catalog entry ID");
            if (!row.AssociationName.Equals(newAssociationName))
                failMessage("name");
            if (!row.AssociationDescription.Equals(newAssociationDescription))
                failMessage("description");
            if (row.SortOrder != newSortOrder)
                failMessage("sort order");

            // Check association type row
            this.TestCategory = "Association type";
            CatalogAssociationDto.AssociationTypeRow associationTypeRow = null;
            int associationTypeIndex = 0;
            try
            {
                for (i = 0; i < dto2.AssociationType.Count; i++)
                {
                    associationTypeRow = dto2.AssociationType[i];
                    if (associationTypeRow.AssociationTypeId.Equals("OPTIONAL"))
                    {
                        associationTypeIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.AssociationType.Count)
                failMessage(true);

            // Check that added catalog entry association row exists and is as was specified
            this.TestCategory = "Catalog entry association";
            CatalogAssociationDto.CatalogEntryAssociationRow addedCatalogEntryAssociation = null;
            int entryAssociationIndex = 0;
            try
            {
                for (i = 0; i < dto2.CatalogEntryAssociation.Count; i++)
                {
                    addedCatalogEntryAssociation = dto2.CatalogEntryAssociation[i];
                    if (addedCatalogEntryAssociation.AssociationTypeId == associationTypeId &&
                        addedCatalogEntryAssociation.CatalogAssociationId == catalogAssociationId &&
                        addedCatalogEntryAssociation.CatalogEntryId == catalogentryId_CEA &&
                        addedCatalogEntryAssociation.SortOrder == sortOrder_CEA)
                    {
                        entryAssociationIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.CatalogAssociation.Count)
                failMessage(true);

            // If tests all pass, remove catalog entry association row
            dto2.CatalogEntryAssociation[entryAssociationIndex].Delete();
            target.SaveCatalogAssociation(dto2);

            // Check for removal of catalog entry association row
            dto2 = target.GetCatalogAssociationDto(newAssociationName);
            CatalogAssociationDto.CatalogEntryAssociationRow removedCatalogEntryAssociation = null;
            try
            {
                for (i = 0; i < dto2.CatalogEntryAssociation.Count; i++)
                {
                    removedCatalogEntryAssociation = dto2.CatalogEntryAssociation[i];
                    if (removedCatalogEntryAssociation.AssociationTypeId == associationTypeId &&
                        removedCatalogEntryAssociation.CatalogAssociationId == catalogAssociationId &&
                        removedCatalogEntryAssociation.CatalogEntryId == catalogentryId_CEA &&
                        removedCatalogEntryAssociation.SortOrder == sortOrder_CEA)
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogAssociation.Count)
                Assert.IsTrue(true);

            // Remove association type row
            dto2.AssociationType[associationTypeIndex].Delete();
            target.SaveCatalogAssociation(dto2);

            // Check removal of association type row
            this.TestCategory = "Association type";
            dto2 = target.GetCatalogAssociationDto(newAssociationName);
            try
            {
                for (i = 0; i < dto2.AssociationType.Count; i++)
                {
                    associationTypeRow = dto2.AssociationType[i];
                    if (associationTypeRow.AssociationTypeId.Equals("MANDATORY"))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.AssociationType.Count)
                Assert.IsTrue(true);

            // If tests all pass, remove catalog association row
            dto2.CatalogAssociation[index].Delete();
            target.SaveCatalogAssociation(dto2);

            // Check for removal
            this.TestCategory = "Catalog association";
            dto2 = target.GetCatalogAssociationDto(newAssociationName);
            CatalogAssociationDto.CatalogAssociationRow removed = null;
            try
            {
                for (i = 0; i < dto2.CatalogAssociation.Count; i++)
                {
                    removed = dto2.CatalogAssociation[i];
                    if (removed.AssociationName.Equals(newAssociationName))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto2.CatalogAssociation.Count)
                Assert.IsTrue(true);
        }

        /// <summary>
        /// A test for SaveCatalog.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_SaveCatalog()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogDto dto = new CatalogDto(); // TODO: Initialize to an appropriate value
            bool enablePerm = ProfileConfiguration.Instance.EnablePermissions;


            #region Set parameters

            // Catalog table
            string name = "API Tester Catalog";
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1.2);
            string defaultCurrency = "gil";
            string weightBase = "kg";
            string defaultLanguage = "ff-games";
            bool isPrimary = true;
            bool isActive = false;
            DateTime created = DateTime.Today.AddDays(-1);
            DateTime modified = DateTime.Today;
            //string creatorId;
            //string modifierId;
            int sortOrder = 0;
            Guid applicationId = AppContext.Current.ApplicationId;

            // Add new rows
            //ProfileContext.Current.Profile.UserName;

            //CatalogRoles.CatalogAdminRole;

            CatalogDto.CatalogRow newRow = dto.Catalog.NewCatalogRow();
            newRow.Name = name;
            newRow.StartDate = startDate;
            newRow.EndDate = endDate;
            newRow.DefaultCurrency = defaultCurrency;
            newRow.WeightBase = weightBase;
            newRow.DefaultLanguage = defaultLanguage;
            newRow.IsPrimary = isPrimary;
            newRow.IsActive = isActive;
            newRow.Created = created;
            newRow.Modified = modified;
            //newRow.CreatorId = creatorId;
            //newRow.ModifierId = modifierId;
            newRow.SortOrder = sortOrder;
            newRow.ApplicationId = applicationId;
            dto.Catalog.AddCatalogRow(newRow);
            target.SaveCatalog(dto);

            #endregion

            #region Test added row for correctness
            CatalogDto dto2 = target.GetCatalogDto();
            CatalogDto.CatalogRow row = null;

            this.TestCategory = "Catalog";
            try
            {
                for (i = 0; i < dto2.Catalog.Count; i++)
                {
                    row = dto2.Catalog[i];
                    if (row.Name.Equals(name))
                    {
                        index = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto2.Catalog.Count)
                failMessage(true);

            if (!row.StartDate.Equals(startDate))
                failMessage("start date");
            if (!row.EndDate.Equals(endDate))
                failMessage("end date");
            if (!row.WeightBase.Equals(weightBase))
                failMessage("weight base");
            if (!row.DefaultLanguage.Equals(defaultLanguage))
                failMessage("default language");
            if (row.IsPrimary != isPrimary)
                failMessage("identity as primary");
            if (row.IsActive != isActive)
                failMessage("active status");
            if (!row.Created.Equals(created))
                failMessage("creation time");
            if (!row.Modified.Equals(modified))
                failMessage("modification time");
            // Should be null
            try
            {
                if (row.CreatorId.Length > 0)
                    failMessage("creator ID");
                if (row.ModifierId.Length > 0)
                    failMessage("modifier ID");
            }
            catch (Exception e)
            {
                // If it was a null exception, that's what should have happened anyways
                //  (given the serialized data was initialized to null)
                if (e.Message.Contains("Null") || e.Message.Contains("null"))
                    Assert.IsTrue(true);
                if (e.Message.Contains("creator ID"))
                    failMessage("creator ID");
                if (e.Message.Contains("modifier ID"))
                    failMessage("modifier ID");
            }
            if (row.SortOrder != sortOrder)
                failMessage("sort order");
            if (!row.ApplicationId.Equals(applicationId))
                failMessage("application ID");
            #endregion

            // Test the other tables in the DTO

            CatalogDto dto3 = target.GetCatalogDto();
            // Catalog language table
            int catalogID_catalogLanguage = dto3.Catalog.Select("Name = 'API Tester Catalog'")[0].Field<int>("CatalogId");          // PK, FK
            string languageCode = "ff-vii";    // PK
            CatalogDto.CatalogLanguageRow newCatalogLangRow = dto3.CatalogLanguage.NewCatalogLanguageRow();
            newCatalogLangRow.CatalogId = catalogID_catalogLanguage;
            newCatalogLangRow.LanguageCode = languageCode;

            #region Catalog security IS NOT USED
            /*
            // Catalog security table
            int catalogID_catalogSecurity = catalogID_catalogLanguage;  // FK
            string sid = CMSContext.Current.SiteId.ToString();
            string scope;
            byte[] allowMask = new byte[8];
            byte[] denyMask = new byte[8];
            CatalogDto.CatalogSecurityRow newCatalogSecurityRow = dto3.CatalogSecurity.NewCatalogSecurityRow();
            newCatalogSecurityRow.CatalogId = catalogID_catalogSecurity;
            newCatalogSecurityRow.AllowMask = allowMask;
            newCatalogSecurityRow.DenyMask = denyMask;
            */
            #endregion

            // Site catalog table
            int catalogID_siteCatalog = catalogID_catalogLanguage;  // FK
            Guid siteId = CMSContext.Current.SiteId;
            CatalogDto.SiteCatalogRow newSiteCatalogRow = dto3.SiteCatalog.NewSiteCatalogRow();
            newSiteCatalogRow.CatalogId = catalogID_siteCatalog;
            newSiteCatalogRow.SiteId = siteId;
            //CMSContext.Current.GetSitesDto(applicationId);

            // Add new rows
            dto3.CatalogLanguage.AddCatalogLanguageRow(newCatalogLangRow);
            //dto3.CatalogSecurity.AddCatalogSecurityRow(newCatalogSecurityRow);
            dto3.SiteCatalog.AddSiteCatalogRow(newSiteCatalogRow);
            target.SaveCatalog(dto3);

            // Check added rows
            dto3 = target.GetCatalogDto();
            this.TestCategory = "Catalog language";
            CatalogDto.CatalogLanguageRow catalogLanguageRow = null;
            int catalogLanguageIndex = 0;
            try
            {
                for (i = 0; i < dto3.CatalogLanguage.Count; i++)
                {
                    catalogLanguageRow = dto3.CatalogLanguage[i];
                    if (catalogLanguageRow.LanguageCode.Equals(languageCode))
                    {
                        catalogLanguageIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto3.CatalogLanguage.Count)
                failMessage(true);
            if (catalogLanguageRow.CatalogId != catalogID_catalogLanguage)
                failMessage("catalog ID");

            // Remove added catalog language row and test for removal (delete first since this references catalog row(s) and cascades)
            dto3.CatalogLanguage[catalogLanguageIndex].Delete();
            target.SaveCatalog(dto3);

            dto3 = target.GetCatalogDto();
            CatalogDto.CatalogLanguageRow removedCatalogLanguage = null;
            try
            {
                for (i = 0; i < dto3.CatalogLanguage.Count; i++)
                {
                    removedCatalogLanguage = dto3.CatalogLanguage[i];
                    if (removedCatalogLanguage.LanguageCode.Equals(languageCode))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto3.CatalogLanguage.Count)
                Assert.IsTrue(true);

            dto3 = target.GetCatalogDto();
            this.TestCategory = "Site catalog";
            CatalogDto.SiteCatalogRow siteCatalogRow = null;
            int siteCatalogIndex = 0;
            try
            {
                for (i = 0; i < dto3.SiteCatalog.Count; i++)
                {
                    siteCatalogRow = dto3.SiteCatalog[i];
                    if (siteCatalogRow.CatalogId == catalogID_siteCatalog)
                    {
                        siteCatalogIndex = i;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, true, false);
            }
            if (i >= dto3.SiteCatalog.Count)
                failMessage(true);
            if (!siteCatalogRow.SiteId.Equals(siteId))
                failMessage("site ID");
            // Remove added site catalog row and test for removal (delete first since this references catalog row(s) and cascades)
            CatalogDto.SiteCatalogRow removedSiteCatalog = null;
            dto3.SiteCatalog[siteCatalogIndex].Delete();
            target.SaveCatalog(dto3);
            dto3 = target.GetCatalogDto();
            try
            {
                for (i = 0; i < dto3.SiteCatalog.Count; i++)
                {
                    removedSiteCatalog = dto3.SiteCatalog[i];
                    if (removedSiteCatalog.CatalogId == catalogID_siteCatalog)
                    {
                        failMessage(false);
                    }
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }
            if (i >= dto3.SiteCatalog.Count)
                Assert.IsTrue(true);

            // Remove added catalog row and test for removal
            dto2.Catalog[index].Delete();
            target.SaveCatalog(dto2);

            dto2 = target.GetCatalogDto();
            CatalogDto.CatalogRow removed = null;
            try
            {
                for (i = 0; i < dto2.Catalog.Count; i++)
                {
                    removed = dto2.Catalog[i];
                    if (removed.Name.Equals(name))
                        failMessage(false);
                }
            }
            catch (Exception e)
            {
                handleNullException(e, false, true);
                handleGenericFailException(e, false);
            }

            if (i >= dto2.Catalog.Count)
                Assert.IsTrue(true);
        }

        #endregion Catalog system save unit tests

        #region Catalog system get unit tests

        /// <summary>
        /// Unit test for getting Merchants.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetMerchantsTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetMerchantsDto();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CurrencyDto.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCurrencyDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            
            CurrencyDto actual;
            actual = target.GetCurrencyDto();

            string[] currencyNames = { "Australian dollar", "Canadian dollar", "Swiss franc", "Czech koruna",
                                         "Danish krone", "Estonian kroon", "Euro", "Pound sterling", "Hungarian forint",
                                         "Iceland krona", "Japanese yen", "South Korean won", "Lithuanian litas",
                                         "Latvian lats", "Norwegian krone", "New Zeland dollar", "Polish zloty",
                                         "Romanian leu", "Ruble", "Swedish krona", "Slovak koruna", "Turkish lira",
                                         "US dollars", "South African rand" };

            string[] currencyCode = { "AUD", "CAD", "CHF", "CZK", "DKK", "EEK", "EUR", "GBP", "HUF", "ISK", "JPY", "KRW",
                                        "LTL", "LVL", "NOK", "NZD", "PLN", "RON", "RUB", "SEK", "SKK", "TRY", "USD", "ZAR" };

            for (int i = 0; i < currencyNames.Length; i++)
            {
                Assert.AreEqual(currencyNames[i], actual.Currency[i].Name);
                Assert.AreEqual(currencyCode[i], actual.Currency[i].CurrencyCode);
                Assert.AreEqual(i + 1, actual.Currency[i].CurrencyId);
            }
            
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogRelationDto by catalog ID, catalog node ID, catalog entry ID, group name, and response group.
        /// Test assumes that getting CatalogRelatinDto by asset key works (CatalogSystem_GetCatalogRelationDtoTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogRelationDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            int catalogNodeId = 0; // TODO: Initialize to an appropriate value
            int catalogEntryId = 0; // TODO: Initialize to an appropriate value
            string groupName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogRelationResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogRelationDto expected = target.GetCatalogRelationDto("asset key"); // TODO: Initialize to an appropriate value
            CatalogRelationDto actual;
            actual = target.GetCatalogRelationDto(catalogId, catalogNodeId, catalogEntryId, groupName, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogRelationDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogRelationDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string assetKey = string.Empty; // TODO: Initialize to an appropriate value
            CatalogRelationDto expected = null; // TODO: Initialize to an appropriate value
            CatalogRelationDto actual;
            actual = target.GetCatalogRelationDto(assetKey);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog name.
        /// Test assuming that getting CatalogNodesDto by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest7()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogDto catalog = target.GetCatalogDto();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodeDto expected = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);

                CatalogNodeDto actual = target.GetCatalogNodesDto(catalog.Catalog[i].Name);

                // Getting CatalogNodesDto by name only gets direct children of nodes specified by catalog name

                bool nestedNodes = false;

                if (actual.CatalogNode.Count < expected.CatalogNode.Count)
                    nestedNodes = true;

                if (nestedNodes)
                {
                    int j = 0;
                    int k = 0;
                    for (; j < actual.CatalogNode.Count; j++)
                    {
                        for (; k < expected.CatalogNode.Count; k++)
                        {
                            if (actual.CatalogNode[j].ParentNodeId == expected.CatalogNode[k].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[j], expected.CatalogNode[k]));
                                k++;
                                break;
                            }
                        }
                    }
                    Assert.AreEqual(j, actual.CatalogNode.Count);
                }

                else
                {
                    Assert.IsTrue(compareCatalogNodeDto(expected, actual));
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog name, parent node code, and response group.
        /// Test assuming that getting CatalogNodes by catalog name, parent node code, and response group works correctly 
        /// since the test methodology is borrowed from it (CatalogSystem_UnitTest_GetCatalogNodesTest5()).
        /// </summary>
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest6()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            string parentNodeCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();
            CatalogNodeDto expected = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;

            CatalogDto catalog = target.GetCatalogDto();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);

                for (int j = 0; j < expected.CatalogNode.Count; j++)
                {
                    parentNodeCode = string.Empty;
                    if (expected.CatalogNode[j].ParentNodeId == 0) // ignore if parent node is the catalog itself (already tested)
                    {
                        ;
                    }
                    else // if parent node is another node
                    {
                        for (int n = 0; n < expected.CatalogNode.Count; n++)
                        {
                            if (expected.CatalogNode[j].ParentNodeId == expected.CatalogNode[n].CatalogNodeId)
                            {
                                parentNodeCode = expected.CatalogNode[n].Code;
                                break;
                            }
                        }

                        actual = target.GetCatalogNodesDto(catalog.Catalog[i].Name, parentNodeCode, responseGroup);
                        int k = 0;
                        int l = 0;
                        for (; k < actual.CatalogNode.Count; k++)
                        {
                            for (; l < expected.CatalogNode.Count; l++)
                            {
                                if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                                {
                                    Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                    l++;
                                    j++; // No need to check it again at the iteration
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog name and parent node code.
        /// Test assuming that getting CatalogNodes by catalog name, parent node code, and response group works correctly 
        /// since the test methodology is borrowed from it (CatalogSystem_UnitTest_GetCatalogNodesTest5()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest5()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            string parentNodeCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogNodeDto expected = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;

            CatalogDto catalog = target.GetCatalogDto();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);

                for (int j = 0; j < expected.CatalogNode.Count; j++)
                {
                    parentNodeCode = string.Empty;
                    if (expected.CatalogNode[j].ParentNodeId == 0) // ignore if parent node is the catalog itself (already tested)
                    {
                        ;
                    }
                    else // if parent node is another node
                    {
                        for (int n = 0; n < expected.CatalogNode.Count; n++)
                        {
                            if (expected.CatalogNode[j].ParentNodeId == expected.CatalogNode[n].CatalogNodeId)
                            {
                                parentNodeCode = expected.CatalogNode[n].Code;
                                break;
                            }
                        }

                        actual = target.GetCatalogNodesDto(catalog.Catalog[i].Name, parentNodeCode);
                        int k = 0;
                        int l = 0;
                        for (; k < actual.CatalogNode.Count; k++)
                        {
                            for (; l < expected.CatalogNode.Count; l++)
                            {
                                if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                                {
                                    Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                    l++;
                                    j++; // No need to check it again at the iteration
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog Id and parent node Id.
        /// Test assuming that getting CatalogNodes by catalog ID, parent node Id, and response group works correctly 
        /// (CatalogSystem_UnitTest_GetCatalogNodesTest4()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest4()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            CatalogNodeDto expected = null;
            CatalogNodeDto actual;

            CatalogDto catalog = target.GetCatalogDto();
            System.Collections.Generic.List<int> parentNodes = new System.Collections.Generic.List<int>();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = catalog.Catalog[i].CatalogId;
                expected = target.GetCatalogNodesDto(catalogId);
                int currentNode;
                bool duplicate = false;

                // Find all parent node ID's
                for (int j = 0; j < expected.CatalogNode.Count; j++)
                {
                    currentNode = expected.CatalogNode[j].ParentNodeId;
                    duplicate = false;
                    if (j == 0) // if looking at first catalog node, automatically add node
                    {
                        parentNodes.Add(currentNode);
                        continue;
                    }
                    foreach (int existing in parentNodes)
                    {
                        if (currentNode == existing)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate)
                        parentNodes.Add(currentNode);
                }

                foreach (int parentNodeId in parentNodes)
                {
                    actual = target.GetCatalogNodesDto(catalogId, parentNodeId);

                    int k = 0;
                    int l = 0;
                    for (; k < actual.CatalogNode.Count; k++)
                    {
                        for (; l < expected.CatalogNode.Count; l++)
                        {
                            if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                l++;
                                break;
                            }
                        }
                    }
                }
                parentNodes.Clear();
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog Id, parent node Id, and response group.
        /// Test assuming that getting CatalogNodes by catalog ID, parent node Id, and response group works correctly 
        /// since the test methodology is borrowed from it (CatalogSystem_UnitTest_GetCatalogNodesTest4()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();
            CatalogNodeDto expected = null;
            CatalogNodeDto actual;

            CatalogDto catalog = target.GetCatalogDto();
            System.Collections.Generic.List<int> parentNodes = new System.Collections.Generic.List<int>();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = catalog.Catalog[i].CatalogId;
                expected = target.GetCatalogNodesDto(catalogId);
                int currentNode;
                bool duplicate = false;

                // Find all parent node ID's
                for (int j = 0; j < expected.CatalogNode.Count; j++)
                {
                    currentNode = expected.CatalogNode[j].ParentNodeId;
                    duplicate = false;
                    if (j == 0) // if looking at first catalog node, automatically add node
                    {
                        parentNodes.Add(currentNode);
                        continue;
                    }
                    foreach (int existing in parentNodes)
                    {
                        if (currentNode == existing)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate)
                        parentNodes.Add(currentNode);
                }

                foreach (int parentNodeId in parentNodes)
                {
                    actual = target.GetCatalogNodesDto(catalogId, parentNodeId, responseGroup);

                    int k = 0;
                    int l = 0;
                    for (; k < actual.CatalogNode.Count; k++)
                    {
                        for (; l < expected.CatalogNode.Count; l++)
                        {
                            if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                l++;
                                break;
                            }
                        }
                    }
                }
                parentNodes.Clear();
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog Id and response group.
        /// Test assuming that getting CatalogNodesDto by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;

            CatalogDto catalog = target.GetCatalogDto();

            int catalogId = 0; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();

            CatalogNodeDto expected;
            CatalogNodeDto actual;

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = catalog.Catalog[i].CatalogId;
                expected = target.GetCatalogNodesDto(catalogId);
                actual = target.GetCatalogNodesDto(catalogId, responseGroup);
                Assert.IsTrue(compareCatalogNodeDto(expected, actual));
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog name and response group.
        /// Test assuming that getting CatalogNodesDto by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;

            CatalogDto catalog = target.GetCatalogDto();

            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodeDto expected = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);

                CatalogNodeDto actual = target.GetCatalogNodesDto(catalog.Catalog[i].Name, responseGroup);

                // Getting CatalogNodesDto by name only gets direct children of nodes specified by catalog name

                bool nestedNodes = false;

                if (actual.CatalogNode.Count < expected.CatalogNode.Count)
                    nestedNodes = true;

                if (nestedNodes)
                {
                    int j = 0;
                    int k = 0;
                    for (; j < actual.CatalogNode.Count; j++)
                    {
                        for (; k < expected.CatalogNode.Count; k++)
                        {
                            if (actual.CatalogNode[j].ParentNodeId == expected.CatalogNode[k].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[j], expected.CatalogNode[k]));
                                k++;
                                break;
                            }
                        }
                    }
                    Assert.AreEqual(j, actual.CatalogNode.Count);
                }

                else
                {
                    Assert.IsTrue(compareCatalogNodeDto(expected, actual));
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodesDto by catalog ID.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// Test assuming that getting CatalogNodeDto by code works correctly (CatalogSystem_UnitTest_GetCatalogNodeDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 1; // TODO: Initialize to an appropriate value
            CatalogNodeDto expected = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;
            actual = target.GetCatalogNodesDto(catalogId);

            String[] codes = { "mobile-phones", "digital-downloads", "phone-plans", "cell-phones", 
                                 "phone-accessories", "digital-cameras", "camera-accessories", "dailyspecials" };

            Assert.AreEqual(codes.Length, actual.CatalogNode.Count);

            for (int i = 0; i < codes.Length; i++)
            {
                expected = target.GetCatalogNodeDto(codes[i]);
                Assert.IsTrue(compareCatalogNode(expected.CatalogNode[0], actual.CatalogNode[i]));
            }


            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog ID and parent node ID.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest7()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId;
            //int parentNodeId; // TODO: Initialize to an appropriate value

            CatalogNodes expected;
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();

            System.Collections.Generic.List<int> parentNodes = new System.Collections.Generic.List<int>();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                int currentNode;
                bool duplicate = false;

                // Find all parent node ID's
                for (int j = 0; j < expected.CatalogNode.Length; j++)
                {
                    currentNode = expected.CatalogNode[j].ParentNodeId;
                    duplicate = false;
                    if (j == 0) // if looking at first catalog node, automatically add node
                    {
                        parentNodes.Add(currentNode);
                        continue;
                    }
                    foreach (int existing in parentNodes)
                    {
                        if (currentNode == existing)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate)
                        parentNodes.Add(currentNode);
                }

                catalogId = catalog.Catalog[i].CatalogId;
                foreach (int parentNodeId in parentNodes)
                {
                    actual = target.GetCatalogNodes(catalogId, parentNodeId);

                    int k = 0;
                    int l = 0;
                    for (; k < actual.CatalogNode.Length; k++)
                    {
                        for (; l < expected.CatalogNode.Length; l++)
                        {
                            if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                l++;
                                break;
                            }
                        }
                    }
                }
                parentNodes.Clear();
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog name and parent node code.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest6()
        {
            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty;
            string parentNodeCode = string.Empty;
            CatalogNodes expected;
            CatalogNodes actual;
            CatalogDto catalog = target.GetCatalogDto();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);

                for (int j = 0; j < expected.CatalogNode.Length; j++)
                {
                    parentNodeCode = string.Empty;
                    if (expected.CatalogNode[j].ParentNodeId == 0) // ignore if parent node is the catalog itself (already tested)
                    {
                        ;
                    }
                    else // if parent node is another node
                    {
                        foreach (CatalogNode node in expected.CatalogNode)
                        {
                            if (node.CatalogNodeId == expected.CatalogNode[j].ParentNodeId)
                            {
                                parentNodeCode = node.ID;
                                break;
                            }
                        }

                        actual = target.GetCatalogNodes(catalog.Catalog[i].Name, parentNodeCode);
                        int k = 0;
                        int l = 0;
                        for (; k < actual.CatalogNode.Length; k++)
                        {
                            for (; l < expected.CatalogNode.Length; l++)
                            {
                                if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                                {
                                    Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                    l++;
                                    j++; // No need to check it again at the iteration
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog name, parent node code, and response group.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest5()
        {
            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty;
            string parentNodeCode = string.Empty;
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();
            CatalogNodes expected;
            CatalogNodes actual;
            CatalogDto catalog = target.GetCatalogDto();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);

                for (int j = 0; j < expected.CatalogNode.Length; j++)
                {
                    parentNodeCode = string.Empty;
                    if (expected.CatalogNode[j].ParentNodeId == 0) // ignore if parent node is the catalog itself (already tested)
                    {
                        ;
                    }
                    else // if parent node is another node
                    {
                        foreach (CatalogNode node in expected.CatalogNode)
                        {
                            if (node.CatalogNodeId == expected.CatalogNode[j].ParentNodeId)
                            {
                                parentNodeCode = node.ID;
                                break;
                            }
                        }
                        
                        actual = target.GetCatalogNodes(catalog.Catalog[i].Name, parentNodeCode, responseGroup);
                        int k = 0;
                        int l = 0;
                        for (; k < actual.CatalogNode.Length; k++)
                        {
                            for (; l < expected.CatalogNode.Length; l++)
                            {
                                if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                                {
                                    Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                    l++;
                                    j++; // No need to check it again at the iteration
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog ID, parent node ID, and response group.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest4()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId;
            //int parentNodeId; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();

            CatalogNodes expected;
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();

            System.Collections.Generic.List<int> parentNodes = new System.Collections.Generic.List<int>();

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                int currentNode;
                bool duplicate = false;

                // Find all parent node ID's
                for (int j = 0; j < expected.CatalogNode.Length; j++)
                {
                    currentNode = expected.CatalogNode[j].ParentNodeId;
                    duplicate = false; 
                    if (j == 0) // if looking at first catalog node, automatically add node
                    {
                        parentNodes.Add(currentNode);
                        continue;
                    }
                    foreach (int existing in parentNodes)
                    {
                        if (currentNode == existing)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate)
                        parentNodes.Add(currentNode);
                }

                catalogId = catalog.Catalog[i].CatalogId;
                foreach (int parentNodeId in parentNodes)
                {
                    actual = target.GetCatalogNodes(catalogId, parentNodeId, responseGroup);

                    int k = 0;
                    int l = 0;
                    for (; k < actual.CatalogNode.Length; k++)
                    {
                        for (; l < expected.CatalogNode.Length; l++)
                        {
                            if (actual.CatalogNode[k].ParentNodeId == expected.CatalogNode[l].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(actual.CatalogNode[k], expected.CatalogNode[l]));
                                l++;
                                break;
                            }
                        }
                    }
                }
                parentNodes.Clear();
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog name.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest3()
        {
            ICatalogSystem target = CatalogContext.Current;
            string catalogName;
            CatalogNodes expected;
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogName = catalog.Catalog[i].Name;
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                actual = target.GetCatalogNodes(catalogName);

                // Getting CatalogNodes by name only gets direct children of nodes specified by catalog name
                bool nestedNodes = false;

                if (actual.CatalogNode.Length < expected.CatalogNode.Length)
                    nestedNodes = true;

                if (nestedNodes)
                {
                    int j = 0;
                    int k = 0;
                    for (; j < actual.CatalogNode.Length; j++)
                    {
                        for (; k < expected.CatalogNode.Length; k++)
                        {
                            if (actual.CatalogNode[j].ParentNodeId == expected.CatalogNode[k].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(expected.CatalogNode[k], actual.CatalogNode[j]));
                                k++;
                                break;
                            }
                        }
                    }
                    Assert.AreEqual(j, actual.CatalogNode.Length);
                }

                else
                {
                    Assert.IsTrue(compareCatalogNodes(expected, actual));
                }
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog name and response group.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName;
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();
            CatalogNodes expected;
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogName = catalog.Catalog[i].Name;
                expected = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                actual = target.GetCatalogNodes(catalogName, responseGroup);

                // Getting CatalogNodes by name only gets direct children of nodes specified by catalog name
                bool nestedNodes = false;

                if (actual.CatalogNode.Length < expected.CatalogNode.Length)
                    nestedNodes = true;

                if (nestedNodes)
                {
                    int j = 0;
                    int k = 0;
                    for (; j < actual.CatalogNode.Length; j++)
                    {
                        for (; k < expected.CatalogNode.Length; k++)
                        {
                            if (actual.CatalogNode[j].ParentNodeId == expected.CatalogNode[k].ParentNodeId)
                            {
                                Assert.IsTrue(compareCatalogNode(expected.CatalogNode[k], actual.CatalogNode[j]));
                                k++;
                                break;
                            }
                        }
                    }
                    Assert.AreEqual(j, actual.CatalogNode.Length);
                }

                else
                {
                    Assert.IsTrue(compareCatalogNodes(expected, actual));
                }
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog ID.
        /// Test assuming that getting CatalogNodes by catalog ID and response group works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId; // TODO: Initialize to an appropriate value
            CatalogNodes expected; // TODO: Initialize to an appropriate value
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = catalog.Catalog[i].CatalogId;
                expected = target.GetCatalogNodes(catalogId, new CatalogNodeResponseGroup());
                actual = target.GetCatalogNodes(catalogId);
                Assert.IsTrue(compareCatalogNodes(expected, actual));
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        // Helper method for comparing multiple catalog nodes.
        private bool compareCatalogNodes(CatalogNodes nodes1, CatalogNodes nodes2)
        {
            Assert.AreEqual(nodes1.CatalogNode.Length, nodes2.CatalogNode.Length);
            for (int i = 0; i < nodes1.CatalogNode.Length; i++)
            {
                Assert.AreEqual(nodes1.CatalogNode[i].CatalogId, nodes2.CatalogNode[i].CatalogId);
                Assert.AreEqual(nodes1.CatalogNode[i].CatalogNodeId, nodes2.CatalogNode[i].CatalogNodeId);
                Assert.AreEqual(nodes1.CatalogNode[i].DisplayTemplate, nodes2.CatalogNode[i].DisplayTemplate);
                Assert.AreEqual(nodes1.CatalogNode[i].EndDate, nodes2.CatalogNode[i].EndDate);
                Assert.AreEqual(nodes1.CatalogNode[i].ID, nodes2.CatalogNode[i].ID);
                Assert.AreEqual(nodes1.CatalogNode[i].IsActive, nodes2.CatalogNode[i].IsActive);
                Assert.AreEqual(nodes1.CatalogNode[i].Name, nodes2.CatalogNode[i].Name);
                Assert.AreEqual(nodes1.CatalogNode[i].ParentNodeId, nodes2.CatalogNode[i].ParentNodeId);
                Assert.AreEqual(nodes1.CatalogNode[i].StartDate, nodes2.CatalogNode[i].StartDate);
            }
            return true;
        }

        // Compares two Catalog rows and returns true if they are equal
        private bool compareCatalogNode(CatalogNode row1, CatalogNode row2)
        {
            Assert.AreEqual(row1.CatalogId, row2.CatalogId);
            Assert.AreEqual(row1.CatalogNodeId, row2.CatalogNodeId);
            Assert.AreEqual(row1.ID, row2.ID);
            Assert.AreEqual(row1.EndDate, row2.EndDate);
            Assert.AreEqual(row1.IsActive, row2.IsActive);
            Assert.AreEqual(row1.Name, row2.Name);
            Assert.AreEqual(row1.ParentNodeId, row2.ParentNodeId);
            Assert.AreEqual(row1.StartDate, row2.StartDate);
            Assert.AreEqual(row1.DisplayTemplate, row2.DisplayTemplate);
            return true;
        }

        /// <summary>
        /// Unit test to get catalog nodes by catalog ID and response group.
        /// Test assuming that getting CatalogNodesDto by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodesTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId;
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();
            CatalogNodes expected;
            CatalogNodes actual;

            CatalogDto catalog = target.GetCatalogDto();
            CatalogNodeDto nodes;

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = catalog.Catalog[i].CatalogId;
                nodes = target.GetCatalogNodesDto(catalogId);

                actual = target.GetCatalogNodes(catalogId, responseGroup);

                Assert.AreEqual(nodes.CatalogNode.Count, actual.CatalogNode.Length);

                // NOT checking order.
                // Assuming that the nodes are retrieved in same order as database.
                for (int j = 0; j < nodes.CatalogNode.Count; j++)
                {
                    Assert.AreEqual(actual.CatalogNode[j].CatalogId, nodes.CatalogNode[j].CatalogId);
                    Assert.AreEqual(actual.CatalogNode[j].CatalogNodeId, nodes.CatalogNode[j].CatalogNodeId);
                    Assert.AreEqual(actual.CatalogNode[j].DisplayTemplate, nodes.CatalogNode[j].TemplateName);
                    Assert.AreEqual(actual.CatalogNode[j].EndDate, nodes.CatalogNode[j].EndDate);
                    Assert.AreEqual(actual.CatalogNode[j].ID, nodes.CatalogNode[j].Code);
                    Assert.AreEqual(actual.CatalogNode[j].IsActive, nodes.CatalogNode[j].IsActive);
                    Assert.AreEqual(actual.CatalogNode[j].Name, nodes.CatalogNode[j].Name);
                    Assert.AreEqual(actual.CatalogNode[j].ParentNodeId, nodes.CatalogNode[j].ParentNodeId);
                    Assert.AreEqual(actual.CatalogNode[j].StartDate, nodes.CatalogNode[j].StartDate);
                }
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by catalog node ID.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// Test assuming that getting CatalogNodeDto by code works correctly (CatalogSystem_UnitTest_GetCatalogNodeDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest5()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = "digital-downloads"; // Arbitrary value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull);
            CatalogNodeDto expected = target.GetCatalogNodeDto(code); // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(code, responseGroup);
            Assert.IsTrue(compareCatalogNodeDto(expected, actual));

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by catalog node ID.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// Test assuming that getting CatalogNodeDto by code works correctly (CatalogSystem_UnitTest_GetCatalogNodeDtoTest()).
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest4()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogNodeId = 5; // Arbitrary value
            CatalogNodeDto expected = target.GetCatalogNodeDto("phone-accessories"); // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(catalogNodeId);
            Assert.IsTrue(compareCatalogNodeDto(expected, actual));

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by catalog node ID and response group.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// Test assuming that getting CatalogNodeDto by code works correctly (CatalogSystem_UnitTest_GetCatalogNodeDtoTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogNodeId = 4; // Arbitrary value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull);
            CatalogNodeDto expected = target.GetCatalogNodeDto("cell-phones"); // 
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(catalogNodeId, responseGroup);
            Assert.IsTrue(compareCatalogNodeDto(actual, expected));

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by URI and language code.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            string uri = "256-MB-SD-Karte-256MB.aspx"; // Arbitrary value
            uri = "Camera-Accessories.aspx"; // Another uri, for CatalogItemSeo rows with non-null catalog node ids
            string languageCode = "de-de"; // Arbitrary value

            CatalogNodeDto expected = target.GetCatalogNodeDto(7); // CatalogNodeId for specified uri
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(uri, languageCode);

            Assert.IsTrue(compareCatalogNodeDto(expected, actual));

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by URI, language code, and response group.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string uri = "256-MB-SD-Karte-256MB.aspx"; // Arbitrary value
            uri = "Camera-Accessories.aspx"; // Another uri, for CatalogItemSeo rows with non-null catalog node ids
            string languageCode = "de-de"; // Arbitrary value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup();

            CatalogNodeDto expected = target.GetCatalogNodeDto(7); // CatalogNodeId for specified uri
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(uri, languageCode, responseGroup);

            Assert.IsTrue(compareCatalogNodeDto(expected, actual));

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get CatalogNodeDto by code.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = "mobile-phones";
            //CatalogNodeDto expected = null;
            CatalogNodeDto actual;
            actual = target.GetCatalogNodeDto(code);

            // Application ID skipped
            Assert.AreEqual(actual.CatalogNode[0].CatalogId, 1);
            Assert.AreEqual(actual.CatalogNode[0].CatalogNodeId, 1);
            Assert.AreEqual(actual.CatalogNode[0].Code, "mobile-phones");
            Assert.AreEqual(actual.CatalogNode[0].StartDate, new DateTime(2008, 6, 27, 20, 0, 0, 0));
            Assert.AreEqual(actual.CatalogNode[0].IsActive, true);
            Assert.AreEqual(actual.CatalogNode[0].MetaClassId, 3);
            Assert.AreEqual(actual.CatalogNode[0].Name, "Mobile Phones & Plans");
            Assert.AreEqual(actual.CatalogNode[0].ParentNodeId, 0);
            Assert.AreEqual(actual.CatalogNode[0].SortOrder, 0);
            Assert.AreEqual(actual.CatalogNode[0].EndDate, new DateTime(2011, 6, 27, 20, 0, 0, 0, 0));
            Assert.AreEqual(actual.CatalogNode[0].TemplateName, "NodeEntriesTemplate");

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private bool compareCatalogNodeDto(CatalogNodeDto dto1, CatalogNodeDto dto2)
        {
            if (dto1.CatalogNode.Count != dto2.CatalogNode.Count)
                return false;
            for (int i = 0; i < dto1.CatalogNode.Count; i++)
            {
                if (!compareCatalogNode(dto1.CatalogNode[i], dto2.CatalogNode[i]))
                    return false;
            }
            return true;
        }

        // Compares two Catalog rows and returns true if they are equal
        private bool compareCatalogNode(CatalogNodeDto.CatalogNodeRow row1, CatalogNodeDto.CatalogNodeRow row2)
        {
            Assert.AreEqual(row1.ApplicationId, row2.ApplicationId);
            Assert.AreEqual(row1.CatalogId, row2.CatalogId);
            Assert.AreEqual(row1.CatalogNodeId, row2.CatalogNodeId);
            Assert.AreEqual(row1.Code, row2.Code);
            Assert.AreEqual(row1.EndDate, row2.EndDate);
            Assert.AreEqual(row1.IsActive, row2.IsActive);
            Assert.AreEqual(row1.MetaClassId, row2.MetaClassId);
            Assert.AreEqual(row1.Name, row2.Name);
            Assert.AreEqual(row1.ParentNodeId, row2.ParentNodeId);
            Assert.AreEqual(row1.SortOrder, row2.SortOrder);
            Assert.AreEqual(row1.StartDate, row2.StartDate);
            Assert.AreEqual(row1.TemplateName, row2.TemplateName);
            return true;
        }

        /// <summary>
        /// Unit test to get a catalog node by catalog node code and response group.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest5()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = string.Empty;
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup(); // TODO: Initialize to an appropriate value
            CatalogNode actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodes nodes = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                foreach (CatalogNode expected in nodes.CatalogNode)
                {
                    code = expected.ID;
                    actual = target.GetCatalogNode(code, responseGroup);
                    Assert.IsTrue(compareCatalogNode(expected, actual));
                }
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get a catalog node by Uri and language code.
        /// Test assuming that getting a CatalogNode by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodeTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest4()
        {
            ICatalogSystem target = CatalogContext.Current;
            string uri = string.Empty; // TODO: Initialize to an appropriate value
            string languageCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogNode expected = null; // TODO: Initialize to an appropriate value
            CatalogNode actual;

            CatalogDto catalog = target.GetCatalogDto();
            CatalogNodeDto nodesDto;

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                nodesDto = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);
                foreach (CatalogNodeDto.CatalogItemSeoRow itemSeo in nodesDto.CatalogItemSeo)
                {
                    try
                    {
                        // Looking at item SEO for catalog nodes
                        expected = target.GetCatalogNode(itemSeo.CatalogNodeId);
                        actual = target.GetCatalogNode(itemSeo.Uri, itemSeo.LanguageCode);
                        Assert.IsTrue(compareCatalogNode(expected, actual));
                    }
                    catch (NullReferenceException e)
                    {
                        // Looking at item SEO for entries, ignore
                        ;
                    }
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get a catalog node by Uri, language code, and response group.
        /// Test assuming that getting a CatalogNode by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodeTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            string uri = string.Empty; // TODO: Initialize to an appropriate value
            string languageCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup(); // TODO: Initialize to an appropriate value
            CatalogNode expected = null; // TODO: Initialize to an appropriate value
            CatalogNode actual;

            CatalogDto catalog = target.GetCatalogDto();
            CatalogNodeDto nodesDto;

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                nodesDto = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);
                foreach (CatalogNodeDto.CatalogItemSeoRow itemSeo in nodesDto.CatalogItemSeo)
                {
                    try
                    {
                        // Looking at item SEO for catalog nodes
                        expected = target.GetCatalogNode(itemSeo.CatalogNodeId);
                        actual = target.GetCatalogNode(itemSeo.Uri, itemSeo.LanguageCode, responseGroup);
                        Assert.IsTrue(compareCatalogNode(expected, actual));
                    }
                    catch (NullReferenceException e)
                    {
                        // Looking at item SEO for entries, ignore
                        ;
                    }
                }
            }
            
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get a catalog node by catalog node ID.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodeTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest2()
        {
            ICatalogSystem target = CatalogContext.Current;
            int catalogNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogNode actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodes nodes = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                foreach (CatalogNode expected in nodes.CatalogNode)
                {
                    catalogNodeId = expected.CatalogNodeId;
                    actual = target.GetCatalogNode(catalogNodeId);
                    Assert.IsTrue(compareCatalogNode(expected, actual));
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get a catalog node by catalog node ID and response group.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodeTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = new CatalogNodeResponseGroup(); // TODO: Initialize to an appropriate value
            CatalogNode actual;
            
            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodes nodes = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                foreach (CatalogNode expected in nodes.CatalogNode)
                {
                    catalogNodeId = expected.CatalogNodeId;
                    actual = target.GetCatalogNode(catalogNodeId, responseGroup);
                    Assert.IsTrue(compareCatalogNode(expected, actual));
                }
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test to get a catalog node by catalog node code.
        /// Test assuming that getting CatalogNodes by catalog ID works correctly (CatalogSystem_UnitTest_GetCatalogNodesTest1()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogNodeTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = string.Empty; 
            CatalogNode actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                CatalogNodes nodes = target.GetCatalogNodes(catalog.Catalog[i].CatalogId);
                foreach (CatalogNode expected in nodes.CatalogNode)
                {
                    code = expected.ID;
                    actual = target.GetCatalogNode(code);
                    Assert.IsTrue(compareCatalogNode(expected, actual));
                }
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntryDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntryDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogEntryId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryDto(catalogEntryId, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntryDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntryDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogEntryCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryDto(catalogEntryCode, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntryDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntryDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogEntryCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryDto(catalogEntryCode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntryDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntryDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogEntryId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryDto(catalogEntryId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting a CatalogEntryDto by URI and language code.
        /// Tets assumes that getting a CatalogEntryDto by URI, language code, and response group work correctly
        /// (CatalogSystem_UnitTest_GetCatalogEntryByUriDtoTest()).
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryByUriDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string uri = "512MB-Secure-DigitalKarte-SD.aspx"; // TODO: Initialize to an appropriate value
            string languageCode = "de-de"; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = target.GetCatalogEntryByUriDto(uri, languageCode, new CatalogEntryResponseGroup());
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryByUriDto(uri, languageCode);

            Assert.AreEqual(expected.CatalogEntry[0].CatalogEntryId, actual.CatalogEntry[0].CatalogEntryId);
            Assert.AreEqual(expected.CatalogEntry[0].TemplateName, actual.CatalogEntry[0].TemplateName);
            Assert.AreEqual(expected.CatalogEntry[0].EndDate, actual.CatalogEntry[0].EndDate);
            Assert.AreEqual(expected.CatalogEntry[0].ClassTypeId, actual.CatalogEntry[0].ClassTypeId);
            Assert.AreEqual(expected.CatalogEntry[0].Code, actual.CatalogEntry[0].Code);
            Assert.AreEqual(expected.CatalogEntry[0].IsActive, actual.CatalogEntry[0].IsActive);
            Assert.AreEqual(expected.CatalogEntry[0].MetaClassId, actual.CatalogEntry[0].MetaClassId);
            Assert.AreEqual(expected.CatalogEntry[0].Name, actual.CatalogEntry[0].Name);
            Assert.AreEqual(expected.CatalogEntry[0].StartDate, actual.CatalogEntry[0].StartDate); 
            
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting a CatalogEntryDto by URI, language code, and entry response group.
        /// Test assumes that getting a catalog entry by ID works (CatalogSystem_UnitTest_GetCatalogEntryTest()).
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryByUriDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string uri = "256-MB-SD-Karte-256MB.aspx"; // TODO: Initialize to an appropriate value
            string languageCode = "de-de"; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = new CatalogEntryResponseGroup();
            Entry expected = target.GetCatalogEntry(1281); // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntryByUriDto(uri, languageCode, responseGroup);

            Assert.AreEqual(expected.CatalogEntryId, actual.CatalogEntry[0].CatalogEntryId);
            Assert.AreEqual(expected.DisplayTemplate, actual.CatalogEntry[0].TemplateName);
            Assert.AreEqual(expected.EndDate, actual.CatalogEntry[0].EndDate);
            Assert.AreEqual(expected.ID, actual.CatalogEntry[0].Code);
            Assert.AreEqual(expected.IsActive, actual.CatalogEntry[0].IsActive);
            Assert.AreEqual(expected.MetaClassId, actual.CatalogEntry[0].MetaClassId);
            Assert.AreEqual(expected.Name, actual.CatalogEntry[0].Name);
            Assert.AreEqual(expected.StartDate, actual.CatalogEntry[0].StartDate);
            
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting a CatalogEntry by entry Id and entry response group.
        /// Test assumes that getting a catalog entry by ID works (CatalogSystem_UnitTest_GetCatalogEntryTest()).
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogEntryId = 656; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = new CatalogEntryResponseGroup(); // TODO: Initialize to an appropriate value
            Entry expected = target.GetCatalogEntry(catalogEntryId); // TODO: Initialize to an appropriate value
            Entry actual;
            actual = target.GetCatalogEntry(catalogEntryId, responseGroup);
            Assert.IsTrue(compareCatalogEntry(expected, actual));
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting a CatalogEntry by entry code.
        /// Test assumes that getting a catalog entry by code and response group works (CatalogSystem_UnitTest_GetCatalogEntryTest()).
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = "027242579965"; // TODO: Initialize to an appropriate value
            Entry expected = target.GetCatalogEntry(code, new CatalogEntryResponseGroup()); // TODO: Initialize to an appropriate value
            Entry actual;
            actual = target.GetCatalogEntry(code);
            Assert.IsTrue(compareCatalogEntry(expected, actual));
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting a CatalogEntry by entry Id.
        /// Test assumes that getting a catalog entry by code and response group works (CatalogSystem_UnitTest_GetCatalogEntryTest()).
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogEntryId = 103; // TODO: Initialize to an appropriate value
            Entry expected = target.GetCatalogEntry("ELCB000JLG5ZY6", new CatalogEntryResponseGroup());
            Entry actual;
            actual = target.GetCatalogEntry(catalogEntryId);
            Assert.IsTrue(compareCatalogEntry(expected, actual));
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private bool compareCatalogEntry(Entry expected, Entry actual)
        {
            Assert.AreEqual(expected.CatalogEntryId, actual.CatalogEntryId);
            Assert.AreEqual(expected.DisplayTemplate, actual.DisplayTemplate);
            Assert.AreEqual(expected.EndDate, actual.EndDate);
            Assert.AreEqual(expected.EntryType, actual.EntryType);
            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.MetaClassId, actual.MetaClassId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            return true;
        }

        /// <summary>
        /// Unit test for getting a CatalogEntry by code and entry response group.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// Test was verified manually.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogEntryTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string code = "ELCB000G622RM6"; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = new CatalogEntryResponseGroup();
            //Entry expected = null; // TODO: Initialize to an appropriate value
            Entry actual;
            actual = target.GetCatalogEntry(code, responseGroup);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest11()
        {

            ICatalogSystem target = CatalogContext.Current;
            string name = string.Empty; // TODO: Initialize to an appropriate value
            string entryType = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(name, entryType, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest10()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            int parentNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogId, parentNodeId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest9()
        {

            ICatalogSystem target = CatalogContext.Current;
            string name = string.Empty; // TODO: Initialize to an appropriate value
            string entryType = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(name, entryType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest8()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            int parentNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogName, parentNodeId, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest7()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            int parentNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogName, parentNodeId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest6()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogId, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest5()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 0; // TODO: Initialize to an appropriate value
            int parentNodeId = 0; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogId, parentNodeId, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest4()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogName, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId = 1; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(catalogId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            int parentEntryId = 0; // TODO: Initialize to an appropriate value
            string entryType = string.Empty; // TODO: Initialize to an appropriate value
            string relationType = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(parentEntryId, entryType, relationType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int parentEntryId = 0; // TODO: Initialize to an appropriate value
            string entryType = string.Empty; // TODO: Initialize to an appropriate value
            string relationType = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesDto(parentEntryId, entryType, relationType, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntriesByNodeDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesByNodeDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            string parentNodeCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesByNodeDto(catalogName, parentNodeCode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting catalog entries by NodeDto
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesByNodeDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            string parentNodeCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetCatalogEntriesByNodeDto(catalogName, parentNodeCode, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCatalogEntries
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogName = string.Empty; // TODO: Initialize to an appropriate value
            string catalogCode = string.Empty; // TODO: Initialize to an appropriate value
            Entries expected = new Entries(); // TODO: Initialize to an appropriate value
            Entries actual;
            actual = target.GetCatalogEntries(catalogName, catalogCode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting Catalg entries by catalog name, catalog node code, and respose group.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogEntriesTest()
        {

            ICatalogSystem target = CatalogContext.Current;

            CatalogDto catalogs = target.GetCatalogDto();

            string catalogName = string.Empty;
            string catalogCode = string.Empty;
            CatalogEntryResponseGroup responseGroup = new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull);
            Entries expected = new Entries();
            Entries actual;

            CatalogDto catalog = target.GetCatalogDto();
            CatalogNodeDto nodes;
            int totalCount = 0;

            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogName = catalog.Catalog[i].Name;
                nodes = target.GetCatalogNodesDto(catalog.Catalog[i].CatalogId);
                for (int j = 0; j < nodes.CatalogNode.Count; j++)
                {
                    catalogCode = nodes.CatalogNode[j].Code;
                    actual = target.GetCatalogEntries(catalogName, catalogCode, responseGroup);
                    totalCount += actual.TotalResults;
                }
            }
            Assert.AreEqual(totalCount, 1319);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogDto with catalogId and responseGroup
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogDtoTest4()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogDto expected = target.GetCatalogDto();
            int expectedCount = expected.Catalog.Count;
            CatalogDto actual;
            CatalogResponseGroup responseGroup = new CatalogResponseGroup();

            int[] catalogIds = new int[expectedCount];

            for (int i = 0; i < expectedCount; i++)
            {
                catalogIds[i] = expected.Catalog[i].CatalogId;
                actual = target.GetCatalogDto(catalogIds[i], responseGroup);
                Assert.IsTrue(compareCatalog(expected.Catalog[i], actual.Catalog[0]));
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogDto without parameters.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogDto expected = null;
            CatalogDto actual;
            actual = target.GetCatalogDto();

            int[] catalogIds = { 1, 2 };

            for (int i = 0; i < catalogIds.Length; i++)
            {
                expected = target.GetCatalogDto(catalogIds[i]);
                // Also assuming that the catalogs will be retrieved in order by catalog ID
                Assert.IsTrue(compareCatalog(expected.Catalog[0], actual.Catalog[i]));
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogDto by catalogId.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogDto expected = target.GetCatalogDto();
            int expectedCount = expected.Catalog.Count;
            CatalogDto actual;

            int[] catalogIds = new int[expectedCount];

            for (int i = 0; i < expectedCount; i++)
            {
                catalogIds[i] = expected.Catalog[i].CatalogId;
                actual = target.GetCatalogDto(catalogIds[i]);
                Assert.IsTrue(compareCatalog(expected.Catalog[i], actual.Catalog[0]));
            }
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogDto by siteId and responseGroup.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            Guid siteId = new Guid(); // TODO: Initialize to an appropriate value
            CatalogResponseGroup responseGroup = new CatalogResponseGroup(CatalogResponseGroup.ResponseGroup.CatalogFull);
            // expected CatalogDto is obtained by default method for getting CatalogDto with no specific parameters
            CatalogDto expected = target.GetCatalogDto();
            CatalogDto actual;
            actual = target.GetCatalogDto(siteId, responseGroup);
            Assert.IsTrue(compareCatalogDto(expected, actual));
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogDto by siteId.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            Guid siteId = new Guid(); // TODO: Initialize to an appropriate value
            CatalogDto expected = null; // TODO: Initialize to an appropriate value
            // expected CatalogDto is obtained by default method for getting CatalogDto with no specific parameters
            expected = target.GetCatalogDto();
            CatalogDto actual;
            actual = target.GetCatalogDto(siteId);
            Assert.IsTrue(compareCatalogDto(actual, expected));
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        // Compares two CatalogDto's and returns true if they are equal
        private bool compareCatalogDto(CatalogDto dto1, CatalogDto dto2)
        {
            if (dto1.Catalog.Count != dto2.Catalog.Count)
                return false;
            for (int i = 0; i < dto1.Catalog.Count; i++)
            {
                if (!compareCatalog(dto1.Catalog[i], dto2.Catalog[i]))
                    return false;
            }
            return true;
        }

        // Compares two Catalog rows and returns true if they are equal
        private bool compareCatalog(CatalogDto.CatalogRow row1, CatalogDto.CatalogRow row2)
        {
            Assert.AreEqual(row1.ApplicationId, row2.ApplicationId);
            Assert.AreEqual(row1.CatalogId, row2.CatalogId);
            Assert.AreEqual(row1.Created, row2.Created);
            Assert.AreEqual(row1.DefaultCurrency, row2.DefaultCurrency);
            Assert.AreEqual(row1.DefaultLanguage, row2.DefaultLanguage);
            Assert.AreEqual(row1.EndDate, row2.EndDate);
            Assert.AreEqual(row1.IsActive, row2.IsActive);
            Assert.AreEqual(row1.IsPrimary, row2.IsPrimary);
            Assert.AreEqual(row1.Modified, row2.Modified);
            Assert.AreEqual(row1.Name, row2.Name);
            Assert.AreEqual(row1.SortOrder, row2.SortOrder);
            Assert.AreEqual(row1.StartDate, row2.StartDate);
            Assert.AreEqual(row1.WeightBase, row2.WeightBase);
            try
            {
                Assert.AreEqual(row1.CreatorId, row2.CreatorId);
            }
            catch (StrongTypingException e)
            { ; }
            try
            {
                Assert.AreEqual(row1.ModifierId, row2.ModifierId);
            }
            catch (StrongTypingException e)
            { ; }
            return true;
        }

        /// <summary>
        /// Unit test for getting CatalogAssociationDto by entry ID.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogAssociationDtoByEntryIdTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogEntryId = 3; // Picked arbitrarily from SQL database
            CatalogAssociationDto actual;
            actual = target.GetCatalogAssociationDtoByEntryId(catalogEntryId);
            int assocId = 1;
            int sortOrder = 0;
            string assocName = "CrossSell";
            string assocDesc = "You Might Also Like";
            Assert.AreEqual(actual.CatalogAssociation[0].CatalogAssociationId, assocId);
            Assert.AreEqual(actual.CatalogAssociation[0].SortOrder, sortOrder);
            Assert.AreEqual(actual.CatalogAssociation[0].AssociationDescription, assocDesc);
            Assert.AreEqual(actual.CatalogAssociation[0].AssociationName, assocName);

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogAssociationDto by catalog ID and entry code.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetCatalogAssociationDtoByEntryCodeTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            int catalogId; // TODO: Initialize to an appropriate value
            string catalogEntryCode = string.Empty; // TODO: Initialize to an appropriate value
            CatalogAssociationDto expected = null; // TODO: Initialize to an appropriate value
            CatalogAssociationDto actual;

            CatalogDto catalog = target.GetCatalogDto();
            for (int i = 0; i < catalog.Catalog.Count; i++)
            {
                catalogId = i;
                
                actual = target.GetCatalogAssociationDtoByEntryCode(catalogId, catalogEntryCode);
            }

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        /// Unit test for getting CatalogAssociationDto by catalog association name.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        /// </summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogAssociationDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string catalogAssociationName = "Accessories";
            CatalogAssociationDto actual;
            actual = target.GetCatalogAssociationDto(catalogAssociationName);
            // Figured out via looking into SQL database directly
            int count = 134;
            Assert.AreEqual(actual.CatalogAssociation.Count, count);

            catalogAssociationName = "CrossSell";
            count = 363;
            actual = target.GetCatalogAssociationDto(catalogAssociationName);
            Assert.AreEqual(actual.CatalogAssociation.Count, count);

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        // Unit test for getting CatalogAssociationDto by catalog association ID.
        /// Values are hard-coded by looking directly at the SQL database in appropriate connection string.
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_UnitTest_GetCatalogAssociationDto()
        {

            ICatalogSystem target = CatalogContext.Current;
            // Arbitrary catalog association ID found in catalog association table in database
            int catalogAssociationId = 1;
            // Doesn't really make sense to have an expected dto to initialize, use expected values instead
            //CatalogAssociationDto expected = null; // TODO: Initialize to an appropriate value
            CatalogAssociationDto actual;
            actual = target.GetCatalogAssociationDto(catalogAssociationId);
            Assert.AreEqual(actual.CatalogAssociation[0].CatalogEntryId, 3);
            Assert.AreEqual(actual.CatalogAssociation[0].AssociationName, "CrossSell");
            Assert.AreEqual(actual.CatalogAssociation[0].AssociationDescription, "You Might Also Like");

            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAssociatedCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetAssociatedCatalogEntriesDtoTest3()
        {

            ICatalogSystem target = CatalogContext.Current;
            int parentEntryId = 0; // TODO: Initialize to an appropriate value
            string associationName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetAssociatedCatalogEntriesDto(parentEntryId, associationName, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAssociatedCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetAssociatedCatalogEntriesDtoTest2()
        {

            ICatalogSystem target = CatalogContext.Current;
            int parentEntryId = 0; // TODO: Initialize to an appropriate value
            string associationName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetAssociatedCatalogEntriesDto(parentEntryId, associationName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAssociatedCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetAssociatedCatalogEntriesDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            string parentEntryCode = string.Empty; // TODO: Initialize to an appropriate value
            string associationName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAssociatedCatalogEntriesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_GetAssociatedCatalogEntriesDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            string parentEntryCode = string.Empty; // TODO: Initialize to an appropriate value
            string associationName = string.Empty; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #endregion

        #region Catalog system find unit tests

        /// <summary>
        ///A test for FindNodesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindNodesDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            int recordsCount = 0; // TODO: Initialize to an appropriate value
            int recordsCountExpected = 0; // TODO: Initialize to an appropriate value
            CatalogNodeDto expected = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;
            actual = target.FindNodesDto(parameters, options, ref recordsCount);
            Assert.AreEqual(recordsCountExpected, recordsCount);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindNodesDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindNodesDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            int recordsCount = 0; // TODO: Initialize to an appropriate value
            int recordsCountExpected = 0; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto expected = null; // TODO: Initialize to an appropriate value
            CatalogNodeDto actual;
            actual = target.FindNodesDto(parameters, options, ref recordsCount, responseGroup);
            Assert.AreEqual(recordsCountExpected, recordsCount);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindItemsDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindItemsDtoTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            int recordsCount = 0; // TODO: Initialize to an appropriate value
            int recordsCountExpected = 0; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.FindItemsDto(parameters, options, ref recordsCount);
            Assert.AreEqual(recordsCountExpected, recordsCount);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindItemsDto
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindItemsDtoTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            int recordsCount = 0; // TODO: Initialize to an appropriate value
            int recordsCountExpected = 0; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto expected = null; // TODO: Initialize to an appropriate value
            CatalogEntryDto actual;
            actual = target.FindItemsDto(parameters, options, ref recordsCount, responseGroup);
            Assert.AreEqual(recordsCountExpected, recordsCount);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindItems
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindItemsTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            CatalogEntryResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            Entries expected = new Entries(); // TODO: Initialize to an appropriate value
            Entries actual;
            actual = target.FindItems(parameters, options, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindItems
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindItemsTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            CatalogSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogSearchOptions options = null; // TODO: Initialize to an appropriate value
            Entries expected = new Entries(); // TODO: Initialize to an appropriate value
            Entries actual;
            actual = target.FindItems(parameters, options);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindCatalogItemsTable
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindCatalogItemsTableTest1()
        {

            ICatalogSystem target = CatalogContext.Current;
            ItemSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            DataTable expected = null; // TODO: Initialize to an appropriate value
            DataTable actual;
            actual = target.FindCatalogItemsTable(parameters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindCatalogItemsTable
        ///</summary>
        [TestMethod()]
        public void CatalogSystem_FindCatalogItemsTableTest()
        {

            ICatalogSystem target = CatalogContext.Current;
            ItemSearchParameters parameters = null; // TODO: Initialize to an appropriate value
            CatalogNodeResponseGroup responseGroup = null; // TODO: Initialize to an appropriate value
            DataTable expected = null; // TODO: Initialize to an appropriate value
            DataTable actual;
            actual = target.FindCatalogItemsTable(parameters, responseGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #endregion

        #region Catalog Import/Export
        [TestMethod()]
        public void CatalogSystem_ImportExportCatalog()
        {
            string catalogName = "UnitTest";

            // Find this new catalog
            ICatalogSystem system = CatalogContext.Current;

            CatalogDto catalogs = system.GetCatalogDto();

            // Check if catalog already exists
            // Delete this new catalog
            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                if (catalog.Name == catalogName)
                {
                    CatalogContext.Current.DeleteCatalog(catalog.CatalogId);
                    break;
                }
            }


            string destDir = "ImportExport";
            // delete folder if it already exists
            if (Directory.Exists(destDir))
                Directory.Delete(destDir, true);

            DirectoryInfo dir = Directory.CreateDirectory(destDir);

            // unpack archive
            ZipHelper.ExtractZip("CatalogExport_Catalog_UnitTest.zip", dir.FullName);

            using (FileStream fileStream = new FileStream(Path.Combine(dir.FullName, "Catalog.xml"), FileMode.Open))
                new CatalogImportExport().Import(fileStream, AppContext.Current.ApplicationId, dir.FullName);

            // Find this new catalog

            // Get catalog lists
            catalogs = system.GetCatalogDto();

            bool foundCatalog = false;
            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                if (catalog.Name == catalogName)
                {
                    foundCatalog = true;
                    break;
                }
            }

            Assert.IsTrue(foundCatalog, String.Format("Couldn't find '{0}' catalog.", catalogName));

            // Now export it
            if (foundCatalog)
            {
                FileStream fs = new FileStream("Catalog_UnitTestExport.xml", FileMode.Create, FileAccess.ReadWrite);
                new CatalogImportExport().Export(catalogName, fs, "");
            }

            // Delete this new catalog
            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                if (catalog.Name == catalogName)
                {
                    CatalogContext.Current.DeleteCatalog(catalog.CatalogId);
                    break;
                }
            }
        }
        #endregion

        #region Catalog Import/Export
        [TestMethod()]
        public void CatalogSystem_CSV_ImportCatalog()
        {
            ICatalogSystem target = CatalogContext.Current;

            // Need prior knowledge of existing catalog to get catalog entries
            int existingCatalogId = Int32.MinValue;

            CatalogDto catalogs = target.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                existingCatalogId = catalog.CatalogId;
                break;
            }

			//CSV import tests
            CSVImportCatalog(existingCatalogId, "UnitTest_CatalogNode_DefaultCatalogNode.csv", "UnitTest_CatalogNode_DefaultCatalogNode.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_CatalogEntry_PhoneAccessories.csv", "UnitTest_CatalogEntry_PhoneAccessories.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_SalePrice.csv", "UnitTest_SalePrice.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_VariationInventory.csv", "UnitTest_VariationInventory.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_CatalogEntry_CatalogEntryEx.csv", "UnitTest_CatalogEntry_CatalogEntryEx.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_CatalogEntryRelation.csv", "UnitTest_CatalogEntryRelation.xml");
            CSVImportCatalog(existingCatalogId, "UnitTest_CatalogEntryAssociation.csv", "UnitTest_CatalogEntryAssociation.xml");

			//Import StringDictionary values
			CSVImportCatalog_MetaData_StringDictionary(existingCatalogId);

			//Delete test product
            Entry entry = target.GetCatalogEntry("UnitTestEntryCode002");
            target.DeleteCatalogEntry(entry.CatalogEntryId, true);

			//Delete test category
            CatalogNode node = target.GetCatalogNode("UnitTestCategoryCode001");
            target.DeleteCatalogNode(node.CatalogNodeId, existingCatalogId);
        }

		public void CSVImportCatalog_MetaData_StringDictionary(int catalogId)
		{
			//Create test metafield
			string unitTest_MetaFieldName = "UnitTest_StringDictionary";
			MetaField stringDictionaryMetaField = MetaField.Load(CatalogContext.MetaDataContext, unitTest_MetaFieldName);
			if (stringDictionaryMetaField == null)
				stringDictionaryMetaField = MetaField.Create(CatalogContext.MetaDataContext, "Mediachase.Commerce.Catalog", unitTest_MetaFieldName, unitTest_MetaFieldName, String.Empty, MetaDataType.StringDictionary, 4, true, false, false, false, false);

			//Add test metafield to PhoneAccessories metaclass
			MetaClass phoneAccessoriesMetaClass = MetaClass.Load(CatalogContext.MetaDataContext, "PhoneAccessories");
			if (phoneAccessoriesMetaClass.MetaFields[unitTest_MetaFieldName] == null)
				phoneAccessoriesMetaClass.AddField(stringDictionaryMetaField);

			//Execute CVS import test
			CSVImportCatalog(catalogId, "UnitTest_CatalogEntry_PhoneAccessories_StringDictionary.csv", "UnitTest_CatalogEntry_PhoneAccessories_StringDictionary.xml");

			//Get imported values
			Entry entry = CatalogContext.Current.GetCatalogEntry("UnitTestEntryCode001");
			string[] retVal = entry.ItemAttributes[unitTest_MetaFieldName].Value;

			//Delete test metafield
			phoneAccessoriesMetaClass.DeleteField(stringDictionaryMetaField);
			MetaField.Delete(CatalogContext.MetaDataContext, stringDictionaryMetaField.Id);

			//Check imported values
			if (retVal.Length != 2)
				throw new Exception("Failed to import StringDictionary values.");
		}

        protected void CSVImportCatalog(int CatalogId, string filePath, string mappingFilePath)
        {
            Mediachase.MetaDataPlus.Import.Rule mapping = Mediachase.MetaDataPlus.Import.Rule.XmlDeserialize(CatalogContext.MetaDataContext, mappingFilePath);
            char chTextQualifier = '\0';
            if (mapping.Attribute["TextQualifier"].ToString() != "")
                chTextQualifier = char.Parse(mapping.Attribute["TextQualifier"]);

            string sEncoding = mapping.Attribute["Encoding"];

            IIncomingDataParser parser = null;
            DataSet rawData = null;

            parser = new CsvIncomingDataParser("", true, char.Parse(mapping.Attribute["Delimiter"].ToString()), chTextQualifier, true, GetEncoding(sEncoding));
            rawData = parser.Parse(Path.GetFileName(filePath), null);

            DataTable dtSource = rawData.Tables[0];

            MappingMetaClass mc = null;

            FillResult fr = null;
            switch (mapping.Attribute["TypeName"])
            {
                case "Category":
                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                    CatalogContext.MetaDataContext.Language = mapping.Attribute["Language"];

                    mc = new CategoryMappingMetaClass(CatalogContext.MetaDataContext, mapping.ClassName, CatalogId);
                    fr = ((CategoryMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);

                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                    break;
                case "Entry":
                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                    CatalogContext.MetaDataContext.Language = mapping.Attribute["Language"];

                    mc = new EntryMappingMetaClass(CatalogContext.MetaDataContext, mapping.ClassName, CatalogId);
                    fr = ((EntryMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);

                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                    break;
                case "EntryRelation":
                    mc = new EntryRelationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
                    fr = ((EntryRelationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                    break;
                case "EntryAssociation":
                    mc = new EntryAssociationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
                    fr = ((EntryAssociationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                    break;
                case "Variation":
                    mc = new VariationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
                    fr = ((VariationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                    break;
                case "SalePrice":
                    mc = new PricingMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
                    fr = ((PricingMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                    break;
            }

            if (fr != null)
            {
                if (fr.ErrorRows > 0)
                {
                    foreach (Exception expt in fr.Exceptions)
                    {
                        throw expt;
                    }
                }

                if (fr.Warnings.Length > 0)
                {
                    foreach (MDPImportWarning MDPWarn in fr.Warnings)
                    {
                        throw new Exception(String.Format("Line {0}: {1}", MDPWarn.RowIndex, MDPWarn.Message));
                    }
                }

            }
        }

        private Encoding GetEncoding(string sEncoding)
        {
            if (sEncoding == String.Empty || sEncoding == "Default" || sEncoding == null)
                return Encoding.Default;
            else
                return Encoding.GetEncoding(sEncoding);
        }

        #endregion
    }
}
