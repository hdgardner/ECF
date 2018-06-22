using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Cms.Web
{
    /// <summary>
    /// Summary description for IMasterPage
    /// </summary>
    public interface IMasterPage
    {
        /// <summary>
        /// Gets the get PH template.
        /// </summary>
        /// <value>The get PH template.</value>
        PlaceHolder GetPHTemplate { get; }
    }
}