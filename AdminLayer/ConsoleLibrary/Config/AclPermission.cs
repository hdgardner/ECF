using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Config
{
    public class AclPermission
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

        private AclGroup _Group = null;

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        /// <value>The Children.</value>
        public AclGroup Group
        {
            get { return _Group; }
            set { _Group = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AclPermission"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public AclPermission(AclGroup group, string id, string name)
        {
            _ID = id;
            _Name = name;
            _Group = group;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder permission = new StringBuilder();
            AclGroup parentGroup = Group;

            while (parentGroup != null)
            {
                if (permission.Length > 0)
                    permission.Insert(0, ":");
                permission.Insert(0, parentGroup.ID);
                parentGroup = parentGroup.ParentGroup;
            }

            permission.Append(":");
            permission.Append(this.ID);

            return permission.ToString();
        }
    }
}
