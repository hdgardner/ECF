using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public class SiteMenu
    {
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private SiteMenuItem[] _Items;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public SiteMenuItem[] Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }

    }
}
