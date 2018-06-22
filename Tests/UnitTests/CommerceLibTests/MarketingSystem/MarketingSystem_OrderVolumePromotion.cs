using System;
using System.Text;
using System.Collections.Generic;
using MetaDataTest.Common;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Engine.Template;
using System.Threading;
using System.Net.Mail;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Configuration;

namespace UnitTests.MarketingSystem
{
    /// <summary>
    /// Summary description for MarketingSystem_OrderVolumePromotion
    /// </summary>
    [TestClass]
    public class MarketingSystem_OrderVolumePromotion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingSystem_OrderVolumePromotion"/> class.
        /// </summary>
        public MarketingSystem_OrderVolumePromotion()
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
        /// Merketings the system_ order volume promotion_10 percent off.
        /// </summary>
        [TestMethod]
        public void MerketingSystem_OrderVolumePromotion_10PercentOff()
        {
            PromotionEntriesSet sourceSet = new PromotionEntriesSet();
            //Add entry totalCost equals 2000 
            sourceSet.Entries.Add(new PromotionEntry("", "", "42-Plasma-EDTV", 2000));
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
