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

public partial class Controls_Common_LinkImageButton : System.Web.UI.UserControl
{

    #region property: InnerImage
    //private ImageButton innerImage;
    /// <summary>
    /// Gets the inner image.
    /// </summary>
    /// <value>The inner image.</value>
    public ImageButton InnerImage
    {
        get { return this.imgButton; }
    } 
    #endregion

    #region property: Text
    private string text = string.Empty;
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
        get { return text; }
        set { text = value; }
    } 
    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //mainDiv.Attributes.Add("onmouseover", String.Format("linkImageButtonHandler('{0}', {1});", lblText.ClientID, "'inline'"));
        //mainDiv.Attributes.Add("onmouseout", String.Format("linkImageButtonHandler('{0}', {1});", lblText.ClientID, "'none'"));

        lblText.Text = text;
    }
}
