using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using System.Collections.Specialized;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using System.IO;
using Mediachase.Web.Console.Common;
using System.Data;
using System.Text;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Core.StoreLogs
{
    public partial class LogExportDialog : BaseUserControl
    {
        #region LogType, GridId, DataSourceId
        private string LogType
        {
            get
            {
                if (Request["logType"] == null)
                    return null;

                return Request["logType"].ToString();
            }
        }

        private string GridId
        {
            get
            {
                return Request["GridId"].ToString();
            }
        }

        private string DataSourceId
        {
            get
            {
                return Request["DataSourceId"].ToString();
            }
        }
        #endregion

        /// <summary>
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.btnExport.Click += new EventHandler(btnExport_Click); 
            base.OnInit(e);

        }

        void btnExport_Click(object sender, EventArgs e)
        {
            // close dialog
            CommandParameters cp = new CommandParameters("cmdLogExport");
            cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
            cp.CommandArguments.Add("Variant", this.rbList.SelectedValue);
            cp.CommandArguments.Add("LogType", LogType);
            cp.CommandArguments.Add("DataSourceId", DataSourceId);
            cp.CommandArguments.Add("PageProgressDisplay", "false");
            CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
        }
    }
}