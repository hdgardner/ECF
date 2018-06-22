using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Asset
{
    public partial class List : UserControl
    {
        private static readonly string _CommandName = "CommandName";
        private static readonly string _CommandArguments = "CommandArguments";
        private static readonly string _MoveCopyDialogCommand = "MoveCopyDialogCommand";

        private int m_TotalRecords = 0;

        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>The parent id.</value>
        public int ParentId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("id");
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
                LoadDataAndDataBind(String.Empty); 
        }

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
        void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
        {
            AdminView view = MyListView.CurrentListView.GetAdminView();
            foreach (ViewColumn column in view.Columns)
            {
                // find the column which is to be sorted
                if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
                {
                    // update DataSource parameters
                    string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
                    LoadDataAndDataBind(sortExpression);
                }
            }
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
                        if (String.Compare((string)cmdName, _MoveCopyDialogCommand, true) == 0)
                        {
                            // process move/copy command
                            Dictionary<string, object> args = cmd[_CommandArguments] as Dictionary<string, object>;
                            if (args != null)
                            {
                                ProcessMoveCopyCommand(args);
                            }
                        }
                    }
                }

                ManagementHelper.SetBindGridFlag(MyListView.CurrentListView.ID);
            }

            if (IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
            {
                LoadDataAndDataBind(String.Empty);
                MyListView.MainUpdatePanel.Update();
            }
        }


        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
        private void LoadDataAndDataBind(string sortExpression)
        {
            int totalRecords = 0;
            DataTable table = GetDataSource(out totalRecords);
            DataView view = table.DefaultView;
            view.Sort = sortExpression;
            MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("ID", "Type", "OutlineNumber");
            MyListView.DataSource = view;
            MyListView.DataBind();
            
            m_TotalRecords = totalRecords;
            //MyListView.Grid.RecordCount = totalRecords;
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        private DataTable GetDataSource(out int totalRecords)
        {
            DataTable table = new DataTable();
            int grandParentId = -1;
            char[] delimiters = new char[] { '.' };

            table.Columns.Add(new DataColumn("ID", typeof(string)));
            table.Columns.Add(new DataColumn("Type", typeof(string)));
            table.Columns.Add(new DataColumn("OutlineNumber", typeof(string)));
			table.Columns.Add(new DataColumn("CheckboxEnabled", typeof(bool)));
            table.Columns.Add(new DataColumn("Name", typeof(string)));
            table.Columns.Add(new DataColumn("Filename", typeof(string)));
            table.Columns.Add(new DataColumn("Size", typeof(string)));
            table.Columns.Add(new DataColumn("Url", typeof(string)));
            table.Columns.Add(new DataColumn("Icon", typeof(string)));
            table.Columns.Add(new DataColumn("Created", typeof(DateTime)));
            table.Columns.Add(new DataColumn("GrandParentId", typeof(string)));

            Mediachase.Ibn.Data.Services.TreeNode[] nodes = TreeManager.GetChildNodes(Folder.GetAssignedMetaClass(), ParentId);
            FolderElement[] elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("ParentId", FilterElementType.Equal, ParentId) });

            int nodeIndex = 0;
            foreach(Mediachase.Ibn.Data.Services.TreeNode node in nodes)
            {
                //check to see if the grandParentId has been set yet
                if (grandParentId == -1)
                {
                    //get the ID for the folder above the current folder
                    string[] outlineArray = node.OutlineNumber.ToString().Split(delimiters);
                    if (node.OutlineLevel > 2)
                        int.TryParse(outlineArray[outlineArray.Length - 3].ToString(), out grandParentId);
                }
                
                //if (nodeIndex >= recordToDisplay)
                {
                    DataRow newRow = table.NewRow();
                    newRow["ID"] = node.ObjectId.ToString();
                    newRow["OutlineNumber"] = node.OutlineNumber;
                    newRow["Type"] = "Folder";
					newRow["CheckboxEnabled"] = true; 
                    newRow["Name"] = node.Title;
                    newRow["Icon"] = String.Format("~/App_Themes/Default/images/icons/Node.gif");
                    newRow["FileName"] = node.Title;
                    newRow["Url"] = String.Empty;
                    newRow["Created"] = (DateTime)node.InnerObject.Properties["Created"].Value;
                    table.Rows.Add(newRow);
                }
                nodeIndex++;
            }

            //check to see if folder outline level was available in folders and that the parent is not the root folder
            if (grandParentId < 0 && ParentId > 1)
            {
                //the grandparent folder id needs to be retrieved through other means
                BusinessObject _bindObject = MetaObjectActivator.CreateInstance<BusinessObject>("Folder", ParentId);
                if (_bindObject != null)
                {
                    //get the ID for the folder above the current folder
                    string[] outlineArray = _bindObject["OutlineNumber"].ToString().Split(delimiters);
                    if (outlineArray.Length > 1)
                        int.TryParse(outlineArray[outlineArray.Length - 2].ToString(), out grandParentId);
                }
            }

            //if this row is below the root level, show an up-level folder link.
            //now that all means have been exhausted, only add the up-level link if there is a valid value
            //for the grandParentId
            if (grandParentId >= 0)
            {                
                //add a 'level up' row at the top
                // add additional row at the top. Don't add row if parent node is a Catalog
                DataRow row = table.NewRow();
                row["ID"] = ParentId;
                row["GrandParentId"] = grandParentId;
				row["CheckboxEnabled"] = false; 
                row["Name"] = "[..]";
                row["Type"] = "LevelUp";
                table.Rows.InsertAt(row, 0);
            }

            
            foreach (FolderElement element in elements)
            {
                //if (nodeIndex >= recordToDisplay)
                {
                    DataRow newRow = table.NewRow();

                    newRow["ID"] = element.PrimaryKeyId.ToString();
                    newRow["OutlineNumber"] = String.Empty;
                    newRow["Type"] = "Node";
					newRow["CheckboxEnabled"] = true; 
                    newRow["Name"] = element.Name;
                    

                    /*
                    BlobStorageProvider prov = BlobStorage.Providers[element.BlobStorageProvider];
                    BlobInfo info = prov.GetInfo(new Guid(element.BlobUid.ToString()));
                     * */

                    newRow["Url"] = String.Format("~{0}", element.GetUrl());

                    /*if (info != null)
                    {
                        newRow["FileName"] = info.FileName;
                        newRow["Icon"] = CHelper.GetIcon(info.FileName);                        
                        newRow["Created"] = info.Created;
                        newRow["Size"] = CommerceHelper.ByteSizeToStr(info.ContentSize);
                    }
                     * */
                    //else
                    {
                        newRow["FileName"] = element.Name;
                        newRow["Icon"] = CHelper.GetIcon(element.Name);
                        newRow["Created"] = element.Created;
                        newRow["Size"] = CommerceHelper.ByteSizeToStr((element.ContentSize != null) ? (long)element.ContentSize : 0);
                    }

                    table.Rows.Add(newRow);
                }
                nodeIndex++;
            }

            // TODO: implement paging
            totalRecords = nodes.Length + elements.Length;
            return table;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
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
            LoadDataAndDataBind(String.Empty);
        }

        /// <summary>
        /// Processes the move copy command.
        /// </summary>
        /// <param name="args">The args.</param>
        void ProcessMoveCopyCommand(Dictionary<string, object> args)
        {
            if (args["cmd"] != null)
            {
                string command = args["cmd"].ToString();
                if (String.Compare(command, "move", true) == 0 || String.Compare(command, "copy", true) == 0)
                {
                    string folder = args["folder"] as string;
                    if (String.IsNullOrEmpty(folder))
                        return;

                    int targetFolderId = Int32.Parse(folder);

                    string[] items = MyListView.CurrentListView.GetCheckedCollection();

                    if (items != null)
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
                            if (keys != null)
                            {
                                int id = Int32.Parse(keys[0]);
                                string type = keys[1];

                                if (String.Compare(command, "move", true) == 0)
                                {
                                    if (type == "Node")
                                        FolderElement.Move(id, targetFolderId);
                                    else
                                        Folder.Move(id, targetFolderId);
                                }
                                else if (String.Compare(command, "copy", true) == 0)
                                {
                                    if (type == "Node")
                                        FolderElement.Copy(id, targetFolderId);
                                    else
                                        Folder.CopyRecursive(id, targetFolderId);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}