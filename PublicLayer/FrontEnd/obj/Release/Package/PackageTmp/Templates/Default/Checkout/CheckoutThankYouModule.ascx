<%@ Control Language="C#"
    Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutThankYouModule" Codebehind="CheckoutThankYouModule.ascx.cs" %>
<table cellpadding="4" cellspacing="4" style="width: 100%">
    <tr>
        <td>
            <h1>
				<%=RM.GetString("CHECKOUT_THANKS_LABEL")%>
            </h1>
        </td>
    </tr>
    <tr>
        <td style="width: 100%">
            <asp:Label ID="ErrorMessage" ForeColor="red" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <%=RM.GetString("CHECKOUT_THANKS_ORDER_NUMBER")%> # <asp:Label runat="server" ID="OrderNumber"></asp:Label><br />
            <%=RM.GetString("CHECKOUT_THANKS_TOTAL_PAYMENT")%> <asp:Label runat="server" Cssclass="ecf-price" Font-Bold="true" ID="OrderTotal"></asp:Label><br />
            <%=RM.GetString("CHECKOUT_THANKS_KEEP_ORDER_NUMBER") %>
            <%=RM.GetString("CHECKOUT_THANKS_EMAIL_CONFIRM") %>
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="RegisteredUsersMessage">
    <tr>
        <td>
            <%=RM.GetString("CHECKOUT_THANKS_CHECK_STATUS")%>
            <asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/AccountInfo.aspx")%>'>
				<%=RM.GetString("CHECKOUT_THANKS_INFO_PAGE")%>
			</asp:HyperLink> 
			<%=RM.GetString("CHECKOUT_THANKS_CLICK_ON")%>
			<asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#ResolveUrl("~/Profile/secure/OrderHistory.aspx")%>'>
				<%=RM.GetString("CHECKOUT_THANKS_ORDER_HISTORY")%>
			</asp:HyperLink>.
        </td>
    </tr>
    </asp:PlaceHolder>    
    <asp:PlaceHolder runat="server" ID="NonRegisteredUsersMessage">
    <tr>
        <td>
            <%=RM.GetString("CHECKOUT_THANKS_NONREGISTERED_MESSAGE1")%>
            <%=RM.GetString("CHECKOUT_THANKS_NONREGISTERED_MESSAGE2")%>
            <%=RM.GetString("CHECKOUT_THANKS_NONREGISTERED_MESSAGE3")%>
            <%=RM.GetString("CHECKOUT_THANKS_NONREGISTERED_MESSAGE4")%>
        </td>
    </tr>
    <tr>
        <td>
			<asp:HyperLink runat="server" ID="RegisterLink" NavigateUrl='<%#ResolveUrl("~/Profile/Register.aspx")%>'>
				<%=RM.GetString("CHECKOUT_THANKS_REGISTER_NEW")%>
			</asp:HyperLink>
        </td>
    </tr>
    <tr>
        <td>
			<asp:HyperLink runat="server" ID="LoginLink"  NavigateUrl='<%#ResolveUrl("~/Login.aspx")%>'>
				<%=RM.GetString("CHECKOUT_THANKS_LOGIN")%>
			</asp:HyperLink>
        </td>
    </tr>    
    </asp:PlaceHolder>
    <tr>
        <td>
			<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#ResolveUrl("~/default.aspx")%>'>
				<%=RM.GetString("CHECKOUT_THANKS_TO_HOME")%>
			</asp:HyperLink>
        </td>
    </tr>
</table>