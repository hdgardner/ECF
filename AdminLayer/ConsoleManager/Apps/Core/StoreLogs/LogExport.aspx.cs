using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.IO;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Core.DataSources;

namespace Mediachase.Commerce.Manager.Apps.Core.StoreLogs
{
    public partial class LogExport : System.Web.UI.Page
    {
        const ApplicationLogDataSource.ApplicationLogDataMode applicationLog = ApplicationLogDataSource.ApplicationLogDataMode.ApplicationLog;
        const ApplicationLogDataSource.ApplicationLogDataMode systemLog = ApplicationLogDataSource.ApplicationLogDataMode.SystemLog;

        #region LogType
        private string LogType
        {
            get
            {
                if (Request["type"] == null)
                    return String.Empty;

                return Request["type"].ToString();
            }
        }
        #endregion

        #region RecordsToRetrieve, StartingRecord
        private int RecordsToRetrieve
        {
            get
            {
                if (Request["RecordsToRetrieve"] == null)
                    return int.MaxValue;

                return Int32.Parse(Request["RecordsToRetrieve"]);
            }
        }

        private int StartingRecord
        {
            get
            {
                if (Request["StartingRecord"] == null)
                    return 0;

                return Int32.Parse(Request["StartingRecord"]);
            }
        }
        #endregion

        #region SourceKey, Operation, ObjectType, Created
        private string Operation
        {
            get
            {
                if (Request["operation"] == null)
                    return String.Empty;

                return Request["operation"].ToString();
            }
        }

        private string SourceKey
        {
            get
            {
                if (Request["sourceKey"] == null)
                    return String.Empty;

                return Request["sourceKey"].ToString();
            }
        }

        private string ObjectType
        {
            get
            {
                if (Request["objectType"] == null)
                    return String.Empty;

                return Request["objectType"].ToString();
            }
        }

        private DateTime Created
        {
            get
            {
                if (Request["created"] == null)
                    return DateTime.MinValue;

                return DateTime.Parse(Request["created"].ToString());
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityManager.CheckRolePermission("core:admin:applog:mng:view,core:admin:syslog:mng:view");

            ProcessExportCommand(LogType);
        }

        /// <summary>
        /// Processes the export command.
        /// </summary>
        private void ProcessExportCommand(string type)
        {
            LogDto dto = new LogDto();
            int totalrecords = 0;

            if (type.Equals(applicationLog.ToString()))
            {
                dto = LogManager.GetAppLog(SourceKey, Operation, ObjectType, Created, StartingRecord, RecordsToRetrieve, ref totalrecords);
            }
            else if (type.Equals(systemLog.ToString()))
            {
                dto = LogManager.GetSystemLog(Operation, ObjectType, Created, StartingRecord, RecordsToRetrieve, ref totalrecords);
            }

            if (dto.ApplicationLog != null)
            {
                string fileName = String.Format("{0}.csv", type.ToString());

                StringWriter sw = DataTable2CSV((DataTable)dto.ApplicationLog);
                ExportCSV(sw.ToString(), fileName);
            }
        }

        /// <summary>
        /// Exports data table to scv format
        /// </summary>
        /// <returns></returns>
        public static StringWriter DataTable2CSV(DataTable dtable)
        {
            string _ExcelCharDelimiter = "\t";
            string _ExcelDoubleQuote = "\"";
            StringBuilder sb = new StringBuilder();
            StringWriter wr = new StringWriter();

            int icolcount = dtable.Columns.Count;
            for (int i = 0; i < icolcount; i++)
            {
                wr.Write(dtable.Columns[i]);
                if (i < icolcount - 1)
                {
                    wr.Write(_ExcelCharDelimiter);
                }
            }
            wr.Write(wr.NewLine);
            foreach (DataRow drow in dtable.Rows)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    if (!Convert.IsDBNull(drow[i]))
                    {
                        string value = drow[i].ToString();
                        if (value.Contains(_ExcelCharDelimiter) || value.Contains(_ExcelDoubleQuote))
                        {
                            value = value.Replace(_ExcelDoubleQuote, String.Concat(_ExcelDoubleQuote, _ExcelDoubleQuote));
                            value = String.Concat(_ExcelDoubleQuote, value, _ExcelDoubleQuote);
                        }
                        wr.Write(value);
                    }
                    if (i < icolcount - 1)
                    {
                        wr.Write(_ExcelCharDelimiter);
                    }
                }
                wr.Write(wr.NewLine);
            }

            wr.Close();
            return wr;
        }

        /// <summary>
        /// Exports in csv format.
        /// </summary>
        /// <param name="txtString"></param>
        /// <param name="filename"></param>
        public static void ExportCSV(string txtString, string filename)
        {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.Charset = "utf-16";
            response.AddHeader("Content-Type", "text/csv");
            response.AddHeader("content-disposition", String.Format("attachment; filename={0}", filename));

            System.IO.StringWriter s = new System.IO.StringWriter();
            response.BinaryWrite(new byte[] { 0xff, 0xfe });
            response.BinaryWrite(Encoding.Unicode.GetBytes(txtString));
            response.End();
        }
    }
}
