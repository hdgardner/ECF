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
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
{
	public class CatalogEntryDeleteHandler : ICommand
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

				//NameValueCollection qs = ((Control)sender).Page.Request.QueryString;
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
					int id = Int32.Parse(keys[0]);

					DeleteEntryRecursive(id, 0);
				}
			}
		}

        /// <summary>
        /// Deletes the entry recursive.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="parentCatalogId">The parent catalog id.</param>
		private void DeleteEntryRecursive(int entryId, int parentCatalogId)
		{
			CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(entryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

			if (catalogEntryDto.CatalogEntry.Count > 0)
			{
				//Delete NodeEntryRelation rows
				CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(0, 0, entryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
				foreach (CatalogRelationDto.NodeEntryRelationRow row in catalogRelationDto.NodeEntryRelation.Rows)
				{
					if (row.CatalogId == parentCatalogId)
						row.Delete();
				}

				if (catalogRelationDto.HasChanges())
				{
					CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
				}
				else //if NodeEntryRelation doesn't exist delete entry
				{
					//Delete CatalogEntryAssociation rows
					foreach (CatalogEntryDto.CatalogAssociationRow catalogAssociationRow in catalogEntryDto.CatalogAssociation)
					{
						CatalogAssociationDto catalogAssociationDto = FrameworkContext.Current.CatalogSystem.GetCatalogAssociationDto(catalogAssociationRow.CatalogAssociationId);
						foreach (CatalogAssociationDto.CatalogEntryAssociationRow itemCatalogEntryAssociation in catalogAssociationDto.CatalogEntryAssociation)
						{
							itemCatalogEntryAssociation.Delete();
						}

						if (catalogAssociationDto.HasChanges())
							CatalogContext.Current.SaveCatalogAssociation(catalogAssociationDto);
					}

					//Delete child entry rows
					CatalogEntryDto childrenDto = CatalogContext.Current.GetCatalogEntriesDto(entryId, String.Empty, String.Empty);
					foreach (CatalogEntryDto.CatalogEntryRow row in childrenDto.CatalogEntry)
						DeleteEntryRecursive(row.CatalogEntryId, 0);

					//Delete entry row
					catalogEntryDto.CatalogEntry[0].Delete();
					CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);
				}
			}
		}
	}
}