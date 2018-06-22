using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class ViewTransition
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
        /// Gets the view id.
        /// </summary>
        /// <value>The view id.</value>
        public string ViewId
        {
            get
            {
                return Attributes["ViewId"].ToString();
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return Attributes["name"].ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTransition"/> class.
        /// </summary>
        public ViewTransition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTransition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="view">The view.</param>
        public ViewTransition(string name, string view)
        {
            Attributes["ViewId"] = view;
            Attributes["name"] = name;
        }
    }
}
