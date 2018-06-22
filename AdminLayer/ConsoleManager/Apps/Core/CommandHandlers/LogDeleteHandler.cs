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

namespace Mediachase.Commerce.Manager.Core.CommandHandlers
{
    public class LogDeleteHandler : ICommand
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
                string gridId = cp.CommandArguments["GridId"];

                if (!String.IsNullOrEmpty(cp.CommandName))
                {
                    int error = 0;
                    string errorMessage = String.Empty;
                    try
                    {
                        if (cp.CommandName.Equals("cmdApplicationLogDeleteAll"))
                        {
                            ProcessDeleteCommand(ApplicationLogType.ApplicationLog);
                        }
                        else if (cp.CommandName.Equals("cmdSystemLogDeleteAll"))
                        {
                            ProcessDeleteCommand(ApplicationLogType.SystemLog);
                        }

                        ManagementHelper.SetBindGridFlag(gridId);
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

        /// <summary>
        /// Processes the delete command.
        /// </summary>
        void ProcessDeleteCommand(ApplicationLogType type)
        {
            LogDto dto = new LogDto();
            int totalrecords = 0;

            if (type == ApplicationLogType.ApplicationLog)
            {
                dto = LogManager.GetAppLog("", "", "", DateTime.MinValue, 0, int.MaxValue, ref totalrecords);
            }
            else if (type == ApplicationLogType.SystemLog)
            {
                dto = LogManager.GetSystemLog("", "", DateTime.MinValue, 0, int.MaxValue, ref totalrecords);
            }

            if (dto.ApplicationLog.Rows.Count > 0)
            {
                for (int i = 0; i < dto.ApplicationLog.Rows.Count; i++)
                {
                    dto.ApplicationLog.Rows[i].Delete();
                }
                LogManager.SaveAppLog(dto);
            }
        }

        enum ApplicationLogType
        {
            SystemLog,
            ApplicationLog
        }
    }
}