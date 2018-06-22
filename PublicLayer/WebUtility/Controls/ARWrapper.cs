namespace Mediachase.Web.UI
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.ComponentModel;
    using Mediachase.Cms;

    /// <summary>
    /// Summary description for ActiveRegion
    /// </summary>
    ///     
    [ParseChildren(true)]
    [PersistChildren(false)]
    public class ActiveRegion : WebControl, INamingContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveRegion"/> class.
        /// </summary>
        public ActiveRegion()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region property: innerHtml
        private ITemplate _innerHtml;
        /// <summary>
        /// Gets or sets the inner HTML.
        /// </summary>
        /// <value>The inner HTML.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate innerHtml
        {
            get { return _innerHtml; }
            set { _innerHtml = value; }
        }
        #endregion

        #region property: innerText
        private string _innerText;
        /// <summary>
        /// Gets or sets the inner text.
        /// </summary>
        /// <value>The inner text.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual string innerText
        {
            get { return _innerText; }
            set { _innerText = value; }
        }
        #endregion

        #region property: innerContainer
        private string _innerContainer;
        /// <summary>
        /// Gets or sets the inner container.
        /// </summary>
        /// <value>The inner container.</value>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual string innerContainer
        {
            get { return _innerContainer; }
            set { _innerContainer = value; }
        }
        #endregion

        #region property: EditWrapper
        private string editWrapper = "1";
        /// <summary>
        /// Gets or sets the edit wrapper.
        /// </summary>
        /// <value>The edit wrapper.</value>
        public string EditWrapper
        {
            get { return editWrapper; }
            set { editWrapper = value; }
        }
        #endregion

        #region property: AllowEdit
        private string _allowEdit = "False";
        /// <summary>
        /// Gets or sets the allow edit.
        /// </summary>
        /// <value>The allow edit.</value>
        public string AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; }
        }
        #endregion

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            //writer.Write("<div style='position:relative;'> <div style='position:absolute; height: 100%; width: 100%;' id='" + this.ClientID + "' ondblclick=\"this.className=\'NonHighlight\'; Editable(this,'"+Mediachase.Cms.AppStart.GetAbsolutePath("~PopUpEdit.aspx", this.Page.Request.ServerVariables["SERVER_PORT"])+"');\" onmouseover=\'HighlightWrapper(this);\' onmouseout=\"this.className=\'NonHighlight\';\">");
            //this.Page.Response.Write(this.editWrapper);
            if (AllowEdit == "True")
            {
                string editorControl = "~/Structure/Base/Controls/Editors/FCKEditor/PopUpEdit.aspx";
                string newEditorCtrl = ConfigurationManager.AppSettings["HtmlPopupEditor"];
                if (!String.IsNullOrEmpty(newEditorCtrl))
                    editorControl = newEditorCtrl;

                if (this.editWrapper == "1")
                {
                    //GA 30.06.2006
					writer.Write("<div style='position:relative;' id='" + this.ClientID + "'onclick=\"return false;\" ondblclick=\"var _uh=$find('MediachaseCmsUtil'); if(_uh!=null) _uh.EditableFCK(this,'" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(editorControl + "?VersionId=" + CMSContext.Current.VersionId) + "');return false;\"  >");
                }
                if (this.editWrapper == "2")
                {
                    //writer.Write("<div id='" + this.ClientID + "' innerText='" + innerText + "' style='width:100%;position:relative;' onclick=\" Editable(this,'" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(editorControl) + "');\" >");
					writer.Write("<div id='" + this.ClientID + "' innerText='" + innerText + "' style='width:100%;position:relative;' onclick=\" var _uh=$find('MediachaseCmsUtil'); if(_uh!=null) _uh.Editable(this,'" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(editorControl) + "');\" >");
                    innerText = innerText.Replace(" ", "&nbsp;");
                }
                if (_innerText == null || _innerText == string.Empty) base.Render(writer);
                else writer.Write(innerText);


                //if (this.editWrapper == "1")
                writer.Write("</div>");
                //if (this.editWrapper == "2")
                //    writer.Write("</span></div>");

            }
            else
            {
                if (this.editWrapper == "1")
                {
                    //this.Page.Response.Write("IF TUTOCHKI!!!");
                    writer.Write("<div id='" + this.ClientID + "' style='whiteSpace:nowrap; display:inline'  >");
                }
                if (this.editWrapper == "2")
                {
                    writer.Write("<div style='whiteSpace:nowrap; display:inline;' id='" + this.ClientID + "'>");
                    innerText = innerText.Replace(" ", "&nbsp;");
                }
                if (_innerText == null || _innerText == string.Empty) base.Render(writer);
                else writer.Write(innerText);

                writer.Write("</div>");
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (innerHtml != null && innerText == null)
                innerHtml.InstantiateIn(this);
        }

    }
}