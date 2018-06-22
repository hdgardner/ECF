using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Commerce.Marketing.Dto;
using System.Web.Security;
using Mediachase.Commerce.Profile;
using System.Threading;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    /// <summary>
    /// This is an activity that calculates and applies discounts to a particular order group.
    /// This can be used out of the box or as a basis for a different promotion engine.
    /// </summary>
	public partial class CalculateDiscountsActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(CalculateDiscountsActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(CalculateDiscountsActivity));

        /// <summary>
        /// Gets or sets the order group.
        /// </summary>
        /// <value>The order group.</value>
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public OrderGroup OrderGroup
        {
            get
            {
                return (OrderGroup)(base.GetValue(CalculateDiscountsActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(CalculateDiscountsActivity.OrderGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public StringDictionary Warnings
        {
            get
            {
                return (StringDictionary)(base.GetValue(CalculateDiscountsActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(CalculateDiscountsActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateDiscountsActivity"/> class.
        /// </summary>
        public CalculateDiscountsActivity()
		{
			InitializeComponent();
        }

        /// <summary>
        /// Executes the specified execution context.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Validate the properties at runtime
                this.ValidateRuntime();

                // Initialize Marketing Context
                this.InitMarketingContext();

                // Calculate order discounts
                this.CalculateDiscounts();

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch
            {
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
                throw;
            }
        }

        /// <summary>
        /// Inits the marketing context.
        /// </summary>
        private void InitMarketingContext()
        {
            OrderGroup group = this.OrderGroup;
            SetContext(MarketingContext.ContextConstants.ShoppingCart, group);

            // Set customer segment context
            MembershipUser user = ProfileContext.Current.User;
            if (user != null)
            {
                CustomerProfile profile = ProfileContext.Current.Profile;

                if (profile != null)
                {
                    SetContext(MarketingContext.ContextConstants.CustomerProfile, profile);

                    Account account = profile.Account;
                    if (account != null)
                    {
                        SetContext(MarketingContext.ContextConstants.CustomerAccount, account);

                        Guid accountId = account.PrincipalId;
                        Guid organizationId = Guid.Empty;
                        if (account.Organization != null)
                        {
                            organizationId = account.Organization.PrincipalId;
                        }

                        SetContext(MarketingContext.ContextConstants.CustomerSegments, MarketingContext.Current.GetCustomerSegments(accountId, organizationId));
                    }
                }
            }

            // Set customer promotion history context
            SetContext(MarketingContext.ContextConstants.CustomerId, ProfileContext.Current.UserId);

            // Now load current order usage dto, which will help us determine the usage limits
            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, group.OrderGroupId);
            SetContext(MarketingContext.ContextConstants.PromotionUsage, usageDto);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        private void SetContext(string key, object val)
        {
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(key))
                MarketingContext.Current.MarketingProfileContext.Add(key, val);
            else
                MarketingContext.Current.MarketingProfileContext[key] = val;
        }

        /// <summary>
        /// Calculates the discounts.
        /// </summary>
        private void CalculateDiscounts()
        {
            // Get current context 
            Dictionary<string, object> context = MarketingContext.Current.MarketingProfileContext;

            // Parameter that tells if we need to use cached values for promotions or not
            bool useCache = false;

            // Constract the filter, ignore conditions for now
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            filter.IgnorePolicy = false;
            filter.IgnoreSegments = false;
            filter.IncludeCoupons = false;

            // Get property
            OrderGroup order = this.OrderGroup;

            // Create Promotion Context
            PromotionEntriesSet sourceSet = null;

            // Reuse the same context so we can track exclusivity properly
            PromotionContext promoContext = new PromotionContext(context, new PromotionEntriesSet(), new PromotionEntriesSet());
            promoContext.PromotionResult.RunningTotal = order.SubTotal;

            #region Determine Line item level discounts
            int totalNumberOfItems = 0;

            // Process line item discounts first
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (OrderFormDiscount discount in form.Discounts)
                {
                    if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                        discount.Delete();
                }

                // Create source from current form
                sourceSet = CreateSetFromOrderForm(form);

                // Now cycle through each line item one by one
                foreach (LineItem lineItem in form.LineItems)
                {
                    // First remove items
                    foreach (LineItemDiscount discount in lineItem.Discounts)
                    {
                        if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                            discount.Delete();
                    }

                    totalNumberOfItems++;

                    // Target only entry promotions
                    PromotionEntriesSet targetSet = new PromotionEntriesSet();
                    targetSet.OwnerId = form.OrderFormId.ToString();
					//ET [16.06.2009] If order contains two item with same code, in target hit only first
                    //targetSet.Entries.Add(sourceSet.FindEntryByCode(lineItem.CatalogEntryId));
					targetSet.Entries.Add(CreatePromotionEntryFromLineItem(lineItem));

                    promoContext.SourceEntriesSet = sourceSet;
                    promoContext.TargetEntriesSet = targetSet;

                    promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;

                    // Evaluate conditions
                    MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter);

                    // from now on use cache
                    useCache = true;
                }
            }
            #endregion

            #region Determine Order level discounts
            foreach (OrderForm form in order.OrderForms)
            {
                // Now process global order discounts
                // Now start processing it
                // Create source from current form
                sourceSet = CreateSetFromOrderForm(form);
                promoContext.SourceEntriesSet = sourceSet;
                promoContext.TargetEntriesSet = sourceSet;
                promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
            }

            // Evaluate conditions
            MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter);
            #endregion

            #region Determine Shipping Discounts
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (Shipment shipment in form.Shipments)
                {
                    // Remove old discounts if any
                    foreach (ShipmentDiscount discount in shipment.Discounts)
                    {
                        if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                            discount.Delete();
                    }

                    // Create content for current shipment
                    /*
                    sourceSet = CreateSetFromOrderForm(form);                    
                    promoContext.SourceEntriesSet.Entries = sourceSet.Entries;
                     * */
                    PromotionEntriesSet targetSet = CreateSetFromShipment(shipment);
                    promoContext.SourceEntriesSet = targetSet;
                    promoContext.TargetEntriesSet = targetSet;
                    promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key;

                    // Evaluate promotions
                    MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter);

                    // Set the total discount for the shipment
                    // shipment.ShippingDiscountAmount = GetDiscountPrice(order, promoContext.PromotionResult);
                }
            }

            #endregion

            #region Start Applying Discounts
            decimal runningTotal = order.SubTotal;
            foreach (PromotionItemRecord itemRecord in promoContext.PromotionResult.PromotionRecords)
            {
                if(itemRecord.Status != PromotionItemRecordStatus.Commited)
                    continue;

                // Pre process item record
                PreProcessItemRecord(order, itemRecord);

                // Applies discount and adjusts the running total
				if (itemRecord.AffectedEntriesSet.Entries.Count > 0)
					runningTotal -= ApplyItemDiscount(order, itemRecord, runningTotal);
            }
            #endregion
        }

        /// <summary>
        /// Pre processes item record adding additional LineItems if needed.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="record">The record.</param>
        private void PreProcessItemRecord(OrderGroup order, PromotionItemRecord record)
        {
            // We do special logic for the gift promotion reward
            if (record.PromotionReward is GiftPromotionReward)
            {
                // Check if item already in the cart, if not add
                if (((GiftPromotionReward)record.PromotionReward).AddStrategy == GiftPromotionReward.Strategy.AddWhenNeeded)
                {
                    // We assume that all affected entries are the gifts that need to be added to the cart
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
						LineItem item = FindLineItemByCatalogEntryId(order, entry.CatalogEntryCode);

                        // Didn't find, add it
                        if (item == null)
                        {
                            // we should some kind of delegate or common implementation here so we can use the same function in both discount and front end
                            Entry catEntry = CatalogContext.Current.GetCatalogEntry(entry.CatalogEntryCode);
                            LineItem lineItem = CreateLineItem(catEntry, entry.Quantity);

                            // Need to determine which order form the entry belongs to (which should be the same as the target entry)

                            // hack: add to the first
                            order.OrderForms[0].LineItems.Add(lineItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the line item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        private LineItem CreateLineItem(Entry entry, decimal quantity)
        {
            LineItem lineItem = new LineItem();

            // If entry has a parent, add parents name
            if (entry.ParentEntry != null)
            {
                lineItem.DisplayName = String.Format("{0}: {1}", entry.ParentEntry.Name, entry.Name);
                lineItem.ParentCatalogEntryId = entry.ParentEntry.ID;
            }
            else
            {
                lineItem.DisplayName = entry.Name;
                lineItem.ParentCatalogEntryId = String.Empty;
            }

            lineItem.CatalogEntryId = entry.ID;
            //Price price = StoreHelper.GetSalePrice(entry, quantity);
			//entry.ItemAttributes always null
            //lineItem.ListPrice = entry.ItemAttributes.ListPrice.Amount;
            lineItem.PlacedPrice = lineItem.ListPrice;
            lineItem.ExtendedPrice = lineItem.ListPrice;
            lineItem.MaxQuantity = entry.ItemAttributes.MaxQuantity;
            lineItem.MinQuantity = entry.ItemAttributes.MinQuantity;
            lineItem.Quantity = quantity;
            return lineItem;
        }

        /// <summary>
        /// Applies the item discount.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="record">The record.</param>
        /// <param name="totalAmount">The total amount.</param>
        /// <returns></returns>
        private decimal ApplyItemDiscount(OrderGroup order, PromotionItemRecord record, decimal totalAmount)
        {

            decimal discountAmount = 0;
            if (record.PromotionReward.RewardType == PromotionRewardType.AllAffectedEntries)
            {
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * record.PromotionReward.AmountOff / 100;
                    decimal averageDiscountAmount = discountAmount / record.AffectedEntriesSet.TotalQuantity;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        // Sasha: changed back, CostPerEntry does not change dynamically, while total cost does
						// AddDiscountToLineItem(order, record, entry, entry.CostPerEntry * entry.Quantity * record.PromotionReward.AmountOff / 100m, 0);
                        // AddDiscountToLineItem(order, record, entry.CatalogEntryCode, averageDiscountAmount * entry.Quantity, 0);
                        AddDiscountToLineItem(order, record, entry, averageDiscountAmount * entry.Quantity, 0);
                    }
                }
                else // need to split discount between all items
                {
                    discountAmount = record.PromotionReward.AmountOff;
                    decimal averageDiscountAmount = record.PromotionReward.AmountOff / record.AffectedEntriesSet.TotalQuantity;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, averageDiscountAmount * entry.Quantity, 0);
                    }
                }
            }
            else if (record.PromotionReward.RewardType == PromotionRewardType.EachAffectedEntry)
            {
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * record.PromotionReward.AmountOff / 100;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, entry.CostPerEntry * entry.Quantity * record.PromotionReward.AmountOff / 100, 0);
                    }
                }
                else
                {
                    discountAmount = record.AffectedEntriesSet.TotalQuantity * record.PromotionReward.AmountOff;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, record.PromotionReward.AmountOff * entry.Quantity, 0);
                    }
                }
            }
            else if (record.PromotionReward.RewardType == PromotionRewardType.WholeOrder)
            {
                decimal percentageOffTotal = 0;
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    // calculate percentage adjusted by the running amount, so it will be a little less if running amount is less than total
                    percentageOffTotal = (record.PromotionReward.AmountOff / 100) * (totalAmount / record.AffectedEntriesSet.TotalCost);
                    //percentageOffTotal = PromotionReward.AmountOff / 100;
                    discountAmount = totalAmount * record.PromotionReward.AmountOff / 100;
                }
                else
                {
                    // Calculate percentage off discount price
                    percentageOffTotal = record.PromotionReward.AmountOff / totalAmount;

                    // but since CostPerEntry is not an adjusted price, we need to take into account additional discounts already applied
                    percentageOffTotal = percentageOffTotal * (totalAmount / record.AffectedEntriesSet.TotalCost);

                    discountAmount = record.PromotionReward.AmountOff;
                }

                // Now distribute discount amount evenly over all entries taking into account running total
                // Special case for shipments, we consider WholeOrder to be a shipment
                if (!record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, 0, (((entry.CostPerEntry * entry.Quantity)/* - entry.Discount*/)) * percentageOffTotal);
                    }
                }
            }

            // Save discounts
            if (record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key, StringComparison.OrdinalIgnoreCase))
            {
                OrderFormDiscount discount = FindOrderFormDiscountById(order, record.PromotionItem.DataRow.PromotionId, Int32.Parse(record.AffectedEntriesSet.OwnerId));

                if (discount == null)
                    discount = new OrderFormDiscount();

                discount.DiscountAmount = record.PromotionItem.DataRow.OfferAmount;
                discount.DiscountCode = record.PromotionItem.DataRow.CouponCode;
                discount.DiscountName = record.PromotionItem.DataRow.Name;
                discount.DiscountValue = record.PromotionItem.DataRow.OfferAmount;
                discount.DisplayMessage = GetDisplayName(record.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                discount.OrderFormId = Int32.Parse(record.AffectedEntriesSet.OwnerId);
                discount.DiscountId = record.PromotionItem.DataRow.PromotionId;

                foreach (OrderForm form in order.OrderForms)
                {
                    if(form.OrderFormId == discount.OrderFormId)
                        form.Discounts.Add(discount);
                }
            }
            else if (record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key, StringComparison.OrdinalIgnoreCase))
            {
                ShipmentDiscount discount = FindShipmentDiscountById(order, record.PromotionItem.DataRow.PromotionId, Int32.Parse(record.AffectedEntriesSet.OwnerId));

                if (discount == null)
                    discount = new ShipmentDiscount();

                discount.DiscountAmount = record.PromotionItem.DataRow.OfferAmount;
                discount.DiscountCode = record.PromotionItem.DataRow.CouponCode;
                discount.DiscountName = record.PromotionItem.DataRow.Name;
                discount.DiscountValue = record.PromotionItem.DataRow.OfferAmount;
                discount.DisplayMessage = GetDisplayName(record.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                discount.ShipmentId = Int32.Parse(record.AffectedEntriesSet.OwnerId);
                discount.DiscountId = record.PromotionItem.DataRow.PromotionId;

                // Only apply discount to the shipment if it is a whole order, otherwise we keep discount on the lineitem level
                if (record.PromotionReward.RewardType == PromotionRewardType.WholeOrder)
                {
                    foreach (OrderForm form in order.OrderForms)
                    {
                        foreach (Shipment shipment in form.Shipments)
                        {
                            if (shipment.ShipmentId == discount.ShipmentId)
                            {
                                shipment.Discounts.Add(discount);
                                shipment.ShippingDiscountAmount = discountAmount; // save shipment discount amount
                                break;
                            }
                        }
                    }
                }
            }

            return discountAmount;
        }

        /// <summary>
        /// Adds the discount to line item.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="itemRecord">The item record.</param>
        /// <param name="entryCode">The entry code.</param>
        /// <param name="itemDiscount">The item discount.</param>
        /// <param name="orderLevelDiscount">The order level discount.</param>
        private void AddDiscountToLineItem(OrderGroup order, PromotionItemRecord itemRecord, PromotionEntry promotionEntry, 
										  decimal itemDiscount, decimal orderLevelDiscount)
        {
			LineItem item = FindLineItemByPromotionEntry(order, promotionEntry);
            if (item != null)
            {
                // Add line item properties
                item.LineItemDiscountAmount += itemDiscount;
                item.OrderLevelDiscountAmount += orderLevelDiscount;
                item.ExtendedPrice = item.ListPrice * item.Quantity - item.LineItemDiscountAmount - item.OrderLevelDiscountAmount;

                if (itemRecord.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key, StringComparison.OrdinalIgnoreCase))
                {
                    LineItemDiscount discount = FindLineItemDiscountById(order, itemRecord.PromotionItem.DataRow.PromotionId, item.LineItemId);

                    if (discount == null)
                        discount = new LineItemDiscount();

                    discount.DiscountAmount = itemRecord.PromotionItem.DataRow.OfferAmount;
                    discount.DiscountCode = itemRecord.PromotionItem.DataRow.CouponCode;
                    discount.DiscountName = itemRecord.PromotionItem.DataRow.Name;
                    discount.DiscountValue = itemRecord.PromotionItem.DataRow.OfferAmount;
                    discount.DisplayMessage = GetDisplayName(itemRecord.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                    discount.LineItemId = item.LineItemId;
                    discount.DiscountId = itemRecord.PromotionItem.DataRow.PromotionId;
                    item.Discounts.Add(discount);
                }
            }
        }

		private static LineItem FindLineItemByCatalogEntryId(OrderGroup order, string catalogEntryId)
		{
			LineItem retVal = null;
			foreach (OrderForm form in order.OrderForms)
			{
				foreach (LineItem item in form.LineItems)
				{
					if (item.CatalogEntryId == catalogEntryId)
					{
						retVal = item;
						break;
					}
				}

				if (retVal != null)
					break;
			}
			return retVal;
		}

        /// <summary>
        /// Finds the line item by code.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="entryCode">The entry code.</param>
        /// <returns></returns>
        private static LineItem FindLineItemByPromotionEntry(OrderGroup order, PromotionEntry prmotionEntry)
        {
			LineItem retVal = null;

            foreach (OrderForm form in order.OrderForms)
            {
                foreach (LineItem item in form.LineItems)
                {
					if (item == prmotionEntry.Owner)
					{
						retVal = item;
						break;
					}
                }

				if (retVal != null)
					break;
            }

			return retVal;
        }

        /// <summary>
        /// Finds the order form discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="orderFormId">The order form id.</param>
        /// <returns></returns>
        private OrderFormDiscount FindOrderFormDiscountById(OrderGroup order, int promotionId, int orderFormId)
        {
            foreach (OrderForm form in order.OrderForms)
            {
                if (form.OrderFormId == orderFormId)
                {
                    foreach (OrderFormDiscount discount in form.Discounts)
                    {
                        if (discount.DiscountId == promotionId)
                            return discount;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the shipment discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="shipmentId">The shipment id.</param>
        /// <returns></returns>
        private ShipmentDiscount FindShipmentDiscountById(OrderGroup order, int promotionId, int shipmentId)
        {
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (Shipment shipment in form.Shipments)
                {
                    if (shipment.ShipmentId == shipmentId)
                    {
                        foreach (ShipmentDiscount discount in shipment.Discounts)
                        {
                            if (discount.DiscountId == promotionId)
                                return discount;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the line item discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        private LineItemDiscount FindLineItemDiscountById(OrderGroup order, int promotionId, int lineItemId)
        {
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    foreach (LineItemDiscount discount in lineItem.Discounts)
                    {
                        if (discount.DiscountId == promotionId && discount.LineItemId == lineItemId)
                            return discount;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        private string GetDisplayName(PromotionDto.PromotionRow row, string languageCode)
        {
            PromotionDto.PromotionLanguageRow[] langRows = row.GetPromotionLanguageRows();
            if (langRows != null && langRows.Length > 0)
            {
                foreach (PromotionDto.PromotionLanguageRow lang in langRows)
                {
                    if (lang.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return lang.DisplayName;
                    }
                }
            }

            return row.Name;
        }

        /// <summary>
        /// Creates the set from order form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        private PromotionEntriesSet CreateSetFromOrderForm(OrderForm form)
        {
            PromotionEntriesSet set = new PromotionEntriesSet();
            set.OwnerId = form.OrderFormId.ToString();

            foreach (LineItem lineItem in form.LineItems)
            {
                set.Entries.Add(CreatePromotionEntryFromLineItem(lineItem));
            }

            return set;
        }

        /// <summary>
        /// Creates the set from shipment.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
        private PromotionEntriesSet CreateSetFromShipment(Shipment shipment)
        {
            PromotionEntriesSet set = new PromotionEntriesSet();
            set.OwnerId = shipment.ShipmentId.ToString();
            foreach (string lineItemIndex in shipment.LineItemIndexes)
            {
                LineItem lineItem = shipment.Parent.LineItems[Int32.Parse(lineItemIndex)];

                if (lineItem != null)
                {
                    PromotionEntry entry = CreatePromotionEntryFromLineItem(lineItem);
                    set.Entries.Add(entry);
                }
            }

            return set;
        }

        /// <summary>
        /// Creates the promotion entry from line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns></returns>
        private PromotionEntry CreatePromotionEntryFromLineItem(LineItem lineItem)
        {
            PromotionEntry entry = new PromotionEntry(lineItem.Catalog, lineItem.CatalogNode, lineItem.CatalogEntryId, lineItem.ListPrice);
            ((IPromotionEntryPopulate)MarketingContext.Current.PromotionEntryPopulateFunctionClassInfo.CreateInstance()).Populate(ref entry, lineItem);
            return entry;
        }

        /// <summary>
        /// Validates the runtime.
        /// </summary>
        /// <returns></returns>
        private bool ValidateRuntime()
        {
            // Create a new collection for storing the validation errors
            ValidationErrorCollection validationErrors = new ValidationErrorCollection();

            // Validate the Order Properties
            this.ValidateOrderProperties(validationErrors);

            // Raise an exception if we have ValidationErrors
            if (validationErrors.HasErrors)
            {
                string validationErrorsMessage = String.Empty;

                foreach (ValidationError error in validationErrors)
                {
                    validationErrorsMessage +=
                        string.Format("Validation Error:  Number {0} - '{1}' \n",
                        error.ErrorNumber, error.ErrorText);
                }

                // Throw a new exception with the validation errors.
                throw new WorkflowValidationFailedException(validationErrorsMessage, validationErrors);
            }


            // If we made it this far, then the data must be valid. 
            return true;
        }

        /// <summary>
        /// Validates the order properties.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        private void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(CalculateDiscountsActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
	}
}
