<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Controls_ToolBar_LanguageMenu" Codebehind="LanguageMenu.ascx.cs" %>
<asp:Menu runat="server" ID="menuPattern" BorderWidth="0" Orientation="Horizontal" Visible="false">
    <StaticMenuItemStyle CssClass="StaticMenu" />
    <DynamicMenuStyle CssClass="DynamicMenu" />
    <DynamicItemTemplate>
        <%#
           (Eval("ImageUrl") == String.Empty) 
           ? Eval("Text") 
           : "<img border='0' src='"+Eval("ImageUrl")+"' height='16px' width='24px'>"
        %>
    </DynamicItemTemplate>
</asp:Menu>

<asp:Table runat="server" CellPadding="2" CellSpacing="0">
    <asp:TableRow runat="server" ID="trLanguageMenu"/>
</asp:Table>
