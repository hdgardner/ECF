using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI
{
	public partial class SelectControl : System.Web.UI.UserControl
	{
		#region _className
        /// <summary>
        /// Gets or sets the name of the _class.
        /// </summary>
        /// <value>The name of the _class.</value>
		private string _className
		{
			get { return (ViewState["_className"] == null ? "" : ViewState["_className"].ToString()); }
			set { ViewState["_className"] = value; }
		} 
		#endregion

		#region SelectPopupID
        /// <summary>
        /// Gets or sets the select popup ID.
        /// </summary>
        /// <value>The select popup ID.</value>
		public string SelectPopupID
		{
			get { return (ViewState["_selectPopupID"] == null ? "" : ViewState["_selectPopupID"].ToString()); }
			set { ViewState["_selectPopupID"] = value; }
		} 
		#endregion

		#region ObjectId
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        /// <value>The object id.</value>
		public int ObjectId
		{
			get { return (ViewState["_objectId"] == null ? -1 : (int)ViewState["_objectId"]); }
			set { ViewState["_objectId"] = value; hfValue.Value = value.ToString(); }
		} 
		#endregion

		#region Width
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
		public string Width
		{
			get { return (ViewState["__width"] == null ? "250px" : ViewState["__width"].ToString()); }
			set { ViewState["__width"] = value; }
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(SelectPopupID))
				throw new Exception("SelectPopupID is required!");
			
			#region ----cut---
			//MCActionManager am = MCActionManager.GetCurrent(this.Page);
			//if (am != null)
			//{
			//    string query = am.AddAction("MC_SelectControl_Select", new string[] { ClassName, hfValue.ClientID });
			//    lblSearch.Text = String.Format("<a href='#' onclick=\"{1}\"><img alt='' align='absmiddle' src='{0}' /></a>",
			//        CommonHelper.GetAbsolutePath("/images/search.gif"), query);
			//} 
			#endregion

			lblSearch.Text = String.Format("<img alt='' align='absmiddle' src='{0}' style='cursor:pointer;' />",
				CHelper.GetAbsolutePath("/images/search.gif"));

			string sQuery = String.Format("javascript:MC_SELECT_POPUPS['{0}'].openSelectPopup(event, '{1}');",
					GetPopupClientID(this.Page), hfValue.ClientID);

			divSearch.Attributes.Add("onclick", sQuery);
			txtName.Attributes.Add("onclick", sQuery);
			txtName.Style.Add("width", Width);
		}

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				if (Request["__EVENTTARGET"] == hfValue.ClientID)
				{
					try
					{
						ObjectId = int.Parse(Request["__EVENTARGUMENT"]);
					}
					catch { }
				}
				if (Request["__EVENTTARGET"] == hfClear.ClientID)
				{
					try
					{
						ObjectId = -1;
					}
					catch { }
				}
			}
			BindObject();
		}

		#region BindObject
        /// <summary>
        /// Binds the object.
        /// </summary>
		private void BindObject()
		{
			if (!String.IsNullOrEmpty(_className) && ObjectId > 0)
			{
				BusinessObject bo = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName(_className), ObjectId);
				txtName.InnerHtml = String.Format("<div style='padding:2px;'><span style='float:left;'>{0}</span><span style='float:right;cursor:pointer;padding-right:4px;' onclick=\"{2}\"><img alt='' align='absmiddle' src='{1}' style='cursor:pointer;' /></span></div>",
						bo.Properties[bo.GetMetaType().TitleFieldName].Value.ToString(),
						CHelper.GetAbsolutePath("/images/delete.gif"),
						String.Format("CancelBubble_SelectPopups(event);__doPostBack('{0}', '')", hfClear.ClientID));
			}
			else
				txtName.InnerHtml = String.Format("<div style='padding:2px;'>{0}</div>", "Not Set");
		} 
		#endregion

		#region GetPopupClientID
        /// <summary>
        /// Gets the popup client ID.
        /// </summary>
        /// <param name="_page">The _page.</param>
        /// <returns></returns>
		private string GetPopupClientID(Page _page)
		{
			SelectPopup sp = null;
			sp = GetSelectPopupFromCollection(_page.Controls);
			if (sp != null)
			{
				_className = sp.ClassName;
				return sp.ClientID;
			}
			return "";
		}

        /// <summary>
        /// Gets the select popup from collection.
        /// </summary>
        /// <param name="coll">The coll.</param>
        /// <returns></returns>
		private SelectPopup GetSelectPopupFromCollection(ControlCollection coll)
		{
			SelectPopup retVal = null;
			foreach (Control c in coll)
			{
				if (c is SelectPopup && c.ID == SelectPopupID)
				{
					retVal = (SelectPopup)c;
					break;

				}
				else
				{
					retVal = GetSelectPopupFromCollection(c.Controls);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion
	}
}