<%@ Control Language="C#" ClassName="NodeInfo" AutoEventWireup="true" Inherits="Mediachase.Cms.WebUtility.BaseStoreUserControl, Mediachase.Cms.WebUtility" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceLineModule.ascx"
    TagName="PriceLineModule" TagPrefix="catalog" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/BuyButtonModule.ascx"
    TagName="BuyButtonModule" TagPrefix="cart" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Dto" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Search" %>
<%@ Import Namespace="Mediachase.Cms" %>

<script runat="server">
    int _MaximumRows = 20;
    int _StartRowIndex = 0;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.IsPostBack)
        {
            BindFields();
            if (Request.QueryString["_view"] != null && Request.QueryString["_view"] == "all")
            {
                DataPager2.PageSize = 1000;
                DataPager3.PageSize = 1000;
            }
            DataBind();
        }
    }

    private void BindFields()
    {
        CatalogSearchDataSource.Parameters.Language = Mediachase.Cms.CMSContext.Current.LanguageName;

        //CatalogSearchDataSource.Options.Classes.Add("Account");

        if (_Parameters.Contains("Catalogs"))
        {
            foreach (string catalog in _Parameters["Catalogs"].ToString().Split(new char[',']))
            {
                CatalogSearchDataSource.Parameters.CatalogNames.Add(catalog);
            }
        }

        if (_Parameters.Contains("NodeCode"))
        {
            foreach (string node in _Parameters["NodeCode"].ToString().Split(new char[',']))
            {
                CatalogSearchDataSource.Parameters.CatalogNodes.Add(node);
            }
        }

        if (_Parameters.Contains("EntryClasses"))
        {
            foreach (string node in _Parameters["EntryClasses"].ToString().Split(new char[',']))
            {
                CatalogSearchDataSource.Options.Classes.Add(node);
            }
        }

        if (_Parameters.Contains("EntryTypes"))
        {
            foreach (string entry in _Parameters["EntryTypes"].ToString().Split(new char[',']))
            {
                if (String.IsNullOrEmpty(CatalogSearchDataSource.Parameters.SqlWhereClause))
                    CatalogSearchDataSource.Parameters.SqlWhereClause = String.Format("ClassTypeId='{0}'", entry);
                else
                    CatalogSearchDataSource.Parameters.SqlWhereClause = CatalogSearchDataSource.Parameters.SqlWhereClause + String.Format(" AND ClassTypeId='{0}'", entry);
            }
        }

        if (_Parameters.Contains("RecordsPerPage"))
        {
            CatalogSearchDataSource.Options.RecordsToRetrieve = Int32.Parse(_Parameters["RecordsPerPage"].ToString());
        }

        if (_Parameters.Contains("FTSPhrase"))
        {
            CatalogSearchDataSource.Parameters.FreeTextSearchPhrase = _Parameters["FTSPhrase"].ToString();
        }

        if (_Parameters.Contains("AdvancedFTSPhrase"))
        {
            CatalogSearchDataSource.Parameters.AdvancedFreeTextSearchPhrase = _Parameters["AdvancedFTSPhrase"].ToString();
        }

        if (_Parameters.Contains("MetaSQLClause"))
        {
            CatalogSearchDataSource.Parameters.SqlMetaWhereClause = _Parameters["MetaSQLClause"].ToString();
        }

        if (_Parameters.Contains("SQLClause"))
        {
            if (String.IsNullOrEmpty(CatalogSearchDataSource.Parameters.SqlWhereClause))
                CatalogSearchDataSource.Parameters.SqlWhereClause = _Parameters["SQLClause"].ToString();
            else
                CatalogSearchDataSource.Parameters.SqlWhereClause = CatalogSearchDataSource.Parameters.SqlWhereClause + String.Format(" AND {0}", _Parameters["SQLClause"].ToString());
        }
    }

    void EntriesList2_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        _MaximumRows = e.MaximumRows;
        _StartRowIndex = e.StartRowIndex;
    }

    protected string GetViewAllUrl(int totalRecords)
    {
        string q = CMSContext.Current.CurrentUrl.Contains("?") ? "&" : "?";
        return CMSContext.Current.CurrentUrl + q + "_view" + "=all&_max=" + totalRecords.ToString();
    }

    void SortBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        string val = ((DropDownList)sender).SelectedValue;
        Response.Redirect(String.Format("{0}?s={1}", CMSContext.Current.CurrentUrl, val));
    }

    void SortBy_PreRender(object sender, EventArgs e)
    {
        if (Request.QueryString["s"] != null)
        {
            Mediachase.Cms.Util.CommonHelper.SelectListItem((DropDownList)sender, Request.QueryString["s"]);
        }
    }

    void EntriesList2_PagePropertiesChanged(object sender, EventArgs e)
    {
        BindFields();
        DataBind();
    }


    private IDictionary _Parameters;

    public override void LoadContext(IDictionary context)
    {
        _Parameters = context;

        if (!String.IsNullOrEmpty(Request.QueryString["search"]))
            _Parameters["FTSPhrase"] = Request.QueryString["search"];
    }

    protected Price GetDiscountPrice(Entry entry)
    {
        return StoreHelper.GetDiscountPrice(entry, String.Empty/*CatalogName*/);
    }    
