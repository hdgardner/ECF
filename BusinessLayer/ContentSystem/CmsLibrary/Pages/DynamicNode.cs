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
    /// Summary description for DynamicNode
    /// </summary>
	/// 
	[Serializable]
    public class DynamicNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicNode"/> class.
        /// </summary>
        public DynamicNode()
        {
            
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        /// <returns></returns>
        public NodeStep Open()
        {
            NodeStep _nodeStep = new NodeStep(this.OwnerDocument);
            this.OwnerDocument.CurrentNode = this;
          
            _nodeStep.PrevCurrentNode = this.OwnerDocument.CurrentNode;
            
            return _nodeStep;
        }

            
        


        private string _controlPlaceId;
        private int _controlPlaceIndex;

        private string _factoryUID;
        private string _factoryControlUID;

        private bool _isModified = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        public bool IsModified
        {
            get
            {
                if (this.Controls.IsModified)
                    return true;
                return _isModified;
            }
            set
            {
                _isModified = value;
            }

        }
        /// <summary>
        /// Gets or sets the control place id.
        /// </summary>
        /// <value>The control place id.</value>
        public string ControlPlaceId
        {
            get
            {
                return _controlPlaceId;
            }
            set
            {
                _controlPlaceId = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the control place.
        /// </summary>
        /// <value>The index of the control place.</value>
        public int ControlPlaceIndex
        {
            get
            { 
                return _controlPlaceIndex;
            }
            set
            {
                _controlPlaceIndex = value;
            }
        }


        /// <summary>
        /// Gets or sets the factory UID.
        /// </summary>
        /// <value>The factory UID.</value>
        public string FactoryUID
        {
            get
            {
                return _factoryUID;
            }
            set
            {
                _factoryUID = value;
            }
        }
        /// <summary>
        /// Gets or sets the factory control UID.
        /// </summary>
        /// <value>The factory control UID.</value>
        public string FactoryControlUID
        {
            get
            {
                return _factoryControlUID;
            }
            set
            {
                _factoryControlUID = value;
            }
        }
        
    }
}