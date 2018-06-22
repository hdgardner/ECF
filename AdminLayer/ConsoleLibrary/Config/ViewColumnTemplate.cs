using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class ViewColumnTemplate
    {
        private string _TemplateId = String.Empty;

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        /// <value>The template id.</value>
        public string TemplateId
        {
            get { return (string)Attributes["id"]; }
            set { Attributes["id"] = value; }
        }
        private string _Url = String.Empty;

        /// <summary>
        /// Gets or sets the control URL.
        /// </summary>
        /// <value>The control URL.</value>
        public string ControlUrl
        {
            get { return (string)Attributes["ControlUrl"]; }
            set { Attributes["ControlUrl"] = value; }
        }

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
        /// Initializes a new instance of the <see cref="ViewColumnTemplate"/> class.
        /// </summary>
        public ViewColumnTemplate()
        {
            /*
            this.TemplateId = id;
            this.ControlUrl = url;
             * */
        }
    }
}
