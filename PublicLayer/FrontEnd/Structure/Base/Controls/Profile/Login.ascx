<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.Login" Codebehind="Login.ascx.cs" %>
<div runat="server" id="divViewer"></div>
<table cellpadding="4" cellspacing="4" style="width: 100%">
	<tr>
		<td>
			<h1>
				<%=RM.GetString("ACCOUNT_LOGIN_TITLE")%>
			</h1>
		</td>
	</tr>
	<tr>
		<td>
			<%=RM.GetString("ACCOUNT_LOGIN_MESSAGE")%>
		</td>
	</tr>	
    <tr>
        <td style="width: 100%">
            <asp:Panel ID="Panel1" runat="server" style="width: 100%" DefaultButton="LoginForm$LoginButton">
            <asp:Login ID="LoginForm" TitleText="" OnAuthenticate="LoginForm_Authenticate" OnLoggedIn="OnLoggedIn"  
				OnLoggingIn="OnLoggingIn" PasswordRecoveryText="<%$resources: StoreResources, ACCOUNT_REGISTRATION_FORGOT_PASSWORD_LABEL%>" 
				PasswordRecoveryUrl='<%#ResolveUrl("~/Profile/PasswordRecovery.aspx") %>' runat="server" 
				CreateUserUrl='<%#ResolveUrl("~/profile/register.aspx") %>' CreateUserText="<%$resources: StoreResources, MAIN_CREATE_ACCOUNT%>"
                DestinationPageUrl='<%#ResolveUrl("~/Default.aspx") %>' RememberMeSet="True"
                FailureText="<%$resources: StoreResources, ACCOUNT_LOGIN_FAILED%>">
            </asp:Login>
            </asp:Panel>
        </td>
    </tr>
</table>
