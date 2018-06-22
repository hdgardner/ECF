using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Commerce.Manager.Core.ImportExport
{
	public partial class FileDownload : System.Web.UI.Page
	{
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
		public string FileName
		{
			get
			{
				object file = Request.QueryString["file"];

				if (file == null)
					return String.Empty;
				else
					return Server.UrlDecode(file.ToString());
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
			response.AddHeader("content-disposition", string.Format("attachment; filename={0}", System.IO.Path.GetFileName(FileName)));
			response.ContentType = "application/octet-stream";
			response.TransmitFile(FileName);
			response.Flush();
            response.End();
		}
	}
}