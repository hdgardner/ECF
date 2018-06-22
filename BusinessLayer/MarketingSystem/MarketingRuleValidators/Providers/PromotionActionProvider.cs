using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Marketing.Dto;
using System.IO;
using System.Xml;
using System.Workflow.ComponentModel.Serialization;
using Mediachase.Commerce.Marketing.Validators.Providers.DomParser;
using System.CodeDom;
using System.Globalization;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
	/// <summary>
	/// Represents the fileter expression povider for creation custom reward.
	/// </summary>
	public class PromotionActionProvider : BaseRuleSetFilterExpressionProvider
	{
		private  Dictionary<string, IEnumerable<FilterExpressionNode>> _filterElementsMap;

		#region Properties
		#region FilterElement definitions
		protected  IEnumerable<ElementDefs> ActionsOrderDefs { get; private set; }
		protected  IEnumerable<ElementDefs> ActionsEntryDefs { get; private set; }
		#endregion
		#region FilterElement Method Definition
		protected  ElementDefs MethodEachEntryDefs { get; private set; }
		protected  ElementDefs MethodAllEntryDefs { get; private set; }
		protected  ElementDefs MethodAllEntryPercentDefs { get; private set; }
		protected  ElementDefs MethodGiftDefs { get; private set; }
		#endregion

		#region FilterElement Conditions Definitions
		protected  ElementDefs ConditionSkuSelectDefs { get; private set; }
		protected  ElementDefs ConditionFreeShipingDefs { get; private set; }
		protected  ElementDefs ConditionPercentDecimalDefs { get; private set; }
		protected  ElementDefs ConditionDecimalDefs { get; private set; }
		#endregion

		#endregion



		#region BaseRuleSetFilterExpressionProvider overrides

		/// <summary>
		/// Registers the provider element definitions.
		/// </summary>
		protected override void RegisterProviderElementDefinitions()
		{
			//First call base implementation
			base.RegisterProviderElementDefinitions();

			#region Condition definitions
			//Conditions not need const key registration in elementDef because is not used fore searching 
			ConditionSkuSelectDefs = new ElementDefs() { Key = ElementDefs.Action_Condition_SkuSelect, Name = "of SKU", Descr = "of SKU" };
			ConditionFreeShipingDefs = new ElementDefs() {  Key = ElementDefs.Action_Condition_FreeShiping,  Name = "", Descr = "" };
			ConditionPercentDecimalDefs = new ElementDefs() {  Key = ElementDefs.Action_Condition_PercentDecimal,  Name = "reward", Descr = "reward" };
			ConditionDecimalDefs = new ElementDefs() { Key = ElementDefs.Action_Condition_Decimal, Name = "reward", Descr = "reward" };
			#endregion

			#region Method definitions
			MethodEachEntryDefs = new ElementDefs() { Key = ElementDefs.Method_Action_EachEntry, Name = "", Descr = "reward amount" };
			MethodAllEntryDefs = new ElementDefs() { Key = ElementDefs.Method_Action_AllEntry, Name = "", Descr = "reward amount" };
			MethodAllEntryPercentDefs = new ElementDefs() { Key = ElementDefs.Method_Action_AllEntryPercent, Name = "", Descr = "reward amount" };
			MethodGiftDefs = new ElementDefs() { Key = ElementDefs.Method_Action_Gift, Name = "", Descr = "gift quantity" };
			#endregion


			#region element definitions
			ActionsEntryDefs =
				 new ElementDefs[] {
					 				new ElementDefs() {Key = ElementDefs.Action_AllEntryValueOfItem, Name = PromotionRewardAmountType.Value + ":" + PromotionRewardType.AllAffectedEntries, 
														Descr = "get $ off item" , Conditions = new ElementDefs[] { ConditionDecimalDefs }},
									new ElementDefs() {Key = ElementDefs.Action_AllEntryPercentOfItem, Name = PromotionRewardAmountType.Percentage + ":" + PromotionRewardType.AllAffectedEntries, 
														Descr = "get % off item", Conditions = new ElementDefs[] { ConditionPercentDecimalDefs } }
								
				 };
			RegisterDefinitions(ActionsEntryDefs);

			ActionsOrderDefs =
				 new ElementDefs[] {
									new ElementDefs() {Key = ElementDefs.Action_WholeOrderValue, Name = PromotionRewardAmountType.Value + ":" + PromotionRewardType.WholeOrder,
													  Descr = "get $ off whole order", Conditions = new ElementDefs[] { ConditionDecimalDefs }},

									new ElementDefs() {Key = ElementDefs.Action_WholeOrderPercent, Name = PromotionRewardAmountType.Percentage + ":" + PromotionRewardType.WholeOrder, 
													  Descr = "get % off whole order", Conditions = new ElementDefs[] { ConditionPercentDecimalDefs } },

									new ElementDefs() {Key = ElementDefs.Action_AllEntryValueOfSkuSet, Name = PromotionRewardAmountType.Value + ":" + PromotionRewardType.AllAffectedEntries, 
													  Descr = "get $ fixed", Methods = new ElementDefs[] { MethodAllEntryDefs } , Conditions = new ElementDefs[] { ConditionSkuSelectDefs } },

									new ElementDefs() {Key = ElementDefs.Action_AllEntryPercentOfSkuSet, Name = PromotionRewardAmountType.Percentage + ":" + PromotionRewardType.AllAffectedEntries, 
													  Descr = "get % fixed", Methods = new ElementDefs[] { MethodAllEntryPercentDefs }, Conditions = new ElementDefs[] { ConditionSkuSelectDefs } },

									new ElementDefs() {Key = ElementDefs.Action_EachEntryValueOfSkuSet, Name = PromotionRewardAmountType.Value + ":" + PromotionRewardType.EachAffectedEntry, 
													  Descr = "get $ per each item", Methods = new ElementDefs[] { MethodEachEntryDefs }, Conditions = new ElementDefs[] { ConditionSkuSelectDefs } },

									new ElementDefs() {Key = ElementDefs.Action_GiftOfSkuSet, Name = "", Descr = "get gift items", Methods = new ElementDefs[] { MethodGiftDefs } , Conditions = new ElementDefs[] { ConditionSkuSelectDefs } }

				 };
			RegisterDefinitions(ActionsOrderDefs);

			#endregion

		}

		/// <summary>
		/// Creates the filter nodes.
		/// </summary>
		protected override void CreateFilterNodes()
		{
			//First call base implementation
			base.CreateFilterNodes();

			_filterElementsMap = new Dictionary<string, IEnumerable<FilterExpressionNode>>();

			#region Entry
			string promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;
			List<FilterExpressionNode> tmpColl = new List<FilterExpressionNode>();
			tmpColl.AddRange(ActionsEntryDefs.Select(x => CreateFilterExpressionNode(x)));
			_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion
			#region OrderGroup
			promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
			tmpColl = new List<FilterExpressionNode>();
			tmpColl.AddRange(ActionsOrderDefs.Select(x => CreateFilterExpressionNode(x)));
			_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion
			#region Shipment
			//tmpColl = new List<FilterExpressionNode>();
			//promotionGroupKey = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key;
			//tmpColl.AddRange(new FilterExpressionNode[] {
			//        CreateFilterExpressionNode(eAvailableActions.EachShipping.ToString(), "get $ off shipping per item", FilterExpressionNodeType.Element),
			//        CreateFilterExpressionNode(eAvailableActions.EachShippingPercent.ToString(), "get % off shipping per item", FilterExpressionNodeType.Element),
			//        CreateFilterExpressionNode(eAvailableActions.AllShipping.ToString(), "get $ off whole shipping", FilterExpressionNodeType.Element),
			//        CreateFilterExpressionNode(eAvailableActions.AllShippingPercent.ToString(), "get % off whole shipping", FilterExpressionNodeType.Element),
			//        CreateFilterExpressionNode(eAvailableActions.FreeShipping.ToString(), "get free shipping", FilterExpressionNodeType.Element)
			//        });
			//_filterElementsMap.Add(promotionGroupKey, tmpColl);
			#endregion

			#region Register method elemets
			//Each entry
			CreateMethodElement(MethodEachEntryDefs, "~/Apps/Marketing/ExpressionFunctions/actionRewardTwoParam.ascx");
			//All entry
			CreateMethodElement(MethodAllEntryDefs, "~/Apps/Marketing/ExpressionFunctions/actionRewardOneParam.ascx");
			//All entry percent
			CreateMethodElement(MethodAllEntryPercentDefs, "~/Apps/Marketing/ExpressionFunctions/actionRewardOneParamPercent.ascx");
			//Gift
			CreateMethodElement(MethodGiftDefs, "~/Apps/Marketing/ExpressionFunctions/actionRewardOneParam.ascx");
			#endregion


			#region Register conditions elements
			//Decimal
			CreateConditionEement(ConditionDecimalDefs, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/DecimalFilter.ascx");
			//Percent
			CreateConditionEement(ConditionPercentDecimalDefs, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/DecimalPercentFilter.ascx");
			//SkuSet
			CreateConditionEement(ConditionSkuSelectDefs, ConditionElementType.Custom, "~/Apps/Marketing/ExpressionFunctions/CatalogEntryFilter.ascx");
			#endregion
		}

		/// <summary>
		/// Creates the filter expression node by him definitions
		/// </summary>
		/// <param name="elementDef">The element def.</param>
		/// <returns></returns>
		protected override FilterExpressionNode CreateFilterExpressionNode(ElementDefs elementDef)
		{
			FilterExpressionNode retVal = base.CreateFilterExpressionNode(elementDef);

			retVal.Attributes = new System.Collections.Specialized.NameValueCollection();
			//Disable adding child elements
			retVal.Attributes.Add(FilterExpressionNode.filterExpressionChildEnableKey, false.ToString());

			return retVal;
		}


		/// <summary>
		/// Converts the node to string expression.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		protected override string ConvertNodeToStringExpression(FilterExpressionNode node)
		{
			string expression = String.Empty;

			decimal quantity = 0;
			decimal rewardAmount = 0;
			var skuList = new string[] { };
			MethodElementParams methodParams = null;
			//Find registered element definition by key
			ElementDefs elementDef = FindElementDef(node.Key);
			if (elementDef == null)
			{
				throw new ArgumentException("element " + node.Key + " not registered");
			}
			if (elementDef.Methods != null)
			{
				skuList = node.Value != null ? ((Dictionary<string, string>)node.Value).Keys.ToArray() : skuList;
				skuList = EntryId2LineItemCode(skuList).ToArray();
				methodParams = node.Method.Params;
			}
			else
			{
				rewardAmount = Convert.ToDecimal(node.Value);
			}
			//Find registered method definition by key
			ElementDefs methodDef = null;
			if (elementDef.Methods != null)
			{
				methodDef = elementDef.GetMethodDef(node.Method.Key);
			}

			string[] namePart = elementDef.Name.Split(':');

			string rewardAmountType = PromotionRewardAmountType.Value;
			string rewardType = PromotionRewardType.WholeOrder;
			if (namePart.Count() == 2)
			{
				rewardAmountType = namePart[0];
				rewardType = namePart[1];
			}

			//gift
			if (methodDef == MethodGiftDefs)
			{
				if (skuList.Count() != 0)
				{
					
					quantity = Convert.ToDecimal(methodParams[0]);
					var skuPattern = skuList.Select(x => String.Format("\"{0}\"", x));
					expression = String.Format("this.AddFreeGiftToCart(\"{0}\", {1})", 
												quantity.ToString(CultureInfo.InvariantCulture), 
												String.Join(",", skuPattern.ToArray()));
				}
			} //with quantity, amount, sku list
			else if (methodDef == MethodEachEntryDefs)
			{
				quantity = Convert.ToDecimal(methodParams[1]);
				rewardAmount = Convert.ToDecimal(methodParams[0]);
				if (skuList.Count() != 0)
				{
					var entryPattern = "this.CreatePromotionEntriesSetFromTarget(\"{0}\", {1})";
					expression = String.Format("AddPromotionItemRecord(this.CreatePromotionReward(\"{0}\", \"{1}\", \"{2}\"), {3})",
												rewardType, rewardAmount.ToString(CultureInfo.InvariantCulture), rewardAmountType,
												String.Join(",", skuList.Select(x => String.Format(entryPattern, x, quantity)).ToArray()));
				}
			} //with amount, sku list
			else if (methodDef == MethodAllEntryDefs || methodDef == MethodAllEntryPercentDefs)
			{
				rewardAmount = Convert.ToDecimal(methodParams[0]);
				if (skuList.Count() != 0)
				{
					var entryPattern = "this.CreatePromotionEntriesSetFromTarget(\"{0}\")";
					expression = String.Format("AddPromotionItemRecord(this.CreatePromotionReward(\"{0}\", \"{1}\", \"{2}\"), {3})",
												rewardType, rewardAmount.ToString(CultureInfo.InvariantCulture), rewardAmountType,
												String.Join(",", skuList.Select(x => String.Format(entryPattern, x)).ToArray()));
				}
			} // only amount
			else
			{
				var pattern = "this.AddPromotionItemRecord(this.CreatePromotionReward(\"{0}\", \"{1}\", \"{2}\")," +
							  "this.PromotionContext.TargetEntriesSet)";
				expression = String.Format(pattern, rewardType, rewardAmount.ToString(CultureInfo.InvariantCulture), rewardAmountType);

			}
			
			return expression;
		}
		#endregion

		#region Provider Helper Methods
		private IEnumerable<string> EntryId2LineItemCode(IEnumerable<string> entryIds)
		{
			List<string> retVal = new List<string>();
			if (CatalogContext.Current != null)
			{
				foreach (string strEntryId in entryIds)
				{
					int entryId;
					//This value from web control
					if (Int32.TryParse(strEntryId, out entryId))
					{
						Mediachase.Commerce.Catalog.Objects.Entry entry =
											CatalogContext.Current.GetCatalogEntry(entryId);
						if (entry != null)
						{
							retVal.Add(entry.ID);
						}
					}
					else
					{
						//this value from code or test
						retVal.Add(strEntryId);
					}
				}
			}

			return retVal;
		}

		private void SaveExpression(RuleSet set, int expressionId)
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

		private RuleSet GetPromotionRuleSet(string expressionPlace, string expressionKey)
		{
			RuleSet retVal = null;

			int expressionId = Int32.Parse(expressionKey);
			if (expressionId > 0)
			{
				if (DataSource == null)
					throw new NullReferenceException("DataSource is null");

				ExpressionDto dto = GetDataSourceDto(DataSource);
				ExpressionDto.ExpressionRow row = dto.Expression.FindByExpressionId(expressionId);

				if (row != null) // new one
				{
					StringReader stringReader = new StringReader(row.ExpressionXml);
					XmlTextReader reader = new XmlTextReader(stringReader);
					WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
					retVal = serializer.Deserialize(reader) as RuleSet;
				}
			}

			return retVal;
		}

		protected virtual IEnumerable<Rule> CreateRuleActions(string expressionKey, FilterExpressionNodeCollection filters)
		{
			int index = 0;
			foreach (FilterExpressionNode filterExp in filters)
			{
				string expr = ConvertNodeToStringExpression(filterExp);
				if (!String.IsNullOrEmpty(expr))
				{
					Parser p = new Parser();

					Rule rule = new Rule();
					rule.Active = true;
					rule.Priority = index;
					rule.Name = expressionKey + "_action" + index.ToString();
					rule.ReevaluationBehavior = RuleReevaluationBehavior.Never;

					//condition
					// this.ValidationResult.IsValid == True
					CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
					CodePropertyReferenceExpression resultRef = new CodePropertyReferenceExpression(thisRef, "ValidationResult");
					CodePropertyReferenceExpression leftRef = new CodePropertyReferenceExpression(resultRef, "IsValid");

					CodePrimitiveExpression righRef = new CodePrimitiveExpression(true);


					CodeBinaryOperatorExpression isValidOperator = new CodeBinaryOperatorExpression();
					isValidOperator.Operator = CodeBinaryOperatorType.ValueEquality;
					isValidOperator.Left = leftRef;
					isValidOperator.Right = righRef;

					rule.Condition = new RuleExpressionCondition(isValidOperator);

					//then action
					rule.ThenActions.Add(new RuleStatementAction(p.ParseExpression(expr)));

					index++;

					yield return rule;
				}
			}

		}

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

		private IEnumerable<CatalogEntryDto.CatalogEntryRow> GetCatalogEntrys(IEnumerable<string> catalogNames)
		{
			// Changed to return all entries here
			string entryType = String.Empty; //Mediachase.Commerce.Catalog.Objects.EntryType.Product.ToString();

			CatalogSearchParameters pars = new CatalogSearchParameters();
			CatalogSearchOptions options = new CatalogSearchOptions();

			options.RecordsToRetrieve = int.MaxValue;
			options.Namespace = "Mediachase.Commerce.Catalog";
			options.StartingRecord = 0;
			options.ReturnTotalCount = true;
			//pars.SqlWhereClause = String.Format("[CatalogEntry].Name like '%{0}%' OR [CatalogEntry].Code like '%{0}%'", sFilter);

			// Add catalogs
			pars.CatalogNames.AddRange(catalogNames.ToArray());
			//CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();
			//foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
			//{
			//    pars.CatalogNames.Add(catalogRow.Name);
			//}

			int totalRecords = 0;
			CatalogEntryDto dto = CatalogContext.Current.FindItemsDto(pars, options, ref totalRecords);
			foreach (CatalogEntryDto.CatalogEntryRow entryRow in dto.CatalogEntry)
			{
				//ComboBoxItem item = new ComboBoxItem(entryRow.Name + " [" + entryRow.Code.ToString() + "]");
				//item.Value = entryRow.Code.ToString();
				//item["icon"] = Page.ResolveClientUrl(String.Format("~/app_themes/Default/images/icons/{0}.gif", entryRow.ClassTypeId));
				//EntriesFilter.Items.Add(item);
				yield return entryRow;
			}
		}

		#endregion

		#region Overrides FilterExpressionProvider methods
		/// <summary>
		/// Gets the new elements.
		/// </summary>
		/// <param name="expressionPlace">The expression place.</param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public override FilterExpressionNodeCollection GetNewElements(string expressionPlace, FilterExpressionNode parent)
		{
			FilterExpressionNodeCollection retVal = new FilterExpressionNodeCollection();
			//set default group
			string currentPromotionGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
			string[] expressionPlaceParts = expressionPlace.Split(':');
			if (expressionPlaceParts.Length != 0)
			{
				currentPromotionGroup = expressionPlaceParts[0];
			}
			IEnumerable<FilterExpressionNode> resultColl;
			_filterElementsMap.TryGetValue(currentPromotionGroup, out resultColl);
			if (resultColl != null)
			{
				retVal.AddRange(resultColl.Select(x => x.Clone()).ToArray());
			}

			return retVal;
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

			ElementDefs elementDef = FindElementDef(node.Key);
			if (elementDef == null)
			{
				throw new ArgumentException("element  " + node.Key + " not registered");
			}

			foreach (ElementDefs condDef in elementDef.Conditions)
			{
				retVal.Add(condDef.Tag as ConditionElement);
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
				foreach (ElementDefs methodDefs in elementDef.Methods)
				{
					retVal.Add(methodDefs.Tag as MethodElement);
				}
			}
			return retVal;
		}


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
		/// <param name="expressionKey">The expression key.</param>
		/// <param name="filters">The filters.</param>
		public override void SaveFilters(string expressionPlace, string expressionKey, FilterExpressionNodeCollection filters)
		{
			RuleSet ruleSet = GetPromotionRuleSet(expressionPlace, expressionKey);

			if (ruleSet != null)
			{
				foreach (Rule actionRule in CreateRuleActions(expressionKey, filters))
				{
					ruleSet.Rules.Add(actionRule);
				}
			}

			SaveExpression(ruleSet, Int32.Parse(expressionKey));
		}

		#endregion




	}
}
