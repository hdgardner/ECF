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

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
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
							ManagementHelper.GetIntFromQueryString("catalogid"),
							ManagementHelper.GetIntFromQueryString("catalognodeid"));

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
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <param name="parentCatalogNodeId">The parent catalog node id.</param>
		void ProcessDeleteCommand(string[] items, int parentCatalogId, int parentCatalogNodeId)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int id = Int32.Parse(keys[0]);
					string type = keys[1];

                    if (id > 0)
                    {
                        if (String.Compare(type, "Node", true) == 0 || String.Compare(type, "LevelUp", true) == 0)
                        {
                            CatalogNodeDto catalogNodeDto = CatalogContext.Current.GetCatalogNodesDto(parentCatalogId);
                            CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(0, 0, 0, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogNode | CatalogRelationResponseGroup.ResponseGroup.NodeEntry));

                            DeleteNodeRecursive(id, parentCatalogId, ref catalogNodeDto, ref catalogRelationDto);

                            if (catalogRelationDto.HasChanges())
                                CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);

                            if (catalogNodeDto.HasChanges())
                                CatalogContext.Current.SaveCatalogNode(catalogNodeDto);
                        }
                        else // entry
                            DeleteEntryRecursive(id, parentCatalogId, parentCatalogNodeId);
                    }
				}
			}
		}

        /// <summary>
        /// Deletes the node recursive.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <param name="catalogNodeDto">The catalog node dto.</param>
        /// <param name="catalogRelationDto">The catalog relation dto.</param>
		private void DeleteNodeRecursive(int catalogNodeId, int parentCatalogId, ref CatalogNodeDto catalogNodeDto, ref CatalogRelationDto catalogRelationDto)
		{
			CatalogNodeDto.CatalogNodeRow row = catalogNodeDto.CatalogNode.FindByCatalogNodeId(catalogNodeId);
			if (row != null)
			{
				DataRow[] catalogRelations = catalogRelationDto.CatalogNodeRelation.Select(String.Format("CatalogId = {0} AND ParentNodeId = {1}", parentCatalogId, catalogNodeId));
				foreach (DataRow catalogRelation in catalogRelations)
				{
					catalogRelation.Delete();
				}

                DataRow[] nodeEntryRelations = catalogRelationDto.NodeEntryRelation.Select(String.Format("CatalogId = {0} AND CatalogNodeId = {1}", parentCatalogId, catalogNodeId));
                foreach (DataRow nodeEntryRelation in nodeEntryRelations)
                {
                    nodeEntryRelation.Delete();
                }

				row.Delete();

				DataRow[] catalogNodes = catalogNodeDto.CatalogNode.Select(String.Format("ParentNodeId = {0}", catalogNodeId));
				if (catalogNodes.Length > 0)
				{
					foreach (DataRow catalogNode in catalogNodes)
					{
						DeleteNodeRecursive((int)catalogNode["CatalogNodeId"], parentCatalogId, ref catalogNodeDto, ref catalogRelationDto);
					}
				}
			}
		}

        /// <summary>
        /// Deletes the entry recursive.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <param name="parentCatalogNodeId">The parent catalog node id.</param>
		private void DeleteEntryRecursive(int entryId, int parentCatalogId, int parentCatalogNodeId)
		{
			CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(entryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

			if (catalogEntryDto.CatalogEntry.Count > 0)
			{
				bool deleteEntry = true; // flag which will determine if we are deleting an entry

				//Delete NodeEntryRelation rows
				CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(0, 0, entryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
				int totalRelations = catalogRelationDto.NodeEntryRelation.Count;
				foreach (CatalogRelationDto.NodeEntryRelationRow row in catalogRelationDto.NodeEntryRelation.Rows)
				{
					if (row.CatalogId == parentCatalogId && row.CatalogNodeId == parentCatalogNodeId)
					{
						row.Delete();
						totalRelations--;
					}
					else if (parentCatalogId == catalogEntryDto.CatalogEntry[0].CatalogId && parentCatalogNodeId == 0) // delete other catalog relationship if we deleting entry in primary catalog the entry belongs to and we deleting from the very root of catalog
					{
						row.Delete();
						totalRelations--;
					}
				}

				if (catalogRelationDto.HasChanges())
				{
					CatalogContext.Current.SaveCatalogRelationDto(catalogRelationDto);
				}

				// Do not delete if there are more than 1 relationships or if the current catalog is not the primary one
				if (totalRelations > 0 || parentCatalogId != catalogEntryDto.CatalogEntry[0].CatalogId)
					deleteEntry = false;

				if (deleteEntry)
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

					//Delete relations with all sub entries
					CatalogRelationDto relation = CatalogContext.Current.GetCatalogRelationDto(parentCatalogId, parentCatalogNodeId, entryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
					foreach (CatalogRelationDto.CatalogEntryRelationRow relationRow in relation.CatalogEntryRelation)
					{
						relationRow.Delete();
					}
					CatalogContext.Current.SaveCatalogRelationDto(relation);

                    CatalogEntryDto.CatalogEntryRow entryRow = catalogEntryDto.CatalogEntry[0];

                    // Delete inventory if on exists
                    if (entryRow.InventoryRow != null)
                    {
                        entryRow.InventoryRow.Delete();
                    }

					//Delete entry row
                    entryRow.Delete();
					CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);
				}
			}
		}
	}
}
