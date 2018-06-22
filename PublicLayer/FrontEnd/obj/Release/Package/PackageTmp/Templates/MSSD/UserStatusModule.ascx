<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserStatusModule.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.MSSD.UserStatusModule" %>
<asp:UpdatePanel runat="server" ID="upShoppingCart" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnShoppingCartUpdate" EventName="Click" />
    </Triggers>
    <ContentTemplate>
		<asp:Button ID="btnShoppingCartUpdate" runat="server" OnClick="btnShoppingCartUpdate_Click" style="display:none;"/>
		<asp:LoginView ID="LoginView1" runat="server" EnableViewState="false">
			<AnonymousTemplate>
				<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#ResolveUrl("~/login.aspx")%>'><%=RM.GetString("MAIN_ACCOUNT_LOGIN")%></asp:HyperLink>
			</AnonymousTemplate>
			<LoggedInTemplate>
				<div class="welcome-txt">
					<%=RM.GetString("GENERAL_WELCOME")%> <asp:Literal runat="server" ID="litFirstName" />
				</div> 
					<asp:HyperLink ID="HyperLink2" EnableViewState="false" runat="server" CssClass="my-account" NavigateUrl='<%#ResolveUrl("~/Profile/secure/ChangeEmailPassword.aspx")%>'>My Account</asp:HyperLink> <asp:HyperLink ID="HyperLink1" runat="server" CssClass="logout-btn" NavigateUrl='<%#ResolveUrl("~/logout.aspx")%>'><%=RM.GetString("MAIN_LOGOUT")%></asp:HyperLink><br />
				<div class="need-help-bottom">
				    <div class="welcome-txt">Active Wish List: </div><%--<asp:HyperLink EnableViewState="false" runat="server" NavigateUrl='<%#ResolveUrl("~/Cart/Manage.aspx")%>'>Your Shopping Carts</asp:HyperLink> | --%>
				    <asp:HyperLink 
						EnableViewState="false" 
						ID="ShoppingCartLink" 
						Runat="server" 
						NavigateUrl='<%#NavigationManager.GetUrl("ViewCart")%>'>
					</asp:HyperLink>&nbsp;&nbsp;
					<asp:HyperLink ID="LnkManageCarts" runat="server" CssClass="manage-carts" NavigateUrl='<%#ResolveUrl("~/Cart/Manage.aspx") %>'>My Wish Lists</asp:HyperLink>
				</div> 
		    </LoggedInTemplate>
		</asp:LoginView>
     </ContentTemplate>
</asp:UpdatePanel>
