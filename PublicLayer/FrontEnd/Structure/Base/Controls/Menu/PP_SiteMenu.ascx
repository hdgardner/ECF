<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PP_SiteMenu.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.Base.Controls.Menu.PP_SiteMenu" %>
<table width="100%">
    <tr>
        <td class="label" style="width: 100px;">Display template:</td>
        <td>
            <asp:DropDownList runat="server" id="DisplayTemplate" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="[use default]"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="label">Data Source:</td>
        <td>
            <asp:DropDownList runat="server" id="DataSourceList" AutoPostBack="true" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="menu" Text="Site Menu"></asp:ListItem>
                <asp:ListItem Value="catalog" Text="Site Catalog"></asp:ListItem>
            </asp:DropDownList>
            <br />
            pick where data for the menu is coming from, the current site menu is used by default
        </td>
    </tr>    
    <tr>
        <td class="label">Source Member:</td>
        <td>
            <asp:DropDownList runat="server" id="DSMemberList" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="[use default]"></asp:ListItem>
            </asp:DropDownList>
            <br />
            pick a specific item from the source selected, the first one will be used by default
        </td>
    </tr>    
    
</table>
