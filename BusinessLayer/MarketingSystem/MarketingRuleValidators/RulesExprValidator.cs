using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities.Rules;
using System.IO;
using System.Xml;
using System.Workflow.ComponentModel.Serialization;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Marketing.Objects;
using System.Collections;
using System.Workflow.ComponentModel.Compiler;
using System.Linq;
using System.Reflection;
using Mediachase.Commerce.Marketing.Validators.Providers;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Marketing.Validators
{
    /// <summary>
    /// The context object that will be passed to the Rules Engine. Provides way to do intellisense
    /// rules creation. Class can also be extended with custom functionality if needed, as well as 
    /// functions that might be useful during rules creation.
    /// </summary>
    public partial class RulesContext
    {
        IDictionary<string, object> _context;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IDictionary<string, object> Context
        {
            get
            {
                return _context;
            }
        }

        Hashtable _runtimeContext = new Hashtable();

        /// <summary>
        /// Runtime context which can be used to store variables in the rules engine.
        /// </summary>
        /// <value>The runtime context.</value>
        public Hashtable RuntimeContext
        {
            get
            {
                return _runtimeContext;
            }
        }

        ValidationResult _Results = new ValidationResult(false, String.Empty);

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>The validation result.</value>
        public ValidationResult ValidationResult
        {
            get
            {
                return _Results;
            }
            set
            {
                _Results = value;
            }
        }

        /// <summary>
        /// Gets the promotion context.
        /// </summary>
        /// <value>The promotion context.</value>
        public PromotionContext PromotionContext
        {
            get
            {
                if (Context.ContainsKey("PromotionContext"))
                {
                    return (PromotionContext)Context["PromotionContext"];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the shopping cart.
        /// </summary>
        /// <value>The shopping cart.</value>
        public OrderGroup ShoppingCart
        {
            get
            {
                if (PromotionContext.Context.ContainsKey(MarketingContext.ContextConstants.ShoppingCart))
                {
                    return (OrderGroup)PromotionContext.Context[MarketingContext.ContextConstants.ShoppingCart];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the customer profile.
        /// </summary>
        /// <value>The customer profile.</value>
        public CustomerProfile CustomerProfile
        {
            get
            {
                if (PromotionContext.Context.ContainsKey(MarketingContext.ContextConstants.CustomerProfile))
                {
                    return (CustomerProfile)PromotionContext.Context[MarketingContext.ContextConstants.CustomerProfile];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the customer account.
        /// </summary>
        /// <value>The customer account.</value>
        public Account CustomerAccount
        {
            get
            {
                if (PromotionContext.Context.ContainsKey(MarketingContext.ContextConstants.CustomerAccount))
                {
                    return (Account)PromotionContext.Context[MarketingContext.ContextConstants.CustomerAccount];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <value>The orders.</value>
        public PurchaseOrder[] Orders
        {
            get
            {
                if (PromotionContext.Context.ContainsKey(MarketingContext.ContextConstants.CustomerOrders))
                {
                    return (PurchaseOrder[])PromotionContext.Context[MarketingContext.ContextConstants.CustomerOrders];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the promotion target line item.
        /// </summary>
        /// <value>The promotion target line item.</value>
		public PromotionEntry PromotionTargetLineItem
		{
			get
			{
				if (PromotionContext.TargetGroup != PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key)
				{
					throw new InvalidOperationException("promotion target group not Entry");
				}

				if (PromotionContext.TargetEntriesSet.Entries.Count != 1)
					throw new IndexOutOfRangeException("TargetEntriesSet.Entries");

				//var entrys = PreparePromotionEntries(PromotionContext.TargetEntriesSet.Entries);
				return PromotionContext.TargetEntriesSet.Entries.First();
			}
		}

        /// <summary>
        /// Gets the promotion current order form.
        /// </summary>
        /// <value>The promotion current order form.</value>
		public OrderForm PromotionCurrentOrderForm
		{
			get
			{
				OrderForm retVal = new OrderForm();

				int orderFormId;
				if (!Int32.TryParse(PromotionContext.SourceEntriesSet.OwnerId, out orderFormId))
				{
					orderFormId = -1;
				}

				if (this.ShoppingCart != null)
				{
					foreach (OrderForm orderForm in ShoppingCart.OrderForms)
					{
						if (orderFormId == -1 || orderForm.OrderFormId == orderFormId)
						{
							retVal = orderForm;
							break;
						}
					}
				}
				return retVal;
			}
		}

		/// <summary>
		/// Gets the promotion order form billing address.
		/// </summary>
		/// <value>The promotion order form billing address.</value>
		public OrderAddress PromotionOrderFormBillingAddress
		{
			get
			{
				OrderAddress retVal = new OrderAddress();
				if (ShoppingCart != null)
				{
					OrderForm orderForm = PromotionCurrentOrderForm;
					
					foreach(OrderAddress billingAddress in ShoppingCart.OrderAddresses)
					{
						if (billingAddress.Name == orderForm.BillingAddressId)
						{
							retVal = billingAddress;
							break;
						}
					}
				}
				return retVal;
			}

		}
        /// <summary>
        /// Gets the promotion current shipment.
        /// </summary>
        /// <value>The promotion current shipment.</value>
		public Shipment PromotionCurrentShipment
		{
			get
			{
				if (PromotionContext.TargetGroup != 
					PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key)

				{
					throw new InvalidOperationException("promotion target group not Shiping");
				}
				Shipment retVal = new Shipment();

				foreach (Shipment shipment in PromotionCurrentOrderForm.Shipments)
				{
					int shipmentId = Convert.ToInt32(PromotionContext.SourceEntriesSet.OwnerId);
					if (shipment.ShipmentId == shipmentId)
					{
						retVal = shipment;
						break;
					}
				}
				return retVal;

			}
		}


        /// <summary>
        /// Prepares the promotion entries.
        /// </summary>
        /// <param name="entrys">The entries.</param>
        /// <returns></returns>
		[Obsolete("Do not use", true)]
		public IEnumerable<PromotionEntry> PreparePromotionEntries(IEnumerable<PromotionEntry> entries)
		{
			foreach (PromotionEntry entry in entries)
			{
				PromotionEntry retVal = entry;
				LineItem ownerLineItem = retVal.Owner != null ? retVal.Owner as LineItem : null;
				if (ownerLineItem == null)
				{
					foreach (LineItem lineItem in this.PromotionCurrentOrderForm.LineItems)
					{
						if (lineItem.CatalogEntryId == entry.CatalogEntryCode)
						{
							ownerLineItem = lineItem;
							break;
						}
					}
				}
				if (ownerLineItem == null)
				{
					ownerLineItem = new LineItem();
				}

				//PromotionFilterExpressionProvider.CopyLineItemTopromotionEntryProperty(retVal, ownerLineItem);

				object shippingAddressId = retVal["ShippingAddressId"];
				if (this.ShoppingCart != null && shippingAddressId != null)
				{
					//found OrderAddress by address id
					OrderAddress orderAddress = null;
					foreach (OrderAddress address in this.ShoppingCart.OrderAddresses)
					{
						if (address.Name.ToString() == shippingAddressId.ToString())
						{
							orderAddress = address;
							break;
						}
					}
					//if (orderAddress != null)
					//{
					//    //copy all properties OrderAddress to entry
					//    PromotionFilterExpressionProvider.CopyOrderAddressToPromotionEntryProperty(retVal, orderAddress);
					//}
				}

				yield return retVal;
			}
		}

		/// <summary>
		/// Adds the free gift to cart.
		/// </summary>
		/// <remarks> This method using in promotion actions rulset</remarks>
		/// <param name="quantity">The quantity.</param>
		/// <param name="entryCodes">The entry codes.</param>
		public void AddFreeGiftToCart(string quantity, params string[] entryCodes)
		{
			decimal giftQuantity = Convert.ToDecimal(quantity, System.Globalization.CultureInfo.InvariantCulture);

			GiftPromotionReward giftAddWhenNeedReward = new GiftPromotionReward(PromotionRewardType.EachAffectedEntry, 0, 
																				 PromotionRewardAmountType.Percentage, 
																				GiftPromotionReward.Strategy.AddWhenNeeded);
			PromotionEntriesSet giftEntrySet = new PromotionEntriesSet();
			giftEntrySet.OwnerId = this.PromotionContext.TargetEntriesSet.OwnerId;
			foreach (string entryCode in entryCodes)
			{
				PromotionEntry entry = new PromotionEntry(entryCode, entryCode, entryCode, 0);
				entry.Quantity = giftQuantity;
				giftEntrySet.Entries.Add(entry);
			}

			this.AddPromotionItemRecord(giftAddWhenNeedReward, giftEntrySet);
		}

        #region Design UI Helper Functions
        /// <summary>
        /// Gets the account property string value using the safe method. Meaning it will always return a string object
        /// even if the object is null.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetAccountPropertyStringValueSafe(string propertyName)
        {
            string ret = String.Empty;
            Account acc = CustomerAccount;
            if (acc != null)
            {
                try
                {
                    ret = (string)acc[propertyName];
                }
                catch
                {
                }
            }

            return ret;
        }

		#region Collection condition methods
        /// <summary>
        /// BaseRuleSetFilterExpressionProvider must interpret all collection expression to string call 
        /// one of methods ( ValidateAll, ValidateAny, ConditionCollectionCount ) with CodeExpression for one of collection item
        /// Example: ConditionCollectionAll(this.CustomerAccount.Addresses, "this.Organization == "Org1" && this.Line1 = "line"")
        /// exactly second parameter must be converted to CodeExpression statement by Parser in provider. 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <example>ConditionCollectionAll(this.CustomerAccount.Addresses, "this.Organization == \"Org1\" && this.Line1 = \"line\"");</example>
        /// <returns></returns>
		private static bool PredicateFunction(object item, System.CodeDom.CodeExpression codeExpr)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (codeExpr == null)
				throw new ArgumentNullException("codeExpr");

			bool retVal = false;
			RuleSet rs = new RuleSet();
			Rule rule = new Rule("collection expression rule");

			RuleExpressionCondition cond = new RuleExpressionCondition(codeExpr);
			RuleValidation validation = new RuleValidation(item.GetType(), null);
			rule.Condition = cond;
			rs.Rules.Add(rule);
			rs.Validate(validation);
			if (validation.Errors.Count > 0)
			{
				throw new Exception("Error count " + validation.Errors.Count);
			}
			RuleExecution execution = new RuleExecution(validation, item);
			retVal = cond.Evaluate(execution);

			return retVal;
		}

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <param name="strExpr">The STR expr.</param>
        /// <returns></returns>
		private static System.CodeDom.CodeExpression ParseExpression(string strExpr)
		{
			Mediachase.Commerce.Marketing.Validators.Providers.DomParser.Parser p =
										new Mediachase.Commerce.Marketing.Validators.Providers.DomParser.Parser();
			//Parser bug. Need replace nested double quotes to unary quotes. Is back rewriting.
			if (strExpr != null)
			{
				strExpr = strExpr.Replace('\'', '\"');
			}
			return p.ParseExpression(strExpr);
		}

		/// <summary>
		/// Validates all.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="strCodeExpr">The STR code expr.</param>
		/// <returns></returns>
		public  bool ValidateAll(object obj, string strCodeExpr)
		{
			return ValidateAll(ConvertToEnumerable(obj), strCodeExpr);
		}

        /// <summary>
        /// Returns true if all of the items in the collection satisfies the specified condition.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="strCodeExpr">The STR code expr.</param>
        /// <returns></returns>
		public  bool ValidateAll(IEnumerable collection, string strCodeExpr)
		{
			return ValidateAll(collection, ParseExpression(strCodeExpr));
		}

        /// <summary>
        /// Returns true if all of the items in the collection satisfies the specified condition.
        /// </summary>
        /// <remarks>
        /// Returns false if collection does not contain any elements.
        /// </remarks>
        /// <param name="collection">The collection.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <returns></returns>
		public  bool ValidateAll(IEnumerable collection, System.CodeDom.CodeExpression codeExpr)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (codeExpr == null)
				throw new ArgumentNullException("codeExpr");

			bool retVal = false; // will return false if no items in the collection
			foreach (object item in collection)
			{
                // Set to true, since we found some items in the collection
                retVal = true;
				if (!PredicateFunction(item, codeExpr))
				{
					retVal = false;
					break;
				}
			}
			return retVal;
			//return collection.All(x => PredicateFunction(x, codeExpr));
		}


		/// <summary>
		/// Validates any.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="strCodeExpr">The STR code expr.</param>
		/// <returns></returns>
		public  bool ValidateAny(object obj, string strCodeExpr)
		{
			return ValidateAny(ConvertToEnumerable(obj), strCodeExpr);
		}

        /// <summary>
        /// Returns true if any of the items in the collection satisfies the specified condition.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="strCodeExpr">The STR code expr.</param>
        /// <returns></returns>
		public  bool ValidateAny(IEnumerable collection, string strCodeExpr)
		{
			return ValidateAny(collection, ParseExpression(strCodeExpr));
		}

        /// <summary>
        /// Returns true if any of the items in the collection satisfies the specified condition.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <returns></returns>
		public  bool ValidateAny(IEnumerable collection, System.CodeDom.CodeExpression codeExpr)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (codeExpr == null)
				throw new ArgumentNullException("codeExpr");

			bool retVal = false;
			foreach (object item in collection)
			{
				if (PredicateFunction(item, codeExpr))
				{
					retVal = true;
					break;
				}
			}

			return retVal;
			//return collection.Any(x => PredicateFunction(x, codeExpr));
		}

		/// <summary>
		/// Gets the validated count.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="strCodeExpr">The STR code expr.</param>
		/// <returns></returns>
		public  int GetValidatedCount(object obj, string strCodeExpr)
		{
			return GetValidatedCount(ConvertToEnumerable(obj), strCodeExpr);
		}
        /// <summary>
        /// Returns the number of items that satisfied the specified condition.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="strCodeExpr">The STR code expr.</param>
        /// <returns></returns>
		public  int GetValidatedCount(IEnumerable collection, string strCodeExpr)
		{
            return GetValidatedCount(collection, ParseExpression(strCodeExpr));
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public  int GetCount(object obj)
		{
			return GetCount(ConvertToEnumerable(obj));
		}
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		public  int GetCount(IEnumerable collection)
		{
			int retVal = 0;
			foreach (object item in collection)
			{
				retVal++;
			}

			return retVal;
		}

        /// <summary>
        /// Returns the number of items that satisfied the specified condition.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <returns></returns>
		public  int GetValidatedCount(IEnumerable collection, System.CodeDom.CodeExpression codeExpr)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (codeExpr == null)
				throw new ArgumentNullException("codeExpr");
			int retVal = 0;
			foreach (object item in collection)
			{
				if(PredicateFunction(item, codeExpr))
				{
					retVal++;
				}
			}
			return retVal;
			//return collection.Where(x => PredicateFunction(x, codeExpr)).Count();
		}


		/// <summary>
		/// Gets the collection sum.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="codeExpr">The code expr.</param>
		/// <returns></returns>
		public  Decimal GetCollectionSum(object obj, string fieldName, string codeExpr)
		{
			return GetCollectionSum(ConvertToEnumerable(obj), fieldName, codeExpr);
		}

        /// <summary>
        /// Gets the collection sum.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <returns></returns>
		public  Decimal GetCollectionSum(IEnumerable collection, string fieldName, string codeExpr)
		{
			return GetCollectionSum(collection, fieldName, ParseExpression(codeExpr));
		}

        /// <summary>
        /// Gets the collection sum.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="codeExpr">The code expr.</param>
        /// <returns></returns>
		public  Decimal GetCollectionSum(IEnumerable collection, string fieldName, System.CodeDom.CodeExpression codeExpr)
		{
			//All digist types convert to Decimal withouts miss value if overflow not occurs
			Decimal retVal = Decimal.Zero;
			PropertyInfo pi = null;
			string[] indexProperty = null;
			foreach (object item in collection)
			{
				object propertyValue = null;
				if (pi == null)
				{
					//Get item type once
					Type itemType = item.GetType();
					//Try get from property by name
					pi = itemType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
					if (pi == null)
					{
						//Try get from indexer property by name
						pi = itemType.GetProperty("Item");
						indexProperty = new string[] { fieldName };
					}
				}

				if (pi != null)
				{
					propertyValue = pi.GetValue(item, indexProperty);

					if (propertyValue != null && PredicateFunction(item, codeExpr))
					{
						try
						{
							retVal += Convert.ToDecimal(propertyValue);
						}
						catch (InvalidCastException)
						{
						}
					}
				}
			}

			return retVal;
		}

		private static IEnumerable ConvertToEnumerable(object obj)
		{
			IEnumerable retVal = obj as IEnumerable;
			if (retVal == null)
			{
				retVal = new object[] { obj };
			}

			return retVal;
		}
	
		#endregion
        #endregion

        #region Helper Functions
        /// <summary>
        /// Finds the line item by entry code.
        /// </summary>
        /// <param name="entryCode">The entry code.</param>
        /// <param name="orderGroup">The order group.</param>
        /// <returns></returns>
        public LineItem FindLineItemByEntryCode(string entryCode, OrderGroup orderGroup)
        {
            foreach (OrderForm orderForm in orderGroup.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    if (lineItem.CatalogEntryId == entryCode || lineItem.ParentCatalogEntryId == entryCode)
                        return lineItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the line item by catalog node code.
        /// </summary>
        /// <param name="nodeCode">The node code.</param>
        /// <param name="orderGroup">The order group.</param>
        /// <returns></returns>
        public LineItem FindLineItemByCatalogNodeCode(string nodeCode, OrderGroup orderGroup)
        {
            foreach (OrderForm orderForm in orderGroup.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    if (lineItem.CatalogNode == nodeCode)
                        return lineItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the promotion item record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public PromotionItemRecord AddPromotionItemRecord(PromotionItemRecord record)
        {
            PromotionContext.PromotionResult.PromotionRecords.Add(record);
            return record;
        }

		
		/// <summary>
		/// Adds the promotion item record.
		/// </summary>
		/// <param name="reward">The reward.</param>
		/// <param name="entriesCollection">The entries collection.</param>
		/// <returns></returns>
		public void AddPromotionItemRecord(PromotionReward reward, params PromotionEntriesSet[] entrySetsCollection)
		{
			foreach (PromotionEntriesSet entrySet in entrySetsCollection)
			{
				PromotionItemRecord record = new PromotionItemRecord(this.PromotionContext.TargetEntriesSet, entrySet, reward);
				record.PromotionItem = PromotionContext.CurrentPromotion;
				PromotionContext.PromotionResult.PromotionRecords.Add(record);
			}
		}
	
		/// <summary>
		/// Creates the promotion entries set from target.
		/// </summary>
		/// <param name="entryCode">The entry code.</param>
		/// <param name="quantity">The quantity.</param>
		/// <returns></returns>
		public PromotionEntriesSet CreatePromotionEntriesSetFromTarget(string entryCode)
		{
			return CreatePromotionEntriesSetFromTarget(entryCode, decimal.MaxValue);
		}

        /// <summary>
        /// Creates the entry from target.
        /// </summary>
        /// <param name="entryCode">The entry code.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public PromotionEntriesSet CreatePromotionEntriesSetFromTarget(string entryCode, decimal quantity)
        {
			PromotionEntriesSet retVal = new PromotionEntriesSet();
			retVal.OwnerId = this.PromotionContext.TargetEntriesSet.OwnerId;
            foreach (PromotionEntry entry in PromotionContext.TargetEntriesSet.Entries)
            {
				if (quantity == 0)
					break;

                if (entry.CatalogEntryCode.Equals(entryCode, StringComparison.CurrentCultureIgnoreCase))
                {
					PromotionEntry entryToAdd = (PromotionEntry)entry.Clone();

					entryToAdd.Quantity = Math.Min(quantity, entryToAdd.Quantity);
					quantity -= entryToAdd.Quantity;

					retVal.Entries.Add(entryToAdd);
                }
            }
			return retVal;
        }

		/// <summary>
		/// Creates the promotion reward.
		/// </summary>
		/// <param name="rewardType">Type of the reward.</param>
		/// <param name="amountOff">The amount off.</param>
		/// <param name="amountType">Type of the amount.</param>
		/// <returns></returns>
		public PromotionReward CreatePromotionReward(string rewardType, decimal amountOff, string amountType)
		{
			return new PromotionReward(rewardType, amountOff, amountType);
		}

		/// <summary>
		/// Creates the promotion reward. And validate input parameters
		/// </summary>
		/// <param name="rewardType">Type of the reward.</param>
		/// <param name="amountOff">The amount off.</param>
		/// <param name="amountType">Type of the amount.</param>
		/// <returns></returns>
		public PromotionReward CreatePromotionReward(string rewardType, string amountOff, string amountType)
		{
			if (String.IsNullOrEmpty(rewardType))
			{
				throw new ArgumentNullException("rewardType");
			}
			if (String.IsNullOrEmpty(amountOff))
			{
				throw new ArgumentNullException("amountOff");
			}
			if (String.IsNullOrEmpty(amountType))
			{
				throw new ArgumentNullException("amountType");
			}
			switch (rewardType)
			{
				case PromotionRewardType.WholeOrder:
				case PromotionRewardType.EachAffectedEntry:
				case PromotionRewardType.AllAffectedEntries:
					break;
				default:
					throw new ArgumentException("invalid promotion reward type");
			}
			switch (amountType)
			{
				case PromotionRewardAmountType.Value:
				case PromotionRewardAmountType.Percentage:
					break;
				default:
					throw new ArgumentException("invalid promotion reward amount type");
			}
			
			decimal amountValue = Convert.ToDecimal(amountOff, System.Globalization.CultureInfo.InvariantCulture);

			return CreatePromotionReward(rewardType, amountValue, amountType);
		}
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RulesContext(IDictionary<string, object> context)
        {
            _context = context;
        }
    }

    /// <summary>
    /// Rule Expression Validator based on Windows Workflow Foundation Rules Engine.
    /// </summary>
    public class RulesExprValidator : IExpressionValidator
    {
        #region IExpressionValidator Members
        /// <summary>
        /// Evals the specified expression against the context passed in the dictionary.
        /// </summary>
        /// <param name="key">The key. Must be a unique key identifying the current expression. It might be used for caching purpose by the engine.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="context">The context, which consists of object that will be accessible during expression evaluation.</param>
        /// <returns></returns>
        public ValidationResult Eval(string key, string expr, IDictionary<string, object> context)
        {
            string cacheKey = MarketingCache.CreateCacheKey("Expression-engine", key);

            // check cache first
            object cachedObject = MarketingCache.Get(cacheKey);

            RuleEngine ruleEngine = null;

            // Return true if empty
            if(String.IsNullOrEmpty(expr))
                return new ValidationResult(true, String.Empty);

            if (cachedObject != null)
                ruleEngine = (RuleEngine)cachedObject;
            else
            {
                lock (MarketingCache.GetLock(cacheKey))
                {
                    ruleEngine = (RuleEngine)MarketingCache.Get(cacheKey);
                    if (ruleEngine == null)
                    {
                        // Try to deserialize rules
                        RuleSet ruleSet = DeserializeRuleSet(expr);

                        // If empty, return false
                        if(ruleSet == null)
                            return new ValidationResult(false, String.Empty);

                        ruleEngine = new RuleEngine(ruleSet, new RuleValidation(typeof(RulesContext), null));

                        // Save in cache
                        MarketingCache.Insert(cacheKey, ruleEngine, MarketingConfiguration.Instance.CacheConfig.ExpressionCollectionTimeout);
                    }
                }
            }

            // Initialize context object
            RulesContext rulesContext = new RulesContext(context);

            if (ruleEngine == null)
                 throw new NullReferenceException("RuleEngine object can not be null");

			try
			{
				ruleEngine.Execute(rulesContext);
			}
			catch (RuleSetValidationException ex)
			{
				string errorMessage = String.Empty;
				foreach (ValidationError error in ex.Errors)
				{
					errorMessage += "\n\r" + error.ToString();
				}
				throw new ApplicationException(errorMessage, ex);
			}
			catch (RuleEvaluationException ex)
			{
				//If runtime error then skip promotion
				System.Diagnostics.Trace.WriteLine(String.Format("evaluation error {0} ... skip promotion", ex.Message));
			}
			catch (TargetInvocationException ex)
			{
				//If runtime error then skip promotion
				System.Diagnostics.Trace.WriteLine(String.Format("evaluation error {0} ... skip promotion", ex.Message));
			}				
			
			
            // Return result
            return rulesContext.ValidationResult;
        }
        #endregion

		
        /// <summary>
        /// Deserializes the rule set.
        /// </summary>
        /// <param name="ruleSetXmlDefinition">The rule set XML definition.</param>
        /// <returns></returns>
        private RuleSet DeserializeRuleSet(string ruleSetXmlDefinition)
        {
            if (!String.IsNullOrEmpty(ruleSetXmlDefinition))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                StringReader stringReader = new StringReader(ruleSetXmlDefinition);
                XmlTextReader reader = new XmlTextReader(stringReader);
                return serializer.Deserialize(reader) as RuleSet;
            }
            else
            {
                return null;
            }
        }
    }
}
