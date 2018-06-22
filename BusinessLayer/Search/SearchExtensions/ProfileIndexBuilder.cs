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
using System.Web.Security;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;


namespace Mediachase.Search.Extensions.Indexers
{
    public class ProfileIndexBuilder : BaseIndexBuilder
    {
        #region ISearchIndexBuilder Members
        /// <summary>
        /// Builds the index.
        /// </summary>
        /// <param name="rebuild">if set to <c>true</c> [rebuild].</param>
        public override void BuildIndex(bool rebuild)
        {
            OnEvent(String.Format("ProfileIndexBuilder Started"), 0);

            IndexBuilder indexer = this.Indexer;
            DateTime lastBuild = DateTime.MinValue;

            // On complete rebuild no need to check date
            if (!rebuild)
                lastBuild = indexer.GetBuildConfig().LastBuildDate;

            double percentage = 0;

            bool incremental = false;
            int allRecords = 0;
            int allCurrentRecord = 0;
            int allRecordsCount = GetTotalRecords();

            // If date is set, we do incremental rebuild
            if (lastBuild != DateTime.MinValue)
                incremental = true;

            int startingRecord = 0;
            // remove deleted items first
            if (incremental)
            {                
                int totalRemoved = 0;

                while (true)
                {
                    int count = 0;
                    LogDto dto = LogManager.GetAppLog("profile", DataRowState.Deleted.ToString(), "ACCOUNT", lastBuild.ToUniversalTime(), startingRecord, 100, ref count);

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
                            OnEvent(String.Format("Removing old profile from index ({1}/{2}) ...", allCurrentRecord, count), percentage);
                        }

                        totalRemoved += indexer.DeleteContent("_id", row.ObjectKey);
                    }

                    startingRecord += 100;
                }

                percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                OnEvent(String.Format("ProfileIndexBuilder Removed {0} records.", totalRemoved), percentage);
            }

            int currentRecord = 0;
            startingRecord = 0;
            int totalIndexed = 0;

            while (true)
            {
                ProfileSearchParameters pars = new ProfileSearchParameters();
                ProfileSearchOptions options = new ProfileSearchOptions();

                options.CacheResults = false;
                options.RecordsToRetrieve = 100;
                options.StartingRecord = startingRecord;

                if (incremental)
                    pars.SqlMetaWhereClause = String.Format("META.Modified > '{0}'", lastBuild.ToUniversalTime().ToString("s"));

                int count = 0;
                Account[] accounts = ProfileContext.Current.FindAccounts(pars, options, out count);
                if (accounts == null)
                    break;

                if (count <= 0)
                    break;

                startingRecord += options.RecordsToRetrieve;

                foreach (Account account in accounts)
                {
                    currentRecord++;
                    allCurrentRecord++;

                    // In case of incremental, check if item already exists and delete it
                    if (incremental)
                    {
                        indexer.DeleteContent("_id", account.PrincipalId.ToString());
                    }

                    totalIndexed += IndexAccount(indexer, account);

                    if (allCurrentRecord % 20 == 0)
                    {
                        percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;
                        OnEvent(String.Format("Indexing profiles ({0}/{1}) ...", allCurrentRecord, allRecordsCount), percentage);
                    }
                }

                if (startingRecord > count)
                    break;
            }

            percentage = ((double)allCurrentRecord / (double)allRecordsCount) * 100;

            allRecords += totalIndexed;

            OnEvent(String.Format("ProfileIndexBuilder Indexed {0} records", allRecords.ToString()), 99);
            OnEvent(String.Format("ProfileIndexBuilder Finished"), 100);
        }

        /// <summary>
        /// Gets the total records.
        /// </summary>
        /// <returns></returns>
        private int GetTotalRecords()
        {
            int numRecords = 0;

            // Get Catalog Nodes
            ProfileSearchParameters pars = new ProfileSearchParameters();
            ProfileSearchOptions options = new ProfileSearchOptions();
            options.CacheResults = false;
            options.RecordsToRetrieve = 1;
            options.StartingRecord = 0;

            int totalCount = 0;
            ProfileContext.Current.FindAccounts(pars, options, out totalCount);
            numRecords += totalCount;

            return numRecords;
        }

        /// <summary>
        /// Indexes the catalog entry dto.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        private int IndexAccount(IndexBuilder indexer, Account account)
        {
            int indexCounter = 0;
            Document doc = new Document();

            // Add constant fields
            doc.Add(new Field("_id", account.PrincipalId.ToString(), Field.Store.YES, Field.Index.UN_TOKENIZED));
            doc.Add(new Field("_providerkey", account.ProviderKey, Field.Store.YES, Field.Index.UN_TOKENIZED));
            doc.Add(new Field("_type", account.Type, Field.Store.YES, Field.Index.UN_TOKENIZED));
            doc.Add(new Field("_metaclass", account.MetaClass.Name, Field.Store.YES, Field.Index.UN_TOKENIZED));

            foreach (MetaField field in account.MetaClass.MetaFields)
            {
                AddField(doc, field, account.GetValues());
            }

            CustomerAddressCollection addresses = account.Addresses;
            if (addresses != null)
            {
                foreach (CustomerAddress address in addresses)
                {
                    foreach (MetaField field in address.MetaClass.MetaFields)
                    {
                        AddField(doc, field, account.GetValues());
                    }
                }
            }

            indexer.AddDocument(doc);

            indexCounter++;

            return indexCounter;
        }
        #endregion
    }
}
