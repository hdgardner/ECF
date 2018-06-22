using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Mediachase.Cms.ImportExport;
using Mediachase.Cms.Dto;
using Mediachase.Cms;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Core;

namespace UnitTests.ContentSystem
{
    [TestClass()]
    public class ContentSystem_UnitTests
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

        [TestMethod()]
        public void ContentSystem_ImportExportSite()
        {
            Guid[] siteIDs;
            Guid applicationId = AppContext.Current.ApplicationId;

            ImportExportHelper importExportHelper = new ImportExportHelper();

            //import data
            using (FileStream stream = File.Open("SiteExport-B2C.xml", FileMode.Open))
            {                
                siteIDs = importExportHelper.ImportSite(stream, applicationId, Guid.Empty, false);
            }

            Guid siteId = siteIDs[0];

            FileStream fs = new FileStream("Content_SiteExport.xml", FileMode.Create, FileAccess.ReadWrite);
            importExportHelper.ExportSite(siteId, fs);

            SiteDto siteDto = CMSContext.Current.GetSiteDto(siteId);

            // delete menus
            MenuDto menuDto = MenuManager.GetMenuDto(siteId);
            if (menuDto.Menu.Rows.Count > 0)
            {
                foreach (MenuDto.MenuRow menuRow in menuDto.Menu.Rows)
                    menuRow.Delete();
            }

            if (menuDto.HasChanges())
                MenuManager.SaveMenuDto(menuDto);

            // delete folders and pages
            int rootId = FileTreeItem.GetRoot(siteId);
            FileTreeItem.DeleteFileItem(rootId);

            //delete site
            siteDto.Site[0].Delete();
            CMSContext.Current.SaveSite(siteDto);
        }
    }
}
