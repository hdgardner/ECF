using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Mediachase.Commerce.Marketing.Validators
{
    public class PropertyCondition : ConditionBase
    {
        private string _PropertyName;

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }
        private string _PropertyValue;

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        /// <value>The property value.</value>
        public string PropertyValue
        {
            get { return _PropertyValue; }
            set { _PropertyValue = value; }
        }
        private string _Operator;

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public string Operator
        {
            get { return _Operator; }
            set { _Operator = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCondition"/> class.
        /// </summary>
        public PropertyCondition()
        {
        }

        /// <summary>
        /// Evals the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool Eval(Dictionary<string, object> context)
        {
            // If object does not contain object name return false or true depending on required attribute
            if (!context.ContainsKey(ObjectName))
                return IsRequired;

            object source = context[PropertyName];

            PropertyInfo property = ConditionBase.GetProperty(context, PropertyName);
            switch(Operator)
            {
                case "=":
                    if (property != null && 
                        property.GetValue(source, null) != null && 
                        property.GetValue(source, null).ToString().Equals(PropertyValue))
                        return true;
                    break;
                case "!=":
                    if (property != null &&
                        property.GetValue(source, null) != null &&
                        !property.GetValue(source, null).ToString().Equals(PropertyValue))
                        return true;
                    break;
                case "contains":
                    if (property != null)
                        return true;
                    break;
                case "doesNotContain":
                    if (property == null)
                        return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCondition"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public PropertyCondition(XmlNode node) : base(node)
        {
            _Operator = node.SelectSingleNode("Parameter[@name='operator']").Attributes["value"].Value;
            _PropertyName = node.SelectSingleNode("Parameter[@name='propertyName']").Attributes["value"].Value;
            
            XmlNode valueNode = (node.SelectSingleNode("Parameter[@name='propertyValue']"));
            if (valueNode != null)
                _PropertyValue = valueNode.Attributes["value"].Value;
        }
    }
}
