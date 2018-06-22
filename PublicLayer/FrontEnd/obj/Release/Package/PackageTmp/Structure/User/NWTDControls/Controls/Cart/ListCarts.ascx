<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="ListCarts.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.ListCarts" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="cms" %>	
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/LineItemTotal.ascx" TagName="LineItemTotal" TagPrefix="NWTD" %>	

<asp:ObjectDataSource 
	runat="server" 
	ID="dsMyCarts" 
	SelectMethod="LoadByCustomer" 
	TypeName="MediaChase.Commerce.Orders.Cart" 
	onselecting="dsMyCarts_Selecting">
	<SelectParameters>
		<asp:Parameter Type="String" Name="CustomerId" />
	</SelectParameters>
</asp:ObjectDataSource>
 
<asp:UpdatePanel runat="server" ID="udpCartViewer" ChildrenAsTriggers="true" UpdateMode="Conditional" >
	<ContentTemplate>
		<div class="nwtd-cartlist-panel">
			<h2>My Shopping Carts</h2>
			updated <%=DateTime.Now.ToString() %> 
			<ul class="nwtd-cartlist-cartlinks">
				<li>
					<asp:HyperLink 
						runat="server" 
						ID="linkMyOrders" 
						Text="My Orders" 
						NavigateUrl="~/Cart/manage.aspx" />
				</li>
			</ul>
			<ul class="nwtd-cartlist-carts">
				<asp:Repeater  
					DataSourceID="dsMyCarts" 
					runat="server" 
					ID="rptrMyCarts" 
					OnItemCommand="makeActive_Click"
					onitemdatabound="rptrMyCarts_ItemDataBound" >
					<ItemTemplate>
						<li>
							<div class="nwtd-cartlist-cartheader">
								<h3  runat="server" ID="CartHeading"><%#Eval("name") %></h3>
								<asp:HyperLink 
									runat="server" 
									ID="linkViewOrder" 
									Visible="false" 
									Text="View Order" 
									NavigateUrl='<%# String.Format("~/Cart/view.aspx?cart={0}", Eval("name")) %>' />
								<asp:LinkButton runat="server" ID="lbMakeActive" Text="Make active" CommandArgument='<%#Eval("name") %>' /> 
							</div>
							<asp:GridView ShowHeader="false" runat="server" SkinID="CartList" ID="gvMyCart" AutoGenerateColumns="false">
								<EmptyDataTemplate>
									There are no items in this Wish List.
								</EmptyDataTemplate>
								<Columns>
									<asp:BoundField ItemStyle-CssClass="quantity" DataField="Quantity" DataFormatString="{0:#}"  />
									<asp:TemplateField ControlStyle-Width="60" FooterStyle-Wrap="true">
										<ItemTemplate>
											<cms:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />			                    
										</ItemTemplate>
									</asp:TemplateField>
									<asp:TemplateField >
										<ItemStyle CssClass="item-total" />
										<ItemTemplate>
											<NWTD:LineItemTotal runat="server" ID="ItemTotal" LineItem="<%#Container.DataItem %>"  />
										</ItemTemplate>
									</asp:TemplateField>
								</Columns>
							</asp:GridView>
						</li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>
