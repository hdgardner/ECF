using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements operations for the security manager.
    /// </summary>
    public static class SecurityManager
    {
        /// <summary>
        /// Checks the required roles permission.
        /// </summary>
        /// <param name="requiredRoles">The required roles.</param>
        /// <returns></returns>
        public static bool CheckPermission(string[] requiredRoles)
        {
            return CheckPermission(requiredRoles, false);
        }

        /// <summary>
        /// Checks the permission and if any of the role matches returns true.
        /// </summary>
        /// <param name="requiredRoles">The required roles.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns></returns>
        public static bool CheckPermission(string[] requiredRoles, bool throwException)
        {
            // If user does not have any roles, then it must be due to authentication module
            // not activated yet so return true. The security will be checked after authentication
            // is done.
            if (!Roles.Enabled)
                return true; 

            if (Roles.GetRolesForUser().Length == 0)
                return false;

            // Always allow admin
            if (Roles.IsUserInRole(AppRoles.AdminRole))
                return true;            
            
            // If not required roles specified return true
            if (requiredRoles == null || requiredRoles.Length == 0)
                return true;

            // cycle through all roles
            foreach (string role in requiredRoles)
            {
                if (Roles.IsUserInRole(role))
                    return true;
            }

            if (throwException)
                throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");

            return false;
        }

        /// <summary>
        /// Check for permissions for a task. If permissions not associated with current user, throw exception
        /// </summary>
        /// <param name="permissionString"></param>
        public static void CheckRolePermission(string permissionString)
        {
            //if permissions are turned off, always provide permission
            if (!ProfileConfiguration.Instance.EnablePermissions)
                return;

            //If user is an admin, always provide permission
            if (SecurityManager.CheckPermission(new string[] { AppRoles.AdminRole }, false))
                return;

            if (!ProfileContext.Current.CheckPermission(permissionString))
                throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");

        }
    }
}
