using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quartz;
using Mediachase.Commerce.Marketing.Data;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Marketing;

namespace Mediachase.Commerce.Services
{
    /// <summary>
    /// Quartz job that executes the stored procedure which cancels all the promotion reservations.
    /// </summary>
    public class RemoveExpiredPromoReservationsJob : IStatefulJob
    {
        private readonly ILog _Log = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveExpiredPromoReservationsJob"/> class.
        /// </summary>
        public RemoveExpiredPromoReservationsJob()
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

                if (dto.Application.Count > 0)
                {
                    foreach (AppDto.ApplicationRow row in dto.Application)
                    {
                        RemoveExpired(row.ApplicationId);
                    }
                }
            }
            else
            {
                AppDto dto = AppContext.Current.GetApplicationDto(applicationName);
                if(dto.Application.Count > 0)
                    RemoveExpired(dto.Application[0].ApplicationId);
            }
        }
        #endregion

        /// <summary>
        /// Removes the expired.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        void RemoveExpired(Guid applicationId)
        {
            _Log.Info(String.Format("Starting to cancel expired promotion job for \"{0}\" application.", applicationId));

            try
            {
                DataCommand cmd = MarketingDataHelper.CreateDataCommand();
                cmd.CommandText = String.Format("ecf_mktg_CancelExpiredPromoReservations");
                cmd.Parameters = new DataParameters();
                cmd.Parameters.Add(new DataParameter("ApplicationId", applicationId, DataParameterType.UniqueIdentifier));
                cmd.Parameters.Add(new DataParameter("Expires", MarketingConfiguration.Instance.ReservationTimeout, DataParameterType.Int));

                DataService.ExecuteNonExec(cmd);
                _Log.Info("Successfully completed.");
            }
            catch (Exception ex)
            {
                _Log.Error("Failed to cancel expired promotion reservations.", ex);
            }
        }
    }
}
