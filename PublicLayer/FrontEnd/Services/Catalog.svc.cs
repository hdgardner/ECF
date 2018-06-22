using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Collections.Generic;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Search.Extensions;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Search;
using Mediachase.Commerce.Catalog.Managers;



namespace Mediachase.Cms.Website.Services {
	[ServiceContract(Namespace = "NWTD")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class Catalog {

		[OperationContract]
		[WebGet]
		public string Test() {
			return "test";
		}


		[OperationContract]
		[WebGet]
		public List<Entry> BookSearch(BookFilter BookFilter) {


			int count = 0;
			bool cacheResults = true;
			
			TimeSpan cacheTimeout = new TimeSpan(0, 0, 30);
			if (String.IsNullOrEmpty(BookFilter.search))
				cacheTimeout = new TimeSpan(0, 1, 0);

			SearchFilterHelper filter = SearchFilterHelper.Current;

			CatalogEntrySearchCriteria criteria = filter.CreateSearchCriteria(BookFilter.search, new SearchSort(BookFilter.sort));


			if (criteria.CatalogNames.Count == 0) {
				CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
				if (catalogs.Catalog.Count > 0) {
					foreach (CatalogDto.CatalogRow row in catalogs.Catalog) {
						if (row.IsActive && row.StartDate <= FrameworkContext.Current.CurrentDateTime && row.EndDate >= FrameworkContext.Current.CurrentDateTime)
							criteria.CatalogNames.Add(row.Name);
					}
				}
			}


			Entries entries = filter.SearchEntries(criteria, BookFilter.startIndex, BookFilter.itemsPerPage, out count, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo), cacheResults, cacheTimeout);

			int resultsCount = entries.Entry != null ? entries.Entry.Count() : 0;

			if (entries.Entry != null && entries.Entry.Count() > BookFilter.itemsPerPage) { //ECF's search helper pads the results by 5
				Entry[] entryset = entries.Entry;
				Array.Resize<Entry>(ref entryset, BookFilter.itemsPerPage);
				entries.Entry = entryset;
			}

			return new List<Entry> (entries.Entry);
		}

		public class BookFilter {
			public string year { get; set; }
			public string grade { get; set; }
			public string subject { get; set; }
			public string type { get; set; }
			public string publisher { get; set; }
			public string search { get; set; }
			public int itemsPerPage { get; set; }
			public int startIndex { get; set; }
			public string sort { get; set; }
		}

		// Add more operations here and mark them with [OperationContract]
	}
}
