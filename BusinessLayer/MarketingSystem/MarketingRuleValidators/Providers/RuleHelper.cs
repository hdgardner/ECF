using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities.Rules;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Marketing.Validators.Providers.DomParser;
using System.CodeDom;

namespace Mediachase.Commerce.Marketing.Validators.Providers
{
    /// <summary>
    /// This class simplifies programming constructions of rules
    /// </summary>
    public class RuleHelper
    {
        /*
        public static RuleSet CreateRuleSet(string ruleSetName, FilterExpressionNodeCollection filters)
        {
            RuleSet customerRuleSet = new RuleSet(ruleSetName);
            customerRuleSet.ChainingBehavior = RuleChainingBehavior.None;
            customerRuleSet.Rules.Add(CreateRule(expressionKey, filters));
            return customerRuleSet;
        }
         * */

        /// <summary>
        /// Creates the rule.
        /// </summary>
        /// <param name="stringRuleName">Name of the string rule.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public static Rule CreateRule(string stringRuleName, int priority, FilterExpressionNodeCollection filters)
        {
            Rule rule = new Rule();
            rule.Active = true;
            rule.Priority = priority;
            rule.Name = stringRuleName;
            rule.ReevaluationBehavior = RuleReevaluationBehavior.Never;

            //Mediachase.Web.Console.Providers.DomParser
            string expr = ConvertNodesToStringExpression(filters);
            if (!String.IsNullOrEmpty(expr))
            {
                Parser p = new Parser();
                rule.Condition = new RuleExpressionCondition(p.ParseExpression(expr));
            }

            return rule;
        }

        /// <summary>
        /// Gets the "this.ValidationResult.IsValid = true" action, that can be assigned to TheActions for the Rule.
        /// </summary>
        /// <value>The is valid true action.</value>
        public static RuleStatementAction IsValidTrueAction
        {
            get
            {
                CodeThisReferenceExpression thisRef = new CodeThisReferenceExpression();
                CodePropertyReferenceExpression resultRef = new CodePropertyReferenceExpression(thisRef, "ValidationResult");
                CodePropertyReferenceExpression validRef = new CodePropertyReferenceExpression(resultRef, "IsValid");
                CodeAssignStatement ruleIsValidAction = new CodeAssignStatement(validRef, new CodePrimitiveExpression(true));
                return new RuleStatementAction(ruleIsValidAction);
            }
        }

        /// <summary>
        /// Gets the halt action.
        /// </summary>
        /// <value>The halt action.</value>
        public static RuleHaltAction HaltAction
        {
            get
            {
                return new RuleHaltAction();
            }
        }

        #region Helper methods
        private static string AddStringExpression(string master, string op, string expr)
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
                return String.Format("({0}) {1} ({2})", master, op.ToString(), expr);
            }
        }


        private static string ConvertNodesToStringExpression(FilterExpressionNodeCollection nodes)
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

            foreach (FilterExpressionNode node in newNodes)
            {
                CodeBinaryOperatorExpression currentExpression = new CodeBinaryOperatorExpression();
                if (node.NodeType == FilterExpressionNodeType.Element)
                {
                    masterExpression = AddStringExpression(masterExpression, "&&", ConvertNodeToStringExpression(node));
                }
                else if (node.NodeType == FilterExpressionNodeType.AndBlock)
                {
                    masterExpression = AddStringExpression(masterExpression, "&&", ConvertNodesToStringExpression(node.ChildNodes));
                }
                else if (node.NodeType == FilterExpressionNodeType.OrBlock)
                {
                    masterExpression = AddStringExpression(masterExpression, "||", ConvertNodesToStringExpression(node.ChildNodes));
                }
            }

            return masterExpression;
        }

        private static string ConvertNodeToStringExpression(FilterExpressionNode node)
        {
            string expression = String.Empty;

            if (node.NodeType == FilterExpressionNodeType.Element)
            {
                // Detect "NOT" statement
                string key = node.Condition.Key;
                bool isNegative = false;

                if (key.StartsWith("Not."))
                    key = key.Substring(key.IndexOf(".") + 1);

                if (node.Condition.Type == ConditionElementType.Text)
                {
                    expression = String.Format("{0}{1}.{2}(\"{3}\")", isNegative ? "!" : String.Empty, node.Key, key, node.Value);
                }
                else if (node.Condition.Type == ConditionElementType.NotSet)
                {

                }
                else
                {
                    expression = String.Format("{0}{1}.{2}({3})", isNegative ? "!" : String.Empty, node.Key, key, node.Value);
                }

                // Add a series of not null 
            }
            else
            {
                throw new ApplicationException("Invalid NodeType");
            }

            return expression;
        }

        /// <summary>
        /// Eliminates the empty nodes (nodes that consist of only logical elements).
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        private static FilterExpressionNodeCollection EliminateEmptyNodes(FilterExpressionNodeCollection nodes)
        {
            FilterExpressionNodeCollection expressions = new FilterExpressionNodeCollection();
            foreach (FilterExpressionNode node in nodes)
            {
                if (node.NodeType == FilterExpressionNodeType.Element)
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
        #endregion
    }
}
