using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public class SiteMenuItem
    {
        private int _SortOrder;

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder
        {
            get { return _SortOrder; }
            set { _SortOrder = value; }
        }
        private string _Text;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        private string _Url;

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

        private SiteMenu _Menu;

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>The menu.</value>
        public SiteMenu Menu
        {
            get { return _Menu; }
            set { _Menu = value; }
        }
        private SiteMenuItem[] _Items;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public SiteMenuItem[] Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        private SiteMenuItem _Parent;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public SiteMenuItem Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        private string _CommandText;

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get { return _CommandText; }
            set { _CommandText = value; }
        }
        private int _CommandType;

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public int CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        private bool _IsVisible;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }

        private SiteMenuItemResources _Resources;
        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public SiteMenuItemResources Resources
        {
            get { return _Resources; }
            set { _Resources = value; }
        }
    }
}
