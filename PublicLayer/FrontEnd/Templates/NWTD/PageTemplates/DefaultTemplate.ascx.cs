using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;

namespace Mediachase.Cms.Website.Templates.NWTD.PageTemplates {

	public partial class DefaultTemplate : BaseStoreUserControl, IPublicTemplate {
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e) {
			this.EnsureID();
			if (!IsPostBack)
				DataBind();
		}

		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e) {
		}

		#region IPublicTemplate Members

		/// <summary>
		/// Gets the control places.
		/// </summary>
		/// <value>The control places.</value>
		public string ControlPlaces {
			get {

				return "MainContentArea";
			}
		}

		#endregion
	}
}
