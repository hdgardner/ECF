<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.RecoverPassword" Codebehind="RecoverPassword.ascx.cs" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	<tr>
		<td><h1 class="standard"><%=RM.GetString("REMIND_TITLE")%></h1></td>
	</tr>
    <tr>
        <td style="width: 100%" id="PasswordRecoveryCSS">
            <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" OnVerifyingUser="UserVerify" OnVerifyingAnswer="AnswerVerify"
				UserNameTitleText="" UserNameInstructionText='<%#RM.GetString("REMIND_MESSAGE")%>' UserNameLabelText='<%#RM.GetString("REMIND_USER_NAME_LABEL")+":" %>' 
				 UserNameFailureText="" SubmitButtonText='<%#RM.GetString("REMIND_SUBMIT")%>' >
            </asp:PasswordRecovery>
        </td>
    </tr>
</table>