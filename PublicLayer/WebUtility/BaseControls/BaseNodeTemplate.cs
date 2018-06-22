using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace Mediachase.Cms.WebUtility.BaseControls
{
    /// <summary>
    /// Base template used to display catalog nodes.
    /// </summary>
    public class BaseNodeTemplate : BaseStoreUserControl
    {
        #region Private Variables
        string _CatalogName;
        private CatalogNode _CurrentNode = null;
        string _Code;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
        public virtual string CatalogName
        {
            get
            {
                return _CatalogName;
            }
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public virtual string Code
        {
            get
            {
                return _Code;
            }
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public virtual CatalogNode Node
        {
            get
            {
                if (_CurrentNode == null)
                {
                    _CurrentNode = LoadNewNode();
                }

                return _CurrentNode;
            }
        }
        #endregion

        /// <summary>
        /// Loads the new node.
        /// </summary>
        /// <returns></returns>
        protected virtual CatalogNode LoadNewNode()
        {
            return CatalogContext.Current.GetCatalogNode(Code, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull));
        }

        /// <summary>
        /// Loads the context. Implements interface IContextUserControl.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void LoadContext(IDictionary context)
        {
            if (context["Code"] != null)
                _Code = context["Code"].ToString();

            if (context["CatalogName"] != null)
                _CatalogName = context["CatalogName"].ToString();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            RenderHeaders();
            base.OnPreRender(e);
        }

        /// <summary>
        /// Sets the headers.
        /// </summary>
        protected virtual void RenderHeaders()
        {
            if (Node == null)
                return;

            Seo seo = StoreHelper.GetLanguageSeo(Node.SeoInfo);

            if (seo == null && Node.ItemAttributes["DisplayName"] != null && Node.ItemAttributes["DisplayName"].Value != null && Node.ItemAttributes["DisplayName"].Value.Length > 0)
            {
                this.Page.Title = Node.ItemAttributes["DisplayName"].Value[0];
            }
            else
            {
                this.Page.Title = seo.Title;

                if (String.IsNullOrEmpty(seo.Title))
                    this.Page.Title = Node.ItemAttributes["DisplayName"].Value[0];

                HtmlMeta ctrl = Page.FindControl("MetaKeyWord") as HtmlMeta;
                if (ctrl != null)
                {
                    ctrl.Attributes.Add("content", seo.Keywords);
                    ctrl.Attributes.Add("name", "keywords");
                }

                ctrl = Page.FindControl("MetaDescription") as HtmlMeta;
                if (ctrl != null)
                {
                    ctrl.Attributes.Add("content", seo.Description);
                    ctrl.Attributes.Add("name", "description");
                }
            }
        }
    }
}
