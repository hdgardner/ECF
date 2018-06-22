<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Enum_Edit" Codebehind="Enum.Edit.ascx.cs" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style="table-layout:fixed">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td width="100%" style="padding-top:1px;">
						<asp:DropDownList ID="ddlValue" runat="server" Width="100%"></asp:DropDownList>
					</td>
					<td width="20px" runat="server" id="tdEdit" style="padding-left:2px;">
						<button id="btnEditItems" runat="server" style="border:1px;padding:0;height:20px;width:22px;background-color:transparent" type="button"><img 
						height="20" title='EditDictionary' src='<%=CHelper.GetAbsolutePath("/Images/IbnFramework/dictionary_edit.gif")%>' width="22" border="0" /></button>
					</td>
				</tr>
			</table>
		</td>
		<td width="20px"></td>
	</tr>
</table>
<asp:Button id="btnRefresh" runat="server" CausesValidation="False" style="display:none;" OnClick="btnRefresh_Click"></asp:Button>

