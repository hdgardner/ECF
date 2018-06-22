<%@ Control Language="C#" Inherits="Mediachase.Cms.Templates.Default.UserStatusModule" Codebehind="UserStatusModule.ascx.cs" EnableViewState="false"%>
<span style="white-space:nowrap">
<asp:UpdatePanel runat="server" ID="upShoppingCart" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnShoppingCartUpdate" EventName="Click" />
    </Triggers>
    <ContentTemplate>
    <asp:Button ID="btnShoppingCartUpdate" runat="server" OnClick="btnShoppingCartUpdate_Click" style="display:none;"/>
<asp:LoginView ID="LoginView1" runat="server" EnableViewState="false">
    <AnonymousTemplate>
        <asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/login.aspx")%>'><%=RM.GetString("MAIN_ACCOUNT_LOGIN")%></asp:HyperLink> | <asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/AccountInfo.aspx")%>'><%=RM.GetString("MAIN_ACCOUNT_INFO")%></asp:HyperLink>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <%=RM.GetString("GENERAL_WELCOME")%> <%=Page.User.Identity.Name%> [<asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/logout.aspx")%>'><%=RM.GetString("MAIN_LOGOUT")%></asp:HyperLink>] |
        <asp:HyperLink EnableViewState="false" runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/AccountInfo.aspx")%>'><%=RM.GetString("MAIN_ACCOUNT_INFO")%></asp:HyperLink>   
    </LoggedInTemplate>
</asp:LoginView>
| <asp:HyperLink EnableViewState="false" ID="ShoppingCartLink" Runat="server" NavigateUrl='<%#ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("ShoppingCart"))%>'></asp:HyperLink>
| <asp:HyperLink EnableViewState="false" ID="HyperLink1" Runat="server" NavigateUrl='<%#ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("WishList"))%>'>Wish List</asp:HyperLink>
     </ContentTemplate>
</asp:UpdatePanel>
</span>