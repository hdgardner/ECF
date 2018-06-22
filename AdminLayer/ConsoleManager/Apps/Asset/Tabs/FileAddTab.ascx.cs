using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.FileUploader;
using Mediachase.FileUploader.Web;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Asset.Tabs
{
    public partial class FileAddTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
        #region prop: ParentId
        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>The parent id.</value>
        private int ParentId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("id");
            }
        }
        #endregion

        #region ObjectId
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int ObjectId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("objectid");
            }
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (ObjectId > 0)
                {
                    //check permissions first
                    SecurityManager.CheckRolePermission("asset:mng:edit");

                    BindData();
                    panelUploadFiles.Visible = false;
                }
                else
                {
                    //check permissions first
                    SecurityManager.CheckRolePermission("asset:mng:create");
                }
            }

            panelMetaFields.Visible = !panelUploadFiles.Visible;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            BusinessObject _bindObject = MetaObjectActivator.CreateInstance<BusinessObject>("FolderElement", ObjectId);
            if (_bindObject != null)
            {
                ucView.DataItem = _bindObject;
                ucView.DataBind();
            }
        }

        /// <summary>
        /// Handles the ServerClick event of the btnUpload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnUpload_ServerClick(object sender, EventArgs e)
        {
            FileStreamInfo[] fsi = FileUpCtrl1.Files;
            if (fsi != null && fsi.Length > 0)
            {
                FileUploaderContext.Current.SessionUid = new Guid(Request.Form["__MEDIACHASE_FORM_UNIQUEID2"]);
                Page.ClientScript.RegisterStartupScript(this.GetType(), FileUploaderContext.Current.SessionUid.ToString(), String.Format("ChangeProgressId('{0}');", FileUploaderContext.Current.SessionUid), true);
                
                if (UploadProgress.Enabled)
                {
                    UploadProgressInfo newUploadProgressInfo = new UploadProgressInfo(FileUploaderContext.Current.SessionUid, (Int32)fsi[0].Size);
                    UploadProgress.Provider.Update(newUploadProgressInfo);
                }
                FolderElement element = FolderElement.Create(this.ParentId, fsi[0].FileName, FileUpCtrl1.Provider.GetStream(fsi[0].StreamUid), FileUploaderContext.Current.SessionUid);
                
                Response.Redirect(this.Request.RawUrl + String.Format("&objectid={0}", element.PrimaryKeyId.Value));
            }
            //McHttpPostedFile file = mcHtmlInputFile.PostedFile;
            //if(file != null)
            //{
            //    FolderElement element = FolderElement.Create(this.ParentId, file.FileName, file.InputStream);

            //    Response.Redirect(this.Request.RawUrl + String.Format("&objectid={0}", element.PrimaryKeyId.Value));
            //}
        }

        public string GetClientId(string serverId)
        {
            return ManagementHelper.GetControlFromCollection<Label>(fuProgress.Controls, serverId).ClientID;
        }

        #region ProcessCollection
        /// <summary>
        /// Processes the collection.
        /// </summary>
        /// <param name="_coll">The _coll.</param>
        /// <param name="_obj">The _obj.</param>
        private void ProcessCollection(ControlCollection coll, BusinessObject obj)
        {
            foreach (Control c in coll)
            {
                ProcessControl(c, obj);
                if (c.Controls.Count > 0)
                    ProcessCollection(c.Controls, obj);
            }
        }

        /// <summary>
        /// Processes the control.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="_obj">The _obj.</param>
        private void ProcessControl(Control c, BusinessObject obj)
        {
            IEditControl editControl = c as IEditControl;
            if (editControl != null)
            {
                string fieldName = editControl.FieldName;
                object eValue = editControl.Value;

                bool makeChange = true;
                if ((!obj.Properties[fieldName].GetMetaType().IsNullable && eValue == null) ||
                 obj.Properties[fieldName].IsReadOnly)
                    makeChange = false;

                if (makeChange)
                    obj.Properties[fieldName].Value = eValue;
            }
        }

        #endregion

        //protected void tProgress_Tick(object sender, EventArgs e)
        //{
        //    // Get ref to the progress and try to display bytes so far.
        //    Guid _progressUid =
        //        new Guid(Request.Form["__MEDIACHASE_FORM_UNIQUEID"]);
        //    UploadProgressInfo _upi =
        //        UploadProgress.Provider.GetInfo(_progressUid);

        //    if (_upi != null)
        //    {
        //        this.lProgress.Text = _upi.BytesReceived.ToString();

        //        // Always update the time display every interval.
        //        this.lTime.Text = DateTime.Now.ToLongTimeString();

        //    }
        //    //else
        //    //{
        //    //    // Just display the form param to make sure it's even there.
        //    //    this.lProgress.Text = _progressUid.ToString();
        //    //}
        //}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            if (ObjectId > 0)
            {
                BusinessObject _bindObject = MetaObjectActivator.CreateInstance<BusinessObject>("FolderElement", ObjectId);
                if (_bindObject != null)
                {
                    ProcessCollection(this.Page.Controls, (BusinessObject)_bindObject);
                    ((BusinessObject)_bindObject).Save();
                }
            }
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
        }

        #endregion
    }
}