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
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Orders;

public partial class Structure_Base_Controls_Catalog_SharedModules_AssociationsListModule : System.Web.UI.UserControl
{
    Association _Association;
    /// <summary>
    /// Gets or sets the association.
    /// </summary>
    /// <value>The association.</value>
    public Association Association
    {
        get
        {
            return _Association;
        }
        set
        {
            _Association = value;
        }
    }

    string _CatalogName;
    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    /// <value>The name of the catalog.</value>
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
        if (Association != null && Association.EntryAssociations != null)
        {
            this.Visible = true;
            AssociationHeader.Text = Association.Description;
            EntryList.DataSource = Association.EntryAssociations.Association;

            EntryList.DataBind();
        }
        else
        {
            this.Visible = false;
        }
    }
}
