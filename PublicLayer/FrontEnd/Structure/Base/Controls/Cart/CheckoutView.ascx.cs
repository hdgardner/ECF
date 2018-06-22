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
using Mediachase.Cms.Dto;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;

public partial class Controls_Cart_CheckoutView : System.Web.UI.UserControl, ICmsDataAdapter
{
    #region prop: DisplayMode
    /// <summary>
    /// Gets the display mode.
    /// </summary>
    /// <value>The display mode.</value>
    private string DisplayMode
    {
        get
        {
            if (Request.QueryString["view"] == null)
                return "checkout";
            else
                return Request.QueryString["view"].ToString();
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
        switch (DisplayMode)
        {
            case "checkout":
                break;
            case "receipt":
                ThemedControlModule1.ThemePath = "Checkout/CheckoutThankYouModule.ascx";
                break;
        }
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CMSContext.Current.IsDesignMode)
            divViewer.InnerHtml = "Checkout Control";
    }

	#region ICmsDataAdapter Members

    /// <summary>
    /// Sets the param info.
    /// </summary>
    /// <param name="param">The param.</param>
	public void SetParamInfo(object param)
	{
		ControlSettings settings = (ControlSettings)param;

		if (settings != null && settings.Params != null)
		{
			//if (settings.Params["ForumPath"] != null)
			//{
			//    news.ForumPath = settings.Params["ForumPath"].ToString();
			//}

			//    if (settings.Params["HeaderText"] != null)
			//    {
			//        rss.HeaderText = settings.Params["HeaderText"].ToString();
			//    }

			//this.DataBind();
		}
	}

	#endregion
}