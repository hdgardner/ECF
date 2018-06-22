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

namespace Mediachase.Cms.Controls
{
    public partial class ToolbarPublicView : System.Web.UI.UserControl
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

            #region Visible Approve/Deny
            using (IDataReader reader = PageVersion.GetVersionById(CMSContext.Current.VersionId))
            {
                if (reader.Read())
                {
                    TopToolbar.Items[0].Visible = (WorkflowStatus.GetNext((int)reader["StatusId"]) != -1) && (CMSContext.Current.PageStatusAccess.ContainsKey(WorkflowStatus.GetNext((int)reader["StatusId"])));

                    if ((int)reader["StatusId"] == WorkflowStatus.DraftId) TopToolbar.Items[0].Visible = true;

                    TopToolbar.Items[1].Visible = (WorkflowStatus.GetPrevious((int)reader["StatusId"]) != -1) && (CMSContext.Current.PageStatusAccess.ContainsKey(WorkflowStatus.GetNext((int)reader["StatusId"])));
                    if (WorkflowStatus.GetNext((int)reader["StatusId"]) == -1 && CMSContext.Current.PageStatusAccess.ContainsKey((int)reader["StatusId"]))
                        TopToolbar.Items[1].Visible = true;
                }
                else
                {
                    TopToolbar.Items[0].Visible = false;
                    TopToolbar.Items[1].Visible = false;
                }

                reader.Close();
            }

            if (!TopToolbar.Items[0].Visible && !TopToolbar.Items[1].Visible)
                TopToolbar.Items[2].Visible = false;

            #endregion
        }
    }
}