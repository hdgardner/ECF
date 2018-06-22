<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Float_Edit" Codebehind="Float.Edit.ascx.cs" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
		<td style="padding-right:3px;">
			<asp:TextBox id="txtValue" runat="server" CssClass="text" Wrap="False" Width="100%"></asp:TextBox>
		</td>
		<td width="20px">
			<asp:RequiredFieldValidator id="vldValue_Required" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
			<asp:RangeValidator ID="vldValue_Range" Runat="server" Type="Double" ErrorMessage="*" ControlToValidate="txtValue" Display="Dynamic" MinimumValue="-1000000000000.00" MaximumValue="1000000000000.00"></asp:RangeValidator>
		</td>
	</tr>
</table>
