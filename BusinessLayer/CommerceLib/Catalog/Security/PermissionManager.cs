using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mediachase.Data.Provider;
using System.Data;
using System.Web.Security;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Data;
using System.Collections.Specialized;

namespace Mediachase.Commerce.Catalog.Security
{
    /// <summary>
    /// Implements helper methods for catalog permissions operations.
    /// </summary>
    public static class PermissionHelper
    {
        /// <summary>
        /// Converts the reader to records.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Dictionary<string, PermissionRecord> ConvertReaderToRecords(IDataReader reader)
        {
            Dictionary<string, PermissionRecord> list = new Dictionary<string, PermissionRecord>();

            while (reader.Read())
            {
                string key = CreatePermissionKey(reader["Scope"].ToString(), reader["SID"].ToString());

                list.Add(key, CreatePermissionRecord(reader["Scope"].ToString(), reader["SID"].ToString(), (byte[])reader["AllowMask"], (byte[])reader["DenyMask"]));
            }

            return list;
        }

        /// <summary>
        /// Creates the permission record.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sid">The sid.</param>
        /// <param name="allowMask">The allow mask.</param>
        /// <param name="denyMask">The deny mask.</param>
        /// <returns></returns>
        public static PermissionRecord CreatePermissionRecord(string scope, string sid, byte[] allowMask, byte[] denyMask)
        {
            return new PermissionRecord(scope, sid, (Permission)(long)BitConverter.ToInt64(allowMask, 0), (Permission)(long)BitConverter.ToInt64(denyMask, 0));
        }

        /*
        public static Dictionary<string, PermissionRecord> ConvertDataRowsToRecords(DataRow[] source)
        {
            Dictionary<string, PermissionRecord> list = new Dictionary<string, PermissionRecord>();
            AddDataRowsToRecords(ref list, source);
            return list;
        }

        public static void AddDataRowsToRecords(ref Dictionary<string, PermissionRecord> list, DataRow[] source)
        {
            foreach (DataRow row in source)
            {
                AddDataRowToRecords(ref list, row);
            }
        }

        public static void AddDataRowToRecords(ref Dictionary<string, PermissionRecord> list, DataRow row)
        {
            string key = CreatePermissionKey((int)row["Scope"], row["RoleName"].ToString());
            list.Add(key, new PermissionRecord((CatalogScope)row["ScopeId"], row["RoleName"].ToString(), (Permission)row["AllowMask"], (Permission)row["DenyMask"]));
        }
         * */

        /// <summary>
        /// Creates the permission key.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sid">The sid.</param>
        /// <returns></returns>
        public static string CreatePermissionKey(string scope, string sid)
        {
            return String.Format("{0}-{1}", scope, sid);
        }
    }

    /// <summary>
    /// Manages catalog permissions.
    /// </summary>
    public static class PermissionManager
    {
        /*
        public static void CreateSitePermission(PermissionRecord record, Guid siteId)
        {
            CreatePermission(CatalogScope.Site, record, DataOperation.Create, String.Empty, siteId);
        }

        public static void UpdateSitePermission(PermissionRecord record, Guid siteId)
        {
            CreatePermission(CatalogScope.Site, record, DataOperation.Update, String.Empty, siteId);
        }

        public static void DeleteSitePermission(PermissionRecord record, Guid siteId)
        {
            CreatePermission(CatalogScope.Site, record, DataOperation.Delete, String.Empty, siteId);
        }

        public static void CreateCatalogPermission(PermissionRecord record, string catalogName, Guid siteId)
        {
            CreatePermission(CatalogScope.CatalogNode, record, DataOperation.Create, catalogName, siteId);
        }

        public static void UpdateCatalogPermission(PermissionRecord record, string catalogName, Guid siteId)
        {
            CreatePermission(CatalogScope.CatalogNode, record, DataOperation.Update, catalogName, siteId);
        }

        public static void DeleteCatalogPermission(PermissionRecord record, string catalogName, Guid siteId)
        {
            CreatePermission(CatalogScope.CatalogNode, record, DataOperation.Delete, catalogName, siteId);
        }
         * */

        /*
        internal static void CreatePermission(CatalogScope scope, PermissionRecord record, DataOperation operation, string entityName, Guid siteId)
        {
            // TODO: need to finish up
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteId));
            cmd.Parameters.Add(new DataParameter("Operation", operation.GetHashCode()));
            cmd.Parameters.Add(new DataParameter("RoleName", record.RoleName, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("AllowMask", (long)record.AllowMask, DataParameterType.BigInt));
            cmd.Parameters.Add(new DataParameter("DenyMask", (long)record.DenyMask, DataParameterType.BigInt));
            cmd.Parameters.Add(new DataParameter("Scope", record.Scope.GetHashCode(), DataParameterType.SmallInt));

            switch (scope)
            {
                case CatalogScope.Site:
                    cmd.CommandText = DataHelper.CreateInsertUpdateDeleteStoredProcedureName("CatalogSecurity");
                    break;
                case CatalogScope.Catalog:
                    cmd.Parameters.Add(new DataParameter("CatalogName", entityName, DataParameterType.NVarChar, 100));
                    cmd.CommandText = DataHelper.CreateInsertUpdateDeleteStoredProcedureName("SiteSecurity");
                    break;
                default:
                    throw new InvalidOperationException("Only Site and Catalog scopes supported.");
            }


            DataResult result = DataService.ExecuteNonExec(cmd);
        }
         * */