</script>

<h1>
    <%=RM.GetString("PRODUCT_SEARCH_RESULTS_LABEL")%></h1>
<catalog:CatalogSearchDataSource runat="server" ID="CatalogSearchDataSource">
    <ResponseGroup ResponseGroups="CatalogEntryInfo" />
</catalog:CatalogSearchDataSource>
<!-- add product pager -->
<div id="search-results">
    <div class="list-paging">
        <asp:DataPager QueryStringField="p" ID="DataPager2" PageSize="20" runat="server"
            PagedControlID="EntriesList2">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        <div class="sortby">
                            Sort By:
                            <asp:DropDownList OnPreRender="SortBy_PreRender" runat="server" ID="SortBy" AutoPostBack="true"
                                OnSelectedIndexChanged="SortBy_SelectedIndexChanged">
                                <asp:ListItem Text="Featured Products" Value="featured"></asp:ListItem>
                                <asp:ListItem Text="Product Name" Value="name"></asp:ListItem>
                                <asp:ListItem Text="Price Low to High" Value="plh"></asp:ListItem>
                                <asp:ListItem Text="Price High to Low" Value="phl"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="<%#GetViewAllUrl(Container.TotalRowCount)%>">View All</asp:HyperLink>
                        (<asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.TotalRowCount%>" />)
                        | Page
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <cms:CmsNumericPagerField NextPageImageUrl="~/Images/button_arrow_right.gif" PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
            </Fields>
        </asp:DataPager>
    </div>
    <div id="list-content">
        <asp:ListView ID="EntriesList2" EnableViewState="false" DataSourceID="CatalogSearchDataSource" OnPagePropertiesChanging="EntriesList2_PagePropertiesChanging"
            OnPagePropertiesChanged="EntriesList2_PagePropertiesChanged" runat="server" ItemPlaceholderID="itemPlaceHolder">
            <EmptyDataTemplate><p>Sorry, we couldn't find any matches for '<%= Request.QueryString["search"]%>'.</p>
<p>
Please check your spelling or search for a different key word.
</p>

</EmptyDataTemplate>
            <LayoutTemplate>
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </LayoutTemplate>
            <ItemTemplate>
                <ul class="list-item">
                    <li class="list-item-image">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'>
                            <cms:MetaImage ID="CatalogImage" Width="120" PropertyName="PrimaryImage" DataSource='<%#Eval("ItemAttributes.Images")%>'
                                runat="server" /></asp:HyperLink>
                    </li>
                    <li class="list-item-info">
                        <div class="brand-name">
                            <%# ((ItemAttributes)Eval("ItemAttributes"))["brand"]%></div>
                        <div class="entry-name">
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><%# StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%></asp:HyperLink></div>
                    </li>
                    <li class="list-item-price">
                        <catalog:PriceLineModule ShowPriceLabel="false" Visible="true" ID="PriceLineModule1"
                            ListPrice='<%# StoreHelper.GetSalePrice((Entry)Container.DataItem, 1)%>' SalePrice='<%# GetDiscountPrice((Entry)Container.DataItem)%>'
                            runat="server"></catalog:PriceLineModule>
                    </li>
                </ul>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <div class="list-paging">
        <asp:DataPager QueryStringField="p" ID="DataPager3" PageSize="20" runat="server"
            PagedControlID="EntriesList2">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="<%#GetViewAllUrl(Container.TotalRowCount)%>">View All</asp:HyperLink>
                        (<asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.TotalRowCount%>" />)
                        | Page
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <cms:CmsNumericPagerField NextPageImageUrl="~/Images/button_arrow_right.gif" PreviousPageImageUrl="~/Images/button_arrow_left.gif" />
            </Fields>
        </asp:DataPager>
    </div>
</div>
