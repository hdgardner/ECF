using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Profile;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
    public class SiteDeleteHandler : ICommand
    {
        #region ICommand Members
        /// <summary>
        /// Invokes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
        public void Invoke(object sender, object element)
        {
            if (!ProfileContext.Current.CheckPermission("content:site:mng:delete"))
                throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");

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
                        ProcessDeleteCommand(items);

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
            }
        }
        #endregion

        /// <summary>
        /// Processes the delete command.
        /// </summary>
        /// <param name="items">The items.</param>
        void ProcessDeleteCommand(string[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
                if (keys != null)
                {
                    string id = keys[0];

                    // delete selected sites
					Guid siteId = new Guid(id);

					using (TransactionScope scope = new TransactionScope())
					{
						SiteDto dto = CMSContext.Current.GetSiteDto(siteId, true);
						if (dto.Site.Rows.Count > 0)
						{
							// delete menus
							MenuDto menuDto = MenuManager.GetMenuDto(siteId);
							if (menuDto.Menu.Rows.Count > 0)
							{
								foreach (MenuDto.MenuRow menuRow in menuDto.Menu.Rows)
									menuRow.Delete();
							}

							if (menuDto.HasChanges())
								MenuManager.SaveMenuDto(menuDto);

							// delete folders and pages
							int rootId = FileTreeItem.GetRoot(siteId);
							FileTreeItem.DeleteFileItem(rootId);

							//delete site
							dto.Site[0].Delete();
							CMSContext.Current.SaveSite(dto);
						}

						scope.Complete();
					}
                }
            }
        }
    }
}
