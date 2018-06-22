using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus;
using System.Reflection;

namespace IndexMetaData
{
    class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            Console.WriteLine(String.Format("Mediachase (R) Catalog Index Utility Version {0}.", Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(String.Format("Copyright (C) Mediachase LLC 2008. All rights reserved."));
            Console.WriteLine("");

            if (args.Length == 0)
            {
                Console.WriteLine("Indexing records with no index (pass all to index all records).");
                IndexCatalog("");
            }
            else
            {
                if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Indexing all records.");
                    IndexCatalog("all");
                }
            }            
        }

        /// <summary>
        /// Indexes the catalog.
        /// </summary>
        private static void IndexCatalog(string indexType)
        {
            ICatalogSystem system = CatalogContext.Current;

            // Get catalog lists
            CatalogDto catalogs = system.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
            {
                string catalogName = catalog.Name;

                Console.WriteLine(String.Format("Indexing {0} catalog ...", catalogName));

                int currentRecord = 0;
                int totalIndexed = 0;
                int currentRecordIndex = 0;
                int origRow = Console.CursorTop;
                int origCol = Console.CursorLeft;
                int overallTotalCount = 0;
                while (true)
                {
                    // Get Catalog Nodes
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();
                    options.CacheResults = false;
                    pars.CatalogNames.Add(catalogName);
                    options.RecordsToRetrieve = 500;
                    options.StartingRecord = currentRecord;
                    if (!indexType.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {                        
                        pars.SqlWhereClause = "SerializedData is null";
                    }

                    int totalCount = 0;
                    CatalogEntryDto entryDto = CatalogContext.Current.FindItemsDto(pars, options, ref totalCount, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

                    if (!indexType.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        currentRecord = 0;
                        if (overallTotalCount == 0)
                            overallTotalCount = totalCount;
                    }
                    else
                    {                        
                        if (currentRecord == 0)
                            overallTotalCount = totalCount;

                        currentRecord += options.RecordsToRetrieve;
                    }

                    
                    List<string> languages = new List<string>();

                    languages.Add(catalog.DefaultLanguage);

                    foreach (CatalogDto.CatalogLanguageRow row in catalog.GetCatalogLanguageRows())
                    {
                        languages.Add(row.LanguageCode);
                    }

                    int currentBatchCount = 0;

                    foreach (CatalogEntryDto.CatalogEntryRow row in entryDto.CatalogEntry)
                    {
                        Console.SetCursorPosition(origCol, origRow);
                        Console.WriteLine(String.Format("Indexing {0}/{1} record                  ", currentRecordIndex+1, overallTotalCount));
                        currentBatchCount += IndexCatalogEntryDto(row, languages.ToArray());
                        totalIndexed += currentBatchCount;
                        currentRecordIndex++;
                    }

                    Console.SetCursorPosition(origCol, origRow);
                    Console.WriteLine(String.Format("Saving {0}-{1}/{2} records            ", currentRecordIndex - currentBatchCount + 1, currentRecordIndex, overallTotalCount));
                    system.SaveCatalogEntry(entryDto);

                    // Break the loop if we retrieved all the record
                    if (currentRecord > overallTotalCount || totalCount <= 0)
                        break;
                }

                Console.WriteLine(String.Format("Successfully indexed {0} language records in {1} catalog", totalIndexed.ToString(), catalogName));
            }
        }

        /// <summary>
        /// Indexes the catalog entry dto.
        /// </summary>
        /// <param name="entryRow">The entry row.</param>
        /// <param name="languages">The languages.</param>
        private static int IndexCatalogEntryDto(CatalogEntryDto.CatalogEntryRow entryRow, string[] languages)
        {
            int indexCounter = 0;
            CatalogContext.MetaDataContext.UseCurrentUICulture = false;
            MetaObjectSerialized serialized = new MetaObjectSerialized();
            foreach (string language in languages)
            {
                CatalogContext.MetaDataContext.Language = language;
                MetaObject metaObj = null;
                metaObj = MetaObject.Load(CatalogContext.MetaDataContext, entryRow.CatalogEntryId, entryRow.MetaClassId);

                if (metaObj == null)
                    continue;
                
                serialized.AddMetaObject(language, metaObj);
                indexCounter++;
            }

            entryRow.SerializedData = serialized.BinaryValue;
            CatalogContext.MetaDataContext.UseCurrentUICulture = true;
            return indexCounter;
        }
    }
}
