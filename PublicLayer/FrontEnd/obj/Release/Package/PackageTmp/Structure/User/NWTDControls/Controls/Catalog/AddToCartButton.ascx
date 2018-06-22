<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddToCartButton.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.AddToCartButton" %>
<div class="nwtd-addToCartButton">
		<div class="addToCartDialogs" style="display:none">
			<div class="addToCartDialog nwtd-dialogBody">
				<div class="option-btns">
					<a href="#"><img id="Img1" src="~/App_Themes/NWTD/images-template/continue-search-btn.png" runat="server" /></a>&nbsp;&nbsp;<a id="A1" href="~/Cart/view.aspx" runat="server"><img id="Img2" src="~/App_Themes/NWTD/images-template/view-cart-btn.png" runat="server" /></a>
				</div>	  
			</div>
		</div>
		<asp:HiddenField runat="server" ID="hfCode" />
		<a href="#" class="nwtd-AddToCart buttons">Add this item to Wish List</a>

</div>
<div class="addToCartDialogs" style="display:none">
	<asp:Panel runat="server" ID="pnlSelectCart" Visible="false">
		<div class="selectCartDialogItem nwtd-dialogBody">
			<p>
				Please select the Wish List that should receive your items. 
				The cart you select will become your active Wish List. 
			</p>
		
			<asp:DropDownList runat="server" ID="ddlCarts" CssClass="nwtd-selectCartList" DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true" >
				<asp:ListItem Text="[Create a new Wish List]" Value="" />
			</asp:DropDownList>
			<div class="nwtd-selectNewCart">
				<asp:Label runat="server"  AssociatedControlID="SelectNewCartName" >Wish List Name:</asp:Label>
				<asp:TextBox runat="server" ID="SelectNewCartName" CssClass="nwtd-selectNewCartName" />
			</div>
			<input type="submit" class="nwtd-selectCart buttons" value="Set as Active Wish List" />
	
		</div>
	</asp:Panel>
</div>