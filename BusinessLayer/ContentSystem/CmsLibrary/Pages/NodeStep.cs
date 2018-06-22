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
    /// Summary description for NodeStep
    /// </summary>
    [Serializable]
    public class NodeStep : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeStep"/> class.
        /// </summary>
        /// <param name="_pageDocument">The _page document.</param>
        public NodeStep(PageDocument _pageDocument)
        {
            _ownerPageStorage = _pageDocument;
        }

        private PageDocument _ownerPageStorage=null;
        
        private Node _prevCurrentNode=null;

        /// <summary>
        /// Gets or sets the prev current node.
        /// </summary>
        /// <value>The prev current node.</value>
        public Node PrevCurrentNode
        {
            get 
            {
                return _prevCurrentNode;
            }
            set
            {
                _prevCurrentNode = value;
            }
        
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if ((_ownerPageStorage.CurrentNode != null) && (_prevCurrentNode != null))
                _ownerPageStorage.CurrentNode = _prevCurrentNode;
        }

        #endregion
    }
}