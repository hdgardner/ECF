using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Mediachase.Commerce.Marketing.Validators
{
    public class ConditionBase
    {
        private string _ObjectName;

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        public string ObjectName
        {
            get { return _ObjectName; }
            set { _ObjectName = value; }
        }

        private bool _IsRequired;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired
        {
            get { return _IsRequired; }
            set { _IsRequired = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionBase"/> class.
        /// </summary>
        public ConditionBase()
        {
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propName">Name of the prop.</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(object source, string propName)
        {
            PropertyInfo property = source.GetType().GetProperty(propName);
            if (property != null)
            {
                return property;
            }
            else if (propName.Contains("."))
            {
                string[] keys = propName.Split(new char[] { '.' });
                object obj = source;

                for (int index = 0; index < keys.Length; index++)
                {
                    string newkey = keys[index];
                    if (property == null)
                    {
                        property = source.GetType().GetProperty(newkey);
                        obj = property.GetValue(obj, null);
                    }
                    else
                    {
                        if (index < keys.Length - 1)
                            obj = property.GetValue(obj, null);

                        property = property.PropertyType.GetProperty(newkey);
                    }

                    if (property == null)
                        return null;
                }

                if (property != null && obj != null)
                {
                    return property;
                }
            }

            return null;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionBase"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public ConditionBase(XmlNode node)
        {
            _ObjectName = node.Attributes["name"].Value;
            _IsRequired = Boolean.Parse(node.Attributes["required"].Value);
        }
    }
}
