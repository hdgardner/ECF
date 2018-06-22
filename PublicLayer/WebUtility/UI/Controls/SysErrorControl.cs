using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Mediachase.Cms.WebUtility.UI.Controls
{
    public class SysErrorControl : WebControl
    {
        private string _text = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysErrorControl"/> class.
        /// </summary>
        /// <param name="Text">The text.</param>
        public SysErrorControl(string Text)
        {
            this._text = Text;
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {            
            writer.Write("Error during loading control : <br/>");
            writer.Write(this._text);
        }
    }
}
