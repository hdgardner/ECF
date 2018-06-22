using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;
using System.Data;

namespace Mediachase.Commerce.Catalog.Impl
{
    /// <summary>
    /// Site Context Proxy class is used to call the methods on the remote web service.
    /// This is client implementation.
    /// </summary>
    public class CatalogContextProxyImpl : ICatalogSystem
    {
        ICatalogSystem _Proxy = null;

        #region Proxy Ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogContextProxyImpl"/> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        public CatalogContextProxyImpl(Binding binding, EndpointAddress address)
        {
            ChannelFactory<ICatalogSystem> factory = new ChannelFactory<ICatalogSystem>(binding, address);
            _Proxy = factory.CreateChannel();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogContextProxyImpl"/> class.
        /// </summary>
        /// <param name="endPointConfigurationName">End name of the point configuration.</param>
        public CatalogContextProxyImpl(string endPointConfigurationName)
        {
            ChannelFactory<ICatalogSystem> factory = new ChannelFactory<ICatalogSystem>(endPointConfigurationName);
            _Proxy = factory.CreateChannel();
        }
        #endregion

        #region Web Services Ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogContextProxyImpl"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal CatalogContextProxyImpl(ICatalogSystem context)
        {
            _Proxy = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogContextProxyImpl"/> class.
        /// </summary>
        public CatalogContextProxyImpl()
        {
            // Create new context
            _Proxy = CatalogContext.Current;
        }
        #endregion

        #region ICatalogSystem Members
        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(Guid siteId, CatalogResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogDto(siteId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(Guid siteId)
        {
            return _Proxy.GetCatalogDto(siteId);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(int catalogId, CatalogResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogDto GetCatalogDto(int catalogId)
        {
            return _Proxy.GetCatalogDto(catalogId);
        }

        /// <summary>
        /// Saves the catalog.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalog(CatalogDto dto)
        {
            _Proxy.SaveCatalog(dto);
        }

        /// <summary>
        /// Deletes the catalog.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        public void DeleteCatalog(int catalogId)
        {
            _Proxy.DeleteCatalog(catalogId);
        }

        /// <summary>
        /// Gets the catalog dto.
        /// </summary>
        /// <returns></returns>
        public CatalogDto GetCatalogDto()
        {
            return _Proxy.GetCatalogDto();
        }

        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options)
        {
            return _Proxy.FindItems(parameters, options);
        }

        /// <summary>
        /// Finds the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries FindItems(CatalogSearchParameters parameters, CatalogSearchOptions options, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.FindItems(parameters, options, responseGroup);
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
            return _Proxy.FindItemsDto(parameters, options, ref recordsCount);
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
            return _Proxy.FindItemsDto(parameters, options, ref recordsCount, responseGroup);
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
            return _Proxy.FindNodesDto(parameters, options, ref recordsCount);
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
            return _Proxy.FindNodesDto(parameters, options, ref recordsCount, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId)
        {
            return _Proxy.GetCatalogNodesDto(catalogId, parentNodeId);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId)
        {
            return _Proxy.GetCatalogNodes(catalogId, parentNodeId);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodesDto(catalogId, parentNodeId, responseGroup);
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
            return _Proxy.GetCatalogNodes(catalogId, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId)
        {
            return _Proxy.GetCatalogNodesDto(catalogId);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId)
        {
            return _Proxy.GetCatalogNodes(catalogId);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodesDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodes(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName)
        {
            return _Proxy.GetCatalogNodesDto(catalogName);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName)
        {
            return _Proxy.GetCatalogNodes(catalogName);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodesDto(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodes(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, string catalogNodeCode)
        {
            return _Proxy.GetCatalogNodesDto(catalogName, catalogNodeCode);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, string catalogNodeCode)
        {
            return _Proxy.GetCatalogNodes(catalogName, catalogNodeCode);
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodesDto(string catalogName, string catalogNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodesDto(catalogName, catalogNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodes GetCatalogNodes(string catalogName, string catalogNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodes(catalogName, catalogNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(int catalogNodeId)
        {
            return _Proxy.GetCatalogNodeDto(catalogNodeId);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(int catalogNodeId)
        {
            return _Proxy.GetCatalogNode(catalogNodeId);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodeDto(catalogNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNode(catalogNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string code)
        {
            return _Proxy.GetCatalogNodeDto(code);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string code)
        {
            return _Proxy.GetCatalogNode(code);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string code, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNodeDto(code, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string code, CatalogNodeResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogNode(code, responseGroup);
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode)
        {
            return _Proxy.GetCatalogNodeDto(uri, languageCode);
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogNode GetCatalogNode(string uri, string languageCode)
        {
            return _Proxy.GetCatalogNode(uri, languageCode);
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
            return _Proxy.GetCatalogNodeDto(uri, languageCode, responseGroup);
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
            return _Proxy.GetCatalogNode(uri, languageCode, responseGroup);
        }

        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public DataTable FindCatalogItemsTable(ItemSearchParameters parameters)
		{
			return _Proxy.FindCatalogItemsTable(parameters);
		}

        /// <summary>
        /// Finds the catalog items table.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public DataTable FindCatalogItemsTable(ItemSearchParameters parameters, CatalogNodeResponseGroup responseGroup)
		{
            return _Proxy.FindCatalogItemsTable(parameters, responseGroup);
		}

        /// <summary>
        /// Saves the catalog node.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogNode(CatalogNodeDto dto)
        {
            _Proxy.SaveCatalogNode(dto);
        }

        /// <summary>
        /// Deletes the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogId">The catalog id.</param>
        public void DeleteCatalogNode(int catalogNodeId, int catalogId)
        {
            _Proxy.DeleteCatalogNode(catalogNodeId, catalogId);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntriesDto(catalogId, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId)
        {
            return _Proxy.GetCatalogEntriesDto(catalogId, parentNodeId);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntriesDto(catalogName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName)
        {
            return _Proxy.GetCatalogEntriesDto(catalogName);
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
            return _Proxy.GetCatalogEntriesDto(catalogName, parentNodeId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId)
        {
            return _Proxy.GetCatalogEntriesDto(catalogName, parentNodeId);
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
            return _Proxy.GetCatalogEntriesByNodeDto(catalogName, parentNodeCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries by node dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesByNodeDto(string catalogName, string parentNodeCode)
        {
            return _Proxy.GetCatalogEntriesByNodeDto(catalogName, parentNodeCode);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntriesDto(catalogId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int catalogId)
        {
            return _Proxy.GetCatalogEntriesDto(catalogId);
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
            return _Proxy.GetCatalogEntriesDto(parentEntryId, entryType, relationType, responseGroup);
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
            return _Proxy.GetCatalogEntriesDto(parentEntryId, entryType, relationType);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(string name, string entryType)
        {
            return _Proxy.GetCatalogEntriesDto(name, entryType);
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
            return _Proxy.GetCatalogEntriesDto(name, entryType, responseGroup);
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName)
        {
            return _Proxy.GetAssociatedCatalogEntriesDto(parentEntryId, associationName);
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
            return _Proxy.GetAssociatedCatalogEntriesDto(parentEntryId, associationName, responseGroup);
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <returns></returns>
        public CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName)
        {
            return _Proxy.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName);
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
            return _Proxy.GetAssociatedCatalogEntriesDto(parentEntryCode, associationName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(int catalogEntryId)
        {
            return _Proxy.GetCatalogEntryDto(catalogEntryId);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntryDto(catalogEntryId, responseGroup);
        }

        /// <summary>
        /// Saves the catalog entry.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogEntry(CatalogEntryDto dto)
        {
            _Proxy.SaveCatalogEntry(dto);
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public void DeleteCatalogEntry(int entryId, bool recursive)
        {
            _Proxy.DeleteCatalogEntry(entryId, recursive);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode)
        {
            return _Proxy.GetCatalogEntryDto(catalogEntryCode);
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntryDto(catalogEntryCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(int catalogEntryId)
        {
            return _Proxy.GetCatalogEntry(catalogEntryId);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntry(catalogEntryId, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(string code)
        {
            return _Proxy.GetCatalogEntry(code);
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entry GetCatalogEntry(string code, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntry(code, responseGroup);
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
            return _Proxy.GetCatalogEntryByUri(uri, languageCode, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public Entry GetCatalogEntryByUri(string uri, string languageCode)
        {
            return _Proxy.GetCatalogEntryByUri(uri, languageCode, new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(string catalogName, string catalogNodeCode)
        {
            return _Proxy.GetCatalogEntries(catalogName, catalogNodeCode);
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(string catalogName, string catalogNodeCode, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntries(catalogName, catalogNodeCode, responseGroup);
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
            return _Proxy.GetCatalogEntriesDto(catalogEntries, cacheResults, cacheTimeout, new CatalogEntryResponseGroup());
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
            return _Proxy.GetCatalogEntriesDto(catalogEntries, cacheResults, cacheTimeout, responseGroup);
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
            return _Proxy.GetCatalogEntries(catalogEntries, cacheResults, cacheTimeout, new CatalogEntryResponseGroup());
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
            return _Proxy.GetCatalogEntries(catalogEntries, cacheResults, cacheTimeout, responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries)
        {
            return _Proxy.GetCatalogEntries(catalogEntries, false, new TimeSpan(), new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public Entries GetCatalogEntries(int[] catalogEntries, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntries(catalogEntries, false, new TimeSpan(), responseGroup);
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries)
        {
            return _Proxy.GetCatalogEntriesDto(catalogEntries, false, new TimeSpan(), new CatalogEntryResponseGroup());
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, CatalogEntryResponseGroup responseGroup)
        {
            return _Proxy.GetCatalogEntriesDto(catalogEntries, false, new TimeSpan(), responseGroup);
        }

        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode)
        {
            return _Proxy.GetCatalogEntryByUriDto(uri, languageCode);
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
            return _Proxy.GetCatalogEntryByUriDto(uri, languageCode, responseGroup);
        }

		/// <summary>
		/// Gets merchants.
		/// </summary>
		/// <returns></returns>
		public CatalogEntryDto GetMerchantsDto()
		{
			return _Proxy.GetMerchantsDto();
		}

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
            return _Proxy.GetCatalogRelationDto(catalogId, catalogNodeId, catalogEntryId, groupName, responseGroup);
        }

        /// <summary>
        /// Gets the catalog relation dto.
        /// </summary>
        /// <param name="assetKey">The asset key.</param>
        /// <returns></returns>
        public CatalogRelationDto GetCatalogRelationDto(string assetKey)
        {
            return _Proxy.GetCatalogRelationDto(assetKey);
        }

        /// <summary>
        /// Saves the catalog relation dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogRelationDto(CatalogRelationDto dto)
        {
            _Proxy.SaveCatalogRelationDto(dto);
        }

        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        /// <returns></returns>
        public CatalogAssociationDto GetCatalogAssociationDto(int catalogAssociationId)
        {
            return _Proxy.GetCatalogAssociationDto(catalogAssociationId);
        }

        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationName">Name of the catalog association.</param>
        /// <returns></returns>
        public CatalogAssociationDto GetCatalogAssociationDto(string catalogAssociationName)
        {
            return _Proxy.GetCatalogAssociationDto(catalogAssociationName);
        }

        /// <summary>
        /// Gets the catalog association dto by entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        public CatalogAssociationDto GetCatalogAssociationDtoByEntryId(int catalogEntryId)
        {
            return _Proxy.GetCatalogAssociationDtoByEntryId(catalogEntryId);
        }

        /// <summary>
        /// Gets the catalog association dto by entry code.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
        public CatalogAssociationDto GetCatalogAssociationDtoByEntryCode(int catalogId, string catalogEntryCode)
        {
            return _Proxy.GetCatalogAssociationDtoByEntryCode(catalogId, catalogEntryCode);
        }

        /// <summary>
        /// Saves the catalog association.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCatalogAssociation(CatalogAssociationDto dto)
        {
            _Proxy.SaveCatalogAssociation(dto);
        }

        /// <summary>
        /// Gets the currency dto.
        /// </summary>
        /// <returns></returns>
        public CurrencyDto GetCurrencyDto()
        {
            return _Proxy.GetCurrencyDto();
        }

        /// <summary>
        /// Saves the currency.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveCurrency(CurrencyDto dto)
        {
            _Proxy.SaveCurrency(dto);
        }

        #endregion
    }
}
