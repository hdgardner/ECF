using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;

namespace UnitTests.MarketingSystem
{
    /// <summary>
    /// Summary description for MarketingSystem_BuyXGetNofYatReducedPrice
    /// </summary>
    [TestClass]
    public class MarketingSystem_BuyXGetNofYatReducedPrice
    {
        public MarketingSystem_BuyXGetNofYatReducedPrice()
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

        /// <summary>
        /// Merketings the system buy X get N off Y at Reduced Price.
        /// </summary>
        [TestMethod]
        public void MerketingSystem_BuyXGetNoffYatReducedPrice()
        {
            PromotionEntriesSet sourceSet = new PromotionEntriesSet();
            PromotionEntriesSet targetSet = new PromotionEntriesSet();
            PromotionEntry itemY1 = new PromotionEntry("", "Apple iPod touch 16 GB (Old)", "ELCB000JNYWBG6", 100);
            PromotionEntry itemY2 = new PromotionEntry("", "Apple iPod touch 16 GB (Old)", "ELCB000JNYWBG6", 100);
            itemY2.Quantity = 10;
            PromotionEntry itemXspecified = new PromotionEntry("", "Plantronics Voyager 510 - Bluetooth Headset Carrying Case", "ELCB000FOM68A6", 100);
            PromotionEntry itemXnotspecified = new PromotionEntry("", "510 Headset Charger", "ELCB000GKLGX46", 100);
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            //Exsist promotion
            //X promotion entry set is ELCB000PBOWNK6, ELCB000GKLGX46
            //Exclude set to off
            //Y promotion entry is ELCB000JNYWBG6
            //Max Y quantity is 3
            //Amount 15 percent


            //First use case. X entry not contains in promotion X entry set
            itemXnotspecified.Quantity = 2;
            sourceSet.Entries.Add(itemXnotspecified);
            //Two Y entry one 1 quantity , other 10 quantity
            targetSet.Entries.Add(itemY1);
            targetSet.Entries.Add(itemY2);

            PromotionContext context = PrepareMarketingContext(sourceSet, targetSet);
            MarketingContext.Current.EvaluatePromotions(true, context, filter);
            Assert.IsTrue(context.PromotionResult.PromotionRecords.Count == 0);


            //Second use case. X entry contains in promotion X entry set
            sourceSet.Entries.Clear();
            sourceSet.Entries.Add(itemXspecified);
            context = PrepareMarketingContext(sourceSet, targetSet);
            MarketingContext.Current.EvaluatePromotions(true, context, filter);
            Assert.IsTrue(context.PromotionResult.PromotionRecords.Count == 1);
            foreach (PromotionItemRecord record in context.PromotionResult.PromotionRecords)
            {
                Assert.IsTrue(record.AffectedEntriesSet.TotalQuantity == 3);
                Assert.IsTrue(record.PromotionReward.AmountOff == 15);
            }

            //Third use case. X entry empty. Y entry specified. First instanse of item Y becomes the eligible item X, and charged bu regular price
            sourceSet.Entries.Clear();
            targetSet.Entries.Clear();
            itemY1.Quantity = 3;
            targetSet.Entries.Add(itemY1);
            context = PrepareMarketingContext(sourceSet, targetSet);
            MarketingContext.Current.EvaluatePromotions(true, context, filter);
            Assert.IsTrue(context.PromotionResult.PromotionRecords.Count == 1);
            foreach (PromotionItemRecord record in context.PromotionResult.PromotionRecords)
            {
                Assert.IsTrue(record.AffectedEntriesSet.TotalQuantity == 2);
                Assert.IsTrue(record.PromotionReward.AmountOff == 15);
            }

        }

        private PromotionContext PrepareMarketingContext(PromotionEntriesSet sourceSet, PromotionEntriesSet targetSet)
        {
            PromotionContext retVal = null;
            MarketingContext ctx = MarketingContext.Current;
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.MarketingProfileContext);
            //IDictionary<string, object> ctx = new Dictionary<string, object>();
            retVal = new PromotionContext(ctx.MarketingProfileContext, sourceSet, targetSet);

            return retVal;
        }
    }
}
