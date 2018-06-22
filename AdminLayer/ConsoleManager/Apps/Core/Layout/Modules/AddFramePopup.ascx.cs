using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Layout;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using System.Collections.Specialized;

namespace Mediachase.Ibn.Web.UI.Layout.Modules
{
	public partial class AddFramePopup : BaseUserControl
	{
		internal static readonly string _SaveOptionKey = "SaveOption";
		internal static readonly string _SaveDefaultKey = "default";

		#region prop: PageUid
		/// <summary>
		/// Gets the page id.
		/// </summary>
		/// <value>The page id.</value>
		private string PageUid
		{
			get
			{
				if (Request["PageUid"] == null)
					return string.Empty;

				return Request["PageUid"].ToString();
			}
		}
		#endregion

		#region prop: ColumnId
		/// <summary>
		/// Gets the column id.
		/// </summary>
		/// <value>The column id.</value>
		private string ColumnId
		{
			get
			{
				if (Request["ColumnId"] == null)
					return string.Empty;

				return Request["ColumnId"].ToString();
			}
		}
		#endregion

		#region prop: IsAdminMode
		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin mode.
		/// If true then control work for administration section
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is admin mode; otherwise, <c>false</c>.
		/// </value>
		public bool IsAdminMode
		{
			get
			{
				if (ViewState["_IsAdminMode"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_IsAdminMode"].ToString());
			}
			set
			{
				ViewState["_IsAdminMode"] = value;
			}
		}
		#endregion

		#region prop: ViewMode
		/// <summary>
		/// Gets or sets the view mode.
		/// </summary>
		/// <value>The view mode.</value>
		public string ViewMode
		{
			get
			{
				if (ViewState["_ViewMode"] == null)
					return "0";

				return ViewState["_ViewMode"].ToString();
			}
			set
			{
				ViewState["_ViewMode"] = value;
			}
		}
		#endregion

		#region prop: IsAdmin

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (ViewState["_IsAdmin"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		}
		#endregion

		#region Page_Load
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadContext();

			if (!this.IsPostBack)
				DataBind();

			btnSaveAndClose.ServerClick += new EventHandler(btnSaveAndClose_ServerClick);
			btnDefault.ServerClick += new EventHandler(btnDefault_ServerClick);
			btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);

			if (Request["closeFramePopup"] != null)
			    btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}} return false;", Request["closeFramePopup"]));
		}
		#endregion

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();

			// Call tabs load context
			ViewControl.LoadContext(dic);
			
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			ApplyLocalization();
		}

		/// <summary>
		/// Handles the ServerClick event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnCancel_ServerClick(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters("cmdCoreLayoutAddControl");
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		/// <summary>
		/// Handles the ServerClick event of the btnDefault control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnDefault_ServerClick(object sender, EventArgs e)
		{
			IDictionary dic = new ListDictionary();
			dic.Add(_SaveOptionKey, _SaveDefaultKey);
			ViewControl.SaveChanges(dic);

			CommandParameters cp = new CommandParameters("cmdCoreLayoutAddControl");
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		/// <summary>
		/// Handles the ServerClick event of the btnSaveAndClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnSaveAndClose_ServerClick(object sender, EventArgs e)
		{
			ViewControl.SaveChanges(null);

			CommandParameters cp = new CommandParameters("cmdCoreLayoutAddControl");
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

		#region ApplyLocalization
		/// <summary>
		/// Applies the localization.
		/// </summary>
		private void ApplyLocalization()
		{
			btnDefault.Text = "Set to default";
			btnDefault.CustomImage = this.ResolveUrl("~/Apps/Core/Images/layout_refresh.gif");
			if (!IsPostBack)
			    btnDefault.Style.Add(HtmlTextWriterStyle.Display, "inline");

			btnSaveAndClose.Text = "Add and close";
			if (this.ViewMode == "1")
			    btnSaveAndClose.Text = "Save and close";

			btnSaveAndClose.CustomImage = this.ResolveUrl("~/Apps/Core/Images/layout_saveclose.gif");
			if (!IsPostBack)
			    btnSaveAndClose.Style.Add(HtmlTextWriterStyle.Display, "inline");

			btnCancel.Text = "Cancel";
			btnCancel.CustomImage = this.ResolveUrl("~/Apps/Core/Images/layout_deny.gif");
			if (!IsPostBack)
			    btnCancel.Style.Add(HtmlTextWriterStyle.Display, "inline");

			btnDefault.Attributes.Add("onclick", String.Format("if (!confirm('{0}')) return false;", "Are you sure you want to reset the settings?"));
		}
		#endregion
	}
}
