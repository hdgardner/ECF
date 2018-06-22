using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using System.Data;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;

namespace Mediachase.Commerce.Catalog
{
    /// <summary>
    /// Provides the methods necessary for the ICatalogSystem.
    /// </summary>
    [ServiceContract(Namespace="urn:services.mediachase.com/ecf/50")]
    public interface ICatalogSystem
    {
        #region Catalog Methods
        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogDtoBySiteIdEx")]
        CatalogDto GetCatalogDto(Guid siteId, CatalogResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogDtoBySiteId")]
        CatalogDto GetCatalogDto(Guid siteId);

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogDto")]
        CatalogDto GetCatalogDto();

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogDtoByCatalogIdEx")]
        CatalogDto GetCatalogDto(int catalogId, CatalogResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogDtoByCatalogId")]
        CatalogDto GetCatalogDto(int catalogId);

        /// <summary>
        /// Saves the catalog.
        /// </summary>
        /// <param name="dto">The dto.</param>
        [OperationContract]
        void SaveCatalog(CatalogDto dto);

        /// <summary>
        /// Deletes the catalog.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        [OperationContract(Name = "DeleteCatalog")]
        void DeleteCatalog(int catalogId);

        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindItems")]
        Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options);
        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindItemsEx")]
        Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options, CatalogEntryResponseGroup responseGroup);

        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindItemsDto")]
        CatalogEntryDto FindItemsDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount);
        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindItemsDtoEx")]
        CatalogEntryDto FindItemsDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount, CatalogEntryResponseGroup responseGroup);

        /// <summary>
        /// Finds the nodes dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindNodesDto")]
        CatalogNodeDto FindNodesDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount);
        /// <summary>
        /// Finds the nodes dto.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "FindNodesDtoEx")]
        CatalogNodeDto FindNodesDto(CatalogSearchParameters parameters, CatalogSearchOptions options, ref int recordsCount, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
		[OperationContract(Name = "GetCatalogItemsTable")]
        DataTable FindCatalogItemsTable(ItemSearchParameters parameters);
        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogItemsTableEx")]
        DataTable FindCatalogItemsTable(ItemSearchParameters parameters, CatalogNodeResponseGroup responseGroup);
        #endregion

        #region CatalogNode Methods
        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesDto")]
        CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodes")]
        CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesDtoEx")]
        CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesEx")]
        CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogIdDto")]
        CatalogNodeDto GetCatalogNodesDto(int catalogId);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogId")]
        CatalogNodes GetCatalogNodes(int catalogId);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogIdDtoEx")]
        CatalogNodeDto GetCatalogNodesDto(int catalogId, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogIdEx")]
        CatalogNodes GetCatalogNodes(int catalogId, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameDto")]
        CatalogNodeDto GetCatalogNodesDto(string catalogName);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogName")]
        CatalogNodes GetCatalogNodes(string catalogName);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameDtoEx")]
        CatalogNodeDto GetCatalogNodesDto(string catalogName, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameEx")]
        CatalogNodes GetCatalogNodes(string catalogName, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeDto")]
        CatalogNodeDto GetCatalogNodeDto(int catalogNodeId);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNode")]
        CatalogNode GetCatalogNode(int catalogNodeId);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCatalogNodeIdDtoEx")]
        CatalogNodeDto GetCatalogNodeDto(int catalogNodeId, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCatalogNodeIdEx")]
        CatalogNode GetCatalogNode(int catalogNodeId, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCodeDto")]
        CatalogNodeDto GetCatalogNodeDto(string code);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCode")]
        CatalogNode GetCatalogNode(string code);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCodeDtoEx")]
        CatalogNodeDto GetCatalogNodeDto(string code, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByCodeEx")]
        CatalogNode GetCatalogNode(string code, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameAndParentNodeCodeDto")]
        CatalogNodeDto GetCatalogNodesDto(string catalogName, string parentNodeCode);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameAndParentNodeCode")]
        CatalogNodes GetCatalogNodes(string catalogName, string parentNodeCode);

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameAndParentNodeCodeDtoEx")]
        CatalogNodeDto GetCatalogNodesDto(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodesByCatalogNameAndParentNodeCodeEx")]
        CatalogNodes GetCatalogNodes(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByUriDto")]
        CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByUri")]
        CatalogNode GetCatalogNode(string uri, string languageCode);

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByUriDtoEx")]
        CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode, CatalogNodeResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogNodeByUriEx")]
        CatalogNode GetCatalogNode(string uri, string languageCode, CatalogNodeResponseGroup responseGroup);

        /// <summary>
        /// Saves the catalog node.
        /// </summary>
        /// <param name="dto">The dto.</param>
        void SaveCatalogNode(CatalogNodeDto dto);

        /// <summary>
        /// Deletes the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogId">The catalog id.</param>
        [OperationContract(Name = "DeleteCatalogNode")]
        void DeleteCatalogNode(int catalogNodeId, int catalogId);
        #endregion

        #region CatalogEntryMethods
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDto")]
        CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId);

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(string catalogName, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogDto")]
        CatalogEntryDto GetCatalogEntriesDto(string catalogName);

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogNameDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogNameDto")]
        CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId);

        /// <summary>
        /// Gets the catalog entries by node dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogNodeCodeDtoEx")]
        CatalogEntryDto GetCatalogEntriesByNodeDto(string catalogName, string parentNodeCode, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries by node dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesCatalogNodeCodeDto")]
        CatalogEntryDto GetCatalogEntriesByNodeDto(string catalogName, string parentNodeCode);

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByCatalogIdDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(int catalogId, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByCatalogIdDto")]
        CatalogEntryDto GetCatalogEntriesDto(int catalogId);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesRelatedDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(int parentEntryId, string entryType, string relationType, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesRelatedDto")]
        CatalogEntryDto GetCatalogEntriesDto(int parentEntryId, string entryType, string relationType);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByNameDto")]
        CatalogEntryDto GetCatalogEntriesDto(string name, string entryType);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByNameDtoEx")]
        CatalogEntryDto GetCatalogEntriesDto(string name, string entryType, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDtoByIdsWithCache")]
        CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDtoByIdsWithCacheEx")]
        CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDtoByIds")]
        CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries);
        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesDtoByIdsEx")]
        CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetAssociatedCatalogEntriesDto")]
        CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName);
        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetAssociatedCatalogEntriesDtoEx")]
        CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetAssociatedCatalogEntriesByCodeDto")]
        CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName);
        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetAssociatedCatalogEntriesByCodeDtoEx")]
        CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName, CatalogEntryResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryDto")]
        CatalogEntryDto GetCatalogEntryDto(int catalogEntryId);
        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryDtoEx")]
        CatalogEntryDto GetCatalogEntryDto(int catalogEntryId, CatalogEntryResponseGroup responseGroup);

        /// <summary>
        /// Saves the catalog entry.
        /// </summary>
        /// <param name="dto">The dto.</param>
        void SaveCatalogEntry(CatalogEntryDto dto);

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        [OperationContract(Name = "DeleteCatalogEntryByCatalogId")]
        void DeleteCatalogEntry(int entryId, bool recursive);

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByCodeDto")]
        CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode);
        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByCodeDtoEx")]
        CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode, CatalogEntryResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntry")]
        Entry GetCatalogEntry(int catalogEntryId);
        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryEx")]
        Entry GetCatalogEntry(int catalogEntryId, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryCode")]
        Entry GetCatalogEntry(string code);
        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryCodeEx")]
        Entry GetCatalogEntry(string code, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entry by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByUri")]
        Entry GetCatalogEntryByUri(string uri, string languageCode);
        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByUriEx")]
        Entry GetCatalogEntryByUri(string uri, string languageCode, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesNameCode")]
        Entries GetCatalogEntries(string catalogName, string catalogNodeCode);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesNameCodeEx")]
        Entries GetCatalogEntries(string catalogName, string catalogNodeCode, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByIdsWithCache")]
        Entries GetCatalogEntries(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByIdsWithCacheEx")]
        Entries GetCatalogEntries(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByIds")]
        Entries GetCatalogEntries(int[] catalogEntries);
        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntriesByIdsEx")]
        Entries GetCatalogEntries(int[] catalogEntries, CatalogEntryResponseGroup responseGroup);
        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByUriDto")]
        CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode);
        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        [OperationContract(Name = "GetCatalogEntryByUriDtoEx")]
        CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode, CatalogEntryResponseGroup responseGroup);

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
        CatalogRelationDto GetCatalogRelationDto(int catalogId, int catalogNodeId, int catalogEntryId, string groupName, CatalogRelationResponseGroup responseGroup);

        /// <summary>
        /// Gets the catalog relation dto.
        /// </summary>
        /// <param name="assetKey">The asset key.</param>
        /// <returns></returns>
        CatalogRelationDto GetCatalogRelationDto(string assetKey);

        /// <summary>
        /// Saves the catalog relation dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        void SaveCatalogRelationDto(CatalogRelationDto dto);
        #endregion

		#region CatalogAssociation methods
        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        /// <returns></returns>
		CatalogAssociationDto GetCatalogAssociationDto(int catalogAssociationId);
        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationName">Name of the catalog association.</param>
        /// <returns></returns>
		CatalogAssociationDto GetCatalogAssociationDto(string catalogAssociationName);
        /// <summary>
        /// Gets the catalog association dto by entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
		CatalogAssociationDto GetCatalogAssociationDtoByEntryId(int catalogEntryId);
        /// <summary>
        /// Gets the catalog association dto by entry code.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
		CatalogAssociationDto GetCatalogAssociationDtoByEntryCode(int catalogId, string catalogEntryCode);
        /// <summary>
        /// Saves the catalog association.
        /// </summary>
        /// <param name="dto">The dto.</param>
		void SaveCatalogAssociation(CatalogAssociationDto dto);
		#endregion

        #region Dictionary Methods
		/// <summary>
		/// Gets the merchants.
		/// </summary>
		/// <returns></returns>
		CatalogEntryDto GetMerchantsDto();
		
        /// <summary>
        /// Gets the currency dto.
        /// </summary>
        /// <returns></returns>
        CurrencyDto GetCurrencyDto();
        /// <summary>
        /// Saves the currency.
        /// </summary>
        /// <param name="dto">The dto.</param>
        void SaveCurrency(CurrencyDto dto);
        #endregion
    }
}