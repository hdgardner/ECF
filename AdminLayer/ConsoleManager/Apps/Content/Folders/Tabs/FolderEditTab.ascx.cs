using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Cms;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Folders.Tabs
{
	public partial class FolderEditTab : BaseUserControl, IAdminTabControl
    {
		private static readonly string _PageIdKey = "PageId";
		private static readonly string _SiteIdKey = "SiteId";
		private static readonly string _OutlineKey = "Outline";

		#region PageId Property
		/// <summary>
		/// Gets the folder id.
		/// </summary>
		/// <value>The folder id.</value>
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
					}
					reader.Close();
				}
			}

			if (!pageExists)
			{
				// update UI for creating a new folder
				BindParentFolderList();
				ParentFolderRow.Visible = true;
			}

			BindRoles();
		}

		/// <summary>
		/// Binds the roles.
		/// </summary>
		private void BindRoles()
		{
			RolesList.DataSource = Roles.GetAllRoles();
			RolesList.DataBind();

			foreach (ListItem item in RolesList.Items)
			{
				using (IDataReader reader = FileTreeItem.PageAccessGetByRoleIdPageId(item.Value, PageId))
				{
					if (reader.Read())
						item.Selected = true;
					else
						item.Selected = false;

					reader.Close();
				}
			}

			// select Everyone role in case it is a new folder
			if (PageId <= 0)
			{
				ListItem everyoneItem = RolesList.Items.FindByValue(AppRoles.EveryoneRole);
				if (everyoneItem != null)
					everyoneItem.Selected = true;
			}
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
		/// Checks is folder with secified name exists.
		/// </summary>
		/// <param name="parentId">The parent id.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private bool DirExists(int parentId, string name)
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

			string childPath = parentPath + name + "/";

			using (IDataReader reader2 = FileTreeItem.GetItemByOutlineAll(childPath, siteId))
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
		#endregion

		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			bool pageExists = false;
			int idToCheck = 0;

			if (PageId > 0)
			{
				// if folder exists, get id of its parent folder
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

			// check if folder exists
			if (DirExists(idToCheck, Name.Text))
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
						// edit folder
						pageExists = true;

						EditFolder(reader);
					}
					reader.Close();
				}
			}

			if (!pageExists)
				CreateFolder();
		}
		#endregion

		/// <summary>
		/// Updates an existing folder.
		/// </summary>
		/// <param name="reader"></param>
		private void EditFolder(IDataReader reader)
		{
			FileTreeItem.UpdateFileItem(PageId, Name.Text, false, true, false, String.Empty, SiteId);

			// update folder access
			foreach (ListItem item in RolesList.Items)
			{
				if (item.Selected)
				{
					using (IDataReader reader1 = FileTreeItem.PageAccessGetByRoleIdPageId(item.Value, PageId))
					{
						if (!reader1.Read())
							FileTreeItem.AddPageAccess(item.Value, PageId);
						reader1.Close();
					}
				}
				else
				{
					using (IDataReader reader2 = FileTreeItem.PageAccessGetByRoleIdPageId(item.Value, PageId))
					{
						if (reader2.Read())
							FileTreeItem.DeletePageAccess((int)reader2["PageAccessId"]);
						reader2.Close();
					}
				}
			}
		}

		/// <summary>
		/// Creates a new folder.
		/// </summary>
		private void CreateFolder()
		{
			//create temporary folder with unique name
			string guid = Guid.NewGuid().ToString();
			int newId = FileTreeItem.AddFileItem(guid, true, true, false, String.Empty, SiteId);

			FileTreeItem.MoveTo(newId, Int32.Parse(Root.SelectedValue));
			FileTreeItem.UpdateFileItem(newId, Name.Text, true, true, false, String.Empty, SiteId);

			// update folder access
			foreach (ListItem item in RolesList.Items)
			{
				if (item.Selected)
					FileTreeItem.AddPageAccess(item.Value, newId);
			}
		}
    }
}