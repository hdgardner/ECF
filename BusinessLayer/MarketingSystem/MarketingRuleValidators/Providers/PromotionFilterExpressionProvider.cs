using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Orders;
using System.Linq;
using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
	/// <summary>
	/// Represents Promotion filter expression provider.
	/// This provider will be used to construct windows workflow foundation rules using the Expression Builder User Interface DHTML Control.
	/// </summary>
	public class PromotionFilterExpressionProvider : BaseRuleSetFilterExpressionProvider
	{
		#region Const
		private  Dictionary<string, IEnumerable<FilterExpressionNode>> _filterElementsMap =
														new Dictionary<string, IEnumerable<FilterExpressionNode>>();
		private const string ContextClass = "this";
		#endregion

		#region Properties
		#region FilterElement definitions
		protected  IEnumerable<ElementDefs> ShoppingCartDefs { get; private set; }
		protected IEnumerable<ElementDefs> OrderFormDefs { get; private set; }
		protected IEnumerable<ElementDefs> ShipmentDefs { get; private set; }
		/// <summary>
		/// Definitions maping fields MC OrderAddress -> indexed property PromotionEntry
		/// </summary>
		protected IEnumerable<ElementDefs> OrderAddressDefs { get; private set; }
		/// <summary>
		/// Definitions maping fields MC LineItem -> PromotionEntry
		/// </summary>
		protected IEnumerable<ElementDefs> LineItemDefs { get; private set; }
		protected IEnumerable<ElementDefs> LineItemWithAddressDefs { get; private set; }
		
		/// <summary>
		/// Definitions maping fields TargetLineItem -> PromotionEntry
		/// </summary>
		protected IEnumerable<ElementDefs> TargetLineItemDefs { get; private set; }

		protected IEnumerable<ElementDefs> RuningTotalDefs { get; private set; }
		#endregion
		#region FilterElement Method Definition
		protected ElementDefs MethodSumDef { get; private set; }
		protected ElementDefs MethodCountDef { get; private set; }
		protected ElementDefs MethodAllDef { get; private set; }
		protected ElementDefs MethodAnyDef { get; private set; }
		protected IEnumerable<ElementDefs> CollectionMethodDefs { get; private set; }
		#endregion
	
		#endregion

		#region Overrides  BaseRuleSetFilterExpressionProvider methods
		protected override void RegisterProviderElementDefinitions()
		{
			//First call base implementation
			base.RegisterProviderElementDefinitions();

		
			#region Method definitions

			MethodSumDef = new ElementDefs() { Key = ElementDefs.Method_Sum, Name = "GetCollectionSum", Descr = "Sum", Conditions = ConditionDecimalDefs };
			MethodCountDef = new ElementDefs() { Key = ElementDefs.Method_Count, Name = "GetValidatedCount", Descr = "Count", Conditions = ConditionIntegerDefs };
			MethodAnyDef = new ElementDefs() { Key = ElementDefs.Method_Any, Name = "ValidateAny", Descr = "Any", Conditions = ConditionBoolDefs };
			MethodAllDef = new ElementDefs() { Key = ElementDefs.Method_All, Name = "ValidateAll", Descr = "All", Conditions = ConditionBoolDefs };
			CollectionMethodDefs = new ElementDefs[] { MethodSumDef, MethodCountDef, MethodAllDef, MethodAnyDef };
			RegisterDefinitions(CollectionMethodDefs);

			#endregion


			#region element definitions
			ShipmentDefs =
					new ElementDefs[] { 
									//new ElementDefs() { Key = ElementDefs.Shipment_LienItems, Name = "this.PreparePromotionEntries(this.PromotionContext.SourceEntriesSet.Entries)", Descr="Shipment.LineItems", Methods = CollectionMethodDefs, Childrens =  LineItemDefs},

									new ElementDefs() { Key = ElementDefs.Shipment_ShippingMethodName, Name = "this.PromotionCurrentShipment.ShippingMethodName", Descr="Shipment.ShippingMethodName", Conditions = ConditionTextDefs },
									//TODO: Add other prop
			};
			RegisterDefinitions(ShipmentDefs);

			OrderAddressDefs =
			new ElementDefs[]{
										new ElementDefs() { Key = ElementDefs.OrderAddress_Name, Name = "this['Name']", Descr="Name", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_FirstName, Name = "this['FirstName']", Descr="FirstName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_LastName, Name = "this['LastName']", Descr="LastName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_Organization, Name = "this['Organization']", Descr="Organization", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_Line1, Name = "this['Line1']", Descr="Line1", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_Line2, Name = "this['Line2']", Descr="Line2", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_City, Name = "this['City']", Descr="City", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_State, Name = "this['State']", Descr="State", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_CountryCode, Name = "this['CountryCode']", Descr="CountryCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_CountryName, Name = "this['CountryName']", Descr="CountryName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_PostalCode, Name = "this['PostalCode']", Descr="PostalCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_RegionCode, Name = "this['RegionCode']", Descr="RegionCode", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_RegionName, Name = "this['RegionName']", Descr="RegionName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_DaytimePhoneNumber, Name = "this['DaytimePhoneNumber']", Descr="DaytimePhoneNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_EveningPhoneNumber, Name = "this['EveningPhoneNumber']", Descr="EveningPhoneNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_FaxNumber, Name = "this['FaxNumber']", Descr="FaxNumber", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.OrderAddress_Email, Name = "this['Email']", Descr="Email", Conditions = ConditionTextDefs },
			}.Concat(base.GetElementDefsByMetaClass("this['{0}']", OrderContext.Current.OrderAddressMetaClass)).Select(x=> { x.Name = x.Name.Replace("\"", "'"); return x; } );

			RegisterDefinitions(OrderAddressDefs);

			
			LineItemDefs =
					new ElementDefs[] { 
										new ElementDefs() { Key = ElementDefs.LineItem_Address, Name = "this['Address']", Descr="Address", Methods = new ElementDefs[] { MethodAnyDef }, Childrens = OrderAddressDefs},

										new ElementDefs() { Key = ElementDefs.LineItem_Quantity, Name = "this.Quantity", Descr= "Quantity", Conditions = ConditionDecimalDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_ListPrice, Name = "this.CostPerEntry", Descr= "ListPrice", Conditions = ConditionDecimalDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_Catalog, Name = "this.CatalogName", Descr= "Catalog", Conditions = ConditionTextDefs  },
										new ElementDefs() { Key = ElementDefs.LineItem_CatalogNode, Name = "this.CatalogNodeCode", Descr= "CatalogNode", Conditions = ConditionTextDefs  },
										new ElementDefs() { Key = ElementDefs.LineItem_CatalogEntryId, Name = "this.CatalogEntryCode", Descr= "CatalogEntryId", Conditions = ConditionTextDefs  },

										new ElementDefs() { Key = ElementDefs.LineItem_MinQuantity, Name = "this['MinQuantity']", Descr="MinQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_MaxQuantity, Name = "this['MaxQuantity']", Descr="MaxQuantity", Conditions = ConditionDecimalDefs},
									    new ElementDefs() { Key = ElementDefs.LineItem_PlacedPrice, Name = "this['PlacedPrice']", Descr="PlacedPrice", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_LineItemDiscountAmount, Name = "this['LineItemDiscountAmount']", Descr="LineItemDiscountAmount", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_OrderLevelDiscountAmount, Name = "this['OrderLevelDiscountAmount']", Descr="OrderLevelDiscountAmount", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_ShippingMethodName, Name = "this['ShippingMethodName']", Descr="ShippingMethodName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_ExtendedPrice, Name = "this['ExtendedPrice']", Descr="ExtendedPrice", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_Description, Name = "this['Description']", Descr="Description", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_Status, Name = "this['Status']", Descr="Status", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_DisplayName, Name = "this['DisplayName']", Descr="DisplayName",Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.LineItem_AllowBackordersAndPreorders, Name = "this['AllowBackordersAndPreorders']", Descr="AllowBackordersAndPreorders", Conditions = ConditionBoolDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_InStockQuantity, Name = "this['InStockQuantity']", Descr="InStockQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_PreorderQuantity, Name = "this['PreorderQuantity']", Descr="PreorderQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_BackorderQuantity, Name = "this['BackorderQuantity']", Descr="BackorderQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.LineItem_InventoryStatus, Name = "this['InventoryStatus']", Descr="InventoryStatus", Conditions = ConditionIntegerDefs}
									}.Concat(base.GetElementDefsByMetaClass("this['{0}']", OrderContext.Current.LineItemMetaClass)).Select(x=> { x.Name = x.Name.Replace("\"", "'"); return x; } );
			RegisterDefinitions(LineItemDefs);

			//Create lineItems with address elements
			LineItemWithAddressDefs = LineItemDefs.Concat(OrderAddressDefs);

			ShoppingCartDefs =
				new ElementDefs[] { 
								new ElementDefs() { Key = ElementDefs.ShoppingCart_BillingCurrency, Name = "this.ShoppingCart.BillingCurrency", Descr="ShoppingCart.BillingCurrency", Conditions = ConditionDecimalDefs},
								new ElementDefs() { Key = ElementDefs.ShoppingCart_CustomerName, Name = "this.ShoppingCart.CustomerName", Descr="ShoppingCart.CustomerName", Conditions = ConditionTextDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_HandlingTotal, Name = "this.ShoppingCart.HandlingTotal", Descr="ShoppingCart.HandlingTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_Name, Name = "this.ShoppingCart.Name", Descr="ShoppingCart.Name", Conditions = ConditionTextDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_ShippingTotal, Name = "this.ShoppingCart.ShippingTotal", Descr="ShoppingCart.ShippingTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_Status, Name = "this.ShoppingCart.Status", Descr="ShoppingCart.Status", Conditions = ConditionTextDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_SubTotal, Name = "this.ShoppingCart.SubTotal", Descr="ShoppingCart.SubTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_TaxTotal, Name = "this.ShoppingCart.TaxTotal", Descr="ShoppingCart.TaxTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.ShoppingCart_Total, Name = "this.ShoppingCart.Total", Descr="ShoppingCart.Total", Conditions = ConditionDecimalDefs }
			}.Concat(base.GetElementDefsByMetaClass("this.ShoppingCart[\"{0}\"]", OrderContext.Current.ShoppingCartMetaClass));
			RegisterDefinitions(ShoppingCartDefs);

			OrderFormDefs =
			new ElementDefs[] { 
								new ElementDefs() { Key = ElementDefs.OrderForm_LineItems, Name = "this.PromotionContext.SourceEntriesSet.Entries", Descr = "OrderForm.LineItems", Methods = CollectionMethodDefs, Childrens =  LineItemDefs},
								//AHTUNG: Need add property to OrderForm.BillingAddress represents coolections of addresses
								new ElementDefs() { Key = ElementDefs.OrderForm_BillingAddress, Name = "this.PromotionOrderFormBillingAddress", Descr = "OrderForm.BillingAddress", Methods = new ElementDefs[] { MethodAnyDef }, Childrens = OrderAddressDefs},
								new ElementDefs() { Key = ElementDefs.OrderForm_Shipments, Name = "this.PromotionCurrentOrderForm.Shipments", Descr="OrderForm.Shipments", Methods = CollectionMethodDefs, Childrens = ShipmentDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_DiscountAmount, Name = "this.PromotionCurrentOrderForm.DiscountAmount", Descr="OrderForm.DiscountAmount", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_Name, Name = "this.PromotionCurrentOrderForm.Name", Descr="OrderForm.Name", Conditions = ConditionTextDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_HandlingTotal, Name = "this.PromotionCurrentOrderForm.HandlingTotal", Descr="OrderForm.HandlingTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_ShippingTotal, Name = "this.PromotionCurrentOrderForm.ShippingTotal", Descr="OrderForm.ShippingTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_Status, Name = "this.PromotionCurrentOrderForm.Status", Descr="OrderForm.Status", Conditions = ConditionTextDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_SubTotal, Name = "this.PromotionCurrentOrderForm.SubTotal", Descr="OrderForm.SubTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_TaxTotal, Name = "this.PromotionCurrentOrderForm.TaxTotal", Descr="OrderForm.TaxTotal", Conditions = ConditionDecimalDefs },
								new ElementDefs() { Key = ElementDefs.OrderForm_Total, Name = "this.PromotionCurrentOrderForm.Total", Descr="OrderForm.Total", Conditions = ConditionDecimalDefs }
			}.Concat(base.GetElementDefsByMetaClass("this.PromotionCurrentOrderForm[\"{0}\"]", OrderContext.Current.OrderFormMetaClass));
			RegisterDefinitions(OrderFormDefs);

			//TargetLineItem
			TargetLineItemDefs =
			new ElementDefs[] { 
										new ElementDefs() { Key = ElementDefs.TargetLineItem_ListPrice, Name = "this.PromotionTargetLineItem.CostPerEntry", Descr= "TargetLineItem.ListPrice", Conditions = ConditionDecimalDefs },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_Catalog, Name = "this.PromotionTargetLineItem.CatalogName", Descr= "TargetLineItem.Catalog", Conditions = ConditionTextDefs  },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_CatalogNode, Name = "this.PromotionTargetLineItem.CatalogNodeCode", Descr= "TargetLineItem.CatalogNode", Conditions = ConditionTextDefs  },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_CatalogEntryId, Name = "this.PromotionTargetLineItem.CatalogEntryCode", Descr= "TargetLineItem.CatalogEntryId", Conditions = ConditionTextDefs  },

										new ElementDefs() { Key = ElementDefs.TargetLineItem_MinQuantity, Name = "this.PromotionTargetLineItem[\"MinQuantity\"]", Descr="TargetLineItem.MinQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_MaxQuantity, Name = "this.PromotionTargetLineItem[\"MaxQuantity\"]", Descr="TargetLineItem.MaxQuantity", Conditions = ConditionDecimalDefs},
									    new ElementDefs() { Key = ElementDefs.TargetLineItem_PlacedPrice, Name = "this.PromotionTargetLineItem[\"PlacedPrice\"]", Descr="TargetLineItem.PlacedPrice", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_LineItemDiscountAmount, Name = "this.PromotionTargetLineItem[\"LineItemDiscountAmount\"]", Descr="TargetLineItem.LineItemDiscountAmount", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_OrderLevelDiscountAmount, Name = "this.PromotionTargetLineItem[\"OrderLevelDiscountAmount\"]", Descr="TargetLineItem.OrderLevelDiscountAmount", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_ShippingMethodName, Name = "this.PromotionTargetLineItem[\"ShippingMethodName\"]", Descr="TargetLineItem.ShippingMethodName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_ExtendedPrice, Name = "this.PromotionTargetLineItem[\"ExtendedPrice\"]", Descr="TargetLineItem.ExtendedPrice", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_Description, Name = "this.PromotionTargetLineItem[\"Description\"]", Descr="TargetLineItem.Description", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_Status, Name = "this.PromotionTargetLineItem[\"Status\"]", Descr="TargetLineItem.Status",Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_DisplayName, Name = "this.PromotionTargetLineItem[\"DisplayName\"]", Descr="TargetLineItem.DisplayName", Conditions = ConditionTextDefs },
										new ElementDefs() { Key = ElementDefs.TargetLineItem_AllowBackordersAndPreorders, Name = "this.PromotionTargetLineItem[\"AllowBackordersAndPreorders\"]", Descr="TargetLineItem.AllowBackordersAndPreorders", Conditions = ConditionBoolDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_InStockQuantity, Name = "this.PromotionTargetLineItem[\"InStockQuantity\"]", Descr="TargetLineItem.InStockQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_PreorderQuantity, Name = "this.PromotionTargetLineItem[\"PreorderQuantity\"]", Descr="TargetLineItem.PreorderQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_BackorderQuantity, Name = "this.PromotionTargetLineItem[\"BackorderQuantity\"]", Descr="TargetLineItem.BackorderQuantity", Conditions = ConditionDecimalDefs},
										new ElementDefs() { Key = ElementDefs.TargetLineItem_InventoryStatus, Name = "this.PromotionTargetLineItem[\"InventoryStatus\"]", Descr="TargetLineItem.InventoryStatus", Conditions = ConditionIntegerDefs}
									}.Concat(base.GetElementDefsByMetaClass("this.PromotionTargetLineItem[\"{0}\"]", OrderContext.Current.LineItemMetaClass));
			RegisterDefinitions(TargetLineItemDefs);

			//RunningTotal
			RuningTotalDefs =
				new ElementDefs[] {
										new ElementDefs() { Key = ElementDefs.RunningTotal, Name = "this.PromotionResult.RunningTotal", Descr = "RunningTotal", Conditions = ConditionDecimalDefs }
				};
			RegisterDefinitions(RuningTotalDefs);

			#endregion
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		protected override void CreateFilterNodes()
		{
			//First call base implementation
			base.CreateFilterNodes();

			//populate cache elements for Order promotion target
			List<FilterExpressionNode> tmpColl = new List<FilterExpressionNode>();
			#region Entry
			string promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;
			//Target Line Item 
			tmpColl.AddRange(TargetLineItemDefs.Select(x => CreateFilterExpressionNode(x)));
			_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion

			#region OrderGroup
			promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
			tmpColl = new List<FilterExpressionNode>();
			//[Card/OrderGroup]
			tmpColl.AddRange(ShoppingCartDefs.Select(x => CreateFilterExpressionNode(x)));
			//[OrderForm]
			tmpColl.AddRange(OrderFormDefs.Select(x => CreateFilterExpressionNode(x)));
			//Running Total
			tmpColl.AddRange(RuningTotalDefs.Select(x => CreateFilterExpressionNode(x)));

			_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion

			#region Shipping
			//populate cache for Shipping promotion target
			tmpColl = new List<FilterExpressionNode>();
			promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key;
			//Running Total
			tmpColl.AddRange(RuningTotalDefs.Select(x => CreateFilterExpressionNode(x)));
			//[Card/OrderGroup] properties
			tmpColl.AddRange(ShoppingCartDefs.Select(x => CreateFilterExpressionNode(x)));
			//[Shipment]
			tmpColl.AddRange(ShipmentDefs.Select(x => CreateFilterExpressionNode(x)));

			_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion

			#region create method elements
			foreach (ElementDefs methodDef in CollectionMethodDefs)
			{
				CreateMethodElement(methodDef, "~/Apps/Marketing/ExpressionFunctions/Sum.ascx");
			}
			#endregion

			#region create condition elements
			//Define available conditions 
			foreach (ElementDefs conditionDef in ConditionTextDefs)
			{
				CreateConditionEement(conditionDef, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/TextFilter.ascx");
			}
			foreach (ElementDefs conditionDef in ConditionIntegerDefs)
			{
				CreateConditionEement(conditionDef, ConditionElementType.Integer, null);
			}
			foreach (ElementDefs conditionDef in ConditionDecimalDefs)
			{
				CreateConditionEement(conditionDef, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/DecimalFilter.ascx");
			}
			foreach (ElementDefs conditionDef in ConditionBoolDefs)
			{
				CreateConditionEement(conditionDef, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/BooleanFilter.ascx");
			}
			#endregion
		}

		/// <summary>
		/// Converts the node to string expression.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		protected override string ConvertNodeToStringExpression(FilterExpressionNode node)
		{
			string retVal = string.Empty;

			if (node.NodeType == FilterExpressionNodeType.OrBlock)
			{
				return ConvertNodesToStringExpression(node.ChildNodes, "||");
			}
			if(node.NodeType == FilterExpressionNodeType.AndBlock)
			{
				return ConvertNodesToStringExpression(node.ChildNodes, "&&");
			}
			if(node!= null && node.Key == FilterExpressionItem.chooseBlock)
			{
				return retVal;
			}

			ElementDefs elementDef = FindElementDef(node.Key);
			if (elementDef == null)
			{
				throw new ArgumentException("element  " + node.Key + " not registered");
			}

			//is Method Block
			if (elementDef.Methods != null)
			{
				//Get selected methods
				ElementDefs methodDef = elementDef.GetMethodDef(node.Method.Key);
				if (methodDef == null)
				{
					throw new ArgumentException("method " + node.Method.Key + " not registered");
				}
				//Get methods conditions 
				ElementDefs conditionDef = methodDef.GetConditionDefByName(node.Condition.Key);
				if (conditionDef == null)
				{
					throw new ArgumentException("condition  " + node.Condition.Key + " not registered");
				}
				string collectionName = elementDef.Name;
				string conditionOp = conditionDef.Name;
				MethodElementParams methodParams = node.Method.Params;

				string rightStatement = GetEscapedAndCleanedExpressionNodeRightValue(node);

				string predicateExpression = "1 == 1";
				string leftStatement = null;

				if (node.ChildNodes != null && node.ChildNodes.Count != 0)
				{
					predicateExpression = ConvertNodesToStringExpression(node.ChildNodes);
					//Parser bug. Nested double quotes
					predicateExpression = predicateExpression.Replace('\"', '\'');
				}
				
				if (methodDef == MethodSumDef)
				{
					FilterExpressionNode filterNode = methodParams[0] as FilterExpressionNode;
					ElementDefs methodElementDef = FindElementDef(filterNode.Key);
					if (methodElementDef == null)
					{
						throw new ArgumentException("element method" + filterNode.Key + " not registered");
					}
					leftStatement = methodElementDef.Name.Replace("this", string.Empty).Trim(new char[] { '[', ']', '\'', '.' });
					retVal = String.Format(@"({0}.{1}({2},""{3}"",""{4}"")) {5} {6}", ContextClass, methodDef.Name,
												collectionName, leftStatement, predicateExpression, conditionOp, rightStatement);
				}
				else if (methodDef == MethodCountDef)
				{
					retVal = String.Format(@"{0}.{1}({2},""{3}"")  {4} {5}", ContextClass, methodDef.Name,
												collectionName, predicateExpression, conditionOp, rightStatement);
				}
				else
				{
					retVal = String.Format(@"{0}.{1}({2},""{3}"")", ContextClass, methodDef.Name,
												collectionName, predicateExpression);
				}
			}
			else
			{
				//call base impl
				retVal = base.ConvertNodeToStringExpression(node);
			}

			return retVal;
		}

		#endregion

		#region Provider helper methods
		
		/// <summary>
		/// Gets the data source dto.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <returns></returns>
		private ExpressionDto GetDataSourceDto(object dataSource)
		{
			if (dataSource is System.Data.DataView)
			{
				return ((System.Data.DataView)dataSource).DataViewManager.DataSet as ExpressionDto;
			}
			else
			{
				return dataSource as ExpressionDto;
			}
		}

		/// <summary>
		/// Saves the expression.
		/// </summary>
		/// <param name="set">The set.</param>
		/// <param name="expressionId">The expression id.</param>
		protected virtual void SaveExpression(RuleSet set, int expressionId)
		{
			if (set == null)
				throw new ArgumentNullException("RuleSet");

			if (DataSource == null)
				throw new ArgumentNullException("DataSource");

			ExpressionDto dto = GetDataSourceDto(DataSource);
			ExpressionDto.ExpressionRow row = dto.Expression.FindByExpressionId(expressionId);

			if (row == null /*&& (expressionId == 0 || dto.Expression.Count == 0)*/) // new one
			{
				row = dto.Expression.NewExpressionRow();
				row.Name = set.Name;
				row.Description = String.Empty;
				row.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
				row.Category = ExpressionCategory.CategoryKey.Segment.ToString();
				row.Created = DateTime.UtcNow;
			}
			else
			{
				row.Modified = DateTime.UtcNow;
			}

			// Serialize ruleset
			StringBuilder ruleDefinition = new StringBuilder();

			#region Serialize

			StringWriter stringWriter = new StringWriter(ruleDefinition, CultureInfo.InvariantCulture);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
			serializer.Serialize(writer, set);
			#endregion

			#region Cleanup

			writer.Flush();
			writer.Close();
			stringWriter.Flush();
			stringWriter.Close();
			#endregion

			row.ExpressionXml = ruleDefinition.ToString();

			if (row.RowState == System.Data.DataRowState.Detached)
				dto.Expression.Rows.Add(row);
		}

		#endregion

		#region Overrides FilterExpressionProvider methods

		/// <summary>
		/// Loads the filters.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="expressionKey">The expression key.</param>
		/// <returns></returns>
		public override FilterExpressionNodeCollection LoadFilters(string expressionPlace, string expressionKey)
		{
			return (FilterExpressionNodeCollection)DataSource;
		}

		/// <summary>
		/// Saves the filters.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="expressionKey">The expression key, should be passed in the format of segmentid:expressionid</param>
		/// <param name="filters">The filters.</param>
		public override void SaveFilters(string expressionPlace, string expressionKey, FilterExpressionNodeCollection filters)
		{
			RuleSet set = CreateRuleSet(expressionPlace, expressionKey, filters);
			SaveExpression(set, Int32.Parse(expressionKey));
		}

		/// <summary>
		/// Gets the new elements.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public override FilterExpressionNodeCollection GetNewElements(string expressionPlace, FilterExpressionNode parent)
		{
			List<FilterExpressionNode> retVal = new List<FilterExpressionNode>();
			FilterExpressionNode parentMethodNode = null;
			FilterExpressionNode parentNext = parent;

			//set default group
			string currentPromotionGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
			//expressionPlace
			string realExpressionPlace = string.Empty;
			//Get current promotion target group from ExpressionPlace {promotionGroup : custom controls data }
			string[] expressionPlaceParts = expressionPlace.Split(':');
			if (expressionPlaceParts.Length != 0)
			{
				currentPromotionGroup = expressionPlaceParts[0];
			}
			if (expressionPlaceParts.Length > 1)
			{
				realExpressionPlace = expressionPlaceParts[1];
			}

			IEnumerable<FilterExpressionNode> resultColl;
			_filterElementsMap.TryGetValue(currentPromotionGroup, out resultColl);
			//detected presents parent block type MethodBlock	
			while (parentNext != null && parentMethodNode == null)
			{
				if (parentNext.NodeType == FilterExpressionNodeType.MethodBlock)
				{
					parentMethodNode = parentNext;
				}
				parentNext = parentNext.ParentNode;
			}

			if (parent == null || parentMethodNode == null)
			{
				if (resultColl != null)
				{
					//Add all registered element
					retVal.AddRange(resultColl);
				}
			}
			else
			{
				if (resultColl != null)
				{
					//Collections support
					FilterExpressionNode parentNode = FilterNodeFindRecursively(resultColl, x => x.Key == parentMethodNode.Key);
					IEnumerable<FilterExpressionNode> childEls = null;
					if (parentNode != null)
					{
						//gets all child element for current parrent node
						childEls = parentNode.ChildNodes;
					}
					//is call from custom cotrol for get elements for binding
					if (realExpressionPlace == "Sum")
					{
						ElementDefs parentElementDef = FindElementDef(parent.Key);
						ElementDefs methodDef = parentElementDef.GetMethodDef(parent.Method.Key);
						//For method SUM return only Decimal type elemnts 
						if (methodDef == MethodSumDef)
						{
							var result = new List<FilterExpressionNode>();
							foreach (FilterExpressionNode node in childEls)
							{
								ElementDefs nodeDef = FindElementDef(node.Key);
								if (nodeDef != null && nodeDef.Conditions == ConditionDecimalDefs)
								{
									result.Add(node);
								}
							}
							childEls = result;
						}
						else
						{
							childEls = new FilterExpressionNode[] { };

						}
					}

					retVal.AddRange(childEls);
				}
			}

			// Sort retVal By Name
			retVal.Sort(delegate(FilterExpressionNode x, FilterExpressionNode y)
			{ return x.Name.CompareTo(y.Name); });

			return new FilterExpressionNodeCollection(retVal.Select(x=>x.Clone(true)).ToArray());
		}


		/// <summary>
		/// Gets the element conditions.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public override ConditionElementCollection GetElementConditions(string expressionPlace, FilterExpressionNode node)
		{

			ConditionElementCollection retVal = new ConditionElementCollection();

			if (node != null)
			{
				ElementDefs elementDef = FindElementDef(node.Key);
				if (elementDef == null)
				{
					throw new ArgumentException("element  " + node.Key + " not registered");
				}
				//Default Decimal conditions
				var conditions = elementDef.Conditions ?? ConditionDecimalDefs;
				if (elementDef.HasChildren && node.Method != null)
				{
					ElementDefs methodDef = elementDef.GetMethodDef(node.Method.Key);
					if (methodDef == null)
					{
						throw new ArgumentException("method  " + node.Method.Key + " not registered");
					}
					conditions = methodDef.Conditions;
				}
				//Add stored condition elements in Tag  property in definition
				foreach (ElementDefs conditionDef in conditions)
				{
					retVal.Add(conditionDef.Tag as ConditionElement);
				}
			}
			return retVal;
		}


		/// <summary>
		/// Gets the element methods.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public override MethodElementCollection GetElementMethods(string expressionPlace, FilterExpressionNode node)
		{
			MethodElementCollection retVal = new MethodElementCollection();
			ElementDefs elementDef = FindElementDef(node.Key);
			if (elementDef != null && elementDef.Methods != null)
			{
				foreach (ElementDefs methodDef in elementDef.Methods)
				{
					retVal.Add(methodDef.Tag as MethodElement);
				}
			}
			return retVal;
		}
		#endregion

	
	}
}
