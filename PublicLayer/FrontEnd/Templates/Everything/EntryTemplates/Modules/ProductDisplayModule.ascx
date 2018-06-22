<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDisplayModule.ascx.cs"
    Inherits="Templates_Everything_Entry_Modules_ProductDisplayModule" EnableViewState="false" %>
<%@ Register Src="ImagesModule.ascx" TagName="ImagesModule" TagPrefix="cms" %>    
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<div id="entry-product-display">
    <div class="logo">
        <!-- logo -->
        <cms:MetaImage OpenFullImage="true" Width="162px" AlternateText='<%#StoreHelper.GetEntryDisplayName(Entry) %>'
            ShowThumbImage="false" ID="BrandLogo" PropertyName="PrimaryImage" DataSource="<%#this.LogoImages %>"
            runat="server" />
    </div>
    <div class="image">
        <!-- product image-->
        <cms:MetaImage OpenFullImage="true" AlternateText='<%#StoreHelper.GetEntryDisplayName(Entry) %>'
            ShowThumbImage="false" ID="PrimaryImage" PropertyName="PrimaryImage" DataSource="<%#Entry.ItemAttributes.Images%>"
            runat="server" />
    </div>
    <br class="clearfloat" />
    <!--
    <div class="rowWrap">
        <div class="zoom">
            <asp:Image ID="ZoomInImage" runat="server" SkinID="ZoomInImage" ImageId="ZoomInImage" />
        </div>
        <div class="item-zoom">
            Zoom In
        </div>
        <br class="clearfloat" />
        <br class="clearfloat" />
    </div>
    -->
    <div class="rowWrap">
        <!-- thumbnails -->
        <cms:ImagesModule GroupName="image" DataSource="<%#Entry.Assets%>" runat="server"></cms:ImagesModule>
        <br class="clearfloat" />
        <br class="clearfloat" />
    </div>
</div>
