using System;
using System.Data;
using System.Collections.Generic;
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
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Manager.Catalog.CommandHandlers
{
	public class CloneSelectedItemsHandler : ICommand
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
						ProcessCloneCommand(items,
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
							String.Format("alert('{0}{1}');", "Failed to copy/move item(s). Error: ", errorMessage), true);
					}
				}
				else
				{
					//((CommandManager)sender).InfoMessage = "No selected items";
					return;
				}
			}
		}

		#endregion

        /// <summary>
        /// Processes the clone command.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <param name="parentCatalogNodeId">The parent catalog node id.</param>
		void ProcessCloneCommand(string[] items, int parentCatalogId, int parentCatalogNodeId)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int id = Int32.Parse(keys[0]);
					string type = keys[1];

					if (type == "Node")
						CloneCatalogNode(parentCatalogId, parentCatalogNodeId, id);
					else // entry
						CloneNodeEntry(parentCatalogId, parentCatalogNodeId, id, parentCatalogId, parentCatalogNodeId);
				}
			}
		}

        /// <summary>
        /// Clones the node entry.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="targetCatalogId">The target catalog id.</param>
        /// <param name="targetCatalogNodeId">The target catalog node id.</param>
		private void CloneNodeEntry(int catalogId, int catalogNodeId, int catalogEntryId, int targetCatalogId, int targetCatalogNodeId)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(catalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
				if (catalogEntryDto.CatalogEntry.Count > 0)
				{
                    if (catalogId <= 0)
                        catalogId = catalogEntryDto.CatalogEntry[0].CatalogId;

                    if (targetCatalogId <= 0)
                        targetCatalogId = catalogId;

					CatalogRelationDto catalogRelationDto = CatalogContext.Current.GetCatalogRelationDto(catalogId, catalogNodeId, catalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry | CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));

					CatalogAssociationDto catalogAssociationDto = CatalogContext.Current.GetCatalogAssociationDtoByEntryId(catalogEntryId);

					CatalogEntryDto newCatalogEntryDto = new CatalogEntryDto();
					newCatalogEntryDto.CatalogEntry.ImportRow(catalogEntryDto.CatalogEntry[0]);
					newCatalogEntryDto.CatalogEntry[0].SetAdded();
					newCatalogEntryDto.CatalogEntry[0].Code = Guid.NewGuid().ToString();

					if (catalogEntryDto.CatalogItemSeo.Count > 0)
					{
						foreach (CatalogEntryDto.CatalogItemSeoRow row in catalogEntryDto.CatalogItemSeo.Rows)
						{
							newCatalogEntryDto.CatalogItemSeo.ImportRow(row);
							newCatalogEntryDto.CatalogItemSeo[newCatalogEntryDto.CatalogItemSeo.Count - 1].SetAdded();
							newCatalogEntryDto.CatalogItemSeo[newCatalogEntryDto.CatalogItemSeo.Count - 1].Uri = Guid.NewGuid().ToString() + ".aspx";
						}
					}

					if (catalogEntryDto.Variation.Count > 0)
					{
						foreach (CatalogEntryDto.VariationRow row in catalogEntryDto.Variation.Rows)
						{
							newCatalogEntryDto.Variation.ImportRow(row);
							newCatalogEntryDto.Variation[newCatalogEntryDto.Variation.Count - 1].SetAdded();
						}
					}

					if (catalogEntryDto.SalePrice.Count > 0)
					{
						foreach (CatalogEntryDto.SalePriceRow row in catalogEntryDto.SalePrice.Rows)
						{
							CatalogEntryDto.SalePriceRow newRow = newCatalogEntryDto.SalePrice.NewSalePriceRow();
							newRow.ItemArray = row.ItemArray;
							newRow.ItemCode = newCatalogEntryDto.CatalogEntry[0].Code;
							newCatalogEntryDto.SalePrice.Rows.Add(newRow);
							//newCatalogEntryDto.SalePrice.ImportRow(row);
							//newCatalogEntryDto.SalePrice[newCatalogEntryDto.SalePrice.Count - 1].ItemCode = newCatalogEntryDto.CatalogEntry[0].Code;
							//newCatalogEntryDto.SalePrice[newCatalogEntryDto.SalePrice.Count - 1].SetAdded();
						}
					}

					if (catalogEntryDto.Inventory.Count > 0)
					{
						foreach (CatalogEntryDto.InventoryRow row in catalogEntryDto.Inventory.Rows)
						{
							newCatalogEntryDto.Inventory.ImportRow(row);
							newCatalogEntryDto.Inventory[newCatalogEntryDto.Inventory.Count - 1].SetAdded();
							newCatalogEntryDto.Inventory[newCatalogEntryDto.Inventory.Count - 1].SkuId = newCatalogEntryDto.CatalogEntry[0].Code;
						}
					}

					if (newCatalogEntryDto.HasChanges())
						CatalogContext.Current.SaveCatalogEntry(newCatalogEntryDto);

					if (newCatalogEntryDto.CatalogEntry.Count > 0)
					{
						CatalogEntryDto.CatalogEntryRow entry = newCatalogEntryDto.CatalogEntry[0];
						int newCatalogEntryId = entry.CatalogEntryId;
						int metaClassId = entry.MetaClassId;

						// load list of MetaFields for MetaClass
						MetaClass metaClass = MetaClass.Load(CatalogContext.MetaDataContext, metaClassId);
						MetaFieldCollection metaFields = metaClass.MetaFields;

						// cycle through each language and get meta objects
                        CatalogContext.MetaDataContext.UseCurrentUICulture = false;
						string[] languages = GetCatalogLanguages(catalogId);
						if (languages != null)
						{
							foreach (string language in languages)
							{
                                CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                CatalogContext.MetaDataContext.Language = language;

                                MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, catalogEntryDto.CatalogEntry[0].CatalogEntryId, metaClassId);

                                MetaObject newMetaObject = MetaObject.NewObject(CatalogContext.MetaDataContext, newCatalogEntryId, metaClassId, FrameworkContext.Current.Profile.UserName);

								foreach (MetaField metaField in metaFields)
								{
									// skip system MetaFields
									if (!metaField.IsUser)
										continue;

									switch (metaField.DataType)
									{
										case MetaDataType.File:
										case MetaDataType.Image:
										case MetaDataType.ImageFile:
											MetaFile metaFile = (MetaFile)metaObject[metaField];
											if (metaFile != null)
												newMetaObject[metaField] = new MetaFile(metaFile.Name, metaFile.ContentType, metaFile.Buffer);
											break;
										default:
											if (metaObject[metaField] != null)
												newMetaObject[metaField] = metaObject[metaField];
											break;
									}
								}
                                newMetaObject.AcceptChanges(CatalogContext.MetaDataContext);
							}
						}
                        CatalogContext.MetaDataContext.UseCurrentUICulture = false;

						CatalogRelationDto newCatalogRelationDto = new CatalogRelationDto();

						foreach (CatalogRelationDto.CatalogEntryRelationRow row in catalogRelationDto.CatalogEntryRelation.Rows)
						{
							if (row.ParentEntryId == catalogEntryId)
							{
								newCatalogRelationDto.CatalogEntryRelation.ImportRow(row);
								newCatalogRelationDto.CatalogEntryRelation[newCatalogRelationDto.CatalogEntryRelation.Count - 1].SetAdded();
								newCatalogRelationDto.CatalogEntryRelation[newCatalogRelationDto.CatalogEntryRelation.Count - 1].ParentEntryId = newCatalogEntryId;
							}
						}

                        if (targetCatalogNodeId > 0)
                        {
                            foreach (CatalogRelationDto.NodeEntryRelationRow row in catalogRelationDto.NodeEntryRelation.Rows)
                            {
                                if (row.CatalogEntryId == catalogEntryId)
                                {
                                    newCatalogRelationDto.NodeEntryRelation.ImportRow(row);
                                    newCatalogRelationDto.NodeEntryRelation[newCatalogRelationDto.NodeEntryRelation.Count - 1].SetAdded();
                                    newCatalogRelationDto.NodeEntryRelation[newCatalogRelationDto.NodeEntryRelation.Count - 1].CatalogId = targetCatalogId;
                                    newCatalogRelationDto.NodeEntryRelation[newCatalogRelationDto.NodeEntryRelation.Count - 1].CatalogNodeId = targetCatalogNodeId;
                                    newCatalogRelationDto.NodeEntryRelation[newCatalogRelationDto.NodeEntryRelation.Count - 1].CatalogEntryId = newCatalogEntryId;
                                }
                            }
                        }

						if (newCatalogRelationDto.HasChanges())
							CatalogContext.Current.SaveCatalogRelationDto(newCatalogRelationDto);

						CatalogAssociationDto newCatalogAssociationDto = new CatalogAssociationDto();

						foreach (CatalogAssociationDto.CatalogAssociationRow row in catalogAssociationDto.CatalogAssociation.Rows)
						{
							newCatalogAssociationDto.CatalogAssociation.ImportRow(row);
							newCatalogAssociationDto.CatalogAssociation[newCatalogAssociationDto.CatalogAssociation.Count - 1].SetAdded();
							newCatalogAssociationDto.CatalogAssociation[newCatalogAssociationDto.CatalogAssociation.Count - 1].CatalogEntryId = newCatalogEntryId;
						}

						foreach (CatalogAssociationDto.CatalogEntryAssociationRow row in catalogAssociationDto.CatalogEntryAssociation.Rows)
						{
							newCatalogAssociationDto.CatalogEntryAssociation.ImportRow(row);
							newCatalogAssociationDto.CatalogEntryAssociation[newCatalogAssociationDto.CatalogEntryAssociation.Count - 1].SetAdded();
							//newCatalogAssociationDto.CatalogEntryAssociation[newCatalogAssociationDto.CatalogEntryAssociation.Count - 1].CatalogEntryId = newCatalogEntryId;
						}

						if (newCatalogAssociationDto.HasChanges())
							CatalogContext.Current.SaveCatalogAssociation(newCatalogAssociationDto);
					}
				}

				scope.Complete();
			}
		}

        /// <summary>
        /// Clones the catalog node.
        /// </summary>
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <param name="parentCatalogNodeId">The parent catalog node id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
		private void CloneCatalogNode(int parentCatalogId, int parentCatalogNodeId, int catalogNodeId)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				CatalogNodeDto catalogNodeDto = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId);
				if (catalogNodeDto.CatalogNode.Count > 0)
				{
					CatalogNodeDto newCatalogNodeDto = new CatalogNodeDto();
					newCatalogNodeDto.CatalogNode.ImportRow(catalogNodeDto.CatalogNode[0]);
					newCatalogNodeDto.CatalogNode[0].SetAdded();
					newCatalogNodeDto.CatalogNode[0].ParentNodeId = parentCatalogNodeId;
					newCatalogNodeDto.CatalogNode[0].Code = Guid.NewGuid().ToString();

					if (catalogNodeDto.CatalogItemSeo.Count > 0)
					{
						foreach (CatalogNodeDto.CatalogItemSeoRow row in catalogNodeDto.CatalogItemSeo.Rows)
						{
							newCatalogNodeDto.CatalogItemSeo.ImportRow(row);
							newCatalogNodeDto.CatalogItemSeo[newCatalogNodeDto.CatalogItemSeo.Count - 1].SetAdded();
							newCatalogNodeDto.CatalogItemSeo[newCatalogNodeDto.CatalogItemSeo.Count - 1].Uri = Guid.NewGuid().ToString() + ".aspx";
						}
					}

					if (newCatalogNodeDto.HasChanges())
						CatalogContext.Current.SaveCatalogNode(newCatalogNodeDto);

					if (newCatalogNodeDto.CatalogNode.Count > 0)
					{
						CatalogNodeDto.CatalogNodeRow node = newCatalogNodeDto.CatalogNode[0];
						int newCatalogNodeId = node.CatalogNodeId;
						int metaClassId = node.MetaClassId;

						// load list of MetaFields for MetaClass
                        MetaClass metaClass = MetaClass.Load(CatalogContext.MetaDataContext, metaClassId);
						MetaFieldCollection metaFields = metaClass.MetaFields;

						// cycle through each language and get meta objects
                        CatalogContext.MetaDataContext.UseCurrentUICulture = false;
						string[] languages = GetCatalogLanguages(parentCatalogId);
						if (languages != null)
						{
							foreach (string language in languages)
							{
                                CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                CatalogContext.MetaDataContext.Language = language;

                                MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, catalogNodeDto.CatalogNode[0].CatalogNodeId, metaClassId);

                                MetaObject newMetaObject = MetaObject.NewObject(CatalogContext.MetaDataContext, newCatalogNodeId, metaClassId, FrameworkContext.Current.Profile.UserName);

								foreach (MetaField metaField in metaFields)
								{
									// skip system MetaFields
									if (!metaField.IsUser)
										continue;

									switch (metaField.DataType)
									{
										case MetaDataType.File:
										case MetaDataType.Image:
										case MetaDataType.ImageFile:
											MetaFile metaFile = (MetaFile)metaObject[metaField];
											if (metaFile != null)
												newMetaObject[metaField] = new MetaFile(metaFile.Name, metaFile.ContentType, metaFile.Buffer);
											break;
										default:
											if (metaObject[metaField] != null)
												newMetaObject[metaField] = metaObject[metaField];
											break;
									}
								}
                                newMetaObject.AcceptChanges(CatalogContext.MetaDataContext);
							}
						}
                        CatalogContext.MetaDataContext.UseCurrentUICulture = false;

						CatalogNodeDto childCatalogNodesDto = CatalogContext.Current.GetCatalogNodesDto(parentCatalogId, catalogNodeId);
						if (childCatalogNodesDto.CatalogNode.Count > 0)
						{
							for (int i = 0; i < childCatalogNodesDto.CatalogNode.Count; i++)
							{
								CloneCatalogNode(parentCatalogId, newCatalogNodeDto.CatalogNode[0].CatalogNodeId, childCatalogNodesDto.CatalogNode[i].CatalogNodeId);
							}
						}

						CatalogEntryDto catalogEntriesDto = CatalogContext.Current.GetCatalogEntriesDto(parentCatalogId, catalogNodeDto.CatalogNode[0].CatalogNodeId);
						if (catalogEntriesDto.CatalogEntry.Count > 0)
						{
							for (int i = 0; i < catalogEntriesDto.CatalogEntry.Count; i++)
							{
								CloneNodeEntry(parentCatalogId, catalogNodeId, catalogEntriesDto.CatalogEntry[i].CatalogEntryId, parentCatalogId, newCatalogNodeDto.CatalogNode[0].CatalogNodeId);
							}
						}
					}
				}

				scope.Complete();
			}
		}

        /// <summary>
        /// Gets the catalog languages.
        /// </summary>
        /// <param name="parentCatalogId">The parent catalog id.</param>
        /// <returns></returns>
		private string[] GetCatalogLanguages(int parentCatalogId)
		{
			CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto(parentCatalogId);

			List<string> list = new List<string>();
			if (catalogDto.Catalog.Count > 0)
			{
				list.Add(catalogDto.Catalog[0].DefaultLanguage);
				if (catalogDto.CatalogLanguage.Count > 0)
					foreach (CatalogDto.CatalogLanguageRow row in catalogDto.CatalogLanguage.Rows)
						list.Add(row.LanguageCode);
			}

			return list.ToArray();
		}
	}
}
