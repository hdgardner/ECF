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
using Mediachase.Cms.WebUtility;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Storage;

namespace Mediachase.Cms.Templates.Default
{
    public partial class SearchModule : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(Object sender, EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            SearchFilter.Items.Clear();
            SearchFilter.Items.Add(new ListItem(RM.GetString("GENERAL_ALL_PRODUCTS"), ""));

            string cacheKey = CatalogCache.CreateCacheKey("mc-catalogentry-list");

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);


            MetaClassCollection metaClasses = null;

            MetaClass catalogEntry = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, "CatalogEntry");
            if (catalogEntry != null)
            {
                metaClasses = catalogEntry.ChildClasses;
            }

            if (metaClasses != null)
            {
                foreach (MetaClass metaClass in metaClasses)
                {
                    SearchFilter.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Name));
                }
            }

            SearchFilter.DataBind();
            Search.Text = Request.QueryString["search"];
            CommonHelper.SelectListItem(SearchFilter, Request.QueryString["filter"]);
        }

        /// <summary>
        /// Called when [search].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void OnSearch(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //Response.Redirect(ClientHelper.FormatUrl("search", false, Server.UrlEncode(SearchFilter.SelectedValue), Server.UrlEncode(Search.Text)));
            Response.Redirect(ResolveUrl(String.Format("~/catalog/searchresults.aspx?filter={0}&search={1}", Server.UrlEncode(SearchFilter.SelectedValue), Server.UrlEncode(Search.Text))));
        }
    }
}