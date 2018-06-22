<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchControl.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.Everything.Modules.SearchControl" %>
<%@ Register Src="../EntryTemplates/Modules/BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cms" %>
<%@ Register Src="~/Structure/Base/Controls/Catalog/SharedModules/PriceLineModule.ascx"
    TagName="PriceLineModule" TagPrefix="catalog" %>
<%@ Register Src="SideMenuControl.ascx" TagPrefix="cms" TagName="SideMenu" %>    
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<div id="sidemenu">
    <cms:SideMenu runat="server" ID="MyMenu" />
</div>
<catalog:CatalogIndexSearchDataSource runat="server" ID="CatalogSearchDataSource"></catalog:CatalogIndexSearchDataSource>
<!-- add product pager -->
<div id="search-results">
    <div class="list-paging" runat="server" id="PagingHeader">
        <asp:DataPager QueryStringField="p" ID="DataPager2" PageSize="20" runat="server"
            PagedControlID="EntriesList2">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        
                        <div class="sortby">
                            Sort By:
                            <asp:DropDownList runat="server" ID="SortBy" AutoPostBack="true"
                                OnSelectedIndexChanged="SortBy_SelectedIndexChanged" OnLoad="SortBy_Load">
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
    <div class="list-content">
        <asp:ListView ID="EntriesList2" GroupItemCount="4" EnableViewState="false" DataSourceID="CatalogSearchDataSource" OnPagePropertiesChanging="EntriesList2_PagePropertiesChanging"
            OnPagePropertiesChanged="EntriesList2_PagePropertiesChanged" runat="server" ItemPlaceholderID="itemPlaceHolder">
            <EmptyDataTemplate>
                <div class="list-empty">
                    <div class="list-empty-suggestion">
                    <p>Sorry, no matching results for '<%= Request.QueryString["search"]%>'. 
                    <%=GetSuggestionUrl()%></div></p>
                    <p>Please check your spelling or search for a different keyword.</p>
                </div>
            </EmptyDataTemplate>
            <LayoutTemplate>                
                <asp:PlaceHolder runat="server" ID="groupPlaceholder"></asp:PlaceHolder>
            </LayoutTemplate>
            <GroupTemplate>
                <div class="list-content-group">
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                </div>
            </GroupTemplate>
            <GroupSeparatorTemplate>
                <div class="clearfix"></div>
            </GroupSeparatorTemplate>
            <ItemSeparatorTemplate>
                <div class="list-item-sep">&nbsp;</div>
            </ItemSeparatorTemplate>
            <ItemTemplate>
                <ul class="list-item">
                    <li class="list-item-image">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'>
                            <cms:MetaImage ID="CatalogImage" PropertyName="PrimaryImage" DataSource='<%#Eval("ItemAttributes.Images")%>'
                                runat="server" /></asp:HyperLink>
                    </li>
                    <li class="list-item-price" style="margin-top:4px;">
                        <catalog:PriceLineModule ShowPriceLabel="false" Visible="true" ID="PriceLineModule1"
                            ListPrice='<%# StoreHelper.GetSalePrice((Entry)Container.DataItem, 1)%>' SalePrice='<%# GetDiscountPrice((Entry)Container.DataItem)%>'
                            runat="server"></catalog:PriceLineModule>
                            
                    </li>
                    <li class="list-item-info">
                        <div class="brand-name">
                            <%# ((ItemAttributes)Eval("ItemAttributes"))["brand"]%></div>
                        <div class="entry-name">
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><%# StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%></asp:HyperLink></div>
                    </li>
                    <li class="list-item-buy"><cms:BuyButtonModule runat="server" ID="BuyButton1" Entry='<%# (Entry)Container.DataItem%>' OnClick="BuyButton_Click" />  </li>
                </ul>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <div class="list-paging" runat="server" id="PagingFooter">
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
