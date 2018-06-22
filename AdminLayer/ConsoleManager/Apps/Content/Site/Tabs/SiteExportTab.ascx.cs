using System;
using System.IO;
using System.Text;
using System.Web;
using Mediachase.Cms.ImportExport;
using Mediachase.Cms.Pages;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Cms;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Content.Site.Tabs
{
	/// <summary>
	///	Used for exporting Site.
	/// </summary>
    public partial class SiteExportTab : BaseUserControl
	{
		private HttpContext _CurrentHttpContext = HttpContext.Current;
		protected const string _SiteExportFolder = "site";

        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
		public Guid SiteId
		{
			get
			{
				object id = _CurrentHttpContext.Request.QueryString["siteid"];
				Guid siteId = Guid.Empty;
				try
				{
					siteId = new Guid(id.ToString());
				}
				catch { }
				return siteId;
			}
		}

        /// <summary>
        /// Gets the name of the site.
        /// </summary>
        /// <value>The name of the site.</value>
		public string SiteName
		{
			get
			{
				object name = _CurrentHttpContext.Request.QueryString["sitename"];
				return (name != null ? name.ToString() : null);
			}
		}

		private ImportExportHelper _ImportExport;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
			BtnExport.Click += new EventHandler(BtnExport_Click);
			ProgressControl1.ProgressCompleted += new Mediachase.Commerce.Manager.Apps.Core.Controls.ProgressCompletedHandler(ProgressControl1_ProgressCompleted);
			FilesControl.Folder = ManagementHelper.GetImportExportFolderPath(_SiteExportFolder);
			base.OnInit(e);
		}

		void ProgressControl1_ProgressCompleted(object data)
		{
			if (data != null)
			{
				int result = 0;
				Int32.TryParse(data.ToString(), out result);
				if (result == 100)
				{
					FilesControl.DataBind();
					FilesPanel.Update();
				}
			}
		}

        /// <summary>
        /// Handles the Click event of the BtnExport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void BtnExport_Click(object sender, EventArgs e)
		{
            Guid appId = AppContext.Current.ApplicationId;

			// start import in a new thread
            ProgressControl1.StartOperation(new System.Threading.ThreadStart(delegate { DoExport(appId); }));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
		void ImportExport_ImportExportProgressMessage(object sender, SiteImportExportEventArgs args)
		{
			ProgressControl1.AddProgressMessageText(args.Message, false, Convert.ToInt32(args.CompletedPercentage));
		}

        /// <summary>
        /// Does the export.
        /// </summary>
		private void DoExport(Guid appId)
		{
            AppContext.Current.ApplicationId = appId;

			try
			{
				string result = String.Empty;

				if (SiteId != Guid.Empty)
				{
					// export site

                    StringBuilder filePath = new StringBuilder(Server.MapPath(ManagementHelper.GetImportExportFolderPath("site")));
                    filePath.AppendFormat("\\SiteExport_{0}_{1}_{2}.xml", SiteName.Replace(' ', '-'), DateTime.Now.ToString("yyyyMMdd_HHmmss"), System.Environment.MachineName);
					FileStream fs = new FileStream(filePath.ToString(), FileMode.Create, FileAccess.ReadWrite);

					try
					{
						PageDocument.Init(new SqlPageDocumentStorageProvider(), new SqlTemporaryStorageProvider());
						_ImportExport = new Mediachase.Cms.ImportExport.ImportExportHelper();
						_ImportExport.ImportExportProgressMessage += new SiteImportExportProgressMessageHandler(ImportExport_ImportExportProgressMessage);
						_ImportExport.ExportSite(SiteId, fs);
					}
					catch (SiteImportExportException ex)
					{
						throw ex;
					}
					finally
					{
						if (fs != null)
							fs.Close();

						// refresh files grid
						//FilesControl.DataBind();
						//FilesPanel.Update();
					}
				}
				else
					throw new Exception("SiteId is empty!");
			}
			catch (Exception ex)
			{
				ProgressControl1.AddProgressMessageText(ex.Message, true, -1);
			}
			finally
			{
			}
		}
	}
}