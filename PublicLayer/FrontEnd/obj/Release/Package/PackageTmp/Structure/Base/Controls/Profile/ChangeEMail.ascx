<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeEMail.ascx.cs" Inherits="Mediachase.Cms.Controls.ChangeEMail" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	<tr>
		<td><h1><%=RM.GetString("ACCOUNT_REGISTRATION_CHANGE_EMAIL_LABEL")%></h1></td>
	</tr>
    <tr>
        <td style="width: 100%">
            <%=RM.GetString("ACCOUNT_ENTER_EMAIL_LABEL")%>:
            <asp:TextBox ID="tbEMail" runat="server" Width="200px"></asp:TextBox>&nbsp;
            <asp:RequiredFieldValidator ID="EMailRequired" runat="server" ControlToValidate="tbEMail">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator Runat="server" ID="EmailValidator1" Display="Dynamic" ControlToValidate="tbEMail"
				ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:RegularExpressionValidator>
			<asp:Button ID="EMailButton"  runat="server" Text="Change eMail" OnClick="EMailButton_Click" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="ErrorLabel" CssClass="ErrorText" EnableViewState="false" runat="server" />
        </td>
    </tr>
</table>