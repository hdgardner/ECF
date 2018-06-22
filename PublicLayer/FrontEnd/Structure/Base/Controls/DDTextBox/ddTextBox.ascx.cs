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

public partial class Controls_DDTextBox_ddTextBox : System.Web.UI.UserControl
{

    /// <summary>
    /// Gets the text value.
    /// </summary>
    /// <value>The text value.</value>
    public string TextValue
    {
        get
        {
            return textTarget.Value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.Visible) this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), new Guid().ToString(), "InitTaObjscts('" + textTarget.ClientID + "','" + ddTarget.ClientID + "');", true);
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (this.Attributes["display"] != null)
            ddTextBox.Style.Add("display", this.Attributes["display"]);
    }
}
