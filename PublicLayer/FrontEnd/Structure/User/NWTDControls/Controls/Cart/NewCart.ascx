<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewCart.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.NewCart" %>
<asp:HyperLink  runat="server" NavigateUrl="#" CssClass="nwtd-createCart" ID="hlCreateCart" />

<div class="nwtd-dialogBody nwtd-createCartDialog" style="display:none;">
	<label for="createCartName">Wish List Name</label>
	<input type="text" name="createCartName" />
	<input type="submit" value="Create Wish List" class="create-cart-btn buttons" />
	<input type="button" value="Cancel" class="cancel-btn buttons" />
</div>