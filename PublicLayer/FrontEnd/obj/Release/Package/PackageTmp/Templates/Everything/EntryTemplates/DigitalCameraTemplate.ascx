<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DigitalCameraTemplate.ascx.cs"
    Inherits="Mediachase.Cms.Website.Templates.Everything.EntryTemplates.DigitalCameraTemplate"
    EnableViewState="false" %>
<%@ Register Src="Modules/ProductDisplayModule.ascx" TagName="ProductDisplayModule"
    TagPrefix="catalog" %>
<%@ Register Src="Modules/RecentlyViewedModule.ascx" TagName="RecentlyViewedModule"
    TagPrefix="catalog" %>
<%@ Register Src="Modules/CompareProductModule.ascx" TagName="CompareProductModule" TagPrefix="catalog" %>
<%@ Register Src="Modules/BuyModule.ascx" TagName="BuyModule" TagPrefix="catalog" %>
<%@ Register Src="Modules/OverviewModule.ascx" TagName="OverviewModule" TagPrefix="catalog" %>
<%@ Register Src="Modules/YouMayAlsoLikeModule.ascx" TagName="YouMayAlsoLikeModule"
    TagPrefix="catalog" %>
<div id="entry-layout-3columns">
    <br class="clearfloat" />
    <div class="rowWrap">
        <!-- COLUMN01 -->
        <div id="entry-column-01">
            <catalog:ProductDisplayModule ID="ProductDisplayModule" Entry="<%#Entry %>" runat="server">
            </catalog:ProductDisplayModule>
            <catalog:CompareProductModule ID="CompareProductModule1" runat="server"/>
            <catalog:RecentlyViewedModule ID="RecentlyViewedModule" runat="server"></catalog:RecentlyViewedModule>
        </div>
        <!-- COLUMN02 -->
        <div id="entry-column-02">
            <h1>
                <%#StoreHelper.GetEntryDisplayName(Entry)%></h1>
            <catalog:BuyModule ID="BuyModule" RelatedProductsAssociationName="Accessories" AdditionalOptionsAssociationName="AdditionalOptions"
                CatalogName="<%#CatalogName %>" Entry="<%#Entry %>" runat="server"></catalog:BuyModule>
            <catalog:OverviewModule ID="OverViewModule" Entry="<%#Entry %>" runat="server"></catalog:OverviewModule>
        </div>
        <!-- COLUMN03 -->
        <div id="entry-column-03">
            <catalog:YouMayAlsoLikeModule ID="YouMayAlsoLikeModule" YouMayAlsoLikeAssociationName="CrossSell"
                CatalogName="<%#CatalogName %>" Entry="<%#Entry %>" runat="server"></catalog:YouMayAlsoLikeModule>
            <br class="clearfloat" />
        </div>
    </div>
</div>
