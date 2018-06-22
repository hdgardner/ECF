using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using System.Web.Security;
using Mediachase.Commerce.Catalog.Security;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Data;

namespace Mediachase.Commerce.Catalog.Impl
{
    internal class CatalogContextImpl : ICatalogSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CatalogContextImpl(MetaDataContext context)
        {
            // Perform auto configuration
            if (CatalogConfiguration.Instance.AutoConfigure)
            {
                // Check if we already configured meta data
                MetaClass metaClass = MetaClass.Load(context, "CatalogEntry");

                // Setup meta data
                if (metaClass == null)
                    CatalogConfiguration.ConfigureMetaData();
            }
        }

        #region Public Methods
        #region Catalog Methods
        /// <summary>
        /// Gets the catalogs.
        /// </summary>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public SiteCatalogs GetCatalogs(CatalogResponseGroup responseGroup, Guid siteId)
        {
            return CatalogManager.GetCatalogs(siteId, responseGroup);
        }

        /// <summary>
        /// Gets the catalogs.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public SiteCatalogs GetCatalogs(Guid siteId)
        {
            return CatalogManager.GetCatalogs(siteId, new CatalogResponseGroup());
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(Guid siteId, CatalogResponseGroup responseGroup)
        {
            return CatalogManager.GetCatalogDto(siteId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(Guid siteId)
        {
            return CatalogManager.GetCatalogDto(siteId, new CatalogResponseGroup());
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(int catalogId, CatalogResponseGroup responseGroup)
        {
            return CatalogManager.GetCatalogDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(int catalogId)
        {
            return CatalogManager.GetCatalogDto(catalogId, new CatalogResponseGroup());
        }

        /// <summary>
        /// Saves the catalog.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalog(CatalogDto dto)
        {
            CatalogManager.SaveCatalog(dto);
        }

        /// <summary>
        /// Deletes the catalog.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        public void DeleteCatalog(int catalogId)
        {
            CatalogManager.DeleteCatalog(catalogId);
        }
        #endregion

        #region Dictionary Methods
        #endregion

        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options, CatalogEntryResponseGroup responseGroup)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogEntryManager.FindItems(search, responseGroup);
        }

        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogEntryManager.FindItems(search, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto FindItemsDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount, CatalogEntryResponseGroup responseGroup)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogEntryManager.FindItemsDto(search, ref recordsCount, responseGroup);
        }

        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <returns></returns>
        public CatalogEntryDto FindItemsDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogEntryManager.FindItemsDto(search, ref recordsCount, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Finds the nodes dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto FindNodesDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount, CatalogNodeResponseGroup responseGroup)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogNodeManager.FindNodesDto(search, ref recordsCount, responseGroup);
        }

        /// <summary>
        /// Finds the nodes dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <returns></returns>
        public CatalogNodeDto FindNodesDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount)
        {
            CatalogSearch search = new CatalogSearch();
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            return CatalogNodeManager.FindNodesDto(search, ref recordsCount, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public DataTable FindCatalogItemsTable(ItemSearchParameters parameters)
		{
			return CatalogNodeManager.GetCatalogItemsTable(parameters, new CatalogNodeResponseGroup());
		}

        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public DataTable FindCatalogItemsTable(ItemSearchParameters parameters, CatalogNodeResponseGroup responseGroup)
		{
            return CatalogNodeManager.GetCatalogItemsTable(parameters, responseGroup);
		}

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <returns></returns>
        public CatalogDto GetCatalogDto()
        {
            return CatalogManager.GetCatalogDto(Guid.Empty, new CatalogResponseGroup());
        }

        #endregion

        #region CatalogNodes methods
        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogId, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogId, parentNodeId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogName, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogName, parentNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, string parentNodeCode)
        {
            return CatalogNodeManager.GetCatalogNodesDto(catalogName, parentNodeCode, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodeDto(catalogNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(int catalogNodeId)
        {
            return CatalogNodeManager.GetCatalogNodeDto(catalogNodeId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string code, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodeDto(code, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string code)
        {
            return CatalogNodeManager.GetCatalogNodeDto(code, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodeDto(uri, languageCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode)
        {
            return CatalogNodeManager.GetCatalogNodeDto(uri, languageCode, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Saves the catalog node.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogNode(CatalogNodeDto dto)
        {
            CatalogNodeManager.SaveCatalogNode(dto);
        }

        /// <summary>
        /// Deletes the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogId">The catalog id.</param>
        public void DeleteCatalogNode(int catalogNodeId, int catalogId)
        {
            CatalogNodeManager.DeleteCatalogNode(catalogNodeId, catalogId);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogName, parentNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, string parentNodeCode)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogName, parentNodeCode, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogId, parentNodeId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogId, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogName, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNodes(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(int catalogNodeId)
        {
            return CatalogNodeManager.GetCatalogNode(catalogNodeId, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNode(catalogNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string code)
        {
            return CatalogNodeManager.GetCatalogNode(code, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string code, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNode(code, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string uri, string languageCode)
        {
            return CatalogNodeManager.GetCatalogNode(uri, languageCode, new CatalogNodeResponseGroup());
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string uri, string languageCode, CatalogNodeResponseGroup responseGroup)
        {
            return CatalogNodeManager.GetCatalogNode(uri, languageCode, responseGroup);
        }
        #endregion

        #region CatalogEntry methods

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogId, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogId, parentNodeId, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogName, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogName, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogName, parentNodeId, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries by node dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesByNodeDto(string catalogName, string parentNodeCode, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesByCatalogDto(catalogName, parentNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries by node dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesByNodeDto(string catalogName, string parentNodeCode)
        {
            return CatalogEntryManager.GetCatalogEntriesByCatalogDto(catalogName, parentNodeCode, new CatalogEntryResponseGroup());
        }


        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogId, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int parentEntryId, string entryType, string relationType, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(parentEntryId, entryType, relationType, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int parentEntryId, string entryType, string relationType)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(parentEntryId, entryType, relationType, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string name, string entryType)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(name, entryType, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string name, string entryType, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(name, entryType, responseGroup);
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName)
        {
            return CatalogEntryManager.GetAssociatedCatalogEntriesDto(parentEntryId, associationName, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetAssociatedCatalogEntriesDto(parentEntryId, associationName, responseGroup);
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName)
        {
            return CatalogEntryManager.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(int catalogEntryId)
        {
            return CatalogEntryManager.GetCatalogEntryDto(catalogEntryId, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntryDto(catalogEntryId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode)
        {
            return CatalogEntryManager.GetCatalogEntryDto(catalogEntryCode, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntryDto(catalogEntryCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntryByUriDto(uri, languageCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode)
        {
            return CatalogEntryManager.GetCatalogEntryByUriDto(uri, languageCode, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Saves the catalog entry.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogEntry(CatalogEntryDto dto)
        {
            CatalogEntryManager.SaveCatalogEntry(dto);
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public void DeleteCatalogEntry(int entryId, bool recursive)
        {
            CatalogEntryManager.DeleteCatalogEntry(entryId, recursive);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntry(catalogEntryId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(int catalogEntryId)
        {
            return CatalogEntryManager.GetCatalogEntry(catalogEntryId, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(string code, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntry(code, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(string code)
        {
            return CatalogEntryManager.GetCatalogEntry(code, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entry GetCatalogEntryByUri(string uri, string languageCode, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntryByUri(uri, languageCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public Entry GetCatalogEntryByUri(string uri, string languageCode)
        {
            return CatalogEntryManager.GetCatalogEntryByUri(uri, languageCode, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogCode">The catalog code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(string catalogName, string catalogCode, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogName, catalogCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogCode">The catalog code.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(string catalogName, string catalogCode)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogName, catalogCode, new CatalogEntryResponseGroup());
        }


        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogEntries, cacheResults, cacheTimeout, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogEntries, cacheResults, cacheTimeout, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogEntries, false, new TimeSpan(), new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntriesDto(catalogEntries, false, new TimeSpan(), responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogEntries, cacheResults, cacheTimeout, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogEntries, cacheResults, cacheTimeout, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogEntries, false, new TimeSpan(), new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries, CatalogEntryResponseGroup responseGroup)
        {
            return CatalogEntryManager.GetCatalogEntries(catalogEntries, false, new TimeSpan(), responseGroup);
        }
        #endregion

        #region CatalogRelation methods
        /// <summary>
        /// Gets the catalog relation dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogRelationDto GetCatalogRelationDto(int catalogId, int catalogNodeId, int catalogEntryId, string groupName, CatalogRelationResponseGroup responseGroup)
        {
            return CatalogRelationManager.GetCatalogRelationDto(catalogId, catalogNodeId, catalogEntryId, groupName, responseGroup);
        }

        public CatalogRelationDto GetCatalogRelationDto(string assetKey)
        {
            return CatalogRelationManager.GetCatalogRelationDto(assetKey);
        }

        /// <summary>
        /// Saves the catalog relation dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogRelationDto(CatalogRelationDto dto)
        {
            CatalogRelationManager.SaveCatalogRelation(dto);
        }

        /*
        public CatalogRelationDto GetCatalogEntryRelatedNodesDto(int catalogEntryId)
        {
            //return CatalogRelationManager.GetCatalogRelationDto(0, 0, catalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry);
        }
         * */
        #endregion

		#region CatalogAssociation methods
        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        /// <returns></returns>
		public CatalogAssociationDto GetCatalogAssociationDto(int catalogAssociationId)
		{
			return CatalogAssociationManager.GetCatalogAssociationDto(catalogAssociationId);
		}

        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationName">Name of the catalog association.</param>
        /// <returns></returns>
		public CatalogAssociationDto GetCatalogAssociationDto(string catalogAssociationName)
		{
			return CatalogAssociationManager.GetCatalogAssociationDto(catalogAssociationName);
		}

        /// <summary>
        /// Gets the catalog association dto by entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
		public CatalogAssociationDto GetCatalogAssociationDtoByEntryId(int catalogEntryId)
		{
			return CatalogAssociationManager.GetCatalogAssociationDtoByEntryId(catalogEntryId);
		}

        /// <summary>
        /// Gets the catalog association dto by entry code.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
		public CatalogAssociationDto GetCatalogAssociationDtoByEntryCode(int catalogId, string catalogEntryCode)
		{
			return CatalogAssociationManager.GetCatalogAssociationDtoByEntryCode(catalogId, catalogEntryCode);
		}

        /// <summary>
        /// Saves the catalog association.
        /// </summary>
        /// <param name="dto">The dto.</param>
		public void SaveCatalogAssociation(CatalogAssociationDto dto)
		{
			CatalogAssociationManager.SaveCatalogAssociation(dto);
		}
		#endregion

        #region Dictionary Methods
		/// <summary>
		/// Gets merchants.
		/// </summary>
		/// <returns></returns>
		public CatalogEntryDto GetMerchantsDto()
		{
			return CatalogEntryManager.GetMerchants();
		}

        /// <summary>
        /// Gets the currency dto.
        /// </summary>
        /// <returns></returns>
        public CurrencyDto GetCurrencyDto()
        {
            return CurrencyManager.GetCurrencyDto();
        }

        /// <summary>
        /// Saves the currency.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCurrency(CurrencyDto dto)
        {
            CurrencyManager.SaveCurrency(dto);
        }
        #endregion

    }
}
