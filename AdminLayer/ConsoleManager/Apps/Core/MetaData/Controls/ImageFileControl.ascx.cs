using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Storage;
using Mediachase.FileUploader.Web;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
    public partial class ImageFileControl : CoreBaseUserControl, IMetaControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			BindData();
			base.DataBind ();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			//RemovePicture.Text = RM.GetString("IMAGEFILECONTROL_DELETE_PICTURE");
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			CurrentPicture.Visible = false;
			RemovePicture.Visible = false;
			MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);
            string description = MetaField.Description;

            /*
            if (MetaField.ImageProperty.AutoResizeImage)
                description += String.Format("<br>Image will be auto resized to {0}x{1} (width x height)", MetaField.ImageProperty.ImageWidth, MetaField.ImageProperty.ImageHeight);

            if (MetaField.ImageProperty.AutoCreateThumbnailImage)
                description += String.Format("<br>Thumbnail Image will be auto created with size of {0}x{1} (width x height)", MetaField.ImageProperty.ThumbWidth, MetaField.ImageProperty.ThumbHeight);
             * */

            MetaDescriptionCtrl.Text = description;

			//MetaSingleValueField val = MetaField.SingleValueField;
			MetaFile file = MetaObject != null ? MetaObject.GetFile(MetaField) : null;
			if (file != null)
			{
				if (file.Buffer != null && file.Buffer.Length > 0)
				{
					CaptionText.Text = file.Name;
					CurrentPicture.Visible = true;
					RemovePicture.Visible = true;
					CurrentPicture.Src = String.Format("~/Apps/Core/MetaData/Controls/ShowImage.aspx?classid={0}&amp;objectid={1}&amp;name={2}", MetaField.OwnerMetaClass.Id, MetaObject.Id, MetaField.Name);
				}
			}

            // Dont allow removing picture if null values are not allowed
            if (MetaField != null && !MetaField.AllowNulls)
                RemovePicture.Visible = false;

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

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
        /// Updates this instance.
        /// </summary>
		public void Update()
		{
            if (RemovePicture.Checked)
            {
                MetaHelper.SetMetaFile(MetaField.Context, MetaObject, MetaField.Name, String.Empty, String.Empty, null);
                return;
            }

			byte[] data = null;
            if (MetaValueCtrl.PostedFile != null && MetaValueCtrl.PostedFile.ContentLength > 0)
            {
                //To create a PostedFile
                McHttpPostedFile File = MetaValueCtrl.PostedFile;

                if (File.ContentLength == 0)
                    return;

                //if (!MetaField.ImageProperty.AutoResizeImage || File.ContentType.Contains("gif"))
                {
                    //Create byte Array with file len
                    data = new Byte[File.ContentLength];

                    //force the control to load data in array
                    File.InputStream.Read(data, 0, File.ContentLength);
                }
                //else
                {
                    //data = ImageGenerator.CreateImageThumbnail(File.InputStream, File.ContentType, MetaField.ImageProperty.ImageHeight, MetaField.ImageProperty.ImageWidth, MetaField.ImageProperty.StretchImage);
                }

                MetaHelper.SetMetaFile(MetaField.Context, MetaObject, MetaField.Name, File.FileName, File.ContentType, data);
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

        #endregion
    }
}
