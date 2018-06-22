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

public partial class Templates_Everything_Entry_Modules_OverviewModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    private Entry _Entry;
    private string _CatalogName;

    #region Properties
    /// <summary>
    /// Gets or sets the Entry object to inspect.
    /// </summary>
    /// <value>The Entry object.</value>
    public Entry Entry
    {
        set
        {
            _Entry = value;
        }
        get
        {
            return _Entry;
        }
    }
    
    /// <summary>
    /// Gets or sets the CatalogName to pass to our StoreHelper object.
    /// </summary>
    /// <value>The CatalogName string.</value>
    public string CatalogName
    {
        get
        {
            return _CatalogName;
        }
        set
        {
            _CatalogName = value;
        }
    }

    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        DownloadsTab.Visible = false;
        SpecificationsTab.Visible = false;

        if (CountAssetsInGroup(Entry.Assets, "Specifications") > 0 || CountAssetsInGroup(Entry.Assets, "UserGuides") > 0)
            SpecificationsTab.Visible = true;

        if (CountAssetsInGroup(Entry.Assets, "Downloads") > 0)
            DownloadsTab.Visible = true;
    }

    /// <summary>
    /// Counts the assets in group.
    /// </summary>
    /// <param name="assets">The assets.</param>
    /// <param name="groupName">Name of the group.</param>
    /// <returns></returns>
    private int CountAssetsInGroup(ItemAsset[] assets, string groupName)
    {
        int count = 0;
        if (assets == null)
            return 0;

        foreach (ItemAsset asset in assets)
        {
            if (asset.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }
        return count;
    }
}