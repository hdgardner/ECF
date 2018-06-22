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
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.ImportExport;
using Mediachase.FileUploader.Web;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class TaxImportTab : OrderBaseUserControl
    {
		private const string _SelectedFilePathString = "Import_SelectedFilePath_9472E46F-9FF4-47fa-9470-EF73EF86E8F5";

		private TaxImportExport _ImportExport = new TaxImportExport();

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
			return OrderConfiguration.Instance.ApplicationId;
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

            FilesControl.Folder = ManagementHelper.GetImportExportFolderPath("tax");
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
                    StringBuilder filePath = new StringBuilder(Server.MapPath(ManagementHelper.GetImportExportFolderPath("tax")));
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
		/// Imports the export_ import export progress message.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args">The <see cref="Mediachase.Commerce.Orders.ImportExport.TaxImportExportEventArgs"/> instance containing the event data.</param>
		void ImportExport_ImportExportProgressMessage(object sender, TaxImportExportEventArgs args)
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
			// start import in a new thread
			ProgressControl1.StartOperation(new System.Threading.ThreadStart(DoImport));
		}

		/// <summary>
		/// Processes the import button event.
		/// </summary>
		protected void DoImport()
		{
			// save current MetaDataContext
			MetaDataContext currentContext = OrderContext.MetaDataContext;

			try
			{
				string filePath = SelectedFilePath;

				if (String.IsNullOrEmpty(filePath))
					throw new Exception("No selected file.");

				// import taxes
				try
				{
					_ImportExport.ImportExportProgressMessage += new Mediachase.Commerce.Orders.ImportExport.TaxImportExportProgressMessageHandler(ImportExport_ImportExportProgressMessage);

					// perform import operation
					_ImportExport.Import(SelectedFilePath, GetCurrentApplicationId(), null, ',');

					SelectedFilePath = null;
				}
				catch (Mediachase.Commerce.Orders.ImportExport.OrdersImportExportException ex)
				{
					throw ex;
				}
				catch (Exception ex1)
				{
					throw ex1;
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