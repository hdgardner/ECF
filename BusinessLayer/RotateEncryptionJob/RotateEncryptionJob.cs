using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quartz;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Encryption
{
    /// <summary>
    /// Quartz job that builds the search indexes for the application specified in the Group attribute for the job.
    /// </summary>
    public class RotateEncryptionJob : IStatefulJob
    {
        private readonly ILog _Log = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndexJob"/> class.
        /// </summary>
        public RotateEncryptionJob()
        {
            _Log = LogManager.GetLogger(GetType());
        }

        #region IJob Members

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(JobExecutionContext context)
        {
            string applicationName = context.JobDetail.Group;

            if (String.IsNullOrEmpty(applicationName) || applicationName == "all") // index all applications
            {
                AppDto dto = AppContext.Current.GetApplicationDto();

                foreach (AppDto.ApplicationRow row in dto.Application)
                {
                    IndexApplication(row.Name);
                }
            }
            else
            {
                IndexApplication(applicationName);
            }            
        }
        #endregion

        void IndexApplication(string applicationName)
        {
            _Log.Info(String.Format("Starting to rotate encryption keys job for \"{0}\" application.", applicationName));

            try
            {
                DataCommand cmd = CoreDataHelper.CreateDataCommand();
                cmd.CommandText = String.Format("mdpsp_sys_RotateEncryptionKeys");
                DataService.ExecuteNonExec(cmd);
                _Log.Info("Successfully completed.");
            }
            catch (Exception ex)
            {
                _Log.Error("Rotating encryption keys failed.", ex);
            }
        }
    }
}
