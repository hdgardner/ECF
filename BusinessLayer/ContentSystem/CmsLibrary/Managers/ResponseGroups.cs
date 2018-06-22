using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Cms.Managers
{
    public partial class SiteResponseGroup
    {
        ResponseGroup _ResponseGroups = ResponseGroup.Info | ResponseGroup.Full;

        [Flags]
        public enum ResponseGroup
        {
            Info = 1,
            Full = 2,
            Menus = 4,
            Folders = 8,
            Pages = 16
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

                if ((_ResponseGroups & ResponseGroup.Info) == ResponseGroup.Info)
                    key.Append(ResponseGroup.Info.ToString());

                if ((_ResponseGroups & ResponseGroup.Full) == ResponseGroup.Full)
                    key.Append(ResponseGroup.Full.ToString());

                if ((_ResponseGroups & ResponseGroup.Menus) == ResponseGroup.Menus)
                    key.Append(ResponseGroup.Menus.ToString());

                if ((_ResponseGroups & ResponseGroup.Pages) == ResponseGroup.Pages)
                    key.Append(ResponseGroup.Pages.ToString());

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
        /// Initializes a new instance of the <see cref="SiteResponseGroup"/> class.
        /// </summary>
        public SiteResponseGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteResponseGroup"/> class.
        /// </summary>
        /// <param name="responseGroups">The response groups.</param>
        public SiteResponseGroup(ResponseGroup responseGroups)
        {
            _ResponseGroups = responseGroups;
        }

        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>The response groups.</value>
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