using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Xml;
using System.IO;
using System.Data;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;


namespace UnitTests.AssetLibrary
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
            string mdCsKey = ConfigurationManager.AppSettings["MetaDataConnection"];
            if (ConfigurationManager.ConnectionStrings[mdCsKey] != null)
                DataContext.Current = new DataContext(ConfigurationManager.ConnectionStrings[mdCsKey].ConnectionString);

            MetaClassManager metaClassManager = DataContext.Current.MetaModel;
        }

        [ClassCleanup]
        public static void Cleanup()
        {
        }
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
        }


        #region Test Methods
        /// <summary>
        /// Creates the new asset.
        /// Unit test dependent on SaveCatalogEntry, which fails as of 3/25/09 3:54 PM
        /// </summary>
        [TestMethod]
        public void AssetLibrary_UnitTest_CreateNewAsset()
        {
            // Get root node first
            TreeNode[] treeNodes = TreeManager.GetRootNodes(Folder.GetAssignedMetaClass());

            if(treeNodes.Length == 0)
                Assert.Fail("Asset library must have atleast one root node defined");

            int rootNodeId = treeNodes[0].ObjectId;

            // Create new Folder
            Guid folderName = Guid.NewGuid();
            Folder folder = new Folder();
            folder.Name = folderName.ToString();
            folder.Save();
            TreeNode currenFolder = TreeManager.AppendNode(Folder.GetAssignedMetaClass(), rootNodeId, true, folder);

            // Read file 
            FileStream fileStream = File.Open("winter.jpg", FileMode.Open);
            FolderElement newFile = FolderElement.Create(currenFolder.ObjectId, fileStream.Name, fileStream);

            // Now create a business object that will store meta data
            BusinessObject businessObject = MetaObjectActivator.CreateInstance<BusinessObject>("FolderElement", newFile.PrimaryKeyId.Value);

            // Now set business object fields
            businessObject.Properties["Description"].Value = "Some description goes here";

            // Save business object
            businessObject.Save();

            FolderElement[] myElements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderElementId", FilterElementType.Equal, newFile.PrimaryKeyId.Value.ToString()) });
            Assert.IsTrue(myElements.Length > 0, "No File FolderElement found. File must have not been created");
            myElements[0].Delete();

            Folder[] myFolders = Folder.List<Folder>(Folder.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderId", FilterElementType.Equal, currenFolder.ObjectId.ToString()) });
            Assert.IsTrue(myFolders.Length > 0, "No Folder found. Folder must have not been created");
            myFolders[0].Delete();

            /*
            // Assign asset to an entry
            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto("samplecode", new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.Assets));

            if(entryDto.CatalogEntry.Count == 0)
                Assert.Fail("specified entry could not be located");

            // Associate file with an entry
            CatalogEntryDto.CatalogItemAssetRow row = entryDto.CatalogItemAsset.NewCatalogItemAssetRow();
            row.AssetType = "file"; // specify "folder", if type is folder
            row.AssetKey = newFile.PrimaryKeyId.ToString();
            row.CatalogEntryId = entryDto.CatalogEntry[0].CatalogEntryId; // specify CatalogNodeId if associated with a category
            row.GroupName = "images";
            row.SortOrder = 0;

            entryDto.CatalogEntry.Rows.Add(row);

            // Save the new data
            CatalogContext.Current.SaveCatalogEntry(entryDto);
             * */

        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
        }
        #endregion
    }
}