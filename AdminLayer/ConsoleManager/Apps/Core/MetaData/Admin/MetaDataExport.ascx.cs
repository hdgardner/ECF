using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	///	Used for creating Exporting Meta Data.
	/// </summary>
    public partial class MetaDataExport : CoreBaseUserControl
	{
        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>The app id.</value>
		public string AppId
		{
			get
			{
				object id = Request.QueryString["_a"];
				if (id == null)
					return String.Empty;
				else
					return id.ToString();
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
            SetMetaContext();
			if (!this.IsPostBack)
				BindData();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			//BindData();
			base.DataBind();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			
		}

        /// <summary>
        /// Sets the meta context.
        /// </summary>
        private void SetMetaContext()
        {
            if (String.Compare(AppId, "Catalog", true) == 0)
            {
                this.MDContext = Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext;
            }
            else if (String.Compare(AppId, "Order", true) == 0)
            {
                this.MDContext = Mediachase.Commerce.Orders.OrderContext.MetaDataContext;
            }
            else if (String.Compare(AppId, "Profile", true) == 0)
            {
                this.MDContext = Mediachase.Commerce.Profile.ProfileContext.MetaDataContext;
            }

        }

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			if (String.Compare(AppId, "Catalog", true) == 0)
			{
				BindMetaClasses(CatalogItemsGrid, "Mediachase.Commerce.Catalog", Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext);
				OrderRow.Visible = false;
                ProfileRow.Visible = false;
			}
			else if (String.Compare(AppId, "Order", true) == 0)
			{
				BindMetaClasses(OrderItemsGrid, "Mediachase.Commerce.Orders", Mediachase.Commerce.Orders.OrderContext.MetaDataContext);
				CatalogRow.Visible = false;
                ProfileRow.Visible = false;
			}
            else if (String.Compare(AppId, "Profile", true) == 0)
            {
                BindMetaClasses(ProfileItemsGrid, "Mediachase.Commerce.Profile", Mediachase.Commerce.Profile.ProfileContext.MetaDataContext);
                OrderRow.Visible = false;
                CatalogRow.Visible = false;
            }
		}

        /// <summary>
        /// Binds the meta classes.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="mcNamespace">The mc namespace.</param>
        /// <param name="mdpContext">The MDP context.</param>
		private void BindMetaClasses(DataGrid grid, string mcNamespace, MetaDataContext mdpContext)
		{
			if (grid != null)
			{
				// bind meta data
                MetaClassCollection coll = MetaClass.GetList(mdpContext, mcNamespace, true);

				string metaClassIdColumn = "MetaClassId";
				string metaClassSystemNameColumn = "SystemName";
				string metaClassFriendlyNameColumn = "FriendlyName";

				DataTable dt = new DataTable();
				dt.Columns.Add(metaClassIdColumn, typeof(int));
				dt.Columns.Add(metaClassSystemNameColumn, typeof(string));
				dt.Columns.Add(metaClassFriendlyNameColumn, typeof(string));

				List<int> listIndices = new List<int>();

				foreach (MetaClass mc in coll)
				{
					if (mc.Parent != null)
					{
						DataRow row = dt.NewRow();
						row[metaClassIdColumn] = mc.Id;
						row[metaClassFriendlyNameColumn] = mc.FriendlyName;
						row[metaClassSystemNameColumn] = mc.Name;
						dt.Rows.Add(row);
						listIndices.Add(listIndices.Count);
					}
				}

				grid.DataSource = new DataView(dt);
				grid.DataBind();

				// select all classes
				if (grid.Items.Count > 0)
					((Mediachase.Web.Console.Controls.RowSelectorColumn)grid.Columns[0]).SelectedIndexes = listIndices.ToArray();
			}
		}

        /// <summary>
        /// Handles the Click event of the BtnExport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void BtnExport_Click(object sender, System.EventArgs e)
		{
			try
			{
				string result = String.Empty;

				List<int> selectedAll = new List<int>();
				List<int> currentIndices = new List<int>();

				if (CatalogRow.Visible)
				{
					// export category metadata
					currentIndices = GetSelectedIndices(CatalogItemsGrid);
					selectedAll.AddRange(currentIndices);
					result = MetaInstaller.BackupMetaClasses(MDContext, false, selectedAll.ToArray());
				}
				else if (OrderRow.Visible)
				{
					// export order metadata
					currentIndices = GetSelectedIndices(OrderItemsGrid);
					selectedAll.Clear();
					selectedAll.AddRange(currentIndices);

					result = MetaInstaller.BackupMetaClasses(MDContext, false, selectedAll.ToArray());
				}
                else if (ProfileRow.Visible)
                {
                    // export order metadata
                    currentIndices = GetSelectedIndices(ProfileItemsGrid);
                    selectedAll.Clear();
                    selectedAll.AddRange(currentIndices);

                    result = MetaInstaller.BackupMetaClasses(MDContext, false, selectedAll.ToArray());
                }

				// save xml file
				SaveXML(result, String.Format("MetaDataBackup_{0}_{1}.xml", AppId, DateTime.Now.ToString("yy-MM-dd")));
			}
			catch (Exception ex)
			{
				DisplayErrorMessage(ex.Message);
				return;
			}
        }

        /// <summary>
        /// Gets the selected indices.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns></returns>
		private List<int> GetSelectedIndices(DataGrid grid)
		{
			List<int> selectedIndices  =new List<int>();
			int[] selectedColumns = ((Mediachase.Web.Console.Controls.RowSelectorColumn)grid.Columns[0]).SelectedIndexes;
			if (selectedColumns != null)
			{
				for (int i = 0; i < selectedColumns.Length; i++)
					selectedIndices.Add((int)(grid.DataKeys[selectedColumns[i]]));
			}
			return selectedIndices;	
		}

        /// <summary>
        /// Writes xml to the response output stream.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="filename">Name of the file.</param>
		public void SaveXML(string xml, string filename)
		{
			if (xml != null)
			{
				System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
				response.Clear();
				response.Charset = "utf-8";
				response.ContentType = "text/xml";
				response.AddHeader("content-disposition", String.Format("attachment; filename={0}", filename));

				response.Write(xml);
				response.End();
			}
		}
	}
}
