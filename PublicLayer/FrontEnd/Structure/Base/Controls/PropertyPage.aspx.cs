using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Cms.Pages;
using Mediachase.Cms;
using Mediachase.Commerce.Profile;

public partial class Controls_PropertyPage : System.Web.UI.Page
{
    /// <summary>
    /// Gets the user UID.
    /// </summary>
    /// <value>The user UID.</value>
    public Guid UserUID
    {
        get
        {
            if (this.Page.User.Identity.IsAuthenticated)
                return (Guid)ProfileContext.Current.User.ProviderUserKey;
            else
                return Guid.Empty;
        }

    }

    #region NodeUid
    private string _nodeUID = string.Empty;
    /// <summary>
    /// Gets the node uid.
    /// </summary>
    /// <value>The node uid.</value>
    public string NodeUid
    {
        get
        {
            if (Request["NodeUID"] != null)
                _nodeUID = Request["NodeUID"].ToString().Trim();
            return _nodeUID;
        }
    }
    #endregion

    #region PropertyPagePath
    private string _ppPath = string.Empty;
    /// <summary>
    /// Gets the property page path.
    /// </summary>
    /// <value>The property page path.</value>
    public string PropertyPagePath
    {
        get
        {
            if (Request["PropertyPagePath"] != null)
            {
                _ppPath = /*"~" +*/ Request["PropertyPagePath"].ToString();
            }
            return _ppPath;
        }
    }
    #endregion

    #region ControlUid
    private string _controlUid;
    /// <summary>
    /// Gets the control uid.
    /// </summary>
    /// <value>The control uid.</value>
    public string ControlUid
    {
        get
        {
            if (Request["ControlUID"] != null)
                _controlUid = Request["ControlUID"].ToString().Trim();
            return _controlUid;
        }
    }

    /// <summary>
    /// Gets the version id.
    /// </summary>
    /// <value>The version id.</value>
    public int VersionId
    {
        get
        {
            return int.Parse(Request.QueryString["VersionId"]);
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
        // Initialize current PageDocument
        if (CMSContext.Current.IsDesignMode)
            Mediachase.Cms.Pages.PageDocument.Current = Mediachase.Cms.Pages.PageDocument.Open(VersionId, OpenMode.Design, UserUID);
        else
            Mediachase.Cms.Pages.PageDocument.Current = Mediachase.Cms.Pages.PageDocument.Open(VersionId, OpenMode.View, UserUID);

        Save.Click += new EventHandler(Save_Click);
        Cancel.Click += new EventHandler(Cancel_Click);
        LoadPropertyPage();
    }

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Cancel_Click(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Handles the Click event of the Save control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Save_Click(object sender, EventArgs e)
    {
        ((IPropertyPage)PPage.Controls[0]).Save(NodeUid, ControlUid);
        Mediachase.Cms.Pages.PageDocument.Current.Save(VersionId, SaveMode.TemporaryStorage, new Guid(ProfileContext.Current.User.ProviderUserKey.ToString()));
        Page.ClientScript.RegisterStartupScript(this.GetType(), "closerefresh",
                                   "<script language=javascript>" +
                                   "try {window.opener.UpdateWindow();}" +
                                   "catch (e){} window.close();</script>");
    }

    #region LoadPropertyPage()
    /// <summary>
    /// Loads the property page.
    /// </summary>
    protected void LoadPropertyPage()
    {
        try
        {
            PPage.Controls.Clear();
            UserControl ppctrl = new UserControl();
            ppctrl = (UserControl)LoadControl(PropertyPagePath);
            PPage.Controls.Add(ppctrl);
            ((IPropertyPage)ppctrl).Load(NodeUid, ControlUid);
            HeaderLabel.Text = ((IPropertyPage)ppctrl).Title;
            DescriptionLabel.Text = ((IPropertyPage)ppctrl).Description;
            this.Title = String.Format("{0} settings", ((IPropertyPage)ppctrl).Title);
        }
        catch (HttpException ex)
        {
            if (ex.GetHttpCode() == 404)
                throw new System.IO.FileNotFoundException("Property control not found", ex);
            else
                throw;
        }

    }
    #endregion
}
