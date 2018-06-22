<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PP_IndexSearchView.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.PropertyPages.PP_IndexSearchView" %>
<table width="100%">
    <tr>
        <td class="label">Display template:</td>
        <td>
            <asp:DropDownList runat="server" id="DisplayTemplate" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="[use default]"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>    
    <tr>
        <td class="label">Catalogs:</td>
        <td>
            <asp:ListBox Width="200px" Rows="2" runat="Server" id="CatalogList" SelectionMode="Multiple"></asp:ListBox>
        </td>
    </tr>    
    <tr>
        <td class="label">Node codes:</td>
        <td colspan="2">
            <asp:TextBox Width="200px" runat="server" ID="NodeCode"></asp:TextBox>
            <br />
            comma separated list
        </td>
    </tr>    
    <tr>
        <td class="label">Entry classes:</td>
        <td>
            <asp:ListBox Width="200px" runat="Server" id="MetaClassList" SelectionMode="Multiple"></asp:ListBox>
        </td>
    </tr>         
    <tr>
        <td class="label">Entry types:</td>
        <td>
            <asp:ListBox Width="200px" runat="Server" id="EntryTypeList" SelectionMode="Multiple"></asp:ListBox>
        </td>
    </tr>
    <tr>
        <td class="label">Records per page:</td>
        <td>
            <asp:TextBox Text="10" Width="50" runat="server" ID="NumberOfRecords"></asp:TextBox>
        </td>
    </tr>        
    <tr>
        <td class="label">Order by:</td>
        <td nowrap>
            <asp:DropDownList runat="server" id="OrderByList" DataTextField="FriendlyName" DataValueField="Name">
                <asp:ListItem Value="" Text="(default)"></asp:ListItem>
                <asp:ListItem Value="Name" Text="Name"></asp:ListItem>
                <asp:ListItem Value="Variation.ListPrice" Text="ListPrice"></asp:ListItem>
                <asp:ListItem Value="custom" Text="[use custom]"></asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox id="OrderBy" runat="server"></asp:TextBox>
            <br /><asp:CheckBox runat="server" ID="OrderDesc" Text="Descending" />
        </td>
    </tr>
    <tr>
		<td>Depository:</td>
		<td>
			<asp:DropDownList runat="server" ID="ddlDepository" />
		</td>
    </tr>   
</table>