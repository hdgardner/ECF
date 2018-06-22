<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationAddressSelector.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.OrganizationAddressSelector" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/Address.ascx" TagPrefix="NWTD" TagName="CustomerAddress" %>
<asp:Repeater runat="server" ID="rptShippingAddresses" onitemdatabound="rptShippingAddresses_ItemDataBound">
	<HeaderTemplate>
		<ul class="nwtd-addressSelector">
	</HeaderTemplate>
	<ItemTemplate>
		<li>
			<cms:GlobalRadioButton GroupName='<%# String.Format("{0}_{1}","OrganizationAddressGroup",rptShippingAddresses.UniqueID) %>'  runat="server" ID="rbSelectedAddress" />
			<asp:Label runat="server" AssociatedControlID="rbSelectedAddress" ID="lblCustomerAddress" >
				<%# Eval("Name") %>
			</asp:Label>
			<NWTD:CustomerAddress runat="server" ID="CustomerAddress" CustomerAddress="<%#Container.DataItem %>" />
			
		</li>
	</ItemTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</asp:Repeater>
