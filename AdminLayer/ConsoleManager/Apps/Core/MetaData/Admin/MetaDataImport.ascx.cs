using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	///	</summary>
	public partial class MetaDataImport : CoreBaseUserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (ViewControl != null)
			{
				ViewControl.AppId = ManagementHelper.GetAppIdFromQueryString();
				ViewControl.ViewId = ManagementHelper.GetViewIdFromQueryString();
			} 
		}
	}
}