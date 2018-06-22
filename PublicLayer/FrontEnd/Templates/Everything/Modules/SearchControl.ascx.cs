using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Cms.Util;
using System.Collections;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility;
using Mediachase.Search;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Search.Extensions;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce;

namespace Mediachase.Cms.Website.Templates.Everything.Modules
{
    public partial class SearchControl : BaseStoreUserControl
    {
        int _MaximumRows = 20;
        int _StartRowIndex = 0;
        SearchResults _Results = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack || ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
            {
                if (Request.QueryString["_view"] != null && Request.QueryString["_view"] == "all")
                {
                    DataPager2.PageSize = 1000;
                    DataPager3.PageSize = 1000;
                }

                _MaximumRows = DataPager2.MaximumRows;
                _StartRowIndex = DataPager2.StartRowIndex;

                BindFields();
                DataBind();
            }
        }

        /// <summary>
        /// Binds the fields.
        /// </summary>
        private void BindFields()
        {
            int recordsToRetrieve = DataPager2.PageSize;

            // Perform search
            string keywords = Request.QueryString["search"];
            int count = 0;
            bool cacheResults = true;
            TimeSpan cacheTimeout = new TimeSpan(0, 0, 30);
            if (String.IsNullOrEmpty(keywords))
                cacheTimeout = new TimeSpan(0, 1, 0);

            SearchFilterHelper filter = SearchFilterHelper.Current;

            string sort = Request.QueryString["s"];

            SearchSort sortObject = null;
            if (!String.IsNullOrEmpty(sort))
            {
                if (sort.Equals("name", StringComparison.OrdinalIgnoreCase))
                    sortObject = new SearchSort("DisplayName");
                else if (sort.Equals("plh", StringComparison.OrdinalIgnoreCase))
                    sortObject = new SearchSort(String.Format("SalePrice{0}", CMSContext.Current.CurrencyCode));
                else if (sort.Equals("phl", StringComparison.OrdinalIgnoreCase))
                    sortObject = new SearchSort(String.Format("SalePrice{0}", CMSContext.Current.CurrencyCode), true);
            }

            // Put default sort order if none is set
            if(sortObject == null)
                sortObject = CatalogEntrySearchCriteria.DefaultSortOrder;

            CatalogEntrySearchCriteria criteria = filter.CreateSearchCriteria(keywords, sortObject);

            if (_Parameters.Contains("Catalogs"))
            {
                foreach (string catalog in _Parameters["Catalogs"].ToString().Split(new char[',']))
                {
                    if (!String.IsNullOrEmpty(catalog))
                        criteria.CatalogNames.Add(catalog);
                }
            }

            if (_Parameters.Contains("NodeCode"))
            {
                foreach (string node in _Parameters["NodeCode"].ToString().Split(new char[',']))
                {
                    if (!String.IsNullOrEmpty(node))
                        criteria.CatalogNodes.Add(node);
                }
            }

            if (_Parameters.Contains("EntryClasses"))
            {
                foreach (string node in _Parameters["EntryClasses"].ToString().Split(new char[',']))
                {
                    if (!String.IsNullOrEmpty(node))
                        criteria.SearchIndex.Add(node);
                }
            }

            if (_Parameters.Contains("EntryTypes"))
            {
                foreach (string entry in _Parameters["EntryTypes"].ToString().Split(new char[',']))
                {
                    if (!String.IsNullOrEmpty(entry))
                        criteria.ClassTypes.Add(entry);
                }
            }

            if (_Parameters.Contains("RecordsPerPage"))
            {
                recordsToRetrieve = Int32.Parse(_Parameters["RecordsPerPage"].ToString());
            }

            // Bind default catalogs if none found
            if (criteria.CatalogNames.Count == 0)
            {
                CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
                if (catalogs.Catalog.Count > 0)
                {
                    foreach (CatalogDto.CatalogRow row in catalogs.Catalog)
                    {
                        if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
                            criteria.CatalogNames.Add(row.Name);
                    }
                }
            }

            // No need to perform search if no catalogs specified
            if (criteria.CatalogNames.Count != 0)
            {
                Entries entries = filter.SearchEntries(criteria, _StartRowIndex, recordsToRetrieve, out count, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo), cacheResults, cacheTimeout);
                CatalogSearchDataSource.TotalResults = count;
                CatalogSearchDataSource.CatalogEntries = entries;
            }
            _Results = filter.Results;

