using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Cms.Objects
{
    /// <summary>
    /// Contains collection of Menu Item Resources.
    /// </summary>
    public partial class SiteMenuItemResources
    {
        /// <summary>
        /// 
        /// </summary>
        private SiteMenuItemResource[] _Resource;

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>The resource.</value>
        public SiteMenuItemResource[] Resource
        {
            get { return _Resource; }
            set { _Resource = value; }
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Cms.Objects.SiteMenuItemResource"/> with the specified language code.
        /// </summary>
        /// <value></value>
        public SiteMenuItemResource this[string languageCode]
        {
            get
            {
                if (_Resource != null)
                    foreach (SiteMenuItemResource res in _Resource)
                    {
                        if (res.LanguageCode.ToLower().CompareTo(languageCode.ToLower()) == 0)
                            return res;
                    }

                return null;
            }
        }
    }
}
