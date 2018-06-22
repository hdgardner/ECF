using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Web.UI.Controls;
using System.Text;

public partial class Controls_Image_ImageView : BaseStaticUserControl, ICmsDataAdapter
{
    #region ImageUrl
    private string _imageUrl = string.Empty;

    /// <summary>
    /// Gets or sets the image URL.
    /// </summary>
    /// <value>The image URL.</value>
    public string ImageUrl
    {
        get { return _imageUrl; }
        set
        {
            _imageUrl = value;
			PrimaryImage.Src = value;
        }
    }
    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        CmsPlaceHolder.RegisterStaticControl(this, this, "4B54E0B0-0423-40b6-AF34-52C493902927");
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CMSContext.Current.IsDesignMode)
        {
            divViewer.Visible = true;
            divViewer.InnerHtml = "Image Control";
        }
        else
        {
            divViewer.Visible = false;
        }
    }

	#region ICmsDataAdapter Members

    /// <summary>
    /// Sets the param info.
    /// </summary>
    /// <param name="param">The param.</param>
	public void SetParamInfo(object param)
	{
		try
		{
			ControlSettings settings = (ControlSettings)param;

			if (settings != null && settings.Params != null)
			{
				if (settings.Params["ImageUrl"] != null)
					this.ImageUrl = settings.Params["ImageUrl"].ToString();
			}

			this.DataBind();
		}
		catch
		{
		}
	}

	#endregion
}