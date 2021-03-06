using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.Managers;
using Mediachase.Cms;
using Mediachase.Cms.Util;

namespace Mediachase.Cms.Controls.Toolbar
{
    public partial class EditorTemplate : System.Web.UI.UserControl
    {
        #region property: VersionId
        /// <summary>
        /// Gets the version id.
        /// </summary>
        /// <value>The version id.</value>
        public int VersionId
        {
            get { return CMSContext.Current.VersionId; }
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            if (!IsPostBack)
            {
                BindInfo();
            }
        }

        /// <summary>
        /// Binds the info.
        /// </summary>
        private void BindInfo()
        {
            #region InfoAboutVersion
            bool canEdit = false;
            string _comment = string.Empty;
            string _commentValue = string.Empty;
            using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
            {
                if (reader.Read())
                {
                    using (IDataReader readerState = Workflow.PageStateGetById((int)reader["StateId"]))
                    {
                        if (readerState.Read())
                        {
                            //if (canEdit) _comment = _comment + "<b>Состояние:</b> " + (string)readerState["FriendlyName"];
                            if (canEdit)
                            {
                                if ((int)reader["StateId"] == 1) _comment = _comment + "<b> <span align='right' style='color:blue'>" + (string)readerState["FriendlyName"] + "</span> </b>";
                                else _comment = _comment + "<b> <span align='right' style='color:red'>" + (string)readerState["FriendlyName"] + "</span> </b>";
                            }
                        }

                        readerState.Close();
                    }

                    /*
                    using (IDataReader readerStatus = WorkflowStatus.LoadById((int)reader["StatusId"]))
                    {
                        //if ((int)reader["StatusId"] == -1)
                        //{
                        //    secHeader.AddText("&nbsp Документ - Черновик &nbsp");
                        //}
                        if (readerStatus.Read())
                        {
                            //_comment = _comment + "<br> <b>Статус:</b> " + (string)readerStatus["FriendlyName"];
                            //secHeader.AddText("&nbsp Документ - " + (string)readerStatus["FriendlyName"] + "&nbsp");
                        }
                    }
                     * */
                    if (!(reader["EditorUID"] is DBNull))
                    {
                        _comment = _comment + "<b>Edited by </b> <span style='color: blue'>" + CommonHelper.GetUserName((Guid)reader["EditorUID"]) + "</span> on " + ((DateTime)reader["Edited"]).ToString("f");
                    }

                    if (!(reader["Comment"] is System.DBNull) && canEdit)
                    {
                        if (((string)reader["Comment"]).Length > 0)
                        {
                            _comment = _comment + "<b>Comments:</b><br>";
                            _commentValue = _commentValue + (string)reader["Comment"];// +"<br>";
                        }
                    }
                }

                reader.Close();
            }
            lblComment.Text = _comment;
            lblCommentValue.Text = _commentValue;
            #endregion

            //ddText.Visible = canEdit;
            if (canEdit)
                ddText.Attributes.Add("display", "inline");
            else
                ddText.Attributes.Add("display", "none");

        }
    }
}