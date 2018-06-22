using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using AjaxControlToolkit;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Core.Controls
{
    /// <summary>
    /// This class loads the sub controls into the tabs.
    /// </summary>
    public partial class EditViewControl : CoreBaseUserControl, IAdminContextControl
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
				//return ManagementHelper.GetValueFromQueryString("_v", String.Empty);
            }
            set
            {
                _ViewId = value;
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                CreateTabs();                
                base.CreateChildControls();
                this.ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            Page.InitComplete += new EventHandler(Page_InitComplete);
        }

        /// <summary>
        /// Handles the InitComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_InitComplete(object sender, EventArgs e)
        {
            this.EnsureChildControls();
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
			EnsureChildControls();
			// Call save method on all the forms loaded
			foreach (TabPanel view in DefaultTabContainer.Tabs)
			{
				// Determine if there are controls in the current view
				if (view.Controls.Count > 0)
				{
					// We are only ineterested in the root control
					Control ctrl = view.Controls[0];

					// Make sure control is of type that has save changes method
					if (ctrl is IAdminContextControl)
					{
						// Persist changes
						((IAdminContextControl)ctrl).LoadContext(context);
					}
				}
			}
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			// Before saving make sure controls are created
			this.EnsureChildControls();

			// Call save method on all the forms loaded
			foreach (TabPanel view in DefaultTabContainer.Tabs)
			{
				// Determine if there are controls in the current view
				if (view.Controls.Count > 0)
				{
					// We are only ineterested in the root control
					Control ctrl = view.Controls[0];

					// Make sure control is of type that has save changes method
					if (ctrl is IAdminTabControl)
					{
						// Persist changes
						((IAdminTabControl)ctrl).SaveChanges(context);
					}
				}
			}

            // Might need to add support for redirects here
        }

        /// <summary>
        /// Commits the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void CommitChanges(IDictionary context)
        {
			// Before saving make sure controls are created
			this.EnsureChildControls();

			// Call save method on all the forms loaded
			foreach (TabPanel view in DefaultTabContainer.Tabs)
			{
				// Determine if there are controls in the current view
				if (view.Controls.Count > 0)
				{
					// We are only ineterested in the root control
					Control ctrl = view.Controls[0];

					// Make sure control is of type that has save changes method
					if (ctrl is IPreCommit)
					{
						// Persist changes
						((IPreCommit)ctrl).PreCommitChanges(context);
					}
				}
			}

            // Might need to add support for redirects here
        }

        /// <summary>
        /// Restores control-state information from a previous page request that was saved by the <see cref="M:System.Web.UI.Control.SaveControlState"/> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object"/> that represents the control state to be restored.</param>
		protected override void LoadControlState(object savedState)
		{
			EnsureChildControls();
			base.LoadControlState(savedState);
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            this.EnsureChildControls();
            base.DataBind();
        }

        /// <summary>
        /// Creates the tabs.
        /// </summary>
        private void CreateTabs()
        {
            AdminView view = ManagementContext.Current.FindView(this.AppId, this.ViewId);

            // Check view permissions
            if (view.Attributes.Contains("permissions"))
            {
                if (!ProfileContext.Current.CheckPermission(view.Attributes["permissions"].ToString()))
                    throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            }
            
            if (view == null)
                return;

            DefaultTabContainer.Tabs.Clear();
            
            foreach (ViewTab node in view.Tabs)
            {
                // Check if the node is visible
                if (!IsVisible(node))
                    continue;

				TabPanel tabPanel = new TabPanel();
				if (node.Attributes["Name"] != null)
                    tabPanel.HeaderText = UtilHelper.GetResFileString(node.Attributes["Name"].ToString());
                
                // Load custom control
                string controlUrl = String.Format("~/Apps/{0}", node.ControlUrl);
                if (File.Exists(Server.MapPath(controlUrl)))
                {
                    Control ctrl = this.LoadControl(controlUrl);
                    tabPanel.Controls.Add(ctrl);
                }
                else
                {
                    throw new FileNotFoundException(Server.MapPath(controlUrl));
                }

                // Add tabs
				DefaultTabContainer.Tabs.Add(tabPanel);
            }

            if (DefaultTabContainer.Tabs.Count > 0)
                DefaultTabContainer.ActiveTabIndex = 0;
        }

        /// <summary>
        /// Determines whether the specified node is visible.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node is visible; otherwise, <c>false</c>.
        /// </returns>
        private bool IsVisible(ViewTab node)
        {
            // TODO: implement personalization here
            return true;
        }
		
        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <returns></returns>
        private object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsEnum)
                return Enum.Parse(conversionType, value.ToString(), true);
            else
                return Convert.ChangeType(value, conversionType);
        }
    }
}