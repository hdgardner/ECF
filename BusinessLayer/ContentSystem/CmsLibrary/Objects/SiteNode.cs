using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.Objects
{
    public abstract class SiteNode
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
        private bool _IsPublic;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
        public bool IsPublic
        {
            get { return _IsPublic; }
            set { _IsPublic = value; }
        }
        private int _ID;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private string[] _Roles;

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public string[] Roles
        {
            get { return _Roles; }
            set { _Roles = value; }
        }
    }
}
