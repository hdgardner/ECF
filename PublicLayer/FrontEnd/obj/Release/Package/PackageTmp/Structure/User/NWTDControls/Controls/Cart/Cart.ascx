<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	CodeBehind="Cart.ascx.cs" 
	Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.Cart" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="cms" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/LineItemPrice.ascx" TagName="PriceLineModule" TagPrefix="cart" %>
<%@ Register src="~/Structure/User/NWTDControls/Controls/Cart/LineItemTotal.ascx" tagname="LineItemTotal" tagprefix="NWTD" %>
<%@ Register src="~/Structure/User/NWTDControls/Controls/Misc/UpdateMessage.ascx" tagname="UpdateMessage" tagprefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/ISBN.ascx" TagName="ISBN" TagPrefix="NWTD" %>


<asp:ObjectDataSource 
	ID="dsCart" 
	runat="server"
	TypeName="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.Cart"
	onselecting="dsCart_Selecting" 
	SelectMethod="getLineItems">
	<SelectParameters>
		<asp:QueryStringParameter Name="CartName" QueryStringField="cart" DefaultValue='Default' Type="String" />
	</SelectParameters>
</asp:ObjectDataSource>

<div class="cart-header">
    <%-- This was original header before Heath's changes below on 08/02/17 - <h1 class="cart-heading"><%= this.SelectedCartName %></h1> --%>
    <%-- On 08/02/17, Heath replaced the above 'SelectedCartName' header with the following 'cartWishListLabel' which includes WL ID# --%>
    <h1 class="cart-heading"><%= this.cartWishListLabel %></h1>
    <asp:HyperLink runat="server" ID="hlBrowseCatalogTop" cssclass="new-search-btn buttons" NavigateUrl="~/Catalog/SearchResults.aspx">Browse Catalog</asp:HyperLink>
