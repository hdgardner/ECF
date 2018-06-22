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

public partial class Controls_DDLabel_ddLabel : System.Web.UI.UserControl
{

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
        get { return textTargetLabel.Value; }
        set { textTargetLabel.Value = value; }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.Visible) this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), new Guid().ToString(), "InitTaObjscts('" + textTargetLabel.ClientID + "','" + ddTargetLabel.ClientID + "');", true);
        //if (ViewState["textValue"] != null) textTargetLabel.Value = ViewState["textValue"].ToString();
        
        //commentStorage.Value = textTargetLabel.Value;
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //commentStorage.Value = textTargetLabel.Value;
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    protected override void Render(HtmlTextWriter writer)
    {
        if (this.textTargetLabel.Value.Length == 0) return;
        commentStorage.Value = textTargetLabel.Value;
        //alert(event.pageY); alert(event.screenY); alert(event.layerY); alert(event.offsetY); alert(event.y);
        this.textTargetLabel.Attributes.Add("onmouseover", "MouseEvent = event.offsetY;   timerId = window.setInterval(\"ExchangeText('" + commentStorage.ClientID + "','" + ddTargetLabel.ClientID + "')\", 1000);");//,'" + commentStorage.ClientID + "','" + ddTargetLabel.ClientID + "');");
        this.textTargetLabel.Attributes.Add("onmouseout", "window.clearInterval(timerId); HideDiv('"+ddTargetLabel.ClientID+"');");
        //this.ddTargetLabel.Attributes.Add("onmouseout", "window.clearInterval(timerId); HideDiv('" + textTargetLabel.ClientID + "','" + ddTargetLabel.ClientID + "');");

        //this.textTargetLabel.Attributes.Add("onmouseover", "ExchangeText('"+commentStorage.ClientID+"','"+ddTargetLabel.ClientID+"');");
        //if (textTarget.Value.Length > 30)
        //    writer.Write(textTarget.Value.Substring(0, 27) + "...");
        //else
        //    writer.Write(textTarget.Value);

        //ViewState["textValue"] = textTarget.Value;
        //textTarget.Value = string.Empty;

        //if (this.textTargetLabel.Value.Length > 30)
        //{
        //    ViewState["textValue"] = textTargetLabel.Value;
        //    textTargetLabel.Value = textTargetLabel.Value.Substring(0, 27) + "...";
        //}

        base.Render(writer);

        if (ViewState["textValue"] != null)
            textTargetLabel.Value = ViewState["textValue"].ToString();
    }

}
