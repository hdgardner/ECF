using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace Mediachase.Cms.Web.UI.Controls
{
    /// <summary>
    /// Summary description for IDataAdapter
    /// </summary>
    public interface ICmsDataAdapter
    {
        // DB -> Web
        /// <summary>
        /// Sets the param info.
        /// </summary>
        /// <param name="param">The param.</param>
        void SetParamInfo(object param);

        // Web -> DB
        //void GetParamInfo(object param);
    }
}