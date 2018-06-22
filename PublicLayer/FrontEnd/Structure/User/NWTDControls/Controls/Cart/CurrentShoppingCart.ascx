<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrentShoppingCart.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.CurrentShoppingCart" %>
<%@ Assembly Name="NWTD" %>
<div class="nwtd-current-cart">
	<strong>Active Wish List:</strong><br />
	<asp:HyperLink 
		runat="server" 
		ID="hlCurrentShoppingCart" 
		CssClass="cart-name"
		NavigateUrl='<%# Mediachase.Cms.NavigationManager.GetUrl("ViewCart")%>'>
		<%=NWTD.Profile.ActiveCart %>
	</asp:HyperLink><br /> 
	<asp:HyperLink 
		runat="server"
		CssClass="change-cart-link" 
		ID="hlChangeCart" 
		Text="Change Wish List"
		NavigateUrl='<%#  Mediachase.Cms.NavigationManager.GetUrl("ManageCarts") %>'
	/>
</div>