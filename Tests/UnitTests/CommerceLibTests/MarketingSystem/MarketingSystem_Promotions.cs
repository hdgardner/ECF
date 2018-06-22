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
    /// Test class for payment plans
    /// </summary>
    [TestClass]
    public class MarketingSystem_Promotion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingSystem_Promotion"/> class.
        /// </summary>
        public MarketingSystem_Promotion()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        /// <summary>
        /// Initialize performance.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        [ClassInitialize()]
        public static void PerformanceInitialize(TestContext testContext)
        {
            //MetaDataContext.Current = new MetaDataContext(OrderConfiguration.Instance.Connections.ConfigurationAppDatabase);
        }
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
        /// Marketings the system_ buy X get X discount.
        /// </summary>
        [TestMethod]
        public void MarketingSystem_BuyXGetXDiscount()
        {
            PromotionEntriesSet sourceSet = new PromotionEntriesSet();            
            sourceSet.Entries.Add(new PromotionEntry("", "", "42-Plasma-EDTV", 1));
            MarketingContext ctx = MarketingContext.Current;
            Assert.IsNotNull(ctx);
            Assert.IsNotNull(ctx.MarketingProfileContext);
            //IDictionary<string, object> ctx = new Dictionary<string, object>();
            PromotionContext context = new PromotionContext(ctx.MarketingProfileContext, sourceSet, sourceSet);
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            MarketingContext.Current.EvaluatePromotions(true, context, filter);
            foreach(PromotionItemRecord record in context.PromotionResult.PromotionRecords)
            {
                // Validate promotion
                Assert.IsTrue(record.PromotionReward.AmountOff == 10);
            }
        }
    }
}