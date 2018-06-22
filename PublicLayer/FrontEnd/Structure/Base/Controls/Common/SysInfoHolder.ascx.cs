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

public partial class Controls_Common_SysInfoHolder : System.Web.UI.UserControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Visible && !Page.ClientScript.IsClientScriptBlockRegistered("ChangeDesignMode"))
        {
            StringBuilder ChangeDesign = new StringBuilder();
            ChangeDesign.Append(@"<script type='text/javascript'>");
            ChangeDesign.Append("function ChangeDesignMode(val){");
            ChangeDesign.Append("var obj = document.getElementById('IsDesignMode');");
            ChangeDesign.Append("obj.value = val;");
            ChangeDesign.Append("} </script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ChangeDesignMode", ChangeDesign.ToString());
        }
    }
}
