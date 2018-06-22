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
using Mediachase.Cms.Providers;
using System.Collections.Generic;
using Lucene.Net.Search;
using Mediachase.Search;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Cms.WebUtility;
using System.Text;
using System.Collections.Specialized;
using Lucene.Net.Index;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Templates.NWTD.Modules {
	public partial class SideMenuControl : System.Web.UI.UserControl {
		SearchFilterHelper _searchHelper = SearchFilterHelper.Current;

		/// <summary>
		/// Gets or sets the search helper.
		/// </summary>
		/// <value>The search helper.</value>
		public SearchFilterHelper SearchHelper {
			get { return _searchHelper; }
			set { _searchHelper = value; }
		}

		FacetGroup[] _Facets = null;
		/// <summary>
		/// Gets or sets the facets.
		/// </summary>
		/// <value>The facets.</value>
		public FacetGroup[] Facets {
			get {
				return _Facets;
			}
			set {
				_Facets = value;
			}
		}

		SelectedFilter[] _Filters = null;
		/// <summary>
		/// Gets or sets the filters.
		/// </summary>
		/// <value>The filters.</value>
		public SelectedFilter[] Filters {
			get {
				return _Filters;
			}
			set {
				_Filters = value;
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			global::NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "SideMenuControl_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/SideMenuControl.js"));
			
		}

		public override void DataBind() {
			BindFilters();
			base.DataBind();
		}

		/// <summary>
		/// Binds the filters.
		/// </summary>
		private void BindFilters() {
			if (Filters != null && Filters.Length > 0) {
				ActiveFilterList.DataSource = Filters;
				ActiveFilterList.DataBind();
				ActiveFilterList.Visible = true;
			} else {
				ActiveFilterList.Visible = false;
			}

			if (Facets != null && Facets.Length > 0) {
				FilterList.Visible = true;
				FilterList.DataSource = Facets;
				FilterList.DataBind();
			} else {
				FilterList.Visible = false;
			}
		}

		/// <summary>
		/// Highlights the menu.
		/// </summary>
		private void HighlightMenu() {
		}
	}
}