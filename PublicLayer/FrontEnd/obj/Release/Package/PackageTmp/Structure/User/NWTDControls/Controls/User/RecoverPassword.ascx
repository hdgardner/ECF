<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecoverPassword.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.RecoverPassword" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	<tr>
		<td><h1 class="standard"><%=RM.GetString("REMIND_TITLE")%></h1></td>
	</tr>
    <tr>
        <td style="width: 100%" id="PasswordRecoveryCSS">
            <asp:PasswordRecovery 
				ID="PasswordRecovery1" 
				runat="server" 
				OnVerifyingUser="UserVerify" 
				OnVerifyingAnswer="AnswerVerify"
				UserNameTitleText="" 
				UserNameRequiredErrorMessage="You must enter a username"
				UserNameInstructionText='<%#RM.GetString("REMIND_MESSAGE")%>' 
				UserNameLabelText='<%#RM.GetString("REMIND_USER_NAME_LABEL")+":" %>' 
				UserNameFailureText="The user name you entered does not exist" 
				SubmitButtonText='<%#RM.GetString("REMIND_SUBMIT")%>'
                SubmitButtonStyle-CssClass="passrec-submit-btn buttons"
                 >
            </asp:PasswordRecovery>
        </td>
    </tr>
</table>