using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Mediachase.Commerce.Catalog.Objects;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Commerce.Catalog;
using Mediachase.Cms.WebUtility.Commerce;

public partial class Templates_Everything_Entry_Modules_RecentlyViewedModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        NameValueCollection history = StoreHelper.GetBrowseHistory();
        //StringCollection history = (StringCollection)Profile["EntryHistory"];

        List<Entry> entries = new List<Entry>();
        string[] values = history.GetValues("Entries");

        if (values != null)
        {
            bool isFirst = true;
            foreach (string key in values)
            {
                // Is it a first item? skip
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                if (!String.IsNullOrEmpty(key))
                {
                    Entry entry = CatalogContext.Current.GetCatalogEntry(key);
                    if (entry != null)
                        entries.Add(entry);
                }
            }
        }

        // Remove last history element, since it will be currently displayed item
        //if (entries.Count > 0)
        //    entries.RemoveAt(entries.Count - 1);

        if (entries.Count == 0)
        {
            this.Visible = false;
            return;
        }

        RecentlyViewedList.DataSource = entries.ToArray();
        RecentlyViewedList.DataBind();
    }
}