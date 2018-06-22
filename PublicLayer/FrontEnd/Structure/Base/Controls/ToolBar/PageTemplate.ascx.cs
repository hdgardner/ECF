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
using System.Threading;
using Mediachase.Commerce.Shared;
using ComponentArt.Web.UI;

namespace Mediachase.Cms.Controls.Toolbar
{
    public partial class PageTemplate : System.Web.UI.UserControl
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
                SaveButton.OnClientClick = String.Format("RunCommand('ChangeTemplate', {0}.value);", TemplateListControl.ClientID);
                BindDropDownList();
            }

            if (Request.QueryString["TemplateId"] != null)
            {
                // Check permissions, only cms manager can change template
                if (!Page.User.Identity.IsAuthenticated || !SecurityManager.CheckPermission(new string[] { CmsRoles.ManagerRole }))
                {
                    this.Visible = false;
                    return;
                }

                TemplateListControl.SelectedValue = Request.QueryString["TemplateId"];
            }

            using (IDataReader reader = PageVersion.GetVersionById(VersionId))
            {
                if (reader.Read())
                {
                    TemplateListControl.SelectedValue = ((int)reader["TemplateId"]).ToString();
                    TemplateListControl.Visible = true;
                    //statusId = (int)reader["statusId"];
                }
            }
        }

        /// <summary>
        /// Called when [save button].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void OnSaveButton(object sender, ImageClickEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Binds the drop down list.
        /// </summary>
        private void BindDropDownList()
        {
            DataView view = DictionaryManager.GetTemplateDto().main_Templates.DefaultView;
            view.RowFilter = String.Format("TemplateType = '{0}' and LanguageCode = '{1}'", "page", Thread.CurrentThread.CurrentCulture.Name);
            if (view.Count == 0)
                view.RowFilter = String.Format("TemplateType = '{0}'", "page");

            /*
            string defaultTemplate = GlobalVariable.GetVariable("default_template", CMSContext.Current.SiteId);
            foreach (DataRowView row in view)
            {
                ToolBarItem item = new ToolBarItem();
                item.Text = row["FriendlyName"].ToString();
                item.ID = row["TemplateId"].ToString();
                TemplateItemList.Items.Add(item);
            }

            TemplateFilter.SelectedValue = defaultTemplate;
             * */

               

            TemplateListControl.DataSource = view;
            TemplateListControl.DataTextField = "FriendlyName";
            TemplateListControl.DataValueField = "TemplateId";
            TemplateListControl.DataBind();
            TemplateListControl.SelectedValue = GlobalVariable.GetVariable("default_template", CMSContext.Current.SiteId);
        }
    }
}