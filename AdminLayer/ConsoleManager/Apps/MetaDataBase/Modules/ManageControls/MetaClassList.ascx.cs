using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaClassList : System.Web.UI.UserControl
	{
		#region Filter
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
		public string Filter
		{
			get
			{
				if (this.Session["MetaClassList_Filter"] != null)
					return this.Session["MetaClassList_Filter"].ToString();
				else return "All";
			}
			set
			{
				this.Session["MetaClassList_Filter"] = value;
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
			if (!IsPostBack)
			{
				BindFilter();
				BindData();
			}

			BindToolbar();
		}

		#region BindFilter
        /// <summary>
        /// Binds the filter.
        /// </summary>
		protected void BindFilter()
		{
			ddlFilter.Items.Clear();
			ddlFilter.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "All").ToString(), "All"));
			ddlFilter.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Info").ToString(), "Info"));
			ddlFilter.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Bridge").ToString(), "Bridge"));
			ddlFilter.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Card").ToString(), "Card"));
			ddlFilter.SelectedValue = Filter;
		}
		#endregion

		#region Page_Init
        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.grdMain.ItemCommand += new DataGridCommandEventHandler(grdMain_ItemCommand);
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.ddlFilter.SelectedIndexChanged += new EventHandler(ddlFilter_SelectedIndexChanged);
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
			dt.Columns.Add("PluralName", typeof(string));
			dt.Columns.Add("Type", typeof(string));
			dt.Columns.Add("TypeString", typeof(string));
			dt.Columns.Add("ClassTypeImageUrl", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));
			dt.Columns.Add("EditLink", typeof(string));

			foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
			{

				DataRow row = dt.NewRow();
				row["Name"] = mc.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mc.FriendlyName);
				row["PluralName"] = CHelper.GetResFileString(mc.PluralName);
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
				{
					row["TypeString"] = CHelper.GetResFileString("Bridge");
					row["Type"] = "Bridge";
					row["ClassTypeImageUrl"] = "../../images/metainfo/bridge.gif";
					if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
						row["ClassTypeImageUrl"] = CHelper.GetAbsolutePath("/images/metainfo/bridge_sys.gif");
					row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaBridgeEdit.aspx?class={0}&back=list", mc.Name);
				}
				else if (mc.Attributes.ContainsKey(MetaClassAttribute.IsCard))
				{
					row["TypeString"] = CHelper.GetResFileString("Card");
					row["Type"] = "Card";
					row["ClassTypeImageUrl"] = "../../images/metainfo/card.gif";
					if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
						row["ClassTypeImageUrl"] = "../../images/metainfo/card_sys.gif";
					row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx?class={0}&back=list", mc.Name);
				}
				else
				{
					row["TypeString"] = CHelper.GetResFileString("Info");
					row["Type"] = "Info";
					row["ClassTypeImageUrl"] = "../../images/metainfo/metaclass.gif";
					if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
						row["ClassTypeImageUrl"] = "../../images/metainfo/metaclass_sys.gif";
					row["EditLink"] = String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassEdit.aspx?class={0}&back=list", mc.Name);
				}

				if (ddlFilter.SelectedValue != "All" && row["Type"].ToString() != ddlFilter.SelectedValue)
					continue;

				dt.Rows.Add(row);
			}
			if (Session["MetaClassList_Sort"] == null)
				Session["MetaClassList_Sort"] = "Name";

			DataView dv = dt.DefaultView;
			dv.Sort = Session["MetaClassList_Sort"].ToString();

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem row in grdMain.Items)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("GlobalMetaInfo", "Delete").ToString() + "?')");

			}

		}
		#endregion

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			secHeader.Title = (String)GetGlobalResourceObject("GlobalMetaInfo", "Tables");
			secHeader.AddLink(
			  (String)GetGlobalResourceObject("GlobalMetaInfo", "NewInfoTable"),
			  "~/Apps/MetaDataBase/Pages/Admin/MetaClassEdit.aspx");

			// Check that we have at least 2 non-Bridge MetaClasses
			int counter = 0;
			foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
			{
				if (!mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
					counter++;
				if (counter >= 2)
				{
					secHeader.AddLink(
					  (String)GetGlobalResourceObject("GlobalMetaInfo", "NewBridge"),
					  "~/Apps/MetaDataBase/Pages/Admin/MetaBridgeEdit.aspx");
					break;
				}
			}

			// Check that we have MetaClasses with Card support
			if (DataContext.Current.MetaModel.GetMetaClassSupportedCard().Length > 0)
			{
				secHeader.AddLink(
			(String)GetGlobalResourceObject("GlobalMetaInfo", "NewCard2"),
			"~/Apps/MetaDataBase/Pages/Admin/MetaCardEdit.aspx");
			}
		}
		#endregion

		#region ddlFilter_SelectedIndexChanged
        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			Filter = ddlFilter.SelectedValue;
			BindData();
		} 
		#endregion

		#region grdMain_Command
        /// <summary>
        /// Handles the SortCommand event of the grdMain control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (Session["MetaClassList_Sort"].ToString() == e.SortExpression)
				Session["MetaClassList_Sort"] += " DESC";
			else
				Session["MetaClassList_Sort"] = e.SortExpression;
			BindData();
		}

        /// <summary>
        /// Handles the ItemCommand event of the grdMain control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		void grdMain_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				DataContext.Current.MetaModel.DeleteMetaClass(e.CommandArgument.ToString());
				BindData();
			}
		} 
		#endregion
	}
}