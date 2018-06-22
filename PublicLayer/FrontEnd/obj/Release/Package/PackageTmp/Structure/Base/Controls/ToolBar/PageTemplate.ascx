<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Controls.Toolbar.PageTemplate"
    CodeBehind="PageTemplate.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<ComponentArt:ComboBox ID="TemplateFilter" runat="server" KeyboardEnabled="false" AutoFilter="true" Visible="false"
    AutoHighlight="true" AutoComplete="true" TextBoxEnabled="false" CssClass="admin-toolbar-comboBox"
    HoverCssClass="admin-toolbar-comboBoxHover" FocusedCssClass="admin-toolbar-comboBoxHover"
    TextBoxCssClass="admin-toolbar-comboTextBox" DropDownCssClass="admin-toolbar-comboDropDown"
    ItemCssClass="admin-toolbar-comboItem" ItemHoverCssClass="admin-toolbar-comboItemHover"
    SelectedItemCssClass="admin-toolbar-comboItemHover" DropHoverImageUrl="~/Structure/Base/Controls/Toolbar/images/combobox/drop_hover.gif"
    DropImageUrl="~/Structure/Base/Controls/Toolbar/images/combobox/drop.gif" Width="160"
    DropDownHeight="297" DropDownWidth="216">
    <DropDownContent>
        <div class="MenuPage">
            <div class="Title">
                <b>Save a copy of the document</b></div>
            <div class="Contents">
                <ComponentArt:ToolBar ID="TemplateItemList" runat="server" ImagesBaseUrl="Images/OfficeMenu/SaveAs/"
                    ItemSpacing="0" Class="ToolBar" DefaultItemImageWidth="44" DefaultItemImageHeight="52"
                    Orientation="Vertical" DefaultItemCssClass="Item" DefaultItemHoverCssClass="ItemHover"
                    UseFadeEffect="false" />
            </div>
        </div>
    </DropDownContent>
</ComponentArt:ComboBox>
<table cellpadding="0" cellspacing="0">
    <tr valign="middle">
        <td valign="middle">
            Template:&nbsp;
        </td>
        <td valign="middle">
            <asp:DropDownList onpropertychange="this.style.visibility='visible';" Style="visibility: visible;"
                Width="180px" runat="server" ID="TemplateListControl" AutoPostBack="false">
            </asp:DropDownList>
        </td>
        <td valign="middle">
            &nbsp;<asp:ImageButton ID="SaveButton" AlternateText="Apply template" runat="server"
                ImageUrl="images/icons/save.png" />
        </td>
    </tr>
</table>
