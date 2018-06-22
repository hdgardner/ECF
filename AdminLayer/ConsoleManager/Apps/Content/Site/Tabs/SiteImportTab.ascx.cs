using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.ImportExport;
using Mediachase.FileUploader.Web;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Content.Site.Tabs
{
    public partial class SiteImportTab : BaseUserControl
    {
		private const string _SelectedFilePathString = "SiteImport_SelectedFilePath_D76F6C95-6987-4137-989B-21FBCC4AAF8D";
		protected const string _SiteImportFolder = "site";

        private static readonly string _CommandName = "CommandName";
        private static readonly string _CommandArguments = "CommandArguments";
        private static readonly string _DoImportCommand = "cmdDoImport";

		private Mediachase.Cms.ImportExport.ImportExportHelper _ImportExport;

        /// <summary>
        /// Gets or sets the uploaded file path.
        /// </summary>
        /// <value>The uploaded file path.</value>
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
			return CmsConfiguration.Instance.ApplicationId;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
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

            CommandManager cm = CommandManager.GetCurrent(this.Page);
            cm.AddCommand("", "Content", "SiteImport", "cmdShowDialogSiteImportConfirm");
		}

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                object objArgs = Request.Form["__EVENTARGUMENT"];
                if (objArgs != null)
                {
                    Dictionary<string, object> cmd = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(objArgs.ToString()) as Dictionary<string, object>;
                    if (cmd != null && cmd.Count > 1)
                    {
                        object cmdName = cmd[_CommandName];
                        if (String.Compare((string)cmdName, _DoImportCommand, true) == 0)
                        {
                            // process move command
                            Dictionary<string, object> args = cmd[_CommandArguments] as Dictionary<string, object>;
                            if (args != null)
                            {
                                // start import in a new thread
                                string siteId = (string)args["SiteId"];
                                Guid appId = GetCurrentApplicationId();
                                ProgressControl1.StartOperation(new System.Threading.ThreadStart(delegate { DoImport(new Guid(siteId), appId); }));
                            }
                        }
                    }
                }
            }
        }

		/// <summary>
		/// 
		/// </summary>
		private void BindData()
		{
			System.Web.UI.Control c = this.FindControl("trUploadedFiles");
			trUploadedFiles.Attributes.Add("onMCFListRefresh",
				String.Format("javascript:StateChanged('{0}', '{1}', '{2}');", fvControl.ClientID, fuActions.ClientID, trUploadedFiles.ClientID));

			FilesControl.Folder = ManagementHelper.GetImportExportFolderPath(_SiteImportFolder);
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			btnImport.Click += new EventHandler(btnImport_Click);
			ProgressControl1.ProgressCompleted+=new Mediachase.Commerce.Manager.Apps.Core.Controls.ProgressCompletedHandler(ProgressControl1_ProgressCompleted);
			lbSaveFiles.Click += new EventHandler(lbSaveFiles_Click);
            Page.LoadComplete += new EventHandler(Page_LoadComplete);
            base.OnInit(e);
        }

		void lbSaveFiles_Click(object sender, EventArgs e)
		{
			FileStreamInfo[] fsi = FileUpCtrl.Files;
			if (fsi != null && fsi.Length > 0)
			{
				try
				{
                    StringBuilder filePath = new StringBuilder(Server.MapPath(ManagementHelper.GetImportExportFolderPath("site")));
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

		void ProgressControl1_ProgressCompleted(object data)
		{
			btnImport.Enabled = true;
			MainPanel.Update();
		}

        /// <summary>
        /// Imports the export_ import export progress message.
        /// </summary>
        /// <param name="message">The message.</param>
		void ImportExport_ImportExportProgressMessage(object sender, SiteImportExportEventArgs args)
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
			_ImportExport = new Mediachase.Cms.ImportExport.ImportExportHelper(); //ImportExportHelper.Current;

            Guid appId = GetCurrentApplicationId();

            string filePath = SelectedFilePath;

			if (String.IsNullOrEmpty(filePath))
				throw new Exception("No selected file.");

			// import site
			FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            Guid siteId = _ImportExport.GetSiteID(fs, appId);

            SiteDto sites = CMSContext.Current.GetSitesDto(appId);

            if (sites.Site.Count > 0)
            {
                CommandParameters cp = new CommandParameters("cmdShowDialogSiteImportConfirm");
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if(!siteId.Equals(Guid.Empty))
                {
                   dic["SiteId"] = siteId.ToString();
                }
                cp.CommandArguments = dic;

                string cmd = CommandManager.GetCommandString("cmdShowDialogSiteImportConfirm", dic);
                cmd = cmd.Replace("\"", "&quot;");
                Mediachase.Ibn.Web.UI.WebControls.ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"), cmd, true);

                return;
            }

            // start import in a new thread
            ProgressControl1.StartOperation(new System.Threading.ThreadStart(delegate { DoImport(new Guid(), appId); }));
		}

		private void DoImport(Guid siteId, Guid appId)
		{
			try
			{
                AppContext.Current.ApplicationId = appId;
				_ImportExport = new Mediachase.Cms.ImportExport.ImportExportHelper(); //ImportExportHelper.Current;

				string filePath = SelectedFilePath;

				if (String.IsNullOrEmpty(filePath))
					throw new Exception("No selected file.");

				// import site
				FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

				try
				{
					_ImportExport.ImportExportProgressMessage += new SiteImportExportProgressMessageHandler(ImportExport_ImportExportProgressMessage);
                    _ImportExport.ImportSite(fs, appId, siteId, false);

					SelectedFilePath = null;
				}
				catch (SiteImportExportException ex)
				{
					throw ex;
				}
				finally
				{
					if (fs != null)
						fs.Close();
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