using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Web.Console.Interfaces
{
    /// <summary>
    /// Summary description for ITextControl
    /// </summary>
    public interface ITextEditorControl : ITextControl
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        Unit Height { get; set;}
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        Unit Width { get; set;}
    }
}