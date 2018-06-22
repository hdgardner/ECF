using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Reference_Edit : System.Web.UI.UserControl, IEditControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			ibClear.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/delete.gif");
            ibSelect.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/search.gif");
		}

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState"/> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object"/> that represents the user control state to be restored.</param>
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
		}

		#region vldCustom_ServerValidate
        /// <summary>
        /// Handles the ServerValidate event of the vldCustom control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		protected void vldCustom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (!AllowNulls && Value == null)
				args.IsValid = false;
		}
		#endregion

		#region btnRefresh_Click
        /// <summary>
        /// Handles the Click event of the btnRefresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
            Value = (PrimaryKeyId)int.Parse(Request["__EVENTARGUMENT"]);
		}
		#endregion

		#region ibClear_Click
        /// <summary>
        /// Handles the Click event of the ibClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
		protected void ibClear_Click(object sender, ImageClickEventArgs e)
		{
			Value = null;
		}
		#endregion

		#region IEditControl Members

		#region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		public object Value
		{
			set
			{
				if (value != null)
				{
                    int iObjectId = (PrimaryKeyId)value;

					ViewState["ObjectId"] = iObjectId;
					string sReferencedClass = ViewState["ReferencedClass"].ToString();
					

					MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);

					MetaObject obj = new MetaObject(mc, iObjectId);
					lblReference.Text = obj.Properties[mc.TitleFieldName].Value.ToString();
					//        lnkReference.NavigateUrl = String.Format("~/Common/ObjectView.aspx?class={0}&Id={1}", sReferencedClass, iObjectId);
					tdClear.Visible = !ReadOnly;
				}
				else
				{
					ViewState.Remove("ObjectId");
					lblReference.Text = String.Empty;
					//        lnkReference.NavigateUrl = String.Empty;
					tdClear.Visible = false;
				}
			}
			get
			{
				if (ViewState["ObjectId"] != null)
					return (int)ViewState["ObjectId"];
				else
					return null;
			}
		}
		#endregion

		#region ShowLabel
        /// <summary>
        /// Gets or sets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
        /// <summary>
        /// Gets or sets a value indicating whether [allow nulls].
        /// </summary>
        /// <value><c>true</c> if [allow nulls]; otherwise, <c>false</c>.</value>
		public bool AllowNulls
		{
			set
			{
				ViewState["AllowNulls"] = value;
			}
			get
			{
				if (ViewState["AllowNulls"] != null)
					return (bool)ViewState["AllowNulls"];
				else
					return false;
			}
		}
		#endregion

		#region RowCount
        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			set
			{
				tdSelect.Visible = !value;
				vldCustom.Enabled = !value;
				tdClear.Visible = !value;
			}
			get
			{
				return !tdSelect.Visible;
			}
		}
		#endregion

		#region LabelWidth
        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>The width of the label.</value>
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="field">The field.</param>
		public void BindData(MetaField field)
		{
			string sReferencedClass = field.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			ViewState["ReferencedClass"] = sReferencedClass;

            string url = CHelper.GetAbsolutePath(String.Format("/Apps/MetaDataBase/MetaUI/Pages/Public/SelectItem.aspx?class={0}&btn={1}", sReferencedClass, Page.ClientScript.GetPostBackEventReference(btnRefresh, "xxx")));

			ibSelect.OnClientClick = String.Format("OpenWindow(\"{0}\", 640, 480, 1); return false;", url);
		}
		#endregion

		#endregion

		#region IEditControl Members


        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}

		#endregion
	}
}