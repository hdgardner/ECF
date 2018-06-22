using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using Mediachase.Commerce.Catalog.ImportExport;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Catalog;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	/// <summary>
	///	Used for creating Exporting Catalog.
	/// </summary>
    public partial class CatalogExportTab : CatalogBaseUserControl
	{
		private HttpContext _CurrentHttpContext = HttpContext.Current;
		protected const string _CatalogExportFolder = "catalog";

        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>The app id.</value>
		public string AppId
		{
			get
			{
				object id = _CurrentHttpContext.Request.QueryString["_a"];
				if (id == null)
					return String.Empty;
				else
					return id.ToString();
			}
		}

        /// <summary>
        /// Gets the catalog id.
        /// </summary>
        /// <value>The catalog id.</value>
		public int CatalogId
		{
			get
			{
				object id = _CurrentHttpContext.Request.QueryString["catalogid"];
				int catalogId = 0;
				if (id == null || !Int32.TryParse(id.ToString(), out catalogId))
					return 0;
				else
					return catalogId;
			}
		}

		private string _CatalogName;
        /// <summary>
        /// Gets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
		public string CatalogName
		{
			get
			{
				if (String.IsNullOrEmpty(_CatalogName))
				{
					object name = _CurrentHttpContext.Request.QueryString["catalogname"];
					if (name == null)
						_CatalogName = String.Empty;
					else
						_CatalogName = _CurrentHttpContext.Server.UrlDecode(name.ToString());
				}

				return _CatalogName;

				//if (String.IsNullOrEmpty(_CatalogName))
				//{
				//    Mediachase.Commerce.Catalog.Dto.CatalogDto dto = FrameworkContext.Current.CatalogSystem.GetCatalogDto(CatalogId);
				//    if (dto.Catalog.Count > 0)
				//        _CatalogName = dto.Catalog[0].Name;
				//}
				//return _CatalogName;
			}
		}

		private Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport _ImportExport = new Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport();

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
		protected override void OnInit(EventArgs e)
		{
			btnExport.Click += new EventHandler(BtnExport_Click);
			ProgressControl1.ProgressCompleted += new Mediachase.Commerce.Manager.Apps.Core.Controls.ProgressCompletedHandler(ProgressControl1_ProgressCompleted);
			FilesControl.Folder = ManagementHelper.GetImportExportFolderPath(_CatalogExportFolder);
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
		void ImportExport_ImportExportProgressMessage(object sender, ImportExportEventArgs args)
		{
			ProgressControl1.AddProgressMessageText(args.Message, false, Convert.ToInt32(args.CompletedPercentage));
		}

        /// <summary>
        /// Processes the export button event.
        /// </summary>
		protected void DoExport(Guid appId)
		{
            AppContext.Current.ApplicationId = appId;

			try
			{

				if (!String.IsNullOrEmpty(CatalogName))
				{
					// export site

					StringBuilder sbDirName = new StringBuilder(Path.GetTempPath());
					sbDirName.AppendFormat("CatalogExport_{0}_{1}_{2}", AppId, CatalogName, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
					string dirName = sbDirName.ToString();
					if (Directory.Exists(dirName))
						Directory.Delete(dirName, true);
					DirectoryInfo dir = Directory.CreateDirectory(dirName);

                    string zipPath = Path.Combine(_CurrentHttpContext.Server.MapPath(ManagementHelper.GetImportExportFolderPath("catalog")), dir.Name);

					StringBuilder filePath = new StringBuilder(dir.FullName);
					filePath.AppendFormat("\\Catalog.xml");
					FileStream fs = new FileStream(filePath.ToString(), FileMode.Create, FileAccess.ReadWrite);

					try
					{
						_ImportExport.ImportExportProgressMessage += new Mediachase.Commerce.Catalog.ImportExport.ImportExportProgressMessageHandler(ImportExport_ImportExportProgressMessage);

						int startTime = Environment.TickCount;

                        _ImportExport.Export(CatalogName, fs, Path.Combine(_CurrentHttpContext.Server.MapPath(ManagementHelper.GetImportExportFolderPath("catalog")), dirName));//dir.FullName);

						if (fs != null)
						{
							fs.Close();
							fs = null;
						}

						ProgressControl1.AddProgressMessageText("Creating zip file.", 99);

						ZipHelper.CreateZip(dir.FullName, String.Concat(zipPath, ".zip"));

						ProgressControl1.AddProgressMessageText("Finished.", 100);
					}
					catch (Mediachase.Commerce.Catalog.ImportExport.ImportExportException ex)
					{
					    throw ex;
					}
					catch (Exception ex1)
					{
					    throw ex1;
					}
					finally
					{
						if (fs != null)
							fs.Close();

						// delete temporary folder
						if (Directory.Exists(dir.FullName))
							Directory.Delete(dir.FullName, true);
					}
				} // if (!String.IsNullOrEmpty(CatalogName))
				else
				    throw new Exception("CatalogName is empty!");
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