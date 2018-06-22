using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class AclGroup
    {
        private string _Name;
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        private string _ID;
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string ID
        {
            get
            {
                return _ID;
            }
        }

        private AclGroup _ParentGroup = null;

        /// <summary>
        /// Gets or sets the parent group.
        /// </summary>
        /// <value>The parent group.</value>
        public AclGroup ParentGroup
        {
            get { return _ParentGroup; }
            set { _ParentGroup = value; }
        }

        private AclGroupCollection _ChildGroups = new AclGroupCollection();

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public AclGroupCollection Groups
        {
            get { return _ChildGroups; }
            set { _ChildGroups = value; }
        }

        private AclPermissionCollection _Permissions = new AclPermissionCollection();

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <value>The permissions.</value>
        public AclPermissionCollection Permissions
        {
            get { return _Permissions; }
            set { _Permissions = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AclGroup"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public AclGroup(string id, string name)
        {
            _ID = id;
            _Name = name;
        }
    }
}
