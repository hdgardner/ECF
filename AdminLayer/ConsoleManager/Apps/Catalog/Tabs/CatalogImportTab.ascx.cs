using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.ImportExport;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Shared;
using Mediachase.FileUploader.Web;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class CatalogImportTab : CatalogBaseUserControl
    {
		private const string _SelectedFilePathString = "Import_SelectedFilePath_D61CCF81-EBF7-4d04-93FE-11C6084036B7";
		protected const string _CatalogImportFolder = "catalog";

		private Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport _ImportExport = new Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport();

		private HttpContext _CurrentHttpContext = HttpContext.Current;

        /// <summary>
        /// Gets or sets the selected file path.
        /// </summary>
        /// <value>The selected file path.</value>
		public string SelectedFilePath
		{
			get
			{
				return Session[_SelectedFilePathString] as string;
			}
			set
			{
				lock (Session.SyncRoot)
					Session[_SelectedFilePathString] = value;
			}
		}

        /// <summary>
        /// Gets the current application id.
        /// </summary>
        /// <returns></returns>
		private Guid GetCurrentApplicationId()
		{
			return CatalogConfiguration.Instance.ApplicationId;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindData();

			string btnId = btnImport.UniqueID;

			if (Page.IsPostBack && !String.IsNullOrEmpty(btnId) && Request.Form[btnId] != null)
			{
				string filePath = FilesControl.GetSelectedFilePath();

				if (String.IsNullOrEmpty(filePath))
				{
					DisplayErrorMessage("No selected file.");
					return;
				}
				else
					SelectedFilePath = filePath;
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			System.Web.UI.Control c = this.FindControl("trUploadedFiles");
			trUploadedFiles.Attributes.Add("onMCFListRefresh",
				String.Format("javascript:StateChanged('{0}', '{1}', '{2}');", fvControl.ClientID, fuActions.ClientID, trUploadedFiles.ClientID));

			FilesControl.Folder = ManagementHelper.GetImportExportFolderPath(_CatalogImportFolder);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			btnImport.Click += new EventHandler(btnImport_Click);
			ProgressControl1.ProgressCompleted += new Mediachase.Commerce.Manager.Apps.Core.Controls.ProgressCompletedHandler(ProgressControl1_ProgressCompleted);
			lbSaveFiles.Click += new EventHandler(lbSaveFiles_Click);
			base.OnInit(e);
		}

        /// <summary>
        /// Progresses the control1_ progress completed.
        /// </summary>
        /// <param name="data">The data.</param>
		void ProgressControl1_ProgressCompleted(object data)
		{
			btnImport.Enabled = true;
			MainPanel.Update();
		}

        /// <summary>
        /// Handles the Click event of the lbSaveFiles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lbSaveFiles_Click(object sender, EventArgs e)
		{
			FileStreamInfo[] fsi = FileUpCtrl.Files;
			if (fsi != null && fsi.Length > 0)
			{
				try
				{
                    string filePath = Path.Combine(HttpContext.Current.Server.MapPath(ManagementHelper.GetImportExportFolderPath("catalog")), fsi[0].FileName);

                    if (File.Exists(filePath))
                        File.Delete(filePath);

					FileUpCtrl.SaveAs(fsi[0], filePath);
					FileUpCtrl.ReleaseAll();

					// update upload status
					lblUploadResult.Text = String.Empty; //"File saved successfully.";
					UploadResultPanel.Update();
				}
				catch (Exception ex)
				{
					lblUploadResult.Text = ex.Message;
					UploadResultPanel.Update();
				}
				finally
				{
					FilesControl.DataBind();
					FilesPanel.Update();
				}
			}
		}

        /// <summary>
        /// Imports the export_ import export progress message.
        /// </summary>
		/// <param name="sender"></param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.ImportExport.ImportExportEventArgs"/> instance containing the event data.</param>
		void ImportExport_ImportExportProgressMessage(object sender, ImportExportEventArgs args)
		{
			ProgressControl1.AddProgressMessageText(args.Message, false, Convert.ToInt32(args.CompletedPercentage));
		}

		/// <summary>
		/// Handles the Click event of the btnImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnImport_Click(object sender, EventArgs e)
		{
            Guid appId = GetCurrentApplicationId();

			// start import in a new thread
            ProgressControl1.StartOperation(new System.Threading.ThreadStart(delegate { DoImport(appId); }));
		}

        /// <summary>
        /// Processes the import button event.
        /// </summary>
		protected void DoImport(Guid appId)
		{
            AppContext.Current.ApplicationId = appId;

			// save current MetaDataContext
			MetaDataContext currentContext = CatalogContext.MetaDataContext;

			try
			{
				string filePath = SelectedFilePath;

				if (String.IsNullOrEmpty(filePath))
					throw new Exception("No selected file.");

				// import site

				ProgressControl1.AddProgressMessageText("Preparing to unpack zip file.", false, 0);

				// get folder name where to extract files from uploaded zip archive
				StringBuilder sbDirName = new StringBuilder(Path.GetTempPath());
				sbDirName.Append(Path.GetFileNameWithoutExtension(filePath));
				string destDir = sbDirName.ToString();

				// delete folder if it already exists
				if (Directory.Exists(destDir))
					Directory.Delete(destDir, true);

				DirectoryInfo dir = Directory.CreateDirectory(destDir);

				// unpack archive
				ZipHelper.ExtractZip(filePath, dir.FullName);

				ProgressControl1.AddProgressMessageText("Unpacked zip file.", false, 0);

				FileStream fs = new FileStream(Path.Combine(dir.FullName, "Catalog.xml"), FileMode.Open, FileAccess.Read);

				try
				{
					_ImportExport.ImportExportProgressMessage += new Mediachase.Commerce.Catalog.ImportExport.ImportExportProgressMessageHandler(ImportExport_ImportExportProgressMessage);

					// perform import operation
					_ImportExport.Import(fs, appId, dir.FullName);

					if (fs != null)
					{
						fs.Close();
						fs = null;
					}

					SelectedFilePath = null;
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