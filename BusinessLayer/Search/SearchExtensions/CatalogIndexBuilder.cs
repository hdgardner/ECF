using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.MetaDataPlus;
using Lucene.Net.Documents;
using Mediachase.MetaDataPlus.Configurator;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using System.Data;
using Mediachase.Commerce.Core;
using System.Globalization;
using Mediachase.Commerce;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Storage;
using System.Collections;

namespace Mediachase.Search.Extensions.Indexers
{
    public class CatalogIndexBuilder : BaseIndexBuilder
    {
        #region ISearchIndexBuilder Members
        /// <summary>
        /// Builds the index.
        /// </summary>
        /// <param name="rebuild">if set to <c>true</c> [rebuild].</param>
        public override void BuildIndex(bool rebuild)
        {
            OnEvent(String.Format("CatalogIndexBuilder Started"), 0);
            
            IndexBuilder indexer = this.Indexer;
            DateTime lastBuild = DateTime.MinValue;

            // Parameters used to restart build from the previous error
            //string restartCatalogName = String.Empty;
            //int restartRecordKey = 0;

            // On complete rebuild no need to check date
            if(!rebuild)
                lastBuild = indexer.GetBuildConfig().LastBuildDate;

            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            double percentage = 0;

            bool incremental = false;
            int allRecords = 0;
            int allCurrentRecord = 0;
            int allRecordsCount = GetTotalRecords();
            int catalogCount = 1;

            // If date is set, we do incremental rebuild
            if (lastBuild != DateTime.MinValue)
                incremental = true;

            // remove deleted items first
            if (incremental)
            {
                int startingRecord = 0;
                int totalRemoved = 0;

                while (true)
                {
                    int count = 0;
                    LogDto dto = LogManager.GetAppLog("catalog", DataRowState.Deleted.ToString(), "entry", lastBuild.ToUniversalTime(), startingRecord, 100, ref count);

                    // add up to a total number to calculate percentage correctly
                    if (startingRecord == 0)
                        allRecordsCount += count;

                    if (count <= 0)
                        break;

                    foreach (LogDto.ApplicationLogRow row in dto.ApplicationLog)
                    {
                        allCurrentRecord++;
                        if (allCurrentRecord % 20 == 0)
                        {                            
                            percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                            OnEvent(String.Format("Removing old entry from index ({1}/{2}) ...", allCurrentRecord, count), percentage);
                        }

                        totalRemoved += indexer.DeleteContent("_id", row.ObjectKey);
                    }

                    startingRecord += 100;
                }

                percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                OnEvent(String.Format("CatalogIndexBuilder Removed {0} records.", totalRemoved), percentage);
            }

			// Index new or updated items
            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;
                string defaultCurrency = catalog.DefaultCurrency;

                OnEvent(String.Format("Indexing {0} catalog ...", catalogName), percentage);

                int currentRecord = 0;
                int startingRecord = 0;
                int totalIndexed = 0;
                int lastRecordKey = 0;
                catalogCount++;

                while (true)
                {
                    // Get Catalog Nodes
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();

                    options.CacheResults = false;
                    pars.CatalogNames.Add(catalogName);
                    options.RecordsToRetrieve = 100;
                    options.StartingRecord = startingRecord;

                    if(incremental)
                        pars.SqlMetaWhereClause = String.Format("META.Modified > '{0}'", lastBuild.ToUniversalTime().ToString("s"));

                    int count = 0;
                    CatalogEntryDto entryDto = CatalogContext.Current.FindItemsDto(pars, options, ref count, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

                    if (count <= 0)
                        break;

                    startingRecord += options.RecordsToRetrieve;

                    List<string> languages = new List<string>();

                    languages.Add(catalog.DefaultLanguage);

                    foreach (CatalogDto.CatalogLanguageRow row in catalog.GetCatalogLanguageRows())
                    {
                        languages.Add(row.LanguageCode);
                    }

                    foreach (CatalogEntryDto.CatalogEntryRow row in entryDto.CatalogEntry)
                    {
                        currentRecord++;
                        allCurrentRecord++;

                        // Do not index non active entries
                        if (!row.IsActive)
                            continue;

                        // In case of incremental, check if item already exists and delete it
                        if (incremental)
                        {
                            indexer.DeleteContent("_id", row.CatalogEntryId.ToString());
                        }

                        try
                        {
                            lastRecordKey = row.CatalogEntryId;
                            totalIndexed += IndexCatalogEntryDto(indexer, row, defaultCurrency, languages.ToArray());
                        }
                        catch (Exception)
                        {
                            percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                            OnEvent(String.Format("Failed to index catalog entry {0}({1}) in {2}.", row.Name, row.CatalogEntryId, catalogName), percentage);
                            throw;
                        }

                        if (allCurrentRecord % 20 == 0)
                        {
                            percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                            OnEvent(String.Format("Indexing {0} catalog entries ({1}/{2}) ...", catalogName, allCurrentRecord, allRecordsCount), percentage);
                        }
                    }

                    // Save index every 500 records
                    if (allCurrentRecord % 500 == 0)
                    {
                        percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                        OnEvent(String.Format("Saving {0} catalog index file.", catalogName), percentage);
                        Build config = indexer.GetBuildConfig();

                        // Preserve the information needed to restart the indexing on the exact same location as before
                        BuildProperty prop1 = new BuildProperty();
                        prop1.name = "StartingRecord";
                        prop1.value = startingRecord.ToString();

                        BuildProperty prop2 = new BuildProperty();
                        prop2.name = "LastRecordKey";
                        prop2.value = lastRecordKey.ToString();

                        BuildProperty prop3 = new BuildProperty();
                        prop3.name = "CatalogName";
                        prop3.value = catalogName;

                        config.Properties = new BuildProperty[] { prop1, prop2, prop3 };
                        indexer.SaveBuild(Status.Started);
                        indexer.Close();
                    }

                    if (startingRecord > count)
                        break;
                }

                percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                catalogCount++;

                allRecords += totalIndexed;
            }

            OnEvent(String.Format("CatalogIndexBuilder Indexed {0} language records in {1} catalog(s)", allRecords.ToString(), (catalogCount - 1).ToString()), 99);
            OnEvent(String.Format("CatalogIndexBuilder Finished"), 100);
        }

