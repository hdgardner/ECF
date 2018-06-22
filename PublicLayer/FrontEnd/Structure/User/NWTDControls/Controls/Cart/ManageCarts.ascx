<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageCarts.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.ManageCarts" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/CartTotal.ascx" TagName="CartTotal" TagPrefix="NWTD" %>	

<h1>My Wish Lists</h1>
<div class="nwtd-dialogs" style="display:none">
	<div class="nwtd-dialogBody nwtd-createCartDialog">
		<label for="createCartName">Wish List Name</label>
		<input type="text" name="createCartName" />
		<input type="submit" value="Create Wish List" class="create-cart-btn buttons" />
		<input type="button" value="Cancel" class="cancel-btn buttons" />
	</div>

	<div class="nwtd-dialogBody nwtd-deleteCartDialog">
		<input type="hidden" name="cartName" value="" />
		<input type="submit" value="Delete" class="delete-cart-blue-btn buttons" />
		<input type="button" value="Cancel" class="cancel-btn buttons" />
	</div>

	<div class="nwtd-dialogBody nwtd-editCartDialog" >
		<label>Wish List Name</label>
		<input type="text" name="newName" value="" />
		<input type="hidden" name="cartName" value="" />
		<asp:Button runat="server" ID="btnRenameCart" CssClass="update-cart-btn buttons" Text="Update Wish List" />
		<input type="button" class="cancel-btn buttons" value="Cancel" />
	</div>

	<div class="nwtd-dialogBody nwtd-copyCartDialog" >
		<label>Wish List Name</label>
		<input type="text" name="copyCartName" value="" />
		<input type="hidden" name="sourceCartName" value="" />
		<div class="float-right"><input type="submit" value="Copy Wish List" class="copy-cart-btn buttons" />
		<input type="button" value="Cancel" class="cancel-btn buttons" /></div>
	</div>
</div>
<div class="manage-carts">
<asp:UpdatePanel runat="server" ID="udpUserCarts"  UpdateMode="Conditional" ChildrenAsTriggers="true" >
	<ContentTemplate>
		<asp:ObjectDataSource 
			ID="dsUserCarts" 
			runat="server" 
			SelectMethod="LoadByCustomer" 
			TypeName="Mediachase.Commerce.Orders.Cart" 
			onselecting="dsUserCarts_Selecting" 
			>
			<SelectParameters>
				<asp:Parameter Type="String" Name="CustomerId" />
			</SelectParameters>
		</asp:ObjectDataSource>
		<asp:GridView 
			EnableViewState="false" 
			ID="gvUserCarts"
			runat="server" 
			AutoGenerateColumns="False"  
			ShowFooter="True"  
			DataKeyNames="Name" 
			DataSourceID="dsUserCarts" 
			onrowdatabound="gvUserCarts_RowDataBound" 
			onrowcreated="gvUserCarts_RowCreated"  >
			<Columns>
				<asp:TemplateField ItemStyle-CssClass="command">
					<ItemTemplate>		
						<input type="hidden" value='<%#Eval("Name") %>' />
						<asp:HiddenField EnableViewState="false" runat="server" ID="hfCartName" Value='<%#Eval("Name") %>' />
						<%--<asp:HyperLink runat="server" ID="h1ViewCart" Text="View" NavigateUrl='<%#string.Format( NWTD.Orders.Cart.Pages.ViewCartPage, Eval("Name")) %>'  />--%>
						<asp:HyperLink CssClass="nwtd-editCart" runat="server" ID="hlEditCart" Text="Rename" NavigateUrl='<%#string.Format( "~/Services/Cart.svc/Edit?cart={0}", Eval("Name")) %>'  />
						<asp:HyperLink runat="server" CssClass="nwtd-copyCart" ID="hlCopyCart" Text="Copy" NavigateUrl='<%#string.Format( "~/Services/Cart.svc/Copy?cart={0}", Eval("Name")) %>'  />
						<asp:HyperLink EnableViewState="false" CssClass="nwtd-deleteCart" runat="server" ID="hlDeleteCart" Text="Delete" NavigateUrl='<%#string.Format( "~/Services/Cart.svc/Edit?cart={0}", Eval("Name")) %>'  />			
				</ItemTemplate>
					<ItemStyle CssClass="command" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Wish List Name" SortExpression="Name">
					<ItemTemplate>
						<asp:LinkButton runat="server" CommandName="SelectCart" CommandArgument='<%# Bind("Name") %>'  Text='<%# Bind("Name") %>' ID="lbGoToCart" oncommand="lbGoToCart_Command"></asp:LinkButton>
						<%--<asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>--%>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField DataField="CustomerName" HeaderText="Customer Name" SortExpression="CustomerName" ReadOnly="True" />
				<asp:TemplateField HeaderText="Total" HeaderStyle-CssClass="price" ItemStyle-CssClass="price" >
					<ItemTemplate>
						<NWTD:CartTotal runat="server" Cart="<%#Container.DataItem%>" ID="CartTotal" />
					</ItemTemplate>
					<HeaderStyle CssClass="price" />
					<ItemStyle CssClass="price" />
				</asp:TemplateField>
				<asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ReadOnly="True" />
				<asp:BoundField DataField="Created" HeaderStyle-CssClass="created" ItemStyle-CssClass="created"  HeaderText="Created" SortExpression="Created" DataFormatString="{0:d}" ReadOnly="True" >
					<HeaderStyle CssClass="created" />
					<ItemStyle CssClass="created" />
				</asp:BoundField>
		<%--		<asp:BoundField DataField="Modified" HeaderStyle-CssClass="modified" ItemStyle-CssClass="modified" HeaderText="Modified" SortExpression="Modified" DataFormatString="{0:d}"  ReadOnly="True" >
					<HeaderStyle CssClass="modified" />
					<ItemStyle CssClass="modified" />
				</asp:BoundField>--%>
					<%--If you want to restore the ability to choose the "active" cart using a radio button, make the control below visible--%>				<asp:TemplateField Visible="false" HeaderText="Active" HeaderStyle-CssClass="radio" ItemStyle-CssClass="radio" >
					<ItemTemplate>
						<asp:RadioButton runat="server" ID="rbActiveCart" AutoPostBack="true" OnCheckedChanged="rbActiveCart_CheckedChanged" />
					</ItemTemplate>
					<HeaderStyle CssClass="radio" />
					<ItemStyle CssClass="radio" />
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
	</ContentTemplate>
</asp:UpdatePanel>
</div>
<asp:HyperLink runat="server" NavigateUrl="#" ID="hlCreateCart" CssClass="nwtd-createCart create-cart-btn buttons" Text="Create New Wish List" />