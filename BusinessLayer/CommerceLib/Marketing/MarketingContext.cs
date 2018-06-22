using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using System.Web.Security;
using System.ServiceModel;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Marketing.Dto;
using System.Web;
using System.Threading;
using Mediachase.MetaDataPlus;
using System.Web.Profile;
using System.Data;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Class serves as a mediator for all the functions that can be performed in the marketing system.
    /// </summary>
    public class MarketingContext
    {
        /// <summary>
        /// List of constants used by the Marketing System
        /// </summary>
        public struct ContextConstants
        {
            /// <summary>
            /// Public string literal for the shopping cart session context.
            /// </summary>
            public static string ShoppingCart = "Session.Cart";
            /// <summary>
            /// Public string literal for the coupons session context.
            /// </summary>
            public static string Coupons = "Session.Coupons";
            /// <summary>
            /// Public string literal for the customer profile session context.
            /// </summary>
            public static string CustomerProfile = "Session.CustomerProfile";
            /// <summary>
            /// Public string literal for the customer segments context.
            /// </summary>
            public static string CustomerSegments = "Customer.Segments";
            /// <summary>
            /// Public string literal for the customer orders context.
            /// </summary>
            public static string CustomerOrders = "Customer.Orders";
            /// <summary>
            /// Public string literal for the customer account context.
            /// </summary>
            public static string CustomerAccount = "Customer.Account";
            /// <summary>
            /// Public string literal for the customer id context.
            /// </summary>
            public static string CustomerId = "Customer.CustomerId";
            /// <summary>
            /// Public string literal for the session promotion usage context.
            /// </summary>            
            public static string PromotionUsage = "Session.PromotionUsage";
        }

        private static volatile MarketingContext _Instance = null;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static MarketingContext Current
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new MarketingContext();
                        }
                    }
                }

                return _Instance;
            }
        }

        private static MetaDataContext _mdContext;
        /// <summary>
        /// Gets or sets the meta data context.
        /// </summary>
        /// <value>The meta data context.</value>
        public static MetaDataContext MetaDataContext
        {
            get
            {
                if (_mdContext == null)
                    _mdContext = new MetaDataContext(MarketingConfiguration.Instance.Connection.AppDatabase);

                return _mdContext;
            }
            set
            {
                _mdContext = value;
            }
        }

        private ClassInfo _ExpressionValidatorClassInfo;
        private ClassInfo _PromotionEntryPopulateFunctionClassInfo;

        #region Class Definitions
        /// <summary>
        /// Gets the expression validator class info.
        /// </summary>
        /// <value>The expression validator class info.</value>
        internal ClassInfo ExpressionValidatorClassInfo
        {
            get
            {
                return _ExpressionValidatorClassInfo;
            }
        }

        /// <summary>
        /// Gets the promotion entry populate function class info.
        /// </summary>
        /// <value>The promotion entry populate function class info.</value>
        public ClassInfo PromotionEntryPopulateFunctionClassInfo
        {
            get
            {
                return _PromotionEntryPopulateFunctionClassInfo;
            }
        }
        #endregion

        #region Context Functions
        /// <summary>
        /// Gets the marketing profile context. Marketing profile defines context that is used by the marketing engine.
        /// 
        /// Context Objects:
        ///     - Session.Cart
        ///     - Session.Attributes
        ///     - Session.Coupons
        ///     - Membership.Profile
        ///     - Membership.User
        ///     - Customer.Segments
        /// </summary>
        /// <value>The marketing profile.</value>
        public Dictionary<string, object> MarketingProfileContext
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    object ctxThread = Thread.GetData(Thread.GetNamedDataSlot("MarketingProfileContext"));
                    if (ctxThread != null)
                        return (Dictionary<string, object>)ctxThread;
                    else
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        Thread.SetData(Thread.GetNamedDataSlot("MarketingProfileContext"), dic);
                        return dic;
                    }
                }
                else
                {
                    if (HttpContext.Current.Items.Contains("MarketingProfileContext"))
                        return (Dictionary<string, object>)HttpContext.Current.Items["MarketingProfileContext"];
                    else
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        HttpContext.Current.Items["MarketingProfileContext"] = dic;
                        return dic;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the context value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void SetContextValue(string name, string value)
        {
            if (MarketingProfileContext.ContainsKey(name))
            {
                MarketingProfileContext[name] = value;
            }
            else
            {
                MarketingProfileContext.Add(name, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingContext"/> class.
        /// </summary>
        MarketingContext()
        {
            MarketingConfiguration config = MarketingConfiguration.Instance;
            MappedTypes types = config.MappedTypes;
            _ExpressionValidatorClassInfo = new ClassInfo(types.ExpressionValidatorType.Name);
            _PromotionEntryPopulateFunctionClassInfo = new ClassInfo(types.PromotionEntryPopulateFunctionType.Name);
        }

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetMarketingSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = MarketingDataHelper.CreateDataCommand();
			command.CommandText = "GetMarketingSchemaVersionNumber";
			DataResult result = DataService.LoadDataSet(command);
			if (result.DataSet != null)
			{
				if (result.DataSet.Tables.Count > 0 && result.DataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = result.DataSet.Tables[0].Rows[0];
					major = (int)row["Major"];
					minor = (int)row["Minor"];
					patch = (int)row["Patch"];
					installDate = (DateTime)row["InstallDate"];
				}
			}

			return retval;
		}

        #region Promotion Items Functions
        /// <summary>
        /// Gets the marketing helper.
        /// </summary>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <returns></returns>
        private MarketingHelper GetMarketingHelper(bool useCache)
        {
            MarketingHelper helper = null;

            string cacheKey = MarketingCache.CreateCacheKey("MarketingHelper");

            if (useCache)
            {
                object cachedObject = MarketingCache.Get(cacheKey);

                if (cachedObject != null)
                    helper = (MarketingHelper)cachedObject;
            }

            // If marketing is not initialized, init it
            if (helper == null)
            {
                // Load promotion Dto
                PromotionDto promotionDto = PromotionManager.GetPromotionDto(FrameworkContext.Current.CurrentDateTime);

                // Get all the data from the database first
                helper = new MarketingHelper(CampaignManager.GetCampaignDto(), ExpressionManager.GetExpressionDto(), PolicyManager.GetPolicyDto(), promotionDto, SegmentManager.GetSegmentDto());

                // Insert cache
                //if (useCache)
                MarketingCache.Insert(cacheKey, helper, MarketingConfiguration.Instance.CacheConfig.PromotionCollectionTimeout);
            }

            return helper;
        }
 
        /// <summary>
        /// Evaluates the promotions.
        /// </summary>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <param name="context">The context.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Collection of promotions that were applied. Look inside PromotionContext for actual rewards and for items that have been affected by these promotions.</returns>
        public PromotionItemCollection EvaluatePromotions(bool useCache, PromotionContext context, PromotionFilter filter)
        {
            MarketingHelper helper = GetMarketingHelper(useCache);
            PromotionItemCollection dto = new PromotionItemCollection(helper);

            // Remove current customer promotion history if we opted to not use cache (which happens during checkout)
            if (!useCache)
            {
                string cacheKey = MarketingCache.CreateCacheKey("MarketingHelper-customer", context.CustomerId.ToString());
                MarketingCache.Remove(cacheKey);
            }

            return EvaluatePromotions(context, dto, filter);
        }

        /// <summary>
        /// Evaluates the promotions.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="promotions">The promotions.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Collection of promotions that were applied. Look inside PromotionContext for actual rewards and for items 
        /// that have been affected by these promotions.</returns>
        public PromotionItemCollection EvaluatePromotions(PromotionContext context, PromotionItemCollection promotions, PromotionFilter filter)
        {
            // Start checking discounts
            List<int> rowIndexes = new List<int>();

            // Retrieve all the coupons customer entered
            List<string> coupons = context.Coupons;

            // Retrieve customer segments, it should be initialized beforehand
            List<int> segments = context.Segments;

            // Retrieve customer id, it should be initialized beforehand
            Guid customerId = context.CustomerId;

            foreach (PromotionItem item in promotions)
            {
                // Set currently executed promotion
                context.CurrentPromotion = item;

                // If discount is global and other discount has been applied already, skip it
                if (item.DataRow.ExclusivityType.Equals(ExclusionType.GlobalLevel) && rowIndexes.Count != 0)
                    continue;

                // Check if it belongs to a group specified
                if (!String.IsNullOrEmpty(context.TargetGroup) && !context.TargetGroup.Equals(item.DataRow.PromotionGroup, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Check group exclusivity
                if (context.ExclusiveGroups.Contains(item.DataRow.PromotionGroup))
                    continue;

                // Check limits here
                int currentOrderPromotionCount = context.PromotionResult.GetCount(item.DataRow.PromotionId);

                // Start with per order limit
                if (item.DataRow.PerOrderLimit > 0 && currentOrderPromotionCount >= item.DataRow.PerOrderLimit)
                    continue;

                // Check application limit limit, only check when limit is set
                if (item.DataRow.ApplicationLimit > 0 && item.TotalUsedCount - context.ReservedCount + currentOrderPromotionCount >= item.DataRow.ApplicationLimit)
                    continue;

                // Check customer limit, only check when limit is set and customer id is set
                if ((item.DataRow.CustomerLimit > 0 && customerId == Guid.Empty) || item.DataRow.CustomerLimit > 0 && item.GetCustomerUsageCount(customerId) - context.ReservedCount + currentOrderPromotionCount >= item.DataRow.CustomerLimit)
                    continue;

                // First do simply checks that will take little time and save us from wasting processor time
                // -----------------------------------------------------------------------------------------
                if (!filter.IncludeInactive)
                {
                    // Skip if start date is past now
                    if (!item.Campaign.IsActive)
                        continue;

                    if (item.Campaign.StartDate > FrameworkContext.Current.CurrentDateTime)
                        continue;

                    // Skip if end date is in the past
                    if (item.Campaign.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue;

                    // Skip if start date is past now
                    if (item.DataRow.StartDate > FrameworkContext.Current.CurrentDateTime)
                        continue;

                    // Skip if end date is in the past
                    if (item.DataRow.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue;

                    // Check promotion status 
                    if (!item.DataRow.Status.Equals(PromotionStatus.Active))
                        continue;
                }

                if (!filter.IncludeCoupons)
                {
                    // Check coupons
                    string couponCode = item.DataRow.CouponCode;
                    if (!String.IsNullOrEmpty(couponCode))
                    {
                        if (coupons == null)
                        {
                            continue;
                        }

                        bool foundCoupon = false;
                        foreach(string coupon in coupons)
                        {
                            if(couponCode.Equals(coupon, StringComparison.OrdinalIgnoreCase))
                            {
                                foundCoupon = true;
                            }
                        }

                        if(!foundCoupon)
                            continue;
                    }
                }

                // Check catalog / node / entry filtering
                if (context.TargetEntriesSet.Entries.Count > 0)
                {
                    bool isValid = true;
                    foreach (PromotionEntry entry in context.TargetEntriesSet.Entries)
                    {
                        string catalogEntryId = entry.CatalogEntryCode;
                        string catalogNodeId = entry.CatalogNodeCode;
                        string catalogName = entry.CatalogName;

                        if (!String.IsNullOrEmpty(catalogEntryId) || !String.IsNullOrEmpty(catalogNodeId) || !String.IsNullOrEmpty(catalogName))
                        {
                            PromotionDto.PromotionConditionRow[] conditions = item.DataRow.GetPromotionConditionRows();
                            if (conditions != null && conditions.Length > 0)
                            {
                                foreach (PromotionDto.PromotionConditionRow condition in conditions)
                                {
                                    if (!String.IsNullOrEmpty(catalogEntryId) && !condition.IsCatalogEntryIdNull() && !condition.CatalogEntryId.Equals(catalogEntryId))
                                    {
                                        isValid = false;
                                        break;
                                    }
                                    else if (!String.IsNullOrEmpty(catalogNodeId) && !condition.IsCatalogNodeIdNull() && !condition.CatalogNodeId.Equals(catalogNodeId))
                                    {
                                        isValid = false;
                                        break;
                                    }
                                    else if (!String.IsNullOrEmpty(catalogName) && !condition.IsCatalogNameNull() && !condition.CatalogName.Equals(catalogName))
                                    {
                                        isValid = false;
                                        break;
                                    }
                                }

                                if (!isValid)
                                    continue;
                            }
                        }
                    }
                    if (!isValid)
                        continue;
                }


                // Start doing more expensive checks here
                // -----------------------------------------------------------------------------------------
                if (!filter.IgnoreSegments)
                {
                    // Check customer segments, customer should belong to a segment for promotion to apply
                    CampaignDto.CampaignSegmentRow[] segmentRows = item.Campaign.GetCampaignSegmentRows();

                    // if there are no segments defined assume it applies to everyone
                    if (segmentRows != null && segmentRows.Length > 0)
                    {
                        // customer is not within any segment, so promotion does not apply
                        if (segments == null || segments.Count == 0)
                            continue;

                        // start checking segments
                        bool apply = false;
                        foreach (CampaignDto.CampaignSegmentRow row in segmentRows)
                        {
                            if (segments.Contains(row.SegmentId))
                            {
                                // mark promotion as apply and leave loop
                                apply = true;
                                break;
                            }
                        }

                        // if does not apply continue with a next promotion
                        if (!apply)
                            continue;
                    }
                }

                if (!filter.IgnoreConditions)
                {
                    // Validate expressions
                    if (item.Expressions.Count > 0)
                    {
                        bool isValid = true;
                        foreach (ExpressionDto.ExpressionRow expression in item.Expressions)
                        {
                            if (!String.IsNullOrEmpty(expression.ExpressionXml))
                            {
                                ValidationResult result = ValidateExpression(expression.ApplicationId.ToString() + "-" + expression.Category + "-" + expression.ExpressionId.ToString(), expression.ExpressionXml, context);
                                if (!result.IsValid)
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }

                        if (!isValid)
                            continue;
                    }
                }
                else
                {
                    // Create Award manually based on default settings
                    PromotionReward reward = new PromotionReward(PromotionRewardType.EachAffectedEntry, item.DataRow.OfferAmount, item.DataRow.OfferType == 0 ? PromotionRewardAmountType.Percentage : PromotionRewardAmountType.Value);
                    PromotionItemRecord record = new PromotionItemRecord(context.TargetEntriesSet, context.TargetEntriesSet, reward);
                    record.PromotionItem = context.CurrentPromotion;
                    context.PromotionResult.PromotionRecords.Add(record);
                }

                if (!filter.IgnorePolicy)
                {
                    // Validate store policies
                    if (item.PolicyExpressions.Count > 0)
                    {
                        bool isValid = true;
                        foreach (ExpressionDto.ExpressionRow expression in item.PolicyExpressions)
                        {
                            if (!String.IsNullOrEmpty(expression.ExpressionXml))
                            {
                                ValidationResult result = ValidateExpression(expression.ApplicationId.ToString() + "-" + expression.Category + "-" + expression.ExpressionId.ToString(), expression.ExpressionXml, context);
                                if (!result.IsValid)
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }

                        if (!isValid)
                        {
                            // Invalidates all records added during this evaluation sequence
                            context.RejectRecords();
                            continue;
                        }
                    }
                }

                // Commits all records added during this evaluation sequence
                context.CommitRecords();

                // Apply item
                rowIndexes.Add(item.RowIndex);

                // Add item to a group if it is applied
                if (item.DataRow.ExclusivityType.Equals(ExclusionType.GroupLevel))
                    context.ExclusiveGroups.Add(item.DataRow.PromotionGroup);

                // Finish processing if global level item has been added
                if (item.DataRow.ExclusivityType.Equals(ExclusionType.GlobalLevel))
                    break;
            }

            // Assign curren promotion to null
            context.CurrentPromotion = null;

            return new PromotionItemCollection(promotions, rowIndexes.ToArray());
        }

        #endregion

        #region Customer Segments Functions

        /// <summary>
        /// Returns customer segments for specified customer or organization using current MarketingProfileContext.
        /// </summary>
        /// <param name="customerPrincipalId">The customer principal id.</param>
        /// <param name="organizationPrincipalId">The organization principal id.</param>
        /// <returns></returns>
        public List<int> GetCustomerSegments(Guid customerPrincipalId, Guid organizationPrincipalId)
        {
            return GetCustomerSegments(MarketingProfileContext, customerPrincipalId, organizationPrincipalId);
        }

        /// <summary>
        /// Returns customer segments for specified customer or organization.
        /// </summary>
        /// <param name="context">The context. Should include ContextConstants.CustomerProfile of type ProfileBase</param>
        /// <param name="customerPrincipalId">The customer principal id.</param>
        /// <param name="organizationPrincipalId">The organization principal id.</param>
        /// <returns>int list of customer segment ids</returns>
        public List<int> GetCustomerSegments(Dictionary<string, object> context, Guid customerPrincipalId, Guid organizationPrincipalId)
        {
            List<int> segments = new List<int>();
            MarketingHelper helper = GetMarketingHelper(true);
            SegmentDto segmentsDto = helper.Segments;

            PromotionContext promotionContext = new PromotionContext(context, null, null);

            // Cycle through customer segments
            foreach (SegmentDto.SegmentRow segmentRow in segmentsDto.Segment)
            {
                bool includeSegment = false;
                bool excludeSegment = false;

                // Check if profile is directly specified in the members tables
                foreach (SegmentDto.SegmentMemberRow memberRow in segmentRow.GetSegmentMemberRows())
                {
                    if (memberRow.PrincipalId == customerPrincipalId || memberRow.PrincipalId == organizationPrincipalId)
                    {
                        if (memberRow.Exclude)
                        {
                            excludeSegment = true;
                            break;
                        }
                        else
                        {
                            includeSegment = true;
                            break;
                        }
                    }
                }

                // if explicitely excluded, skip the segment
                if (excludeSegment)
                    continue;

                // Validate expressions, if any of the expression is met, user is part of the group
                foreach (SegmentDto.SegmentConditionRow condition in segmentRow.GetSegmentConditionRows())
                {
                    ExpressionDto.ExpressionRow expressionRow = helper.Expressions.Expression.FindByExpressionId(condition.ExpressionId);

                    if (expressionRow != null && !String.IsNullOrEmpty(expressionRow.ExpressionXml))
                    {
                        ValidationResult result = ValidateExpression(expressionRow.ApplicationId.ToString() + "-" + expressionRow.Category + "-" + expressionRow.ExpressionId.ToString(), expressionRow.ExpressionXml, promotionContext);
                        if (!result.IsValid)
                        {
                            includeSegment = false;
                            continue;
                        }
                        else
                        {
                            includeSegment = true;
                            break;
                        }
                    }
                }

                if (!includeSegment)
                    continue;

                segments.Add(segmentRow.SegmentId);
            }

            return segments;
        }
        #endregion

        #region Expression Validator Functions
        /// <summary>
        /// Validates the expression.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public ValidationResult ValidateExpression(string key, string expr, PromotionContext context)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PromotionContext", context);
            return ValidateExpression(key, expr, dic);
        }

        /// <summary>
        /// Validates the expression using the validator configured in the marketing settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Validation results, which will either say it is successfully passed or not.
        /// </returns>
        public ValidationResult ValidateExpression(string key, string expr, IDictionary<string, object> context)
        {
            return ((IExpressionValidator)MarketingContext.Current.ExpressionValidatorClassInfo.CreateInstance()).Eval(key, expr, context);
        }
        #endregion
    }
}