        /// <summary>
        /// Gets the total records.
        /// </summary>
        /// <returns></returns>
        private int GetTotalRecords()
        {
            int numRecords = 0;
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();
            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                // Get Catalog Nodes
                CatalogSearchParameters pars = new CatalogSearchParameters();
                CatalogSearchOptions options = new CatalogSearchOptions();
                options.CacheResults = false;
                pars.CatalogNames.Add(catalogName);
                options.RecordsToRetrieve = 1;
                options.StartingRecord = 0;

                int totalCount = 0;
                CatalogEntryDto entryDto = CatalogContext.Current.FindItemsDto(pars, options, ref totalCount);
                numRecords += totalCount;
            }

            return numRecords;
        }

        /// <summary>
        /// Indexes the catalog entry dto.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <param name="entryRow">The entry row.</param>
        /// <param name="defaultCurrency">The default currency.</param>
        /// <param name="languages">The languages.</param>
        /// <returns></returns>
        private int IndexCatalogEntryDto(IndexBuilder indexer, CatalogEntryDto.CatalogEntryRow entryRow, string defaultCurrency, string[] languages)
        {
            int indexCounter = 0;
            CatalogContext.MetaDataContext.UseCurrentUICulture = false;

            // Import categories
            CatalogRelationDto relationDto = CatalogContext.Current.GetCatalogRelationDto(0, 0, entryRow.CatalogEntryId, "", new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));

