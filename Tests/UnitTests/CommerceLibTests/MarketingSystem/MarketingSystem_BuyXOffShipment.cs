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
    /// Summary description for MarketingSystem_BuyXOffShipment
    /// </summary>
    [TestClass]
    public class MarketingSystem_BuyXOffShipment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingSystem_BuyXOffShipment"/> class.
        /// </summary>
        public MarketingSystem_BuyXOffShipment()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        /// Merketings the system_ buy X off shipment_10 percent off.
        /// </summary>
        [TestMethod]
        public void MerketingSystem_BuyXOffShipment_10PercentOff()
        {
            // three Promotion #1 - 10 MinQuantity, 5$ reward; 
            //                 #2 - 20 MinQuantity, 10$ Reward; 
            //                 #3 - 30 MinQuantity 15$ Reward.
            PromotionEntriesSet sourceSet = new PromotionEntriesSet();
            PromotionEntry entry = new PromotionEntry("", "", "Plasma-EDTV", 100);
            entry.Quantity = 20;
            sourceSet.Entries.Add(entry);
            MarketingContext ctx = MarketingContext.Current;
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.MarketingProfileContext);
            //IDictionary<string, object> ctx = new Dictionary<string, object>();
            PromotionContext context = new PromotionContext(ctx.MarketingProfileContext, sourceSet, sourceSet);
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            MarketingContext.Current.EvaluatePromotions(true, context, filter);
            foreach (PromotionItemRecord record in context.PromotionResult.PromotionRecords)
            {
                decimal amountOff = record.PromotionReward.AmountOff;
                // Validate promotion
                Assert.IsTrue(amountOff == 10);
            }
        }
    }
}
