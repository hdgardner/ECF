<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.Cms.Controls.ToolbarPublicView" Codebehind="ToolbarPublicView.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cms" TagName="PageTemplate" Src="~/Structure/Base/Controls/ToolBar/PageTemplate.ascx" %>
<%@ Register TagPrefix="cms" TagName="EditorTemplate" Src="~/Structure/Base/Controls/ToolBar/EditorTemplate.ascx" %>
<%@ Register TagPrefix="cms" TagName="LangMenu" Src="~/Structure/Base/Controls/ToolBar/LanguageMenu.ascx" %>
<ComponentArt:ToolBar ID="TopToolbar" runat="server" ImagesBaseUrl="~/Structure/Base/Controls/Toolbar/Images/"
    ItemSpacing="1" DefaultItemCssClass="admin-toolbar-item" DefaultItemHoverCssClass="admin-toolbar-item-hover"
    DefaultItemCheckedCssClass="admin-toolbar-item-checked" DefaultItemImageWidth="24"
    DefaultItemImageHeight="24" DefaultItemTextImageSpacing="2" DefaultItemTextImageRelation="ImageBeforeText"
    Orientation="Horizontal">
    <Items>
        <ComponentArt:ToolBarItem Text="" ToolTip="Approve current version" ClientSideCommand="RunCommand('Approve')"
            ItemType="Command" ImageUrl="icons/approve.png" HoverImageUrl="icons/approve-on.png"
            DisabledImageUrl="icons/approve-off.png"></ComponentArt:ToolBarItem>
        <ComponentArt:ToolBarItem Text="" ToolTip="Rollback current version and return it to draft"
            ClientSideCommand="RunCommand('Deny')" ItemType="Command" ImageUrl="icons/rollback.png"
            HoverImageUrl="icons/rollback-on.png" DisabledImageUrl="icons/rollback-off.png">
        </ComponentArt:ToolBarItem>
        <ComponentArt:ToolBarItem ItemType="Separator" ImageUrl="icons/break.gif" ImageHeight="24"
            ImageWidth="2" />
        <ComponentArt:ToolBarItem HoverCssClass="" ServerTemplateId="LanguageTemplateId"
            ImageHeight="24" ImageWidth="2" />
        <ComponentArt:ToolBarItem ItemType="Separator" ImageUrl="icons/break.gif" ImageHeight="24"
            ImageWidth="2" />
        <ComponentArt:ToolBarItem HoverCssClass="" ServerTemplateId="EditorTemplateId" ImageHeight="24"
            ImageWidth="2" />
    </Items>
    <ClientTemplates>
        <ComponentArt:ClientTemplate ID="MyDescriptionTemplate">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <b>## DataItem.GetProperty('Text') ##</b></td>
                </tr>
                <tr>
                    <td>
                        ## DataItem.GetProperty('Description') ##</td>
                </tr>
            </table>
        </ComponentArt:ClientTemplate>
    </ClientTemplates>
    <ServerTemplates>
        <ComponentArt:ToolBarCustomTemplate ID="LanguageTemplateId">
            <Template>
                <table cellpadding="0" cellspacing="0">
                    <tr valign="middle">
                        <td valign="middle">
                            Language:&nbsp;
                        </td>
                        <td valign="middle">
                            <cms:LangMenu runat="server" ID="langMenu" />
                        </td>
                    </tr>
                </table>
            </Template>
        </ComponentArt:ToolBarCustomTemplate>
        <ComponentArt:ToolBarCustomTemplate ID="EditorTemplateId">
            <Template>
                <cms:EditorTemplate runat="server" />
            </Template>
        </ComponentArt:ToolBarCustomTemplate>
        <ComponentArt:ToolBarCustomTemplate ID="ToolBarCustomTemplate1">
            <Template>
            </Template>
        </ComponentArt:ToolBarCustomTemplate>
    </ServerTemplates>
</ComponentArt:ToolBar>
