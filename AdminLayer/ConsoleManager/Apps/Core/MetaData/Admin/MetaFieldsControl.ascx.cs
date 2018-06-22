using System;
using System.Data;
using System.Web.UI.WebControls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	///		Summary description for MetaFieldsControl.
	/// </summary>
	public partial class MetaFieldsControl : CoreBaseUserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if(!IsPostBack)
				BindData();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			ItemsGrid.Columns[0].HeaderText = RM.GetString("ATTRIBUTES_HDR_NAME");
			ItemsGrid.Columns[1].HeaderText = RM.GetString("GENERAL_OPTIONS");
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("MetaFieldId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));
				
			// Fields
			MetaFieldCollection mfc = MetaField.GetList(MDContext);

			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					DataRow row = dt.NewRow();
					row["MetaFieldId"] = field.Id;
					row["FriendlyName"] = field.FriendlyName;
					dt.Rows.Add(row);
				}
			}

			ItemsGrid.DataSource = new DataView(dt);
			ItemsGrid.DataBind();
		}

        /// <summary>
        /// Handles the EditCommand event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void ItemsGrid_EditCommand(object sender, DataGridCommandEventArgs e)
		{
			int id = int.Parse(((DataGrid) sender).DataKeys[e.Item.ItemIndex].ToString());
			Response.Redirect(String.Format("~/AttributeEdit.aspx?id={0}", id));
		}

        /// <summary>
        /// Handles the Page event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		private void ItemsGrid_Page(object sender, DataGridPageChangedEventArgs e)
		{
			this.ItemsGrid.CurrentPageIndex = e.NewPageIndex;
			this.BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			ItemsGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(ItemsGrid_EditCommand);
			this.ItemsGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ItemsGrid_Page);
			//ItemsGrid.PageSizeChanged += new DataGridPageSizeChangedEventHandler(ItemsGrid_PageSizeChanged);
		}
		#endregion
	}
}
