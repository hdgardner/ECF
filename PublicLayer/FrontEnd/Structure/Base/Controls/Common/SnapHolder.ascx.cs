using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using ComponentArt.Web.UI;
using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebActionSet;
using Mediachase.Commerce.Profile;

public partial class Controls_Common_SnapHolder : System.Web.UI.UserControl, IScriptControl
{
    #region UserUID Property
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
    #endregion

    #region Page Events

    #region OnInit

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }
    #endregion

    #region Page_Load

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CMSContext.Current.IsDesignMode)
            ClearHiddenSnapFields();

        //GA: 01.06.2006
        if (CMSContext.Current.IsDesignMode)
        {
            //REGISTER SCRIPT
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/StringBuilder.js") + "'></script>");
			/*//done in client side Mediachase.Cms.Snap class
			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/Snap.js") + "'></script>");

            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), this.ClientID, "InitStaticEditedContainer('" + editedInfo.ClientID + "');InitPopUpTempContainer('" + PopUpTemp.ClientID + "');", true);
            this.Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), Guid.NewGuid().ToString(), "RecordAllInfo()");*/
        }

        #region Load Controls
        /* done in template.cs
        if (CMSContext.Current.IsDesignMode)
            Mediachase.Cms.Pages.PageDocument.Current.Open(CMSContext.Current.VersionId, OpenMode.Design, UserUID);
        else
            Mediachase.Cms.Pages.PageDocument.Current.Open(CMSContext.Current.VersionId, OpenMode.View, UserUID);
         * */

		//done in client side Mediachase.Cms.Snap class
        /*if (CMSContext.Current.IsDesignMode)
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString().Replace("-", ""), "InitTempAddingContainer('" + isModified.ClientID + "');", true);*/

        #endregion

        this.Page.PreRenderComplete +=new EventHandler(Page_PreRenderComplete);
    }

    /// <summary>
    /// Handles the PreRenderComplete event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void  Page_PreRenderComplete(object sender, EventArgs e)
    {
 	    #region ActionSet
        string ConnectionStr = "";
        ConnectionStringSettingsCollection cssc = WebConfigurationManager.ConnectionStrings;
        if (cssc["XmlActionSetProvider"] != null)
        {
            ConnectionStr = cssc["XmlActionSetProvider"].ConnectionString;
        }
        if (ConnectionStr != string.Empty && CMSContext.Current.IsDesignMode)
        {
            XmlActionSetProvider asp = new XmlActionSetProvider("Administrator", this.MapPath(ConnectionStr), this.Page);
            asp.RegisterPage(true);
        }
        #endregion
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
        if ((this.IsPostBack) && (PageDocument.Current.IsModified))
        {
            Mediachase.Cms.Pages.PageDocument.Current.Save(CMSContext.Current.VersionId, SaveMode.TemporaryStorage, UserUID);
        }


    }
    #endregion

    #endregion
    
    #region  Snap Property
    /// <summary>
    /// Temporary array for deleting dynamic nodes from collection
    /// </summary>
    private ArrayList dnDeleted = new ArrayList();
   
    /// <summary>
    /// Temporary array for saving all snapins
    /// </summary>
    private ArrayList snapAL = new ArrayList();

    #region ControlCollection Property
    public ArrayList _controlCollection;

    /// <summary>
    /// Gets or sets the control collection.
    /// </summary>
    /// <value>The control collection.</value>
    public ArrayList ControlCollection
    {
        get
        {
            return _controlCollection;
        }
        set
        {
            _controlCollection = value;
        }
    }
    #endregion
    #endregion

    #region Snap Methods
    #region ClearHiddenSnapFields
    /// <summary>
    /// Clears the hidden snap fields.
    /// </summary>
    public void ClearHiddenSnapFields()
    {
        editedInfo.Value = string.Empty;
        deletedInfo.Value = string.Empty;
        dnDeleted.Clear();
        isModified.Value = string.Empty;
    }
    #endregion
    #endregion

	protected override void OnPreRender(EventArgs e)
	{
		if (!this.DesignMode && CMSContext.Current.IsDesignMode)
		{
			ScriptManager sm = ScriptManager.GetCurrent(this.Page);
			if (sm != null)
			{
				sm.RegisterScriptControl(this);
				sm.RegisterScriptDescriptors(this);
			}
		}
		base.OnPreRender(e);
	}

	#region IScriptControl Members

	public System.Collections.Generic.IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptComponentDescriptor sd = new ScriptComponentDescriptor("Mediachase.Cms.Snap");
		sd.ID = this.ClientID + "_Snap";
		sd.AddProperty("ContainerId", editedInfo.ClientID);
		sd.AddProperty("EditedContainerId", isModified.ClientID);
		sd.AddProperty("PopUpTempContainerId", PopUpTemp.ClientID);
		return new ScriptDescriptor[] { sd };
	}

	public System.Collections.Generic.IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference sr = new ScriptReference(Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("~/Scripts/CMS_Snap.js"));
		return new ScriptReference[] { sr };
	}

	#endregion
}
