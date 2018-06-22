<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Menu_MainMenu" Codebehind="MainMenu.ascx.cs" %>
<table  cellspacing="0" cellpadding="0" border="0" height="25" runat="server" id="mcMenu" >
    <tr>
        <td><asp:Image runat="server" ID="Image1" SkinID="MainMenuLeft" /></td>
        <td>
            <asp:Menu runat="server" ID="mcMainMenu" >
                <StaticItemTemplate>
                    <asp:Image ID="imgLeft" runat="server" SkinID="MainMenuItemLeft" />
                    <%#Eval("Text") %>
                    <asp:Image ID="imgRight" runat="server" SkinID="MainMenuItemRight" />
                </StaticItemTemplate>
                <DynamicItemTemplate>
                    <asp:Image ID="imgLeft" runat="server" SkinID="SubMenuItemRight" />
                    <%#Eval("Text") %>
                </DynamicItemTemplate>
            </asp:Menu>
        </td>
        <td><asp:Image runat="server" ID="Image2" SkinID="MainMenuRight" /></td>
    </tr>
</table>
