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
using System.Text;

public partial class Modules_CultureHolder : System.Web.UI.UserControl,INamingContainer
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Visible && !Page.ClientScript.IsStartupScriptRegistered("ChangeCurrentCulture"))
        {
            StringBuilder ChangeCulture = new StringBuilder();
            ChangeCulture.Append("function ChangeCurrentCulture(cult){");
			ChangeCulture.Append("var CurrentCulture = document.getElementById('CurrentCulture');");
            ChangeCulture.Append("CurrentCulture" + ".value = cult;return true;");
            ChangeCulture.Append("}");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ChangeCurrentCulture", ChangeCulture.ToString(), true);
        }        
    }
}
