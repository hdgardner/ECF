using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaClassView : System.Web.UI.UserControl
	{
		MetaClass mc = null;

		#region ClassName
		private string _className = "";
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();

			CHelper.AddToContext("ClassName", ClassName);
			//CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(mc.FriendlyName));

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			xmlStruct.InnerDataBind += new XmlFormBuilder.InnerDataBindEventHandler(xmlStruct_InnerDataBind);

			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = ClassName;
				xmlStruct.LayoutType = Mediachase.Ibn.Web.UI.WebControls.LayoutType.MetaClassView;
				xmlStruct.LayoutMode = Mediachase.Ibn.Web.UI.WebControls.LayoutMode.WithTabs;
				xmlStruct.CheckVisibleTab = mc;

				xmlStruct.DataBind();
			}
			BindToolbar();
		}

		#region Page_PreRender
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			object needtodatabind = CHelper.GetFromContext("NeedToDataBind");
			if (needtodatabind != null && needtodatabind.ToString() == "true")
			{
				xmlStruct.CheckVisibleTab = mc;
				xmlStruct.DataBind();
			}

			object rebindPage = CHelper.GetFromContext("RebindPage");
			if (rebindPage != null && rebindPage.ToString() == "true")
			{
				MakeDataBind(this);
			}
		}
		#endregion

		#region Page_PreRenderComplete
        /// <summary>
        /// Handles the PreRenderComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			object needtodatabind = CHelper.GetFromContext("NeedToDataBind");
			if (needtodatabind != null && needtodatabind.ToString() == "true")
				CHelper.RemoveFromContext("NeedToDataBind");
		}
		#endregion

		#region xmlStruct_InnerDataBind
        /// <summary>
        /// Handles the InnerDataBind event of the xmlStruct control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void xmlStruct_InnerDataBind(object sender, EventArgs e)
		{
			MakeDataBind(this);
		}
		#endregion

		#region LoadRequestVariables
        /// <summary>
        /// Loads the request variables.
        /// </summary>
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}
		}
		#endregion

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			string title = "";
			if (mc != null)
			{
				if (mc.IsCard)
					title = "CardCustomization";
				else if (mc.IsBridge)
					title = "BridgeCustomization";
				else
					title = "InfoCustomization";
			}
			secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", title).ToString();

			secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "NewField").ToString(), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaFieldEdit.aspx?class={0}", mc.Name));

            secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "NewLink").ToString(), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaLinkEdit.aspx?class={0}", mc.Name));

			if (mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
			{
                secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/edit.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "Edit").ToString(), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaBridgeEdit.aspx?class={0}&back=view", mc.Name));
			}
			else if (mc.Attributes.ContainsKey(MetaClassAttribute.IsCard))
			{
				secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/edit.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "Edit").ToString(), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx?class={0}&back=view", mc.Name));
			}
			else
			{
				if (mc.SupportsCards)
                    secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/metainfo/card.gif") + "' border='0' align='absmiddle' />&nbsp;" + (String)GetGlobalResourceObject("GlobalMetaInfo", "NewCard"), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx?owner={0}&back=owner", mc.Name));

                secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/edit.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "Edit").ToString(), String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassEdit.aspx?class={0}&back=view", mc.Name));
			}

            secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle'/>&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx");
		}
		#endregion

		#region MakeDataBind
        /// <summary>
        /// Makes the data bind.
        /// </summary>
        /// <param name="_cntrl">The _CNTRL.</param>
		private void MakeDataBind(Control _cntrl)
		{
			foreach (Control c in _cntrl.Controls)
			{
				if (c is MCDataBoundControl)
				{
					((MCDataBoundControl)c).DataItem = mc;
					((MCDataBoundControl)c).DataBind();
					continue;
				}
				else if (c.Controls.Count > 0)
					MakeDataBind(c);
			}
		}
		#endregion
	}
}