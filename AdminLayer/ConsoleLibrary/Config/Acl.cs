using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Web.Console.Config
{
    public class Acl
    {
        private AclGroupCollection _AclGroups;

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public AclGroupCollection Groups
        {
            get { return _AclGroups; }
            set { _AclGroups = value; }
        }
    }
}
