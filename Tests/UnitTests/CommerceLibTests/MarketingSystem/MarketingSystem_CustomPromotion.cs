using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Commerce.Orders;
using MetaDataTest.Common;
using System.Workflow.Activities.Rules;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Marketing.Validators.Providers;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using System.Data;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Data;
using Mediachase.Commerce.Marketing.Validators;

namespace UnitTests.MarketingSystem
{
	/// <summary>
	/// Extension method for friendly access to OrderForm.LineItems collection
	/// </summary>
	public static class LineItemCollExtension
	{
		public static LineItem FirstOrDefault(this LineItemCollection lineItemColl, Func<LineItem, bool> predicate)
		{
			LineItem retVal = null;

			foreach (LineItem lineItem in lineItemColl)
			{
				if (predicate(lineItem))
				{
					retVal = lineItem;
					break;
				}
			}

			return retVal;
		}
	}


	/// <summary>
	/// Summary description for CustomPromotionTest
	/// </summary>
	[TestClass]
	public class MarketingSystem_CustomPromotionTest
	{

		private class PromotionDef
		{
			public string Name;
			public decimal RewardAmount;
			public decimal RewardQuantity;
		}


		private List<PromotionBuilder> _promoBuilderScope = new List<PromotionBuilder>();

		public MarketingSystem_CustomPromotionTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		private PromotionBuilder GetPromotionBuilder(string name, PromotionGroup.PromotionGroupKey prmotionGroupTarget,
													 FilterExpressionProvider conditionProvider, FilterExpressionProvider actionProvider)
		{
			PromotionBuilder retVal = _promoBuilderScope.FirstOrDefault(x => x.Name == name);
			if (retVal == null)
			{
				//TODO: Get Campaing id
				retVal = PromotionBuilder.CreateInstance(name, 0, "", PromotionStatus.Active, AllCampaingsId.First(), prmotionGroupTarget,
														 conditionProvider, actionProvider);
				_promoBuilderScope.Add(retVal);
			}
			return retVal;
		}

		private IEnumerable<int> _allCampaingsId;

		public IEnumerable<int> AllCampaingsId
		{
			get
			{
				if (_allCampaingsId == null)
				{
					CampaignDto dto = CampaignManager.GetCampaignDto();
					List<int> campaingIds = new List<int>();
					foreach (CampaignDto.CampaignRow row in dto.Campaign.Rows)
					{
						campaingIds.Add(row.CampaignId);
					}
					_allCampaingsId = campaingIds;
				}

				return _allCampaingsId;
			}
		}
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

		// Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void MyClassCleanup()
		{
			//TODO: Need Cleanup all promotion created in tests
		}

		/// <summary>
		/// Clean all promotion from datatbase after each test
		/// </summary>
		[TestCleanup()]
		public void MyTestCleanup()
		{
			foreach (PromotionBuilder promoBuilder in _promoBuilderScope)
			{
				promoBuilder.DeletePromotion();
			}
		}

		#region Promotion target Entry
		[TestMethod]
		public void MarketingSystem_CombinatedPromotion()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Create orderform
			OrderForm orderForm = CreateRandomOrderForm();
			LineItem lineItem1 = OrderHelper.CreateLineItem();
			while (orderForm.LineItems.FirstOrDefault(x => x.CatalogEntryId == lineItem1.CatalogEntryId) != null)
			{
				lineItem1 = OrderHelper.CreateLineItem();
			}
			lineItem1.Quantity = 2;
			LineItem lineItem2 = CloneLineItem(orderForm.LineItems[0]);
			lineItem2.Quantity = 3;
			orderForm.LineItems.Add(lineItem1);
			orderForm.LineItems.Add(lineItem2);
			var promotions = new PromotionDef[] { new PromotionDef() { Name = "promotion1", RewardAmount = 0m, RewardQuantity = 0m},
												  new PromotionDef() { Name = "promotion2", RewardAmount = 0m, RewardQuantity = 0m},
												  new PromotionDef() { Name= "promotion3", RewardAmount = 0m, RewardQuantity = 1m }};
			//Promotion #1 Target - OrderEntry. 
			var promotion1 = promotions.First();
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion1.Name, PromotionGroup.PromotionGroupKey.Entry,
											  conditionProvider, actionProvider);
			//Condition empty
			//Action
			var action = CreateElementDef(ElementDefs.Action_AllEntryValueOfItem, 
										  CreateElementDef(ElementDefs.Action_Condition_Decimal));
			FilterExpressionNode actionNode = promoBuilder.CreateActionExpression(action);
			actionNode.Value = promotion1.RewardAmount;

			promoBuilder.SavePromotion();

