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
using Mediachase.Web.Console.Interfaces;

public partial class Module_Editors_CuteEditor_EditorControl : System.Web.UI.UserControl, ITextEditorControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #region ITextControl Members

    /// <summary>
    /// Gets or sets the text content of a control.
    /// </summary>
    /// <value></value>
    /// <returns>The text content of a control.</returns>
    public string Text
    {
        get
        {
            return HtmlTextBoxCtrl.Text;
        }
        set
        {
            HtmlTextBoxCtrl.Text = value;
        }
    }

    #endregion

    #region ITextEditorControl Members

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>The height.</value>
    public Unit Height
    {
        get
        {
            return HtmlTextBoxCtrl.Height;
        }
        set
        {
            HtmlTextBoxCtrl.Height = value;
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The width.</value>
    public Unit Width
    {
        get
        {
            return HtmlTextBoxCtrl.Width;
        }
        set
        {
            HtmlTextBoxCtrl.Width = value;
        }
    }

    #endregion
}
