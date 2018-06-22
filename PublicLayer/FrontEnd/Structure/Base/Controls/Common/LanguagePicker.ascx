<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Web.Common.LanguagePicker" Codebehind="LanguagePicker.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<componentart:menu id="LanguageMenu" orientation="Horizontal" cssclass="Toolbar-MenuTopGroup"
    defaultgroupcssclass="Language-MenuGroup" defaultitemlookid="DefaultItemLook"
    defaultgroupitemspacing="1" imagesbaseurl="~/Structure/Base/Controls/ToolBar/Images/"
    enableviewstate="false" expanddelay="100" expandonclick="true" runat="server">
    <ItemLooks>
        <ComponentArt:ItemLook LookId="TopItemLook" LeftIconWidth="18" LeftIconHeight="12" RightIconUrl="menu/arrow.gif" CssClass="Language-TopMenuItem"
            HoverCssClass="Language-TopMenuItemHover" ExpandedCssClass="Language-TopMenuItemExpanded" LabelPaddingLeft="6"
            LabelPaddingRight="5" LabelPaddingTop="2" LabelPaddingBottom="2" />
        <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="Language-MenuItem" HoverCssClass="Language-MenuItemHover"
            ExpandedCssClass="Language-MenuItemHover" LeftIconWidth="18" LeftIconHeight="12" LabelPaddingLeft="6"
            LabelPaddingRight="10" LabelPaddingTop="2" LabelPaddingBottom="2" />
        <ComponentArt:ItemLook LookId="BreakItem" CssClass="Language-MenuBreak" />
        <ComponentArt:ItemLook LookId="TopDefaultItemLook" CssClass="Language-TopMenuItem" HoverCssClass="Language-TopMenuItemHover"
            LabelPaddingLeft="6" LabelPaddingRight="5" LabelPaddingTop="2" LabelPaddingBottom="2" />
    </ItemLooks>
</componentart:menu>
