using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;



namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Summary description for Node
    /// </summary>
    [Serializable]
    public class Node
    {
        [NonSerialized]
        private System.Web.UI.Control _assignedControl = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            NodeUID = Guid.NewGuid().ToString();
            Controls = new NodeControlSettingsCollection();
        }

        /// <summary>
        /// Gets or sets the assigned control.
        /// </summary>
        /// <value>The assigned control.</value>
        public System.Web.UI.Control AssignedControl
        {
            get { return _assignedControl; }
            set { _assignedControl = value; }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="controlUID">The control UID.</param>
        /// <returns></returns>
        public ControlSettings GetSettings(string controlUID)
        {
            try
            {
                return Controls[controlUID];
            }
            catch
            {
                throw new EvaluateException();
            }
        }

        private string _nodeUID;

        private PageDocument _ownerDocument = null;

        /// <summary>
        /// Sets the owner document.
        /// </summary>
        /// <param name="pDocument">The p document.</param>
        public void SetOwnerDocument(PageDocument pDocument)
        {
            if (pDocument != null)
                _ownerDocument = pDocument;
        
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>The owner document.</value>
        public PageDocument OwnerDocument
        {
            get
            {
                return _ownerDocument;
            }
        
        }
        private NodeControlSettingsCollection _controls;


        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        /// <value>The controls.</value>
        public NodeControlSettingsCollection Controls
        {

            get
            {
                return _controls;
            }
            set
            {

                _controls = value;
            }
        }

        /// <summary>
        /// Gets or sets the node UID.
        /// </summary>
        /// <value>The node UID.</value>
        public string NodeUID
        {
            get
            {
				return _nodeUID;
            }
			set
			{
				_nodeUID = value;
			}
        }

       
    }
}
