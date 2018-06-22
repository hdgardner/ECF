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
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.MetaDataPlus.Import;
using Mediachase.Commerce.Catalog.CSVImport;
using System.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Core;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class CatalogCSVImportTab : CatalogBaseUserControl
    {
		private const string _SelectedFilePathString = "Import_SelectedFilePath_D61CCF81-EBF7-4d04-93FE-11C6084036B7";
        private const string _SelectedMappingFilePathString = "Import_SelectedMappingFilePath_D61CCF81-EBF7-4d04-93FE-11C6084036B7";

		private Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport _ImportExport = new Mediachase.Commerce.Catalog.ImportExport.CatalogImportExport();

		private HttpContext _CurrentHttpContext = HttpContext.Current;
        private string _sourcePath = "";
        private string _rulesPath = "";

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

        public string SelectedMappingFilePath
        {
            get
            {
                return Session[_SelectedMappingFilePathString] as string;
            }
            set
            {
                lock (Session.SyncRoot)
                    Session[_SelectedMappingFilePathString] = value;
            }
        }

        public string SourcePath
        {
            get
            {
                if (_sourcePath == String.Empty)
                    _sourcePath = _CurrentHttpContext.Server.MapPath(ManagementHelper.GetImportExportFolderPath("csv/data"));

                return _sourcePath;
            }
        }

        public string RulesPath
        {
            get
            {
                if (_rulesPath == String.Empty)
                    _rulesPath = _CurrentHttpContext.Server.MapPath(ManagementHelper.GetImportExportFolderPath("csv/rule"));

                return _rulesPath;
            }
        }

        private Encoding GetEncoding(string sEncoding)
        {
            if (sEncoding == String.Empty || sEncoding == "Default" || sEncoding == null)
                return Encoding.Default;
            else
                return Encoding.GetEncoding(sEncoding);
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
		}

		/// <summary>
		/// 
		/// </summary>
		private void BindData()
		{
			System.Web.UI.Control c = this.FindControl("trUploadedFiles");
			trUploadedFiles.Attributes.Add("onMCFListRefresh",
				String.Format("javascript:StateChanged('{0}', '{1}', '{2}');", fvControl.ClientID, fuActions.ClientID, trUploadedFiles.ClientID));

            BindCatalogsList();
		}

        /// <summary>
        /// Binds the catalogs list.
        /// </summary>
        private void BindCatalogsList()
        {
            CatalogDto dto = CatalogContext.Current.GetCatalogDto();

            ListCatalogs.DataSource = dto.Catalog.Rows;
            ListCatalogs.DataBind();
        }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
            FilesControl.Folder = ManagementHelper.GetImportExportFolderPath("csv/data");
            MappingFilesControl.Folder = ManagementHelper.GetImportExportFolderPath("csv/rule");

			btnImport.Click += new EventHandler(btnImport_Click);
			ProgressControl1.ProgressCompleted += new Mediachase.Commerce.Manager.Apps.Core.Controls.ProgressCompletedHandler(ProgressControl1_ProgressCompleted);
			lbSaveFiles.Click += new EventHandler(lbSaveFiles_Click);
			base.OnInit(e);
		}

		void ProgressControl1_ProgressCompleted(object data)
		{
			btnImport.Enabled = true;
			MainPanel.Update();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void lbSaveFiles_Click(object sender, EventArgs e)
		{
			FileStreamInfo[] fsi = FileUpCtrl.Files;
			if (fsi != null && fsi.Length > 0)
			{
				try
				{
                    string filePath = Path.Combine(HttpContext.Current.Server.MapPath(ManagementHelper.GetImportExportFolderPath("csv\\data")), fsi[0].FileName);

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
                    DropDownList ddlDataFiles = ManagementHelper.GetControlFromCollection<DropDownList>(this.Parent.Parent.Controls, "ddlDataFiles");
                    if (ddlDataFiles != null)
                    {
                        BindDataFiles(this.SourcePath, String.Empty, ddlDataFiles);
                    }
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
            string filePath = FilesControl.GetSelectedFilePath();
            string mappingFilePath = MappingFilesControl.GetSelectedFilePath();

            if (String.IsNullOrEmpty(filePath))
            {
                DisplayErrorMessage("No data file selected.");
                return;
            }
            else
                SelectedFilePath = filePath;

            if (String.IsNullOrEmpty(mappingFilePath))
            {
                DisplayErrorMessage("No mapping file selected.");
                return;
            }
            else
                SelectedMappingFilePath = mappingFilePath;

            if(!String.IsNullOrEmpty(ListCatalogs.SelectedValue))
            {
                Guid appId = GetCurrentApplicationId();

                // start import in a new thread
                ProgressControl1.StartOperation(new System.Threading.ThreadStart(delegate { DoImport(appId); }));
            }
		}

        /// <summary>
        /// Processes the import button event.
        /// </summary>
		protected void DoImport(Guid appId)
		{
            AppContext.Current.ApplicationId = appId;

			try
			{
				string filePath = SelectedFilePath;
                string mappingFilePath = SelectedMappingFilePath;

				if (String.IsNullOrEmpty(filePath))
					throw new Exception("No selected file.");

                if (!String.IsNullOrEmpty(mappingFilePath))
                {
                    MetaDataPlus.Import.Rule mapping = MetaDataPlus.Import.Rule.XmlDeserialize(CatalogContext.MetaDataContext, mappingFilePath);
                    char chTextQualifier = '\0';
                    if (mapping.Attribute["TextQualifier"].ToString() != "")
                        chTextQualifier = char.Parse(mapping.Attribute["TextQualifier"]);

                    string sEncoding = "Default";
                    try
                    {
                        sEncoding = mapping.Attribute["Encoding"];
                    }
                    catch { }
                    IIncomingDataParser parser = null;
                    DataSet rawData = null;
                    try
                    {
                        parser = new CsvIncomingDataParser(SourcePath, true, char.Parse(mapping.Attribute["Delimiter"].ToString()), chTextQualifier, true, GetEncoding(sEncoding));
                        rawData = parser.Parse(Path.GetFileName(filePath), null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    DataTable dtSource = rawData.Tables[0];

                    MappingMetaClass mc = null;
                    try
                    {
                        int CatalogId = 0;
                        FillResult fr = null;
                        switch (mapping.Attribute["TypeName"])
                        {
                            case "Category":
                                if (!String.IsNullOrEmpty(ListCatalogs.SelectedValue))
                                    CatalogId = Int32.Parse(ListCatalogs.SelectedValue);

                                CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                CatalogContext.MetaDataContext.Language = mapping.Attribute["Language"];

								mc = new CategoryMappingMetaClass(CatalogContext.MetaDataContext, mapping.ClassName, CatalogId);

                                fr = ((CategoryMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);

                                CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                                break;
                            case "Entry":
                                if (!String.IsNullOrEmpty(ListCatalogs.SelectedValue))
                                    CatalogId = Int32.Parse(ListCatalogs.SelectedValue);

                                CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                CatalogContext.MetaDataContext.Language = mapping.Attribute["Language"];

								mc = new EntryMappingMetaClass(CatalogContext.MetaDataContext, mapping.ClassName, CatalogId);

                                fr = ((EntryMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);

                                CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                                break;
                            case "EntryRelation":
                                mc = new EntryRelationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
								fr = ((EntryRelationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                                break;
                            case "EntryAssociation":
                                mc = new EntryAssociationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
                                fr = ((EntryAssociationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                                break;
                            case "Variation":
                                mc = new VariationMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
								fr = ((VariationMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                                break;
                            case "SalePrice":
                                mc = new PricingMappingMetaClass(CatalogContext.MetaDataContext, CatalogId);
								fr = ((PricingMappingMetaClass)mc).FillData(FillDataMode.All, dtSource, mapping, -1, DateTime.UtcNow);
                                break;
                        }

                        if (fr != null)
                        {
                            if (fr.ErrorRows > 0)
                            {
                                foreach (Exception expt in fr.Exceptions)
                                {
									string exMessage = expt.Message;
									if (expt is MDPImportException)
									{
										MDPImportException mdpEx = (MDPImportException)expt;
										if(mdpEx.RowIndex > -1)
											exMessage = String.Format("Import error. Line {0}: {1}", mdpEx.RowIndex + 1, exMessage);
									}

									ProgressControl1.AddProgressMessageText(exMessage, true, 0);
                                }
                            }

                            if (fr.Warnings.Length > 0)
                            {
                                foreach (MDPImportWarning MDPWarn in fr.Warnings)
                                {
                                    ProgressControl1.AddProgressMessageText(String.Format("Line {0}: {1}", MDPWarn.RowIndex + 1, MDPWarn.Message), false, 0);
                                }
                            }

                            string msgSuccessful = RM.GetString("IMPORT_MSG_SUCCESSFUL") + ". " + string.Format(RM.GetString("IMPORT_MSG_RESULT"), fr.SuccessfulRows.ToString(), fr.TotalRows.ToString());
                            ProgressControl1.AddProgressMessageText(msgSuccessful, false, 100);
                        }
                    }
                    catch (Exception ex) 
                    {
                        throw ex;
                    }
                }
			}
			catch (Exception ex)
			{
				ProgressControl1.AddProgressMessageText(ex.Message, true, 100);
			}
			finally
			{
			}
		}

        #region BindDataFiles
        private void BindDataFiles(string _path, string _chosenFile, object _ddl)
        {
            DropDownList ddl = (DropDownList)_ddl;
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            else
            {
                ddl.DataSource = Directory.GetFiles(_path);
                ddl.DataBind();
                for (int i = 0; i < ddl.Items.Count; i++)
                {
                    string[] val = ddl.Items[i].Value.Split('\\');
                    ddl.Items[i].Text = val[val.Length - 1];
                }
            }
            ddl.Items.Insert(0, new ListItem("", ""));

            if (_chosenFile != "")
            {
                for (int i = 0; i < ddl.Items.Count; i++)
                    if (ddl.Items[i].Value.Equals(_chosenFile))
                    {
                        ddl.SelectedIndex = i;
                        break;
                    }
            }
        }
        #endregion
    }
}