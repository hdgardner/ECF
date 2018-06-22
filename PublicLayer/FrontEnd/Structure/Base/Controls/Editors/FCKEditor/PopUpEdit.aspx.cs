namespace Mediachase.Cms.Controls.Editors.FCKEditor
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
            btnCancel.Style.Add("display", "none");

            htmlEditor.BasePath = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Structure/Base/Controls/Editors/FCKeditor/");
            //GA: 05.07.2005 resource copy fix VersionId -> PageVersionId
            htmlEditor.ImageBrowserURL = htmlEditor.BasePath + "Browser.aspx?RepositoryPath=~/Repository/&TempPath=~/Temp/&ArchivePath=~/Archive/&Mode=IMAGEONLY&PageVersionId=" + Request.QueryString["VersionId"];
            htmlEditor.LinkBrowserURL = htmlEditor.BasePath + "Browser.aspx?RepositoryPath=~/Repository/&TempPath=~/Temp/&ArchivePath=~/Archive/&Mode=ALL&PageVersionId=" + Request.QueryString["VersionId"];
            //htmlEditor.f.FlashBrowserURL = htmlEditor.BasePath + "Browser.aspx?RepositoryPath=~/Repository/&TempPath=~/Temp/&ArchivePath=~/Archive/&Mode=FLASHONLY&PageVersionId=" + Request.QueryString["VersionId"];
            htmlEditor.StylesXmlPath = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Styles/FCKStyle.xml");

            if (ViewState["Changed"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                "InitFCK('" + ValueFCK.ClientID + "','" + Request["Object"] + "');",
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
            htmlEditor.Value = ValueFCK.Value;
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
            ValueFCK.Value = htmlEditor.Value;

            Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
                "loadFCK('" + ValueFCK.ClientID + "','" + Request["Object"] + "');" +
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
                "cancelFCK('" + Request["Object"] + "');" +
                "window.close();",
                true);
        }
        #endregion
    }
}