using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassFields : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string sort = "MetaClassView_Sort";

		#region DataItem
        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
					mc = (MetaClass)value;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc = null;
        /// <summary>
        /// Gets or sets the mc.
        /// </summary>
        /// <value>The mc.</value>
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[className] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[className].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[className] = value.Name;
				_mc = value;
			}
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			if (mc != null)
				BindData();
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("TypeName", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));
			dt.Columns.Add("FieldTypeImageUrl", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));

			foreach (MetaField field in mc.Fields)
			{
				DataRow row = dt.NewRow();
				row["Name"] = field.Name;
				row["FriendlyName"] = CHelper.GetResFileString(field.FriendlyName);
				row["TypeName"] = CHelper.GetResFileString(field.GetOriginalMetaType().FriendlyName);
				row["IsSystem"] = field.Attributes.ContainsKey(MetaClassAttribute.IsSystem) || mc.TitleFieldName == field.Name;
				if(field.IsLink)
					row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaLinkEdit.aspx?class={0}&field={1}", mc.Name, field.Name);
				else
                    row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaFieldEdit.aspx?class={0}&field={1}", mc.Name, field.Name);
				string postfix = string.Empty;
				if ((bool)row["IsSystem"])
				{
					postfix = "_sys";
				}
				if (field.TypeName == "ReferencedField")
				{
					row["FieldTypeImageUrl"] = "../../images/metainfo/referencedfield" + postfix + ".gif";
					row["TypeName"] = String.Format(CultureInfo.InvariantCulture,
						"{0} ({1})",
						CHelper.GetResFileString(field.GetMetaType().FriendlyName),
						CHelper.GetResFileString(field.GetOriginalMetaType().FriendlyName));
				}
				else if (field.TypeName == "Reference")
				{
                    row["FieldTypeImageUrl"] = "../../images/metainfo/reference" + postfix + ".gif";
				}
				else if (field.TypeName == "BackReference")
				{
					row["FieldTypeImageUrl"] = "../../images/metainfo/backreference" + postfix + ".gif";
				}
				else
				{
					row["FieldTypeImageUrl"] = "../../images/metainfo/metafield" + postfix + ".gif";
				}
				dt.Rows.Add(row);
			}

			DataView dv = dt.DefaultView;
			if (Session["MetaClassView_Sort"] == null)
				Session["MetaClassView_Sort"] = "Name";

			dv.Sort = Session["MetaClassView_Sort"].ToString();

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("GlobalMetaInfo", "Delete").ToString() + "?')");
			}
		}
		#endregion

		#region grdMain_RowCommand
        /// <summary>
        /// Handles the RowCommand event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == deleteCommand && mc != null)
			{
				MetaField mf = MetaDataWrapper.GetMetaFieldByName(mc, e.CommandArgument.ToString());

				//if (mf.GetMetaType().McDataType == Mediachase.Ibn.Data.Meta.Management.McDataType.Reference
				//    && mf.Attributes.ContainsKey(McDataTypeAttribute.ReferenceUseSecurity)
				//    && BusinessObjectServiceManager.IsServiceInstalled(mc, SecurityService.ServiceName)
				//    && BusinessObjectServiceManager.IsServiceInstalled(MetaDataWrapper.GetAttributeValue(mf, McDataTypeAttribute.ReferenceMetaClassName).ToString(), SecurityService.ServiceName)
				//    )
				//    Mediachase.Ibn.Data.Services.Security.RemoveObjectRolesFromReference(mf);

				mc.DeleteMetaField(mf);
				CHelper.RequireDataBind();
			}
		}
		#endregion

		#region grdMain_Sorting
        /// <summary>
        /// Handles the Sorting event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (Session[sort].ToString() == e.SortExpression)
				Session[sort] += " DESC";
			else
				Session[sort] = e.SortExpression;
			BindData();
		}
		#endregion
	}
}