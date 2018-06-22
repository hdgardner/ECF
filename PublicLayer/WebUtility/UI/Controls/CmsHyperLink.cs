using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Mediachase.Cms.WebUtility.UI.Controls
{
    public class CmsHyperLink : HyperLink
    {
        /// <summary>
        /// Displays the <see cref="T:System.Web.UI.WebControls.HyperLink"/> control on a page.
        /// </summary>
        /// <param name="writer">The output stream to render on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            string imageUrl = this.ImageUrl;
            if (imageUrl.Length > 0)
            {
                Image image = new Image();
                image.ImageUrl = imageUrl; // base.ResolveClientUrl(imageUrl);
                imageUrl = this.ToolTip;
                if (imageUrl.Length != 0)
                {
                    image.ToolTip = imageUrl;
                }
                imageUrl = this.Text;
                if (imageUrl.Length != 0)
                {
                    image.AlternateText = imageUrl;
                }
                image.RenderControl(writer);
            }
            else 
            {
                base.RenderContents(writer);
            }
        }
    }
}
