using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Storage;
using Mediachase.FileUploader.Web;
using Mediachase.FileUploader.Web.UI;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
    public partial class FileControl : CoreBaseUserControl, IMetaControl
	{
		string StateChangedScript = String.Empty;

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			StateChangedScript = "<script language=\"JavaScript\">" + "\r\n" +
			"function StateChanged_" + FileUpMetaValueCtrl.ClientID + "(fuList, fuActionsObj, uploadedFilesRowObj) " + "\r\n" +
			"{ " + "\r\n" +
				"if(!MCFU_Array || !MCFU_Array[fuList]) " + "\r\n" +
				"{ " + "\r\n" +
					"setTimeout('StateChanged_" + FileUpMetaValueCtrl.ClientID + "(\"'+fuList+'\", \"'+fuActionsObj+'\", \"'+uploadedFilesRowObj+'\");', 50); " + "\r\n" +
					"return; " + "\r\n" +
				"} " + "\r\n" +
				"var fa = MCFU_Array[fuActionsObj];" + "\r\n" +
				"var uc = MCFU_Array[fuList]; " + "\r\n" +
				"if(uc!=null) " + "\r\n" +
				"{ " + "\r\n" +
					"var h = $get(uploadedFilesRowObj); " + "\r\n" +
					"if(h!=null) " + "\r\n" +
					"{ " + "\r\n" +
						"if(uc.FilesCount>0) " + "\r\n" +
							"{ " + "\r\n" +
								"var sessionUidField = $get('" + hfFileUpMetaValueCtrlSessionUID.ClientID + "');\r\n" +
								"if (sessionUidField != null) {\r\n" +
									"sessionUidField.value = MCFU_Array[uc.MCFUId].sessionUid;\r\n" +
								"}" +
								"h.style.display = \"block\"; " + "\r\n" +
								"h.cells[0].style.width = h.offsetWidth+\"px\"; " + "\r\n" +
								"if(fa != null) {" + "\r\n" +
									"fa.mainDiv.style.display = \"none\";" + "\r\n" +
								"}" + "\r\n" +
							"} " + "\r\n" +
						"else {" + "\r\n" +
							"h.style.display = \"none\"; " + "\r\n" +
							"if(fa != null) {" + "\r\n" +
							   "fa.mainDiv.style.display = \"block\";" + "\r\n" +
								"}" + "\r\n" +
						"}" + "\r\n" +
					"} " + "\r\n" +
				"} " + "\r\n" +
			"} " + "\r\n" +
			"</script>";

			string scriptKey = "StateChangedScript";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(FileControl), scriptKey))
				Page.ClientScript.RegisterClientScriptBlock(typeof(FileControl), scriptKey, StateChangedScript);

			ApplyLocalization();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			if (!this.IsPostBack)
				BindData();

			base.DataBind();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			RemoveFile.Text = Resources.CommerceManager.FILECONTROL_DELETE_FILE;
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			System.Web.UI.Control c = this.FindControl("trUploadedFiles");
			trUploadedFiles.Attributes.Add("onMCFListRefresh",
				String.Format("javascript:StateChanged_{3}('{0}', '{1}', '{2}');", fvControl.ClientID, fuActions.ClientID, trUploadedFiles.ClientID, FileUpMetaValueCtrl.ClientID));

			FileUrl.Visible = false;
			RemoveFile.Visible = false;
			/*HtmlTable uploadTable = this.FindControl("tblUpload") as HtmlTable;
			if (uploadTable != null)
				uploadTable.Visible = true;*/
			MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);
			MetaDescriptionCtrl.Text = MetaField.Description;
			if (MetaObject != null && MetaObject.GetFile(MetaField) != null)
			{
				MetaFile file = MetaObject.GetFile(MetaField);
				FileUrl.Text = String.Format("{0} ({1})", file.Name, Mediachase.FileUploader.Web.Util.CommonHelper.FormatBytes(Convert.ToInt64(file.Size)));
				RemoveFile.Visible = true;
				FileUrl.Visible = true;
				fuActions.Visible = false;
				/*if (uploadTable != null)
					uploadTable.Visible = false;*/
			}

			if (MetaField != null && !MetaField.AllowNulls)
				RemoveFile.Visible = false;

			//FileUpMetaValueCtrl.SessionUid = Guid.NewGuid();
		}

		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		#region IMetaControl Members
		private MetaField _MetaField;
        /// <summary>
        /// Gets or sets the meta field.
        /// </summary>
        /// <value>The meta field.</value>
		public MetaField MetaField
		{
			get
			{
				return _MetaField;
			}
			set
			{
				_MetaField = value;
			}
		}
		
		MetaObject _MetaObject;
        /// <summary>
        /// Gets or sets the meta object.
        /// </summary>
        /// <value>The meta object.</value>
		public MetaObject MetaObject
		{
			get
			{
				return _MetaObject;
			}
			set
			{
				_MetaObject = value;
			}
		}

		private string _LanguageCode;
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
		public string LanguageCode
		{
			get
			{
				return _LanguageCode;
			}
			set
			{
				_LanguageCode = value;
			}
		}

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
		public string ValidationGroup
		{
			get
			{
				return String.Empty;
			}
			set
			{
			}
		}

        /// <summary>
        /// Updates this instance.
        /// </summary>
		public void Update()
		{
			if (!String.IsNullOrEmpty(hfFileUpMetaValueCtrlSessionUID.Value))
			{
				// restore old SessionUid value
				FileUpMetaValueCtrl.SessionUid = new Guid(hfFileUpMetaValueCtrlSessionUID.Value);
			}

			if (RemoveFile.Checked)
			{
                MetaHelper.SetMetaFile(MetaField.Context, MetaObject, MetaField.Name, String.Empty, String.Empty, null);
				return;
			}
			
			if (FileUpMetaValueCtrl.Files != null && FileUpMetaValueCtrl.Files.Length > 0)
			{
				//string folderPath = ConfigurationManager.AppSettings["FolderPath"].ToString();
				string localPath = Path.GetTempPath(); //Server.MapPath(folderPath);
				DirectoryInfo di = new DirectoryInfo(localPath);
				if (di != null)
				{
					FileStreamInfo[] fsi = FileUpMetaValueCtrl.Files;

					//Create byte Array with file length
					// use [0] index here, since we allow to load only 1 file
					Byte[] data = new Byte[fsi[0].Size];
					Stream stream = TempFileStorage.Provider.GetStream(fsi[0].StreamUid);
					stream.Read(data, 0, Convert.ToInt32(fsi[0].Size));

                    MetaHelper.SetMetaFile(MetaField.Context, MetaObject, MetaField.Name, fsi[0].FileName, fsi[0].ContentTypeName, data);
				}
			}
		}

		#endregion
	}
}
