using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class ViewTab
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
        /// Gets or sets the control URL.
        /// </summary>
        /// <value>The control URL.</value>
        public string ControlUrl
        {
            get
            {
                return Attributes["ControlUrl"].ToString();
            }
            set
            {
                Attributes["ControlUrl"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTab"/> class.
        /// </summary>
        /// <param name="controlUrl">The control URL.</param>
        internal ViewTab(string controlUrl)
        {
            ControlUrl = controlUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTab"/> class.
        /// </summary>
        internal ViewTab()
        {
            ControlUrl = String.Empty;
        }
    }
}
