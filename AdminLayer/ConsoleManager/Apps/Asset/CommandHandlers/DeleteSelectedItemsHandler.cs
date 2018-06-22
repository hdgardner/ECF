using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;

namespace Mediachase.Commerce.Manager.Asset.CommandHandlers
{
	public class DeleteSelectedItemsHandler : ICommand
	{
		#region ICommand Members
        /// <summary>
        /// Invokes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
			    CommandParameters cp = (CommandParameters)element;

                string gridId = cp.CommandArguments["GridId"];
                string[] items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);

				if (items != null)
				{
					int error = 0;
					string errorMessage = String.Empty;
					try
					{
						ProcessDeleteCommand(items, 
							ManagementHelper.GetIntFromQueryString("id"));

                        ManagementHelper.SetBindGridFlag(gridId);
					}
					catch (Exception ex)
					{
						error++;
						errorMessage = ex.Message;
					}

					if (error > 0)
					{
						errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
						ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
							String.Format("alert('{0}{1}');", "Failed to delete item(s). Error: ", errorMessage), true);
					}
				}
				else
					return;

			    //NameValueCollection qs = ((Control)sender).Page.Request.QueryString;
			}
		}
		#endregion

        /// <summary>
        /// Processes the delete command.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="Id">The id.</param>
		void ProcessDeleteCommand(string[] items, int Id)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
                    int id = Int32.Parse(keys[0]);
                    string type = keys[1];
                    string outlineNumber = keys[2];
 
                    if (type.Equals("Node"))
                    {
                        FolderElement.Delete(id);
                    }
                    else if (type.Equals("Folder"))
                    {
                        Mediachase.Ibn.Data.Services.TreeNode node = TreeManager.GetNodeByOulineNumber(Folder.GetAssignedMetaClass(), outlineNumber);
                        if (node != null)
                        {
                            DeleteEntryRecursive(node);
                            TreeManager.DeleteNode(node);
                        }
                    }
				}
			}
		}

        /// <summary>
        /// Deletes the entry recursive.
        /// </summary>
        /// <param name="node">The node.</param>
        private void DeleteEntryRecursive(Mediachase.Ibn.Data.Services.TreeNode node)
        {
            if (TreeManager.GetAllChildNodeCount(node) > 0)
            {
                foreach (Mediachase.Ibn.Data.Services.TreeNode childNode in TreeManager.GetAllChildNodes(node))
                {
                    DeleteEntryRecursive(childNode);
                }
            }

            int folderId = node.ObjectId;

            FolderElement[] elements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("ParentId", FilterElementType.Equal, folderId) });
            foreach (FolderElement element in elements)
            {
                element.Delete();
            }
        }
	}
}
