using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class ModuleSetting
    {
        private Hashtable _Attributes = new Hashtable();

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Hashtable Attributes
        {
            get { return _Attributes; }
            set { _Attributes = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return Attributes["Name"].ToString();
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return Attributes["Value"].ToString();
            }
        }

        private ModuleSettingCollection _Children = new ModuleSettingCollection();

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        /// <value>The Children.</value>
        public ModuleSettingCollection Children
        {
            get { return _Children; }
            set { _Children = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren
        {
            get
            {
                return _Children.Count == 0 ? false : true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleSetting"/> class.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="settingValue">The setting value.</param>
        public ModuleSetting(string settingName, string settingValue)
        {
            Attributes["Value"] = settingValue;
            Attributes["Name"] = settingName;
        }
    }
}
