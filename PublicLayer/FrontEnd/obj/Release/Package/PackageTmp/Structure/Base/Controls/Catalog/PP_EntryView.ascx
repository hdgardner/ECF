<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Catalog_PP_EntryView" Codebehind="PP_EntryView.ascx.cs" %>
<table width="100%">
    <tr>
        <td>Catalog Name: <br />(leave empty to use dynamic parameter)</td>
        <td>
            <asp:DropDownList runat="server" id="CatalogList" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="[use default]"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>    
    <tr>
        <td>Parent Node Code: <br />(leave empty to use dynamic parameter)</td>
        <td>
            <asp:TextBox runat="server" ID="NodeCode"></asp:TextBox>
        </td>
    </tr>        
    <tr>
        <td>Entry Code: <br />(leave empty to use dynamic parameter)</td>
        <td>
            <asp:TextBox runat="server" ID="EntryCode"></asp:TextBox>
        </td>
    </tr>    
    <tr>
        <td>Display Template: <br />(override default template)</td>
        <td>
            <asp:DropDownList runat="server" id="DisplayTemplate" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="[use default]"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>