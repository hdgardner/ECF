using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Data.Provider;
using System.Collections.Generic;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;
using System.IO;
using System.Text;
using System.Collections;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Core.DataSources;
using Mediachase.Commerce.Core.Log;

namespace Mediachase.Commerce.Manager.Core.CommandHandlers
{
    public class LogExportHandler : ICommand
    {
        #region ICommand Members
        /// <summary>
        /// Invokes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="element">The element.</param>
        public void Invoke(object sender, object element)
        {
            if (element is CommandParameters)
            {
                CommandParameters cp = (CommandParameters)element;

                string logType = cp.CommandArguments["LogType"];
                string variant = cp.CommandArguments["Variant"];
                string dataSourceId = cp.CommandArguments["DataSourceId"];
                
                ApplicationLogDataSource applicationLogDataSource = ManagementHelper.GetControlFromCollection<ApplicationLogDataSource>(((Control)sender).Page.Controls, dataSourceId);
                if (applicationLogDataSource != null)
                {
                    string exportPageUrl = "~/Apps/Core/StoreLogs/LogExport.aspx";

                    int error = 0;
                    string errorMessage = String.Empty;
                    try
                    {
                        exportPageUrl += String.Concat("?type=", applicationLogDataSource.DataMode.ToString());

                        if (variant.Equals("View") || variant.Equals("CurrentPage"))
                        {
                            exportPageUrl += GetApplicationLogDataSourceUrlParamaters(applicationLogDataSource);

                            if (variant.Equals("CurrentPage"))
                            {
                                exportPageUrl += String.Concat("&RecordsToRetrieve=", applicationLogDataSource.Options.RecordsToRetrieve);
                                exportPageUrl += String.Concat("&StartingRecord=", applicationLogDataSource.Options.StartingRecord);
                            }
                        }

                        ((Control)sender).Page.Response.Redirect(exportPageUrl, true);
                    }
                    catch (Exception ex)
                    {
                        error++;
                        errorMessage = ex.Message;
                    }

                    if (error > 0)
                    {
                        errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
                        ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
                            String.Format("alert('{0}{1}');", "Failed to delete item(s). Error: ", errorMessage), true);
                    }
                }
            }
        }
        #endregion

        private string GetApplicationLogDataSourceUrlParamaters(ApplicationLogDataSource applicationLogDataSource)
        {
            string retval = String.Empty;

            if (applicationLogDataSource != null)
            {
                if (applicationLogDataSource.DataMode == ApplicationLogDataSource.ApplicationLogDataMode.ApplicationLog)
                {
                    string sourceKey = applicationLogDataSource.Parameters.SourceKey;
                    if (!String.IsNullOrEmpty(sourceKey))
                        retval += String.Concat("&sourceKey=", sourceKey);
                }

                string objectType = applicationLogDataSource.Parameters.ObjectType;
                if(!String.IsNullOrEmpty(objectType))
                    retval += String.Concat("&objectType=", objectType);

                string operation = applicationLogDataSource.Parameters.Operation;
                if (!String.IsNullOrEmpty(operation))
                    retval += String.Concat("&operation=", operation);

                DateTime created = applicationLogDataSource.Parameters.Created;
                if (!created.Equals(DateTime.MinValue))
                    retval += String.Concat("&created=", created.ToString("yyyy-mm-dd"));
            }

            return retval;
        }
    }
}