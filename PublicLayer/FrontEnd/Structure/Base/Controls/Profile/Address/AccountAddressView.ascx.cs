using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Controls;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.Controls;
using Mediachase.Cms.Web.UI.Controls;

namespace Mediachase.Cms.Controls
{
    public partial class AccountAddressView : System.Web.UI.UserControl, ICmsDataAdapter
    {
		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CMSContext.Current.IsDesignMode)
				divViewer.InnerHtml = "Address List Control";
		}

		#region ICmsDataAdapter Members

		/// <summary>
		/// Sets the param info.
		/// </summary>
		/// <param name="param">The param.</param>
		public void SetParamInfo(object param)
		{
			ControlSettings settings = (ControlSettings)param;

			if (settings != null && settings.Params != null)
			{
				this.DataBind();
			}
		}

		#endregion
	}
}