using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;

namespace Mediachase.Commerce.Engine.Events
{
    /// <summary>
    /// Implements operations for the event collection. (Inherits <see cref="ConfigurationElementCollection"/>.)
    /// </summary>
    public class EventCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Commerce.Engine.Events.EventDefinition"/> at the specified index.
        /// </summary>
        /// <value></value>
        public EventDefinition this[int index]
        {
            get
            {
                return base.BaseGet(index) as EventDefinition;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new EventDefinition();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EventDefinition)element).ClassName;
        } 
    }

    //TODO: Implement StringValidator validation on Name and ClassName properties
    /// <summary>
    /// Represents the event definition. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class EventDefinition : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDefinition"/> class.
        /// </summary>
        public EventDefinition() { }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string ClassName
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
    }
}
