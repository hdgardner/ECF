using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.Security;
using Mediachase.Cms.Data;
using Mediachase.Commerce.Core;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
    #region -- Workflow --
	public static class Workflow
    {
        #region AddWorkflow
        /// <summary>
        /// Adds the workflow.
        /// </summary>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns></returns>
        public static int AddWorkflow(string friendlyName, bool isDefault)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_Add]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        } 
        #endregion

        #region DeleteWorkflow
        /// <summary>
        /// Deletes the workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        public static void DeleteWorkflow(int workflowId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_Delete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region UpdateWorkflow
        /// <summary>
        /// Updates the workflow.
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        public static void UpdateWorkflow(int workflowId, string friendlyName, bool isDefault)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_Update]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
			cmd.Parameters.Add(new DataParameter("IsDefault", isDefault, DataParameterType.Bit));
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			DataService.Run(cmd);
        }
        #endregion

        #region LoadAll
        /// <summary>
        /// WorkflowId, FriendlyName
        /// </summary>
        /// <returns></returns>
        public static IDataReader LoadAll()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_GetAll]");
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// WorkflowId, FriendlyName, IsDefault
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadAllDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_GetAll]");
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region LoadById
        /// <summary>
        /// WorkflowId, FriendlyName, IsDefault
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        /// <returns></returns>
        public static IDataReader LoadById(int workflowId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_GetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        } 
        #endregion

        #region LoadDefault()
        /// <summary>
        /// Loads the default workflow.
        /// </summary>
        /// <returns>WorkflowId, FriendlyName, IsDefault</returns>
        public static IDataReader LoadDefault()
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_Workflow_GetDefault]");
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region -- PageState --
        /// <summary>
        /// Pages the state get by id.
        /// </summary>
        /// <param name="stateId">The state id.</param>
        /// <returns></returns>
        public static IDataReader PageStateGetById(int stateId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_PageStateGetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StateId", stateId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion
    }
    #endregion

    #region -- WorkflowStatus --
	public static class WorkflowStatus
    {
        #region const: DraftId
        /// <summary>
        /// Gets the draft id.
        /// </summary>
        /// <value>The draft id.</value>
        public static int DraftId
        {
            get { return GetDraftStatus(); }
        } 
        #endregion

        #region AddStatus
        /// <summary>
        /// Adds the status.
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        /// <returns></returns>
        public static int AddStatus(int workflowId, int weight, string friendlyName)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_Add]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Weight", weight, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
            //cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region DeleteStatus
        /// <summary>
        /// Deletes the status.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        public static void DeleteStatus(int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_Delete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			DataService.Run(cmd);
        }
        #endregion

        #region UpdateStatus
        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <param name="workflowId">The workflow id.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="friendlyName">Name of the friendly.</param>
        public static void UpdateStatus(int statusId, int workflowId, int weight, string friendlyName)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_Update]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Weight", weight, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("FriendlyName", friendlyName, DataParameterType.NVarChar, 250));
			DataService.Run(cmd);
        }
        #endregion

        #region LoadAll
        /// <summary>
        /// StatusId, WorkflowId, Weight, FriendlyName
        /// </summary>
        /// <returns></returns>
        public static IDataReader LoadAll()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetAll]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// StatusId, WorkflowId, Weight, FriendlyName
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadAllDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetAll]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region LoadById
        /// <summary>
        /// StatusId, WorkflowId, Weight, FriendlyName
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static IDataReader LoadById(int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadByWorkflowId
        /// <summary>
        /// StatusId, WorkflowId, Weight, FriendlyName
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        /// <returns></returns>
        public static IDataReader LoadByWorkflowId(int workflowId)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetByWorkflowId]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("WorkflowId", workflowId, DataParameterType.Int));
            return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetNext
        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static int GetNext(int statusId)
        {
            int nextStatusId = -1;
            using (IDataReader reader = LoadById(statusId))
            {
                if (reader.Read())
                {
                    int curWeight = (int)reader["Weight"];
                    using (IDataReader reader2 = LoadByWorkflowId((int)reader["WorkflowId"]))
                    {
                        int dMin = Int32.MaxValue;

                        while (reader2.Read())
                        {

                            if ((int)reader2["Weight"] - curWeight < dMin && ((int)reader2["Weight"] - curWeight > 0) && (curWeight != (int)reader2["Weight"]))
                            {
                                dMin = (int)reader2["Weight"] - curWeight;
                                nextStatusId = (int)reader2["StatusId"];
                            }
                        }

                        reader2.Close();
                    }
                }

                reader.Close();
            }
            return nextStatusId;
        }
        #endregion

        #region GetPrevious
        /// <summary>
        /// Gets the previous.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static int GetPrevious(int statusId)
        {
            int prevStatusId = -1;
            using (IDataReader reader = LoadById(statusId))
            {
                if (reader.Read())
                {
                    //if ((int)reader["Weight"] < 0) continue;
                    int curWeight = (int)reader["Weight"];
                    using (IDataReader reader2 = LoadByWorkflowId((int)reader["WorkflowId"]))
                    {
                        int dMin = Int32.MaxValue;

                        while (reader2.Read())
                        {
                            if ((int)reader2["Weight"] < 0) continue;
                            if (curWeight - (int)reader2["Weight"] < dMin && (curWeight - (int)reader2["Weight"] > 0) && (curWeight != (int)reader2["Weight"]))
                            {
                                dMin = curWeight - (int)reader2["Weight"];
                                prevStatusId = (int)reader2["StatusId"];
                            }
                        }

                        reader2.Close();
                    }
                }

                reader.Close();
            }
            return prevStatusId;
        }
        #endregion

        #region GetLastByWorkflowId
        /// <summary>
        /// Gets the last by workflow id.
        /// </summary>
        /// <param name="workflowId">The workflow id.</param>
        /// <returns></returns>
        public static int GetLastByWorkflowId(int workflowId)
        {
            int lastStatusId = -1;
            int tmpStatus = -1;

            using (IDataReader reader2 = LoadByWorkflowId(workflowId))
            {
                if (reader2.Read())
                    tmpStatus = (int)reader2["StatusId"];

                reader2.Close();
            }

            while (true)
            {
                lastStatusId = GetNext(tmpStatus);
                if (lastStatusId != -1) tmpStatus = lastStatusId;
                else break;
            }

            return tmpStatus;
        }
        #endregion

        #region GetLast
        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static int GetLast(int statusId)
        {
            int lastStatusId = -1;
            int tmpStatus = statusId;
            if (statusId == -1)
            {
                int workflowId = -1;
                using (IDataReader reader = Workflow.LoadDefault())
                {
                    if (reader.Read())
                        workflowId = (int)reader["WorkflowId"];


                    reader.Close();
                }
                using (IDataReader reader2 = WorkflowStatus.LoadByWorkflowId(workflowId))
                {
                    if (reader2.Read())
                        tmpStatus = (int)reader2["StatusId"];

                    reader2.Close();
                }
            }

            while (true)
            {
                lastStatusId = GetNext(tmpStatus);
                if (lastStatusId != -1) tmpStatus = lastStatusId;
                else break;
            }

            return tmpStatus;
        }

        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <returns></returns>
        public static int GetLast()
        {
            string cacheKey = CmsCache.CreateCacheKey("workflow-published-status");

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                return (int)cachedObject;
            }

            int status = GetLast(-1);

            // Insert to the cache collection
            CmsCache.Insert(cacheKey, status, CmsConfiguration.Instance.Cache.WorkflowTimeout);

            return status;
        }
        #endregion

        #region GetArcStatus
        /// <summary>
        /// Gets the arc status.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static int GetArcStatus(int statusId)
        {
            string cacheKey = CmsCache.CreateCacheKey("workflow-archive-status");

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                return (int)cachedObject;
            }

            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetArcStatus]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));

            int status = DataService.RunReturnInteger(cmd);

            // Insert to the cache collection
            CmsCache.Insert(cacheKey, status, CmsConfiguration.Instance.Cache.WorkflowTimeout);

            return status;
        }
        #endregion

        #region GetDraftStatus
        /// <summary>
        /// Gets the draft status.
        /// </summary>
        /// <returns></returns>
        public static int GetDraftStatus()
        {
            string cacheKey = CmsCache.CreateCacheKey("workflow-draft-status");

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                return (int)cachedObject;
            }

            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatus_GetDraftStatus]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));

            int status = DataService.RunReturnInteger(cmd);

            // Insert to the cache collection
            CmsCache.Insert(cacheKey, status, CmsConfiguration.Instance.Cache.WorkflowTimeout);

            return status;
        }
        #endregion

        #region GetStatusName
        /// <summary>
        /// Gets the status friendly name.
        /// </summary>
        /// <returns></returns>
        public static string GetStatusName(int statusId)
        {
            string cacheKey = CmsCache.CreateCacheKey(String.Format("workflow-statusname{0}", statusId));

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                return (string)cachedObject;
            }
            
            string statusName = String.Empty;

            using(IDataReader reader = WorkflowStatus.LoadById(statusId))
            {
                if (reader.Read())
                {
                    statusName = reader["FriendlyName"].ToString();
                }

                reader.Close();
            }

            // Insert to the cache collection
            CmsCache.Insert(cacheKey, statusName, CmsConfiguration.Instance.Cache.WorkflowTimeout);

            return statusName;
        }
        #endregion

    }
    #endregion

    #region -- WorkflowStatusAccess --
	public static class WorkflowAccess
    {
        private static readonly CultureInfo _CultureInfo = CultureInfo.InvariantCulture;

        #region AddAccess
        /// <summary>
        /// Adds the access.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static int AddAccess(int statusId, string roleId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_Add]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region DeleteAccess
        /// <summary>
        /// Deletes the access.
        /// </summary>
        /// <param name="statusAccessId">The status access id.</param>
        public static void DeleteAccess(int statusAccessId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_Delete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusAccessId", statusAccessId, DataParameterType.Int));
			DataService.Run(cmd);
        }
        #endregion

        #region UpdateAccess
        /// <summary>
        /// Updates the access.
        /// </summary>
        /// <param name="statusAccessId">The status access id.</param>
        /// <param name="statusId">The status id.</param>
        /// <param name="roleId">The role id.</param>
        public static void UpdateAccess(int statusAccessId, int statusId, string roleId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_Update]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusAccessId", statusAccessId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region LoadAll
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <returns></returns>
        public static IDataReader LoadAll()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadAllDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region LoadById
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="statusAccessId">The status access id.</param>
        /// <returns></returns>
        public static IDataReader LoadById(int statusAccessId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetById]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("StatusAccessId", statusAccessId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadByStatusId
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static IDataReader LoadByStatusId(int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetByStatusId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadByRoleId
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static IDataReader LoadByRoleId(string roleId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetByRoleId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("EveryoneRoleId", AppRoles.EveryoneRole, DataParameterType.NVarChar, 256));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadListByRoleId
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static ArrayList LoadListByRoleId(string roleId)
        {
            ArrayList result = new ArrayList();
            using (IDataReader reader = LoadByRoleId(roleId))
            {
                while (reader.Read())
                {
                    result.Add(reader["StatusId"]);
                }

                reader.Close();
            }
            return result;
        }

        #endregion

        #region LoadByRoleIdStatusId
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static IDataReader LoadByRoleIdStatusId(string roleId, int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetByRoleIdStatusId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("EveryoneRoleId", AppRoles.EveryoneRole, DataParameterType.NVarChar, 256));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region LoadByRoleIdStatusIdNotEveryone
        /// <summary>
        /// StatusAccessId, StatusId, RoleId
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        public static IDataReader LoadByRoleIdStatusIdNotEveryone(string roleId, int statusId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetByRoleIdStatusIdNotEveryone]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("RoleId", roleId, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("StatusId", statusId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("EveryoneRoleId", AppRoles.EveryoneRole, DataParameterType.NVarChar, 256));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetMaxStatus
        /// <summary>
        /// Get highest StatusId for current RolesId
        /// </summary>
        /// <param name="rolesId">The roles id.</param>
        /// <param name="lastStatus">The last status.</param>
        /// <returns></returns>
        public static int GetMaxStatus(string[] rolesId, int lastStatus)
        {
            int curStatus = lastStatus;
            while (true)
            {
                foreach (string roleId in rolesId)
                {
                    using (IDataReader reader = LoadByRoleIdStatusId(roleId, curStatus))
                    {
                        if (reader.Read())
                        {
                            reader.Close();
                            return curStatus;
                        }

                        reader.Close();
                    }
                }
                curStatus = WorkflowStatus.GetPrevious(curStatus);
                if (curStatus == -1) return -1;
            }
        }
        #endregion

        #region GetNextStatusId
        /// <summary>
        /// Gets the next status id.
        /// </summary>
        /// <param name="currentStatus">The current status.</param>
        /// <returns></returns>
        public static int GetNextStatusId(int currentStatus/*, Guid userId*/)
        {
            int statusId = -1;

			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetNextStatus]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StatusId", currentStatus, DataParameterType.Int));

            using (IDataReader reader = DataService.LoadReader(cmd).DataReader)
            {
                while (reader.Read())
                {
                    statusId = (int)reader["StatusId"];
                    string role = (string)reader["RoleId"];

					if (String.Compare(role, AppRoles.EveryoneRole, true, _CultureInfo) == 0 || Roles.IsUserInRole(role))
                    {
                        reader.Close();
                        return statusId;
                    }
                }

                reader.Close();
            }

            return statusId;
        } 
        #endregion

        #region GetPrevStatusId
        /// <summary>
        /// Gets the prev status id.
        /// </summary>
        /// <param name="currentStatus">The current status.</param>
        /// <returns></returns>
        public static int GetPrevStatusId(int currentStatus/*, Guid userId*/)
        {
            int statusId = -1;

			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_WorkflowStatusAccess_GetPrevStatus]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("StatusId", currentStatus, DataParameterType.Int));

			using (IDataReader reader = DataService.LoadReader(cmd).DataReader)
            {
                while (reader.Read())
                {
                    statusId = (int)reader["StatusId"];
                    string role = (string)reader["RoleId"];

					if (String.Compare(role, AppRoles.EveryoneRole, true, _CultureInfo) == 0 || Roles.IsUserInRole(role))
                    {
                        reader.Close();
                        return statusId;
                    }
                }

                reader.Close();
            }

            return statusId;
        }
        #endregion

        #region GetNextStatus
        /// <summary>
        /// Get next StatusId for current RolesId
        /// </summary>
        /// <param name="rolesId">The roles id.</param>
        /// <param name="currentStatus">The current status.</param>
        /// <returns></returns>
        public static int GetNextStatus(string[] rolesId, int currentStatus)
        {
            int curStatus = currentStatus;
            while (true)
            {
                //while not last
                while (curStatus != -1)
                {
                    curStatus = WorkflowStatus.GetNext(curStatus);
                    //foreach Role 
                    foreach (string roleId in rolesId)
                    {
                        using (IDataReader reader = LoadByRoleIdStatusId(roleId, curStatus))
                        {
                            if (reader.Read())
                            {
                                reader.Close();
                                return curStatus;
                            }

                            reader.Close();
                        }
                    }
                }
                if (curStatus == -1) return -1;
            }
        } 
        #endregion

        #region GetPrevStatus
        /// <summary>
        /// Get previous StatusId for current RolesId
        /// </summary>
        /// <param name="rolesId">The roles id.</param>
        /// <param name="currentStatus">The current status.</param>
        /// <returns></returns>
        public static int GetPrevStatus(string[] rolesId, int currentStatus)
        {
            int curStatus = currentStatus;
            while (true)
            {
                //while not last
                while (curStatus != -1)
                {
                    curStatus = WorkflowStatus.GetPrevious(curStatus);
                    //foreach Role 
                    foreach (string roleId in rolesId)
                    {
                        using (IDataReader reader = LoadByRoleIdStatusId(roleId, curStatus))
                        {
                            if (reader.Read())
                            {
                                reader.Close();
                                return curStatus;
                            }

                            reader.Close();
                        }
                    }
                }
                if (curStatus == -1) return -1;
            }
        }
        #endregion
    }
    #endregion
}