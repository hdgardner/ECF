using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms;
using Mediachase.Cms.Pages;

namespace Mediachase.Cms.Web.UI.Controls
{
	/// <summary>
	/// Base class for static user controls.
	/// </summary>
	public class BaseStaticUserControl : UserControl
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStaticWrapper"/> class.
        /// </summary>
        public BaseStaticUserControl()
		{
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

        protected override void OnPreRender(EventArgs e)
        {
            if (PageDocument.Current.StaticNode.Controls[this.ID] != null)
                this.Attributes.Add("ActionSet", "NoneMenu");

            base.OnPreRender(e);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            bool isStatic = false;

            if (PageDocument.Current.StaticNode.Controls[this.ID] != null && CMSContext.Current.IsDesignMode)
                isStatic = true;

            if (isStatic)
            {
                writer.AddAttribute("id", this.ClientID);
                //writer.AddAttribute("ActionSet", "NoneMenu");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
                writer.AddStyleAttribute(HtmlTextWriterStyle.ZIndex, "1");

                foreach (string attr in this.Attributes.Keys)
                {
                    writer.AddAttribute(attr, this.Attributes[attr]);
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
            }
            base.Render(writer);
            if (isStatic)
            {
                writer.RenderEndTag();
            }
        }
        
	}
}