using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.FileUploader.Web;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin.Tabs
{
	public partial class MetaDataImportTab : CoreBaseUserControl
	{
		private const string _SelectedFilePathString = "Import_SelectedFilePath_46C7E8B1-CFDD-4fcc-B71F-E95D4044F3C6";
		protected const string _MetaDataImportFolder = "metadata";

		private HttpContext _CurrentHttpContext = HttpContext.Current;

		/// <summary>
		/// Gets the app id.
		/// </summary>
		/// <value>The app id.</value>
		public string AppId
		{
			get
			{
				string appId = String.Empty;
				if (_CurrentHttpContext != null)
				{
					object id = _CurrentHttpContext.Request.QueryString["_a"];
					if (id != null)
						appId = id.ToString();
				}

				return appId;
			}
		}

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

			FilesControl.Folder = ManagementHelper.GetImportExportFolderPath(_MetaDataImportFolder);
			FilesControl.GridAppId = AppId;
			FilesControl.SelectedBtnId = btnImport.ClientID;
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
					StringBuilder filePath = new StringBuilder(Server.MapPath(ManagementHelper.GetImportExportFolderPath(_MetaDataImportFolder)));
					filePath.Append(fsi[0].FileName);

					if (File.Exists(filePath.ToString()))
						File.Delete(filePath.ToString());

					FileUpCtrl.SaveAs(fsi[0], filePath.ToString());
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
		/// Progresses the control1_ progress completed.
		/// </summary>
		/// <param name="data">The data.</param>
		void ProgressControl1_ProgressCompleted(object data)
		{
			btnImport.Enabled = true;
			MainPanel.Update();
		}

		/// <summary>
		/// Handles the Click event of the btnImport control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnImport_Click(object sender, EventArgs e)
		{
            Guid appId = AppContext.Current.ApplicationId;

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
			MetaDataContext currentContext = null;

			try
			{
				// set md context
				if (String.Compare(AppId, "Catalog", true) == 0)
					currentContext = Mediachase.Commerce.Catalog.CatalogContext.MetaDataContext;
				else if (String.Compare(AppId, "Order", true) == 0)
					currentContext = Orders.OrderContext.MetaDataContext;
				else if (String.Compare(AppId, "Profile", true) == 0)
					currentContext = Mediachase.Commerce.Profile.ProfileContext.MetaDataContext;
				
				if (currentContext == null)
					throw new ArgumentNullException("currentContext");

				string filePath = SelectedFilePath;

				if (String.IsNullOrEmpty(filePath))
					throw new Exception("No selected file.");

				// import
				try
				{
					// perform import operation
					ProgressControl1.AddProgressMessageText("Starting import.", 0);

					MetaInstaller.RestoreFromFile(currentContext, SelectedFilePath);

					SelectedFilePath = null;

					ProgressControl1.AddProgressMessageText("Import completed successfully.", 100);
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
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