            if (count == 0)
            {
                PagingHeader.Visible = false;
                PagingFooter.Visible = false;
                DataPager2.Visible = false;
                DataPager3.Visible = false;
                MyMenu.Visible = false;
            }
            else
            {
                MyMenu.Filters = filter.SelectedFilters;
                MyMenu.Facets = filter.GetFacets(cacheResults, cacheTimeout);
                MyMenu.Visible = true;
                //MyMenu.DataBind();
                PagingHeader.Visible = true;
                PagingFooter.Visible = true;
                DataPager2.Visible = true;
                DataPager3.Visible = true;
            }
        }

        /// <summary>
        /// Gets the suggestion.
        /// </summary>
        /// <returns></returns>
        public string GetSuggestionUrl()
        {
            string[] suggestions = _Results.GetSimilarWords("_content", Request.QueryString["search"]);
            if (suggestions != null && suggestions.Length > 0)
            {
                string resurnUrl = String.Format("Did you mean: <a href='{0}'>{1}</a> ", Mediachase.Cms.WebUtility.Search.SearchFilterHelper.GetQueryStringNavigateUrl("search", suggestions[0]), suggestions[0]);
                if(suggestions.Length > 1)
                    resurnUrl += String.Format("or <a href='{0}'>{1}</a>", Mediachase.Cms.WebUtility.Search.SearchFilterHelper.GetQueryStringNavigateUrl("search", suggestions[1]), suggestions[1]);

                resurnUrl += "?";
                return resurnUrl;
            }

            return String.Empty;
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the EntriesList2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
        protected void EntriesList2_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            _MaximumRows = e.MaximumRows;
            _StartRowIndex = e.StartRowIndex;
        }

        /// <summary>
        /// Gets the view all URL.
        /// </summary>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        protected string GetViewAllUrl(int totalRecords)
        {
            string q = CMSContext.Current.CurrentUrl.Contains("?") ? "&" : "?";
            return CMSContext.Current.CurrentUrl + q + "_view" + "=all&_max=" + totalRecords.ToString();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the SortBy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
			// redirect only if postback was caused by changing sort value
			if (Request.Form["__EVENTTARGET"] == ((DropDownList)sender).UniqueID)
			{
				string val = ((DropDownList)sender).SelectedValue;
				Response.Redirect(SearchFilterHelper.GetQueryStringNavigateUrl("s", val));
			}
        }

		/// <summary>
		/// Handles the PreRender event of the SortBy control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SortBy_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["s"] != null)
				CommonHelper.SelectListItem((DropDownList)sender, Request.QueryString["s"]);
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the EntriesList2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EntriesList2_PagePropertiesChanged(object sender, EventArgs e)
        {
            BindFields();
            DataBind();
        }


        private IDictionary _Parameters;

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void LoadContext(IDictionary context)
        {
            _Parameters = context;

            if (!String.IsNullOrEmpty(Request.QueryString["search"]))
                _Parameters["FTSPhrase"] = Request.QueryString["search"];
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        protected Price GetDiscountPrice(Entry entry)
        {
            return StoreHelper.GetDiscountPrice(entry, String.Empty/*CatalogName*/);
        }    

        /// <summary>
        /// Handles the Click event of the BuyButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void BuyButton_Click(object sender, EventArgs e, Entry entry, ref bool reject)
        {
            CartHelper ch = new CartHelper(Cart.DefaultName);

            // Check if Entry Object is null.
            if (entry != null)
            {
                // Add item to a cart.
                ch.AddEntry(entry);
            }
        }
    }
}