namespace Mediachase.Cms.Controls.Editors.CuteEditor
{
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

    using Mediachase.Cms;
    using Mediachase.Cms.Pages;
    using Mediachase.Cms.Util;

    public partial class PopUpEdit : System.Web.UI.Page
    {
        #region Page_Load
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string stylesheetsList = String.Empty;

            foreach(Control ctrl in Page.Header.Controls)
            {
                if(ctrl.GetType() == typeof(HtmlLink))
                {
                    if(!String.IsNullOrEmpty(stylesheetsList))
                        stylesheetsList += ",";

                    stylesheetsList += Page.ResolveUrl(((HtmlLink)ctrl).Href);
                }
            }

            // Bind Styles
            HtmlLink link = new HtmlLink();
            link.Href = "~/app_themes/popupedit/PopupStyle.css";
            link.Attributes.Add("rel", "Stylesheet");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("media", "all");
            Page.Header.Controls.Add(link);

            btnCancel.Style.Add("display", "none");

            htmlEditor.SetSecurityImageGalleryPath("~/Repository");
            htmlEditor.SetSecurityFlashGalleryPath("~/Repository");
            htmlEditor.SetSecurityGalleryPath("~/Repository");
            htmlEditor.SetSecurityMediaGalleryPath("~/Repository");
            //htmlEditor.BaseHref = CMSContext.Current.AppPath;

            htmlEditor.EditorWysiwygModeCss = stylesheetsList;// String.Format("~/App_Themes/Default/ecf.css");
            htmlEditor.SetSecurityFilesGalleryPath("~/Repository");

            if (ViewState["Changed"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                "InitEditor('" + ValueFCK.ClientID + "','" + Request["Object"] + "');",
                true);

                ViewState["Changed"] = "1";
            }

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }
        #endregion

        #region Page_PreRender
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            htmlEditor.Text = ValueFCK.Value;
            btnCancel.Style.Add("display", "inline");
        }
        #endregion

        #region btnOK_Click
        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnOK_Click(object sender, EventArgs e)
        {
            ValueFCK.Value = htmlEditor.Text;

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                "loadEditor('" + ValueFCK.ClientID + "','" + Request["Object"] + "');" +
                "window.close();",
                true);
        }

        #endregion

        #region btnCancel
        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnCancel_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                "cancelEditor('" + Request["Object"] + "');" +
                "window.close();",
                true);
        }
        #endregion
    }
}