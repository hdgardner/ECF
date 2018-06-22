using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using ComponentArt.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Core.Controls
{
    public partial class DialogControl : CoreBaseUserControl, IAdminContextControl
    {
        string _AppId = String.Empty;
        /// <summary>
        /// Gets or sets the app id.
        /// </summary>
        /// <value>The app id.</value>
        public string AppId
        {
            get
            {
                return _AppId;
            }
            set
            {
                _AppId = value;
            }
        }

        string _ViewId = String.Empty;
        /// <summary>
        /// Gets or sets the view id.
        /// </summary>
        /// <value>The view id.</value>
        public string ViewId
        {
            get
            {
                return _ViewId;
            }
            set
            {
                _ViewId = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public Unit Width
        {
            get
            {
                return DialogCtrl.Width;
            }
            set
            {
                DialogCtrl.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public Unit Height
        {
            get
            {
                return DialogCtrl.Height;
            }
            set
            {
                DialogCtrl.Height = value;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
				EnsureChildControls();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			DialogCtrl.ID = this.ID;

			base.OnInit(e);
		}

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                CreateControls();
                base.CreateChildControls();
                this.ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        private void CreateControls()
        {
            AdminView view = ManagementContext.Current.FindView(this.AppId, this.ViewId);

            if (view == null)
                return;

            // find control does not work for some unknown reason
            //Panel panel = (Panel)DialogCtrl.Content.FindControl("DialogContentPanel");

            Panel panel = ContentPanel;
            panel.Controls.Clear();

            // Load custom control
            string controlUrl = String.Format("~/Apps/{0}", view.ControlUrl);
            if (File.Exists(Server.MapPath(controlUrl)))
            {
                Control ctrl = this.LoadControl(controlUrl);
                panel.Controls.Add(ctrl);
            }
            else
            {
                throw new FileNotFoundException(Server.MapPath(controlUrl));
            }
        }

        /// <summary>
        /// Gets the content panel.
        /// </summary>
        /// <value>The content panel.</value>
        Panel ContentPanel
        {
            get
            {
                Panel panel = null;

                foreach (Control ctrl in DialogCtrl.Content.Controls)
                {
                    if (ctrl == null || String.IsNullOrEmpty(ctrl.ID))
                        continue;

                    if (ctrl.ID.Equals("DialogContentPanel"))
                    {
                        panel = (Panel)ctrl;
                        break;
                    }
                }

                return panel;
            }
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            EnsureChildControls();
            // Call save method on all the forms loaded
            Panel panel = ContentPanel;
            foreach (Control ctrl in panel.Controls)
            {
                // Make sure control is of type that has save changes method
                if (ctrl is IAdminContextControl)
                {
                    // Persist changes
                    ((IAdminContextControl)ctrl).LoadContext(context);
                }
            }
        }
    }
}