			//Promotion #2 Target - OrderForm
			var promotion2 = promotions.ElementAt(1);
			promoBuilder =  GetPromotionBuilder(promotion2.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			//Condition enpty (always TRUE)
			//Action
			action = CreateElementDef(ElementDefs.Action_AllEntryPercentOfSkuSet, 
								     CreateElementDef(ElementDefs.Action_Condition_SkuSelect),
									 CreateElementDef(ElementDefs.Method_Action_AllEntryPercent));

			actionNode = promoBuilder.CreateActionExpression(action);
			//reward amount
			actionNode.Method.Params.Add(promotion2.RewardAmount);
			//Sku list
			actionNode.Value = new Dictionary<string, string>() { { lineItem1.CatalogEntryId, lineItem1.CatalogEntryId} };

			promoBuilder.SavePromotion();

			//Promotion #3 Target - OrderForm
			var promotion3 = promotions.ElementAt(2);
			promoBuilder =  GetPromotionBuilder(promotion3.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			//Condition empty (always TRUE)
			//Action
			action = CreateElementDef(ElementDefs.Action_EachEntryValueOfSkuSet, 
									  CreateElementDef(ElementDefs.Action_Condition_SkuSelect),
									  CreateElementDef(ElementDefs.Method_Action_EachEntry));
			actionNode = promoBuilder.CreateActionExpression(action);

			actionNode.Method.Params.Add(promotion3.RewardAmount);
			actionNode.Method.Params.Add(promotion3.RewardQuantity);
			actionNode.Value = new Dictionary<string, string>() { { lineItem1.CatalogEntryId, lineItem1.CatalogEntryId }, 
																  { lineItem2.CatalogEntryId, lineItem2.CatalogEntryId} };
			promoBuilder.SavePromotion();


			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			//Check Discount presents in Order
			Assert.IsNotNull(GetPromotionInEntryByName(lineItem1, promotion1.Name), "Discount " + promotion1.Name + " not assigned for lineItem1");
			Assert.IsNotNull(GetPromotionInEntryByName(lineItem2, promotion1.Name), "Discount " + promotion1.Name + " not assigned for lineItem2");

			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion2.Name), "Discount " + promotion2.Name + " not assigned");
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion3.Name), "Discount " + promotion3.Name + " not assigned");


		}

		/// <summary>
		/// Target OrderEntry; IF TargetLineItem.DisplayName Contains [specified text]  THEN ADD specified percent discount
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderEntry_AllEntryPercentDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			//Create Order contains two same code lineItem haved another quantity
			var promotion = new PromotionDef() { Name = "AllEntryPercent_Promo", RewardAmount = 30m };
			OrderForm orderForm = CreateRandomOrderForm();
			LineItem lineItem1 = OrderHelper.CreateLineItem();
			//Generate lineItem for discount, not contains in orderForm
			while (orderForm.LineItems.FirstOrDefault(x => x.CatalogEntryId == lineItem1.CatalogEntryId) != null)
			{
				lineItem1 = OrderHelper.CreateLineItem();
			}
			orderForm.LineItems.Add(lineItem1);

			//Construct condition for promotion target Entry
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Entry,
											  conditionProvider, actionProvider);

			var condition = CreateElementDef(ElementDefs.TargetLineItem_CatalogEntryId, 
											 CreateElementDef(ElementDefs.Condition_Text_Contains));
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = lineItem1.CatalogEntryId.Substring(0, 4);

			//Actions
			var action = CreateElementDef(ElementDefs.Action_AllEntryPercentOfItem, 
										  CreateElementDef(ElementDefs.Action_Condition_PercentDecimal));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Value = promotion.RewardAmount;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			//Check Discount presents in Entry
			Assert.IsNotNull(GetPromotionInEntryByName(lineItem1, promotion.Name), "Discount " + promotion.Name + " not assigned");

			decimal discountAmount = lineItem1.ListPrice * lineItem1.Quantity * promotion.RewardAmount * 0.01m;
			Assert.IsTrue(lineItem1.LineItemDiscountAmount == discountAmount);
		}

		/// <summary>
		/// Target  OrderForm; IF ANY { Lineitems.Quantity > 0 || LineItems.DisplayName != "sometext" } THEN ADD SkuList in specified quantity as GIFT
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderForm_GiftDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			//Create Order contains two same code lineItem haved another quantity
			var promotion = new PromotionDef() { Name = "Gift_Promo_Of_SKU", RewardQuantity = 3m };
			Dictionary<string, string> skuList = new Dictionary<string, string>();
			OrderForm orderForm = CreateRandomOrderForm();
			LineItem lineItem1 = OrderHelper.CreateLineItem();
			//Generate lineItem for gift not contains in orderForm
			while (orderForm.LineItems.FirstOrDefault(x=> x.CatalogEntryId == lineItem1.CatalogEntryId) != null )
			{
				lineItem1 = OrderHelper.CreateLineItem();
			}
			skuList.Add(lineItem1.CatalogEntryId, lineItem1.CatalogEntryId);

			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			//OrderForm.LineItems.Any()
			var condition = CreateElementDef(ElementDefs.OrderForm_LineItems,
											 CreateElementDef(ElementDefs.Condition_Bool_Equals),
											 CreateElementDef(ElementDefs.Method_Any));
			//OrderForm.LineItems Any Equals true
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = true;
			//create OR block
			FilterExpressionNode orBlock = new FilterExpressionNode(FilterExpressionItem.filterOrBlock);
			orBlock.NodeType = FilterExpressionNodeType.OrBlock;
			orBlock.ChildNodes = new FilterExpressionNodeCollection();
			//Add to OR block expressions 
			FilterExpressionNode subCondition1_node =  promoBuilder.CreateConditionExpressionWithoutRegistration(CreateElementDef(ElementDefs.LineItem_Quantity, 
																											     CreateElementDef(ElementDefs.Condition_Decimal_Greater)));
			FilterExpressionNode subCondition2_node = promoBuilder.CreateConditionExpressionWithoutRegistration(CreateElementDef(ElementDefs.LineItem_DisplayName,
																												CreateElementDef(ElementDefs.Condition_Text_Equals)));
			//Always true Quantity > 0
			subCondition1_node.Value = 0m;
			//Always false DisplayName == "%(2jhda"
			subCondition2_node.Value = @"%(2jhda";
			//Add OrBlock to LineItems filter expression node
			orBlock.ChildNodes.Add(subCondition1_node);
			orBlock.ChildNodes.Add(subCondition2_node);
			condition_node.ChildNodes.Add(orBlock);

			//Create action
			var action = CreateElementDef(ElementDefs.Action_GiftOfSkuSet,
										  CreateElementDef(ElementDefs.Action_Condition_SkuSelect), 
										  CreateElementDef(ElementDefs.Method_Action_Gift));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Method.Params.Add(promotion.RewardQuantity);
			action_node.Value = skuList;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			//Check Discount presents in Order
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");

			//Check that gift ben added
			LineItem giftLineItem = orderForm.LineItems.FirstOrDefault(x => x.CatalogEntryId == lineItem1.CatalogEntryId);
			Assert.IsTrue(giftLineItem != null, string.Format("gift {0} not assigned", lineItem1.DisplayName));
			Assert.IsTrue(giftLineItem.Quantity == promotion.RewardQuantity, string.Format("gift {0} quantity != {1}", lineItem1.Quantity, promotion.RewardQuantity));
			
		}

		/// <summary>
		/// Target - OrderForm; IF TRUE THEN Add 14.32$ promotion to 8 lineitem with specified code.
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderForm_Any_EachItemDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			//Create Order contains two same code lineItem haved another quantity
			var promotion = new PromotionDef() { Name = "WholeOrder_EachItem_Promo_For_SKU", RewardAmount = 14.32m, RewardQuantity = 8m };
			OrderForm orderForm = CreateRandomOrderForm();
			Dictionary<string, string> skuList1 = new Dictionary<string, string>();
			LineItem lineItem1 = OrderHelper.CreateLineItem();
			lineItem1.ListPrice = 1m;
			lineItem1.Quantity = 5;
			LineItem lineItem2 = CloneLineItem(lineItem1);
			lineItem2.Quantity = 10;

			orderForm.LineItems.Add(lineItem1);
			orderForm.LineItems.Add(lineItem2);

			skuList1.Add(lineItem1.CatalogEntryId, lineItem1.CatalogEntryId);

			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			//Always true condition
			var condition = CreateElementDef(ElementDefs.OrderForm_Name, 
											 CreateElementDef(ElementDefs.Condition_Text_NotEquals));
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = "some string do not equals order name";

			//Construct action
			//Add discount reward value greather that entry cost for lineItem1
			var action = CreateElementDef(ElementDefs.Action_EachEntryValueOfSkuSet,
										  CreateElementDef(ElementDefs.Action_Condition_SkuSelect), 
										CreateElementDef(ElementDefs.Method_Action_EachEntry));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Method.Params.Add(promotion.RewardAmount);
			action_node.Method.Params.Add(promotion.RewardQuantity);
			action_node.Value = skuList1;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");

			//Check discount amount for each lineItem
			decimal remainQuantity = promotion.RewardQuantity;
			foreach (LineItem item in orderForm.LineItems)
			{
				if (!skuList1.ContainsKey(item.CatalogEntryId))
				{
					Assert.IsTrue(item.LineItemDiscountAmount == 0);
				}
				else
				{
					decimal effectiveQuantity = Math.Min(remainQuantity, item.Quantity);
					decimal discountAmount = effectiveQuantity * promotion.RewardAmount;
					remainQuantity -= effectiveQuantity;
					Assert.IsTrue(discountAmount == item.LineItemDiscountAmount);
				}
			}
		}

		/// <summary>
		/// Target - OrderForm: IF COUNT LineItems IN OrderForm haves specified id GREATHER  2 THEN add 13% discount for some line items
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderForm_Count_FixedPercentDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			var promotion = new PromotionDef() { Name = "WholeOrder_FixedPercent_Promo_For_SKU", RewardAmount = 13.22m };
			OrderForm orderForm = CreateRandomOrderForm();

			Dictionary<string, string> skuList = new Dictionary<string, string>();
			foreach (LineItem item in orderForm.LineItems)
			{
				if (!skuList.ContainsKey(item.CatalogEntryId))
				{
					skuList.Add(item.CatalogEntryId, item.CatalogEntryId);
				}
			}
			LineItem lineItem = OrderHelper.CreateLineItem();
			orderForm.LineItems.Add(lineItem);

			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			//Count(OrderForm.LineItems) > 2
			var condition = CreateElementDef(ElementDefs.OrderForm_LineItems,
											CreateElementDef(ElementDefs.Condition_Int_Greater), 
											CreateElementDef(ElementDefs.Method_Count));
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = 2;
			//OR block
			FilterExpressionNode orBlock = new FilterExpressionNode(FilterExpressionItem.filterOrBlock);
			orBlock.NodeType = FilterExpressionNodeType.OrBlock;
			orBlock.ChildNodes = new FilterExpressionNodeCollection();

			var subCondition1 = CreateElementDef(ElementDefs.LineItem_CatalogEntryId, 
												 CreateElementDef(ElementDefs.Condition_Text_Equals));
			var subCondition2 = CreateElementDef(ElementDefs.LineItem_CatalogEntryId,
												 CreateElementDef(ElementDefs.Condition_Text_Equals));
			var subCondition3 = CreateElementDef(ElementDefs.LineItem_CatalogEntryId,
												 CreateElementDef(ElementDefs.Condition_Text_Equals));
			FilterExpressionNode subCondition1_node = promoBuilder.CreateConditionExpressionWithoutRegistration(subCondition1);
			FilterExpressionNode subCondition2_node = promoBuilder.CreateConditionExpressionWithoutRegistration(subCondition2);
			FilterExpressionNode subCondition3_node = promoBuilder.CreateConditionExpressionWithoutRegistration(subCondition3);
			
			subCondition1_node.Value = orderForm.LineItems[0].CatalogEntryId;
			subCondition2_node.Value = orderForm.LineItems[1].CatalogEntryId;
			subCondition3_node.Value = orderForm.LineItems[2].CatalogEntryId;

			orBlock.ChildNodes.Add(subCondition1_node);
			orBlock.ChildNodes.Add(subCondition2_node);
			orBlock.ChildNodes.Add(subCondition3_node);

			condition_node.ChildNodes.Add(orBlock);

			////Where CountryCode == "USA" AND RegionName Contains "Los Angeles"
			//var subCondition1 = CreateElementDef(ElementDefs.OrderAddress_CountryCode, 
			//                                     CreateElementDef(ElementDefs.Condition_Text_Equals));
			//var subCondition2 = CreateElementDef(ElementDefs.OrderAddress_RegionName, 
			//                                     CreateElementDef(ElementDefs.Condition_Text_Contains));
			//FilterExpressionNode subCondition1_node = promoBuilder.CreateConditionExpression(condition_node,
			//                                                                                 subCondition1);
			//FilterExpressionNode subCondition2_node = promoBuilder.CreateConditionExpression(condition_node,
			//                                                                                subCondition2);
			//subCondition1_node.Value = "USA";
			//subCondition2_node.Value = "Los Angeles";

			//Construct action
			//Add 13% discount reward for specified lineItems
			var action = CreateElementDef(ElementDefs.Action_AllEntryPercentOfSkuSet,
										  CreateElementDef(ElementDefs.Action_Condition_SkuSelect),
										  CreateElementDef(ElementDefs.Method_Action_AllEntryPercent));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Method.Params.Add(promotion.RewardAmount);
			action_node.Value = skuList;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");

			foreach (LineItem item in orderForm.LineItems)
			{
				if (!skuList.ContainsKey(item.CatalogEntryId))
				{
					Assert.IsTrue(item.LineItemDiscountAmount == 0);
				}
				else
				{
					decimal discountAmount = item.ListPrice * item.Quantity * promotion.RewardAmount * 0.01m;
					Assert.IsTrue(discountAmount == item.LineItemDiscountAmount);
				}
			}
		}

		/// <summary>
		/// Target - OrderForm: IF SUM Quantity lineItems IN OrderForm haves specified Id GREATHET 20 THEN add 100$ discount for some lineItems
		/// </summary>
		/// <remarks>
		/// if orderForm have multiple LineItems with same code FixedDiscount must be distributed for all LineItems.
		/// </remarks>
		[TestMethod]
		public void MarketingSystem_OrderForm_Sum_FixedDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			var promotion =  new PromotionDef() { Name =  "WholeOrder_Fixed_Promo_For_SKU", RewardAmount = 100.12m };
			OrderForm orderForm = CreateRandomOrderForm();
			LineItem lineItem1 = OrderHelper.CreateLineItem();
			lineItem1.Quantity = 21;
			//Add new line item with same code and other quantity
			LineItem lineitem2 = CloneLineItem(lineItem1);
			lineitem2.Quantity = 10;
			orderForm.LineItems.Add(lineItem1);
			orderForm.LineItems.Add(lineitem2);
			Dictionary<string, string> skuList = new Dictionary<string, string>();
			skuList.Add(lineItem1.CatalogEntryId, lineItem1.CatalogEntryId);

			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						  GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											  conditionProvider, actionProvider);
			var condition = CreateElementDef(ElementDefs.OrderForm_LineItems, 
											 CreateElementDef(ElementDefs.Condition_Decimal_Greater), 
											 CreateElementDef(ElementDefs.Method_Sum));
			//Sum(Quantity) > 20
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			//Add (aggregated field name) in method Sum
			var sumParam = CreateElementDef(ElementDefs.LineItem_Quantity, 
											/*dummy condition must be decimal type*/ CreateElementDef(ElementDefs.Condition_Decimal_Equals));
			FilterExpressionNode sumParam_node = promoBuilder.CreateConditionExpressionWithoutRegistration(sumParam);
			condition_node.Method.Params.Add(sumParam_node);
			//Set condition value
			condition_node.Value = 20m;

			//Add sub condition block for collections
			//LineItem.CatalogEntryId == specified
			var subCondition = CreateElementDef(ElementDefs.LineItem_CatalogEntryId, 
													CreateElementDef(ElementDefs.Condition_Text_Equals));
			FilterExpressionNode subCondition_node = promoBuilder.CreateConditionExpression(condition_node,
																							subCondition);
			//Set condition value
			subCondition_node.Value = lineItem1.CatalogEntryId;

			//Construct action
			//Add whole 100$ discount reward for specified lineItem 
			var action = CreateElementDef(ElementDefs.Action_AllEntryValueOfSkuSet,
										  CreateElementDef(ElementDefs.Action_Condition_SkuSelect), 
										  CreateElementDef(ElementDefs.Method_Action_AllEntry));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Method.Params.Add(promotion.RewardAmount);
			action_node.Value = skuList;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");
			decimal averageAmount = promotion.RewardAmount / (lineItem1.Quantity + lineitem2.Quantity);
			Assert.IsTrue(lineItem1.LineItemDiscountAmount == averageAmount * lineItem1.Quantity);
			Assert.IsTrue(lineitem2.LineItemDiscountAmount == averageAmount * lineitem2.Quantity);
		}

		/// <summary>
		/// Target - OrderForm: IF ALL lineItems IN OrderForm have Quantity greather that 3 AND BillingAddress.Email = store@mediachase.com
		/// THEN add wholeOrder discount 87%
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderForm_All_WholeOrderPercentDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			var promotion = new PromotionDef() { Name = "WholeOrder percent promotion", RewardAmount = 87.321m };
			OrderForm orderForm = CreateRandomOrderForm();
			foreach (LineItem lineItem in orderForm.LineItems)
			{
				if (lineItem.Quantity <= 3m)
				{
					lineItem.Quantity += 3m;
				}
			}
			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						   GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											   conditionProvider, actionProvider);

			
			//Construct condition
			//OrderForm.LineItems All Equals true
			var condition = CreateElementDef(ElementDefs.OrderForm_LineItems,
											CreateElementDef(ElementDefs.Condition_Bool_Equals),
											CreateElementDef(ElementDefs.Method_All));
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = true;
			//LineItem.Quantity > 3
			var subCondition = CreateElementDef(ElementDefs.LineItem_Quantity, 
												CreateElementDef(ElementDefs.Condition_Decimal_Greater));
			FilterExpressionNode subCondition_node =
									   promoBuilder.CreateConditionExpression(condition_node, subCondition);
			subCondition_node.Value = 3m;

			//Construct condition2
			//OrderForm.BillingAddress All Equals True
			var condition2 = CreateElementDef(ElementDefs.OrderForm_BillingAddress,
											CreateElementDef(ElementDefs.Condition_Bool_Equals),
											CreateElementDef(ElementDefs.Method_Any));
			FilterExpressionNode condition_node2 = promoBuilder.CreateConditionExpression(condition2);
			condition_node2.Value = true;
			//email contains "store@mediachase.com"
			var subCondition2 = CreateElementDef(ElementDefs.OrderAddress_Email,
												CreateElementDef(ElementDefs.Condition_Text_Contains));
			FilterExpressionNode subCondition_node2 =
									   promoBuilder.CreateConditionExpression(condition_node2, subCondition2);
			subCondition_node2.Value = @"store";

			//Construct action
			//Add 87% discount reward whole order
			var action = CreateElementDef(ElementDefs.Action_WholeOrderPercent, 
										  CreateElementDef(ElementDefs.Action_Condition_PercentDecimal));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Value = promotion.RewardAmount;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");
			decimal discountPercentVal = Decimal.Round(orderForm.DiscountAmount / orderForm.SubTotal * 100, 20);
			Assert.IsTrue(discountPercentVal == promotion.RewardAmount);
			
		}

		/// <summary>
		/// Target - OrderForm: IF EXIST lineItem IN OrderForm WITH specified CatalogEntryId THEN add whole order discount 10$
		/// </summary>
		[TestMethod]
		public void MarketingSystem_OrderForm_Any_WholeOrderDiscount()
		{
			//Providers
			FilterExpressionProvider actionProvider = new PromotionActionProvider();
			FilterExpressionProvider conditionProvider = new PromotionFilterExpressionProvider();
			//Promotion name
			var promotion = new PromotionDef() { Name = "Whole order promotion", RewardAmount = 10.32m };
			OrderForm orderForm = CreateRandomOrderForm();

			//Add line item to cart
			LineItem lineItem = OrderHelper.CreateLineItem();
			lineItem.ListPrice = 100m;
			orderForm.LineItems.Add(lineItem);

			//Construct condition for promotion
			PromotionBuilder promoBuilder =
						   GetPromotionBuilder(promotion.Name, PromotionGroup.PromotionGroupKey.Order,
											   conditionProvider, actionProvider);

			var condition = CreateElementDef(ElementDefs.OrderForm_LineItems, 
											 CreateElementDef(ElementDefs.Condition_Bool_Equals), 
											 CreateElementDef(ElementDefs.Method_Any));
			//OrderForm.LineItems Any Equals true
			FilterExpressionNode condition_node = promoBuilder.CreateConditionExpression(condition);
			condition_node.Value = true;
			//LineItem.CatalogEntryId Equals {Entry Id (add in this method)}
			var subCondition = CreateElementDef(ElementDefs.LineItem_CatalogEntryId, 
												CreateElementDef(ElementDefs.Condition_Text_Equals));
			FilterExpressionNode subCondition_node = promoBuilder.CreateConditionExpression(condition_node, subCondition);
			subCondition_node.Value = lineItem.CatalogEntryId;

			//Construct action for apply reward 
			//Add 10$ discount reward whole order
			var action = CreateElementDef(ElementDefs.Action_WholeOrderValue, 
										  CreateElementDef(ElementDefs.Action_Condition_Decimal));
			FilterExpressionNode action_node = promoBuilder.CreateActionExpression(action);
			action_node.Value = promotion.RewardAmount;

			//Save promotion to database
			promoBuilder.SavePromotion();

			//Run worflow cart
			orderForm.Parent.RunWorkflow("CartValidate");

			//Check result
			Assert.IsNotNull(GetPromotionInOrderByName(orderForm, promotion.Name), "Discount " + promotion.Name + " not assigned");
			Assert.IsTrue(Decimal.Round(orderForm.DiscountAmount, 20) == promotion.RewardAmount);
		}
		#endregion

		private static LineItemDiscount GetPromotionInEntryByName(LineItem lineItem, string promoName)
		{
			LineItemDiscount retVal = null;
			foreach (LineItemDiscount discount in lineItem.Discounts)
			{
				if (discount.DiscountName == promoName)
				{
					retVal = discount;
					break;
				}
			}

			return retVal;
		}

		private static OrderFormDiscount GetPromotionInOrderByName(OrderForm orderForm, string promoName)
		{
			OrderFormDiscount retVal = null;
			foreach (OrderFormDiscount discount in orderForm.Discounts)
			{
				if (discount.DiscountName == promoName)
				{
					retVal = discount;
					break;
				}
			}

			return retVal;
		}
		/// <summary>
		/// Clones line item. Because LineItem.Clone() not WORKS!!!
		/// </summary>
		/// <returns></returns>
		private static LineItem CloneLineItem(LineItem otherLineItem)
		{
			LineItem lineItem = new LineItem();

			lineItem.DisplayName = otherLineItem.DisplayName;
			lineItem.CatalogEntryId = otherLineItem.CatalogEntryId;
			lineItem.ShippingMethodId = otherLineItem.ShippingMethodId;
			lineItem.ShippingMethodName = otherLineItem.ShippingMethodName;
			lineItem.ShippingAddressId = otherLineItem.ShippingAddressId;
			lineItem.ListPrice = otherLineItem.ListPrice;
			lineItem.Quantity = otherLineItem.Quantity;
			lineItem.CatalogNode = otherLineItem.CatalogNode;

			return lineItem;
		}
		private static OrderForm CreateRandomOrderForm()
		{
			//Create random cart
			Cart cart = OrderHelper.CreateCart(Guid.NewGuid());
			cart.OrderForms.RemoveAt(1);
			cart.OrderForms.RemoveAt(1);
			OrderForm retVal = cart.OrderForms[0];
			return retVal;

		}

		private static ElementDefs CreateElementDef(string key)
		{
			return CreateElementDef(key, null, null);
		}
		private static ElementDefs CreateElementDef(string key, ElementDefs conditions)
		{
			return CreateElementDef(key, conditions, null);
		}
		private static ElementDefs CreateElementDef(string key, ElementDefs conditions, ElementDefs methods)
		{

			return new ElementDefs()
			{
				Key = key,
				Conditions = new ElementDefs[] { conditions },
				Methods = new ElementDefs[] { methods }
			};

		}


	
		/// <summary>
		/// Construct promotion from specified conditions and actions
		/// </summary>
		private class PromotionBuilder
		{
			public string Name { get; set; }
			private PromotionDto InnerPromotionDto { get; set; }
			private PromotionDto.PromotionRow PromotionRow { get; set; }
			private PromotionGroup.PromotionGroupKey PromoGroup { get; set; }
			private FilterExpressionNode ConditionsExpressionRoot { get; set; }
			private FilterExpressionNodeCollection ActionsExpression { get; set; }
			public FilterExpressionProvider ConditionProvider { get; set; }
			public FilterExpressionProvider ActionProvider { get; set; }

			private PromotionBuilder(string name, PromotionGroup.PromotionGroupKey promoGroup,
									 FilterExpressionProvider conditionProvider,
									 FilterExpressionProvider actionProvider)
			{
				Name = name;
				PromoGroup = promoGroup;

				ConditionsExpressionRoot = new FilterExpressionNode();
				ConditionsExpressionRoot.NodeType = FilterExpressionNodeType.AndBlock;
				ConditionsExpressionRoot.ChildNodes = new FilterExpressionNodeCollection();

				ActionsExpression = new FilterExpressionNodeCollection();
				ConditionProvider = conditionProvider;
				ActionProvider = actionProvider;

			}
			public static PromotionBuilder CreateInstance(string name, int priority, string couponCode,
														  string status, int campaingId,
														  PromotionGroup.PromotionGroupKey promotionGroup,
														  FilterExpressionProvider conditionProvider,
														  FilterExpressionProvider ationProvider)
			{
				PromotionBuilder retVal = new PromotionBuilder(name, promotionGroup, conditionProvider, ationProvider);
				retVal.InnerPromotionDto = PromotionManager.GetPromotionDto();
				PromotionDto.PromotionRow row = retVal.InnerPromotionDto.Promotion.NewPromotionRow();
				retVal.PromotionRow = row;
				row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
				row.Created = DateTime.UtcNow;
				row.ModifiedBy = "test owner";
				row.Name = name;
				row.StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10));
				row.EndDate = DateTime.UtcNow.AddDays(10);
				row.Priority = priority;
				row.CouponCode = couponCode;
				row.Status = status;
				row.ExclusivityType = "";
				row.CampaignId = campaingId;
				row.ApplicationLimit = 10;
				row.PerOrderLimit = 10;
				if (row.RowState == DataRowState.Detached)
				{
					row.PromotionGroup = Mediachase.Commerce.Marketing.PromotionGroup.GetPromotionGroup(promotionGroup).Key;
					row.OfferType = 0;
					row.OfferAmount = 0;
					retVal.InnerPromotionDto.Promotion.Rows.Add(row);
					
				}

				return retVal;
			}

			#region Actions methods

			private void AddAction(FilterExpressionNode action)
			{
				this.ActionsExpression.Add(action);
			}

			#endregion

			#region Conditions methods
			private void AddCondition(FilterExpressionNode condition)
			{
				ConditionsExpressionRoot.ChildNodes.Add(condition);
			}
			#endregion

			/// <summary>
			/// Creates the condition expression without registration.
			/// </summary>
			/// <param name="elementDefs">The element defs.</param>
			/// <returns></returns>
			public FilterExpressionNode CreateConditionExpressionWithoutRegistration(ElementDefs elementDefs)
			{
				return CreateConditionExpression(null, elementDefs);
			}

			/// <summary>
			/// Creates the expression.
			/// </summary>
			/// <param name="elementDefs">The element defs.</param>
			/// <returns></returns>
			public FilterExpressionNode CreateConditionExpression(ElementDefs elementDefs)
			{
				FilterExpressionNode retVal = CreateConditionExpressionWithoutRegistration(elementDefs);
				this.AddCondition(retVal);

				return retVal;
			}

			/// <summary>
			/// Creates the expression from ElementsDef.
			/// </summary>
			/// <param name="parent">The parent.</param>
			/// <param name="elementDefs">The element defs.</param>
			/// <returns></returns>
			public FilterExpressionNode CreateConditionExpression(FilterExpressionNode parent, ElementDefs elementDefs)
			{
				//2. Construct FilterExpression collections
				FilterExpressionNode retVal =
							ElementDef2FilterExprNode(this.ConditionProvider as BaseRuleSetFilterExpressionProvider,
													  elementDefs);
				if (parent != null)
				{
					if (!parent.HasChildNodes)
					{
						parent.NodeType = FilterExpressionNodeType.MethodBlock;
						parent.ChildNodes = new FilterExpressionNodeCollection();
					}

					parent.ChildNodes.Add(retVal);
				}
				return retVal;
			}

			public FilterExpressionNode CreateActionExpression(ElementDefs elementDefs)
			{
				//2. Construct FilterExpression collections
				FilterExpressionNode retVal =
							ElementDef2FilterExprNode(this.ActionProvider as BaseRuleSetFilterExpressionProvider,
													  elementDefs);

				this.AddAction(retVal);

				return retVal;
			}

			public void DeletePromotion()
			{
				using (Mediachase.Data.Provider.TransactionScope scope = new Mediachase.Data.Provider.TransactionScope())
				{
					List<int> expressionList = new List<int>();
					foreach (PromotionDto.PromotionConditionRow condition in this.PromotionRow.GetPromotionConditionRows())
					{
						expressionList.Add(condition.ExpressionId);
					}
					this.PromotionRow.Delete();
					PromotionManager.SavePromotion(InnerPromotionDto);
					// Delete corresponding expressions
					foreach (int expressionId in expressionList)
					{
						ExpressionDto expressionDto = ExpressionManager.GetExpressionDto(expressionId);
						if (expressionDto != null && expressionDto.Expression.Count > 0)
						{
							expressionDto.Expression[0].Delete();
							ExpressionManager.SaveExpression(expressionDto);
						}
					}

					scope.Complete();
				}
			}
			public void SavePromotion()
			{
				using (Mediachase.Data.Provider.TransactionScope scope = new Mediachase.Data.Provider.TransactionScope())
				{
					//Expression
					ExpressionDto expressionDto = CreateExpressionDto();
					int expressionId = expressionDto.Expression.First().ExpressionId;

					ConditionProvider.DataSource = expressionDto;
					FilterExpressionNodeCollection conditionColl = new FilterExpressionNodeCollection();
					conditionColl.Add(ConditionsExpressionRoot);
					ConditionProvider.SaveFilters(GetExpressionPlace(this.PromoGroup, string.Empty),
												  expressionId.ToString(), conditionColl);
					ExpressionManager.SaveExpression(expressionDto);

					ActionProvider.DataSource = expressionDto;
					ActionProvider.SaveFilters(GetExpressionPlace(this.PromoGroup, string.Empty),
											   expressionId.ToString(), this.ActionsExpression);
					ExpressionManager.SaveExpression(this.ActionProvider.DataSource as ExpressionDto);

					//PromotionCondition
					PromotionDto.PromotionConditionRow promotionContionRow = 
												this.InnerPromotionDto.PromotionCondition.NewPromotionConditionRow();
					promotionContionRow.PromotionId = PromotionRow.PromotionId;
					promotionContionRow.ExpressionId = expressionId;
					InnerPromotionDto.PromotionCondition.Rows.Add(promotionContionRow);

					//Promotion
					PromotionManager.SavePromotion(InnerPromotionDto);

					scope.Complete();
				}
			}

			#region Private helper methods
			private static string GetExpressionPlace(PromotionGroup.PromotionGroupKey promoGroup, string realPlace)
			{
				return PromotionGroup.GetPromotionGroup(promoGroup).Key + ":" + realPlace;
			}
			
			private static ExpressionDto CreateExpressionDto()
			{
				ExpressionDto expressionDto = new ExpressionDto();
				ExpressionDto.ExpressionRow	expressionRow = expressionDto.Expression.NewExpressionRow();
				expressionRow.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
				expressionRow.Category = ExpressionCategory.CategoryKey.Promotion.ToString();
				expressionRow.Created = DateTime.UtcNow;
				expressionRow.ExpressionXml = string.Empty;
				expressionRow.Category = ExpressionCategory.CategoryKey.Promotion.ToString();
				expressionRow.ModifiedBy = "test";
				expressionRow.Description = "config test";
				expressionRow.Name = "test type";

				expressionDto.Expression.Rows.Add(expressionRow);

				ExpressionManager.SaveExpression(expressionDto);

				return expressionDto;
			}

			private static FilterExpressionNode ElementDef2FilterExprNode(BaseRuleSetFilterExpressionProvider provider,
																		   ElementDefs elementDef)
			{
				FilterExpressionNode retVal = null;
				//element
				ElementDefs providerElementDef = provider.FindElementDef(elementDef.Key);
				retVal = (providerElementDef.Tag as FilterExpressionNode).Clone();
				//Clear child nodes, child node we not interest
				if (retVal.HasChildNodes)
				{
					retVal.ChildNodes.Clear();
				}

				//method
				ElementDefs method = null;
				if (providerElementDef.Methods != null)
				{
					method = providerElementDef.Methods.First(x => x.Key == elementDef.Methods.First().Key);
					retVal.Method = method.Tag as MethodElement;
				}

				//condition
				var conditions = providerElementDef.Conditions;
				//Find condition in element definitions
				if (conditions == null)
				{
					if (method == null)
					{
						throw new NullReferenceException("condition must be specified (not found)");
					}
					conditions = method.Conditions;
				}

				ElementDefs condition = conditions.First(x => x.Key == elementDef.Conditions.First().Key);
				retVal.Condition = condition.Tag as ConditionElement;
				return retVal;

			}

			#endregion

		}
	}
}
