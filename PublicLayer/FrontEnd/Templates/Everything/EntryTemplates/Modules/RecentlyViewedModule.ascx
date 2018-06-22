<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentlyViewedModule.ascx.cs"
    Inherits="Templates_Everything_Entry_Modules_RecentlyViewedModule" EnableViewState="false" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<div id="entry-box">
    <div class="header">
        <div class="text">
            Recently Viewed Products
        </div>
        <br class="clearfloat" />
    </div>
    <br class="clearfloat" />
    <asp:Repeater runat="server" ID="RecentlyViewedList">
        <ItemTemplate>
            <div class="row">
                <div class="image">
                    <!-- product image-->
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Container.DataItem).ItemAttributes.Images%>'
                        Width="47" ShowThumbImage="true" ID="PrimaryImage" PropertyName="PrimaryImage" /></asp:HyperLink>
                </div>
                <div class="desc">
                    <div class="brand-name">
                            <%# ((Entry)Container.DataItem).ItemAttributes["Brand"]%>
                    </div>
                    <div class="link">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Container.DataItem)%>'><%#StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%></asp:HyperLink>
                    </div>
                </div>
                <br class="clearfloat" />
                <br class="clearfloat" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