</div>   
<div id="selectedCart">
	<asp:Panel runat="server" DefaultButton="btnSaveChanges">
		<div class="nwtd-cartview-buttonrow">
			<div class="nwtd-cartview-buttonrow-left">
				<asp:Button runat="server" ID="btnDeleteSelectedTop" CssClass="delete-selected-btn buttons" Text="Delete Selected" onclick="btnDeleteSelected_Click" />	
				<asp:hyperlink ID="hlISBNQuickEntryTop" runat="server" cssclass="quick-isbn-btn buttons" Text="Quick ISBN Entry" NavigateUrl="~/cart/QuickISBN.aspx" />
			</div>
			<div class="nwtd-cartview-buttonrow-right">
				<%--<asp:Button runat="server" ID="btnPrint" Text="Print/Export" SkinID="BlueButton" onclick="btnPrint_Click" />--%>
				<span class="nwtd-error nwtd-submitCartWarning" style="display:none">please add quantities before proceeding</span>
				<%--<asp:Button  runat="server" ID="btnSubmitTop" CssClass="nwtd-submitCart add-shipping-info-btn buttons" Text="Next" onclick="btnSubmit_Click" />--%>
                <%-- On 08/02/17, Heath replaced the above 'Add Shipping Info' btn with the following 'Continue' btn --%>
                <asp:Button  runat="server" ID="btnSubmitTop" CssClass="nwtd-submitCart add-shipping-continue-blue-btn buttons" Text="Next" onclick="btnSubmit_Click" />
			</div>
		</div>
		<div style="clear:left;float:left;width: 948px;">
		<asp:UpdatePanel runat="server" ID="udpViewCart" >
			<Triggers>
				<asp:AsyncPostBackTrigger ControlID="btnDeleteSelectedTop" />
				<asp:AsyncPostBackTrigger ControlID="btnDeleteSelected" />
				<asp:AsyncPostBackTrigger ControlID="btnSaveChanges" />
			</Triggers>
			<ContentTemplate>
				<asp:UpdateProgress  runat="server" ID="progressViewCart"  DynamicLayout="true" >
					<ProgressTemplate>
							<NWTD:UpdateMessage CssClass="nwtd-update-message" runat="server" Message="Updating Wish List..." ImageUrl="~/Structure/User/NWTDControls/Images/loading.gif" />
					</ProgressTemplate>
				</asp:UpdateProgress>
				<asp:Label runat="server" ID="lblCartMessage" CssClass="nwtd-message nwtd-cart-message" />
				<asp:GridView 
					ID="gvCart" 
					runat="server" 
					SkinID="CartView" 
					AutoGenerateColumns="false" 
					DataSourceID="dsCart"
					DataKeyNames="LineItemId,Quantity" 
					ondatabound="gvCart_DataBound" 
					onrowdatabound="gvCart_RowDataBound" 
					ShowFooter="true" Width="100%">
					<EmptyDataTemplate>
						This Wish List is Empty.
					</EmptyDataTemplate>
				
					<Columns>
						<asp:TemplateField HeaderStyle-CssClass="col1" ItemStyle-CssClass="col1" HeaderText="" FooterStyle-CssClass="cart-total">
							<ItemTemplate>
								<asp:CheckBox runat="server" ID="cbItemSelected" />
							</ItemTemplate>
							<FooterTemplate>
								Order estimate before tax and shipping charges:
							</FooterTemplate>
				   		 <FooterStyle CssClass="cart-total" />
							<HeaderStyle CssClass="col1" />
							<ItemStyle CssClass="col1" />
					   </asp:TemplateField>
						<asp:TemplateField HeaderStyle-CssClass="col2" HeaderText="Title">
							<ItemTemplate>
								<cms:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />
							</ItemTemplate>
							<HeaderStyle CssClass="col2" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Publisher No/ISBN13" HeaderStyle-CssClass="col3">
							<ItemTemplate>
								<NWTD:ISBN runat="server" ID="ISBN" />
							</ItemTemplate>
							<HeaderStyle CssClass="col3" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Grade" HeaderStyle-CssClass="col4">
							<ItemTemplate>
								<asp:Literal ID="litGrade" runat="server" />
							</ItemTemplate>
							<HeaderStyle CssClass="col4" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Year" HeaderStyle-CssClass="col5">
							<ItemTemplate>
								<asp:Literal ID="litYear" runat="server" />
							</ItemTemplate>
							<HeaderStyle CssClass="col5" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="col6">
							<ItemTemplate>
								<cart:PriceLineModule ID="PriceLineModule1" runat="server" LineItem="<%#Container.DataItem%>"></cart:PriceLineModule>
							</ItemTemplate>
							<HeaderStyle CssClass="col6" />
                            <ItemStyle CssClass="price" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Qty Charged" ItemStyle-CssClass="quantity" HeaderStyle-CssClass="col7">
							<ItemTemplate>
								<asp:TextBox runat="server" TabIndex='<%#Container.DataItemIndex %>'  ID="tbQuantityCharged" Columns="1"></asp:TextBox>
								<asp:CompareValidator
									runat="server" 
									ID="qtyValidator" 
									ControlToValidate="tbQuantityCharged" 
									Type="Integer" ValueToCompare="-1" 
									Operator="GreaterThan" 
									ErrorMessage="* invalid" CssClass="invalid-character"  Display="Dynamic" />
                                    <asp:Label
									runat="server" 
									ID="lblQtyValidator" 
                                    ForeColor="Red"
									EnableViewState="False" Visible="False"
									Text="* invalid" CssClass="invalid-character"  />


							</ItemTemplate>
							<HeaderStyle CssClass="col7" />
							<ItemStyle CssClass="quantity" />
						</asp:TemplateField>
						<asp:TemplateField FooterStyle-CssClass="footer-cart-total" HeaderText="Total"  HeaderStyle-CssClass="col8">
							<ItemTemplate>
								<NWTD:LineItemTotal 
									LineItem="<%#Container.DataItem %>" 
									runat="server" 
									ID="LineItemTotal" 
								/>
							</ItemTemplate>
							<FooterStyle CssClass="footer-cart-total" />
							<HeaderStyle CssClass="col8" />
							<ItemStyle CssClass="total" />
						</asp:TemplateField>
						<asp:TemplateField ItemStyle-CssClass="gratis" HeaderStyle-CssClass="col9">
							<HeaderTemplate>
								Qty Gratis
							</HeaderTemplate>
							<ItemTemplate>
								<asp:TextBox runat="server" ID="tbGratis" Columns="1" />
								<asp:CompareValidator 
									runat="server" 
									ID="gratisValidator" 
									ControlToValidate="tbGratis" 
									Type="Integer" ValueToCompare="-1" 
									Operator="GreaterThan" 
									ErrorMessage="* invalid"
									CssClass="invalid-character"
									Display="Dynamic" />
                                    <asp:Label
									runat="server" 
									ID="lblGratisValidator" 
                                    ForeColor="Red"
									EnableViewState="False" Visible="False"
									Text="* invalid" CssClass="invalid-character"  />	
							</ItemTemplate>
							<HeaderStyle CssClass="col9" />
							<ItemStyle CssClass="gratis" />
						</asp:TemplateField>        	            
					</Columns>
				</asp:GridView>
			</ContentTemplate>
		</asp:UpdatePanel>
		</div>

		<div class="nwtd-cartview-buttonrow">
			<div class="nwtd-cartview-buttonrow-left">
				<asp:Button runat="server" ID="btnDeleteSelected" CssClass="delete-selected-btn buttons" Text="Delete Selected" onclick="btnDeleteSelected_Click" />	
				<asp:hyperlink ID="hlISBNQuickEntryBottom" runat="server" cssclass="quick-isbn-btn buttons" Text="Quick ISBN Entry" NavigateUrl="~/cart/QuickISBN.aspx" />
				<%--<asp:HyperLink runat="server" ID="hlBrowseCatalogBottom" cssclass="new-search-btn buttons" NavigateUrl="~/Catalog/SearchResults.aspx"></asp:HyperLink>--%>
			</div>
			<div class="nwtd-cartview-buttonrow-right">&nbsp;<asp:Button CssClass="save-changes-btn buttons" runat="server" ID="btnSaveChanges" Text="Save Changes" onclick="btnSaveChanges_Click" />
				<span class="nwtd-error nwtd-submitCartWarning" style="display:none">please add quantities before proceeding</span>
				<%--<asp:Button ValidationGroup="SubmitCartGroup" runat="server" ID="btnSubmit" CssClass="nwtd-submitCart add-shipping-info-btn buttons" Text="Add Shipping Info" onclick="btnSubmit_Click" />--%>
                <%-- On 08/02/17, Heath replaced the above 'Add Shipping Info' btn with the following 'Continue' btn --%>
                <asp:Button ValidationGroup="SubmitCartGroup" runat="server" ID="btnSubmit" CssClass="nwtd-submitCart add-shipping-continue-blue-btn buttons" Text="Add Shipping Info" onclick="btnSubmit_Click" />
			</div>
		</div>
	</asp:Panel>

    <!-- On 03/01/18, Heath Gardner added the following 'panel' with Price and No Charge disclaimers per Customer Service's request -->
    <asp:Panel runat="server" ID="pnlCartFinePrint" CssClass="order-summary-notification-panel" Visible="true">
		<p class="cart-price-disclaimers"> <br /> Prices are set by the publisher and are subject to change. <br /> All no-charge items are subject to approval. </p>
	</asp:Panel>
    
</div>



