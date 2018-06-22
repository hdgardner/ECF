using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using AjaxControlToolkit;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Core.Controls
{
	public partial class DynamicEditViewControl : CoreBaseUserControl, IAdminContextControl
	{
		private readonly string _DefaultTabContainerId = "DefaultTabContainer";

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

		protected void Page_Load(object sender, EventArgs e)
		{
			string scriptKey = "tabsScriptKey";
			StringBuilder sbScript = new StringBuilder();

			TabContainer tc = this.phTabs.FindControl(_DefaultTabContainerId) as TabContainer;

			if (tc != null)
			{
				sbScript.AppendFormat("function SaveTabState_{0}(){1}", tc.ID, "{");
				sbScript.AppendFormat("var objTab = $get('{0}');", hfTabId.ClientID);
				sbScript.AppendFormat("var objTabContainer = $find('{0}');", tc.ClientID);
				//sbScript.Append("alert('objTabContainer =' + objTabContainer);");
				//sbScript.Append("alert('objTab =' + objTab);");
				sbScript.Append("if(objTab != null && objTabContainer!=null) {");
				sbScript.Append("	var index = objTabContainer.get_activeTabIndex();");
				sbScript.Append("objTab.value = index;");
				sbScript.Append("   }");
				sbScript.Append("}");

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), scriptKey, sbScript.ToString(), true);
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

			phTabs.Controls.Clear();

			AjaxControlToolkit.TabContainer tc = new AjaxControlToolkit.TabContainer();

			tc.ID = _DefaultTabContainerId;
			tc.CssClass = "gray";
			tc.OnClientActiveTabChanged = "SaveTabState_" + tc.ID;

			tc.Tabs.Clear();

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
				tc.Tabs.Add(tabPanel);
			}

			phTabs.Controls.Add(tc);

			if (tc.Tabs.Count > 0)
				tc.ActiveTabIndex = 0;
		}

		#region IAdminContextControl Members
		public void LoadContext(IDictionary context)
		{
			EnsureChildControls();

			TabContainer tc = this.phTabs.FindControl(_DefaultTabContainerId) as TabContainer;

			if (tc != null)
			{
				// Call save method on all the forms loaded
				foreach (TabPanel view in tc.Tabs)
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
		}
		#endregion

		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			// Before saving make sure controls are created
			this.EnsureChildControls();

			TabContainer tc = this.phTabs.FindControl(_DefaultTabContainerId) as TabContainer;

			if (tc != null)
			{
				// Call save method on all the forms loaded
				foreach (TabPanel view in tc.Tabs)
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

			TabContainer tc = this.phTabs.FindControl(_DefaultTabContainerId) as TabContainer;

			if (tc != null)
			{
				// Call save method on all the forms loaded
				foreach (TabPanel view in tc.Tabs)
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
			}

			// Might need to add support for redirects here
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