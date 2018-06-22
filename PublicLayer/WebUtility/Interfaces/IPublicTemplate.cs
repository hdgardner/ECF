using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


/// <summary>
/// Summary description for IPublicTemplate
/// </summary>
/// 
namespace Mediachase.Cms.Util
{

    public interface IPublicTemplate
    {
        /// <summary>
        /// Gets the control places.
        /// </summary>
        /// <value>The control places.</value>
        string ControlPlaces{get;}
    } 
}
