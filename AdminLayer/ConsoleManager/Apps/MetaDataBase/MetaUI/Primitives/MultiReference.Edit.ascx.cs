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
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class MultiReference_Edit : System.Web.UI.UserControl, IEditControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            ibClear.ImageUrl = Page.ResolveClientUrl("~/Apps/MetaDataBase/images/delete.gif");
            ibSelect.ImageUrl = Page.ResolveClientUrl("~/Apps/MetaDataBase/images/search.gif");
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
			string param = Request["__EVENTARGUMENT"];

			string[] mas = param.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
			if (mas.Length != 2)
				return;
			try
			{
				MultiReferenceObject mro = new MultiReferenceObject(mas[0], int.Parse(mas[1]));
				Value = mro;
			}
			catch { }
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
				if (value != null && value is MultiReferenceObject && ((MultiReferenceObject)value).HasValue)
				{
					MultiReferenceObject mro = (MultiReferenceObject)value;

					ViewState["ObjectId"] = mro.ObjectId.Value;
					ViewState["Class"] = mro.ActiveReference.Name;

					lblReference.Text = mro.ObjectTitle;
					tdClear.Visible = !ReadOnly;
				}
				else
				{
					ViewState.Remove("ObjectId");
					ViewState.Remove("Class");
					lblReference.Text = String.Empty;
					tdClear.Visible = false;
				}
			}
			get
			{
				if (ViewState["ObjectId"] != null && ViewState["Class"] != null)
					return new MultiReferenceObject(ViewState["Class"].ToString(), (int)ViewState["ObjectId"]);
				else
					return null;
			}
		}
		#endregion

		#region FieldName
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

		#region ShowLabel
        /// <summary>
        /// Gets or sets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public bool ShowLabel
		{
			set {}
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
			get { return String.Empty; }
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
			get { return String.Empty; }
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="field">The field.</param>
		public void BindData(MetaField field)
		{
            string url = CHelper.GetAbsolutePath(String.Format("/Apps/MetaDataBase/MetaUI/Pages/Public/SelectMultiReference.aspx?type={0}&btn={1}", field.GetMetaType().Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, "xxx")));

			ibSelect.OnClientClick = String.Format("OpenWindow(\"{0}\", 640, 480, 1); return false;", url);
		}
		#endregion

		#endregion
	}
}