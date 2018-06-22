<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Everything_Compare_ProductsCompareView" Codebehind="ProductsCompareView.ascx.cs" %>
<%@ Register Src="../EntryTemplates/Modules/BuyButtonModule.ascx" TagName="BuyButtonModule" TagPrefix="cms" %>
<%@ Import Namespace="Mediachase.Commerce.Catalog.Objects" %>
<%@ Import Namespace="Mediachase.Cms.Util" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:HiddenField ID="hfCurrentComparisonGroup" runat="server" />
<asp:Panel ID="pnlCompareProducts" runat="server">
    <div id="ComparisonContainer">
        <div id="ComparisonHeaderContainer">
         <asp:Repeater runat="server" ID="CompareGroupsRepeater">
            <ItemTemplate>
                <asp:HyperLink ID="hlCompareGroup" runat="server" CssClass="comparisonGroupLink"/>
                <asp:Label ID="lblCompareGroup" runat="server" CssClass="comparisonGroupText"></asp:Label>&nbsp;
            </ItemTemplate>
        </asp:Repeater>
        </div>
        <table width="" cellspacing="0" cellpadding="0" border="0" class="comparison">
            <tr class="productimage">
                <td class="first top"> </td>
                <asp:Repeater runat="server" DataSource='<%# ProductsToCompare%>' ID="rptrHeaderImage">
                    <ItemTemplate>
                        <td align="center" class="top">
                            <div class="comparisonHeaderContainer">
                                <asp:ImageButton runat="server" ID="removeButton" 
                                    ToolTip='<%# RM.GetString("COMPAREPRODUCTSMODULE_REMOVE")%>'
                                    AlternateText='<%# RM.GetString("COMPAREPRODUCTSMODULE_REMOVE")%>'
                                    ImageUrl='<%# CommonHelper.GetImageUrl("Images/remove_from_compare.gif", this.Page)%>'
                                    CommandName="Remove" OnCommand="removeButton_Command" CommandArgument='<%# String.Format("{0};{1}", ((Entry)Container.DataItem).CatalogEntryId, CurrentMetaClassName)%>'
                                    ImageAlign="AbsMiddle" class="removebutton" />
	                              
	                              <a href="#" onclick="CSCheckout.RedirectFromPopupWindow('<%# CommerceHelper.GetAbsolutePath(StoreHelper.GetEntryUrl((Entry)Container.DataItem))%>')">
                                    <cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#((Entry)Container.DataItem).ItemAttributes.Images%>'
                                        Width="47" ShowThumbImage="true" ID="PrimaryImage" PropertyName="PrimaryImage" />
                                </a>        
                                
                            </div>
                            <br />
                            <a href="#" class="productname" onclick="CSCheckout.RedirectFromPopupWindow('<%# CommerceHelper.GetAbsolutePath(StoreHelper.GetEntryUrl((Entry)Container.DataItem))%>')">
                                <%#StoreHelper.GetEntryDisplayName((Entry)Container.DataItem)%>
                            </a>    
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
            <tr class="header">
                <td class="first"> </td>
                <asp:Repeater runat="server" DataSource='<%# ProductsToCompare%>' ID="rptrHeaderPurchaseLink">
                    <ItemTemplate>
                        <td>
                            <div class="price">
                                <asp:Literal ID="Literal1" runat="server" Text='<%# StoreHelper.GetSalePrice((Entry)Container.DataItem, 1).FormattedPrice %>'/>
                            </div>
                            <%--<asp:ImageButton ID="PurchaseLink" OnCommand="addToCartButton_Command" CommandName="AddToCart" OnClientClick="javascript:return CSCheckout.ConfirmAddToCart();"
                                CommandArgument='<%# ((Entry)Container.DataItem).CatalogEntryId %>' AlternateText='<%#RM.GetString("SKU_ADD_TO_CART")%>' SkinID="AddToCartImageButtonLarge" runat="server" />--%>
                                <cms:BuyButtonModule runat="server" ID="BuyButton1" Entry='<%# (Entry)Container.DataItem%>' OnClick='BuyButton_Click' IsPopupWindow="true" />
                            <div class="wishlistlink">
                              <asp:LinkButton ID="WishListLink" OnCommand="WishlistLink_Command" CommandName="AddToWishList" 
                                  CommandArgument='<%# ((Entry)Container.DataItem).CatalogEntryId %>' runat="server">Add to Wishlist</asp:LinkButton>
                            </div>   
                        </td>
                    </ItemTemplate>
                </asp:Repeater> 
            </tr>
            
            <asp:Repeater ID="rptrMainTable" runat="server">
                <ItemTemplate>
                  <tr class='<%# ((Container.ItemIndex % 2 == 0) ? "odd" : "even") + (((CompareItem)Container.DataItem).EqualValues ? " equal" : String.Empty) + ((Container.ItemIndex == ((List<CompareItem>)((Repeater)Container.Parent).DataSource).Count - 1) ? " bottom" : String.Empty) %>'>
		                <td class="first"><asp:Literal ID="Literal1" runat="server" Text='<%# ((CompareItem)Container.DataItem).Title %>' /></td>
		                <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# ((CompareItem)Container.DataItem).Attributes %>'>
		                <ItemTemplate>
		                    <td valign="top">
		                        <asp:Literal ID="Literal1" runat="server" Text='<%# String.IsNullOrEmpty(Container.DataItem.ToString()) ? "&nbsp;" : Container.DataItem %>' />
		                    </td>
		                </ItemTemplate>
		                </asp:Repeater>
	                </tr>
                </ItemTemplate>
            </asp:Repeater> 
        </table>
        <div id="ComparisonFooterContainer">
            <asp:LinkButton OnClick="ClearCompareButton_Click" ID="ClearCompareButton" runat="server" CssClass="ecf-product-comparison-clear1" Text='<%# RM.GetString("COMPAREPRODUCTSMODULE_CLEAR") %>' />
        </div>
    </div>
</asp:Panel>
