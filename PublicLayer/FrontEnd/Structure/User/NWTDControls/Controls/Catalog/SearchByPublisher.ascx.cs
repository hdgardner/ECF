using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Search;
using Mediachase.Cms.WebUtility.Search;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	
	/// <summary>
	/// Control that allows you to search by publisher and keyword. Current search values are read from the query string.
	/// </summary>
	public partial class SearchByPublisher : System.Web.UI.UserControl {

		#region Fields

		private string _defaultSearchText = "Enter ISBN or Keyword";

		#endregion

		#region Properties

		/// <summary>
		/// The initial text to be in the search box
		/// </summary>
		public string DefaultSearchText {
			get { return this._defaultSearchText; }
			set { this._defaultSearchText = value; }
		}

		/// <summary>
		/// Whether to show the label next to the search box
		/// </summary>
		public bool ShowLabel {
			get { return this.lblearch.Visible; }
			set { this.lblearch.Visible = value; }
		}

		/// <summary>
		/// The text for the label next to the search box
		/// </summary>
		public string LabelText {
			get { return this.lblearch.Text; }
			set { this.lblearch.Text = value; }
		}

		/// <summary>
		/// Whether to show the advanced search controls
		/// </summary>
		public Boolean ShowAdvancedSearch { 
			get { return this.hlAdvancedSearch.Visible; } 
			set { this.hlAdvancedSearch.Visible = value; } 
		}
		
		/// <summary>
		/// The text for the submit button
		/// </summary>
		public String ButtonText { 
			get { return this.btnSubmitSearch.Text; } 
			set { this.btnSubmitSearch.Text = value; } 
		}

		/// <summary>
		/// Whether to allow a search to occur for the default text that gets entered
		/// </summary>
		public bool AllowSearchForDefaultText { get; set; }

		/// <summary>
		/// The current search string. This will come from the querystring.
		/// </summary>
		protected string KeyWords {
			get { return String.IsNullOrEmpty(Request.QueryString["search"]) ? string.Empty : Request.QueryString["search"]; }
		}
		
		/// <summary>
		/// The selected publisher for the current search (read from the query string)
		/// </summary>
		protected string Publisher {
			get { return String.IsNullOrEmpty(Request.QueryString["Publisher"]) ? string.Empty : Request.QueryString["search"]; }
		}

		/// <summary>
		/// The selected publisher in the dropdown
		/// </summary>
		protected string SelectedPublisher {
			get {
				string publisher = string.Empty;
				if(this.Request["Publisher"]!=null){
					publisher = this.Request["Publisher"];
				}
				return publisher;
			}
		}


		#endregion

		#region Event Handlers

		/// <summary>
		/// During page load, we use lucene search API to get a list of publishers. This information comes from the cache for performance.
		/// That information is bound to the search dropdown.
		/// Also, rebinds the freetext search to the search box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			if (NWTD.Profile.CurrentUserLevel.Equals(NWTD.UserLevel.ANONYMOUS)) { this.pnlSearchByPublisher.Visible = false; return; }

			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "Search_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/Search.js"));
			this.btnSubmitSearch.Text = this.ButtonText;

			//it would be weird to show the facet if it's already there
			//if (!CMSContext.Current.CurrentUrl.Contains("/catalog/searchresults.aspx")) {
			this.ddlPublisher.Visible = true;

			//The key used for retrieving cache data
			string cacheKey = Mediachase.Commerce.Catalog.CatalogCache.CreateCacheKey("search-publisher-names");

			FacetGroup publisherfacet;

			// check cache first
			object cachedObject = Mediachase.Commerce.Catalog.CatalogCache.Get(cacheKey);
			if (cachedObject != null) {
				publisherfacet = (FacetGroup)cachedObject;
			}
			else {
				//So far, this seems like the best way to ennumerate the existing Publishers
				//Although, it does seem kind of convoluted.
				//Anyway, get the list using search and then cache it
				Mediachase.Search.Extensions.CatalogEntrySearchCriteria criteria = new Mediachase.Search.Extensions.CatalogEntrySearchCriteria();

				if (!string.IsNullOrEmpty(global::NWTD.Catalog.UserStateAvailablityField)) {
					//Incorporate the state availablity flag
					criteria.Add(
						global::NWTD.Catalog.UserStateAvailablityField,
						new SimpleValue() {
							key = string.Empty,
							value = "y",
							locale = "en-us",
							Descriptions = new Descriptions() {
								defaultLocale = "en-us"
							}
						});
				}

				//Create a search criteria object
				criteria.Add(SearchFilterHelper.Current.SearchConfig.SearchFilters.SingleOrDefault(filter => filter.field == "Publisher"));
				var manager = new SearchManager(Mediachase.Commerce.Core.AppContext.Current.ApplicationName);

				//conduct the search
				Mediachase.Search.SearchResults results = manager.Search(criteria);

				//get the results
				FacetGroup[] facets = results.FacetGroups;

				//get the publisher field
				publisherfacet = facets.SingleOrDefault(facet => facet.FieldName == "Publisher");


				//cache the results for five minutes for faster future loads
				TimeSpan cacheTimeout = new TimeSpan(0, 5, 00);
				Mediachase.Commerce.Catalog.CatalogCache.Insert(cacheKey, publisherfacet, cacheTimeout);
			}


			//Bind the publisehrs to the dropdown
			this.ddlPublisher.DataSource = publisherfacet.Facets;
			this.ddlPublisher.DataBind();

			//Add an empty item for searching all publishers
			this.ddlPublisher.Items.Insert(0, new ListItem("All Publishers"));
			
			//select the currently selected publisehr
			foreach (ListItem item in ddlPublisher.Items) {
				if (item.Value == this.SelectedPublisher) {
					item.Selected = true;
				}
			}
			
			//Re-do the keyword in the textbox as well
			this.tbKeyWord.Text = string.IsNullOrEmpty( this.KeyWords) ?this.DefaultSearchText : this.KeyWords ;
			//this.ddlPublisher.SelectedValue = this.Publisher;

		}

		/// <summary>
		/// When someone clicks the search button, 
		/// build a url that includes all the relevant search criteria in the query string:
		/// <list type="bullet">
		///	<item>Keyword</item>
		///	<item>Publisher</item>
		/// </list>
		/// The URL is built using ECF's  SearchFilterHelper.GetQueryStringNavigateUrl method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSubmitSearch_Click(object sender, EventArgs e) {

			//get the url of the search results page
			string searchPageUrl = NavigationManager.GetUrl("Search");

			//base the new search on the current url and start passing it along
			string url = CMSContext.Current.CurrentUrl;
		
			//if we're not already on the search results page, get the correct url
			if (!url.Contains( this.ResolveUrl( searchPageUrl) )) url = searchPageUrl;

			//first, deal with the keyword.
            if ((!this.AllowSearchForDefaultText && tbKeyWord.Text != this.DefaultSearchText)) //if there is a keyword, add it to the query string
                url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Search", tbKeyWord.Text, false);
            else
                url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Search", string.Empty, true); //this method has an overload that allows you to omit the value, but it doesn't seem to work, so we're passing an empty string and false to remove the value from the url

			//next deal with the selected publisher
			if (ddlPublisher.Visible) {
				if ( ddlPublisher.SelectedIndex != 0 && ddlPublisher.SelectedValue != this.Publisher) //if there is a selected publisher, and its not the current one, chang it
					url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Publisher", ddlPublisher.SelectedValue, false);
				//if no publisher is selected, remove publisher from the query string altogether
				else url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "Publisher", string.Empty, true); //this method has an overload that allows you to omit the value, but it doesn't seem to work, so we're passing an empty string and false to remove the value from the url
			}
			//remove the page number
			url = SearchFilterHelper.GetQueryStringNavigateUrl(url, "page",string.Empty, true);
			Response.Redirect(url);
		}

		#endregion
	}
}