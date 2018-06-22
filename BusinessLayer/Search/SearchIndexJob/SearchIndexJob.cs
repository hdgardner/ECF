using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Search
{
    /// <summary>
    /// Quartz job that builds the search indexes for the application specified in the Group attribute for the job.
    /// </summary>
    public class SearchIndexJob : IStatefulJob
    {
        private readonly ILog _Log = null;
        private SearchManager _SearchManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndexJob"/> class.
        /// </summary>
        public SearchIndexJob()
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

                foreach(AppDto.ApplicationRow row in dto.Application)
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

        /// <summary>
        /// Indexes the application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        void IndexApplication(string applicationName)
        {
            _Log.Info(String.Format("Creating Search Manager for \"{0}\" Application.", applicationName));
            _SearchManager = new SearchManager(applicationName);
            _Log.Info("Created Search Manager.");

            try
            {
                _SearchManager.SearchIndexMessage += new SearchIndexHandler(_SearchManager_SearchIndexMessage);
                _SearchManager.BuildIndex(false);
            }
            catch (Exception ex)
            {
                _Log.Error("Search Manager Failed.", ex);
            }
        }

        /// <summary>
        /// Handles the SearchIndexMessage event of the _SearchManager control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchIndexEventArgs"/> instance containing the event data.</param>
        void _SearchManager_SearchIndexMessage(object source, SearchIndexEventArgs args)
        {
            _Log.Info(String.Format("Percent Complete: {0}%, {1}", Convert.ToInt32(args.CompletedPercentage), args.Message));
        }
    }
}
