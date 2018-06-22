using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Search.Extensions;
using Mediachase.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms.Util;
using Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart;
using NWTD;


namespace Mediachase.Cms.Website.Templates.NWTD.Modules {
	/// <summary>
	/// A template for book sarches
	/// </summary>
	public partial class BookSearchControl : Mediachase.Cms.WebUtility.BaseStoreUserControl {
		
		#region Fields

		private IDictionary _parameters;
		

		#endregion

		#region Properties

		/// <summary>
		/// How the results will be sorted. Obtained from the Query String, uses NWTD's default sort if no query string is present
		/// </summary>
		protected SearchSort SortBy { 
			get{
				if(string.IsNullOrEmpty(Request.QueryString["sort"])) return this.DefaultSort;
				return new SearchSort(Request.QueryString["sort"]);
			}
		}

		/// <summary>
		/// The way results will be sorted by default
		/// </summary>
		public SearchSort DefaultSort {
			get {
				return new SearchSort("TypeSort");
				//return new SearchSort(
				//    new SearchSortField[] {
				//        new SearchSortField("TypeSort"),
				//        new SearchSortField("Publisher"),
				//        new SearchSortField("Subject"),
				//        new SearchSortField("Year",true),
				//        new SearchSortField("GradeNumber"),
				//        new SearchSortField("Type"), 
				//        new SearchSortField("DisplayName") 
				//    }
				//);
			}
		}

		/// <summary>
		/// Search keywords. Obtained from the Query String
		/// </summary>
		protected string KeyWords {
			get { return String.IsNullOrEmpty(Request.QueryString["search"]) ? string.Empty : Request.QueryString["search"].ToLower() ; } 
		}
		/// <summary>
		/// The search results page we're currently viewing. Obtained from the Query String
		/// </summary>
		protected int PageNumber {
			get { return String.IsNullOrEmpty(Request.QueryString["page"]) ? 1 : int.Parse(Request.QueryString["page"]); } 
		}

		/// <summary>
		/// The number of items to show per page
		/// </summary>
		public int ItemsPerPage{
			get { return int.Parse(this._parameters["RecordsPerPage"].ToString()); }
		}

		/// <summary>
		/// The start index of the current page. Calculated from the page PagNumber and ItemsPerPage properties
		/// </summary>
		public int StartIndex {
			get { return (this.ItemsPerPage * this.PageNumber) - this.ItemsPerPage; }
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// When the page loads, we'll perform the search based on the criteria defined in the query string.
		/// Some of this is automatically done by ECF, for better or for worse, other things are done by us in our
		/// public properties such as KeyWords
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
	
			//before we conduct the search, we'd better make sure the prices are set for the user
			global::NWTD.Profile.SetSaleInformation();

            //Hide the "Add Selected items to cart" button on the top of the results
            this.srBookSearch.AddToCartTop.Visible = false;
			
            
            //The criteria for this search is going to be based on the query string.

			//first get some variables ready
			int count = 0;
			bool cacheResults = true;
			TimeSpan cacheTimeout = new TimeSpan(0, 0, 30); //by default we'll have our cache timeout after 30 seconds
			if (String.IsNullOrEmpty(this.KeyWords))
				cacheTimeout = new TimeSpan(0, 1, 0); //if there's no keyword search, we'll up the timeout to a minute

			//get the current filter 
			//(ECF will automatically pull filters from the query string)
			SearchFilterHelper filter = SearchFilterHelper.Current;

			
			string keyWords = this.KeyWords;
			////check to see if the KeyWords matches an ISBN Pattern
			//if(System.Text.RegularExpressions.Regex.IsMatch(keyWords, @"^[0-9-]+[a-z-0-9]?$")){ //any string of all hyphens or numbers and possibly an alpha character at the end
			//    keyWords = keyWords.Replace("-",string.Empty);
			//}


			//build the criteria based on sort and keywords
			CatalogEntrySearchCriteria criteria = filter.CreateSearchCriteria(keyWords, this.SortBy);
		

			// the current catalog if that's not been added yet (which it shouldn't be)
			if (criteria.CatalogNames.Count == 0) {
				CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
				if (catalogs.Catalog.Count > 0) {
					foreach (CatalogDto.CatalogRow row in catalogs.Catalog) {
						if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
							criteria.CatalogNames.Add(row.Name);
					}
				}
			}

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

			//setting this in the helper class now
			//Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(Int32.MaxValue);
			//exectute the search
			Entries entries = filter.SearchEntries (
				criteria, 
				this.StartIndex, 
				this.ItemsPerPage, 
				out count, 
				new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo), 
				cacheResults, 
				cacheTimeout
			);

			//Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(1024);
			//the number of results that were returned
			int resultsCount = entries.Entry != null ? entries.Entry.Count() : 0;

			if (entries.Entry != null && entries.Entry.Count() > this.ItemsPerPage) { //ECF's search helper pads the results by 5
				Entry[] entryset = entries.Entry;
				Array.Resize<Entry>(ref entryset, this.ItemsPerPage);
				
				entries.Entry = entryset;
			}

