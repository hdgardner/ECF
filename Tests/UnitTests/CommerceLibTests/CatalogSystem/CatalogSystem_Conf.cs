using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Collections;
using Mediachase.Commerce.Orders;
using System.Xml;
using System.IO;
using System.Data;
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
        private static Guid _SiteGuid = new Guid("1ab08b1a-c480-47b5-a98e-3d50b433dcb5");
        //private static int _CatalogId = 0;

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
        /// Restores the meta data.
        /// </summary>
        private static void RestoreMetaData()
        {
            CatalogConfiguration.ConfigureMetaData();
        }

        /// <summary>
        /// Initializes the specified test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]       
        public static void Initialize(TestContext testContext) 
        {
            //RestoreMetaData();
            //CreateCatalog();
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
            //DeleteCatalog();
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
        /// Configures the catalog meta data.
        /// </summary>
        [TestMethod]
        [Description("Catalog System: recreate Default Meta Data")]
        public void ConfigureCatalogMetaData()
        {
            CatalogConfiguration.ConfigureMetaData();
        }

        /// <summary>
        /// Examples the of meta data.
        /// </summary>
        public void ExampleOfMetaData()
        {
            int objectId = 1245; // need to get that
            MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, objectId, "CatalogEntryEx");
            metaObject["wfo_SomeMetaField"] = "newvalue";
            metaObject.AcceptChanges(CatalogContext.MetaDataContext);
        }

        /// <summary>
        /// Createds the serialized meta data.
        /// </summary>
        [TestMethod()]
        public void CreatedSerializedMetaData()
        {
            MetaClassCollection metaClasses = MetaClass.GetList(CatalogContext.MetaDataContext, "Mediachase.Commerce.Catalog", true);
            foreach(MetaClass metaClass in metaClasses)
            {
                if(!metaClass.IsSystem)
                {
                    MetaObject[] metaObjects = MetaObject.GetList(CatalogContext.MetaDataContext, metaClass);
                    foreach(MetaObject metaObject in metaObjects)
                    {
                        metaObject[metaClass.UserMetaFields[0]] = metaObject[metaClass.UserMetaFields[0]];
                        metaObject.AcceptChanges(CatalogContext.MetaDataContext);
                    }
                }
            }            
        }
         * */

        /*
        public static void CreateCatalog()
        {
            _CatalogId = CatalogAdmin.Create(_SiteGuid, "SampleName" + new Random().Next(), "en-us", "USD", "kgs", DateTime.Now.ToUniversalTime(), DateTime.Now.AddMonths(1).ToUniversalTime());
        }

        [TestMethod]
        public void ModifyCatalog()
        {
            CatalogAdmin admin = new CatalogAdmin();
            admin.Load(_SiteGuid);
            CatalogDto dto = admin.CurrentDto;
            CatalogDto.CatalogRow[] rows = (CatalogDto.CatalogRow[])dto.Catalog.Select(String.Format("CatalogId = {0}", _CatalogId));
            
            if (rows == null || rows.Length == 0)
                throw new Exception("Failed to locate the catalog");

            rows[0].IsActive = true;

            CatalogDto.CatalogLanguageRow languageRow = dto.CatalogLanguage.NewCatalogLanguageRow();
            languageRow.LanguageCode = "us-uk";
            languageRow.CatalogId = rows[0].CatalogId;
            dto.CatalogLanguage.AddCatalogLanguageRow(languageRow);

            admin.Save();

            // Update the language
            rows[0].GetCatalogLanguageRows()[0].LanguageCode = "ru";
            admin.Save();
        }

        [TestMethod]
        public void CreateCatalogNode()
        {
            int catalogNodeId = CatalogNodeAdmin.Create(_CatalogId, "TestName" + new Random().Next(), "Code", 1, 0, 0, DateTime.Now.ToUniversalTime(), DateTime.Now.AddMonths(1).ToUniversalTime());

            CatalogNodeAdmin admin = new CatalogNodeAdmin();
            admin.Load(catalogNodeId);
            CatalogNodeDto dto = admin.CurrentDto;

            int catalogEntryId = CatalogEntryAdmin.Create(_CatalogId, "TestName" + new Random().Next(), "Code", 1, 0, EntryType.Product, DateTime.Now.ToUniversalTime(), DateTime.Now.AddMonths(1).ToUniversalTime());
            CatalogEntryAdmin adminEntry = new CatalogEntryAdmin();
            adminEntry.Load(catalogEntryId);
            CatalogEntryDto entryDto = adminEntry.CurrentDto;

            CatalogEntryDto.CatalogAssociationRow assRow = entryDto.CatalogAssociation.NewCatalogAssociationRow();
            assRow.AssociationName = "Related Products";
            assRow.AssociationDescription = "Displays related products";
            assRow.CatalogEntryId = catalogEntryId;
            assRow.SortOrder = 0;
            entryDto.CatalogAssociation.AddCatalogAssociationRow(assRow);
            adminEntry.Save();
        }

        public static void DeleteCatalog()
        {
            CatalogAdmin admin = new CatalogAdmin();
            admin.Load(_SiteGuid);

            CatalogDto dto = admin.CurrentDto;
            CatalogDto.CatalogRow[] rows = (CatalogDto.CatalogRow[])dto.Catalog.Select(String.Format("CatalogId = {0}", _CatalogId));

            // Remove language
            rows[0].GetCatalogLanguageRows()[0].Delete();
            admin.Save();

            CatalogAdmin.Delete(_CatalogId);
        }

        [TestMethod]
        public void PrintAllCatalogs()
        {
            SiteCatalogs catalogs = CatalogContext.Current.GetCatalogs();

            foreach (SiteCatalog catalog in catalogs.Catalog)
            {
                Console.WriteLine(catalog.Name);
            }
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
    }
}