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
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class Triggers : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string dialogWidth = "500";
		protected readonly string dialogHeight = "500";

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
			{
				lnkNew.Text = GetGlobalResourceObject("GlobalMetaInfo", "NewItem").ToString();
				lnkNew.NavigateUrl = String.Format("javascript:ShowWizard(\"TriggerEdit.aspx?ClassName={0}&btn={1}\", {2}, {3});", mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), dialogWidth, dialogHeight);

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
        /// <summary>
        /// Checks the visibility.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
		public override bool CheckVisibility(object dataItem)
		{
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region BindGrid
        /// <summary>
        /// Binds the grid.
        /// </summary>
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));

			Trigger[] triggers = TriggerManager.GetTriggers(mc);

			foreach (Trigger trigger in triggers)
			{
				DataRow row = dt.NewRow();
				row["Name"] = trigger.Name;
				row["EditLink"] = String.Format("javascript:ShowWizard(\"TriggerEdit.aspx?ClassName={0}&TriggerName={1}&btn={2}\", {3}, {4});", mc.Name, trigger.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), dialogWidth, dialogHeight);

				dt.Rows.Add(row);
			}
			grdMain.DataSource = dt;
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
			if (e.CommandName == deleteCommand)
			{
				string triggerName = e.CommandArgument.ToString();

				TriggerManager.RemoveTrigger(mc, TriggerManager.GetTrigger(mc, triggerName));
			}

			CHelper.AddToContext("NeedToDataBind", "true");
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
			CHelper.AddToContext("NeedToDataBind", "true");
		}
		#endregion
	}
}