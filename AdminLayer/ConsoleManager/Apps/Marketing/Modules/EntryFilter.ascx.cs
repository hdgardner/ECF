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
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;

public partial class Apps_Marketing_Modules_EntryFilter : System.Web.UI.UserControl
{
    /// <summary>
    /// Gets or sets the selected entry code.
    /// </summary>
    /// <value>The selected entry code.</value>
    public string SelectedEntryCode
    {
        get
        {
            return EntriesFilter.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            BindValue(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is field required.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is field required; otherwise, <c>false</c>.
    /// </value>
    public bool IsFieldRequired
    {
        get
        {
            return RequiredValidator.Enabled;
        }
        set
        {
            RequiredValidator.Enabled = value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!EntriesFilter.CausedCallback && !ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack && !this.IsPostBack)
        {
            EnsureChildControls();
        }
            //LoadEntries(0, EntriesFilter.DropDownPageSize * 2, "");
    }

    protected override void CreateChildControls()
    {
        if (!this.ChildControlsCreated)
        {
            if (!EntriesFilter.CausedCallback && !ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack && !this.IsPostBack)
                LoadEntries(0, EntriesFilter.DropDownPageSize * 2, "");

            base.CreateChildControls();
            ChildControlsCreated = true;
        }
    }

    /// <summary>
    /// Binds the value.
    /// </summary>
    /// <param name="entryCode">The entry code.</param>
    private void BindValue(string entryCode)
    {
        if (String.IsNullOrEmpty(entryCode))
            return;

        ComboBoxItem item = EntriesFilter.Items.FindByValue(entryCode);
        if (item != null)
        {
            EntriesFilter.SelectedItem = item;
        }
        else
        {
            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(entryCode);
            if (entryDto.CatalogEntry.Count > 0)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Text = entryDto.CatalogEntry[0].Name;
                newItem.Value = entryDto.CatalogEntry[0].Code;
                EntriesFilter.Items.Add(newItem);
                EntriesFilter.SelectedItem = newItem;
            }
        }
    }

    /// <summary>
    /// Loads the entries.
    /// </summary>
    /// <param name="iStartIndex">Start index of the i.</param>
    /// <param name="iNumItems">The i num items.</param>
    /// <param name="sFilter">The s filter.</param>
    private void LoadEntries(int iStartIndex, int iNumItems, string sFilter)
    {
        EntriesFilter.Items.Clear();

        // Changed to return all entries here
        string entryType = String.Empty; //Mediachase.Commerce.Catalog.Objects.EntryType.Product.ToString();

        CatalogSearchParameters pars = new CatalogSearchParameters();
        CatalogSearchOptions options = new CatalogSearchOptions();

        options.RecordsToRetrieve = iNumItems;
        options.Namespace = "Mediachase.Commerce.Catalog";
        options.StartingRecord = iStartIndex;
        options.ReturnTotalCount = true;
        pars.SqlWhereClause = String.Format("[CatalogEntry].Name like '%{0}%' OR [CatalogEntry].Code like '%{0}%'", sFilter);

        // Add catalogs
        CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();

        foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
        {
            pars.CatalogNames.Add(catalogRow.Name);
        }

        int totalRecords = 0;
        CatalogEntryDto dto = CatalogContext.Current.FindItemsDto(pars, options, ref totalRecords);
        foreach (CatalogEntryDto.CatalogEntryRow entryRow in dto.CatalogEntry)
        {
            ComboBoxItem item = new ComboBoxItem(entryRow.Name + " [" + entryRow.Code.ToString() + "]");
            item.Value = entryRow.Code.ToString();
            item["icon"] = Page.ResolveClientUrl(String.Format("~/app_themes/Default/images/icons/{0}.gif", entryRow.ClassTypeId));
            EntriesFilter.Items.Add(item);
        }

        EntriesFilter.ItemCount = totalRecords;
    }


    /// <summary>
    /// Handles the DataRequested event of the EntriesFilter control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
    void EntriesFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
    {
        LoadEntries(args.StartIndex, args.NumItems, args.Filter);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        EntriesFilter.DataRequested += new ComboBox.DataRequestedEventHandler(EntriesFilter_DataRequested);
        base.OnInit(e);
    }
}
