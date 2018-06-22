<%@ Control Language="c#" Inherits="Mediachase.Cms.Controls.CustomerAddressNewControl" Codebehind="CustomerAddressNewControl.ascx.cs" %>
<%@ Register Src="SharedModules/CustomerAddressEditModule.ascx" TagName="CustomerAddressEditModule" TagPrefix="ecf" %>
<asp:TextBox ID="tbAddressID" Visible="false" runat="server"></asp:TextBox>
<table style="width: 100%; text-align: left" border="0" cellspacing="1" cellpadding="5">
	<tr>
		<td style="width: 100%">
			<h1>
				<%=RM.GetString("ACCOUNT_ADDRESS_NEW_TITLE")%>
			</h1>
		</td>
	</tr>
	<tr>
		<td style="width: 100%" id="ViewAllAddressesRow" runat="server">
            <asp:HyperLink ID="hlAddNewAddress" runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/secure/AccountAddress.aspx")%>'>
				<%=RM.GetString("ACCOUNT_ADDRESS_NEW_VIEW_ADDRESSES")%>
            </asp:HyperLink>
        </td>
	</tr>
	
    <tr>
        <td>
            <ecf:CustomerAddressEditModule ID="AddressEditModule1" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Button id="btnSave" OnClick="SaveAddress" runat="server" Text='Save' />
            <asp:Button id="btnCancel" OnClick="CancelAddress" runat="server" CausesValidation="false" Text='Cancel' />
        </td>
    </tr>
</table>

