using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Web.Console.Reporting
{
    /// <summary>
    /// Summary description for RdlcEngine
    /// </summary>
    public class RdlcEngine
    {
        public RdlcEngine()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /*

        #region bind control
        public static void BindControl(Microsoft.Reporting.WebForms.ReportViewer rv, DataSet data, string name)
        {
            string virtualRldc = BuildRDLC(data, name);
            BindControl(rv, data, name, virtualRldc);
        }

        public static void BindControl(Microsoft.Reporting.WebForms.ReportViewer rv, DataSet data, string name, string virtualRldc)
        {
            rv.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;

            Microsoft.Reporting.WebForms.ReportDataSource rds = new Microsoft.Reporting.WebForms.ReportDataSource();
            rds.Name = name + "_Table";
            rds.Value = data.Tables[0];

            Microsoft.Reporting.WebForms.LocalReport r = rv.LocalReport;
            r.ReportPath = virtualRldc;
            r.DataSources.Add(rds);
        }
        #endregion

        #region RDLC
        /// <summary>
        /// constructs a simple report RDLC file based on a DataSet
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string BuildRDLC(DataSet data, string name)
        {
            // establish some file names
            string virtualXslt = "xslt/rdlc.xsl";
            string virtualRdlc = "rdlc/" + name + ".rdlc";
            string virtualSchema = "rdlc/" + name + ".schema";

            // set the NAME on the DataSet
            // this may or may not be necessary, but the RDLC and DataSet
            // will both have the same name if this is done.
            data.DataSetName = name;

            // write the DataSet Schema to a file
            // we should be passing a DataSet with only one DataTable
            // the rdlc.xsl does not account for multiple DataTables
            string physicalSchema = HttpContext.Current.Server.MapPath(virtualSchema);
            data.WriteXmlSchema(physicalSchema);

            // load the DataSet schema in a DOM
            XmlDocument xmlDomSchema = new XmlDocument();
            xmlDomSchema.Load(physicalSchema);

            // append the NAME to the schema DOM 
            // this is so we can pick it up in the rdlc.xsl
            // and use it
            xmlDomSchema.DocumentElement.SetAttribute("Name", name + "_Table");

            // transform the Schema Xml with rdlc.xsl
            string physicalXslt = HttpContext.Current.Server.MapPath(virtualXslt);
            string xml = HRWebsite.General.TransformXml(xmlDomSchema.OuterXml, physicalXslt);

            // save off the resultng RDLC file
            string physicalRdlc = HttpContext.Current.Server.MapPath(virtualRdlc);
            XmlDocument xmlDomRdlc = new XmlDocument();
            xmlDomRdlc.LoadXml(xml);
            xmlDomRdlc.Save(physicalRdlc);

            // return the virtual path of the RDLC file
            // this is needed by the asp:ReportViewer
            return virtualRdlc;
        }
        #endregion

        #region Render
        public static byte[] RenderReport(DataSet data, string name, string type)
        {
            Microsoft.Reporting.WebForms.ReportDataSource rds = new Microsoft.Reporting.WebForms.ReportDataSource();
            rds.Name = name + "_Table";
            rds.Value = data.Tables[0];

            string virtualRdlc = BuildRDLC(data, name);
            Microsoft.Reporting.WebForms.LocalReport lr = new Microsoft.Reporting.WebForms.LocalReport();
            lr.ReportPath = HttpContext.Current.Server.MapPath(virtualRdlc);
            lr.DataSources.Add(rds);

            return RenderReport(lr, name, type);
        }

        public static byte[] RenderReport(Microsoft.Reporting.WebForms.LocalReport lr, string name, string type)
        {
            string extension = string.Empty;
            string mimeType = string.Empty;
            switch (type)
            {
                case "PDF":
                    extension = "pdf";
                    mimeType = "application/pdf";
                    break;
                case "Excel":
                    extension = "xls";
                    mimeType = "application/vnd.excel";
                    break;
                case "Image":
                    extension = "emf";
                    mimeType = "application/image";
                    break;
                default:
                    throw new Exception("Unrecognized type: " + type + ".  Type must be PDF, Excel or Image.");
            }

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<DeviceInfo>");
            sb.Append("<OutputFormat>");
            sb.Append(type);
            sb.Append("</OutputFormat>");
            sb.Append("<PageWidth>11in</PageWidth>");
            sb.Append("<PageHeight>8.5in</PageHeight>");
            sb.Append("<MarginTop>1in</MarginTop>");
            sb.Append("<MarginLeft>1in</MarginLeft>");
            sb.Append("<MarginRight>1in</MarginRight>");
            sb.Append("<MarginBottom>1in</MarginBottom>");
            sb.Append("</DeviceInfo>");
            string deviceInfo = sb.ToString();

            string encoding;
            Microsoft.Reporting.WebForms.Warning[] warnings;
            string[] streams;
            byte[] result;

            //Render the report
            result = lr.Render(
                type,
                deviceInfo,
                out mimeType,
                out encoding,
                out extension,
                out streams,
                out warnings);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = mimeType;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + name + "." + extension);
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.End();

            return result;
        }
        #endregion
         * */
    } 
}
