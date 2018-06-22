using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Web.UI.Layout;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Layout.Extender
{
	[TargetControlType(typeof(IbnControlPlaceManager))]
	public class WsLayoutExtender : ExtenderControl
	{
		#region DeleteMessage
		/// <summary>
		/// Gets or sets the delete message.
		/// </summary>
		/// <value>The delete message.</value>
		[Bindable(true)]
		public string DeleteMessage
		{
			get
			{
				if (this.ViewState["__DeleteMessage"] != null)
					return this.ViewState["__DeleteMessage"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__DeleteMessage"] = value;
			}
		}
		#endregion

		#region AddTemplateClientId
		/// <summary>
		/// Gets or sets the add template client id.
		/// </summary>
		/// <value>The add template client id.</value>
		private string AddTemplateClientId
		{
			get
			{
				if (this.ViewState["__AddTemplateClientId"] != null)
					return this.ViewState["__AddTemplateClientId"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__AddTemplateClientId"] = value;
			}
		}
		#endregion

		#region PageUid
		/// <summary>
		/// Gets or sets the workspace page uid.
		/// </summary>
		/// <value>The ws page uid.</value>
		public string PageUid
		{
			get
			{
				if (this.ViewState["__PageUid"] != null)
					return this.ViewState["__PageUid"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__PageUid"] = value;
			}
		}
		#endregion

		#region ContainerId
		/// <summary>
		/// Gets or sets the container id.
		/// </summary>
		/// <value>The container id.</value>
		public string ContainerId
		{
			get
			{
				if (this.ViewState["__ContainerId"] != null)
					return this.ViewState["__ContainerId"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__ContainerId"] = value;
			}
		}
		#endregion

		#region PropertyPageCommand
		[Bindable(true)]
		public string PropertyPageCommand
		{
			get
			{
				if (this.ViewState["__PropertyPageCommand"] != null)
					return this.ViewState["__PropertyPageCommand"].ToString();

				return string.Empty;
			}
			set
			{
				this.ViewState["__PropertyPageCommand"] = value;
			}
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			UserControl c = (UserControl)this.Page.LoadControl("~/Apps/Core/Layout/Modules/AddTemplate.ascx");

			c.ID = "ctrlAddTemplate";
			c.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "none");

			if (c != null)
				this.Controls.Add(c);

			this.AddTemplateClientId = c.ClientID;
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.EnsureChildControls();
		}

		#region implementation ExtenderControl
		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor("Ibn.WsLayoutExtender", targetControl.ClientID);

			descriptor.AddProperty("jsonItems", ((IbnControlPlaceManager)targetControl).JsonItems.Replace(" ", string.Empty));
			if (!String.IsNullOrEmpty(this.ContainerId))
				descriptor.AddElementProperty("popupElement", this.ContainerId);

			descriptor.AddProperty("deleteMsg", this.DeleteMessage);
			descriptor.AddProperty("propertyCommand", this.PropertyPageCommand);

			descriptor.AddProperty("addElementContainer", this.AddTemplateClientId);
			descriptor.AddProperty("wsPageUid", this.PageUid);
			descriptor.AddProperty("contextKey", new JavaScriptSerializer().Serialize(new LayoutContextKey(this.PageUid)));

			return new ScriptDescriptor[] { descriptor };
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference = new ScriptReference();

			reference.Path = CommerceHelper.GetAbsolutePath("~/Apps/Core/Layout/Scripts/WsLayoutExtender.js");

			return new ScriptReference[] { reference };
		}
		#endregion
	}
}
