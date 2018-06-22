using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Core.Controls
{
    public partial class HtmlEditControl : CoreBaseUserControl
    {
        private ITextEditorControl _EditorControl = null;

        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <value>The width.</value>
        public Unit Width
        {
            set
            {
                EnsureChildControls();
                _EditorControl.Width = value;
            }
        }

        /// <summary>
        /// Sets the height.
        /// </summary>
        /// <value>The height.</value>
        public Unit Height
        {
            set
            {
                EnsureChildControls();
                _EditorControl.Height = value;
            }
        }

        private bool _IsRequired = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                _IsRequired = value;
            }
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        private string BaseUrl
        {
            get
            {
                string url = Request.Url.AbsoluteUri;
                int index = url.IndexOf(Request.ApplicationPath);

                string replaceUrl = url.Substring(0, index + Request.ApplicationPath.Length + 1);
                return replaceUrl;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            set
            {
                EnsureChildControls();
                _EditorControl.Text = value;
            }
            get
            {
                EnsureChildControls();
                return _EditorControl.Text;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            EnsureChildControls();
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            EnsureChildControls();
            base.DataBind();
        }

        /// <summary>
        /// Determines whether the server control contains child controls. If it does not, it creates child controls.
        /// </summary>
        protected override void EnsureChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                CreateChildControls2();
                base.EnsureChildControls();
                ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Creates the child controls2.
        /// </summary>
        private void CreateChildControls2()
        {
            string editorControl = "Editors/Simple/EditorControl.ascx";
            string newEditorCtrl = ConfigurationManager.AppSettings["HtmlEditorControl"];
            if (!String.IsNullOrEmpty(newEditorCtrl))
                editorControl = newEditorCtrl;

            UserControl ctrl = (UserControl)this.LoadControl(editorControl);
            EditorControl.Controls.Add(ctrl);
            _EditorControl = (ITextEditorControl)ctrl;
        }
    }
}