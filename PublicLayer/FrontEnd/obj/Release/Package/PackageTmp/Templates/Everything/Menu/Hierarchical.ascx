<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Hierarchical.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.Everything.Menu.Hierarchical" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>  
<ComponentArt:Menu ImagesBaseUrl="~/App_Themes/everything/images/menu/" BaseUrl="/" DefaultItemLookID="DefaultItemLook"
	Orientation="Horizontal" CssClass="mnu-top"
	id="SiteMenu" runat="server" EnableViewState="False"
	ExpandDelay="100" ForceSearchEngineStructure="True"
      DefaultGroupCssClass="mnu-grp"
      DefaultSubGroupExpandOffsetX="-10"
      DefaultSubGroupExpandOffsetY="-5"
      TopGroupItemSpacing="1"
      DefaultGroupItemSpacing="2">
	<ItemLooks>
        <ComponentArt:ItemLook LookID="TopItemLook" CssClass="mnu-top-item" HoverCssClass="mnu-top-item-h" LabelPaddingLeft="15" LabelPaddingRight="15" LabelPaddingTop="4" LabelPaddingBottom="4" />
		<ComponentArt:ItemLook LookID="BreakLook" ImageHeight="20" ImageWidth="1" ImageUrl="middle.gif" />
	    <ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="mnu-item" HoverCssClass="mnu-item-h" ExpandedCssClass="mnu-item-h" LabelPaddingLeft="18" LabelPaddingRight="12" LabelPaddingTop="3" LabelPaddingBottom="4" />
	    <ComponentArt:ItemLook LookID="ChildItemLook" CssClass="mnu-item" HoverCssClass="mnu-item-h" ExpandedCssClass="mnu-item-h" LabelPaddingLeft="18" LabelPaddingRight="12" LabelPaddingTop="3" LabelPaddingBottom="4" />	    
	</ItemLooks>
</ComponentArt:Menu>
<asp:SiteMapDataSource runat="server" ID="MenuDataSource" SiteMapProvider="CmsSiteMapProvider" />
<asp:SiteMapDataSource runat="server" ID="CatalogDataSource" StartingNodeOffset="1" ShowStartingNode="false" SiteMapProvider="CatalogSiteMapProvider" />