        /// <summary>
        /// Validates the permission.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="entityName">Name of the entity.</param>
        public static void ValidatePermission(string scope, Permission permission, string entityName)
        {
            //if (!CheckPermission(scope, permission, entityName))
            //    throw new AccessDeniedException();
        }

        /// <summary>
        /// Checks permissions in the following order:
        /// 1. Check permissions for the organization active user belongs to.
        /// 2. Check permissions for the role user belongs to.
        /// 3. Check permissions for the user himself.
        /// 
        /// The later check overrides the previous one if permission is explicitely defined.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="permission"></param>
        /// <param name="recordSet"></param>
        /// <returns></returns>
        public static bool CheckPermission(string scope, Permission permission, PermissionRecordSet recordSet)
        {
            bool allow = true;
            PermissionRecord permissionRecord = null;

            // Administrator has unlimited access rights
            if (Roles.IsUserInRole("Administrator"))
                return true;

            //return true; // for development return true always
            // Step 1: check organization
            CustomerProfile profile = ProfileContext.Current.Profile;
            if (profile != null)
            {
                // check if user is anonymous, if so just check roles
                if (profile.IsAnonymous)
                {
                    return CheckAnonymous(scope, permission, recordSet);

                    //string[] roles = ProfileContext.Current.GetRolesForUser();
                }
             
                Account account = profile.Account;
                if (account == null) // if no account exists, continue to check as if user is anonymous
                {
                    return CheckAnonymous(scope, permission, recordSet);
                }

                permissionRecord = ResolvePermissions(scope, recordSet, new string[1] { account.Organization.PrincipalId.ToString() });
                if (permissionRecord!=null)
                    allow = permissionRecord.GetBit(permission);              
            }

            // Step 2: check roles
            string[] roles = ProfileContext.Current.GetRolesForUser();
            if (roles != null)
            {
                permissionRecord = ResolvePermissions(scope, recordSet, roles);
                if (permissionRecord != null)
                    allow = permissionRecord.GetBit(permission);              
            }


            // Step 3: check user
            permissionRecord = ResolvePermissions(scope, recordSet, new string[1] { ProfileContext.Current.UserId.ToString() });
            if (permissionRecord != null)
                allow = permissionRecord.GetBit(permission);

            //ProfileContext.Current.GetAccount()
            return allow;
        }

        /// <summary>
        /// Checks the anonymous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="recordSet">The record set.</param>
        /// <returns></returns>
        private static bool CheckAnonymous(string scope, Permission permission, PermissionRecordSet recordSet)
        {
            return true;
        }

        /// <summary>
        /// Resolves the permissions.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="recordSet">The record set.</param>
        /// <param name="sids">The sids.</param>
        /// <returns></returns>
        private static PermissionRecord ResolvePermissions(string scope, PermissionRecordSet recordSet, string[] sids)
        {
            //string[] roles = ProfileContext.Current.GetRolesForUser();

            PermissionRecord returnPR = new PermissionRecord();
            PermissionRecord pr = null;

            if (sids != null)
            {
                foreach (string sid in sids)
                {
                    pr = recordSet[PermissionHelper.CreatePermissionKey(scope, sid)];
                    if (pr != null)
                        returnPR.Merge(pr);
                }
            }

            return returnPR;
        }

        /*
        private static PermissionRecord[] LoadPermissions(CatalogScope scope)
        { 
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_PaymentMethod_Language");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("LanguageId", language, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("UserId", FrameworkContext.Current.User.UserName, DataParameterType.NVarChar));
            cmd.DataSet = new CatalogDto();
            cmd.TableMapping = DataHelper.MapTables("PaymentMethod", "PaymentMethodParameters");

            DataResult results = DataService.LoadDataSet(cmd);

            return (CatalogDto)results.DataSet;
        }
         * */
    }

    /// <summary>
    /// The Permission enumeration defines the catalog permissions.
    /// </summary>
    [Flags()]
    public enum Permission : long
    {
        /// <summary>
        /// Represents the undefined catalog permission.
        /// </summary>
        Undefined = 0x0000000000000000,
        /// <summary>
        /// Represents the catalog view permission.
        /// </summary>
        View = 0x0000000000000001,
        /// <summary>
        /// Represents the catalog read permission.
        /// </summary>
        Read = 0x0000000000000002,
        /// <summary>
        /// Represents the catalog edit permission.
        /// </summary>
        Edit = 0x0000000000000004,
        /// <summary>
        /// Represents the catalog create permission.
        /// </summary>
        Create = 0x0000000000000008,
        /// <summary>
        /// Represents the catalog delete permission.
        /// </summary>
        Delete = 0x0000000000000016,
        /// <summary>
        /// Represents the catalog export permission.
        /// </summary>
        Export = 0x0000000000000010,
        /// <summary>
        /// Represents the catalog admin permission.
        /// </summary>
        Admin = 0x0000000000000020,
    }

    /// <summary>
    /// The AccessControlEntry enumeration defines the access control entry for the catalog permissions.
    /// </summary>
    public enum AccessControlEntry
    {
        /// <summary>
        /// Represents that entry is not set.
        /// </summary>
        NotSet = 0x00,
        /// <summary>
        /// Represents that access control is allowed.
        /// </summary>
        Allow = 0x01,
        /// <summary>
        /// Represents that access control is denied.
        /// </summary>
        Deny = 0x02
    }
}
