using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Library
{
	partial class Folder
	{
        protected override void OnLoad()
        {
            base.OnLoad();

            this["ParentId"] = (PrimaryKeyId?)((int?)this["ParentId"]);
        }

		/// <summary>
		/// Gets the tree service.
		/// </summary>
		/// <value>The tree service.</value>
		public TreeService GetTreeService()
		{
			return this.GetService<TreeService>();
		}

        /// <summary>
        /// Moves the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="newParentId">The new parent id.</param>
        public static void Move(Folder folder, int newParentId)
        {
            TreeManager.AppendNode(GetAssignedMetaClass(), newParentId, folder);
        }

        /// <summary>
        /// Moves the specified folder id.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <param name="newParentId">The new parent id.</param>
        public static void Move(int folderId, int newParentId)
        {
            Folder folder = new Folder(folderId);
            Move(folder, newParentId);
        }

        /// <summary>
        /// Copies the specified folder id.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        /// <param name="parentId">The parent id.</param>
        public static void CopyRecursive(int folderId, int parentId)
        {
            Folder srcFolder = new Folder(folderId);
            Folder newFolder = (Folder)srcFolder.Clone();
            newFolder.Save();
            Move(newFolder, parentId);

            parentId = (int)newFolder.PrimaryKeyId.Value;

            TreeNode[] nodes = TreeManager.GetChildNodes(GetAssignedMetaClass(), folderId);
            foreach (TreeNode node in nodes)
            {
                CopyRecursive(node.ObjectId, parentId);
            }

            FolderElement[] elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("ParentId", FilterElementType.Equal, folderId) });
            foreach (FolderElement element in elements)
            {
                FolderElement.Copy(element, parentId);
            }
        }
	}
}
