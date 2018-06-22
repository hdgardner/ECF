using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Mediachase.Cms.WebUtility.Controls
{
    public class PluginPlaceHolder : PlaceHolder
    {
        #region property: AllowEdit
        private bool _allowEdit = false;
        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; }
        }
        #endregion

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            Image propertyImg = new Image();
            propertyImg.Attributes.Add("ActionBtn", "Props");
            propertyImg.CssClass = "imgProperty";
			propertyImg.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Images/props.gif");
            propertyImg.AlternateText = "Edit section properties";

            base.Render(writer);
            writer.RenderEndTag();
        }
    }
}
