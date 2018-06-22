using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;

namespace Mediachase.Commerce
{
    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public class WorkflowConfiguration : ConfigurationSection
    {
        private static volatile WorkflowConfiguration _instance;
        private static object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowConfiguration"/> class.
        /// </summary>
        private WorkflowConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static WorkflowConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (WorkflowConfiguration)ConfigurationManager.GetSection("CommerceFramework/Workflow");
                        }

                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Collection of <see cref="WorkflowDefinition"/> instances defined in the config file
        /// </summary>
        /// <value>The workflows.</value>
        [ConfigurationProperty("Workflows", IsRequired=true)]
        public WorkflowCollection Workflows
        {
            get
            {
                return Instance["Workflows"] as WorkflowCollection;
            }
        }

        /// <summary>
        /// Returns class names mapped to the event key (name)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public WorkflowDefinition GetWorkflow(string key)
        {
            if (Instance.Workflows != null)
            {
                for (int i = 0; i < Workflows.Count; i++)
                {
                    if (Workflows[i].Name == key)
                    {
                        return Workflows[i];
                    }
                }

                //Workflow not found, return null
                return null;
            }
            //If no workflows have been defined, return null
            return null;
        }

        /// <summary>
        /// Returns entire collection of WorkflowDefinition instances as an array
        /// </summary>
        /// <returns></returns>
        public WorkflowDefinition[] GetWorkflowDefinitionArray()
        {
            List<WorkflowDefinition> list = new List<WorkflowDefinition>();
            foreach (WorkflowDefinition workflow in Workflows)
            {
                list.Add(workflow);
            }

            return list.ToArray();
        }
    }

    /// <summary>
    /// Implements the operations for the work flow collection. (Inherits <see cref="ConfigurationElementCollection"/>.)
    /// </summary>
    public class WorkflowCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Commerce.WorkflowDefinition"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WorkflowDefinition this[int index]
        {
            get
            {
                return base.BaseGet(index) as WorkflowDefinition;
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
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WorkflowDefinition();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WorkflowDefinition)element).Name;
        } 
    }

    //TODO: Implement StringValidator validation on Name and ClassName properties
    /// <summary>
    /// Implements operations for the work flow definition. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class WorkflowDefinition : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowDefinition"/> class.
        /// </summary>
        public WorkflowDefinition() { }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the display.
        /// </summary>
        /// <value>The name of the display.</value>
        [ConfigurationProperty("displayname", IsRequired = true)]
        public string DisplayName
        {
            get
            {
                return (string)this["displayname"];
            }
            set
            {
                this["displayname"] = value;
            }
        }

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

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [ConfigurationProperty("xomlpath", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)this["xomlpath"];
            }
            set
            {
                this["xomlpath"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        /// <value>The rules.</value>
        [ConfigurationProperty("rulespath", IsRequired = true)]
        public string Rules
        {
            get
            {
                return (string)this["rulespath"];
            }
            set
            {
                this["rulespath"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [ConfigurationProperty("description", IsRequired=true)]
        public string Description
        {
            get
            {
                return (string)this["description"];
            }
            set
            {
                this["description"] = value;
            }
        }

        //make the configuration section editable by overriding this ConfigurationElement property
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