            foreach (string language in languages)
            {
                Document doc = new Document();

                // Add constant fields
                doc.Add(new Field("_id", entryRow.CatalogEntryId.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
                doc.Add(new Field("Code", entryRow.Code, Field.Store.YES, Field.Index.UN_TOKENIZED));
                doc.Add(new Field("_content", entryRow.Code, Field.Store.NO, Field.Index.TOKENIZED));
                doc.Add(new Field("Name", entryRow.Name, Field.Store.YES, Field.Index.TOKENIZED));
                doc.Add(new Field("_content", entryRow.Name, Field.Store.NO, Field.Index.TOKENIZED));
                doc.Add(new Field("_lang", language, Field.Store.YES, Field.Index.UN_TOKENIZED));
                doc.Add(new Field("StartDate", entryRow.StartDate.ToString("s"), Field.Store.YES, Field.Index.UN_TOKENIZED));
                doc.Add(new Field("EndDate", entryRow.EndDate.ToString("s"), Field.Store.YES, Field.Index.UN_TOKENIZED));
                doc.Add(new Field("_classtype", entryRow.ClassTypeId, Field.Store.YES, Field.Index.UN_TOKENIZED));
                AddPriceFields(doc, entryRow, defaultCurrency);

                bool sortOrderAdded = false;
                foreach (CatalogRelationDto.NodeEntryRelationRow relation in relationDto.NodeEntryRelation)
                {
                    CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto(relation.CatalogId);
                    string catalogName = String.Empty;
                    if (catalogDto != null && catalogDto.Catalog != null && catalogDto.Catalog.Count > 0)
                        catalogName = catalogDto.Catalog[0].Name;
                    else
                        continue;

                    CatalogNodeDto catalogNodeDto = CatalogContext.Current.GetCatalogNodeDto(relation.CatalogNodeId);
                    string catalogNodeCode = String.Empty;
                    if (catalogNodeDto != null && catalogNodeDto.CatalogNode != null && catalogNodeDto.CatalogNode.Count > 0)
                        catalogNodeCode = catalogNodeDto.CatalogNode[0].Code;
                    else
                        continue;

                    BuildPath(doc, catalogDto, catalogNodeDto, 0);

                    /*
                    BuildPath(doc, entryRow.CatalogId, relation.CatalogNodeId);
                    string path = BuildPath(entryRow.CatalogId, relation.CatalogNodeId);
                    doc.Add(new Field(String.Format("_outline"), path, Field.Store.YES, Field.Index.UN_TOKENIZED));
                     * */
                    doc.Add(new Field(String.Format("_sortorder-{0}-{1}", catalogName, catalogNodeCode), relation.SortOrder.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
                    if (!sortOrderAdded && entryRow.CatalogId == relation.CatalogId) // add default sort order, which will be the first node added in the default catalog
                    {
                        doc.Add(new Field(String.Format("_sortorder"), relation.SortOrder.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
                        sortOrderAdded = true;
                    }
                }

                CatalogContext.MetaDataContext.Language = language;

                if (entryRow.MetaClassId != 0)
                {
                    // load list of MetaFields for MetaClass
                    MetaClass metaClass = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, entryRow.MetaClassId);

                    if (metaClass != null)
                    {
                        MetaFieldCollection mfs = metaClass.MetaFields;
                        if (mfs != null)
                        {
                            //MetaObject metaObj = null;
                            //metaObj = MetaObject.Load(CatalogContext.MetaDataContext, entryRow.CatalogEntryId, entryRow.MetaClassId);

                            Hashtable hash = ObjectHelper.GetMetaFieldValues(entryRow);

                            if (hash != null)
                            {
                                foreach (MetaField field in mfs)
                                {
                                    AddField(doc, field, hash);
                                }
                            }
                        }
                    }

                    doc.Add(new Field("_metaclass", metaClass.Name, Field.Store.YES, Field.Index.UN_TOKENIZED));
                }
                indexer.AddDocument(doc);

                indexCounter++;
            }

            CatalogContext.MetaDataContext.UseCurrentUICulture = true;
            return indexCounter;
        }

        /// <summary>
        /// Builds the path.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="catalog">The catalog.</param>
        /// <param name="catalogNode">The catalog node.</param>
        /// <param name="level">The level.</param>
        private void BuildPath(Document doc, CatalogDto catalog, CatalogNodeDto catalogNode, int level)
        {
            CatalogNodeDto node = catalogNode;
            doc.Add(new Field("_catalog", catalog.Catalog[0].Name, Field.Store.YES, Field.Index.UN_TOKENIZED));
            doc.Add(new Field("_node", catalogNode.CatalogNode[0].Code, Field.Store.YES, Field.Index.UN_TOKENIZED));
            doc.Add(new Field("_level", level.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
            level++;

            CatalogRelationDto relationDto = CatalogContext.Current.GetCatalogRelationDto(catalog.Catalog[0].CatalogId, catalogNode.CatalogNode[0].CatalogNodeId, 0, "", new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogNode));

            foreach (CatalogRelationDto.CatalogNodeRelationRow relation in relationDto.CatalogNodeRelation)
            {
                CatalogNodeDto catalogNodeDto = CatalogContext.Current.GetCatalogNodeDto(relation.ParentNodeId);
                CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto(relation.CatalogId);
                BuildPath(doc, catalogDto, catalogNodeDto, level);
            }

            string path = String.Format("{0}/{1}", catalog.Catalog[0].Name, node.CatalogNode[0].Code);
            int catalogNodeId = node.CatalogNode[0].ParentNodeId;
            while ((node = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId)) != null)
            {
                if (node.CatalogNode == null || node.CatalogNode.Count == 0)
                    break;
                
                path = String.Format("{0}/{1}", path, node.CatalogNode[0].Code);
                catalogNodeId = node.CatalogNode[0].ParentNodeId;
                if (catalogNodeId == 0)
                    break;
            }

            doc.Add(new Field(String.Format("_outline"), path, Field.Store.YES, Field.Index.UN_TOKENIZED));
        }

        /// <summary>
        /// Adds the price fields.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="entryRow">The entry row.</param>
        /// <param name="defaultCurrency">The default currency.</param>
        private void AddPriceFields(Document doc, CatalogEntryDto.CatalogEntryRow entryRow, string defaultCurrency)
        {
            CatalogEntryDto.VariationRow[] varRows = entryRow.GetVariationRows();
            if (varRows.Length > 0)
            {
                if (!varRows[0].IsListPriceNull())
                {
                    doc.Add(new Field(String.Format("ListPrice{0}", defaultCurrency), ConvertStringToSortable(varRows[0].ListPrice), Field.Store.NO, Field.Index.UN_TOKENIZED));
                    doc.Add(new Field(String.Format("ListPrice{0}-Value", defaultCurrency), varRows[0].ListPrice.ToString(), Field.Store.YES, Field.Index.NO));
                }
            }

            CatalogEntryDto.SalePriceRow[] saleRows = entryRow.GetSalePriceRows();
            if (saleRows != null)
            {
                // We only get sale price which is assigned to all customers, and has no quantity restrictions
                foreach (CatalogEntryDto.SalePriceRow priceRow in saleRows)
                {
                    // Check inventory first
                    if (priceRow.MinQuantity > 0)
                        continue; // didn't meet min quantity requirements

                    // Check dates
                    if (priceRow.StartDate > FrameworkContext.Current.CurrentDateTime || priceRow.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue; // falls outside of acceptable range

                    if ((SaleType.TypeKey)priceRow.SaleType == SaleType.TypeKey.AllCustomers) // no need to check, applies to everyone
                    {
                        doc.Add(new Field(String.Format("SalePrice{0}", priceRow.Currency), ConvertStringToSortable(priceRow.UnitPrice), Field.Store.NO, Field.Index.UN_TOKENIZED));
                        doc.Add(new Field(String.Format("SalePrice{0}-Value", priceRow.Currency), priceRow.UnitPrice.ToString(), Field.Store.YES, Field.Index.NO));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="entryRow">The entry row.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        private decimal GetPrice(CatalogEntryDto.CatalogEntryRow entryRow, string defaultCurrency, string languageCode)
        { 
            string currencyCode = new RegionInfo(languageCode).ISOCurrencySymbol;
            decimal price = -1;
            CatalogEntryDto.VariationRow[] varRows = entryRow.GetVariationRows();
            if (varRows.Length > 0 && defaultCurrency.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
            {
                price = varRows[0].ListPrice;
            }

            CatalogEntryDto.SalePriceRow[] saleRows = entryRow.GetSalePriceRows();
            if (saleRows != null)
            {
                // We only get sale price which is assigned to all customers, and has no quantity restrictions
                foreach (CatalogEntryDto.SalePriceRow priceRow in saleRows)
                {
                    if (!priceRow.Currency.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Check inventory first
                    if (priceRow.MinQuantity > 0)
                        continue; // didn't meet min quantity requirements

                    // Check dates
                    if (priceRow.StartDate > FrameworkContext.Current.CurrentDateTime || priceRow.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue; // falls outside of acceptable range

                    if ((SaleType.TypeKey)priceRow.SaleType == SaleType.TypeKey.AllCustomers) // no need to check, applies to everyone
                    {
                        price = priceRow.UnitPrice;
                        break;
                    }
                }
            }

            return price;
        }

        /// <summary>
        /// Builds the path.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        private string BuildPath(int catalogId, int catalogNodeId)
        {
            CatalogNodeDto node = null;
            string path = CatalogContext.Current.GetCatalogDto(catalogId).Catalog[0].Name;

            while ((node = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId)) != null)
            {
                path = String.Format("{0}/{1}", path, node.CatalogNode[0].Code);
                catalogNodeId = node.CatalogNode[0].ParentNodeId;
                if (catalogNodeId == 0)
                    break;
            }

            return path;
        }


        /// <summary>
        /// Builds the path.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        private void BuildPath(Document doc, int catalogId, int catalogNodeId)
        {
            int level = 0;
            CatalogNodeDto node = null;
            string code = CatalogContext.Current.GetCatalogDto(catalogId).Catalog[0].Name;
            doc.Add(new Field("_catalog", code, Field.Store.YES, Field.Index.UN_TOKENIZED));

            while ((node = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId)) != null)
            {
                level++;
                doc.Add(new Field("_node", node.CatalogNode[0].Code, Field.Store.YES, Field.Index.UN_TOKENIZED));
                catalogNodeId = node.CatalogNode[0].ParentNodeId;
                if (catalogNodeId == 0)
                    break;
            }

            doc.Add(new Field("_level", level.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
        }
        #endregion
    }
}
