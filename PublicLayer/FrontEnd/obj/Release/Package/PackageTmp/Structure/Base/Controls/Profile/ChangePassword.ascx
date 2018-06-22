<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.ascx.cs" Inherits="Mediachase.Cms.Controls.ChangePassword" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	<tr>
		<td><h1><%=RM.GetString("ACCOUNT_REGISTRATION_CHANGE_PASS_LABEL")%></h1></td>
	</tr>
    <tr>
        <td style="width: 100%">
            <asp:ChangePassword Width="100%" ID="ChangePwd" runat="server" ChangePasswordTitleText="">
                <ChangePasswordTemplate>
                    <table border="0" cellpadding="0" width="500">
                        <tr>
                            <td align="right">
                                <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"><%=RM.GetString("GENERAL_PASSWORD_LABEL")%>:</asp:Label></td>
                            <td>
                                <asp:TextBox ID="CurrentPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                    ErrorMessage='<%#RM.GetString("ACCOUNT_CURRENT_PASSWORD_REQUIRED")%>' ToolTip='<%#RM.GetString("ACCOUNT_CURRENT_PASSWORD_REQUIRED")%>' ValidationGroup="ctl00$ChangePwd"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"><%=RM.GetString("ACCOUNT_REGISTRATION_NEW_PASSWORD_LABEL")%>:</asp:Label></td>
                            <td>
                                <asp:TextBox ID="NewPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                    ErrorMessage='<%#RM.GetString("ACCOUNT_NEW_PASSWORD_REQUIRED") %>' ToolTip='<%#RM.GetString("ACCOUNT_NEW_PASSWORD_REQUIRED") %>'
                                    ValidationGroup="ctl00$ChangePwd"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="white-space: nowrap">
                                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"><%=RM.GetString("ACCOUNT_REGISTRATION_CONFIRM_PASSWORD_LABEL")%>:</asp:Label></td>
                            <td>
                                <asp:TextBox ID="ConfirmNewPassword" Width="250" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                    ErrorMessage='<%#RM.GetString("ACCOUNT_CONFIRM_NEW_PASSWORD_REQUIRED") %>' ToolTip='<%#RM.GetString("ACCOUNT_CONFIRM_NEW_PASSWORD_REQUIRED") %>'
                                    ValidationGroup="ctl00$ChangePwd"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                    ControlToValidate="ConfirmNewPassword" Display="Dynamic" 
                                    ErrorMessage='<%#RM.GetString("ACCOUNT_PASSWORD_CONFIRM_MUST_MATCH") %>'
                                    ValidationGroup="ctl00$ChangePwd"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2" style="color: red">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                                    Text="Change Password" ValidationGroup="ctl00$ChangePwd" />
                                <asp:Button ID="CancelPushButton" Visible="false" runat="server" CausesValidation="False"
                                    CommandName="Cancel" Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                </ChangePasswordTemplate>
            </asp:ChangePassword>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="ErrorLabel" CssClass="ErrorText" EnableViewState="false" runat="server" />
        </td>
    </tr>
</table>