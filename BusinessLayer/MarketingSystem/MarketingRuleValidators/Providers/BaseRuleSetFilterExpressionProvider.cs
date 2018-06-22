using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Workflow.Activities.Rules;
using System.CodeDom;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Marketing;
using System.IO;
using System.Globalization;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using Mediachase.Commerce.Marketing.Validators.Providers.DomParser;
using Mediachase.MetaDataPlus.Configurator;
using System.Web;
using System.Linq;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
    /// <summary>
    /// Base class for Filter Expression Providers that use Windows Workflow Foundation Rules engine to store and retrieve expressions.
    /// </summary>
    public abstract class BaseRuleSetFilterExpressionProvider : FilterExpressionProvider
    {

		private Dictionary<string, ElementDefs> AllRegisteredDefinitionsMap = 
																new Dictionary<string, ElementDefs>();
		private Dictionary<MetaDataType, IEnumerable<ElementDefs>> MetaDataType2ElDefMap = 
																new Dictionary<MetaDataType, IEnumerable<ElementDefs>>();

		#region FilterElement Conditions Definitions
		protected IEnumerable<ElementDefs> ConditionTextDefs { get; private set; }
		protected IEnumerable<ElementDefs> ConditionDecimalDefs { get; private set; }
		protected IEnumerable<ElementDefs> ConditionBoolDefs { get; private set; }
		protected IEnumerable<ElementDefs> ConditionIntegerDefs { get; private set; }
		#endregion

		//do not show keys and disabled fields and file type
		private Func<MetaField, bool>[] customFieldFiter = new Func<MetaField, bool>[] { 
																						x=>x.Enabled, 
																						x=>!x.IsSystem
																					  };

		#region Ctor
		public BaseRuleSetFilterExpressionProvider()
		{
			Initialize();
		} 
		#endregion

		#region Element definitions support methods
		private void Initialize()
		{
			RegisterProviderElementDefinitions();
			CreateFilterNodes();
		}

		/// <summary>
		/// Registers the provider element definitions.
		/// </summary>
		protected virtual void RegisterProviderElementDefinitions()
		{
				#region Condition definitions
			ConditionTextDefs =
					new ElementDefs[] {
									 new ElementDefs() { Key = ElementDefs.Condition_Text_Equals, Name = "Equals", Descr= "Equals (Text)" },
									 new ElementDefs() { Key = ElementDefs.Condition_Text_NotEquals, Name = "Not.Equals", Descr= "Does not equal (Text)" },
									 new ElementDefs() { Key = ElementDefs.Condition_Text_Contains, Name = "Contains", Descr= "Contains" },
									 new ElementDefs() { Key = ElementDefs.Condition_Text_NotContains, Name = "Not.Contains", Descr= "Does not contain" },
									 new ElementDefs() { Key = ElementDefs.Condition_Text_StartsWith, Name = "StartsWith", Descr= "Begins with" },
									 new ElementDefs() { Key = ElementDefs.Condition_Text_EndsWith, Name = "EndsWith", Descr= "Ends with" }
				}.Select(x => { x.ConverstionPattern = "{0}.ToString()"; return x; });
			RegisterDefinitions(ConditionTextDefs);

			ConditionDecimalDefs =
				   new ElementDefs[] {
									 new ElementDefs() {  Key = ElementDefs.Condition_Decimal_Equals, Name = "==", Descr= "Equals" },
									 new ElementDefs() {  Key = ElementDefs.Condition_Decimal_NotEquals, Name = "!=", Descr= "Not equal" },
									 new ElementDefs() {  Key = ElementDefs.Condition_Decimal_Less, Name = "<", Descr= "Less than" },
									 new ElementDefs() {  Key = ElementDefs.Condition_Decimal_Greater, Name = ">", Descr= "Greater than" }
				}.Select(x => { x.ConverstionPattern = "((decimal){0})"; return x; });
			RegisterDefinitions(ConditionDecimalDefs);

			ConditionIntegerDefs =
				   new ElementDefs[] {
									 new ElementDefs() { Key = ElementDefs.Condition_Int_Equals, Name = "==", Descr= "Equals" },
									 new ElementDefs() { Key = ElementDefs.Condition_Int_NotEquals, Name = "!=", Descr= "Not equal" },
									 new ElementDefs() { Key = ElementDefs.Condition_Int_Less, Name = "<", Descr= "Less than" },
									 new ElementDefs() { Key = ElementDefs.Condition_Int_Greater, Name = ">", Descr= "Greater than" }
				}.Select(x => { x.ConverstionPattern = "((int){0})"; return x; });
			RegisterDefinitions(ConditionIntegerDefs);

			ConditionBoolDefs =
					new ElementDefs[] {
									 new ElementDefs() { Key = ElementDefs.Condition_Bool_Equals, Name = "==", Descr= "Equals" }
				}.Select(x => { x.ConverstionPattern = "((bool){0})"; return x; });
			RegisterDefinitions(ConditionBoolDefs);
			#endregion

			//Register available conditions for McDataType types
			//Integer
			var metaDataTypes = new MetaDataType[]{    MetaDataType.BigInt, MetaDataType.Int, MetaDataType.Integer, 
													   MetaDataType.SmallInt, MetaDataType.Timestamp};
			foreach (MetaDataType metaDataType in metaDataTypes)
			{
				MetaDataType2ElDefMap.Add(metaDataType, ConditionIntegerDefs);
			}

			//Decimal
			metaDataTypes = new MetaDataType[] { MetaDataType.Decimal, MetaDataType.Money, MetaDataType.Numeric, 
												 MetaDataType.SmallMoney,   MetaDataType.Float, MetaDataType.Real};
			foreach (MetaDataType metaDataType in metaDataTypes)
			{
				MetaDataType2ElDefMap.Add(metaDataType, ConditionDecimalDefs);
			}

			//Text
			metaDataTypes = new MetaDataType[] { MetaDataType.LongHtmlString, MetaDataType.LongString, MetaDataType.NChar, 
												 MetaDataType.NText, MetaDataType.NVarChar, MetaDataType.ShortString,
												 MetaDataType.Text, MetaDataType.URL, MetaDataType.VarChar };
			foreach (MetaDataType metaDataType in metaDataTypes)
			{
				MetaDataType2ElDefMap.Add(metaDataType, ConditionTextDefs);
			}
			//Boolean
			metaDataTypes = new MetaDataType[] { MetaDataType.Bit, MetaDataType.Boolean, MetaDataType.MetaObject };
			foreach (MetaDataType metaDataType in metaDataTypes)
			{
				MetaDataType2ElDefMap.Add(metaDataType, ConditionBoolDefs);
			}

			//TODO: Not suppoerted types 	
			//  MetaDataType.UniqueIdentifier
			//  MetaDataType.Date, MetaDataType.SmallDateTime, , MetaDataType.DateTime

		}
		/// <summary>
		/// Creates the filter nodes.
		/// </summary>
		protected virtual void CreateFilterNodes()
		{
			//Nothing todo;
		}


		/// <summary>
		/// Searches in registered ElementDefs
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public ElementDefs FindElementDef(string key)
		{
			ElementDefs retVal = null;

			AllRegisteredDefinitionsMap.TryGetValue(key, out retVal);

			return retVal;
		}

		/// <summary>
		/// Registers the definitions in to cache map.
		/// </summary>
		/// <param name="definitions">The definitions.</param>
		protected void RegisterDefinitions(IEnumerable<ElementDefs> definitions)
		{

			foreach (ElementDefs definition in definitions)
			{
				if (AllRegisteredDefinitionsMap == null)
				{
					AllRegisteredDefinitionsMap = new Dictionary<string, ElementDefs>();
				}

				if (!AllRegisteredDefinitionsMap.ContainsKey(definition.Key))
				{
					AllRegisteredDefinitionsMap.Add(definition.Key, definition);
				}

			}
		}

		/// <summary>
		/// Creates the method element by him definitions
		/// </summary>
		/// <param name="methodDef">The method def.</param>
		/// <param name="customControlPath">The custom control path.</param>
		/// <returns></returns>
		protected virtual MethodElement CreateMethodElement(ElementDefs methodDef, string customControlPath)
		{
			MethodElement retVal = new MethodElement(methodDef.Key, methodDef.Descr, methodDef.Descr);
			retVal.CustomControlPath = customControlPath;
			methodDef.Tag = retVal;

			return retVal;
		}

		/// <summary>
		/// Creates the condition eement by him definitions
		/// </summary>
		/// <param name="conditionDef">The condition def.</param>
		/// <param name="condElType">Type of the cond el.</param>
		/// <param name="customControlPath">The custom control path.</param>
		/// <returns></returns>
		protected virtual ConditionElement CreateConditionEement(ElementDefs conditionDef, ConditionElementType condElType,
															 string customControlPath)
		{
			ConditionElement retVal = new ConditionElement(condElType, conditionDef.Key,
														   conditionDef.Descr, conditionDef.Descr);
			retVal.CustomControlPath = customControlPath;
			conditionDef.Tag = retVal;

			return retVal;
		}

		/// <summary>
		/// Creates the filter expression node.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="displayName">The display name.</param>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		protected virtual IEnumerable<ElementDefs> GetElementDefsByMetaClass(string pattern, MetaClass metaClass)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			foreach (MetaField field in metaClass.MetaFields)
			{
				if (customFieldFiter.All(x => x(field)))
				{
					IEnumerable<ElementDefs> conditonDef;
					if (MetaDataType2ElDefMap.TryGetValue(field.DataType, out conditonDef))
					{
						string codeExpr = string.IsNullOrEmpty(pattern) ? field.Name : string.Format(pattern, field.Name);
						string dynamicKey = ElementDefs.GetDynamicKey(metaClass.Name + "." + field.Name + "." + codeExpr);
						ElementDefs retVal = new ElementDefs() { Key = dynamicKey, Name = codeExpr, 
																	 Descr = field.FriendlyName, Conditions = conditonDef };
						yield return retVal;
					}

				}
			}
		}

		/// <summary>
		/// Creates the filter expression node by him definitions
		/// </summary>
		/// <param name="elementDef">The element def.</param>
		/// <returns></returns>
		protected virtual FilterExpressionNode CreateFilterExpressionNode(ElementDefs elementDef)
		{
			FilterExpressionNode retVal = new FilterExpressionNode(elementDef.Key, elementDef.Descr);
			elementDef.Tag = retVal;

			if (elementDef.Childrens != null || elementDef.Methods != null)
			{
				retVal.NodeType = FilterExpressionNodeType.MethodBlock;
				retVal.ChildNodes = new FilterExpressionNodeCollection();
				if (elementDef.Childrens != null)
				{
					retVal.ChildNodes.AddRange(elementDef.Childrens.Select(x => CreateFilterExpressionNode(x)).ToArray());
				}
			}

			return retVal;
		} 
		#endregion
		
        #region Rules Helper Methods - Saving Operation
        /// <summary>
        /// Eliminates the empty nodes (nodes that consist of only logical elements).
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        protected virtual FilterExpressionNodeCollection EliminateEmptyNodes(FilterExpressionNodeCollection nodes)
        {
            FilterExpressionNodeCollection expressions = new FilterExpressionNodeCollection();
            foreach (FilterExpressionNode node in nodes)
            {
				if (node.NodeType == FilterExpressionNodeType.Element 
					|| node.NodeType == FilterExpressionNodeType.MethodBlock)
				{
					expressions.Add(node);
				}
				else
				{
					// Only add node if it has the actual "Element's" and does not consist of only logical AND's and OR's
					if (node.HasChildNodes)
					{
						FilterExpressionNodeCollection subTree = EliminateEmptyNodes(node.ChildNodes);
						if (subTree != null && subTree.Count > 0)
							expressions.Add(node);
					}
				}
            }

            return expressions;
        }

        #region Using DOM
        /// <summary>
        /// Adds the new expression to the current master.
        /// </summary>
        /// <param name="master">The master.</param>
        /// <param name="op">The op.</param>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual CodeBinaryOperatorExpression AddExpression(CodeBinaryOperatorExpression master, CodeBinaryOperatorType op, CodeBinaryOperatorExpression expr)
        {
            if (master == null)
            {
                return expr;
            }
            else
            {
                return new CodeBinaryOperatorExpression(master, op, expr);
            }
        }


        /// <summary>
        /// Converts the nodes to expression.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        protected virtual CodeBinaryOperatorExpression ConvertNodesToExpression(FilterExpressionNodeCollection nodes)
        {
            FilterExpressionNodeCollection newNodes = EliminateEmptyNodes(nodes);

            // If not nodes found return null
            if (newNodes.Count == 0)
                return null;

            // If only one node, return it as an expression
            if (newNodes.Count == 1)
            {
                return ConvertNodeToExpression(newNodes[0]);
            }

            // Start processing more than 1 node
            CodeBinaryOperatorExpression masterExpression = null;

            foreach (FilterExpressionNode node in newNodes)
            {
                CodeBinaryOperatorExpression currentExpression = new CodeBinaryOperatorExpression();
                if (node.NodeType == FilterExpressionNodeType.Element)
                {
                    masterExpression = AddExpression(masterExpression, CodeBinaryOperatorType.BooleanAnd, ConvertNodeToExpression(node));
                }
                else if (node.NodeType == FilterExpressionNodeType.AndBlock)
                {
                    masterExpression = AddExpression(masterExpression, CodeBinaryOperatorType.BooleanAnd, ConvertNodesToExpression(node.ChildNodes));
                }
                else if (node.NodeType == FilterExpressionNodeType.OrBlock)
                {
                    masterExpression = AddExpression(masterExpression, CodeBinaryOperatorType.BooleanOr, ConvertNodesToExpression(node.ChildNodes));
                }
            }

            return masterExpression;
        }

        /// <summary>
        /// Converts the node to block.
        /// </summary>
        /// <param name="leftNode">The left node.</param>
        /// <param name="rightNode">The right node.</param>
        /// <returns></returns>
        protected virtual CodeBinaryOperatorExpression ConvertNodeToBlock(FilterExpressionNode leftNode, FilterExpressionNode rightNode)
        {
            CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression();
            condition.Left = ConvertNodeToExpression(leftNode);
            condition.Operator = CodeBinaryOperatorType.BooleanAnd;
            condition.Right = ConvertNodeToExpression(rightNode);
            return condition;
        }

        /// <summary>
        /// Converts the node to expression.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected virtual CodeBinaryOperatorExpression ConvertNodeToExpression(FilterExpressionNode node)
        {
            CodeBinaryOperatorExpression expression = null;

            if (node.NodeType == FilterExpressionNodeType.Element)
            {
                CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
                CodeFieldReferenceExpression leftExprRef = new CodeFieldReferenceExpression(thisRef, node.Key);
                expression = new CodeBinaryOperatorExpression();
                expression.Left = leftExprRef;
                expression.Operator = (CodeBinaryOperatorType)Enum.Parse(typeof(CodeBinaryOperatorType), node.Condition.Key, true);
                expression.Right = new CodePrimitiveExpression(node.Value);
            }
            else
            {
                throw new ApplicationException("Invalid NodeType");
            }

            return expression;
        }

        protected virtual Rule CreateRule(string expressionKey, FilterExpressionNodeCollection filters)
        {
            Rule rule = new Rule();

            //Mediachase.Web.Console.Providers.DomParser
            CodeBinaryOperatorExpression expr = ConvertNodesToExpression(filters);
            if (expr != null)
                rule.Condition = new RuleExpressionCondition(expr);

            /*
            CodeAssignStatement ruleSurchargeAction = new CodeAssignStatement(SurchargeRef, new CodePrimitiveExpression(500));
            carChargeRule.ThenActions.Add(new RuleStatementAction(ruleSurchargeAction));
             * */

            return rule;
        }
        #endregion


        #region Using String Expression
        protected virtual string AddStringExpression(string master, string op, string expr)
        {
            if (String.IsNullOrEmpty(master))
            {
                return expr;
            }
            else if (String.IsNullOrEmpty(expr))
            {
                return master;
            }
            else
            {
                return String.Format("{0} {1} {2}", master, op.ToString(), expr);
            }
        }

		/// <summary>
		/// Converts the node to string expression.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
        protected virtual string ConvertNodeToStringExpression(FilterExpressionNode node)
        {
			string retVal = String.Empty;
	
			ElementDefs elementDef = FindElementDef(node.Key);
			if (elementDef == null)
			{
				throw new ArgumentException("element  " + node.Key + " not registered");
			}

			if (node.Condition != null)
			{
				ElementDefs condition = elementDef.GetConditionDefByName(node.Condition.Key);
				//Apply type conversion pattern 
				string nodeCode = string.Format(condition.ConverstionPattern, elementDef.Name);
				string conditionCode = condition.Name;
                bool isNegative = false;
				// Detect "NOT" statement
				if (conditionCode.StartsWith("Not."))
				{
					conditionCode = conditionCode.Substring(conditionCode.IndexOf(".") + 1);
					isNegative = true;
				}
				string nodeValue = GetEscapedAndCleanedExpressionNodeRightValue(node);
				//for text conditions nees specific format
                if (ConditionTextDefs.Contains(condition))
                {
					retVal = String.Format("{0}{1}.{2}(\"{3}\")", isNegative ? "!" : String.Empty, nodeCode, conditionCode, nodeValue);
                }
                else
                {
					retVal = String.Format("{0} {1} {2}", nodeCode, conditionCode, nodeValue);
                }
            }
			else
			{
				throw new ApplicationException("Invalid NodeType");
			}

			return retVal;
        }

		protected virtual string ConvertNodesToStringExpression(FilterExpressionNodeCollection nodes)
		{
			return ConvertNodesToStringExpression(nodes, "&&");
		}

        protected virtual string ConvertNodesToStringExpression(FilterExpressionNodeCollection nodes, string separator)
        {
            FilterExpressionNodeCollection newNodes = EliminateEmptyNodes(nodes);

            // If not nodes found return null
            if (newNodes.Count == 0)
                return null;

            // If only one node, return it as an expression
            if (newNodes.Count == 1)
            {
                return ConvertNodeToStringExpression(newNodes[0]);
            }

            // Start processing more than 1 node
            string masterExpression = String.Empty;

			//And block default
			separator = separator ?? "&&";

            foreach (FilterExpressionNode node in newNodes)
            {
                CodeBinaryOperatorExpression currentExpression = new CodeBinaryOperatorExpression();
                if (node.NodeType == FilterExpressionNodeType.Element)
                {
                    masterExpression = AddStringExpression(masterExpression, separator, ConvertNodeToStringExpression(node));
                }
                else if (node.NodeType == FilterExpressionNodeType.AndBlock)
                {
					masterExpression = AddStringExpression(masterExpression, separator, ConvertNodesToStringExpression(node.ChildNodes, "&&"));
                }
                else if (node.NodeType == FilterExpressionNodeType.OrBlock)
                {
					masterExpression = AddStringExpression(masterExpression, separator , ConvertNodesToStringExpression(node.ChildNodes, "||"));
                }
				else if (node.NodeType == FilterExpressionNodeType.MethodBlock)
				{
					masterExpression = AddStringExpression(masterExpression, separator , ConvertNodeToStringExpression(node));
				}
            }

            return string.Format("({0})", masterExpression);
        }

        protected virtual Rule CreateRule2(string expressionKey, FilterExpressionNodeCollection filters)
        {
            Rule rule = new Rule();
            rule.Active = true;

            //Mediachase.Web.Console.Providers.DomParser
            string expr = ConvertNodesToStringExpression(filters);
			RuleExpressionCondition conditionExpr = new RuleExpressionCondition();
			if (!String.IsNullOrEmpty(expr))
			{
				Parser p = new Parser();
				conditionExpr.Expression = p.ParseExpression(expr);
				rule.Name = expressionKey;
			}
			else
			{
				CodePrimitiveExpression truePrimitive = new CodePrimitiveExpression(true);
				conditionExpr.Expression = truePrimitive;
				rule.Name = "AlwaysTrue";
			}

			rule.Condition = conditionExpr;
			rule.Priority = 9999; //Nust be first evaluate
			rule.ReevaluationBehavior = RuleReevaluationBehavior.Never;

			// Create succeeded assignment
			// this.ValidationResult.IsValid = True
			CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
			CodePropertyReferenceExpression resultRef = new CodePropertyReferenceExpression(thisRef, "ValidationResult");
			CodePropertyReferenceExpression validRef = new CodePropertyReferenceExpression(resultRef, "IsValid");
			CodeAssignStatement ruleIsValidAction = new CodeAssignStatement(validRef, new CodePrimitiveExpression(true));
			rule.ThenActions.Add(new RuleStatementAction(ruleIsValidAction));

            return rule;
        }
        #endregion

        protected virtual RuleSet CreateRuleSet(string expressionPlace, string expressionKey, FilterExpressionNodeCollection filters)
        {
            RuleSet customerRuleSet = new RuleSet(String.Format("{0}-{1}-CustomerRuleSet", expressionPlace, expressionKey));
            customerRuleSet.ChainingBehavior = RuleChainingBehavior.None;
            customerRuleSet.Rules.Add(CreateRule2(expressionKey, filters));
            return customerRuleSet;
        }
        #endregion

        #region Rules Helper Methods - Loading Operation
        /// <summary>
        /// Loads the expression.
        /// </summary>
        /// <param name="expressionId">The expression id.</param>
        /// <returns></returns>
        protected virtual FilterExpressionNodeCollection LoadExpressionNodeCollectionFromXaml(string xaml)
        {
            FilterExpressionNodeCollection col = new FilterExpressionNodeCollection();

            if (!String.IsNullOrEmpty(xaml))
            {
                RuleSet rules = DeserializeRuleSet(xaml);
                col = ConvertRuleSetToExpressionNodes(rules);
            }

            return col;
        }

        /// <summary>
        /// Converts the rule set to expression nodes.
        /// </summary>
        /// <param name="rules">The rules.</param>
        /// <returns></returns>
        protected virtual FilterExpressionNodeCollection ConvertRuleSetToExpressionNodes(RuleSet rules)
        {
            FilterExpressionNodeCollection col = new FilterExpressionNodeCollection();

            foreach (Rule rule in rules.Rules)
            {
                if (rule.Condition != null)
                {
                    CodeExpression expr = ((RuleExpressionCondition)rule.Condition).Expression;
                    AddExpressionAsNode(ref col, expr);
                }
            }

            return col;
        }

        /// <summary>
        /// Adds the expression as node.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="expr">The expr.</param>
        protected virtual void AddExpressionAsNode(ref FilterExpressionNodeCollection col, CodeExpression expr)
        {
            if (expr is CodeBinaryOperatorExpression)
            {
                AddExpressionAsNode(ref col, (CodeBinaryOperatorExpression)expr);
            }
            else if (expr is CodeMethodInvokeExpression)
            {
                col.Add(ConvertExpressionToNode((CodeMethodInvokeExpression)expr));
            }
        }

        /// <summary>
        /// Adds the expression as node.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="expr">The expr.</param>
        protected virtual void AddExpressionAsNode(ref FilterExpressionNodeCollection col, CodeBinaryOperatorExpression expr)
        {
            if (expr.Left is CodeMethodInvokeExpression)
                col.Add(ConvertExpressionToNode((CodeMethodInvokeExpression)expr.Left));
            else if (expr.Left is CodeBinaryOperatorExpression)
                AddExpressionAsNode(ref col, expr.Left);

            // Only need an operator if right is an expression
            FilterExpressionNode blockNode = null;
            if (expr.Right is CodeBinaryOperatorExpression)
            {
                if (expr.Operator == CodeBinaryOperatorType.BooleanAnd)
                {
                    blockNode = new FilterExpressionNode(FilterExpressionNodeType.AndBlock, String.Empty);
                }
                else if (expr.Operator == CodeBinaryOperatorType.BooleanOr)
                {
                    blockNode = new FilterExpressionNode(FilterExpressionNodeType.OrBlock, String.Empty);
                }

                col.Add(blockNode);
                FilterExpressionNodeCollection blockCol = new FilterExpressionNodeCollection();
                AddExpressionAsNode(ref blockCol, expr.Right);
                blockNode.ChildNodes = blockCol;
            }

            if (expr.Right is CodeMethodInvokeExpression)
                col.Add(ConvertExpressionToNode((CodeMethodInvokeExpression)expr.Right));
        }

        /// <summary>
        /// Converts the expression to node.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual FilterExpressionNode ConvertExpressionToNode(CodeMethodInvokeExpression expr)
        {
            CodeExpression target = expr.Method.TargetObject;
            FilterExpressionNode node = null;

            string targetString = ConvertTargetToString(expr.Method.TargetObject);

            if (target is CodeMethodInvokeExpression)
            {
                // This is a safier alternative to doing direct propery call (we always assume call is made using the
                // method, for example this.GetPropertyValueSafe("ParameterName") and extract the first parameter)
                node = new FilterExpressionNode(String.Format("{0}(\"{1}\")", targetString, ExtractPropertyFromTarget(target)));
            }
            else
            {
                // This is a method that can be used to call property directly, for example this.Customer.Email,
                // but you need to make sure the Customer object and Email are not null.
                node = new FilterExpressionNode(ConvertTargetToString(target));
            }

            node.Name = String.Empty;
            node.Condition = new ConditionElement(ConditionElementType.Text, expr.Method.MethodName);

            if (expr.Parameters[0] is CodePrimitiveExpression)
                node.Value = ((CodePrimitiveExpression)expr.Parameters[0]).Value;

            return node;
            //CodePropertyReferenceExpression
        }

        /// <summary>
        /// Extracts the property from target.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual string ExtractPropertyFromTarget(CodeExpression expr)
        {
            if (expr is CodeMethodInvokeExpression)
            {
                CodeMethodInvokeExpression invoke = (CodeMethodInvokeExpression)expr;
                return ((CodePrimitiveExpression)invoke.Parameters[0]).Value.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Converts the target to string.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual string ConvertTargetToString(CodeExpression expr)
        {
            if (expr is CodeThisReferenceExpression)
            {
                return "this";
            }
            else if (expr is CodePropertyReferenceExpression)
            {
                return ConvertTargetToString((CodePropertyReferenceExpression)expr);
            }
            else if (expr is CodeMethodInvokeExpression)
            {
                return ConvertTargetToString((CodeMethodInvokeExpression)expr);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Converts the target to string.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual string ConvertTargetToString(CodeMethodInvokeExpression expr)
        {
            return String.Format("{0}.{1}", ConvertTargetToString(expr.Method.TargetObject), expr.Method.MethodName);
        }

        /// <summary>
        /// Converts the target to string.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual string ConvertTargetToString(CodePropertyReferenceExpression expr)
        {
            return String.Format("{0}.{1}", ConvertTargetToString(expr.TargetObject), expr.PropertyName);
        }
        #endregion
		#region Helper static methods


		/// <summary>
		/// Gets the escaped and cleaned expression node right value.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public static string GetEscapedAndCleanedExpressionNodeRightValue(FilterExpressionNode node)
		{
			string retVal = string.Empty;
			if (node != null && node.Value != null)
			{
				retVal = node.Value.ToString();
				if (retVal.Contains("\""))
				{
					throw new ArgumentException("Invalid char in node value");
				}
			}
			return retVal;
		}

		/// <summary>
		/// Filters the node find recursively.
		/// </summary>
		/// <param name="nodeCollection">The node collection.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static FilterExpressionNode FilterNodeFindRecursively(IEnumerable<FilterExpressionNode> nodeCollection,
														Func<FilterExpressionNode, bool> predicate)
		{
			FilterExpressionNode retVal = null;
			if (nodeCollection == null)
				throw new ArgumentNullException("nodeCollection");
			
			foreach (FilterExpressionNode node in nodeCollection)
			{
				if (node.ChildNodes != null)
				{
					retVal = FilterNodeFindRecursively(node.ChildNodes, predicate);
					if (retVal != null)
					{
						break;
					}
				}
				if (predicate(node))
				{
					retVal = node;
					break;
				}
			}

			return retVal;
		}
		#endregion

        /// <summary>
        /// Deserializes the rule set.
        /// </summary>
        /// <param name="ruleSetXml">The rule set XML.</param>
        /// <returns></returns>
        private RuleSet DeserializeRuleSet(string ruleSetXml)
        {
            if (!String.IsNullOrEmpty(ruleSetXml))
            {
                StringReader stringReader = new StringReader(ruleSetXml);
                XmlTextReader reader = new XmlTextReader(stringReader);
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                return serializer.Deserialize(reader) as RuleSet;
            }
            else
            {
                return null;
            }
        }

        #region Meta Class/Meta Fields
        /// <summary>
        /// Gets the new elements.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <param name="namePrefix">The name prefix.</param>
        protected virtual void GetNewElements(List<FilterExpressionNode> collection, MetaClass metaClass, string keyPrefix, string namePrefix)
        {
            foreach (MetaField field in metaClass.MetaFields)
            {
                string key = String.Format("{0}(\"{1}\")", keyPrefix, field.Name);
                FilterExpressionNode node = new FilterExpressionNode(key,
                        namePrefix + field.FriendlyName);

                collection.Add(node);
            }
        }

        /// <summary>
        /// Gets the element conditions.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        protected virtual ConditionElementCollection GetElementConditions(MetaField field)
        {
            ConditionElementCollection retVal = new ConditionElementCollection();

            switch (field.DataType)
            {
                case MetaDataType.ShortString:
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Equals", "Equals", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Not.Equals", "Does not equal", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Contains", "Contains", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Not.Contains", "Does not contain", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "StartsWith", "Begins with", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "EndsWith", "Ends with", string.Empty));
                    break;
                case MetaDataType.LongString:
                case MetaDataType.LongHtmlString:
                case MetaDataType.NText:
                case MetaDataType.Text:
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Contains", "Contains", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "Not.Contains", "Does not contain", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "StartsWith", "Begins with", string.Empty));
                    retVal.Add(new ConditionElement(ConditionElementType.Text, "EndsWith", "Ends with", string.Empty));
                    break;
            }

            return retVal;
        }
        #endregion


        /// <summary>
        /// Gets the element methods.
        /// </summary>
        /// <param name="expressionPlace">The expression place.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public override MethodElementCollection GetElementMethods(string expressionPlace, FilterExpressionNode node) {return null;}

        /// <summary>
        /// Gets the new elements.
        /// </summary>
        /// <param name="expressionPlace">The expression place.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public override FilterExpressionNodeCollection GetNewElements(string expressionPlace, FilterExpressionNode parent) {return null;}

        /// <summary>
        /// Gets the element conditions.
        /// </summary>
        /// <param name="expressionPlace">The expression place.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public override ConditionElementCollection GetElementConditions(string expressionPlace, FilterExpressionNode node)
        {
            return null;
        }

        /// <summary>
        /// Loads the filters.
        /// </summary>
        /// <param name="expressionPlace">The expression place.</param>
        /// <param name="expressionKey">The expression key.</param>
        /// <returns></returns>
        public override FilterExpressionNodeCollection LoadFilters(string expressionPlace, string expressionKey)
        {
            return null;
        }

        /// <summary>
        /// Saves the filters.
        /// </summary>
        /// <param name="expressionPlace">The expression place.</param>
        /// <param name="expressionKey">The expression key.</param>
        /// <param name="filters">The filters.</param>
        public override void SaveFilters(string expressionPlace, string expressionKey, FilterExpressionNodeCollection filters)
        {
        }
    }
}
