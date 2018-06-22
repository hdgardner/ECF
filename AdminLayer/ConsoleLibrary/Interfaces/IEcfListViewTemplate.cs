using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Mediachase.Web.Console.Interfaces
{
	/// <summary>
	/// Summary description for IEcfListViewTemplate
	/// </summary>
	public interface IEcfListViewTemplate
	{
        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
		object DataItem { get; set; }
	}
}