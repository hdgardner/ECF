using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Implements operations for the catalog response group.
    /// </summary>
    [DataContract]
    public partial class CatalogResponseGroup
    {
        ResponseGroup _ResponseGroups = ResponseGroup.Request | ResponseGroup.CatalogInfo;

        /// <summary>
        /// Defines and specifies the catalog response group type.
        /// </summary>
        [Flags]
        public enum ResponseGroup
        {
            /// <summary>
            /// Represents the request response group.
            /// </summary>
            Request = 1,
            /// <summary>
            /// Public string literal for the catalog info response group.
            /// </summary>
            CatalogInfo = 2,
            /// <summary>
            /// Public string literal for the catalog full response group.
            /// </summary>
            CatalogFull = 4,
            /// <summary>
            /// Public string literal for the children response group.
            /// </summary>
            Children = 8
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                if ((_ResponseGroups & ResponseGroup.Request) == ResponseGroup.Request)
                    key.Append("Request");

                if ((_ResponseGroups & ResponseGroup.Children) == ResponseGroup.Children)
                    key.Append("Children");

                if ((_ResponseGroups & ResponseGroup.CatalogInfo) == ResponseGroup.CatalogInfo)
                    key.Append("CatalogInfo");

                if ((_ResponseGroups & ResponseGroup.CatalogFull) == ResponseGroup.CatalogFull)
                    key.Append("CatalogFull");

                return key.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified group contains group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>
        /// 	<c>true</c> if the specified group contains group; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsGroup(ResponseGroup group)
        {
            if ((_ResponseGroups & group) == group)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogResponseGroup"/> class.
        /// </summary>
        public CatalogResponseGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogResponseGroup"/> class.
        /// </summary>
        /// <param name="responseGroups">The response groups.</param>
        public CatalogResponseGroup(ResponseGroup responseGroups)
        {
            _ResponseGroups = responseGroups;
        }

        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>The response groups.</value>
        [DataMember]
        public ResponseGroup ResponseGroups
        {
            get
            {
                return _ResponseGroups;
            }
            set
            {
                _ResponseGroups = value;
            }
        }
    }

    /// <summary>
    /// Implements operations for the catalog node response group.
    /// </summary>
    [DataContract]
    public partial class CatalogNodeResponseGroup
    {
        ResponseGroup _ResponseGroups = ResponseGroup.Request | ResponseGroup.CatalogNodeInfo;

        /// <summary>
        /// The ResponseGroup enumeration defines and specifies the catalog node response groups.
        /// </summary>
        [Flags]
        public enum ResponseGroup
        {
            /// <summary>
            /// Represents the request response group.
            /// </summary>
            Request = 1,
            /// <summary>
            /// Represents the catalog node info response group.
            /// </summary>
            CatalogNodeInfo = 2,
            /// <summary>
            /// Represents the catalog node full response group.
            /// </summary>
            CatalogNodeFull = 4,
            /// <summary>
            /// Represents the children response group.
            /// </summary>
            Children = 8,
            /// <summary>
            /// Represents the ancestor response group.
            /// </summary>
            Ancestor = 16,
            /// <summary>
            /// Represents the assets response group.
            /// </summary>
            Assets = 32
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                if ((_ResponseGroups & ResponseGroup.Request) == ResponseGroup.Request)
                    key.Append("Request");

                if ((_ResponseGroups & ResponseGroup.Children) == ResponseGroup.Children)
                    key.Append("Children");

                if ((_ResponseGroups & ResponseGroup.CatalogNodeInfo) == ResponseGroup.CatalogNodeInfo)
                    key.Append("CatalogNodeInfo");

                if ((_ResponseGroups & ResponseGroup.CatalogNodeFull) == ResponseGroup.CatalogNodeFull)
                    key.Append("CatalogNodeFull");

                if ((_ResponseGroups & ResponseGroup.Ancestor) == ResponseGroup.Ancestor)
                    key.Append("Ancestor");

                return key.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified group contains group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>
        /// 	<c>true</c> if the specified group contains group; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsGroup(ResponseGroup group)
        {
            if ((_ResponseGroups & group) == group)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNodeResponseGroup"/> class.
        /// </summary>
        public CatalogNodeResponseGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNodeResponseGroup"/> class.
        /// </summary>
        /// <param name="responseGroups">The response groups.</param>
        public CatalogNodeResponseGroup(ResponseGroup responseGroups)
        {
            _ResponseGroups = responseGroups;
        }

        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>The response groups.</value>
        [DataMember]
        public ResponseGroup ResponseGroups
        {
            get
            {
                return _ResponseGroups;
            }
            set
            {
                _ResponseGroups = value;
            }
        }
    }

    /// <summary>
    /// Implements operations for the catalog entry response group.
    /// </summary>
    [DataContract]
    public partial class CatalogEntryResponseGroup
    {
        ResponseGroup _ResponseGroups = ResponseGroup.Request | ResponseGroup.CatalogEntryInfo;

        /// <summary>
        /// The ResponseGroup enumeration defines and specifies the catalog entry response group.
        /// </summary>
        [Flags]
        public enum ResponseGroup
        {
            /// <summary>
            /// Represents the request response group.
            /// </summary>
            Request = 1,
            /// <summary>
            /// Represents the catalog entry info response group.
            /// </summary>
            CatalogEntryInfo = 2,
            /// <summary>
            /// Represents the catalog entry full response group.
            /// </summary>
            CatalogEntryFull = 4,
            /// <summary>
            /// Represents the associations response group.
            /// </summary>
            Associations = 8,
            /// <summary>
            /// Represents the children response group.
            /// </summary>
            Children = 16,
            /// <summary>
            /// Represents the assets response group.
            /// </summary>
            Assets = 32
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                if ((_ResponseGroups & ResponseGroup.Request) == ResponseGroup.Request)
                    key.Append("Request");

                if ((_ResponseGroups & ResponseGroup.Children) == ResponseGroup.Children)
                    key.Append("Children");

                if ((_ResponseGroups & ResponseGroup.CatalogEntryInfo) == ResponseGroup.CatalogEntryInfo)
                    key.Append("CatalogEntryInfo");

                if ((_ResponseGroups & ResponseGroup.CatalogEntryFull) == ResponseGroup.CatalogEntryFull)
                    key.Append("CatalogEntryFull");

                if ((_ResponseGroups & ResponseGroup.Associations) == ResponseGroup.Associations)
                    key.Append("Associations");

                if ((_ResponseGroups & ResponseGroup.Assets) == ResponseGroup.Assets)
                    key.Append("Assets");                

                return key.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified group contains group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>
        /// 	<c>true</c> if the specified group contains group; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsGroup(ResponseGroup group)
        {
            if ((_ResponseGroups & group) == group)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogEntryResponseGroup"/> class.
        /// </summary>
        public CatalogEntryResponseGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogEntryResponseGroup"/> class.
        /// </summary>
        /// <param name="responseGroups">The response groups.</param>
        public CatalogEntryResponseGroup(ResponseGroup responseGroups)
        {
            _ResponseGroups = responseGroups;
        }

        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>The response groups.</value>
        [DataMember]
        public ResponseGroup ResponseGroups
        {
            get
            {
                return _ResponseGroups;
            }
            set
            {
                _ResponseGroups = value;
            }
        }
    }


    /// <summary>
    /// Implements operations for the catalog relation response group.
    /// </summary>
    [DataContract]
    public partial class CatalogRelationResponseGroup
    {
        ResponseGroup _ResponseGroups = ResponseGroup.CatalogNode | ResponseGroup.CatalogEntry | ResponseGroup.NodeEntry;

        /// <summary>
        /// The ResponseGroup enumeration defines and specifies the catalog relation response group.
        /// </summary>
        [Flags]
        public enum ResponseGroup
        {
            /// <summary>
            /// Represents the catalog node response group.
            /// </summary>
            CatalogNode = 1,
            /// <summary>
            /// Represents the catalog entry response group.
            /// </summary>
            CatalogEntry = 2,
            /// <summary>
            /// Represents the node entry response group.
            /// </summary>
            NodeEntry = 4
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                if ((_ResponseGroups & ResponseGroup.CatalogEntry) == ResponseGroup.CatalogEntry)
                    key.Append("CatalogEntry");

                if ((_ResponseGroups & ResponseGroup.CatalogNode) == ResponseGroup.CatalogNode)
                    key.Append("CatalogNode");

                if ((_ResponseGroups & ResponseGroup.NodeEntry) == ResponseGroup.NodeEntry)
                    key.Append("NodeEntry");

                return key.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified group contains group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>
        /// 	<c>true</c> if the specified group contains group; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsGroup(ResponseGroup group)
        {
            if ((_ResponseGroups & group) == group)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogRelationResponseGroup"/> class.
        /// </summary>
        public CatalogRelationResponseGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogRelationResponseGroup"/> class.
        /// </summary>
        /// <param name="responseGroups">The response groups.</param>
        public CatalogRelationResponseGroup(ResponseGroup responseGroups)
        {
            _ResponseGroups = responseGroups;
        }

        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>The response groups.</value>
        [DataMember]
        public ResponseGroup ResponseGroups
        {
            get
            {
                return _ResponseGroups;
            }
            set
            {
                _ResponseGroups = value;
            }
        }
    }
}