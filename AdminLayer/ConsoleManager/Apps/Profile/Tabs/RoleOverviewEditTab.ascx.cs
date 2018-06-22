using System;
using System.Collections;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Dto;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Profile.Tabs
{
	public partial class RoleOverviewEditTab : ProfileBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _RoleIdString = "RoleId";
		private const string _PermissionDtoString = "PermissionDto";

		private const string _moduleNodePrefix = "module_";
		private const string _groupNodePrefix = "group_";
		private const string _permissionNodePrefix = "permission_";

		private PermissionDto _Permission = null;

		/// <summary>
		/// Gets the role id.
		/// </summary>
		/// <value>The role id.</value>
		public string RoleId
		{
			get
			{
				return ManagementHelper.GetValueFromQueryString("RoleId", String.Empty);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
                BindForm();

            if(!Page.ClientScript.IsClientScriptIncludeRegistered("TreeView"))
                Page.ClientScript.RegisterClientScriptInclude("TreeView", Page.ResolveClientUrl("~/scripts/treeview.js"));
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			tbRoleName.Text = RoleId;

            //first handle permissions; if permissions not present, deny
            if (String.IsNullOrEmpty(RoleId))
                SecurityManager.CheckRolePermission("profile:roles:mng:create");
            else
                SecurityManager.CheckRolePermission("profile:roles:mng:edit");

            if (!String.IsNullOrEmpty(RoleId))
				tbRoleName.ReadOnly = true;
			else
				tbRoleName.ReadOnly = false;

			BindPermissionsTree();
		}

        /// <summary>
        /// Binds the permissions tree.
        /// </summary>
		private void BindPermissionsTree()
		{
			PermissionsTree.Nodes.Clear();
			foreach (ModuleConfig config in ManagementContext.Current.Configs)
			{
                // If there no acl defined, do not show items
                if (config.Acl == null)
                    continue;

                foreach (AclGroup group in config.Acl.Groups)
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode.Value = GetTreeNodeUniqueId(_groupNodePrefix, group.ID);
                    treeNode.Text = UtilHelper.GetResFileString(group.Name);
                    treeNode.ShowCheckBox = false;
                    treeNode.SelectAction = TreeNodeSelectAction.Expand;
                    treeNode.ImageUrl = Page.ResolveUrl("~/Apps/Profile/images/security_folder.png");
                    foreach (AclGroup childGroup in group.Groups)
                    {
                        if (AddAclGroup(childGroup, treeNode) == 0)
                            treeNode.Select();
                    }

                    PermissionsTree.Nodes.Add(treeNode);
                }
				//PermissionsTree.CollapseAll();
			}
		}

        /// <summary>
        /// Adds the acl group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="treeNode">The tree node.</param>
        /// <returns></returns>
		private int AddAclGroup(AclGroup group, TreeNode treeNode)
		{
            int childrenChecked = -1; // 1 - checked, 2 - unchecked, 0 - some checked, -1 - not set
			TreeNode groupNode = new TreeNode();
			groupNode.Value = GetTreeNodeUniqueId(_groupNodePrefix, group.ID);
            groupNode.Text = UtilHelper.GetResFileString(group.Name);
			groupNode.ShowCheckBox = true;
			groupNode.ImageUrl = Page.ResolveUrl("~/Apps/Profile/images/security_folder.png");
			groupNode.SelectAction = TreeNodeSelectAction.Expand;
			treeNode.ChildNodes.Add(groupNode);

			// add child groups
            foreach (AclGroup childGroup in group.Groups)
            {
                int result = AddAclGroup(childGroup, groupNode);
                if (result == 2 && childrenChecked == 1)
                    childrenChecked = 0;
                else if (result == 2 && childrenChecked == -1)
                    childrenChecked = 2;
                else if (result == 1 && childrenChecked == -1)
                    childrenChecked = 1;
                else if (result == 1 && childrenChecked == 2)
                    childrenChecked = 0;
                else if (result == 0)
                    childrenChecked = 0;
            }

			// add permissions
			foreach (AclPermission permission in group.Permissions)
			{
				TreeNode permissionNode = new TreeNode();
				permissionNode.Value = GetTreeNodeUniqueId(_permissionNodePrefix, permission.ToString());
                permissionNode.Text = UtilHelper.GetResFileString(permission.Name);
				permissionNode.SelectAction = TreeNodeSelectAction.None;
				permissionNode.ImageUrl = Page.ResolveUrl("~/Apps/Profile/images/security_key.png");

				// check node if permission is set
				if (_Permission != null)
				{
					PermissionDto.RolePermissionRow[] permissionRows = (PermissionDto.RolePermissionRow[])_Permission.RolePermission.Select(String.Format("Permission='{0}'", permission.ToString()));
                    if (permissionRows != null && permissionRows.Length > 0)
                    {
                        permissionNode.Checked = true;
                        if (childrenChecked == -1)
                            childrenChecked = 1;
                    }
				}

                if (!permissionNode.Checked && childrenChecked == 1)
                    childrenChecked = 0;
                if (!permissionNode.Checked && childrenChecked == -1)
                    childrenChecked = 2;

				groupNode.ChildNodes.Add(permissionNode);
			}

            groupNode.Checked = childrenChecked == 1;

            if (childrenChecked == 0)
            {
                groupNode.Select();
                groupNode.Expand();
            }
            else
                groupNode.Collapse();

            return childrenChecked;
		}

        // Returns a value indicating whether the specified 
        // TreeNode has checked child nodes.
        private bool HasCheckedChildNodes(TreeNode node)
        {
            if (node.ChildNodes.Count == 0) return false;
            foreach (TreeNode childNode in node.ChildNodes)
            {
                if (childNode.Checked) return true;
                // Recursively check the children of the current child node.
                if (HasCheckedChildNodes(childNode)) return true;
            }
            return false;
        }

		#region 2delete
		private void BindPermissionsTree2()
		{
			JsonTreeNode rootNode = new JsonTreeNode();
			foreach (ModuleConfig config in ManagementContext.Current.Configs)
			{
				JsonTreeNode jsonTreeNode = new JsonTreeNode();
				jsonTreeNode.id = _moduleNodePrefix + config.Name;
				jsonTreeNode.itemid = config.Name;
				jsonTreeNode.text = config.DisplayName;
				rootNode.children.Add(jsonTreeNode);

				// add acl groups
				foreach (AclGroup group in config.Acl.Groups)
				{
					AddAclGroup2(group, jsonTreeNode);
				}
			}
			
			string json = JsonSerializer.Serialize(rootNode);
		}

		private void AddAclGroup2(AclGroup group, JsonTreeNode treeNode)
		{
			JsonTreeNode groupNode = new JsonTreeNode();
			groupNode.id = _groupNodePrefix + group.ID;
			groupNode.itemid = group.ID;
			groupNode.text = group.Name;
			treeNode.children.Add(groupNode);

			// add child groups
			foreach (AclGroup childGroup in group.Groups)
				AddAclGroup2(childGroup, groupNode);

			// add permissions
			foreach (AclPermission permission in group.Permissions)
			{
				JsonTreeNode permissionNode = new JsonTreeNode();
				permissionNode.id = _permissionNodePrefix + permission.ID;
				permissionNode.itemid = permission.ID;
				permissionNode.text = permission.Name;

				groupNode.children.Add(permissionNode);
			}
		}
		#endregion

		/// <summary>
		/// Entries the code check.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void RoleNameCheck(object sender, ServerValidateEventArgs args)
		{
			if (String.IsNullOrEmpty(RoleId) && Roles.RoleExists(args.Value))
				args.IsValid = false;
			else
				args.IsValid = true;
		}

		#region IAdminContextControl Members
		public void LoadContext(IDictionary context)
		{
			_Permission = (PermissionDto)context[_PermissionDtoString];
		}
		#endregion

		#region IAdminTabControl Members
		public void SaveChanges(IDictionary context)
		{
			if (String.IsNullOrEmpty(RoleId) || !Roles.RoleExists(RoleId))
				Roles.CreateRole(tbRoleName.Text);

			// update permissions
			PermissionDto permissionDto = (PermissionDto)context[_PermissionDtoString];

			if (permissionDto == null)
				return;

			// remove all existing permissions
			foreach(PermissionDto.RolePermissionRow rpRow in permissionDto.RolePermission.Rows)
				rpRow.Delete();

			// add checked permissions
			foreach (TreeNode node in PermissionsTree.CheckedNodes)
			{
				// if this is permission node, add its value
				if (node.Value.StartsWith(_permissionNodePrefix))
				{
					PermissionDto.RolePermissionRow rpRow = permissionDto.RolePermission.NewRolePermissionRow();
					rpRow.RoleName = tbRoleName.Text;
					rpRow.ApplicationId = ProfileConfiguration.Instance.ApplicationId;
					rpRow.Permission = GetIdFromTreeNodeUniqueId(node.Value);

					if (rpRow.RowState == DataRowState.Detached)
						permissionDto.RolePermission.Rows.Add(rpRow);
				}
			}
		}
		#endregion

        /// <summary>
        /// Gets the tree node unique id.
        /// </summary>
        /// <param name="perfix">The perfix.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
		private string GetTreeNodeUniqueId(string perfix, string id)
		{
			return perfix + id;
		}

        /// <summary>
        /// Gets the id from tree node unique id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
		private string GetIdFromTreeNodeUniqueId(string id)
		{
			if (String.IsNullOrEmpty(id))
				return id;

			string retVal = id;

			int index = id.IndexOf('_');
			if (index >= 0 && index < id.Length - 1)
				retVal = id.Substring(index + 1);
			else if (index == id.Length - 1)
				retVal = String.Empty;

			return retVal.ToLower();
		}
	}
}