			int fromResult = (this.StartIndex + 1);
			int toResult = this.StartIndex + (this.ItemsPerPage > resultsCount ? resultsCount : this.ItemsPerPage);

			//indicate to the user what page we're on
			if (resultsCount > 0)
				this.litPageNumber.Text = string.Format("({0}-{1} of {2})", fromResult.ToString(), toResult.ToString(), count.ToString());
			else {
				//this.ddlSortBy.Visible = false;
			}

			decimal numberOfPages = 1 + Convert.ToDecimal( Math.Floor( Convert.ToDouble((count / this.ItemsPerPage)) ));

			//if we're not on page 1, add a prev button to the pager
			if (this.PageNumber != 1) {
				this.blPager.Items.Add(new ListItem("Prev", SearchFilterHelper.GetQueryStringNavigateUrl("page", (PageNumber - 1).ToString())));
				this.blBottomPager.Items.Add(new ListItem("Prev", SearchFilterHelper.GetQueryStringNavigateUrl("page", (PageNumber - 1).ToString())));

			}
			//if we're not on the last page, add a next button to the pager
			if (this.PageNumber != numberOfPages) {
				this.blPager.Items.Add(new ListItem("Next", SearchFilterHelper.GetQueryStringNavigateUrl("page", (PageNumber + 1).ToString())));
				this.blBottomPager.Items.Add(new ListItem("Next", SearchFilterHelper.GetQueryStringNavigateUrl("page", (PageNumber + 1).ToString())));

			}

			//this.srBookSearch.ResultsGrid.ShowFooter = true;
			this.srBookSearch.ResultsGrid.Parent.Controls.AddAt(this.srBookSearch.ResultsGrid.Parent.Controls.IndexOf(this.srBookSearch.ResultsGrid) + 1, this.pnlBottomPager);

			//I can't figure out why this needs to be cast. For some reason VS is calling it a UserControl, not a SideMenu
			FiltersSideMenu.Filters = filter.SelectedFilters;

            //Only show facet if results are more than 1 - Heath Gardner 01/22/16 ////////////////////////////////////////
            FacetGroup[] facets = null;


            if (resultsCount > 1)
            {
                facets = filter.GetFacets(cacheResults, cacheTimeout);
            }

            FiltersSideMenu.Facets = facets;
            //End modify//////////////////////////////////////////////////////////////////////////////////////////////////

            //Original facet logic that was replaced with the above - Heath Gardner 01/22/16
                //FacetGroup[]  facets = filter.GetFacets(cacheResults, cacheTimeout);
                //FiltersSideMenu.Facets = facets;

			//Bind the results to our search results control
			this.srBookSearch.Entries = entries;
			this.srBookSearch.TotalResults = count;

            //We don't want them to be able to just view everything with no search (for reasons unbeknownst to me)
            if (Request.QueryString.Count == 0)
            {
                this.srBookSearch.Visible = false;
                this.blPager.Visible = false;
                this.blBottomPager.Visible = false;
                this.litPageNumber.Visible = false;
                this.pnlBrowseCatalog.Visible = true;
                this.pnlSearchHead.Visible = false;
                //return;
            }
			//if there are no results, we need to hide certain things
            else if (count == 0)
            {
				this.srBookSearch.Visible = false;
				this.blPager.Visible = false;
				this.blBottomPager.Visible = false;
				this.litPageNumber.Visible = false;
				this.pnlNoResults.Visible = true;
				this.pnlSearchHead.Visible = false;
				//return;
			}

            if (!string.IsNullOrEmpty(this.KeyWords))
                this.litSearchString.Text = string.Format("<span class=\"nwtd-searchString\">for \"{0}\"</span>",this.KeyWords);

			DataBind();

		}

		/// <summary>
		/// This event handler existed for some sort dropdown lists that were once part of the UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlSortBy_SelectedIndexChanged(object sender, EventArgs e) {
			string val = ((DropDownList)sender).SelectedValue;
			Response.Redirect(SearchFilterHelper.GetQueryStringNavigateUrl("sort", val));
		}

		/// <summary>
		/// This event handler existed for some sort dropdown lists that were once part of the UI
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlSortBy_PreRender(object sender, EventArgs e) {
			if (!String.IsNullOrEmpty(Request.QueryString["sort"])) {
				CommonHelper.SelectListItem((DropDownList)sender, Request.QueryString["sort"]);
			}
		}

		#endregion

		#region Methods

		public override void LoadContext(IDictionary context) {
			this._parameters = context;

			if (context.Contains("Depository")) this.srBookSearch.Depository = (global::NWTD.InfoManager.Depository)Enum.Parse( typeof(global::NWTD.InfoManager.Depository), context["Depository"].ToString() );

			if (!String.IsNullOrEmpty(Request.QueryString["search"]))
				_parameters["FTSPhrase"] = Request.QueryString["search"];
		}

		#endregion
	}
}