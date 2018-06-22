<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Web.Common.SitePicker"
    CodeBehind="SitePicker.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<ComponentArt:Menu ID="SitesMenu" Orientation="Horizontal" CssClass="Toolbar-MenuTopGroup"
    DefaultGroupCssClass="Language-MenuGroup" DefaultItemLookId="DefaultItemLook"
    DefaultGroupItemSpacing="1" ImagesBaseUrl="~/Structure/Base/Controls/ToolBar/Images/"
    EnableViewState="false" ExpandDelay="100" ExpandOnClick="true" runat="server">
    <ItemLooks>
        <ComponentArt:ItemLook LookId="TopItemLook" LeftIconWidth="18" LeftIconHeight="12"
            RightIconUrl="menu/arrow.gif" CssClass="Language-TopMenuItem" HoverCssClass="Language-TopMenuItemHover"
            ExpandedCssClass="Language-TopMenuItemExpanded" LabelPaddingLeft="6" LabelPaddingRight="5"
            LabelPaddingTop="2" LabelPaddingBottom="2" />
        <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="Language-MenuItem" HoverCssClass="Language-MenuItemHover"
            ExpandedCssClass="Language-MenuItemHover" LeftIconWidth="18" LeftIconHeight="12"
            LabelPaddingLeft="6" LabelPaddingRight="10" LabelPaddingTop="2" LabelPaddingBottom="2" />
        <ComponentArt:ItemLook LookId="BreakItem" CssClass="Language-MenuBreak" />
        <ComponentArt:ItemLook LookId="TopDefaultItemLook" CssClass="Language-TopMenuItem"
            HoverCssClass="Language-TopMenuItemHover" LabelPaddingLeft="6" LabelPaddingRight="5"
            LabelPaddingTop="2" LabelPaddingBottom="2" />
    </ItemLooks>
    <ClientTemplates>
        <ComponentArt:ClientTemplate ID="DescriptionTemplate">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <b>## DataItem.GetProperty('Text') ##</b>
                    </td>
                </tr>
                <tr>
                    <td>
                        ## DataItem.GetProperty('Description') ##
                    </td>
                </tr>
            </table>
        </ComponentArt:ClientTemplate>
    </ClientTemplates>
</ComponentArt:Menu>
