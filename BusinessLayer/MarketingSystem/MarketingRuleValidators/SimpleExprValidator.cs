using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mediachase.Commerce.Marketing.Validators
{
    public class SimpleExprValidator : IExpressionValidator
    {
        /*
        Dictionary<string, object> _Dictionary = null;
        public Dictionary<string, object> Dictionary
        {
            get
            {
                return _Dictionary;
            }
        }
         * */

        /// <summary>
        /// Validates the xml string and compares it to existing profile.
        /// The xml string must have the following format:
        /// <![CDATA[
        /// <Profile>
        /// <propertyCondition name="Profile" required="true|false">
        /// <Parameter name="operator" value="=|!=|contains|doesNotContain"/>
        /// <Parameter name="propertyName" value="propertyName"/>
        /// <Parameter name="propertyValue" value="propertyValue"/>
        /// </propertyCondition>
        /// </Profile>
        /// ]]>
        /// Condition.Name should correspond to the object in the context dictionary with a same name.
        /// </summary>
        /// <param name="key">The key. Must be a unique key identifying the current expression. It might be used for caching purpose by the engine.</param>
        /// <param name="expr">The expression that needs to be evaluated.</param>
        /// <param name="context">The context, which consists of object that will be accessible during expression evaluation.</param>
        /// <returns></returns>
        public virtual ValidationResult Eval(string key, string expr, IDictionary<string, object> context)
        {
            bool isValid = true;
            string error = String.Empty;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(expr);

            //doc
            XmlNodeList propertyConditionList = doc.SelectNodes("//Profile/propertyCondition");
            if (propertyConditionList.Count > 0)
            {
                foreach (XmlNode condition in propertyConditionList)
                {
                    // Immediately terminate if condition is not satisfied
                    if (!EvalPropertyCondition(condition))
                        return new ValidationResult(false, String.Empty);
                }
            }

            doc = null;

            return new ValidationResult(isValid, error);
        }

        /// <summary>
        /// Evals the property condition.
        /// </summary>
        /// <param name="propertyCondition">The property condition.</param>
        /// <returns></returns>
        private bool EvalPropertyCondition(XmlNode propertyCondition)
        {
            PropertyCondition condition = new PropertyCondition(propertyCondition);
            return condition.Eval(MarketingContext.Current.MarketingProfileContext);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleExprValidator"/> class.
        /// </summary>
        public SimpleExprValidator()
        {
            //_Dictionary = dictionary;
        }
    }
}
