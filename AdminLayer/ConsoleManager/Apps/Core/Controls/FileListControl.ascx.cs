using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using Mediachase.Web.Console.BaseClasses;
using ComponentArt.Web.UI;

namespace Mediachase.Commerce.Manager.Core.Controls
{
	/// <summary>
	///	Used for displaying list of files in the specified folder.
	/// </summary>
    public partial class FileListControl : CoreBaseUserControl
	{
		private string _Folder = String.Empty;
        private int _KeyFieldIndex = 0;

        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>The folder.</value>
		public string Folder
		{
			get
			{
				return _Folder;
			}
			set
			{
				_Folder = value;
			}
		}

		private string _GridAppId = String.Empty;
        /// <summary>
        /// Gets or sets the grid app id.
        /// </summary>
        /// <value>The grid app id.</value>
		public string GridAppId
		{
			get
			{
				return _GridAppId;
			}
			set
			{
				_GridAppId = value;
			}
		}

		private string _GridViewId = String.Empty;
        /// <summary>
        /// Gets or sets the grid view id.
        /// </summary>
        /// <value>The grid view id.</value>
		public string GridViewId
		{
			get
			{
				return _GridViewId;
			}
			set
			{
				_GridViewId = value;
			}
		}

		private string _SelectedBtnId = String.Empty;
        /// <summary>
        /// Button that needs to be enabled/disabled when an item in grid is selected/deselected
        /// </summary>
        /// <value>The selected BTN id.</value>
		public string SelectedBtnId
		{
			get
			{
				return _SelectedBtnId;
			}
			set
			{
				_SelectedBtnId = value;
			}
		}

        public int KeyFieldIndex
        {
            get
            {
                return _KeyFieldIndex;
            }
            set
            {
                _KeyFieldIndex = value;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!DefaultGrid.IsCallback)
				DataBind();
			else
			{
				GridHelper.BindGrid(DefaultGrid, GridAppId, GridViewId);
				base.DataBind();
			}

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ecf-catalog-filelist-select" + this.ID, "function " + this.ID + "_FilesListDefaultGrid_onItemSelect(sender, eventArgs){ id = '" + this.SelectedBtnId + "'; if (id != '' ){var btn = $get(id);if (btn != null)btn.disabled = false;} var obj = $get('" + hfSelectedItems.ClientID + "'); if (obj != null) obj.value = eventArgs.get_item().Index; }", true);

			Response.Cache.SetNoStore();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			DefaultGrid.SortCommand += new Grid.SortCommandEventHandler(DefaultGrid_SortCommand);
			DefaultGrid.NeedDataSource += new Grid.NeedDataSourceEventHandler(DefaultGrid_NeedDataSource);
			DefaultGrid.NeedRebind += new Grid.NeedRebindEventHandler(DefaultGrid_NeedRebind);
            DefaultGrid.ClientEvents.ItemSelect = new ClientEvent(String.Format("{0}_FilesListDefaultGrid_onItemSelect", this.ID));

            base.OnInit(e);
		}

		void DefaultGrid_NeedRebind(object sender, EventArgs e)
		{
			DefaultGrid.DataBind();
		}

		void DefaultGrid_NeedDataSource(object sender, EventArgs e)
		{
			DirectoryInfo dir = new DirectoryInfo(MapPath(Folder));
			FileInfo[] files = dir.GetFiles();
			// sort files by grid's sort expression
			IEnumerable<FileInfo> filesQuery;

			bool ascendingSort = DefaultGrid.Sort.EndsWith("ASC");

			if (DefaultGrid.Sort.StartsWith("Name"))
			{
				if(ascendingSort)
					filesQuery = from file in files orderby file.Name ascending select file;
				else
					filesQuery = from file in files orderby file.Name descending select file;
			}
			else if (DefaultGrid.Sort.StartsWith("Length"))
			{
				if (ascendingSort)
					filesQuery = from file in files orderby file.Length ascending select file;
				else
					filesQuery = from file in files orderby file.Length descending select file;
			}
			else if (DefaultGrid.Sort.StartsWith("LastWriteTime"))
			{
				if (ascendingSort)
					filesQuery = from file in files orderby file.LastWriteTime ascending select file;
				else
					filesQuery = from file in files orderby file.LastWriteTime descending select file;
			}
			else if (DefaultGrid.Sort.StartsWith("CreationTime"))
			{
				if (ascendingSort)
					filesQuery = from file in files orderby file.CreationTime ascending select file;
				else
					filesQuery = from file in files orderby file.CreationTime descending select file;
			}
			else
				filesQuery = from file in files orderby file.CreationTime descending select file;

			// set sorted result as datasource
			DefaultGrid.DataSource = filesQuery.ToArray<FileInfo>();
		}

		void DefaultGrid_SortCommand(object sender, GridSortCommandEventArgs e)
		{
			DefaultGrid.Sort = e.SortExpression;
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			BindGrid();
			base.DataBind();
		}

        /// <summary>
        /// Binds the grid.
        /// </summary>
		protected void BindGrid()
		{
			GridHelper.BindGrid(DefaultGrid, GridAppId, GridViewId);
			DirectoryInfo dir = new DirectoryInfo(MapPath(Folder));
			FileInfo[] files = dir.GetFiles();
			// sort files by created date desc
			IEnumerable<FileInfo> filesQuery = from file in files orderby file.CreationTime descending select file;
			// set sorted result as datasource
			DefaultGrid.DataSource = filesQuery.ToArray<FileInfo>();
			DefaultGrid.DataBind();
		}

		/// <summary>
		/// Returns selected filepath (must be in the first column).
		/// </summary>
		/// <returns></returns>
		public string GetSelectedFilePath()
		{
			GridItem item = null;
			if (!String.IsNullOrEmpty(hfSelectedItems.Value))
			{
				int index = -1;
				if (Int32.TryParse(hfSelectedItems.Value, out index) && index >= 0 && DefaultGrid.Items.Count > index)
					item = DefaultGrid.Items[index];
			}
            return item != null ? (string)item[_KeyFieldIndex] : null;
		}
	}
}
