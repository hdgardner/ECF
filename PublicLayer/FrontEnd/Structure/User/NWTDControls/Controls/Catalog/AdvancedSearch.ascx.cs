using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Search;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	public partial class AdvancedSearch : System.Web.UI.UserControl {
		protected void Page_Load(object sender, EventArgs e) {


			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(),"Search_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/Search.js"));

			FacetGroup[] facets = SearchFilterHelper.Current.GetFacets(false, new TimeSpan());


			foreach (FacetGroup facetgroup in facets) {

				switch (facetgroup.FieldName) {
					case "Grade":
						ddlGrades.DataSource = facetgroup.Facets;
						ddlGrades.DataBind();
						ddlGrades.Items.Insert(0, new ListItem(facetgroup.Name, string.Empty));
						break;
					case "Publisher":
						ddlPublisher.DataSource = facetgroup.Facets;
						ddlPublisher.DataBind();
						ddlPublisher.Items.Insert(0, new ListItem(facetgroup.Name, string.Empty));
						break;
					case "Year":
						ddlYear.DataSource = facetgroup.Facets;
						ddlYear.DataBind();
						ddlYear.Items.Insert(0, new ListItem(facetgroup.Name, string.Empty));
						break;
					case "Subject":
						ddlSubject.DataSource = facetgroup.Facets;
						ddlSubject.DataBind();
						ddlSubject.Items.Insert(0, new ListItem(facetgroup.Name, string.Empty));
						break;
					case "Type":
						ddlType.DataSource = facetgroup.Facets;
						ddlType.DataBind();
						ddlType.Items.Insert(0, new ListItem(facetgroup.Name, string.Empty));
						break;
				}
			}
		}

		protected void btnSumbitSearch_Click(object sender, EventArgs e) {
			string url = "~/catalog/searchresults.aspx";
			
			if (tbSearchTerms.Text != "enter search terms") url = SearchFilterHelper.GetQueryStringNavigateUrl(url,"search",tbSearchTerms.Text,false);
			if (ddlGrades.SelectedIndex != 0) url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Grade", ddlGrades.SelectedValue, false);
			if (ddlPublisher.SelectedIndex != 0) url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Publisher", ddlPublisher.SelectedValue, false);
			if (ddlYear.SelectedIndex != 0) url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Year", ddlYear.SelectedValue, false);
			if (ddlSubject.SelectedIndex != 0) url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Subject", ddlSubject.SelectedValue, false);
			if (ddlType.SelectedIndex != 0) url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Type", ddlType.SelectedValue, false);



			Response.Redirect(url);
		}
	}
}