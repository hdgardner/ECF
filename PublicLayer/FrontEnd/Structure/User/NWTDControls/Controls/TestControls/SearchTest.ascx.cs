using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility.Search;
using Mediachase.Search.Extensions;
using Mediachase.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Lucene.Net;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using System.Data;
using Lucene.Net.Documents;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.TestControls{
    public partial class SearchTest : System.Web.UI.UserControl{



        protected void BindQuery() {
            SqlDataSource querySource = new SqlDataSource();
            querySource.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EcfSqlConnection"].ConnectionString;
            querySource.SelectCommand = @"
                SELECT TOP 10 entry.*, book.TypeSort FROM CatalogEntry entry 
                LEFT JOIN CatalogEntryEx_Book book ON entry.CatalogEntryId = book.ObjectId
                ORDER BY book.TypeSort";

            this.gvQueryResults.DataSource = querySource;
            this.gvQueryResults.DataBind();
        }

        protected void BindLucene() {
            StandardAnalyzer analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer();
            IndexSearcher searcher = new IndexSearcher("C:\\Clients\\NWTD5.0.200\\SearchIndex\\eCommerceFramework\\CatalogEntryIndexer");
            BooleanQuery boolQuery = new BooleanQuery();

            boolQuery.Add(new TermQuery(new Term("_catalog", "NWTD")), BooleanClause.Occur.MUST);

            Hits hits = searcher.Search(boolQuery, new Sort("TypeSort"));


            DataTable dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Columns.Add("Name");
            dt.Columns.Add("TypeSort");

           for (int hitIndex = 0; hitIndex < hits.Length() && hitIndex < 10; hitIndex++) {
                Document hitDocument = hits.Doc(hitIndex);
                object[] param = new[] { 
				hitDocument.GetField("Code").StringValue(), 
				hitDocument.GetField("Name").StringValue(), 
				hitDocument.GetField("TypeSort").StringValue()};
                dt.Rows.Add(param);
            }

            this.gvLuceneSearchResults.DataSource = dt;
            this.gvLuceneSearchResults.DataBind();
        }

        protected void BindSearch()
        {
            int count = 0;
            bool cacheResults = false;
            TimeSpan cacheTimeout = new TimeSpan(0, 0, 1);
            SearchSort sort = new SearchSort("TypeSort");

            SearchFilterHelper filter = SearchFilterHelper.Current;

            CatalogEntrySearchCriteria criteria = filter.CreateSearchCriteria(null, sort);
            criteria.CatalogNames.Add("NWTD");

            Entries entries = filter.SearchEntries(criteria, 0, 5, //ECF's API pads the results
                out count,
                new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo),
                cacheResults, cacheTimeout
            );

            this.gvSearchResults.DataSource = entries.Entry;
            this.gvSearchResults.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e){
            this.BindSearch();
            this.BindQuery();
            this.BindLucene();
        }
    }
}