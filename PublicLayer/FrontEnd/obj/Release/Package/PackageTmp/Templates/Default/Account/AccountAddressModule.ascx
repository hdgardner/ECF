<%@ Control Language="c#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.AccountAddressModule" Codebehind="AccountAddressModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Profile/Address/SharedModules/CustomerAddressViewModule.ascx" TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Import Namespace="Mediachase.Commerce.Profile" %>
<asp:TextBox ID="tbAddressID" Visible="false" runat="server"></asp:TextBox>
<table style="width: 100%; text-align: left" border="0" cellspacing="1" cellpadding="5">
	<tr>
		<td style="width: 100%">
			<h1>
				<%=RM.GetString("ACCOUNT_ADDRESS_MANAGEMENT_LABEL")%>
			</h1>
		</td>
	</tr>
	<tr>
		<td style="width: 100%">
            <asp:HyperLink ID="hlAddNewAddress" runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/secure/AccountAddressNew.aspx")%>'>
				New Address
			</asp:HyperLink>
        </td>
	</tr>
    <tr>
        <td>
        <asp:DataList RepeatColumns="2" style="width: 100%" runat="Server" ID="AddressList" DataKeyField="AddressId" OnItemCreated="AddressList_ItemCreated">
            <ItemTemplate>
                <%# GetCurrentRowIndex() %>.
                 <ecf:AddressViewModule ID="AddressViewModule1" AddressInfo='<%# (CustomerAddress)Container.DataItem %>' runat="server" />
                 <asp:Button id="btnEditAddress" runat="server" OnClick="EditAddress" CommandArgument='<%# Eval("AddressId") %>' Text='<%#RM.GetString("ACCOUNT_ADDRESS_EDIT") %>' />
                 <asp:Button id="btnDeleteAddress" runat="server" OnClick="DeleteAddress" CommandArgument='<%# Eval("AddressId") %>' Text='<%#RM.GetString("ACCOUNT_ADDRESS_DELETE") %>' />
            </ItemTemplate>
        </asp:DataList>
        </td>
    </tr>
</table>