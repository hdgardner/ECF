<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuyModule.ascx.cs" Inherits="Templates_Everything_Entry_Modules_BuyModule" %>
<%@ Register Src="CompareButtonModule.ascx" TagName="CompareButtonModule" TagPrefix="cms" %>
<%@ Register Src="BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cms" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Dto" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Managers" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>

<div id="entry-buy">
    <div class="rowWrap">
        <div id="entry-buy-product-info-col-01">
            <div id="entry-buy-product-info-col-01-row-01">
                <div id="entry-buy-product-info-price">
                    <asp:Label runat="server" ID="PriceLabel"></asp:Label>
                </div>
                <br class="clearfloat" />
                <div id="ProductInfoSavingsDiv" runat="server">
                    <%=RM.GetString("GENERAL_LISTPRICE_LABEL")%>:<asp:Label runat="server" ID="ListPriceLabel"></asp:Label><br />
                    You save: <asp:Label runat="server" ID="DiscountPrice"></asp:Label>
                </div>
                <br class="clearfloat" />
                <div id="entry-buy-product-info-savings-availability">
                    <asp:Label runat="server" ID="AvailabilityLabel"></asp:Label>
                </div>
                <br class="clearfloat" />
            </div>
        </div>
        <div id="entry-buy-product-info-col-02">
            Item Number:<br />
            <asp:Label runat="server" ID="CodeLabel" Text="<%#Entry.ID%>"></asp:Label><br />
            <cms:CompareButtonModule ID="CompareButtonModule1" runat="server" Product='<%# Entry %>' />
        </div>
        <br class="clearfloat" />
    </div>
    <div id="entry-buy-product-info-quantity">
        Select Quantity:<br />
        <asp:DropDownList runat="server" ID="QuantityList">
            <asp:ListItem Value="1">1</asp:ListItem>
            <asp:ListItem Value="2">2</asp:ListItem>
            <asp:ListItem Value="3">3</asp:ListItem>
            <asp:ListItem Value="4">4</asp:ListItem>
            <asp:ListItem Value="5">5</asp:ListItem>
            <asp:ListItem Value="6">6</asp:ListItem>
            <asp:ListItem Value="7">7</asp:ListItem>
            <asp:ListItem Value="8">8</asp:ListItem>
            <asp:ListItem Value="9">9</asp:ListItem>
            <asp:ListItem Value="10">10</asp:ListItem>
        </asp:DropDownList>
    </div>
    
    <asp:Panel ID="RelatedProductsPanel" runat="server">
        <div id="entry-related-products">
            <div class="header">
                <asp:Label runat="server" ID="RelatedProductsLabel">Related Products and Accessories</asp:Label>
            </div>
            <br class="clearfloat" />
            <asp:Repeater ID="RelatedProductsRepeater" runat="server">
                <ItemTemplate>
                    <div class="rowWrap">
                        <div class="checkbox">
                            <asp:CheckBox ID="RelatedProductsCheckbox" Text='<%#((Entry)Eval("Entry")).ID%>'
                                runat="server" />
                        </div>
                        <div class="image">
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'><cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Eval("Entry")).ItemAttributes.Images%>' Width="47" ShowThumbImage="true"
                                ID="PrimaryImage" PropertyName="PrimaryImage" /></asp:HyperLink>
                        </div>
                        <div class="link">
                            <div class="brand-name">
                                <%# ((Entry)Eval("Entry")).ItemAttributes["Brand"]%>
                            </div>                        
                            <asp:HyperLink ID="Link" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'><%#StoreHelper.GetEntryDisplayName((Entry)Eval("Entry"))%></asp:HyperLink>
                        </div>
                        <div class="price">
                            <asp:Label ID="PriceLabel" CssClass="ecf-price" Text='<%# StoreHelper.GetDiscountPrice((Entry)Eval("Entry"), CatalogName).FormattedPrice%>'
                                runat="server"></asp:Label>
                        </div>
                        <br class="clearfloat" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <asp:Panel ID="AdditionalOptionsPanel" runat="server">
        <div id="entry-additional-options">
            <div class="header">
                <asp:Label runat="server" ID="AdditionalOptionsLabel">Additional Options/Plans</asp:Label>
            </div>
            <br class="clearfloat" />
            <asp:Repeater ID="AdditionalOptionsRepeater" runat="server">
                <ItemTemplate>
                    <div class="rowWrap">
                        <div class="checkbox">
                            <asp:CheckBox ID="AdditionalOptionsCheckBox" Text='<%#((Entry)Eval("Entry")).ID%>'
                                runat="server" />
                        </div>
                        <div class="image">
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'><cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Eval("Entry")).ItemAttributes.Images%>' Width="47" ShowThumbImage="true"
                                ID="PrimaryImage" PropertyName="PrimaryImage" /></asp:HyperLink>
                        </div>                       
                        <div class="price">
                            <asp:Label ID="PriceLabel" CssClass="ecf-price" Text='<%# StoreHelper.GetDiscountPrice((Entry)Eval("Entry"), CatalogName).FormattedPrice%>'
                                runat="server"></asp:Label>
                        </div>
                        <div class="link">
                            <div class="brand-name">
                                <%# ((Entry)Eval("Entry")).ItemAttributes["Brand"]%>
                            </div>                                               
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl((Entry)Eval("Entry"))%>'><%#StoreHelper.GetEntryDisplayName((Entry)Eval("Entry"))%></asp:HyperLink>
                        </div>
                        <br class="clearfloat" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <div class="rowWrap">
        <div id="entry-purchase">
            <cms:BuyButtonModule runat="server" ID="BuyButton1" Entry='<%# Entry%>' OnClick='BuyButton_Click' />
            <br />    
            <asp:LinkButton ID="WishListLink" OnClick="WishlistLink_Click" runat="server">Add to Wishlist</asp:LinkButton>
        </div>
        <div id="entry-shipping-note">
            Usually Ships in 24 Hours
        </div>
        <br class="clearfloat" />
    </div>
</div>