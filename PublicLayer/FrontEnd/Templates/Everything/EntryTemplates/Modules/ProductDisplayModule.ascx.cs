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

using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Orders;

public partial class Templates_Everything_Entry_Modules_ProductDisplayModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    private Entry _Entry;
    private Images _LogoImages;

    #region Properties
    /// <summary>
    /// Gets or sets the Entry object to inspect.
    /// </summary>
    /// <value>The Entry object.</value>
    public Entry Entry
    {
        get
        {
            return _Entry;
        }
        set
        {
            _Entry = value;
        }
    }

    /// <summary>
    /// Gets the logo images.
    /// </summary>
    /// <value>The logo images.</value>
    public Images LogoImages
    {
        get
        {
            string brandName = Entry.ItemAttributes["Brand"].ToString();
            CatalogNode brandNode = CatalogContext.Current.GetCatalogNode(brandName);
            if (brandNode != null)
            {
                // With the current API, we're not allowed to create an empty Images collection to pass.
                // TODO: Modify caller to show a default image from default url if we have an empty Images collection to pass.
                if (brandNode.ItemAttributes != null)
                {
                    return brandNode.ItemAttributes.Images; // Mediachase.Commerce.Catalog.Objects.Image[]
                }
                else
                {
                    return new Mediachase.Commerce.Catalog.Objects.Images();
                }
            }
            else
            {
                return new Mediachase.Commerce.Catalog.Objects.Images();
            }
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
    }
}