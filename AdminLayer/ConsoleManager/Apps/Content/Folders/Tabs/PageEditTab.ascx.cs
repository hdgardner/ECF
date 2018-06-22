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
using Mediachase.Cms;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Folders.Tabs
{
    public partial class PageEditTab : BaseUserControl, IAdminTabControl
    {
		private static readonly string _PageIdKey = "PageId";
		private static readonly string _SiteIdKey = "SiteId";
		private static readonly string _OutlineKey = "Outline";

		private static readonly string _GlobalVariableTitleKey = "title";
		private static readonly string _GlobalVariableMetaKeywordsKey = "meta_keywords";
		private static readonly string _GlobalVariableMetaDescriptionKey = "meta_description";

		#region PageId Property
		/// <summary>
        /// Gets the page id.
        /// </summary>
        /// <value>The page id.</value>
        public int PageId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("PageId");
            }
		}
		#endregion

		#region SiteId Property
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
		#endregion

		#region FolderId Property
		/// <summary>
		/// Gets the folder id.
		/// </summary>
		/// <value>The folder id.</value>
		protected int FolderId
		{
			get
			{
				if (Parameters["FolderId"] != null)
					return int.Parse(Parameters["FolderId"]);
				return -1;
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
			if (PageId > 0)
				SecurityManager.CheckRolePermission("content:site:nav:mng:edit");
			else
				SecurityManager.CheckRolePermission("content:site:nav:mng:create");

			if (!this.IsPostBack)
            {
                base.DataBind();
                BindForm();
            }
		}

		#region Binding form
		/// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			bool pageExists = false;

			if (PageId > 0)
			{
				using (IDataReader reader = FileTreeItem.GetItemById(PageId))
				{
					if (reader.Read())
					{
						pageExists = true;

						Name.Text = reader["Name"].ToString();
						IsDefault.IsSelected = (bool)reader["IsDefault"];
						MasterPageText.Text = (reader["MasterPage"] != DBNull.Value) && (reader["MasterPage"] != null) ? (string)reader["MasterPage"] : String.Empty;
					}
					reader.Close();
				}

				// Load meta attributes
				using (IDataReader reader = PageAttributes.GetByPageId(PageId))
				{
					if (reader.Read())
					{
						txtTitle.Text = (string)reader["Title"];
						txtKeywords.Text = (string)reader["MetaKeys"];
						txtDescription.Text = (string)reader["MetaDescriptions"];
					}
					else
					{
						// add default site attributes
						PageAttributes.Add(PageId, GlobalVariable.GetVariable(_GlobalVariableTitleKey, SiteId), GlobalVariable.GetVariable(_GlobalVariableMetaKeywordsKey, SiteId), GlobalVariable.GetVariable(_GlobalVariableMetaDescriptionKey, SiteId));
						txtTitle.Text = GlobalVariable.GetVariable(_GlobalVariableTitleKey, SiteId);
						txtKeywords.Text = GlobalVariable.GetVariable(_GlobalVariableMetaKeywordsKey, SiteId);
						txtDescription.Text = GlobalVariable.GetVariable(_GlobalVariableMetaDescriptionKey, SiteId);
					}
					reader.Close();
				}
			}

			if (!pageExists)
			{
				// update UI for creating a new page
				BindParentFolderList();
				ParentFolderRow.Visible = true;
			}

            // since mater page is not used anywhere for now, hide it alway
            MasterPageRow.Visible = false;
		}

		/// <summary>
		/// Binds the parent folder list.
		/// </summary>
		protected void BindParentFolderList()
		{
			this.Root.DataSource = FileTreeItem.LoadAllFoldersDT(SiteId);
			this.Root.DataTextField = _OutlineKey;
			this.Root.DataValueField = _PageIdKey;
			this.Root.DataBind();
			if (FolderId != -1)
				this.Root.SelectedValue = FolderId.ToString();
		}
		#endregion

		#region Helpers
		/// <summary>
		/// Pages the exists.
		/// </summary>
		/// <param name="parentId">The parent id.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private bool PageExists(int parentId, string name)
		{
			bool exists = false;

			string parentPath = string.Empty;
			Guid siteId = Guid.Empty;

			using (IDataReader reader = FileTreeItem.GetItemById(parentId))
			{
				if (reader.Read())
				{
					parentPath = (string)reader[_OutlineKey] == "/" ? "" : (string)reader[_OutlineKey];
					siteId = (Guid)reader[_SiteIdKey];
				}
				reader.Close();
			}

			if (!parentPath.EndsWith("/"))
				parentPath = parentPath + "/";

			string childPath = parentPath + name;

			using (IDataReader reader2 = FileTreeItem.GetItemByOutline(childPath, siteId))
			{
				if (reader2.Read())
				{
					if (PageId != (int)reader2[_PageIdKey])
						exists = true;
				}
				reader2.Close();
			}

			return exists;
		}

		/// <summary>
		/// Adds .aspx to page name if needed.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private string MakePageName(string name)
		{
			return !name.EndsWith(".aspx") ? name + ".aspx" : name;
		}
		#endregion

		/// <summary>
		/// Checks if entered name unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			bool pageExists = false;
			int idToCheck = 0;

			if (PageId > 0)
			{
				using (IDataReader reader = FileTreeItem.GetItemById(PageId))
				{
					if (reader.Read())
					{
						pageExists = true;

						// get parent id
						using (IDataReader parentReader = FileTreeItem.LoadParent(PageId))
						{
							if (parentReader.Read())
								idToCheck = (int)parentReader[_PageIdKey];
							parentReader.Close();
						}
					}
					reader.Close();
				}
			}

			if (!pageExists)
				idToCheck = Int32.Parse(Root.SelectedValue);

			// check if page exists
			string newPageName = MakePageName(Name.Text);

			if (PageExists(idToCheck, newPageName))
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminTabControl Members
		/// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			bool pageExists = false;

			if (PageId > 0)
			{
				using (IDataReader reader = FileTreeItem.GetItemById(PageId))
				{
					if (reader.Read())
					{
						// edit page
						pageExists = true;

						EditPage(reader);
					}
					reader.Close();
				}
			}

			if (!pageExists)
				CreatePage();
        }
        #endregion

		/// <summary>
		/// Updates an existing page.
		/// </summary>
		/// <param name="reader"></param>
		private void EditPage(IDataReader reader)
		{
			string fileName = MakePageName(Name.Text);
			bool fileIsDefault = false;
			bool fileIsPublic = false;

			bool isFolder = (bool)reader["IsFolder"];
			if (isFolder)
				return;

			fileIsDefault = IsDefault.IsSelected;

			// clear old default in current folder
			if (fileIsDefault)
			{
				string outline = ((string)reader[_OutlineKey]).Substring(0, ((string)reader[_OutlineKey]).LastIndexOf('/') + 1);
				using (IDataReader reader2 = FileTreeItem.GetItemByOutline(outline, SiteId))
				{
					while (reader2.Read())
						FileTreeItem.UpdateFileItem((int)reader2[_PageIdKey], (string)reader2["Name"], (bool)reader2["IsPublic"], (bool)reader["IsFolder"], false, (string)reader2[reader2.FieldCount - 1] /*MasterPage*/, SiteId);
					reader2.Close();
				}
			}

			// update page attributes
			FileTreeItem.UpdateFileItem(PageId, fileName, fileIsPublic, false, fileIsDefault, MasterPageText.Text, SiteId);
			PageAttributes.Update(PageId, txtTitle.Text, txtKeywords.Text, txtDescription.Text);
		}

		/// <summary>
		/// Creates a new page.
		/// </summary>
		private void CreatePage()
		{
			int newId = 0;
			string newPageName = MakePageName(Name.Text);

			//create new page
			string guid = Guid.NewGuid().ToString();
			newId = FileTreeItem.AddFileItem(guid, true, false, false, MasterPageText.Text, SiteId);
			FileTreeItem.MoveTo(newId, int.Parse(Root.SelectedValue));
			FileTreeItem.UpdateFileItem(newId, newPageName, true, true, false, MasterPageText.Text, SiteId);

			//append meta description and keywords
			PageAttributes.Add(newId, 
				String.IsNullOrEmpty(txtTitle.Text) ? GlobalVariable.GetVariable(_GlobalVariableTitleKey, SiteId) : txtTitle.Text, 
				String.IsNullOrEmpty(txtKeywords.Text) ? GlobalVariable.GetVariable(_GlobalVariableMetaKeywordsKey, SiteId) : txtKeywords.Text, 
				String.IsNullOrEmpty(txtDescription.Text) ? GlobalVariable.GetVariable(_GlobalVariableMetaDescriptionKey, SiteId) : txtDescription.Text);
		}
    }
}
