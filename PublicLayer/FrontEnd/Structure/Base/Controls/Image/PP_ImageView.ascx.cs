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
using Mediachase.Cms.Pages;

public partial class Controls_Image_PP_ImageView : System.Web.UI.UserControl, IPropertyPage
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region IPropertyPage Members

    /// <summary>
    /// Loads the specified node uid.
    /// </summary>
    /// <param name="NodeUid">The node uid.</param>
    /// <param name="ControlUid">The control uid.</param>
    void IPropertyPage.Load(string NodeUid, string ControlUid)
    {
		ControlSettings settings = new ControlSettings();

		DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
        if (dNode != null)
        {
            settings = dNode.GetSettings(NodeUid);
        }
        else
        {
            settings = PageDocument.Current.StaticNode.Controls[NodeUid];
        }

		if (settings != null && settings.Params != null)
		{
			Param prm = settings.Params;
			if ((prm["ImageUrl"] != null) && (prm["ImageUrl"] is string))
				txtUrl.Text = prm["ImageUrl"].ToString();
        }
    }

    /// <summary>
    /// Saves the specified node uid.
    /// </summary>
    /// <param name="NodeUid">The node uid.</param>
    /// <param name="ControlUid">The control uid.</param>
    void IPropertyPage.Save(string NodeUid, string ControlUid)
    {
		ControlSettings settings = new ControlSettings();

		DynamicNode dNode = PageDocument.Current.DynamicNodes.LoadByUID(NodeUid);
        if (dNode != null)
        {
            settings = dNode.GetSettings(dNode.NodeUID);
            dNode.IsModified = true;
        }
        else
        {
            settings = PageDocument.Current.StaticNode.Controls[NodeUid];
            if (settings == null)
            {
                settings = new ControlSettings();
                PageDocument.Current.StaticNode.Controls.Add(NodeUid, settings);
            }
        }

        if (settings.Params == null)
        {
            settings.Params = new Param();   
        }

        settings.IsModified = true;

		settings.Params.Remove("ImageUrl"); 

		if (!String.IsNullOrEmpty(txtUrl.Text))
			settings.Params.Add("ImageUrl", txtUrl.Text);
    }

    #endregion

    #region IPropertyPage Members

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title
    {
        get { return "Image"; }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
        get { return "Shows specified image on the page"; }
    }

    #endregion
}
