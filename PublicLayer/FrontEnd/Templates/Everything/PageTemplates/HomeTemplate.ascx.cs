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

using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms;

public partial class Templates_Home : BaseStoreUserControl,IPublicTemplate
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(Object sender, EventArgs e)
    {
        this.EnsureID();
        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("rotator"))
        {
            this.Page.ClientScript.RegisterClientScriptInclude("rotator", Page.ResolveClientUrl("~/client_scripts/fadeshow.js"));
        }

        if (!this.Page.ClientScript.IsClientScriptBlockRegistered("rotator-block"))
        {
            string rotatorScript = String.Empty;
            rotatorScript += "var fadeimages=new Array();";
            rotatorScript += String.Format("fadeimages[0]=[\"{0}\", \"Digital-Cameras.aspx\", \"\"];", this.ResolveClientUrl("~/App_themes/" + Page.Theme + "/images/home/main-cameras.png"));
            rotatorScript += String.Format("fadeimages[1]=[\"{0}\", \"Mobile-Phones.aspx\", \"\"];", this.ResolveClientUrl("~/App_themes/" + Page.Theme + "/images/home/main-phones.png"));
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "rotator-block", rotatorScript, true);
        }

        //if(!IsPostBack)
        DataBind();
    }

    /// <summary>
    /// Handles the Click event of the CompareButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    protected void CompareButton_Click(object sender, ImageClickEventArgs e)
    {
        /*
        int electronicsMetaClass = 11;
        CommonHelper.ClearCompareCookie();
        int res = CommonHelper.SetCompareCookie("2", electronicsMetaClass.ToString());
        res = CommonHelper.SetCompareCookie("7", electronicsMetaClass.ToString());
        res = CommonHelper.SetCompareCookie("8", electronicsMetaClass.ToString());
        res = CommonHelper.SetCompareCookie("9", electronicsMetaClass.ToString());
        Response.Redirect(ClientHelper.FormatUrl("products_compare"));
         * */
		Response.Redirect(ResolveUrl(String.Format("~/compare.aspx")));
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_PreRender(object sender, EventArgs e)
	{
	}

    #region IPublicTemplate Members

    /// <summary>
    /// Gets the control places.
    /// </summary>
    /// <value>The control places.</value>
    public string ControlPlaces
    {
        get 
        {

            return "MainContentArea";
        }
    }

    #endregion
}
