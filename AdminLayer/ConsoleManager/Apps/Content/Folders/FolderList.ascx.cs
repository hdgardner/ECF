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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Web.Console.Common;
using ComponentArt.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Controls;
using System.Collections.Generic;

namespace Mediachase.Commerce.Manager.Content.Folders
{
    public partial class FolderList : BaseUserControl
    {
		private static readonly string _CommandName = "CommandName";
		private static readonly string _CommandArguments = "CommandArguments";
		private static readonly string _MoveDialogCommand = "MoveFolderDialogCommand";

        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId
        {
            get
            {
                return new Guid(Request.QueryString["SiteId"].ToString());
            }
        }

        /// <summary>
        /// Gets the folder id.
        /// </summary>
        /// <value>The folder id.</value>
        public int FolderId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("FolderId");
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
            MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

			base.OnInit(e);
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            LoadDataAndDataBind();
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				object objArgs = Request.Form["__EVENTARGUMENT"];
				if (objArgs != null)
				{
					Dictionary<string, object> cmd = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(objArgs.ToString()) as Dictionary<string, object>;
					if (cmd != null && cmd.Count > 1)
					{
						object cmdName = cmd[_CommandName];
						if (String.Compare((string)cmdName, _MoveDialogCommand, true) == 0)
						{
							// process move command
							Dictionary<string, object> args = cmd[_CommandArguments] as Dictionary<string, object>;
							if (args != null)
							{
								ProcessMoveCommand(args);
								ManagementHelper.SetBindGridFlag(MyListView.CurrentListView.ID);
							}
						}
					}
				}
			}

			if (IsPostBack &&
				(ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0))
			{
				LoadDataAndDataBind();
				DataBind();
				MyListView.MainUpdatePanel.Update();
			}
		}

        /// <summary>
        /// Processes the move command.
        /// </summary>
        /// <param name="args">The args.</param>
		void ProcessMoveCommand(Dictionary<string, object> args)
        {
			int folderId = 0;

			string folderString = args["folder"] as string;
			if (String.IsNullOrEmpty(folderString) || !Int32.TryParse(folderString, out folderId))
				return;

			string[] items = MyListView.CurrentListView.GetCheckedCollection();

			if (items != null)
			{
				for (int i = 0; i < items.Length; i++)
				{
					string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
					if (keys != null)
					{
						int id = Int32.Parse(keys[0]);
						FileTreeItem.MoveTo(id, folderId);
					}
				}
			}
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
            DataTable files = FileTreeItem.LoadItemByFolderIdDT(FolderId);
			DataColumn column = new DataColumn("CheckboxEnabled", typeof(System.Boolean));
            files.Columns.Add(column);
			files.Columns.Add(new DataColumn("ImageUrl", typeof(string)));
            using (IDataReader reader = FileTreeItem.LoadParent(FolderId))
            {
                if (reader.Read())
                {
                    DataRow row = files.NewRow();
					row["CheckboxEnabled"] = false;
                    row["PageId"] = (int)reader["PageId"];
                    row["Name"] = "[..]";
                    row["IsDefault"] = false;
                    row["IsPublic"] = true;
                    row["IsFolder"] = true;
                    row["Order"] = -1000000;
                    row["SiteId"] = SiteId;
					row["ImageUrl"] = Page.ResolveUrl("~/Apps/Content/images/Up One Level.png");
                    files.Rows.InsertAt(row, 0);
                }
                reader.Close();
            }

            DataView view = new DataView(files);
            view.Sort = "IsFolder DESC, Order";

			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("PageId", "SiteId", "IsFolder");
			MyListView.DataSource = view;
			DataBind();
        }
    }
}