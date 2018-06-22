using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities.Rules;
using System.IO;
using System.Xml.Serialization;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using System.Windows.Forms;

namespace ExpressionEditor.BO
{
    public class RuleSetObject
    {
        #region Properties

        private Type _activity;
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public Type Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
            }
        }

        private string _assemblyPath;
        /// <summary>
        /// Gets or sets the assembly path.
        /// </summary>
        /// <value>The assembly path.</value>
        public string AssemblyPath
        {
            get
            {
                return _assemblyPath;
            }
            set
            {
                _assemblyPath = value;
            }
        }

        private string _name;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (this.ActivityRuleSet != null)
                        this.ActivityRuleSet.Name = _name;
            }
        }

        private RuleSet _activityRuleSet;
        /// <summary>
        /// Gets or sets the activity rule set.
        /// </summary>
        /// <value>The activity rule set.</value>
        public RuleSet ActivityRuleSet
        {
            get
            {
                if (_activityRuleSet == null)
                    _activityRuleSet = DeserializeRuleSet(_ruleSetDefinition);
                
                return _activityRuleSet;
            }
            set
            {
                _activityRuleSet = value;
                _name = _activityRuleSet.Name;
            }
        }

        private string _ruleSetDefinition;
        /// <summary>
        /// Gets or sets the rule set definition.
        /// </summary>
        /// <value>The rule set definition.</value>
        public string RuleSetDefinition
        {
            get
            {
                return _ruleSetDefinition;
            }
            set
            {
                _ruleSetDefinition = value;
            }
        }
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the RuleSetObject class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="ruleSetDefinition">The rule set definition.</param>
        public RuleSetObject(Type activity, string ruleSetDefinition)
        {
            _activity = activity;
            _assemblyPath = _activity.Assembly.Location;
            _ruleSetDefinition = ruleSetDefinition;
        }

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

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", _name, _activity.AssemblyQualifiedName);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public RuleSetObject Clone()
        {
            return new RuleSetObject(_activity, _ruleSetDefinition);
        }
    }
}
