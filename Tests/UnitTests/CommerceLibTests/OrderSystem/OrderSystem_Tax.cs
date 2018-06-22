using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Orders.ImportExport;
using Mediachase.Commerce.Core;
using Mediachase.Cms;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Orders;

namespace UnitTests.OrderSystem
{
    [TestClass]
    public class OrderSystem_Tax
    {
        public OrderSystem_Tax()
        {
            //
            // TODO: Add constructor logic here
            //
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

        [TestMethod]
        public void OrderSystem_ImportTax()
        {
            #region Keep existing taxes
            // Keep track of existing taxes
            Dictionary<int, int> existing = new Dictionary<int, int>();
            List<int> existingTaxes = new List<int>();
            TaxDto taxDto;
            foreach (TaxType taxType in Enum.GetValues(typeof(TaxType)))
            {
                taxDto = TaxManager.GetTaxDto(taxType);

                for (int i = 0; i < taxDto.Tax.Count; i++)
                {
                    existingTaxes.Add(taxDto.Tax[i].TaxId);
                    existing.Add(existing.Count, taxDto.Tax[i].TaxId);
                }
            }
            int totalExisting = existing.Count;
            #endregion

            // TODO: Connect to an online source to update contents

            // Import sample tax configurations from sample CSV file
            // TODO: Figure out why import does not work if there are existing taxes that
            //        match the taxes in CSV file.
            TaxImportExport tie = new TaxImportExport();
            tie.Import("TaxCSVSample2.csv", AppContext.Current.ApplicationId, CMSContext.Current.SiteId, ',');

            #region Delete imported taxes
            // Delete imported tax configurations, but keep pre-existing taxes. Works fine.
            foreach (TaxType taxType in Enum.GetValues(typeof(TaxType)))
            {
                taxDto = TaxManager.GetTaxDto(taxType);

                #region Foreach version
                /*
                TaxDto copy = (TaxDto)taxDto.Copy();
                foreach (TaxDto.TaxRow taxRow in copy.Tax)
                {
                    if (existing.ContainsValue(taxRow.TaxId))
                        continue;
                    else
                    {
                        TaxDto.TaxRow real = TaxManager.GetTax(taxRow.TaxId).Tax[0];
                        real.Delete();
                        TaxManager.SaveTax(taxDto);
                    }
                }
                 */
                #endregion

                // Delete code copied from OrderDeleteHandler.ProcessDeleteCommand(...)
                for (int i = 0; i < taxDto.Tax.Count; i++)
                {
                    if (existing.ContainsValue(taxDto.Tax[i].TaxId))
                        continue;
                    else
                    {
                        taxDto.Tax[i].Delete();
                        TaxManager.SaveTax(taxDto);
                        i--;
                    }
                }
            }
            #endregion
        }
    }
}
