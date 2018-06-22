using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Apps.Core.Search.Tabs
{
    public partial class IndexTab : System.Web.UI.UserControl
    {
        private Mediachase.Search.SearchManager _SearchManager;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RebuildIndex(object sender, EventArgs e)
        {
            ProcessRebuildButtonEvent(true);
        }

        protected void BuildIndex(object sender, EventArgs e)
        {
            ProcessRebuildButtonEvent(false);
        }

        /// <summary>
        /// Processes the rebuild button event.
        /// </summary>
        protected void ProcessRebuildButtonEvent(bool rebuild)
        {
            _SearchManager = new Mediachase.Search.SearchManager(AppContext.Current.ApplicationName);

            // start export in a new thread
            if (rebuild)
                ProgressControl1.StartOperation(new System.Threading.ThreadStart(StartIndexing));
            else
                ProgressControl1.StartOperation(new System.Threading.ThreadStart(StartIndexing2));
        }

        /// <summary>
        /// Starts the indexing.
        /// </summary>
        private void StartIndexing()
        {
            try
            {
                string result = String.Empty;

                _SearchManager.SearchIndexMessage += new Mediachase.Search.SearchIndexHandler(_SearchManager_SearchIndexMessage);
                _SearchManager.BuildIndex(true);

                // set status to 100, meaning that operation completed
                //Session[_ProgressStatus] = 100;
            }
            catch (Exception ex)
            {
                ProgressControl1.AddProgressMessageText(ex.Message, true, -1);
            }
            finally
            {
            }
        }

        private void StartIndexing2()
        {
            try
            {
                string result = String.Empty;

                _SearchManager.SearchIndexMessage += new Mediachase.Search.SearchIndexHandler(_SearchManager_SearchIndexMessage);
                _SearchManager.BuildIndex(false);

                // set status to 100, meaning that operation completed
                //Session[_ProgressStatus] = 100;
            }
            catch (Exception ex)
            {
                ProgressControl1.AddProgressMessageText(ex.Message, true, -1);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Handles the SearchIndexMessage event of the _SearchManager control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="Mediachase.Search.SearchIndexEventArgs"/> instance containing the event data.</param>
        void _SearchManager_SearchIndexMessage(object source, Mediachase.Search.SearchIndexEventArgs args)
        {
            ProgressControl1.AddProgressMessageText(args.Message, false, Convert.ToInt32(args.CompletedPercentage));
        }
    }
}