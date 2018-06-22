using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Search.Extensions;
using System.Collections;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Cms.Website.Templates.NWTD.Modules {

	/// <summary>
	/// Control for displaying a search for related books to a given publicathion
	/// </summary>
	public partial class BookSubSearch : Mediachase.Cms.WebUtility.BaseStoreUserControl {

		#region Event Handlers
		
		/// <summary>
		/// During Init, we'll add some event hanlders to the gridview that's found in the searchresults control
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e) {

			this.srBookSearch.ResultsGrid.AllowPaging = this.AllowPaging;
			this.srBookSearch.ResultsGrid.PageSize = this.PageSize;
			this.srBookSearch.ResultsGrid.AllowSorting = true;
			this.srBookSearch.ResultsGrid.RowDataBound += new GridViewRowEventHandler(ResultsGrid_RowDataBound);
			this.srBookSearch.ResultsGrid.Sorting += new GridViewSortEventHandler(ResultsGrid_Sorting);
			this.srBookSearch.ResultsGrid.PageIndexChanging += new GridViewPageEventHandler(ResultsGrid_PageIndexChanging);
			base.OnInit(e);
		}
		
		/// <summary>
		/// When the page loats for the first time, we'll do a search based on only the book we're looking at, and no other filters applied.
		/// From these results, we'll populate the filters too
		/// Also, load some required JS.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {

			//before we conduct the search, we'd better make sure the prices are set for the user
			global::NWTD.Profile.SetSaleInformation();

			if (!this.Page.IsPostBack) {
				this.Search();
				this.BindFilters();
				this.BindResults();
			}
			OakTree.Web.UI.ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, "SubSearch", "{updatePanelID:'" + this.udpBookSearch.ClientID + "'}");
		}

		/// <summary>
		/// Handle paging
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResultsGrid_PageIndexChanging(object sender, GridViewPageEventArgs e) {
			this.PageIndex = e.NewPageIndex;
			this.AddFiltersToSearch();
			this.Search();
			this.BindResults();
			//throw new NotImplementedException();
		}
		
		/// <summary>
		/// Handle sorts
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResultsGrid_Sorting(object sender, GridViewSortEventArgs e) {
			this.ResetPager();
			this.Criteria.Sort = new SearchSort(e.SortExpression, (e.SortDirection == SortDirection.Ascending));
			this.AddFiltersToSearch();
			this.Search();
			this.BindResults();
		}

		/// <summary>
		/// As data is bound to each row, we'll see if the row is the current Book we're looking at. If so, we'll highlight the row
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ResultsGrid_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.DataRow) {
				Entry entry = e.Row.DataItem as Entry;
				HyperLink link = e.Row.FindControl("linkEntryLink") as HyperLink;
				
				Literal litEntryName = new Literal() { Text = entry.Name };
				link.Parent.Controls.Add(litEntryName);
				link.Visible = false;

				if (entry.ID.Equals(this.SelectedEntryId)) {
					e.Row.RowState = DataControlRowState.Selected;
					
					//this bit was part of brining the current item to the top
					//if(e.Row.RowIndex != 0)
					//    e.Row.Visible = false;
					
				}
			}
		}
		
		/// <summary>
		/// When a filter is selected from the dropdowns
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void filter_changed(object sender, EventArgs e) {
			this.ResetPager();
			this.AddFiltersToSearch();
			this.Search();
			this.BindResults();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Whether to allow paging of the results grid
		/// </summary>
		public bool AllowPaging { get; set; }

		/// <summary>
		/// The way results will be sorted by default
		/// </summary>
		public SearchSort DefaultSort {
			get {
                return new SearchSort("TypeSort"
                    //new SearchSortField[] {
                    //    new SearchSortField("TypeSort"),
                    //    new SearchSortField("Publisher"),
                    //    new SearchSortField("Subject"),
                    //    new SearchSortField("Year",true),
                    //    new SearchSortField("GradeNumber"),
                    //    new SearchSortField("Type"), 
                    //    new SearchSortField("DisplayName") 
                    //}
				);
			}
		}

		/// <summary>
		/// The search criteria that'll be used when Searcn() is called
		/// </summary>
		public CatalogEntrySearchCriteria Criteria { 
			get {
				if (this._criteria == null) this._criteria = SearchFilterHelper.Current.CreateSearchCriteria(string.Empty, this.DefaultSort);
				return this._criteria;
			} 
		}
		
		/// <summary>
		/// The results of the search
		/// </summary>
		public SearchResults Results { get; set; }
		
		/// <summary>
		/// The ID of the book that this sub-search will be based on
		/// </summary>
		public string SelectedEntryId { get; set; }
		
		/// <summary>
		/// Sets the series to use for search
		/// </summary>
		public string Series {
			set {
				if (!string.IsNullOrEmpty(value)) {
					SimpleValue seriesValue = new SimpleValue() {
						key = string.Empty,
						value = value.ToLower(),
						locale = Locale,
						Descriptions = new Descriptions() {
							defaultLocale = Locale
						}
					};

					seriesValue.Descriptions.Description = new Description[] { 
						new Description(){
							Value = value.ToLower(),
							locale = Locale
						} 
					};
					this.Criteria.Add("Series", seriesValue);
				}
			}
		}
		
		/// <summary>
		/// Sets the Grade to use for search
		/// </summary>
		public string Grade {
			set {
				if (!string.IsNullOrEmpty(value)) {
					if (ddlGradeFilter.SelectedIndex > 0) {
						SearchFilter filter = SearchFilterHelper.Current.SearchConfig.SearchFilters.SingleOrDefault(f => f.field == "Grade");
						SimpleValue gradeValue = filter.Values.SimpleValue.SingleOrDefault(v => v.key == value);
						this.Criteria.Add("Grade", gradeValue);
					}
				}
			}
		}
		
		/// <summary>
		/// Sets the Year for the search
		/// </summary>
		public string Year {
			set {
				if (!string.IsNullOrEmpty(value)) {
					SearchFilter filter = SearchFilterHelper.Current.SearchConfig.SearchFilters.SingleOrDefault(f => f.field == "Year");
					SimpleValue yearValue = filter.Values.SimpleValue.SingleOrDefault(v => v.key == value);
					this.Criteria.Add("Year", yearValue);
				}
			}
		}
		
		/// <summary>
		/// Sets the type for search
		/// </summary>
		public string Type {
			set {
				if (!string.IsNullOrEmpty(value)) {
					SearchFilter filter = SearchFilterHelper.Current.SearchConfig.SearchFilters.SingleOrDefault(f => f.field == "TypeGroup");
					SimpleValue typeValue = filter.Values.SimpleValue.SingleOrDefault(v=>v.key==value);
					this.Criteria.Add("TypeGroup", typeValue);
				}
			}
		}
		
		/// <summary>
		/// Sets the publisher for the search
		/// </summary>
		public string Publisher {
			set {
				if (!string.IsNullOrEmpty(value)) {
					SimpleValue publisherValue = new SimpleValue() {
						key = string.Empty,
						value = value.ToLower(),
						locale = Locale,
						Descriptions = new Descriptions() {
							defaultLocale = Locale
						}
					};

					publisherValue.Descriptions.Description = new Description[] { 
						new Description(){
							Value = value.ToLower(),
							locale = Locale
						} 
					};
					this.Criteria.Add("Publisher", publisherValue);
				}
			}
		}
		
		public int PageIndex {
			get { return this._pageIndex; }
			set { this._pageIndex = value; }
		}

		public int PageSize {
			get { return this._pageSize; }
			set { this._pageSize = value; }
		}

		public string Locale = "en-us";
		

		#endregion

		#region Methods

		/// <summary>
		/// The selected filter for the search. 
		/// </summary>
		/// <param name="Field"></param>
		/// <param name="Key"></param>
		/// <returns></returns>
		public SelectedFilter GetSelectedFilter(string Field, string Key) {
			SelectedFilter selectedFilter = null;

			SearchFilter[] configuredFilters = SearchFilterHelper.Current.SearchConfig.SearchFilters;
			SearchFilter configuredFilter = configuredFilters.SingleOrDefault(filter => filter.field == Field);
			if (configuredFilter != null) {
				SimpleValue value = configuredFilter.Values.SimpleValue.SingleOrDefault(simpleValue => simpleValue.key == Key);
				if (value != null) {
					selectedFilter = new SelectedFilter(configuredFilter, value);
				}
			}
			return selectedFilter;
		}


		/// <summary>
		/// Conduncts the actual search, incorporating the State Availablity Flag
		/// </summary>
		protected void Search() {

			//we have to add one additional filter for State availablity flag

			this.Criteria.Add(
				global::NWTD.Catalog.UserStateAvailablityField, 
				new SimpleValue() {
					key = string.Empty,
					value = "y",
					locale = Locale,
					Descriptions = new Descriptions() {
						defaultLocale = Locale
					}
			});
			
			Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(1024);
			this.Results= SearchFilterHelper.Current.SearchEntries(this.Criteria);
		}

		/// <summary>
		/// Using the facets returned with the search results, we can bind some filter dropdowns to the Grade, Type, and Year facets
		/// We must first make sure those facets exist.
		/// </summary>
		protected void BindFilters() {
			
			FacetGroup gradeFacet = this.Results.FacetGroups.SingleOrDefault(facet => facet.FieldName == "Grade");
			if (gradeFacet != null) {
				this.ddlGradeFilter.DataSource = gradeFacet.Facets;
				this.ddlGradeFilter.DataBind();
			}
			this.ddlGradeFilter.Items.Insert(0, new ListItem("All Grades"));


			FacetGroup typeFacet = this.Results.FacetGroups.SingleOrDefault(facet => facet.FieldName == "TypeGroup");
			if(typeFacet != null){
				this.ddlTypeFilter.DataSource = typeFacet.Facets;
				this.ddlTypeFilter.DataBind();
			}

			this.ddlTypeFilter.Items.Insert(0, new ListItem("All Types"));

			FacetGroup yearFacet = this.Results.FacetGroups.SingleOrDefault(facet => facet.FieldName == "Year");
			if(yearFacet != null){
				this.ddlYearFilter.DataSource = yearFacet.Facets;
				this.ddlYearFilter.DataBind();
			}
			this.ddlYearFilter.Items.Insert(0, new ListItem("All Years"));
		}
		
		/// <summary>
		/// We react to a user selecting some filters from the dropdowns
		/// </summary>
		protected void AddFiltersToSearch() {
			if (this.ddlGradeFilter.SelectedIndex > 0) this.Grade = this.ddlGradeFilter.SelectedValue;
			if (this.ddlTypeFilter.SelectedIndex > 0) this.Type = this.ddlTypeFilter.SelectedValue;
			if (this.ddlYearFilter.SelectedIndex > 0) this.Year = this.ddlYearFilter.SelectedValue;
		}
		
		/// <summary>
		/// Bind our subsearch results to the gridview on the page
		/// </summary>
		protected void BindResults(){
			int[] resultIndexes ;
			if (this.AllowPaging)
				resultIndexes = this.Results.GetIntResults((this.PageIndex * this.srBookSearch.ResultsGrid.PageSize), this.srBookSearch.ResultsGrid.PageSize);
			else {
				resultIndexes = this.Results.GetIntResults(0, this.Results.TotalCount);
			}
			//Entries entries = CatalogContext.Current.GetCatalogEntries(resultIndexes, false, new TimeSpan(0,0,1), new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));


			if (resultIndexes.Length > 0) {

				Entries entries = CatalogContext.Current.GetCatalogEntries(resultIndexes);

				//process the list of entries so that the current entry always shows up at the top
				//Entry currentEntry = CatalogContext.Current.GetCatalogEntry(this.SelectedEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
				//List<Entry> entryList = entries.Entry.ToList<Entry>();
				//entryList.Insert(0, currentEntry);
				//entries.Entry = entryList.ToArray();


				this.srBookSearch.TotalResults = this.Results.TotalCount;
				this.srBookSearch.Entries = entries;
				this.srBookSearch.DataBind();
			}
		}
		
		protected void ResetPager() {
			this.srBookSearch.ResultsGrid.PageIndex = 0;
			this.PageIndex = 0;
		}

		public override void LoadContext(IDictionary context) {
			//base.LoadContext(context);
			this._parameters = context;
			if (!String.IsNullOrEmpty(Request.QueryString["search"]))
				_parameters["FTSPhrase"] = Request.QueryString["search"];
		}

		#endregion

		#region Private Variables

		private int _pageSize = 200;
		private CatalogEntrySearchCriteria _criteria;
		private int _pageIndex = 0;
		private IDictionary _parameters;
		
		#endregion

	}
}