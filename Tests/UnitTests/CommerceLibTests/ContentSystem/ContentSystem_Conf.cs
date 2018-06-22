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
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Pages;
using Mediachase.Cms.ImportExport;

namespace UnitTests.ContentSystem
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Configuration
    {
        private static Guid _SiteGuid = new Guid("1ab08b1a-c480-47b5-a98e-3d50b433dcb5");
        /// <summary>
        /// Gets the results path.
        /// </summary>
        /// <value>The results path.</value>
        private static string ResultsPath
        {
            get
            {
                return Directory.GetParent(Directory.GetParent(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName).FullName) + "\\Tests\\UnitTests\\CommerceLibTests\\Results\\";
            }
        }

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
            PageDocument.Init(new SqlPageDocumentStorageProvider(), new SqlTemporaryStorageProvider());

            if (!System.IO.Directory.Exists(ResultsPath))
                System.IO.Directory.CreateDirectory(ResultsPath);
        }

        /// <summary>
        /// Cleans up this instance.
        /// </summary>
        [ClassCleanup]
        public static void Cleanup()
        {
        }

        #region Test Methods
        /// <summary>
        /// Initial test.
        /// </summary>
        [TestInitialize]
        public void InitTest()
        {
            
        }

        /// <summary>
        /// Exports the site.
        /// </summary>
        [TestMethod]
        [Description("Content System: export sites")]
        public void ExportSite()
        {
            SiteDto siteDto = CMSContext.Current.GetSitesDto(CmsConfiguration.Instance.ApplicationId);
            foreach (SiteDto.SiteRow siteRow in siteDto.Site)
            {
                FileStream stream = File.Create(String.Format(ResultsPath + "{0}.site", siteRow.Name));
                ImportExportHelper importExportHelper = new ImportExportHelper();
                importExportHelper.ExportSite(siteRow.SiteId, stream);
                stream.Close();
            }
        }

        /// <summary>
        /// Imports the site.
        /// </summary>
        [TestMethod]
        [Description("Content System: import site")]
        public void ImportSite()
        {
            Mediachase.Commerce.Core.Dto.AppDto dto = Mediachase.Commerce.Core.Managers.AppManager.GetApplicationDto("eCommerceFramework");
            
            //import data
            FileStream stream = File.Open(String.Format(ResultsPath + "{0}.site", "Default Sample Site"), FileMode.Open);
            ImportExportHelper importExportHelper = new ImportExportHelper();
            importExportHelper.ImportSite(stream, dto.Application[0].ApplicationId, Guid.Empty, false);
            stream.Close();
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
