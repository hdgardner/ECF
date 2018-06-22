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

public partial class Controls_Common_OtherInfoHolder : System.Web.UI.UserControl
{

    #region property: ToolBarVisible_Value
    /// <summary>
    /// Gets or sets the tool bar visible_ value.
    /// </summary>
    /// <value>The tool bar visible_ value.</value>
    public string ToolBarVisible_Value
    {
        get { return ToolBarVisible.Value; }
        set { ToolBarVisible.Value = value; }
    } 
    #endregion

    #region property: ToolBoxVisible_Value
    /// <summary>
    /// Gets or sets the tool box visible_ value.
    /// </summary>
    /// <value>The tool box visible_ value.</value>
    public string ToolBoxVisible_Value
    {
        get { return ToolBoxVisible.Value; }
        set { ToolBoxVisible.Value = value; }
    } 
    #endregion

    #region property: tbSettings_Value
    /// <summary>
    /// Gets or sets the tb settings_ value.
    /// </summary>
    /// <value>The tb settings_ value.</value>
    public string tbSettings_Value
    {
        get { return tbSettingsVisible.Value; }
        set { tbSettingsVisible.Value = value; }
    } 
    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
