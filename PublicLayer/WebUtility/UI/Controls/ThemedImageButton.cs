namespace Mediachase.Cms.Web.UI.Controls
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///	Used to load image from the theme directory.
	/// </summary>
    public partial class ThemedImageButton : ImageButton
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.Page.IsPostBack || !this.IsViewStateEnabled)
            {
                string theme = String.IsNullOrEmpty(Page.Theme) ? "Default" : Page.Theme;
                string path = this.Page.MapPath(String.Format("~/App_Themes/{0}/{1}", theme, ImageUrl));

                if (System.IO.File.Exists(path)) // try current theme
                    this.ImageUrl = String.Format("~/App_Themes/{0}/{1}", theme, ImageUrl);
                else // try default theme
                    this.ImageUrl = String.Format("~/App_Themes/{0}/{1}", "Default", ImageUrl);
            }

            base.OnPreRender(e);
        }
	}
}