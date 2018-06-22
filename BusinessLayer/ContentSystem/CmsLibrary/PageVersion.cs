using System;
using System.Data;
using System.Text;
using System.Web.Security;
using Mediachase.Cms.Data;
using Mediachase.Commerce.Core;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class PageVersion
    {
        #region AddPageVersion
        /// <summary>
        /// Adds the page version.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="templateId">The template id.</param>
        /// <param name="langId">The lang id.</param>
        /// <param name="creatorUID">The creator UID.</param>
        /// <param name="stateId">The state id.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public static int AddPageVersion(int pageId, int templateId, int langId, Guid creatorUID, int stateId, string comment)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionAdd]");
			cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("TemplateId", templateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("CreatorUID", creatorUID, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Comment", comment, DataParameterType.NVarChar, 1024));
			return DataService.RunReturnInteger(cmd);
        }

		/// <summary>
		/// Adds the page version. For INTERNAL USE ONLY!!!
		/// </summary>
		/// <param name="pageId">The page id.</param>
		/// <param name="templateId">The template id.</param>
		/// <param name="langId">The lang id.</param>
		/// <param name="creatorUID">The creator UID.</param>
		/// <param name="stateId">The state id.</param>
		/// <param name="comment">The comment.</param>
		/// <returns></returns>
		internal static int AddPageVersion2(int pageId, int templateId, int versionNum, int langId, int statusId, Guid creatorUID, DateTime created, Guid editorUID, DateTime edited, int stateId, string comment)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionAdd2]");
			cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("TemplateId", templateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("VersionNum", versionNum, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Created", created, DataParameterType.DateTime));
			cmd.Parameters.Add(new DataParameter("CreatorUID", creatorUID, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Edited", edited, DataParameterType.DateTime));
			cmd.Parameters.Add(new DataParameter("EditorUID", editorUID, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Comment", comment, DataParameterType.NVarChar, 1024));
			return DataService.RunReturnInteger(cmd);
		}
        #endregion

        #region AddDraft
        /// <summary>
        /// Adds the draft.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="templateId">The template id.</param>
        /// <param name="langId">The lang id.</param>
        /// <param name="creatorUID">The creator UID.</param>
        /// <returns></returns>
        public static int AddDraft(int pageId, int templateId, int langId, Guid creatorUID)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionAddDraft]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("TemplateId", templateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("CreatorUID", creatorUID, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        } 
        #endregion

        #region UpdatePageVersion
        /// <summary>
        /// Updates the page version.
        /// </summary>
        /// <param name="versionId">The version id.</param>
        /// <param name="templateId">The template id.</param>
        /// <param name="langId">The lang id.</param>
        /// <param name="oldStatusId">The old status id.</param>
        /// <param name="newStatusId">The new status id.</param>
        /// <param name="editorUID">The editor UID.</param>
        /// <param name="stateId">The state id.</param>
        /// <param name="comment">The comment.</param>
        /// <returns>VersionId for new version of the Page</returns>
        public static int UpdatePageVersion(int versionId, int templateId, int langId, int oldStatusId, int newStatusId,
                            Guid editorUID, int stateId, string comment)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("VersionId", versionId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("TemplateId", templateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("OldStatusId", oldStatusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("NewStatusId", newStatusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("EditorUID", editorUID, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Comment", comment, DataParameterType.NVarChar, 1024));
			return DataService.RunReturnInteger(cmd);
		}
        #endregion

        #region DeletePageVersion
        /// <summary>
        /// Deletes the page version.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        public static void DeletePageVersion(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageVersionId", pageId, DataParameterType.Int));
			DataService.Run(cmd);
        }
        #endregion

        #region GetVersionById
        /// <summary>
        /// PageId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID, StateId, Comment
        /// </summary>
        /// <param name="versionId">The version id.</param>
        /// <returns></returns>
        public static IDataReader GetVersionById(int versionId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("VersionId", versionId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetVersionByPageId
        /// <summary>
        /// VersionId, PageId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID, StateId, Comment
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static IDataReader GetVersionByPageId(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// VersionId, PageId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static DataTable GetVersionByPageIdDT(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region GetVersionByLangId
        /// <summary>
        /// VersionId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID, StateId, Comment
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="langId">The lang id.</param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetVersionByLangId(int pageId, int langId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByLangId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// VersionId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="langId">The lang id.</param>
        /// <returns>DataTable</returns>
        public static DataTable GetVersionByLangIdDT(int pageId, int langId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByLangId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", langId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region GetVersionByStatusId
        /// <summary>
        /// VersionId, PageId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID, StateId, Comment
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="statusId">The status id.</param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetVersionByStatusId(int pageId, int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByStatusId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// VersionId, PageId, TemplateId, VersionNum, LangId, StatusId, Created, CreatorUID, Edited,EditorUID
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="statusId">The status id.</param>
        /// <returns>DataTable</returns>
        public static DataTable GetVersionByStatusIdDT(int pageId, int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByStatusId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }
        #endregion

		#region GetVersionByStateId
        /// <summary>
        /// Get page versions by StateId.
        /// </summary>
        /// <param name="pageId">PageId</param>
        /// <param name="stateId">StateId</param>
        /// <returns>IDataReader</returns>
		public static IDataReader GetVersionByStateId(int pageId, int stateId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByStateId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
		}

        /// <summary>
        /// Get page versions by StateId.
        /// </summary>
        /// <param name="pageId">PageId</param>
        /// <param name="stateId">StateId</param>
        /// <returns>DataTable</returns>
		public static DataTable GetVersionByStateIdDT(int pageId, int stateId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByStateId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
		}
		#endregion

		#region GetVersionByUserId
        /// <summary>
        /// Gets page version by UserId.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>IDataReader</returns>
		public static IDataReader GetVersionByUserId(Guid userId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByUserId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("UserId", userId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
		}

        /// <summary>
        /// Gets page version by UserId.
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>DataTable</returns>
		public static DataTable GetVersionByUserIdDT(Guid userId)
		{
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByUserId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("UserId", userId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
		}
		#endregion

        #region GetByLangIdAndStatusId() : NEW!
        /// <summary>
        /// Gets the by lang id and status id.
        /// VersionId, TemplateId, VersionNum, StatusId, Created, CreatorUID, Edited,EditorUID
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="languageId">The language id.</param>
        /// <param name="statusId">The status id.</param>
        /// <returns>
        /// VersionId, TemplateId, VersionNum, StatusId, Created, CreatorUID, Edited,EditorUID
        /// </returns>
        public static IDataReader GetByLangIdAndStatusId(int pageId, int languageId, int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[main_PageVersionGetByLangIdAndStatusId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("LangId", languageId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetWorkVersionByUserId()
        /// <summary>
        /// Gets the work version by user id.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>
        /// [VersionId], [TemplateId], [VersionNum], [LangId], [StatusId], [Created], [CreatorUID], [Edited], [EditorUID], [StateId], [Comment], [PageId]
        /// </returns>
        public static DataTable GetWorkVersionByUserId(string userName)
        {
			MembershipUser user = Membership.GetUser(userName);

			if (user != null)
			{
				string[] roles = Roles.GetRolesForUser(userName);

				string rolesList = String.Empty;

				if (roles != null && roles.Length > 0)
				{
					foreach (string role in roles)
					{
						if (String.IsNullOrEmpty(rolesList))
							rolesList += role;
						else
							rolesList += "," + role;
					}
				}

				DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_page_PageVersionGetByUserId2]");
				cmd.Parameters = new DataParameters();
                cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("UserId", new Guid(user.ProviderUserKey.ToString()), DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("RoleNames", rolesList, DataParameterType.NVarChar, 1000));
				return DataService.LoadTable(cmd);
			}
			else
				return null;
        }
        #endregion
    }
}