using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Web.Console.Config
{
    public class AclManager
    {
        /// <summary>
        /// Creates the acl collection.
        /// </summary>
        /// <param name="aclNode">The acl node.</param>
        /// <returns></returns>
        public static AclGroupCollection CreateAclCollection(ModuleConfig module, XmlNode aclNode)
        {
            if (aclNode == null)
                return null;

            // Create list
            AclGroupCollection aclGroupCol = new AclGroupCollection();
            AclGroup rootGroup = new AclGroup(module.Name, module.DisplayName);
            aclGroupCol.Add(rootGroup);
            XmlNodeList groups = aclNode.SelectNodes("Group");

            foreach (XmlNode group in groups)
            {
                AclGroup aclGroup = PopulateGroupRecursive(group);
                aclGroup.ParentGroup = rootGroup;
                rootGroup.Groups.Add(aclGroup);
            }

            return aclGroupCol;
        }

        /// <summary>
        /// Populates the group recursive.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static AclGroup PopulateGroupRecursive(XmlNode node)
        {
            AclGroup group = new AclGroup(node.Attributes["id"].Value, node.Attributes["text"].Value);

            // Populate children
            XmlNodeList permissions = node.SelectNodes("Permission");
            foreach (XmlNode nodeChild in permissions)
            {
                group.Permissions.Add(PopulatePermission(group, nodeChild));
            }

            // Populate children
            XmlNodeList groups = node.SelectNodes("Group");
            foreach (XmlNode nodeChild in groups)
            {
                AclGroup subGroup = PopulateGroupRecursive(nodeChild);
                subGroup.ParentGroup = group;
                group.Groups.Add(subGroup);
            }

            return group;
        }

        /// <summary>
        /// Populates the permission.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static AclPermission PopulatePermission(AclGroup group, XmlNode node)
        {
            return new AclPermission(group, node.Attributes["id"].Value, node.Attributes["text"].Value);
        }
    }
}
