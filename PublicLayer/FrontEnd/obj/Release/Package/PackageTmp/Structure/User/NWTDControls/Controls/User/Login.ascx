<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User.Login" %>
<div runat="server" id="divViewer"></div>
<h3>Member Login</h3>
<asp:Panel ID="Panel1" runat="server" DefaultButton="LoginForm$LoginButton">
	<asp:Login ID="LoginForm" 
		TitleText="" 
		OnAuthenticate="LoginForm_Authenticate" 
		OnLoggedIn="OnLoggedIn"  
		OnLoggingIn="OnLoggingIn"  
		PasswordRecoveryText="<%$resources: StoreResources, ACCOUNT_REGISTRATION_FORGOT_PASSWORD_LABEL%>" 
		PasswordRecoveryUrl='<%#ResolveUrl("~/Profile/PasswordRecovery.aspx") %>' 
		runat="server" 
		CreateUserUrl='<%#ResolveUrl("~/profile/register.aspx") %>' 
		CreateUserText="<%$resources: StoreResources, MAIN_CREATE_ACCOUNT%>"
		DestinationPageUrl='<%#ResolveUrl("~/Default.aspx") %>' 
		RememberMeSet="True"
		FailureText="<%$resources: StoreResources, ACCOUNT_LOGIN_FAILED%>" 
		UserNameLabelText="Username:" RememberMeText="remember me" 
		TextBoxStyle-CssClass="text" 
		CheckBoxStyle-CssClass="home1-check" 
		LoginButtonStyle-CssClass="login-btn buttons">
	</asp:Login>
	<asp:Label runat="server" ID="LoginMessage" ForeColor="Red"></asp:Label>
</asp:Panel>

