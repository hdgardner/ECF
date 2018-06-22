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
using Mediachase.Cms.Pages;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
	public class PageLanguageHandler : ICommand
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

				int error = 0;
				string errorMessage = String.Empty;

				try
				{
					string[] items = null;

					if (cp.CommandArguments.ContainsKey(EcfListView.GridCommandParameterName) &&
						Boolean.Parse(cp.CommandArguments[EcfListView.GridCommandParameterName]))
					{
						// process command from grid (delete only one item)
						string primaryKeyId = cp.CommandArguments["primaryKeyId"];
						items = new string[] { primaryKeyId };
					}
					else
					{
						// get checked items and process batch delete command (from toolbar)
						string gridId = cp.CommandArguments["GridId"];
						items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);
						ManagementHelper.SetBindGridFlag(gridId);
					}

					if (items != null)
						ProcessLanguageCommand(items);
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
						String.Format("alert('{0}{1}');", "Failed to copy item(s). Error: ", errorMessage), true);
				}
			}
		}
		#endregion

        /// <summary>
        /// Processes the language command.
        /// </summary>
        /// <param name="items">The items.</param>
		void ProcessLanguageCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int folderId = Int32.Parse(keys[0]);
                    PopulateLanguagesRecursive(folderId);
				}
			}
		}

        /// <summary>
        /// Populates the languages recursive.
        /// </summary>
        /// <param name="folderId">The folder id.</param>
        private void PopulateLanguagesRecursive(int folderId)
        {
            Guid siteId = Guid.Empty;

            DataTable item = FileTreeItem.GetItemByIdDT(folderId);

            // We only care about pages
            if (item != null && item.Rows.Count > 0)
            {
                if ((bool)item.Rows[0]["IsFolder"])
                {
                    DataTable childItems = FileTreeItem.LoadItemByFolderIdDT(folderId);
                    foreach (DataRow childItem in childItems.Rows)
                    {
                        PopulateLanguagesRecursive((int)childItem["PageId"]);
                    }
                }
            }

            siteId = (Guid)item.Rows[0]["SiteId"];
            PageDocument pageDocument = null;

            //GET PUBLISHED VERSION
            int statusId = WorkflowStatus.GetLast();
            DataTable versionsTable = PageVersion.GetVersionByStatusIdDT(folderId, statusId);

            // Find page document
            int templateId = 0;
            int stateId = 0;
            int langId = 0;

            if (versionsTable != null && versionsTable.Rows.Count > 0)
            {
                foreach (DataRow versionRow in versionsTable.Rows)
                {
                    int versionId = 0;
                    int.TryParse(versionRow["VersionId"].ToString(), out versionId);
                    templateId = Int32.Parse(versionRow["templateid"].ToString());
                    PageDocument.PersistentDocumentStorage = new SqlPageDocumentStorageProvider();
                    pageDocument = PageDocument.Open(versionId, OpenMode.View, Guid.Empty);
                    stateId = Int32.Parse(versionRow["stateid"].ToString());
                    langId = Int32.Parse(versionRow["LangId"].ToString());

                    // Found a non empty page document
                    if (pageDocument != null)
                        break;
                }
            }

            // Get list of languages
            SiteDto siteDto = SiteManager.GetSite(siteId);

            foreach (SiteDto.SiteLanguageRow langRow in siteDto.SiteLanguage)
            {
                IDataReader reader = Language.GetLangByName(langRow.LanguageCode);
                if (reader.Read())
                {
                    int currentLangId = (int)reader["LangId"];

                    // We skip the default page document language
                    if (currentLangId == langId)
                    {
                        reader.Close();
                        continue;
                    }

                    // If we already have a specified language define, skip it
                    IDataReader versionReader = PageVersion.GetVersionByLangId(folderId, langId);
                    if (!versionReader.Read())
                    {
                        versionReader.Close();
                        continue;
                    }

                    int versionId = PageVersion.AddPageVersion(folderId, templateId, currentLangId, ProfileContext.Current.UserId, stateId, String.Empty);
                    PageDocument.PersistentDocumentStorage = new SqlPageDocumentStorageProvider();
                    PageDocument.PersistentDocumentStorage.Save(pageDocument, versionId, ProfileContext.Current.UserId);
                    versionReader.Close();
                }
                reader.Close();
            }
        }
	}
}