<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="True" CodeBehind="BuyButtonModule.ascx.cs" Inherits="Templates_Everything_Entry_Modules_BuyButtonModule" %>
<asp:UpdatePanel runat="server" ID="uplByButton">
    <ContentTemplate>
        <asp:ImageButton ID="PurchaseLink" OnClientClick="tb_show_overlay()" OnClick="PurchaseLink_Click" AlternateText='<%#RM.GetString("SKU_ADD_TO_CART")%>' SkinID="AddToCartImageButtonLarge" runat="server" />
        <asp:Panel runat="server" ID="pnlResponseBlock" Visible="false" style="display: none;">
            <div id="entry-thickbox">
                <div class="row">
                      <div class="image">
                          <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl(Entry)%>'>
                          <cms:MetaImage runat="server" OpenFullImage="false" DataSource='<%#Entry.ItemAttributes.Images%>'
                             Width="47" ShowThumbImage="true" ID="PrimaryImage" PropertyName="PrimaryImage" /></asp:HyperLink>
                      </div>
                      <div class="desc">       
                          The product <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# StoreHelper.GetEntryUrl(Entry)%>'
                              Text='<%#StoreHelper.GetEntryDisplayName(Entry)%>' CssClass="ecf-compareproduct-itemlink" />
                          has been added to your Wish List.   
                      </div>
                </div>
            </div>
            <div style="text-align:center;">
              <input type='button' value='Continue Shopping' onclick='tb_remove();return false;' class="continue-button button" style=" width:140px;"/>
              <asp:Button runat="server" ID="btnGoToCart" Text="View My Wish List ..." class="continue-button button" style=" width:140px;"/>
            </div>
        </asp:Panel>    
    </ContentTemplate>
</asp:UpdatePanel>    
