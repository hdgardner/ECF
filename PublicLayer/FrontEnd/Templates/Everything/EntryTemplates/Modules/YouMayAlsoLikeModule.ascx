<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YouMayAlsoLikeModule.ascx.cs"
    Inherits="Templates_Everything_Entry_Modules_YouMayAlsoLikeModule" EnableViewState="false" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<asp:Panel ID="YouMayAlsoLikePanel" runat="server">
    <div id="entry-box">
        <div class="header">
            <div class="text">
                You May Also Like...
            </div>
            <br class="clearfloat" />
        </div>
        <br class="clearfloat" />
        <asp:Repeater ID="YouMayAlsoLikeRepeater" runat="server">
            <ItemTemplate>
                <div class="row">
                    <div class="image">
                        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'>
                            <cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Eval("Entry")).ItemAttributes.Images%>'
                                Width="140" ImageAlign=Middle ShowThumbImage="false" ID="MetaImage1" PropertyName="PrimaryImage" /></asp:HyperLink>
                    </div>
                    <div class="desc">
                        <div class="brand-name">
                            <%# ((EntryAssociation)Container.DataItem).Entry.ItemAttributes["Brand"]%>
                        </div>
                        <br class="clearfloat" />
                        <div class="link">
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl(((EntryAssociation)Container.DataItem).Entry)%>'><%#StoreHelper.GetEntryDisplayName(((EntryAssociation)Container.DataItem).Entry)%></asp:HyperLink>
                        </div>
                    </div>
                    <div style="width: 100%">
                        <div class="price">
                            <asp:Label ID="PriceLabel" CssClass="ecf-price" Text='<%# StoreHelper.GetDiscountPrice((Entry)Eval("Entry"), CatalogName).FormattedPrice%>'
                                runat="server"></asp:Label>
                        </div>
                        <div class="add-to-cart-image">
                            <asp:ImageButton ID="AddToCartImageButton" OnClick="AddToCartImageButton_Click" AlternateText="Add To Wish List"
                                SkinID="AddToCartImageButtonSmall" runat="server" />
                        </div>
                        <div class="add-to-cart-text">
                            <asp:LinkButton ID="AddToCartLinkButton" OnClick="AddToCartLinkButton_Click" Text="Add to Wish List"
                                runat="server"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>
