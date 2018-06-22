using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;

namespace Mediachase.Commerce.Manager.Core.Controls.GridTemplates
{
	public partial class FileListEditTemplate : System.Web.UI.UserControl
	{
		private string _FilePathArgument = String.Empty;

        /// <summary>
        /// Gets or sets the file path argument.
        /// </summary>
        /// <value>The file path argument.</value>
		public string FilePathArgument
		{
			get { return _FilePathArgument; }
			set { _FilePathArgument = value; }
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			btnDownload.Command += new CommandEventHandler(btnDownload_Command);
			btnDelete.Command += new CommandEventHandler(btnDelete_Command);
			base.OnInit(e);
		}

        /// <summary>
        /// Handles the Command event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		protected void btnDelete_Command(object sender, CommandEventArgs e)
		{
			if (String.Compare(e.CommandName, "Delete", true) == 0)
			{
				// get the file
				GridServerTemplateContainer container = this.Parent as GridServerTemplateContainer;
				if (container != null)
				{
					object obj = container.DataItem[FilePathArgument];
					if (obj != null)
					{
						string filePath = obj.ToString();
						if (File.Exists(filePath))
						{
							// delete file
							try
							{
								File.Delete(filePath);
							}
							catch(Exception ex)
							{
								// TODO: handle exception here
                                // Dummy call on ex to suppress warning
                                ex.ToString();
							}
						}
					}
				}
			}

			// rebind control that contains parent grid
			UserControl parentControl = this.Parent.Parent.Parent as UserControl;
			if (parentControl != null)
				parentControl.DataBind();
		}

        /// <summary>
        /// Handles the Command event of the btnDownload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		protected void btnDownload_Command(object sender, CommandEventArgs e)
		{
			if (String.Compare(e.CommandName, "Download", true) == 0)
			{
				// get the file
				GridServerTemplateContainer container = this.Parent as GridServerTemplateContainer;
				if (container != null)
				{
					object obj = container.DataItem[FilePathArgument];
					if (obj != null)
					{
						string filePath = obj.ToString();
						if (File.Exists(filePath))
						{
							// shouldn't call methods like response.Write or TransmitFile here since the current control may be inside an UpdatePanel
							string redirectString = "~/Apps/Core/ImportExport/FileDownload.aspx?file=" + Server.UrlEncode(filePath);
							Response.Redirect(redirectString);

							// download the file
							//System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
							//response.AddHeader("content-disposition", string.Format("attachment; filename={0}", Path.GetFileName(filePath)));
							//response.ContentType = "application/octet-stream";
							//response.TransmitFile(filePath);
							//response.Flush();
						}
					}
				}
			}
		}
	}
}