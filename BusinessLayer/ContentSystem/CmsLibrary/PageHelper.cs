using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Security;
using System.Data;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms
{
    /// <summary>
    /// Conatins methods that help with commong functions that might be performed on CMS Pages
    /// </summary>
    public class PageHelper
    {
        /// <summary>
        /// Determines whether there is a language version exists for a specified page.
        /// It will return true, if user specified has a draft version already created.
        /// It will return false if version does not exist or if currently logged in user does not
        /// have access to the status'es page versions are in.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="langId">The lang id.</param>
        /// <returns>
        /// 	<c>true</c> if [has language version] [the specified page id]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasLanguageVersion(int pageId, int langId)
        {
            int versionId = 0;
            return HasLanguageVersion(pageId, langId, versionId);
        }

        /// <summary>
        /// Determines whether there is a language version exists for a specified page.
        /// It will return true, if user specified has a draft version already created.
        /// It will return false if version does not exist or if currently logged in user does not
        /// have access to the status'es page versions are in.
        /// 
        /// Specify version id to check if current user has access to that specific version.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="langId">The lang id.</param>
        /// <param name="versionId">The version id.</param>
        /// <returns>
        /// 	<c>true</c> if [has language version] [the specified page id]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasLanguageVersion(int pageId, int langId, int versionId)
        {
            // Load current user
            Guid userId = (Guid)ProfileContext.Current.User.ProviderUserKey;

            // Load user roles and populate status ids allowed for user roles
            string[] roles = Roles.GetRolesForUser();

            ArrayList allowedStatusId = new ArrayList();

            if (roles != null && roles.Length > 0)
            {
                foreach (string role in roles)
                {
                    ArrayList statusList = WorkflowAccess.LoadListByRoleId(role);

                    foreach (object status in statusList)
                    {
                        if (!allowedStatusId.Contains(status))
                        {
                            allowedStatusId.Add(status);
                        }
                    }
                }
            }            

            //get archive status id
            int archiveStatusId = WorkflowStatus.GetArcStatus(0);
            using (IDataReader reader = PageVersion.GetVersionByLangId(pageId, langId))
            {
                while (reader.Read())
                {
                    // Check the access to the specific version
                    if (versionId > 0 && versionId != (int)reader["VersionId"])
                        continue;

                    int statusId = (int)reader["StatusId"];
                    using (IDataReader status = WorkflowStatus.LoadById(statusId))
                    {
                        if (status.Read())
                        {
                            if (statusId == archiveStatusId || allowedStatusId.Contains(statusId))
                            {
                                versionId = (int)reader["VersionId"];
                                status.Close();                                
                                return true;
                            }
                        }

                        status.Close();
                    }

                    //add user draft
                    Guid ownerKey = new Guid(reader["EditorUID"].ToString());
                    if (statusId == WorkflowStatus.DraftId && userId == ownerKey)
                    {
                        versionId = (int)reader["VersionId"];
                        reader.Close();
                        return true;
                    }
                }

                reader.Close();
            }
            return false;
        }
    }
}