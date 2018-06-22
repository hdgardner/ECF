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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

public partial class Apps_Marketing_Promotions_Simple_SimpleControl : MarketingBaseUserControl, IAdminTabControl, IAdminContextControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region IAdminTabControl Members

    /// <summary>
    /// Saves the changes.
    /// </summary>
    /// <param name="context">The context.</param>
    public void SaveChanges(IDictionary context)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    #region IAdminContextControl Members

    /// <summary>
    /// Loads the context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void LoadContext(IDictionary context)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion
}
