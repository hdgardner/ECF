<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Cart_WishListView" Codebehind="WishListView.ascx.cs" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Register Src="SharedModules/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="uc1" %>
<%@ Register Src="SharedModules/PriceLineModule.ascx" TagName="PriceLineModule" TagPrefix="cart" %>
<div runat="server" id="divViewer"></div>
<table style="width: 100%" border="0" cellspacing="0" cellpadding="10">
	<tr>
		<td><h1>Wish List</h1></td>
		<td align="right"><cms:ThemedImage runat="server" ImageUrl="images/spacer.gif" /></td>
	</tr>
</table>
<table cellspacing="1" cellpadding="3" style="width: 100%">
	<tr>
		<td>
		    <asp:GridView OnRowDeleting="ShoppingCart_RowDeleting" OnRowCreated="ShoppingCart_RowCreated" EnableViewState="true" runat="server" ID="ShoppingCart" SkinID="ShoppingCart" DataKeyNames="LineItemId,Quantity" AutoGenerateColumns="false" style="width: 100%">
		        <EmptyDataTemplate>
					Your Wish List is empty.
		        </EmptyDataTemplate>
		        <Columns>
		            <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top" ItemStyle-Width="80px">
		                <ItemTemplate>
		                    <asp:LinkButton runat="server" ID="RemoveItem" ToolTip='' CssClass="ecf-delete" Text="Remove" CommandName="Delete" CausesValidation="false"></asp:LinkButton>
		                </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="" ItemStyle-Width="50">
		                <ItemTemplate>
		                    <cms:MetaImage OpenFullImage="true" Width="35" ShowThumbImage="true" ID="PrimaryImage" DataSource="<%#GetEntryImageSource((Mediachase.Commerce.Orders.LineItem)Container.DataItem) %>" PropertyName="PrimaryImage" runat="server" />
		                </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Description" ItemStyle-Width="400">
		                <ItemTemplate>
		                    <uc1:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />
		                    <div class="ecf-note">added <%#((DateTime)Eval("Created")).ToString("MMMM dd, yyyy") %></div>		                    
		                </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="180px" HeaderText="Sale Price">
		                <ItemTemplate>
		                    <cart:PriceLineModule ID="PriceLineModule1" runat="server" LineItem="<%#Container.DataItem%>"></cart:PriceLineModule>
		                </ItemTemplate>
		            </asp:TemplateField>
		            <asp:TemplateField ItemStyle-Width="70px" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="Qty">
		                <ItemTemplate>
		                    <asp:TextBox runat="server" ID="Quantity" Width="20"></asp:TextBox>
		                </ItemTemplate>
		            </asp:TemplateField>	            
		            <asp:TemplateField ItemStyle-Width="160px" HeaderText="" HeaderStyle-HorizontalAlign="left" ItemStyle-HorizontalAlign="center">
		                <ItemTemplate>
		                    <asp:Label runat="server" ID="AvailabilityLabel" CssClass="ecf-inventory-status"></asp:Label><br />
		                    <asp:LinkButton runat="server" ID="AddCartLink" CssClass="ecf-buybutton" OnCommand="AddToCart" CommandArgument='<%#Eval("CatalogEntryId")%>'>Add to Wish List</asp:LinkButton>	                    
		                </ItemTemplate>
		            </asp:TemplateField>		            
		        </Columns>
		    </asp:GridView>
		</td>
	</tr>
</table>
<table cellpadding="3" cellspacing="1" style="width: 100%">
	<tr>
		<td align="left">
			<asp:Button id="UpdateCartButton" runat="server" Text='Update Wishlist' OnClick="UpdateButton_Click">
			</asp:Button>
		</td>
		<td align="right">
			<asp:Button id="ContinueButton" runat="server" OnClick="ContinueButton_Click" Text='Continue Shopping'>
			</asp:Button>
		</td>
	</tr>
</table>