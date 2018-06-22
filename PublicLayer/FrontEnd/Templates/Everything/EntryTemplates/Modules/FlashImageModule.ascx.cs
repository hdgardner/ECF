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

public partial class Templates_Everything_Entry_Modules_FlashImageModule : System.Web.UI.UserControl
{
    private string _Code;

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    public string Code
    {
        get
        {
            return _Code;
        }
        set
        {
            _Code = value;
        }
    }

    /// <summary>
    /// Gets the XML source URL.
    /// </summary>
    /// <value>The XML source URL.</value>
    protected string XmlSourceUrl
    {
        get
        {
            return String.Format("{1}?xsl=xslt/images.xsl%26code={0}", this.Code, this.TemplateSourceDirectory + "/imagesxml.aspx");
        }
    }

    /// <summary>
    /// Gets the flash URL.
    /// </summary>
    /// <value>The flash URL.</value>
    protected string FlashUrl
    {
        get
        {
            return String.Format("{0}", this.TemplateSourceDirectory + "/cycler.swf");
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
